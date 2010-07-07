using System;
using System.Collections.Generic;

using Do.Platform;

namespace Transmission {

	public class ConnectionParameters {
		public ConnectionParameters(string url, string username, string password) {
			this.url = url;
			this.username = username;
			this.password = password;
		}

		public string url;
		public string username;
		public string password;
	};

	public class TransmissionPlugin {
		private static TransmissionAPI transmission;

		public static TransmissionAPI getTransmission() {
			if (transmission == null) {
				ConnectionParameters p = getTransmissionConnectionParameters();
				Log<TransmissionPlugin>.Info("Using Transmission on {0}", p.url);
				Log<TransmissionPlugin>.Debug("Using name, password: {0}:{1}", p.username, p.password);
				transmission = new TransmissionAPI(p.url, p.username, p.password);
			}
			return transmission;
		}

		public static void ResetConnection() {
			transmission = null;
		}

		public static ConnectionParameters getTransmissionConnectionParameters() {
			string host, username, password;
			int port;
			
			if (TransmissionConfig.UseRemote) {
				host = TransmissionConfig.RemoteAddress;
				port = TransmissionConfig.RemotePort;
				username = TransmissionConfig.RemoteUserName;
				password = TransmissionConfig.RemotePassword;

			} else {
				Log<TransmissionPlugin>.Debug("Reading Transmission configuration file at {0}", TransmissionConfig.settings_path);
				Jayrock.Json.JsonObject config = TransmissionConfig.ReadTransmissionConfig(TransmissionConfig.settings_path);
				if (! (bool)config["rpc-enabled"])
					throw new Exception("Transmission RPC is disabled");
				
				Log<TransmissionPlugin>.Debug("Local Transmission RPC interface is enabled");

				if ((bool)config["rpc-whitelist-enabled"]) {
					Log<TransmissionPlugin>.Debug("Transmission RPC white list is enabled");
					string comma_separated_ips = (string)config["rpc-whitelist"];
					string[] ips = comma_separated_ips.Split(',');
					if (Array.IndexOf(ips, "127.0.0.1") < 0)
						throw new Exception("Transmission RPC is not available from localhost");
				}

				host = "localhost";
				port = (int)(Jayrock.Json.JsonNumber)config["rpc-port"];
				Log<TransmissionPlugin>.Debug("Local Transmission address: {0}:{1}", host, port);

				if ((bool)config["rpc-authentication-required"]) {
					username = (string)config["rpc-username"];
					password = (string)config["rpc-password"];
				} else {
					username = null;
					password = null;
				}
			}

			string url = string.Format("http://{0}:{1}/transmission/rpc", host, port);
			return new ConnectionParameters(url, username, password);
		}

	};

}

