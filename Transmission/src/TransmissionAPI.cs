
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Jayrock.Json;

namespace Transmission {

	/// <summary>
	/// Transmission API client.
	/// </summary>
	public class TransmissionAPI {

		/// <summary>
		/// File loading priority.
		/// </summary>
		public enum FilePriority {
			Low, Normal, High
		};

		/// <summary>
		/// Operation on file.
		/// </summary>
		public struct FileOperation {
			public FileOperation(bool? download, FilePriority? priority) {
				this.download = download;
				this.priority = priority;
			}

			/// <summary>
			///
			/// </summary>
			public bool? download;
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

		private void NullHandler(JsonReader json) {}

		/// <summary>
		/// Create API client.
		/// </summary>
		/// <param name="url">Transmission API-RPC URL</param>
		public TransmissionAPI(string url, string username, string password) {
			_url = url;
			_username = username;
			_password = password;
		}

		private string ComposeRequest(string method, IDictionary<string, object> arguments) {
			StringWriter sw = new StringWriter();

			using (JsonWriter json = new JsonTextWriter(sw)) {
				json.WriteStartObject();

				json.WriteMember("method");
				json.WriteString(method);

				json.WriteMember("arguments");
				json.WriteStartObject();
				foreach (KeyValuePair<string, object> pair in arguments) {
					string name = pair.Key;
					object value = pair.Value;

					json.WriteMember(name);
					Jayrock.Json.Conversion.JsonConvert.Export(value, json);
				}
				json.WriteEndObject();

				json.WriteEndObject();
			}

			return sw.ToString();
		}

		private string MakeRequest(byte[] payload) {
			// Connect to Transmission.
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.Headers[SESSION_HEADER] = _session_id;
			request.ContentLength = payload.Length;

			if (_username != null && _password != null) {
				string auth = Convert.ToBase64String(Encoding.Default.GetBytes(_username + ":" + _password));
				request.Headers["Authorization"] = "Basic " + auth;
			}

			using (Stream stream = request.GetRequestStream()) {
				stream.Write(payload, 0, payload.Length);
			}

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			string resp = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8).ReadToEnd();
			return resp;
		}

		/// <summary>
		/// Call Transmission API method.
		/// </summary>
		/// <param name="method">API method name</param>
		/// <param name="arguments">Arguments passed to API method</param>
		/// <param name="handler">Function called with value returned by method</param>
		/// <exception cref="TransmissionError" />
		/// <exception cref="TransmissionAPIError" />
		private void Call(string method, IDictionary<string, object> arguments, ResultReader handler) {
			// Compose request and encode it to UTF-8.
			string req = ComposeRequest(method, arguments);
			byte[] reqb = System.Text.Encoding.UTF8.GetBytes(req);

			try {

				string resp = null;
				try {
					resp = MakeRequest(reqb);

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
						resp = MakeRequest(reqb);

					} else {
						throw;
					}
				}

				//System.Console.WriteLine(resp);
				//JsonReader json_reader = new JsonTextReader(new StreamReader(response.GetResponseStream()));
				JsonReader json_reader = new JsonTextReader(new StringReader(resp));

				json_reader.ReadToken(JsonTokenClass.Object);
				while (json_reader.TokenClass == JsonTokenClass.Member) {
					switch (json_reader.ReadMember()) {
					case "result":
						string result = json_reader.ReadString();
						if (result != "success")
							throw new TransmissionError(result);
						break;
					case "tag":
						json_reader.ReadNumber();
						break;
					case "arguments":
						json_reader.ReadToken(JsonTokenClass.Object);
						handler(json_reader);
						json_reader.ReadToken(JsonTokenClass.EndObject);
						break;
					default:
						json_reader.Skip();
						break;
					}
				}
				json_reader.ReadToken(JsonTokenClass.EndObject);
				json_reader.ReadToken(JsonTokenClass.EOF);
			} catch (System.Net.WebException err) {
				throw new TransmissionAPIError("Cannot access Transmission RPC service", err);
			}
		}

		/// <summary>
		/// Call API method and ignore return value.
		///
		/// This is equivalent to <c>Call(method, arguments, NullHandler)</c>.
		/// </summary>
		private void Call(string method, IDictionary<string, object> arguments) {
			Call(method, arguments, NullHandler);
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

			if (peer_limit != null)
				arguments.Add("peer-limit", peer_limit);

			if (limit_download.HasValue) {
				if (limit_download.Value) {
					arguments.Add("speed-limit-down-enabled", true);
					if (download_speed_limit.HasValue)
						arguments.Add("speed-limit-down", download_speed_limit.Value);
				} else {
					arguments.Add("speed-lmit-down-enabled", false);
				}
			}

			if (limit_upload.HasValue) {
				if (limit_upload.Value) {
					arguments.Add("speed-limit-up-enabled", true);
					if (upload_speed_limit.HasValue)
						arguments.Add("speed-limit-up", upload_speed_limit.Value);
				} else {
					arguments.Add("speed-lmit-up-enabled", false);
				}
			}

			Call("torrent-set", arguments);
		}

