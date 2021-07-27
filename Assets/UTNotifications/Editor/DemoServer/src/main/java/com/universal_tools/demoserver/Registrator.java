package com.universal_tools.demoserver;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.Serializable;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Date;

/// <summary>
/// Very simple registration of devices on a server side (storing pairs of push notifications system provider & registrationId).
/// </summary>
/// <remarks>
/// You should use some database instead in production.
/// </remarks>
@SuppressWarnings("unchecked")
public class Registrator {
//public
	public static class Item implements Serializable
	{
	//public
		public Item(String provider, String id) {
			this.provider = provider;
			m_id = id;
		}
		
		public String getId() {
			return m_id;
		}
		
		public void setId(String id) {
			m_id = id;
			
			save();
		}
		
		public final String provider;
		
	//private
		private String		m_id;
		
		private static final long serialVersionUID = 1L;
	}
	
	public static void register(String uid, String provider, String id) {
		synchronized (m_registration) {
			m_registration.put(uid, new Item(provider, id));
			save();
		}
	}
	
	public static List<Item> items()
	{
		synchronized (m_registration) {
			return new ArrayList<Item>(m_registration.values());
		}
	}
	
	public static String getOAuth2Token(String provider) {
		if (m_oath2Tokens.containsKey(provider)) {
			return m_oath2Tokens.get(provider).getToken();
		} else {
			return null;
		}
	}
	
	public static void setOAuth2Token(String provider, String token, Date tokenExpires) {
		m_oath2Tokens.put(provider, new OAuth2Token(token, tokenExpires));
		save();
	}
	
//private
	static {
		ObjectInputStream stream = null;
		HashMap<String, Item> registration = null;
		HashMap<String, Registrator.OAuth2Token> oath2Tokens = null;
		
		try {
			FileInputStream fileStream = new FileInputStream(DB_FILE_NAME = "utnotifications_reg.db");
			stream = new ObjectInputStream(fileStream);
			
			registration = (HashMap<String, Item>)stream.readObject();
			oath2Tokens = (HashMap<String, Registrator.OAuth2Token>)stream.readObject();
		} catch (FileNotFoundException e) {
			//It's OK!
		} catch (Throwable t) {
			t.printStackTrace();
		} finally {
			m_registration = registration != null ? registration : new HashMap<String, Item>();
			m_oath2Tokens = oath2Tokens != null ? oath2Tokens : new HashMap<String, OAuth2Token>();
			
			if (stream != null) {
				try {
					stream.close();
				} catch (Throwable t) {
					t.printStackTrace();
				}
			}
		}
	}
	
	private static void save() {
		ObjectOutputStream stream = null;
		try {
			FileOutputStream fileStream = new FileOutputStream(DB_FILE_NAME);
			stream = new ObjectOutputStream(fileStream);
			
			stream.writeObject(m_registration);
			stream.writeObject(m_oath2Tokens);
		} catch (Throwable t) {
			t.printStackTrace();
		} finally {
			if (stream != null) {
				try {
					stream.close();
				} catch (Throwable t) {
					t.printStackTrace();
				}
			}
		}
	}
	
	private static class OAuth2Token implements Serializable {
		public final String token;
		public final Date tokenExpires;
		
		public OAuth2Token(String token, Date tokenExpires) {
			this.token = token;
			this.tokenExpires = tokenExpires;
		}
		
		public String getToken() {
			if (tokenExpires != null && tokenExpires.after(new Date())) {
				return token;
			} else {
				return null;
			}
		}
		
		private static final long serialVersionUID = 1L;
	}
	
	private static String DB_FILE_NAME;
	private static HashMap<String, Item> m_registration;
	private static HashMap<String, OAuth2Token> m_oath2Tokens;
}
