/* Notifications.cs 
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

using Mono.Addins;

using Do.Platform;

namespace ImageShack
{
	public class UploadNotification : Notification
	{
		static readonly string message_title = AddinManager.CurrentLocalizer.GetString ("ImageShack");
		static readonly string upload_message = AddinManager.CurrentLocalizer.GetString ("Do is uploading your image... Please wait a moment...");
		
		public UploadNotification (string icon) 
			: base (message_title, upload_message, icon)
		{
		}
	}
	
	public class GeneralErrorNotification : Notification
	{
		static readonly string message_title = AddinManager.CurrentLocalizer.GetString ("ImageShack");
		static readonly string error_message = AddinManager.CurrentLocalizer.GetString ("Unable to upload image to ImageShack at this time.");
		
		public GeneralErrorNotification () 
			: base (message_title, error_message, "")
		{
		}
	}
	
	public class InvalidFileNotification : Notification
	{
		static readonly string message_title = AddinManager.CurrentLocalizer.GetString ("ImageShack");
		
		public InvalidFileNotification (string message) 
			: base (message_title, message, "")
		{
		}
	}
}
