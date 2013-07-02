using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreLocation;
using System.Collections.Generic;

namespace FrameWorkApp
{
	public partial class RawGPS
	{
		SDMFileManager fileManager = new SDMFileManager();

		public List<CLLocation> listOfTripLocationCoordinates { get; set; }
		CLLocationManager commonLocationManager;

		public RawGPS ()
		{
			commonLocationManager = new CLLocationManager ();
			listOfTripLocationCoordinates = new List<CLLocation> ();
		}

		public double getCurrentUserLatitude ()
		{
			commonLocationManager.DesiredAccuracy = CLLocation.AccuracyBest;
			if (CLLocationManager.LocationServicesEnabled) {
				commonLocationManager.StartUpdatingLocation ();
			}
			commonLocationManager.StopUpdatingLocation ();
			return commonLocationManager.Location.Coordinate.Latitude;
		}

		public double getCurrentUserLongitude ()
		{
			commonLocationManager.DesiredAccuracy = CLLocation.AccuracyBest;
			if (CLLocationManager.LocationServicesEnabled) {
				commonLocationManager.StartUpdatingLocation ();
			}
			commonLocationManager.StopUpdatingLocation ();
			return commonLocationManager.Location.Coordinate.Longitude;
		}

		public double getSpeedInMetersPerSecondUnits ()
		{
			commonLocationManager.DesiredAccuracy = CLLocation.AccuracyBest;
			if (CLLocationManager.LocationServicesEnabled) {
				commonLocationManager.StartUpdatingLocation ();
			}
			return commonLocationManager.Location.Speed;
		}

		public double convertToKilometersPerHour (double metersPerSecond)
		{
			return (metersPerSecond / 1000.0) * 3600;
		}

		public double convertMetersToKilometers (double meters)
		{
			return meters / 1000.0;
		}

		public void createCoordinatesWhenHeadingChangesToAddToList ()
		{
			commonLocationManager.DesiredAccuracy = CLLocation.AccuracyBest;
			commonLocationManager.HeadingFilter = 30;
			CLLocation temp;

			if (CLLocationManager.LocationServicesEnabled) {
				commonLocationManager.StartUpdatingLocation ();
			}
			if (CLLocationManager.HeadingAvailable) {
				commonLocationManager.StartUpdatingHeading ();
			}

			commonLocationManager.UpdatedHeading += (object sender, CLHeadingUpdatedEventArgs e) => {
				Double lat= this.getCurrentUserLatitude ();
				Double longt=this.getCurrentUserLongitude ();
				temp = new CLLocation (lat, longt);
				listOfTripLocationCoordinates.Add (temp);
				//Add to Temp File
				fileManager.addLocationToTripDistanceFile(new CLLocationCoordinate2D(lat, longt));
			};
		}

		public double CalculateDistanceTraveled (List<CLLocation> locations)
		{
			double distance = 0;
			double temp = 0;
			for (int i = 0; i<locations.Count; i++) {
				if (i + 1 < locations.Count) {
					temp = locations [i].DistanceFrom (locations [i + 1]);
					distance = distance + temp;
				}
			}
			return distance;
		}
	}
}

