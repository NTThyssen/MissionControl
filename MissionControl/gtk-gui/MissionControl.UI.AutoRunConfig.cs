namespace MissionControl.UI
{
    public partial class AutoRunConfig
    {
        protected virtual void Build()
        {
            global::Stetic.Gui.Initialize(this);
            // Widget MissionControl.UI.PlotView
            this.Name = "MissionControl.UI.AutoRunConfig";
            this.Title = global::Mono.Unix.Catalog.GetString("AutoRunConfig");
            this.WindowPosition = ((global::Gtk.WindowPosition)(4));
            if ((this.Child != null))
            {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 400;
            this.DefaultHeight = 300;
            this.Show();
        }
    }
}