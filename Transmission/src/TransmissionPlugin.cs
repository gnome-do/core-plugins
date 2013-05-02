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
			string host = TransmissionConfig.Address;
			int port = TransmissionConfig.Port;
			string username = TransmissionConfig.UserName;
			string password = TransmissionConfig.Password;

			string url = string.Format("http://{0}:{1}/transmission/rpc", host, port);
			return new ConnectionParameters(url, username, password);
		}

	};

}

