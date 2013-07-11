using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.CoreLocation;
using System.Collections.Generic;
using Google.Maps;

namespace FrameWorkApp
{
	public partial class GoogleMapScreen : UIViewController
	{
		LoadingOverlay loadingOverlay;
		Google.Maps.MapView mapView;
		private Event[] markersToAdd;
		private CLLocationCoordinate2D[] pathMarkers;
		SDMFileManager fileManager = new SDMFileManager();

		public GoogleMapScreen (IntPtr handle) : base (handle)
		{
			//From Internal Data Structures
			//markersToAdd = (CLLocationCoordinate2D[])StopScreenn.coordList.ToArray (typeof(CLLocationCoordinate2D));

			//From File

			//Temporarily Disabled....########################
			//markersToAdd = StopScreenn.fileManager.readDataFromTripEventFile ();
			//#############################

			pathMarkers = new CLLocationCoordinate2D[StopScreen.listOfTripLocationCoordinates.Count];
			for(int i = 0; i< StopScreen.listOfTripLocationCoordinates.Count; i++){
				CLLocation newClLocation = StopScreen.listOfTripLocationCoordinates[i];
				pathMarkers[i] = new CLLocationCoordinate2D(newClLocation.Coordinate.Latitude, newClLocation.Coordinate.Longitude);
			}
			//From Internal Data Structures
			//markersToAdd = (CLLocationCoordinate2D[])StopScreenn.coordList.ToArray (typeof(CLLocationCoordinate2D));

			//EVENTS
			markersToAdd = TripSummaryScreen.getEvents ();
		}

		public GoogleMapScreen (IntPtr handle,Event[] markerLocationsToAdd) : base (handle)
		{
			markersToAdd = markerLocationsToAdd;
		}

		//Comment
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public void addMarkerAtLocationsWithGoogleMarker(Event[] position){

			foreach(Event newEve in position){
				var newMarker = new Marker(){
					Title = newEve.Text,
					Position = newEve.Location,
					Icon = Google.Maps.Marker.MarkerImage(newEve.Color),
					Map = mapView
				};
			}
		}

		/*
		public void addMarkerAtLocationsWithCustomMarker(CLLocationCoordinate2D position, string title, string snippet, UIImage markerIcon){
			var newMarker = new Marker(){
				Title = title,
				Snippet = snippet,
				Position = position,
				Icon = markerIcon,
				Map = mapView
			};

		}
		*/

		public int getZoomLevel(double minLat, double maxLat, double minLong, double maxLong, float mapWidth, float mapHeight){
			float mapDisplayDimension = Math.Min (mapHeight, mapWidth);
			int earthRadiusinKm = 6371;
			double degToRadDivisor = 57.2958;
			double distanceToBeCovered = (earthRadiusinKm * Math.Acos (Math.Sin(minLat / degToRadDivisor) * Math.Sin(maxLat / degToRadDivisor) + 
			                                            (Math.Cos (minLat / degToRadDivisor)  * Math.Cos (maxLat / degToRadDivisor) * Math.Cos ((maxLong / degToRadDivisor) - (minLong / degToRadDivisor)))));

			double zoomLevel = Math.Floor (8 - Math.Log(1.6446 * distanceToBeCovered / Math.Sqrt(2 * (mapDisplayDimension * mapDisplayDimension))) / Math.Log (2));
			if(minLat == maxLat && minLong == maxLong){zoomLevel = 15;}

			return (int) zoomLevel;
		}

		public void cameraAutoZoomAndReposition(Event[] eventMarkers){
			double minimumLongitudeInGoogle = 180.0f;
			double maximumLongitudeInGoogle = -180.0f;
			double minimumLatitudeInGoogle = 90.0f;
			double maximumLatitudeInGoogle = -90.0f;
			foreach (Event currentMarker in eventMarkers) {
				maximumLongitudeInGoogle = Math.Max (maximumLongitudeInGoogle, currentMarker.Location.Longitude);
				minimumLongitudeInGoogle = Math.Min (minimumLongitudeInGoogle, currentMarker.Location.Longitude);
				maximumLatitudeInGoogle = Math.Max (maximumLatitudeInGoogle, currentMarker.Location.Latitude);
				minimumLatitudeInGoogle = Math.Min (minimumLatitudeInGoogle, currentMarker.Location.Latitude);
			}
			CLLocationCoordinate2D northWestBound = new CLLocationCoordinate2D (maximumLatitudeInGoogle, minimumLongitudeInGoogle);
			CLLocationCoordinate2D southEastBound = new CLLocationCoordinate2D (minimumLatitudeInGoogle, maximumLongitudeInGoogle);
			mapView.MoveCamera(CameraUpdate.FitBounds(new CoordinateBounds(northWestBound, southEastBound)));	
			float desiredZoomlevel = (float) (getZoomLevel (minimumLatitudeInGoogle,maximumLatitudeInGoogle,minimumLongitudeInGoogle,maximumLongitudeInGoogle,mapViewOutlet.Frame.Size.Width,mapViewOutlet.Frame.Size.Height));
			mapView.MoveCamera (CameraUpdate.ZoomToZoom(desiredZoomlevel));
		}

		public override void LoadView ()
		{
			base.LoadView ();
			CameraPosition camera = CameraPosition.FromCamera (37.797865, -122.402526,0);
			mapView = Google.Maps.MapView.FromCamera (RectangleF.Empty, camera);


		}

		public override void ViewDidLoad ()
		{

			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			cameraAutoZoomAndReposition (this.markersToAdd);
			loadingOverlay = new LoadingOverlay (UIScreen.MainScreen.Bounds);
			this.View.Add (loadingOverlay);


		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			this.NavigationController.SetNavigationBarHidden (false, animated);



		}

		public override void ViewDidAppear (bool animated)
		{


			base.ViewDidAppear (animated);

			mapView.MyLocationEnabled = true;
			addMarkerAtLocationsWithGoogleMarker (this.markersToAdd);
			View = mapView;

			//Stuff from ViewWillAppear
			//Get path for the trip that will be shown on the map
			List<CLLocationCoordinate2D> googlePathCoordinates = new List<CLLocationCoordinate2D> ();
			foreach (CLLocationCoordinate2D coord in pathMarkers) {
				googlePathCoordinates.Add(coord);
			}

			GoogleMapsDirectionService gmds = new GoogleMapsDirectionService (googlePathCoordinates);
			List<Polyline> polylinesToPlot = gmds.performGoogleDirectionServiceApiCallout ();
			var startMarker = new Marker(){
				Title = "Start",
				Position = polylinesToPlot[0].Path.CoordinateAtIndex(0),
				Icon = Google.Maps.Marker.MarkerImage(UIColor.Green),
				Map = mapView
			};
			foreach (Polyline line in polylinesToPlot) {
				line.StrokeWidth = 10;
				line.StrokeColor = UIColor.FromRGBA(0f, 25/255f, 1f, 0.5f);
				line.Map = this.mapView;
			}
			var endMarker = new Marker(){
				Title = "End",
				Position = polylinesToPlot[polylinesToPlot.Count-1].Path.CoordinateAtIndex(polylinesToPlot[polylinesToPlot.Count-1].Path.Count-1),
				Icon = Google.Maps.Marker.MarkerImage(UIColor.Red),
				Map = mapView
			};

			mapView.StartRendering ();
			loadingOverlay.Hide ();
		}

		public override void ViewWillDisappear (bool animated)
		{	
			mapView.StopRendering ();
			this.NavigationController.SetNavigationBarHidden (true, animated);
			base.ViewWillDisappear (animated);
		}
	}
}