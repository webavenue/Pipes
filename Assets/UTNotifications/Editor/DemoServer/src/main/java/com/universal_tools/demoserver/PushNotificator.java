package com.universal_tools.demoserver;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.URL;
import java.util.Date;
import java.util.LinkedList;
import java.util.List;

import javax.net.ssl.HttpsURLConnection;

import org.json.*;

import com.clevertap.apns.ApnsClient;
import com.clevertap.apns.Notification;
import com.clevertap.apns.NotificationResponse;
import com.clevertap.apns.clients.ApnsClientBuilder;

/// <summary>
/// The sample class showing how you can send push notifications for different "providers", such as APNS, FCM, ADM and WNS.
/// </summary>
public class PushNotificator {
// private
	// Please provide the required values. Find more details in the manual: Assets/UTNotifications/Documentation/Manual.pdf
	private static final String FIREBASE_SERVER_KEY = null;
	private static final String AMAZON_CLIENT_ID = null;
	private static final String AMAZON_CLIENT_SECRET = null;
	private static final String APNS_AUTH_KEY = null;
	private static final String APNS_TEAM_ID = null;
	private static final String APNS_KEY_ID = null;
	private static final String APNS_BUNDLE_ID = null;
	private static final boolean APNS_DEVELOPMENT = true;
	private static final String WINDOWS_PACKAGE_SID = null;
	private static final String WINDOWS_CLIENT_SECRET = null;

// public
	/// <summary>
	/// Sends a push notification to every registered device.
	/// </summary>
	public static int notifyAll(int id, String title, String text, String serverMessage, String notificationProfile, int badge) throws Throwable {
		return notifyItems(id, Registrator.items(), title, text, serverMessage, notificationProfile, badge);
	}

	/// <summary>
	/// Sends a push notification to every registered device.
	/// </summary>
	public static boolean notifyTopic(String topic, int id, String title, String text, String serverMessage, String notificationProfile, int badge) throws Throwable {
		return notifyFCM(id, topic, title, text, serverMessage, notificationProfile, badge);
	}

	/// <summary>
	/// Sends a push notification to every device in <c>items</c> list.
	/// </summary>
	public static int notifyItems(int id, List<Registrator.Item> items, String title, String text, String serverMessage, String notificationProfile, int badge) throws Throwable {
		int notified = 0;

		LinkedList<Registrator.Item> fcmItems = new LinkedList<Registrator.Item>();
		LinkedList<Registrator.Item> admItems = new LinkedList<Registrator.Item>();
		LinkedList<Registrator.Item> apnsItems = new LinkedList<Registrator.Item>();
		LinkedList<Registrator.Item> wnsItems = new LinkedList<Registrator.Item>();

		for (Registrator.Item item : items) {
			if ("FCM".equals(item.provider) || "GooglePlay".equals(item.provider)) {
				fcmItems.add(item);
			} else if ("ADM".equals(item.provider) || "Amazon".equals(item.provider)) {
				admItems.add(item);
			} else if ("APNS".equals(item.provider) || "iOS".equals(item.provider)) {
				apnsItems.add(item);
			} else if ("WNS".equals(item.provider) || "Windows".equals(item.provider)) {
				wnsItems.add(item);
			}
		}

		notified += notifyFCM(id, fcmItems, title, text, serverMessage, notificationProfile, badge);
		notified += notifyADM(id, admItems, title, text, serverMessage, notificationProfile, badge);
		notified += notifyAPNS(id, apnsItems, title, text, serverMessage, notificationProfile, badge);
		notified += notifyWNS(id, wnsItems, title, text, serverMessage, notificationProfile, badge);

		return notified;
	}

	/// <summary>
	/// Sends a push notification to Google Play featured Android devices, which are subscribed to the specified topic.
	/// </summary>
	/// <remarks>
	/// See also:
	/// https://firebase.google.com/docs/cloud-messaging/android/topic-messaging
	/// </remarks>
	public static boolean notifyFCM(int id, final String topic, String title, String text, String serverMessage, String notificationProfile, int badge) throws Throwable {
		return notifyFCM(id, "to", "/topics/" + topic, title, text, serverMessage, notificationProfile, badge);
	}

