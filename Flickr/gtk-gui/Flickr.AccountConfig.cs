// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Flickr {
    
    
    public partial class AccountConfig {
        
        private Gtk.VBox vbox1;
        
        private Gtk.Frame frame1;
        
        private Gtk.Alignment GtkAlignment;
        
        private Gtk.Alignment alignment1;
        
        private Gtk.VBox vbox2;
        
        private Gtk.Label status_lbl;
        
        private Gtk.VBox vbox3;
        
        private Gtk.Label label4;
        
        private Gtk.Label auth_lbl;
        
        private Gtk.HButtonBox hbuttonbox1;
        
        private Gtk.Button auth_btn;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget Flickr.AccountConfig
            Stetic.BinContainer.Attach(this);
            this.Name = "Flickr.AccountConfig";
            // Container child Flickr.AccountConfig.Gtk.Container+ContainerChild
            this.vbox1 = new Gtk.VBox();
            this.vbox1.Name = "vbox1";
            this.vbox1.Spacing = 6;
            this.vbox1.BorderWidth = ((uint)(8));
            // Container child vbox1.Gtk.Box+BoxChild
            this.frame1 = new Gtk.Frame();
            this.frame1.Name = "frame1";
            this.frame1.ShadowType = ((Gtk.ShadowType)(0));
            // Container child frame1.Gtk.Container+ContainerChild
            this.GtkAlignment = new Gtk.Alignment(0F, 0F, 1F, 1F);
            this.GtkAlignment.Name = "GtkAlignment";
            this.GtkAlignment.LeftPadding = ((uint)(12));
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            this.alignment1 = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.alignment1.Name = "alignment1";
            this.alignment1.LeftPadding = ((uint)(12));
            // Container child alignment1.Gtk.Container+ContainerChild
            this.vbox2 = new Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 6;
            // Container child vbox2.Gtk.Box+BoxChild
            this.status_lbl = new Gtk.Label();
            this.status_lbl.Name = "status_lbl";
            this.status_lbl.LabelProp = Mono.Addins.AddinManager.CurrentLocalizer.GetString("Do needs your authorization in order to upload photos to your flickr account. Press the \"Authorize\" button to open a web browser and give Do authorization. ");
            this.status_lbl.Wrap = true;
            this.vbox2.Add(this.status_lbl);
            Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(this.vbox2[this.status_lbl]));
            w1.Position = 0;
            w1.Expand = false;
            w1.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.vbox3 = new Gtk.VBox();
            this.vbox3.Name = "vbox3";
            this.vbox3.Spacing = 6;
            this.vbox2.Add(this.vbox3);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.vbox2[this.vbox3]));
            w2.Position = 1;
            // Container child vbox2.Gtk.Box+BoxChild
            this.label4 = new Gtk.Label();
            this.label4.Name = "label4";
            this.vbox2.Add(this.label4);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.vbox2[this.label4]));
            w3.Position = 2;
            w3.Expand = false;
            w3.Fill = false;
            this.alignment1.Add(this.vbox2);
            this.GtkAlignment.Add(this.alignment1);
            this.frame1.Add(this.GtkAlignment);
            this.auth_lbl = new Gtk.Label();
            this.auth_lbl.Name = "auth_lbl";
            this.auth_lbl.LabelProp = Mono.Addins.AddinManager.CurrentLocalizer.GetString("<b>Account</b>");
            this.auth_lbl.UseMarkup = true;
            this.frame1.LabelWidget = this.auth_lbl;
            this.vbox1.Add(this.frame1);
            Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.vbox1[this.frame1]));
            w7.Position = 0;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbuttonbox1 = new Gtk.HButtonBox();
            this.hbuttonbox1.Name = "hbuttonbox1";
            // Container child hbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
            this.auth_btn = new Gtk.Button();
            this.auth_btn.CanFocus = true;
            this.auth_btn.Name = "auth_btn";
            this.auth_btn.UseUnderline = true;
            // Container child auth_btn.Gtk.Container+ContainerChild
            Gtk.Alignment w8 = new Gtk.Alignment(0.5F, 0.5F, 0F, 0F);
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            Gtk.HBox w9 = new Gtk.HBox();
            w9.Spacing = 2;
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Image w10 = new Gtk.Image();
            w10.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-yes", Gtk.IconSize.Menu, 16);
            w9.Add(w10);
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Label w12 = new Gtk.Label();
            w12.LabelProp = Mono.Addins.AddinManager.CurrentLocalizer.GetString("_Authorize");
            w12.UseUnderline = true;
            w9.Add(w12);
            w8.Add(w9);
            this.auth_btn.Add(w8);
            this.hbuttonbox1.Add(this.auth_btn);
            Gtk.ButtonBox.ButtonBoxChild w16 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox1[this.auth_btn]));
            w16.Expand = false;
            w16.Fill = false;
            this.vbox1.Add(this.hbuttonbox1);
            Gtk.Box.BoxChild w17 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbuttonbox1]));
            w17.Position = 1;
            w17.Expand = false;
            w17.Fill = false;
            this.Add(this.vbox1);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
            this.auth_btn.Clicked += new System.EventHandler(this.OnAuthBtnClicked);
        }
    }
}
