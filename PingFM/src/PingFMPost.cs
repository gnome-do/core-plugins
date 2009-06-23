// PingFMPost.cs
// 
// Copyright (C) 2009 GNOME Do
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Addins;
using PingFM.API;

using Do.Universe;
using Do.Platform;

namespace PingFM
{	
	public sealed class PingFMPost : Act
	{
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Post via Ping.FM"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Post a text message as microblog or status update to your social network"); }
        }
			
		public override  string Icon {
			get { return "pingfm.png@" + GetType ().Assembly.FullName; }
		}
		
		public override  IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
		    get { yield return typeof (PingFMServiceItem); }
		}
		
		public override bool ModifierItemsOptional {
			get { return false; }
		}
		
		public override bool SupportsItem (Item item) 
		{
			return GetMessageLength ((item as ITextItem).Text) < 200;
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem) 
		{
			bool support_status = (modItem as PingFMServiceItem).Method.Contains ("status");
			bool support_microblog = (modItem as PingFMServiceItem).Method.Contains ("microblog");
			bool support_media = (modItem as PingFMServiceItem).Method.Contains ("images");
			bool match_trigger = true;
			
			string trigger = FindTrigger ((items.First() as ITextItem).Text);
			if (!String.IsNullOrEmpty (trigger))
				match_trigger = ((modItem as PingFMServiceItem).Trigger == trigger);
			if (GetMessageLength ((items.First () as ITextItem).Text) > 140)
				return (support_status && !support_media && match_trigger);
			else
				return ((support_status || support_microblog) && !support_media && match_trigger);
		}
		
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
		{
			string service = null;
			string icon = Icon;
			string method = "default";
			
			int len = GetMessageLength ((items.First () as ITextItem).Text);
			string body = MessageWithoutTrigger ((items.First () as ITextItem).Text);
			
			if (len < 140 && (modifierItems.First () as PingFMServiceItem).Method.Contains ("microblog"))
				method = "microblog";
			else
				method = "status";
			
			service = (modifierItems.First () as PingFMServiceItem).Id;
			icon = (modifierItems.First () as PingFMServiceItem).Icon;
			
			Services.Application.RunOnThread (() => {
				PingFM.Post (method, body, service, null, icon);
			});
			
			yield break;
		}
		
		int GetMessageLength (string message)
		{
			// If the url length >= 24, Ping.FM will replace it to a short one,
			// we calculate here the length after the replacement
			const string LinkPattern = @"https:\/\/[\S]{16,}|http:\/\/[\S]{17,}|ftp:\/\/[\S]{18,}";
			const string DummyPingFMLink = "http://ping.fm/xxxxx";
			return Regex.Replace (MessageWithoutTrigger (message), LinkPattern, DummyPingFMLink).Length;
		}
		
		string FindTrigger (string message)
		{
			const string TriggerPattern = @"^@[\S]{1,2}\s";
			string trigger = String.Empty;
			Match match = Regex.Match (message, TriggerPattern);
			
			if (match.Success)
				trigger = match.Value.Trim ();
			else
				return String.Empty;
			
			if (PingFM.Services.Any (s => (s as PingFMServiceItem).Trigger == trigger))
				return trigger;
			else
				return String.Empty;
		}
		
		string MessageWithoutTrigger (string message)
		{
			string trigger = FindTrigger (message);
			return message.Substring (trigger.Length).Trim ();
		}
	}
}