	/// <summary>
	/// Sends a push notification to Google Play featured Android devices.
	/// </summary>
	/// <remarks>
	/// See also:
	/// https://firebase.google.com/docs/cloud-messaging/http-server-ref#downstream
	/// </remarks>
	public static int notifyFCM(int id, List<Registrator.Item> items, String title, String text, String serverMessage, String notificationProfile, int badge) throws Throwable {
		if (items == null || items.size() == 0) {
			return 0;
		} else {
			String addresseeKey;
			Object addresseeValue;

			// Multiple ids are sent in "registration_ids" array, single one in "to" string field
			if (items.size() > 1) {
				JSONArray registrationIds = new JSONArray();
				for (Registrator.Item it : items) {
					registrationIds.put(it.getId());
				}
				addresseeKey = "registration_ids";
				addresseeValue = registrationIds;
			} else {
				addresseeKey = "to";
				addresseeValue = items.get(0).getId();
			}

			return notifyFCM(id, addresseeKey, addresseeValue, title, text, serverMessage, notificationProfile, badge)
				? items.size()
				: 0;
		}
	}

	/// <summary>
	/// Sends a push notification to Google Play featured Android devices.
	/// addresseeKey can be one of: "registration_ids", "to" or "topic" with an appropriate value of addresseeValue.
	/// </summary>
	/// <remarks>
	/// See also:
	/// https://firebase.google.com/docs/cloud-messaging/http-server-ref#downstream
	/// </remarks>
	public static boolean notifyFCM(int id, final String addresseeKey, final Object addresseeValue, String title, String text, String serverMessage, String notificationProfile, int badge) throws Throwable {
		if (FIREBASE_SERVER_KEY == null || addresseeKey == null || addresseeValue == null) {
			return false;
		}

		// Request data json by default should look like:
		/*
		 * { "registration_ids":["<id1>", ...], <or "to":"id1", / or "to":"/topic/topic1",>
		 * "data": { "title":"<Title>", "text":"<Text>", "id":<int id>, "badge_number":<int
		 * badge>, "buttons": "[ { \"title\":\"<Button title>\",
		 * \"<Button user data key 1>\":\"<Button user data value 1>\", ... }, ... ]",
		 * "<User data key 1>":"<User data value 1>", ... } }
		 */
		JSONObject requestData = new JSONObject();

		requestData.put(addresseeKey, addresseeValue);
		requestData.put("data", prepareData(id, title, text, serverMessage, notificationProfile, badge));

		final byte[] requestDataBytes = requestData.toString().getBytes("UTF-8");

		final String httpsURL = "https://fcm.googleapis.com/fcm/send";

		final URL url = new URL(httpsURL);
		final HttpsURLConnection connection = (HttpsURLConnection) url.openConnection();
		connection.setRequestMethod("POST");
		connection.setRequestProperty("Content-length", String.valueOf(requestDataBytes.length));
		connection.setRequestProperty("Content-Type", "application/json");
		connection.setRequestProperty("Charset", "UTF-8");
		connection.setRequestProperty("Authorization", "key=" + FIREBASE_SERVER_KEY);
		connection.setDoOutput(true);
		connection.setDoInput(true);

		final DataOutputStream output = new DataOutputStream(connection.getOutputStream());
		output.write(requestDataBytes);
		output.close();

		final int responseCode = connection.getResponseCode();
		if (responseCode != 200) {
			final String errorContent = readResponse(connection.getErrorStream());
			throw new RuntimeException(
					String.format("ERROR: The request failed with response code: %d and message: %s",
							responseCode, errorContent));
		} else {
			System.out.println("    FCM response:");
			System.out.println("    " + readResponse(connection.getInputStream()));
		}

		return true;
	}

