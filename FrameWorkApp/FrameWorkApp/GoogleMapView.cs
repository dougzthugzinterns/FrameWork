using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.CoreLocation;
using Google.Maps;


namespace MapDisplayTest
{
	public partial class GoogleMapView : UIViewController
	{
		private CLLocationCoordinate2D[] markersToAdd;

		public GoogleMapView () : base ("GoogleMapView", null)
		{
			markersToAdd =  new CLLocationCoordinate2D[]{new CLLocationCoordinate2D(37.797865, -122.402526), 
				new CLLocationCoordinate2D(37.897865, -122.502526),
				new CLLocationCoordinate2D(37.997865, -122.602526),
				new CLLocationCoordinate2D(37.697865, -122.302526)};
		}

		public GoogleMapView (CLLocationCoordinate2D[] markerLocationsToAdd) : base ("GoogleMapView", null)
		{
			markersToAdd = markerLocationsToAdd;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		Google.Maps.MapView mapView;



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
			float mapdisplay = Math.Min (mapHeight, mapWidth);
			int earthRadiusinKm = 6371;
			double degToRadDivisor = 57.2958;
			double dist = (earthRadiusinKm * Math.Acos (Math.Sin(minLat / degToRadDivisor) * Math.Sin(maxLat / degToRadDivisor) + 
			                                            (Math.Cos (minLat / degToRadDivisor)  * Math.Cos (maxLat / degToRadDivisor) * Math.Cos ((maxLong / degToRadDivisor) - (minLong / degToRadDivisor)))));
			Console.WriteLine ("Dist: "+dist);
			double zoom = Math.Floor (8 - Math.Log(1.6446 * dist / Math.Sqrt(2 * (mapdisplay * mapdisplay))) / Math.Log (2));
			if(minLat == maxLat && minLong == maxLong){zoom = 11;}
			Console.WriteLine ("Zoom Level:"+zoom);
			return (int) zoom;
		}

		public void cameraAutoZoomAndReposition(CLLocationCoordinate2D[] markerPositions){
			double minLong = 180.0f;
			double maxLong = -180.0f;
			double minLat = 90.0f;
			double maxLat = -90.0f;
			foreach (CLLocationCoordinate2D currentMarkerPosition in markerPositions) {
				maxLong = Math.Max (maxLong, currentMarkerPosition.Longitude);
				minLong = Math.Min (minLong, currentMarkerPosition.Longitude);
				maxLat = Math.Max (maxLat, currentMarkerPosition.Latitude);
				minLat = Math.Min (minLat, currentMarkerPosition.Latitude);
			}
			CLLocationCoordinate2D coord1 = new CLLocationCoordinate2D (maxLat, minLong);
			CLLocationCoordinate2D coord2 = new CLLocationCoordinate2D (minLat, maxLong);
			mapView.MoveCamera(CameraUpdate.FitBounds(new CoordinateBounds(coord1, coord2)));		
			mapView.MoveCamera (CameraUpdate.ZoomToZoom((float) (getZoomLevel (minLat,maxLat,minLong,maxLong,mapViewOutlet.Frame.Size.Width,mapViewOutlet.Frame.Size.Height))));

		}

		public override void LoadView ()
		{
			base.LoadView ();
			CameraPosition camera = CameraPosition.FromCamera (37.797865, -122.402526,0);
			mapView = Google.Maps.MapView.FromCamera (RectangleF.Empty, camera);
			mapView.MyLocationEnabled = true;

			addMarkerAtLocationsWithGoogleMarker (this.markersToAdd);
			cameraAutoZoomAndReposition (this.markersToAdd);
			View = mapView;

		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			mapView.StartRendering ();
		}

		public override void ViewWillDisappear (bool animated)
		{	
			mapView.StopRendering ();
			base.ViewWillDisappear (animated);
		}
	}
}

