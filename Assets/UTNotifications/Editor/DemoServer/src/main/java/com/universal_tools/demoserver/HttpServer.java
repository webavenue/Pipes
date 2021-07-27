package com.universal_tools.demoserver;

import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.HashMap;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.InputStreamReader;
import java.io.BufferedReader;
import java.io.UnsupportedEncodingException;
import java.text.SimpleDateFormat;
import java.util.Date;

/// <summary>
/// Very simple HTTP server that manages devices registration and push notifications requests.
/// </summary>
/// <remarks>
/// You will use your game or specific notifications server instead in production.
/// </remarks>
public class HttpServer {
// public
	public static int PORT = 8080;

	public static void main(String[] args) throws Throwable {
		ServerSocket serverSocket = new ServerSocket(PORT);
		System.out.println(
				"The demo server is running as http://" + InetAddress.getLocalHost().getHostAddress() + ":" + PORT);
		try {
			while (true) {
				Socket socket = serverSocket.accept();
				new Thread(new SocketProcessor(socket)).start();
			}
		} finally {
			serverSocket.close();
		}
	}

// private
	private static class SocketProcessor implements Runnable {
		private SocketProcessor(Socket socket) throws Throwable {
			this.socket = socket;
			this.is = socket.getInputStream();
			this.os = socket.getOutputStream();
		}

		public void run() {
			try {
				read();
				process();
			} catch (Throwable t) {
				t.printStackTrace();
				try {
					writeResponse("500 ERROR", t.toString());
				} catch (Throwable tt) {
					tt.printStackTrace();
				}
			} finally {
				try {
					socket.close();
				} catch (Throwable t) {
					t.printStackTrace();
				}
				socket = null;
			}
		}

		private void writeResponse(String status, String content) throws Throwable {
			if (content == null) {
				content = "EMPTY";
			}

			String response = "HTTP/1.1 " + status + "\r\n" + "Server: UTNotificationsDemoServer\r\n"
					+ "Content-Type: text/html\r\n" + "Content-Length: " + content.length() + "\r\n"
					+ "Connection: close\r\n\r\n";
			String result = response + content;
			os.write(result.getBytes());
			os.flush();

			System.out.println("  >> " + result.replace("\n", "\n  >> "));
		}

		private void read() throws Throwable {
			BufferedReader br = new BufferedReader(new InputStreamReader(is));

			String requestString = br.readLine();
			if (requestString == null || !requestString.contains(" ")) {
				System.out.println("Unexpected request: " + requestString);
				writeResponse("500 ERROR", "Unexpected request: " + requestString);
				return;
			}

			request = requestString.split("\\ ")[1];

			int contentLength = 0;
			boolean chunked = false;

			while (true) {
				String s = br.readLine();

				if (s == null || s.trim().length() == 0) {
					break;
				} else if (s.toLowerCase().startsWith("content-length:")) {
					contentLength = Integer.parseInt(s.substring(15).trim());
				} else if (s.toLowerCase().startsWith("transfer-encoding:")) {
					chunked = (s.substring(18).trim().toLowerCase().equals("chunked"));
				}
			}

			if (chunked) {
				// See https://en.wikipedia.org/wiki/Chunked_transfer_encoding. Important due to changes in Unity 2017.3+
				StringBuilder contentBuilder = new StringBuilder();

				int chunkLength = 0;
				while (true) {
					String s = br.readLine();

					if (s == null || s.trim().length() == 0) {
						break;
					} else {
						if (chunkLength <= 0) {
							String chunkLengthStr = s.trim();
							final int indexOfSemicolon = s.indexOf(';');
							if (indexOfSemicolon > 0) {
								chunkLengthStr = chunkLengthStr.substring(0, indexOfSemicolon);
							}
							chunkLength = Integer.parseInt(chunkLengthStr, 16);
							if (chunkLength == 0) {
								break;
							}
						} else {
							// Check for last line
							if (s.length() == chunkLength + 2) {
								if (s.charAt(chunkLength) != '\r' || s.charAt(chunkLength + 1) != '\n') {
									throw new RuntimeException("Invalid chunk format");
								}
								s = s.substring(0, chunkLength);
							}
							contentBuilder.append(s);
							chunkLength -= s.length();
							
							if (chunkLength < 0) {
								throw new RuntimeException("Invalid chunk size");
							}
						}
					}
				}

				content = contentBuilder.toString();
			} else if (contentLength > 0) {
				char[] buff = new char[contentLength];
				int offset = 0;
				while (contentLength > 0) {
					int read = br.read(buff, offset, contentLength);
					if (read < 0) {
						break;
					}

					offset += read;
					contentLength -= read;
				}

				content = new String(buff);
			}

			if (request.contains("?")) {
				String[] pair = request.split("\\?");
				request = pair[0];

				if (pair[1] != null && !pair[1].isEmpty()) {
					if (content != null) {
						content += "&" + pair[1];
					} else {
						content = pair[1];
					}
				}
			}
		}