	/// <summary>
	/// Sends a push notification to Amazon Android devices.
	/// </summary>
	/// <remarks>
	/// See also:
	/// https://developer.amazon.com/public/apis/engage/device-messaging/tech-docs/06-sending-a-message
	/// </remarks>
	public static int notifyADM(int id, List<Registrator.Item> items, String title, String text, String serverMessage, String notificationProfile, int badge) throws Throwable {
		if (items == null || items.size() == 0 || AMAZON_CLIENT_ID == null || AMAZON_CLIENT_SECRET == null) {
			return 0;
		}

		String token = oauth2GetAuthToken("Amazon", "https://api.amazon.com/auth/O2/token", "messaging:push",
				AMAZON_CLIENT_ID, AMAZON_CLIENT_SECRET, false);

		boolean tokenUpdated = false;
		int notifiedCount = 0;
		for (Registrator.Item it : items) {
			try {
				String regId = amazonSendMessageToDevice(it.getId(), token, id, title, text, serverMessage,
						notificationProfile, badge);

				if (!tokenUpdated && TOKEN_EXPIRED.equals(regId)) {
					token = oauth2GetAuthToken("Amazon", "https://api.amazon.com/auth/O2/token", "messaging:push",
							AMAZON_CLIENT_ID, AMAZON_CLIENT_SECRET, true);
					tokenUpdated = true;

					regId = amazonSendMessageToDevice(it.getId(), token, id, title, text, serverMessage,
							notificationProfile, badge);
				}

				++notifiedCount;
				if (regId != null) {
					it.setId(regId);
				}
			} catch (Exception e) {
				e.printStackTrace();
			}
		}

		return notifiedCount;
	}

	/* APNS implementation uses https://github.com/CleverTap/apns-http2
	 * 
	 * Copyright (c) 2016, CleverTap All rights reserved.
	 * 
	 * Redistribution and use in source and binary forms, with or without
	 * modification, are permitted provided that the following conditions are met:
	 * 
	 * Redistributions of source code must retain the above copyright notice, this
	 * list of conditions and the following disclaimer.
	 * 
	 * Redistributions in binary form must reproduce the above copyright notice,
	 * this list of conditions and the following disclaimer in the documentation
	 * and/or other materials provided with the distribution.
	 * 
	 * Neither the name of CleverTap nor the names of its contributors may be used
	 * to endorse or promote products derived from this software without specific
	 * prior written permission.
	 * 
	 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
	 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
	 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
	 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
	 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
	 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
	 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
	 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
	 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
	 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
	 * POSSIBILITY OF SUCH DAMAGE.
	 */
	
	/// <summary>
	/// Sends a push notification to iOS devices. com.notnoop.apns library is used
	/// (its source code is provided).
	/// </summary>
	/// <remarks>
	/// Note that iOS Registration Ids are considered as HEX- or Base64- encoded
	/// APNS tokens (which are originally binary buffers).
	/// <seealso cref="UTNotifications.Manager.OnSendRegistrationId"/>
	/// See also:
	/// https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/CommunicatingwithAPNs.html.
	/// </remarks>
	public static int notifyAPNS(int id, List<Registrator.Item> items, String title, String text, String serverMessage, String notificationProfile, int badge) throws Throwable {
		if (items == null || items.size() == 0 || APNS_AUTH_KEY == null || APNS_TEAM_ID == null || APNS_KEY_ID == null || APNS_BUNDLE_ID == null) {
			return 0;
		}

		if (apnsClient == null) {
			apnsClient = new ApnsClientBuilder()
					.inSynchronousMode()
					.withProductionGateway(!APNS_DEVELOPMENT)
					.withApnsAuthKey(APNS_AUTH_KEY)
					.withTeamID(APNS_TEAM_ID)
					.withKeyID(APNS_KEY_ID)
					.withDefaultTopic(APNS_BUNDLE_ID)
					.build();
		}
		
		int notifiedCount = 0;
		for (Registrator.Item item : items) {
			String sound;
			if (notificationProfile != null && !notificationProfile.isEmpty()) {
				sound = "Data/Raw/" + notificationProfile;
			} else {
				sound = "default";
			}
			
			Notification.Builder builder = new Notification.Builder(item.getId())
					.alertTitle(title)
					.alertBody(text)
					.sound(sound)
					.customField("server_message", serverMessage);
			
			if (id >= 0) {
				builder.customField("id", new Integer(id).toString());
			}
			
			if (badge >= 0) {
				builder.badge(badge);
			}
			
			NotificationResponse result = apnsClient.push(builder.build());
			if (result.getHttpStatusCode() != 200) {
				System.err.println("Error pushing to APNS: " + result);
			} else {
				++notifiedCount;
			}
		}
		
		return notifiedCount;
	}

