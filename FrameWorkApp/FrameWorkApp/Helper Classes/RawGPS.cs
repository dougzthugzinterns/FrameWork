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

		public List<CLLocation> listOfRawGPSTripLocationCoordinates { get; set; }
		CLLocationManager commonLocationManager;

		public RawGPS ()
		{
			commonLocationManager = new CLLocationManager ();
			listOfRawGPSTripLocationCoordinates = new List<CLLocation> ();
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
			double currentSpeed = 0;
			commonLocationManager.DesiredAccuracy = CLLocation.AccuracyBest;
			if (CLLocationManager.LocationServicesEnabled) {
				commonLocationManager.StartUpdatingLocation ();
			}
			currentSpeed = commonLocationManager.Location.Speed;
			if(currentSpeed < 0){
				currentSpeed = 0;
			}
			return currentSpeed;
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
			CLLocation newCoordinate;

			if (CLLocationManager.LocationServicesEnabled) {
				commonLocationManager.StartUpdatingLocation ();
			}
			if (CLLocationManager.HeadingAvailable) {
				commonLocationManager.StartUpdatingHeading ();
			}

			commonLocationManager.UpdatedHeading += (object sender, CLHeadingUpdatedEventArgs e) => {
				Double lattitude= this.getCurrentUserLatitude ();
				Double longitude=this.getCurrentUserLongitude ();
				newCoordinate = new CLLocation (lattitude, longitude);
				listOfRawGPSTripLocationCoordinates.Add (newCoordinate);
				//Add to Temp File
				fileManager.addLocationToTripDistanceFile(new CLLocationCoordinate2D(lattitude, longitude));
			};
		}

		public void stopGPSReadings(){
			commonLocationManager.StopUpdatingLocation ();
		}

		public double CalculateDistanceTraveled (List<CLLocation> locations)
		{
			double distance = 0;
			double partialDistance = 0;
			for (int i = 0; i<locations.Count; i++) {
				if (i + 1 < locations.Count) {
					partialDistance = locations [i].DistanceFrom (locations [i + 1]);
					distance = distance + partialDistance;
				}
			}
			return distance;
		}
	}
}