		private void process() throws Throwable {
			if (request == null) {
				return;
			}

			System.out.println("\n<< " + request + " " + content);

			switch (request) {
			case "/register": {
				HashMap<String, String> argsMap = conentAsArgumentsMap();
				
				if (argsMap.get("uid") == null) {
					throw new IllegalArgumentException("uid is not specified!");
				}
				
				if (argsMap.get("provider") == null) {
					throw new IllegalArgumentException("provider is not specified!");
				}
				
				if (argsMap.get("id") == null) {
					throw new IllegalArgumentException("id is not specified!");
				}
				
				Registrator.register(argsMap.get("uid"), argsMap.get("provider"), argsMap.get("id"));
				writeResponse("200 OK", "Registered!");
			}
			break;

			case "/notify": {
				final HashMap<String, String> argsMap = conentAsArgumentsMap();
				int id = -1;
				if (argsMap.containsKey("id")) {
					try {
						id = Integer.parseInt(argsMap.get("id"));
					} catch (Throwable e) {
					}
				}

				final String topic = argsMap.containsKey("topic")
					? argsMap.get("topic")
					: null;

				final String title = argsMap.get("title");
				final String text = argsMap.get("text");
				final String notificationProfile = argsMap.containsKey("notification_profile")
						? argsMap.get("notification_profile")
						: null;

				int badge = -1;
				if (argsMap.containsKey("badge")) {
					try {
						badge = Integer.parseInt(argsMap.get("badge"));
					} catch (Throwable e) {
					}
				}

				final String serverMessage = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(new Date());

				String notified;
				boolean success;
				if (topic == null) {
					int count = PushNotificator.notifyAll(id, title, text, serverMessage, notificationProfile, badge);
					notified = count + " clients";
					success = count > 0;
				} else {
					success = PushNotificator.notifyTopic(topic, id, title, text, serverMessage, notificationProfile, badge);
					notified = success ? "topic: " + topic : "nothing";
				}
				writeResponse("200 OK", "Notified " + notified +
					(success
						? ""
						: "\nPlease make sure you initialized push notifications using UTNotifications.Manager.Instance.Initialize()"));
			}
			break;

			default: {
				final String notifyAllHref = "/notify?id=1&title=Push%20Notification&text=Works%20%F0%9F%98%BA%21&badge=1";
				final String notifyTopicHref = notifyAllHref + "&topic=all";

				writeResponse("200 OK", "<h2>UTNotifications Demo Server</h2><p>Use, f.e. <a href=\"" + notifyAllHref + "\">" + notifyAllHref
						+ "</a> to send notifications to all registered clients<br>"
						+ "You can also add &topic=<topic>, f.e. <a href=\"" + notifyTopicHref + "\">" + notifyTopicHref
						+ "</a> to send notifications to FCM topics (Firebase Cloud Messaging only)</p>");
			}
			}
		}

		private HashMap<String, String> conentAsArgumentsMap() throws UnsupportedEncodingException {
			HashMap<String, String> resultMap = new HashMap<String, String>();

			if (content != null && content.contains("=")) {
				String[] args = content.split("&");
				for (String arg : args) {
					String[] pair = arg.split("=");
					resultMap.put(java.net.URLDecoder.decode(pair[0], "UTF-8"),
							java.net.URLDecoder.decode(pair[1], "UTF-8"));
				}
			}

			return resultMap;
		}

		private Socket socket;
		private InputStream is;
		private OutputStream os;
		private String request;
		private String content;
	}
}