	/// <summary>
	/// Sends a push notification to Windows/Windows Phone devices.
	/// </summary>
	/// <remarks>
	/// Note that in order to correctly support Unicode characters, <c>title</c>,
	/// <c>text</c> and any other text values should be URL-encoded:
	/// See also:
	/// https://msdn.microsoft.com/en-us/library/windows/apps/hh465435.aspx
	/// </remarks>
	public static int notifyWNS(int id, List<Registrator.Item> items, String title, String text, String serverMessage, String notificationProfile, int badge) throws Throwable {
		if (items == null || items.size() == 0) {
			return 0;
		}

		String token = oauth2GetAuthToken("Windows", "https://login.live.com/accesstoken.srf", "notify.windows.com",
				WINDOWS_PACKAGE_SID, WINDOWS_CLIENT_SECRET, false);

		boolean tokenUpdated = false;
		int notifiedCount = 0;
		for (Registrator.Item it : items) {
			try {
				String regId = notifyWindows(token, it.getId(), id, title, text, serverMessage, notificationProfile,
						badge);

				if (!tokenUpdated && TOKEN_EXPIRED.equals(regId)) {
					token = oauth2GetAuthToken("Windows", "https://login.live.com/accesstoken.srf",
							"notify.windows.com", WINDOWS_PACKAGE_SID, WINDOWS_CLIENT_SECRET, true);
					tokenUpdated = true;

					regId = notifyWindows(token, it.getId(), id, title, text, serverMessage, notificationProfile,
							badge);
				}

				++notifiedCount;
				if (regId != null) {
					it.setId(regId);
				}
			} catch (Exception e) {
				e.printStackTrace();
			}
		}

		return notifiedCount;
	}

// private
	// Returns updated registrationId if changed, TOKEN_EXPIRED if expired and null otherwise
	private static String amazonSendMessageToDevice(String registrationId, String accessToken, int id, String title, String text, String serverMessage, String notificationProfile, int badge) throws Exception {
		// JSON payload representation of the message.
		JSONObject payload = new JSONObject();

		// Define the key/value pairs for your message content and add them to the
		// message payload.
		payload.put("data", prepareData(id, title, text, serverMessage, notificationProfile, badge));

		// Convert the message from a JSON object to a string.
		System.out.println(payload.toString());
		byte[] payloadBytes = payload.toString().getBytes("UTF-8");

		// Establish the base URL, including the section to be replaced by the
		// registration
		// ID for the desired app instance. Because we are using String.format to create
		// the URL, the %1$s characters specify the section to be replaced.
		String admUrlTemplate = "https://api.amazon.com/messaging/registrations/%1$s/messages";

		URL admUrl = new URL(String.format(admUrlTemplate, registrationId));

		// Generate the HTTPS connection for the POST request. You cannot make a
		// connection
		// over HTTP.
		HttpsURLConnection connection = (HttpsURLConnection) admUrl.openConnection();
		connection.setRequestMethod("POST");
		connection.setDoOutput(true);

		// Set the content type and accept headers.
		connection.setRequestProperty("Content-Type", "application/json");
		connection.setRequestProperty("Charset", "UTF-8");
		connection.setRequestProperty("Accept", "application/json");
		connection.setRequestProperty("Content-length", String.valueOf(payloadBytes.length));
		connection.setRequestProperty("X-Amzn-Type-Version ", "com.amazon.device.messaging.ADMMessage@1.0");
		connection.setRequestProperty("X-Amzn-Accept-Type", "com.amazon.device.messaging.ADMSendResult@1.0");

		// Add the authorization token as a header.
		connection.setRequestProperty("Authorization", "Bearer " + accessToken);

		// Obtain the output stream for the connection and write the message payload to
		// it.
		OutputStream os = connection.getOutputStream();
		os.write(payloadBytes);
		os.flush();
		connection.connect();

		// Obtain the response code from the connection.
		int responseCode = connection.getResponseCode();

		// Check if we received a failure response, and if so, get the reason for the
		// failure.
		if (responseCode != 200) {
			if (responseCode == 401) {
				// If a 401 response code was received, the access token has expired. The token
				// should be refreshed
				// and this request may be retried.
				return TOKEN_EXPIRED;
			}

			String errorContent = readResponse(connection.getErrorStream());
			throw new RuntimeException(
					String.format("ERROR: The request failed with a %d response code, with the following message: %s",
							responseCode, errorContent));
		} else {
			// The request was successful. The response contains the canonical Registration
			// ID for the specific instance of your
			// app, which may be different that the one used for the request.

			String responseContent = readResponse(connection.getInputStream());

			System.out.println("    ADM response:");
			System.out.println("    " + responseContent);

			JSONObject parsedObject = new JSONObject(responseContent);

			String canonicalRegistrationId = parsedObject.getString("registrationID");

			// Check if the two Registration IDs are different.
			if (!canonicalRegistrationId.equals(registrationId)) {
				// At this point the data structure that stores the Registration ID values
				// should be updated
				// with the correct Registration ID for this particular app instance.
				return canonicalRegistrationId;
			}
		}

		return null;
	}

