using GMap.NET;
using GMap.NET.MapProviders;

namespace MapViewer;

public partial class FormMain : Form
{
  const double UsMinLat = 24.5;   // Southern border
  const double UsMaxLat = 53.0;   // Northern border
  const double UsMinLng = -125.0; // West Coast
  const double UsMaxLng = -66.5;  // East Coast

  const int MinZoom = 4, MaxZoom = 8;  // values appropriate for the continental US map

  string executablePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath)!;

  RectLatLng continentalUsArea;

  public FormMain()
  {
    InitializeComponent();

    continentalUsArea = RectLatLng.FromLTRB(UsMinLng, UsMaxLat, UsMaxLng, UsMinLat);

    CreateTileCache(); // copies our map tiles to a local cache for GMap to access

    InitializeMap();
  }

  // Copy the US map tiles included in the project to the GMap cache.
  // The cache is only created on the first run of this app.
  void CreateTileCache()
  {
    string tileCache = System.IO.Path.Combine(executablePath, "TileDBv5", "en", "Data.gmdb");
    if (File.Exists(tileCache)) return;

    string? tileCacheFolder = System.IO.Path.GetDirectoryName(tileCache);
    Directory.CreateDirectory(tileCacheFolder!);
    string usMapTiles = System.IO.Path.Combine(executablePath, "US Map Tiles.gmdb");
    File.Copy(usMapTiles, tileCache);
  }

  void InitializeMap()
  {
    // basic properties
    gMapControl.DragButton = MouseButtons.Left;
    gMapControl.CanDragMap = true;
    gMapControl.MapProvider = OpenStreetMapProvider.Instance;
    gMapControl.MinZoom = MinZoom;
    gMapControl.MaxZoom = MaxZoom;
    gMapControl.Zoom = 4; // to show the entire US at startup
    gMapControl.ShowCenter = false; // hide the red crosshair in map's center

    // configure missing tiles
    gMapControl.EmptyTileText = string.Empty;
    gMapControl.FillEmptyTiles = false;
    gMapControl.EmptyTileColor = Color.Black;
    gMapControl.ShowTileGridLines = false;

    // load tiles from the local cache
    gMapControl.CacheLocation = executablePath;
    GMaps.Instance.Mode = AccessMode.CacheOnly;

    // center the map
    double midLat = (UsMinLat + UsMaxLat) / 2.0;
    double midLng = (UsMinLng + UsMaxLng) / 2.0;
    gMapControl.Position = new PointLatLng(midLat, midLng);

    // Constrain the map to US. Blank tiles along the map borders will still be visible
    gMapControl.BoundsOfMap = continentalUsArea;

    GMapControl_OnMapZoomChanged(); // Update the status bar with the initial zoom level
    labelStatusBar.Text += "                  Use the mouse wheel to zoom the map";
  }

  void GMapControl_OnMapZoomChanged()
  {
    string text = $"Zoom level: {gMapControl.Zoom}";

    if (gMapControl.Zoom == MinZoom)
      text += " (Minimum level supported by currently cached tiles)";
    else if (gMapControl.Zoom == MaxZoom)
      text += " (Maximum level supported by currently cached tiles)";

    labelStatusBar.Text = text;
  }
}

