// WindowTorrentView.cs
//
//GNOME Do is the legal property of its developers. Please refer to the
//COPYRIGHT file distributed with this
//source distribution.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//

using System;
using Gdk;
using Gtk;

using MonoTorrent.Client;
using MonoTorrent.Common;

using Do.UI;

namespace Do.Riptide
{
	public class WindowTorrentView : Gtk.Window
	{
		private GlossyRoundedFrame frame;
		private VBox vbox;
		
		public WindowTorrentView() : base (Gtk.WindowType.Toplevel)
		{
			AppPaintable = true;
			KeepBelow    = true;
			Decorated    = false;
			Resizable    = false;
			SkipPagerHint = true;
			SkipTaskbarHint = true;
			AcceptFocus = false;
			
			SetColormap ();
			
			frame = new GlossyRoundedFrame ();
			frame.DrawFill = frame.DrawFrame = true;
			frame.FillColor = frame.FrameColor = new Gdk.Color ((byte) 0, (byte) 0, (byte) 0);
			frame.FillAlpha = frame.FrameAlpha = 0.7;
			frame.Radius = 4;
			Add (frame);
			
			vbox = new VBox ();
			frame.Add (vbox);
			
			ShowAll ();
		}
		
		public void AddTorrent (TorrentManager torrent)
		{
			TorrentDisplay tor      = new TorrentDisplay (torrent);
			tor.TorrentPauseToggle += OnTorrentPauseToggle;
			tor.TorrentStopped     += OnTorrentStopped;
			
			vbox.PackStart (tor);
			tor.ShowAll ();
			ShowAll ();
		}
		
		protected virtual void SetColormap ()
		{
			Gdk.Colormap  colormap;

			colormap = Screen.RgbaColormap;
			if (colormap == null) {
				colormap = Screen.RgbColormap;
				Console.WriteLine ("No alpha support.");
			}
			
			Colormap = colormap;
		}
		
		protected override bool OnExposeEvent (EventExpose evnt)
		{
			Cairo.Context cairo;
			
			using (cairo = Gdk.CairoHelper.Create (GdkWindow)) {
				cairo.Rectangle (evnt.Area.X, evnt.Area.Y, evnt.Area.Width, evnt.Area.Height);
				cairo.Color = new Cairo.Color (1.0, 1.0, 1.0, 0.0);
				cairo.Operator = Cairo.Operator.Source;
				cairo.Paint ();
			}
			return base.OnExposeEvent (evnt);
		}
		
		protected void OnTorrentPauseToggle (TorrentDisplay display, TorrentManager manager)
		{
			if (manager.State == TorrentState.Paused)
				manager.Start ();
			else
				manager.Pause ();
		}
		
		protected void OnTorrentStopped (TorrentDisplay display, TorrentManager manager)
		{
			vbox.Remove (display);
			
			display.Dispose ();
			display = null;
		}
	}
}
				
				
				
				
				
				
				
				
				
				
				
				
				
				