	// Returns updated registrationId if changed (never happens in the current
	// version of WNS though), TOKEN_EXPIRED if expired and null otherwise
	private static String notifyWindows(String accessToken, String registrationId, int id, String title, String text, String serverMessage, String notificationProfile, int badge) throws Throwable {
		URL url = new URL(registrationId);

		if (!url.getHost().endsWith(".notify.windows.com")) {
			throw new SecurityException("Unexpected WNS channel URI: " + registrationId);
		}

		byte[] requestDataBytes = prepareData(id, title, text, serverMessage, notificationProfile, badge).toString()
				.getBytes("UTF-8");

		HttpsURLConnection connection = (HttpsURLConnection) url.openConnection();
		connection.setRequestMethod("POST");
		connection.setRequestProperty("Content-Type", "application/octet-stream");
		connection.setRequestProperty("Charset", "UTF-8");
		connection.setRequestProperty("Authorization", "Bearer " + accessToken);
		connection.setRequestProperty("Content-length", String.valueOf(requestDataBytes.length));
		connection.setRequestProperty("X-WNS-Type", "wns/raw");
		connection.setRequestProperty("X-WNS-Cache-Policy", "cache");
		connection.setDoOutput(true);
		connection.setDoInput(true);

		DataOutputStream output = new DataOutputStream(connection.getOutputStream());
		output.write(requestDataBytes);
		output.close();

		int responseCode = connection.getResponseCode();
		if (responseCode != 200) {
			if (responseCode == 401 || responseCode == 410) {
				// If a 401 response code was received, the access token has expired. The token
				// should be refreshed
				// and this request may be retried.
				return TOKEN_EXPIRED;
			}

			String errorContent;
			try {
				errorContent = readResponse(connection.getErrorStream() != null ? connection.getErrorStream()
						: connection.getInputStream());
			} catch (final Throwable e) {
				errorContent = "Unable to read the response content: " + e.toString();
			}

			throw new RuntimeException(
					String.format("ERROR: The request failed with a %d response code, with the following message: %s",
							responseCode, errorContent));
		} else {
			// Success! Current version of WNS never sends the changed registrationId, so no
			// need to check a response content.
			System.out.println("    WNS response:");
			System.out.println("    " + readResponse(connection.getInputStream()));

			return null;
		}
	}

