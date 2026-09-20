namespace MapViewer
{
  partial class FormMain
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      panelStatusBar = new Panel();
      labelStatusBar = new Label();
      gMapControl = new GMap.NET.WindowsForms.GMapControl();
      panelStatusBar.SuspendLayout();
      SuspendLayout();
      // 
      // panelStatusBar
      // 
      panelStatusBar.BorderStyle = BorderStyle.Fixed3D;
      panelStatusBar.Controls.Add(labelStatusBar);
      panelStatusBar.Dock = DockStyle.Bottom;
      panelStatusBar.Location = new Point(0, 438);
      panelStatusBar.Name = "panelStatusBar";
      panelStatusBar.Size = new Size(850, 26);
      panelStatusBar.TabIndex = 1;
      // 
      // labelStatusBar
      // 
      labelStatusBar.Dock = DockStyle.Fill;
      labelStatusBar.Location = new Point(0, 0);
      labelStatusBar.Name = "labelStatusBar";
      labelStatusBar.Size = new Size(846, 22);
      labelStatusBar.TabIndex = 0;
      labelStatusBar.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // gMapControl
      // 
      gMapControl.Bearing = 0F;
      gMapControl.Dock = DockStyle.Fill;
      gMapControl.EmptyTileColor = Color.Black;
      gMapControl.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
      gMapControl.Location = new Point(0, 0);
      gMapControl.MaxZoom = 2;
      gMapControl.MinZoom = 2;
      gMapControl.MouseWheelZoomEnabled = true;
      gMapControl.Name = "gMapControl";
      gMapControl.RetryLoadTile = 0;
      gMapControl.SelectedAreaFillColor = Color.FromArgb(33, 65, 105, 225);
      gMapControl.ShowTileGridLines = false;
      gMapControl.Size = new Size(850, 438);
      gMapControl.TabIndex = 2;
      gMapControl.Zoom = 0D;
      gMapControl.OnMapZoomChanged += GMapControl_OnMapZoomChanged;
      // 
      // FormMain
      // 
      AutoScaleDimensions = new SizeF(7F, 15F);
      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(850, 464);
      Controls.Add(gMapControl);
      Controls.Add(panelStatusBar);
      Name = "FormMain";
      StartPosition = FormStartPosition.CenterScreen;
      Text = "MapViewer - The Continental USA";
      panelStatusBar.ResumeLayout(false);
      ResumeLayout(false);
    }

    #endregion
    private Panel panelStatusBar;
    private Label labelStatusBar;
    private GMap.NET.WindowsForms.GMapControl gMapControl;
  }
}
