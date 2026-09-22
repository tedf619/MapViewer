# MapViewer
## A simple .Net 10 Windows Forms app that supports OpenStreetMap, Google Maps and many other providers

The app is hard-coded to show an OpenStreetMap view of the continental USA. By changing a few longitude/latitude variables it can show any area on the planet.
You can zoom the map using the mouse wheel. To pan the map, click and drag with the left mouse button. The following figures show MapViewer in action with different levels of zoom.

<img width="852" height="496" alt="image" src="https://github.com/user-attachments/assets/cca092d9-80c9-43c3-9eb2-55c7c74b6d0f" />

*Figure 1* - The map at zoom level 4.
<br><br>

<img width="852" height="496" alt="image" src="https://github.com/user-attachments/assets/e4f02dff-452a-4944-97ca-b8cb3b52b1e3" />

*Figure 2* - The map at zoom level 8.


The hard lifting is done by GMapControl, an open source component available on NuGet in the GMap.Net.WinForms package. It's based on the project at [https://github.com/judero01col/GMap.NET/](https://github.com/judero01col/GMap.NET/). GMapControl is a UserControl, so it can be added to the Visual Studio Toolbox, dropped on a Form and manipulated using the Forms Designer Properties. GMapControl handles everything you typically need with a map, including downloading, rendering, panning and scaling. 
<br>
By default, GMapControl internally uses SQLite to manage tiles. In case you're new to SQLite, it's a SQL-based database system that stores data in a single file. The GitHub repo referenced about has code to handle other database types, such as PostgreSQL, SQL Server and MySQL. I've only tried SQLite. 
<br>
GMapControl supports several map providers, such as OpenStreetMap, Google Maps, and others. I chose OpenStreetMap because it doesn't require an API key to access it, unlike most other providers. You specify the provider you want during GMapControl initialization with a single line of code:

```gMapControl.MapProvider = OpenStreetMapProvider.Instance;```

The USA map was downloaded from OpenStreetMap as a database of tiles, which form a mosaic for the requested geographic area. How much area a tile covers depends on the zoom level. 
I downloaded tiles for zoom levels from 4 to 8. At zoom level 4, a tile covers an area of roughly 1,900 x 1,900 km. At zoom level 8, a tile covers an area of roughly 120 x 120 km.

## UI Layout

The app consists of a single form that hosts GMapControl as a docked conntrol, as shown in the following figure.

<img width="653" height="472" alt="image" src="https://github.com/user-attachments/assets/440168d5-9272-4281-bd7b-c375c43a084c" />

*Figure 3* - The UI layout with docked controls.

To achieve the correct layout, Panel StatusBar panel needs to be added to the form first, then GMapControl. For controls with Dock=Fill to fill space correctly, they must always be added last.
The following code shows how GMapControl is initialized.

```csharp
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

  void CreateTileCache()  // see next listing
  {
    //...
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

   //...
  }
 }
```
*Listing 1* - The code to initialize GMapControl.

## Creating the Local Cache of Map Tiles

GMapControl needs tiles to render a map. It can retrieve these tiles either from a server or a local file cache. It isn't practical to download tiles from a server every time you run the app, so I downloaded the tiles once and add the tile file to the project. As mentioned earlier, the file contains a SQLite database. I named the file "US Map Tiles.gmdb" and included it in the Visual Studio project, as shown in the next figure.

<img width="809" height="371" alt="image" src="https://github.com/user-attachments/assets/ef057f28-1ed1-42a0-b687-4cfa549576ae" />

*Figure 4* - The database file containing the map tiles.

As you can see, the file is configured to be copied to the executable folder at runtime. During initialization, the method CreateTileCache is called. It copies "US Map Tiles.gmdb" from the executable file to the folder where GMapControl will find it, which is the folder

```<Project Folder>\bin\Debug\net10.0-windows\TileDBv5\en```

During copying, the file is also renamed to Data.gmdb, which is what GMapControl expects. The code for CreateTileCache looks like this:

```
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
```

*Listing 2* - Copying "US Map Tiles.gmdb" to the local cache.

## Downloading Tiles

In the project, I included tiles with zoom level from 4 to 8, producing a 62MB database file. It contains about 1700 tiles. Going to higher zoom levels causes the file to increase exponentially in both tile count and size. At zoom level 9, the file includes over 5000 tiles and the size jumps to 120MB, which exceeds the max file size allowed with ordinary GitHub projects. You can however configure MapViewer to show zoom levels or 9 or higher (OpenStreetMap permitting). To do so, you need to first download the tiles. The following code does the trick:

```csharp
using GMap.NET;
using GMap.NET.MapProviders;

namespace MapViewer;

public partial class FormMain : Form
{
  const double MinLat = 24.5;   // Southern US border
  const double MaxLat = 53.0;   // Northern US border
  const double MinLng = -125.0; // West Coast
  const double MaxLng = -66.5;  // East Coast

  const int MinZoom = 4, MaxZoom = 9;  // 19 is the max recommended

  string executablePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath)!;
  RectLatLng mapArea;

  public FormMain()
  {
    InitializeComponent();

    mapArea = RectLatLng.FromLTRB(MinLng, MaxLat, MaxLng, MinLat);
    Shown += FormMain_Shown;
  }

  void FormMain_Shown(object? sender, EventArgs e)
  {
    DownloadUsTiles();
  }

  void DownloadUsTiles()
  {
    gMapControl.MapProvider = OpenStreetMapProvider.Instance;

    // define the cache location for downloaded tiles
    gMapControl.CacheLocation = executablePath;  // GMapControl internally adds "TileDBv5\en\"

    GMapProvider provider = OpenStreetMapProvider.Instance;
    GMaps.Instance.Mode = AccessMode.ServerAndCache;

    // download tiles for the entire US area at zoom levels 4 to 9
    for (int zoom = MinZoom; zoom <= MaxZoom; zoom++)
    {
      // Create a prefetcher for this specific zoom level and area
      var prefetcher = new TilePrefetcher
      {
        Owner = this,
        Text = $"Downloading tiles for zoom level {zoom}...",
        ShowCompleteMessage = false,
        Icon = null
      };

      // start the download for the area at this zoom level
      prefetcher.Start(mapArea, zoom, provider, sleep: 100, retry: 3);
      prefetcher.Owner = null; // to prevent component from being disposed when the form closes
    }

    MessageBox.Show(this, "The map has been cached. You can now close the app.", "Download complete", MessageBoxButtons.OK);
    Application.Exit();
  }
}
```

*Listing 3* - The code to download tiles for zoom levels 4 through 9.

Downloading works reliably up to about zoom level 18 or 19. At zoom level 19, tiles cover just over 50 x 50 meters. At higher levels, OpenStreetMap starts returning throttling or HTTP 404 errors. 
To get maps of other geographic areas, just change the constants MinLat, MaxLat, MinLng and MaxLng as appropriate.