	private static JSONObject prepareData(int id, String title, String text, String serverMessage, String notificationProfile, int badge) throws JSONException {
		/*
		 * "data": { "title":"<Title>", "text":"<Text>", "id":<int id>,
		 * "badge_number":<int badge>, "buttons": "[ { \"title\":\"<Button title>\",
		 * \"<Button user data key 1>\":\"<Button user data value 1>\", ... }, ... ]",
		 * "<User data key 1>":"<User data value 1>", ... }
		 */

		JSONObject data = new JSONObject();
		if (id >= 0) {
			data.put("id", new Integer(id).toString());
		}
		data.put("title", title);
		data.put("text", text);
		data.put("server_message", serverMessage);
		if (notificationProfile != null && !notificationProfile.isEmpty()) {
			data.put("notification_profile", notificationProfile);
		}
		if (badge >= 0) {
			data.put("badge_number", new Integer(badge).toString());
		}

		// Buttons example
		JSONArray buttons = new JSONArray();
		for (int i = 0; i < 2; ++i) {
			JSONObject button = new JSONObject();
			button.put("title", "Demo Button " + i);
			button.put("example_button_userdata_value", "" + i);
			buttons.put(button);
		}
		data.put("buttons", buttons.toString());

		return data;
	}

	private static String oauth2GetAuthToken(String provider, String url, String scope, String clientId, String clientSecret, boolean forceUpdateAuthToken) throws Exception {
		if (!forceUpdateAuthToken) {
			String token = Registrator.getOAuth2Token(provider);
			if (token != null) {
				return token;
			}
		}

		// Encode the body of your request, including your clientID and clientSecret
		// values.
		String body = "grant_type=" + java.net.URLEncoder.encode("client_credentials", "UTF-8") + "&" + "scope="
				+ java.net.URLEncoder.encode(scope, "UTF-8") + "&" + "client_id="
				+ java.net.URLEncoder.encode(clientId, "UTF-8") + "&" + "client_secret="
				+ java.net.URLEncoder.encode(clientSecret, "UTF-8");

		// Create a new URL object with the base URL for the access token request.
		URL authUrl = new URL(url);

		// Generate the HTTPS connection. You cannot make a connection over HTTP.
		HttpsURLConnection con = (HttpsURLConnection) authUrl.openConnection();
		con.setDoOutput(true);
		con.setRequestMethod("POST");

		// Set the Content-Type header.
		con.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");
		con.setRequestProperty("Charset", "UTF-8");

		// Send the encoded parameters on the connection.
		OutputStream os = con.getOutputStream();
		os.write(body.getBytes("UTF-8"));
		os.flush();
		con.connect();

		// Convert the response into a String object.
		String responseContent = readResponse(con.getInputStream());

		// Create a new JSONObject to hold the access token and extract
		// the token from the response.
		JSONObject parsedObject = new JSONObject(responseContent);
		String accessToken = parsedObject.getString("access_token");
		int expiresIn = parsedObject.has("expires_in") ? parsedObject.getInt("expires_in") : (Integer.MAX_VALUE / 1000);
		expiresIn = Math.min(10, expiresIn - 10); // It took some time to deliver the response

		Date tokenExpires = new Date();
		tokenExpires.setTime(tokenExpires.getTime() + expiresIn * 1000);

		Registrator.setOAuth2Token(provider, accessToken, tokenExpires);

		return accessToken;
	}

	private static String readResponse(InputStream in) throws Exception {
		InputStreamReader inputStream = new InputStreamReader(in, "UTF-8");
		BufferedReader buff = new BufferedReader(inputStream);

		StringBuilder sb = new StringBuilder();
		String line = buff.readLine();
		while (line != null) {
			sb.append(line);
			line = buff.readLine();
		}

		buff.close();

		return sb.toString();
	}

	private static final String TOKEN_EXPIRED = "TOKEN_EXPIRED";
	private static ApnsClient apnsClient;
}
