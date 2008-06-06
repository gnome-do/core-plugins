/* UploadConfig.cs
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
using Gtk;

namespace Flickr
{
	public partial class UploadConfig : Gtk.Bin
	{		
		public UploadConfig()
		{
			this.Build();
			tags_text.Buffer.Text = AccountConfig.Tags;
			tags_text.Buffer.Changed += new EventHandler (OnTagsEdited);
			if (AccountConfig.IsPublic)
					public_radio.Toggle ();
				else
					private_radio.Toggle ();
					
				friends_chk.Active = AccountConfig.FriendsAllowed;
				family_chk.Active = AccountConfig.FamilyAllowed;
		}
		protected void OnTagsEdited (object sender, EventArgs args)
		{
			AccountConfig.Tags = tags_text.Buffer.Text;
		}
		
		protected virtual void OnPublicRadioToggled (object sender, EventArgs e)
		{
			AccountConfig.IsPublic = true;
			friends_chk.Sensitive = false;
			family_chk.Sensitive = false;
		}
		
		protected virtual void OnPrivateRadioToggled (object sender, EventArgs e)
		{
			AccountConfig.IsPublic = false;
			friends_chk.Sensitive = true;
			family_chk.Sensitive = true;
		}

		protected virtual void OnFriendsChkClicked (object sender, EventArgs e)
		{
			AccountConfig.FriendsAllowed = friends_chk.Active;
		}

		protected virtual void OnFamilyChkClicked (object sender, EventArgs e)
		{
			AccountConfig.FamilyAllowed = family_chk.Active;
		}
	}
}