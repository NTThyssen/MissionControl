
// This file has been generated by the GUI designer. Do not modify.
namespace MissionControl.UI
{
	public partial class SessionSettingsView
	{
		private global::Gtk.Button button1;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget MissionControl.UI.NewSessionView
			this.Name = "MissionControl.UI.NewSessionView";
			this.Title = global::Mono.Unix.Catalog.GetString("NewSessionView");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child MissionControl.UI.NewSessionView.Gtk.Container+ContainerChild
			this.button1 = new global::Gtk.Button();
			this.button1.CanFocus = true;
			this.button1.Name = "button1";
			this.button1.UseUnderline = true;
			this.button1.Label = global::Mono.Unix.Catalog.GetString("GtkButton");
			this.Add(this.button1);
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 300;
			this.Show();
			this.button1.Clicked += new global::System.EventHandler(this.OnButton1Clicked);
		}
	}
}