
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Do.Platform;
using Do.Universe;

namespace Transmission {
	
	class Utils {

		public static int ParseSpeed(string speed) {
			Regex regex = new Regex(
				@"^(\d+)\s*(b|[km]i?b?)$",
				RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace
			);
			Match match = regex.Match(speed);

			if (match.Success) {
				int number = int.Parse(match.Groups[1].Value);
				string unit = match.Groups[2].Value.ToLower();
				int scale = 1;

				if (unit == "" || unit[0] == 'k') scale = 1;
				else if (unit[0] == 'm') scale = 1024;

				return number * scale;

			} else {
				throw new ArgumentException("Invalid speed string");
			}
		}

		public static string FormatAmount(float amount, string baseFormat, float[] scales, string[] formats) {
			if (scales.Length != formats.Length)
				throw new ArgumentException("'scales' and 'formats' arguments must have equal length");

			// The typical scales count is three to five, so don't use binary search,
			// but just plain reverse loop.
			for (int i = scales.Length-1; i >= 0; --i) {
				if (amount >= scales[i])
					return string.Format(formats[i], amount / scales[i]);
			}

			return string.Format(baseFormat, amount);
		}

		// Format speed in KiB/sec into human-readable representation.
		public static string FormatSpeed(int speed_kbytes_sec) {
			return FormatAmount(speed_kbytes_sec, "{0} KiB/sec",
				new float[]  {             1024,         1024*1024},
				new string[] {"{0:#.#} MiB/sec", "{0:#.#} GiB/sec"}
			);
		}

		// Format size in bytes into human-readable representation.
		public static string FormatSize(long size_bytes) {
			return FormatAmount(size_bytes, "{0} B",
				new float[]  {         1024,     1024*1024, 1024*1024*1024},
				new string[] {"{0:#.#} KiB", "{0:#.#} MiB", "{0:#.##} GiB"}
			);
		}

		public readonly static IEnumerable<PredefinedSpeed> PredefinedSpeedItems = new List<PredefinedSpeed>() {
			new PredefinedSpeed(        10,  "10 KiB/sec", ""),
			new PredefinedSpeed(        20,  "20 KiB/sec", ""),
			new PredefinedSpeed(        50,  "50 KiB/sec", ""),
			new PredefinedSpeed(       100, "100 KiB/sec", ""),
			new PredefinedSpeed(       200, "200 KiB/sec", ""),
			new PredefinedSpeed(       500, "500 KiB/sec", ""),
			new PredefinedSpeed(  1 * 1024,   "1 MiB/sec", ""),
			new PredefinedSpeed(  2 * 1024,   "2 MiB/sec", ""),
			new PredefinedSpeed(  5 * 1024,   "5 MiB/sec", ""),
			new PredefinedSpeed( 10 * 1024,  "10 MiB/sec", ""),
			new PredefinedSpeed( 20 * 1024,  "20 MiB/sec", ""),
			new PredefinedSpeed( 50 * 1024,  "50 MiB/sec", ""),
			new PredefinedSpeed(100 * 1024, "100 MiB/sec", ""),
		};

	}

	public class PredefinedSpeed: Item {
		private string _name, _desc;
		private int _value;

		public PredefinedSpeed(int value, string name, string desc) {
			_value = value;
			_name = name;
			_desc = desc;
		}

		public override string Name {
			get { return _name; }
		}
		
		public override string Description {
			get { return _desc; }
		}

		public override string Icon {
			get { return "top"; }
		}

		public int Value {
			get { return _value; }
		}
	}

}