		public void SetTorrent(string torrent_hash, int? peer_limit, bool? limit_download, int? download_speed_limit, bool? limit_upload, int? upload_speed_limit, IDictionary<int, FileOperation> files) {
			Dictionary<string, object> arguments = new Dictionary<string, object>();

			arguments.Add("ids", new string[] { torrent_hash });

			if (peer_limit != null)
				arguments.Add("peer-limit", peer_limit);

			if (limit_download.HasValue) {
				if (limit_download.Value) {
					arguments.Add("speed-limit-down-enabled", true);
					if (download_speed_limit.HasValue)
						arguments.Add("speed-limit-down", download_speed_limit.Value);
				} else {
					arguments.Add("speed-lmit-down-enabled", false);
				}
			}

			if (limit_upload.HasValue) {
				if (limit_upload.Value) {
					arguments.Add("speed-limit-up-enabled", true);
					if (upload_speed_limit.HasValue)
						arguments.Add("speed-limit-up", upload_speed_limit.Value);
				} else {
					arguments.Add("speed-lmit-up-enabled", false);
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
			public int Id; // Torrent's unique ID within Transmission.
			public string Comment;
			public string HashString;
			public string Name;
			public string DownloadDir;
			public IList<TorrentFileInfo> files;
			public TorrentStatus Status;
			public long TotalSize;

			public int DownloadLimitMode;
			public int DownloadLimit;
			public int UploadLimitMode;
			public int UploadLimit;
		};

		public class TorrentFileInfo {
			public string Name;
			public long Length;
			public long BytesCompleted;
			public bool Wanted;
			public FilePriority Priority;
		};

		/// <summary>
		/// Get information about all torrents.
		///
		/// This is equivalent to <c>GetTorrents(null)</c>.
		/// </summary>
		public IEnumerable<TorrentInfo> GetAllTorrents() {
			return GetTorrents(null);
		}

		public IEnumerable<TorrentInfo> GetTorrents(IEnumerable<string> torrent_hashes) {
			Dictionary<string, object> arguments = new Dictionary<string, object>();
			if (torrent_hashes != null) arguments.Add("ids", torrent_hashes);
			arguments.Add("fields", new string[] {"comment", "downloadDir", "files", "hashString", "id", "name", "priorities", "status", "totalSize", "wanted", "downloadLimitMode", "downloadLimit", "uploadLimitMode", "uploadLimit"});

			List<TorrentInfo> torrents = new List<TorrentInfo>();

			Jayrock.Json.Conversion.ImportContext jsonctx = new Jayrock.Json.Conversion.ImportContext();
			jsonctx.Register(new ListImporter<int>());
			jsonctx.Register(new ListImporter<TorrentFileInfo>());

			Call("torrent-get", arguments, delegate(JsonReader json) {
				while (json.TokenClass == JsonTokenClass.Member) {
					switch (json.ReadMember()) {
					case "torrents":
						json.ReadToken(JsonTokenClass.Array);
						while (json.TokenClass != JsonTokenClass.EndArray) {

							json.ReadToken(JsonTokenClass.Object);

							TorrentInfo torrent = new TorrentInfo();
							IList<TorrentFileInfo> files = null;
							IList<int> wanted = null;
							IList<int> priorities = null;

							while (json.TokenClass == JsonTokenClass.Member) {
								switch (json.ReadMember()) {
								case "comment": torrent.Comment = json.ReadString(); break;
								case "hashString": torrent.HashString = json.ReadString(); break;
								case "name": torrent.Name = json.ReadString(); break;
								case "id": torrent.Id = json.ReadNumber().ToInt32(); break;
								case "downloadDir": torrent.DownloadDir = json.ReadString(); break;
								case "status": torrent.Status = (TorrentStatus)json.ReadNumber().ToInt32(); break;
								case "totalSize": torrent.TotalSize = json.ReadNumber().ToInt64(); break;
								case "wanted": wanted = (IList<int>)jsonctx.Import(typeof(List<int>), json); break;
								case "priorities": priorities = (IList<int>)jsonctx.Import(typeof(List<int>), json); break;
								case "files": files = jsonctx.Import(typeof(List<TorrentFileInfo>), json) as IList<TorrentFileInfo>; break;
								case "downloadLimitMode": torrent.DownloadLimitMode = json.ReadNumber().ToInt32(); break;
								case "downloadLimit": torrent.DownloadLimit = json.ReadNumber().ToInt32(); break;
								case "uploadLimitMode": torrent.UploadLimitMode = json.ReadNumber().ToInt32(); break;
								case "uploadLimit": torrent.UploadLimit = json.ReadNumber().ToInt32(); break;
								}
							}

							for (int i = 0; i < files.Count; ++i) {
								files[i].Wanted = wanted[i] == 1;

								int prio = priorities[i];
								if (prio < -1 || prio > 1)
									throw new TransmissionAPIError(string.Format("Invalid priority value: {0}", prio));
								files[i].Priority = (FilePriority)(prio+1);
							}
							torrent.files = files;
							torrents.Add(torrent);

							json.ReadToken(JsonTokenClass.EndObject);
						}
						json.ReadToken(JsonTokenClass.EndArray);
						break;
					default:
						json.Skip();
						break;
					}
				}
			});

			return torrents;
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
					arguments.Add("speed-limit-down-enabled", true);
					if (download_speed_limit.HasValue)
						arguments.Add("speed-limit-down", download_speed_limit.Value);
				} else {
					arguments.Add("speed-lmit-down-enabled", false);
				}
			}

			if (limit_upload.HasValue) {
				if (limit_upload.Value) {
					arguments.Add("speed-limit-up-enabled", true);
					if (upload_speed_limit.HasValue)
						arguments.Add("speed-limit-up", upload_speed_limit.Value);
				} else {
					arguments.Add("speed-lmit-up-enabled", false);
				}
			}

			Call("session-set", arguments);
		}

		public void GetSession() {
			Call("session-get", new Dictionary<string, object>());
		}

	}
}
