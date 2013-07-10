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
		Google.Maps.MapView mapView;
		private CLLocationCoordinate2D[] markersToAdd;
		private CLLocationCoordinate2D[] pathMarkers;

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
			markersToAdd = TripSummaryScreen.importedGpsEvents;
		}


		public GoogleMapScreen (IntPtr handle,CLLocationCoordinate2D[] markerLocationsToAdd) : base (handle)
		{
			markersToAdd = markerLocationsToAdd;
		}
		//COmment
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public void addMarkerAtLocationsWithGoogleMarker(CLLocationCoordinate2D[] position){

			foreach(CLLocationCoordinate2D newPos in position){
				var newMarker = new Marker(){
					Title = "Incident Occured",
					Position = newPos,
					Icon = Google.Maps.Marker.MarkerImage(UIColor.Red),
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

		public void cameraAutoZoomAndReposition(CLLocationCoordinate2D[] markerPositions){
			const double minimumLongitudeInGoogle = 180.0f;
			const double maximumLongitudeInGoogle = -180.0f;
			const double minimumLatitudeInGoogle = 90.0f;
			const double maximumLatitudeInGoogle = -90.0f;
			foreach (CLLocationCoordinate2D currentMarkerPosition in markerPositions) {
				maximumLongitudeInGoogle = Math.Max (maximumLongitudeInGoogle, currentMarkerPosition.Longitude);
				minimumLongitudeInGoogle = Math.Min (minimumLongitudeInGoogle, currentMarkerPosition.Longitude);
				maximumLatitudeInGoogle = Math.Max (maximumLatitudeInGoogle, currentMarkerPosition.Latitude);
				minimumLatitudeInGoogle = Math.Min (minimumLatitudeInGoogle, currentMarkerPosition.Latitude);
			}
			CLLocationCoordinate2D northWestBound = new CLLocationCoordinate2D (maximumLatitudeInGoogle, minimumLongitudeInGoogle);
			CLLocationCoordinate2D southEastBound = new CLLocationCoordinate2D (minimumLatitudeInGoogle, maximumLongitudeInGoogle);
			mapView.MoveCamera(CameraUpdate.FitBounds(new CoordinateBounds(northWestBound, southEastBound)));	
			float desiredZoomlevel = (float) (getZoomLevel (minimumLatitudeInGoogle,maximumLatitudeInGoogle,minimumLongitudeInGoogle,maximumLongitudeInGoogle,mapViewOutlet.Frame.Size.Width,mapViewOutlet.Frame.Size.Height)));
			mapView.MoveCamera (CameraUpdate.ZoomToZoom(desiredZoomlevel);

		}

		public override void LoadView ()
		{
			base.LoadView ();
			CameraPosition camera = CameraPosition.FromCamera (37.797865, -122.402526,0);
			mapView = Google.Maps.MapView.FromCamera (RectangleF.Empty, camera);
			mapView.MyLocationEnabled = true;
			addMarkerAtLocationsWithGoogleMarker (this.markersToAdd);
			View = mapView;
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.
			cameraAutoZoomAndReposition (this.markersToAdd);
		}
		public override void ViewWillAppear (bool animated)
		{
			//Get path for the trip that will be shown on the map
			List<CLLocationCoordinate2D> googlePathCoordinates = new List<CLLocationCoordinate2D> ();
			foreach (CLLocationCoordinate2D coord in pathMarkers) {
				googlePathCoordinates.Add(coord);
			}

			GoogleMapsDirectionService gmds = new GoogleMapsDirectionService (googlePathCoordinates);
			List<Polyline> polylinesToPlot = gmds.performGoogleDirectionServiceApiCallout ();
			foreach (Polyline line in polylinesToPlot) {
				line.StrokeWidth = 10;
				line.Map = this.mapView;
			}

			base.ViewWillAppear (animated);
			this.NavigationController.SetNavigationBarHidden (false, animated);
			mapView.StartRendering ();
		}

		public override void ViewWillDisappear (bool animated)
		{	
			mapView.StopRendering ();
			this.NavigationController.SetNavigationBarHidden (true, animated);
			base.ViewWillDisappear (animated);
		}
	}
}

