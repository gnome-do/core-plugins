/* AliasItemSource.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace Alias
{
	
	public class AliasItemSource : ItemSource {

		[Serializable]
		class AliasRecord {
			
			public string UniqueId { get; protected set; }
			public string Alias { get; protected set; }
			
			public AliasRecord (string uniqueId, string alias)
			{
				UniqueId = uniqueId;
				Alias = alias;
			}

			public AliasItem MaybeGetItem ()
			{
				Item item = Services.Core.GetItem (UniqueId) as Item;
				return item == null ? null : new AliasItem (item, Alias);
			}
		}
		
		static List<AliasRecord> aliases;
		
		static string AliasFile {
			get {
				return Path.Combine (Services.Paths.UserDataDirectory, typeof (AliasItemSource).FullName);
			}
		}
		
		static AliasItemSource ()
		{
			Deserialize ();
		}
		
		static void Deserialize ()
		{
			aliases = null;
			try {
				using (Stream s = File.OpenRead (AliasFile)) {
					BinaryFormatter f = new BinaryFormatter ();
					aliases = f.Deserialize (s) as List<AliasRecord>;
				}
			} catch (FileNotFoundException) {
			} catch (Exception e) {
				Log.Error ("Could not deserialize alias records: {0}", e.Message);
				Log.Debug (e.StackTrace);
			} finally {
				aliases = aliases ?? new List<AliasRecord> ();
			}
		}
		
		static void Serialize ()
		{
			try {
				using (Stream s = File.OpenWrite (AliasFile)) {
					BinaryFormatter f = new BinaryFormatter ();
					f.Serialize (s, aliases);
				}
			} catch (Exception e) {
				Log.Error ("Could not serialize alias records: {0}", e.Message);
				Log.Debug (e.StackTrace);
			}
		}
		
		public static Item Alias (Item item, string alias)
		{
			AliasItem aliasItem;
			
			if (!ItemHasAlias (item, alias)) {
				aliases.Add (new AliasRecord (item.UniqueId, alias));
			}
			aliasItem = new AliasItem (item, alias);

			Serialize ();
			return aliasItem;
		}
		
		public static void Unalias (Item item)
		{
			int i = IndexOfAlias (item);
			
			if (i == -1) return;
			
			aliases.RemoveAt (i);
			Serialize ();
		}
		
		public static bool ItemHasAlias (Item item)
		{
			return IndexOfAlias (item) != -1;
		}
		
		public static bool ItemHasAlias (Item item, string alias)
		{
			int i = IndexOfAlias (item);
			return i != -1 && aliases [i].Alias == alias;
		}
		
		static int IndexOfAlias (Item item)
		{
			return aliases.FindIndex (a => a.UniqueId == item.UniqueId);
		}
		
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Alias items"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Aliased items from Do's universe."); }
		}

		public override string Icon {
			get { return "emblem-symbolic-link"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (AliasItem); }
		}

		public override IEnumerable<Item> Items {
			get {
				return aliases
					.Select (a => a.MaybeGetItem ())
					.Where (i => i != null)
					.OfType<Item> ();
			}
		}
		
	}
}
