
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Transmission {

	/// <summary>
	/// Transmission API client.
	/// Compatible with RPC version 5 to 10 (release version 1.60 to 2.10).
	/// </summary>
	public class TransmissionAPI {

		/// <summary>
		/// File loading priority.
		/// </summary>
		public enum FilePriority : int {
			Low = -1,
			Normal = 0, 
			High = 1
		};

		/// <summary>
		/// Operation on individual file from torrent.
		/// </summary>
		/// <remarks>
		/// All fields are nullable, <c>null</c> means "don't change current value".
		/// </remarks>
		public struct FileOperation {
			public FileOperation(bool? download, FilePriority? priority) {
				this.download = download;
				this.priority = priority;
			}

			/// <summary>Whether it is needed to download this file</summary>
			public bool? download;

			/// <summary>Priority relative to other files of the same torrent</summary>
			public FilePriority? priority;
		};

		/// <summary>
		/// Error communicating to Transmission.
		/// </summary>
		public class TransmissionAPIError: Exception {
			public TransmissionAPIError(string message): base(message) {}
			public TransmissionAPIError(string message, Exception reason): base(message, reason) {}
		};

		/// <summary>
		/// Error returned by Transmission.
		/// </summary>
		public class TransmissionError: Exception {
			public TransmissionError(string message): base(message) {
			}
		};

		public const int DEFAULT_PORT = 9091;
		public const string DEFAULT_PATH = "/transmission/rpc";
		public const string SESSION_HEADER = "X-Transmission-Session-Id";

		private string _url, _username, _password;
		private string _session_id = "";

		private delegate void ResultReader(JsonReader json);

		/// <summary>Response handler which does nothing</summary>
		/// <remarks>Prefer using <see cref="Call(string, IDictionary<string, object>)"/> instead</remarks>
		private void NullHandler(JsonReader json) {}

		/// <summary>
		/// Create API client.
		/// </summary>
		/// <param name="url">Transmission API-RPC URL</param>
		/// <param name="username">Username for authentication</param>
		/// <param name="password">Password for authentication</param>
		/// <remarks>Pass <c>null</c> for both <paramref="username"/> and <paramref="username"/> is
		/// authentication isn't needed.</remarks>
		public TransmissionAPI(string url, string username, string password) {
			_url = url;
			_username = username;
			_password = password;
		}

		/// <summary></summary>
		/// <param name="payload">HTTP POST request content</param>
		/// <returns>HTTP response content</returns>
		/// <exception cref="System.Net.WebException"/>
		protected string PerformRequest(byte[] payload) {
			// Prepare request.
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.Headers[SESSION_HEADER] = _session_id;
			request.ContentLength = payload.Length;

			// Authenticate if credentials are given.
			if (_username != null && _password != null) {
				string auth = Convert.ToBase64String(Encoding.Default.GetBytes(_username + ":" + _password));
				request.Headers["Authorization"] = "Basic " + auth;
			}

			// Perform request.
			using (Stream stream = request.GetRequestStream()) {
				stream.Write(payload, 0, payload.Length);
			}

			// Get response.
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			return new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8).ReadToEnd();
		}

		private class APIRequest
		{
			public string method;
			public IDictionary<string, object> arguments = new Dictionary<string, object> ();
			public int? tag;
		}

		private class APIResult<T> where T : class
		{
			public string result;
			public T arguments;
			public int? tag;
		}

		/// <summary>
		/// Call Transmission API method.
		/// </summary>
		/// <param name="method">API method name</param>
		/// <param name="arguments">Arguments passed to API method</param>
		/// <param name="handler">Function called with value returned by method</param>
		/// <exception cref="TransmissionError" />
		/// <exception cref="TransmissionAPIError" />
		private T Call<T>(APIRequest request) where T : class {
			// Compose request and encode it to UTF-8.
			string req = JsonConvert.SerializeObject (request, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
			byte[] reqb = System.Text.Encoding.UTF8.GetBytes(req);

			try {

				string resp = null;
				try {
					resp = PerformRequest(reqb);

				} catch (System.Net.WebException err) {
					HttpWebResponse response = (HttpWebResponse)err.Response;

					// Transmission 1.53 and 1.6 introduced X-Transmission-Session-Id header
					// in order to protect from CSRF attacks. If you make request without
					// this header (or your session expire) you'll get 409 response with
					// this header set. Just copy header to your request and try again.
					if (
						err.Status == WebExceptionStatus.ProtocolError &&
						response.StatusCode == HttpStatusCode.Conflict
					) {
						_session_id = response.Headers[SESSION_HEADER];
						resp = PerformRequest(reqb);

					} else {
						throw;
					}
				}

				var result = JsonConvert.DeserializeObject<APIResult<T>> (resp, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate });
				return result.arguments;
			} catch (System.Net.WebException err) {
				throw new TransmissionAPIError("Cannot access Transmission RPC service", err);
			}
		}

		/// <summary>Call API method and ignore return value</summary>
		/// <remarks>This is equivalent to <c>Call(method, arguments, NullHandler)</c>.</remarks>
		private void Call(string method, IDictionary<string, object> arguments)
		{
			APIRequest request = new APIRequest ();
			request.method = method;
			request.arguments = arguments;
			Call<object>(request);
		}

		/// <summary>
		/// Start torrents.
		/// </summary>
		/// <param name="torrent_hashes">Sequence of torrent's hashes</param>
		/// <exception cref="TransmissionAPIError" />
		/// <exception cref="TransmissionError" />
		public void StartTorrents(IEnumerable<string> torrent_hashes) {
			Dictionary<string, object> arguments = new Dictionary<string, object>();
			arguments.Add("ids", torrent_hashes);
			Call("torrent-start", arguments);
		}

		/// <summary>
		/// Start all torrents.
		/// </summary>
		/// <exception cref="TransmissionAPIError" />
		/// <exception cref="TransmissionError" />
		public void StartAllTorrents() {
			Call("torrent-start", new Dictionary<string, object>());
		}

		/// <summary>
		/// Stop torrents.
		/// </summary>
		/// <param name="torrent_hashes">Sequence of torrent's hashes</param>
		/// <exception cref="TransmissionAPIError" />
		/// <exception cref="TransmissionError" />
		public void StopTorrents(IEnumerable<string> torrent_hashes) {
			Dictionary<string, object> arguments = new Dictionary<string, object>();
			arguments.Add("ids", torrent_hashes);
			Call("torrent-stop", arguments);
		}

		/// <summary>
		/// Stop all torrents.
		/// </summary>
		/// <exception cref="TransmissionAPIError" />
		/// <exception cref="TransmissionError" />
		public void StopAllTorrents() {
			Call("torrent-stop", new Dictionary<string, object>());
		}

		/// <summary>
		/// Start torrents verification.
		/// </summary>
		/// <param name="torrent_hashes">Sequence of torrent's hashes to verify</param>
		/// <exception cref="TransmissionAPIError" />
		/// <exception cref="TransmissionError" />
		public void VerifyTorrents(IEnumerable<string> torrent_hashes) {
			Dictionary<string, object> arguments = new Dictionary<string, object>();
			arguments.Add("ids", torrent_hashes);
			Call("torrent-verify", arguments);
		}

		/// <summary>
		/// Start torrents verification for all torrents.
		/// </summary>
		/// <exception cref="TransmissionAPIError" />
		/// <exception cref="TransmissionError" />
		public void VerifyAllTorrents() {
			Call("torrent-verify", new Dictionary<string, object>());
		}

		/// <summary>
		/// Set torrents' properties.
		///
		/// Not all torrent properties can be changed using this method, because some of them
		/// are meaningless for torrent group, e.g. file priorities. Use <c>SetTorrent</c> method
		/// to set such properties.
		/// </summary>
		/// <param name="torrent_hashes">Sequence of torrents hashed to modify</param>
		/// <param name="peer_limit">Maximum number of used peers, if it is <c>null</c> then value won't be changed.</param>
		/// <param name="limit_download">
		/// Downloading speed limit switch, if it is <c>true</c>, downloading speed will be limited,
		/// if it is <c>false</c>, downloading speed isn't limited, if it is <c>null</c>, limit switch won't be changed.
		/// A <see cref="System.Nullable"/>
		/// </param>
		/// <param name="download_speed_limit">
		/// A <see cref="System.Nullable"/>
		/// </param>
		/// <param name="limit_upload">
		/// A <see cref="System.Nullable"/>
		/// </param>
		/// <param name="upload_speed_limit">
		/// A <see cref="System.Nullable"/>
		/// </param>
		public void SetTorrents(IEnumerable<string> torrent_hashes, int? peer_limit, bool? limit_download, int? download_speed_limit, bool? limit_upload, int? upload_speed_limit) {
			Dictionary<string, object> arguments = new Dictionary<string, object>();

			arguments.Add("ids", torrent_hashes);
			arguments.Add("peer-limit", peer_limit);

			if (limit_download.HasValue) {
				if (limit_download.Value) {
					arguments.Add("downloadLimited", true);
					if (download_speed_limit.HasValue)
						arguments.Add("downloadLimit", download_speed_limit.Value);
				} else {
					arguments.Add("downloadLimited", false);
				}
			}

			if (limit_upload.HasValue) {
				if (limit_upload.Value) {
					arguments.Add("uploadLimited", true);
					if (upload_speed_limit.HasValue)
						arguments.Add("uploadLimit", upload_speed_limit.Value);
				} else {
					arguments.Add("uploadLimited", false);
				}
			}

			Call("torrent-set", arguments);
		}

		public void SetTorrent(string torrent_hash, int? peer_limit, bool? limit_download, int? download_speed_limit, bool? limit_upload, int? upload_speed_limit, IDictionary<int, FileOperation> files) {
			Dictionary<string, object> arguments = new Dictionary<string, object>();

			arguments.Add("ids", new string[] { torrent_hash });
			arguments.Add("peer-limit", peer_limit);

			if (limit_download.HasValue) {
				if (limit_download.Value) {
					arguments.Add("downloadLimited", true);
					if (download_speed_limit.HasValue)
						arguments.Add("downloadLimit", download_speed_limit.Value);
				} else {
					arguments.Add("downloadLimited", false);
				}
			}

			if (limit_upload.HasValue) {
				if (limit_upload.Value) {
					arguments.Add("uploadLimited", true);
					if (upload_speed_limit.HasValue)
						arguments.Add("uploadLimit", upload_speed_limit.Value);
				} else {
					arguments.Add("uploadLimited", false);
				}
			}

			List<int> wanted_files = new List<int>();
			List<int> unwanted_files = new List<int>();
			List<int> low_priority_files = new List<int>();
			List<int> normal_priority_files = new List<int>();
			List<int> high_priority_files = new List<int>();
			foreach (KeyValuePair<int, FileOperation> op in files) {
				int index = op.Key;
				FileOperation file = op.Value;

				if (file.download.HasValue) {
					if (file.download.Value) wanted_files.Add(index);
					else unwanted_files.Add(index);
				}

				if (file.priority.HasValue) {
					switch (file.priority.Value) {
					case FilePriority.Low: low_priority_files.Add(index); break;
					case FilePriority.Normal: normal_priority_files.Add(index); break;
					case FilePriority.High: high_priority_files.Add(index); break;
					}
				}
			}
			if (wanted_files.Count > 0) arguments.Add("files-wanted", wanted_files);
			if (unwanted_files.Count > 0) arguments.Add("files-unwanted", unwanted_files);
			if (low_priority_files.Count > 0) arguments.Add("priority-low", low_priority_files);
			if (normal_priority_files.Count > 0) arguments.Add("priority-normal", normal_priority_files);
			if (high_priority_files.Count > 0) arguments.Add("priority-high", high_priority_files);

			Call("torrent-set", arguments);
		}

		/// <summary>
		/// Torrent status.
		/// </summary>
		public enum TorrentStatus {
			CheckWait = 1, Check = 2, Download = 4, Seed = 8, Stopped = 16
		};

		public class TorrentInfo {
			[JsonProperty("id")]
			public int Id; // Torrent's unique ID within Transmission.

			[JsonProperty("comment")]
			public string Comment;

			[JsonProperty("hashString")]
			public string HashString;

			[JsonProperty("name")]
			public string Name;

			[JsonProperty("downloadDir")]
			public string DownloadDir;

			[JsonProperty("files")]
			private IList<FileResponse> files;
			[JsonProperty("wanted")]
			private IList<bool> wanted;
			[JsonProperty("priorities")]
			private IList<int> priorities;

			[JsonIgnore]
			public IEnumerable<TorrentFileInfo> Files 
			{
				get {
					for (int i = 0; i < files.Count ; ++i) {
						yield return new TorrentFileInfo  {
							Name = files[i].name,
							Length = files[i].length,
							BytesCompleted = files[i].bytesCompleted,
							Wanted = wanted[i],
							Priority = (FilePriority)priorities[i]
						};
					}
					yield break;
				}
			}

			[JsonProperty("status")]
			private int status;
			[JsonIgnore]
			public TorrentStatus Status {
				get {
					return (TorrentStatus)status;
				}
			}

			[JsonProperty("totalSize")]
			public ulong TotalSize;

			[JsonProperty("downloadLimit")]
			public int DownloadLimit;

			[JsonProperty("downloadLimited")]
			public bool DownloadLimited;

			[JsonProperty("uploadLimit")]
			public int UploadLimit;

			[JsonProperty("uploadLimited")]
			public bool UploadLimited;
		};

		public class TorrentFileInfo {
			public string Name;
			public ulong Length;
			public ulong BytesCompleted;
			public bool Wanted;
			public FilePriority Priority;
		};

		public class FileResponse
		{
			public UInt64 bytesCompleted;
			public UInt64 length;
			public string name;
		}

		public class TorrentGetResponse
		{
			public IList<TorrentInfo> torrents;
		}

		/// <summary>Get information about all torrents.</summary>
		/// <remarks>This is equivalent to <c>GetTorrents(null)</c>.</remarks>
		public IEnumerable<TorrentInfo> GetAllTorrents() {
			return GetTorrents(null);
		}

		public IEnumerable<TorrentInfo> GetTorrents(IEnumerable<string> torrent_hashes) {
			APIRequest request = new APIRequest ();

			request.method = "torrent-get";

			if (torrent_hashes != null)
				request.arguments.Add("ids", torrent_hashes);
			request.arguments.Add("fields", new string[] {"comment", "downloadDir", "files", "hashString", "id",
														  "name", "priorities", "status", "totalSize", "wanted",
														  "downloadLimited", "downloadLimit", "uploadLimited",
														  "uploadLimit"});

			return Call<TorrentGetResponse> (request).torrents;
		}

		// Transmission RPC allows to use .torrent file content instead of it's filename, but
		// this isn't supported.
		public void AddTorrent(string filename, string download_to, bool paused, int? peer_limit) {
			Dictionary<string, object> arguments = new Dictionary<string, object>();
			arguments.Add("filename", filename);
			arguments.Add("download-dir", download_to);
			arguments.Add("paused", paused);
			if (peer_limit.HasValue) arguments.Add("peer-limit", peer_limit.Value);
			Call("torrent-add", arguments);
		}

		/// <summary>
		/// Remove torrents.
		/// </summary>
		/// <param name="hashes">Sequence of torrent hashes to remove.</param>
		/// <param name="delete_files">Whether downloaded files should be deleted or not.</param>
		public void RemoveTorrent(IEnumerable<string> hashes, bool delete_files) {
			Dictionary<string, object> arguments = new Dictionary<string, object>();
			arguments.Add("ids", hashes);
			arguments.Add("delete-load-data", delete_files);
			Call("torrent-remove", arguments);
		}

		/// <summary>
		/// Set session parameters.
		/// </summary>
		/// <param name="download_dir">
		/// Path to directory to download torrent contents to.
		/// Pass <c>null</c> to keep old value.
		/// </param>
		/// <param name="peer_limit">
		/// Global limit on number of connected peers. Pass <c>null</c> to keep old value.
		/// </param>
		/// <param name="limit_download">
		/// Global limit on number of connected peers. Pass <c>null</c> to keep old value.
		/// </param>
		public void SetSession(
			string download_dir, int? peer_limit,
			bool? limit_download, int? download_speed_limit, bool? limit_upload, int? upload_speed_limit
		) {
			Dictionary<string, object> arguments = new Dictionary<string, object>();

			if (download_dir != null)
				arguments.Add("download-dir", download_dir);

			if (peer_limit.HasValue)
				arguments.Add("peer-limit", peer_limit.Value);

			if (limit_download.HasValue) {
				if (limit_download.Value) {
					arguments.Add("downloadLimited", true);
					if (download_speed_limit.HasValue)
						arguments.Add("downloadLimit", download_speed_limit.Value);
				} else {
					arguments.Add("downloadLimited", false);
				}
			}

			if (limit_upload.HasValue) {
				if (limit_upload.Value) {
					arguments.Add("uploadLimited", true);
					if (upload_speed_limit.HasValue)
						arguments.Add("uploadLimit", upload_speed_limit.Value);
				} else {
					arguments.Add("uploadLimited", false);
				}
			}

			Call("session-set", arguments);
		}

		public void GetSession() {
			Call("session-get", new Dictionary<string, object>());
		}

	}
}
