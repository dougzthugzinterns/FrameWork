using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreMotion;
using MonoTouch.CoreLocation;

namespace FrameWorkApp
{
	public partial class StopScreenn : UIViewController
	{
		public static SDMFileManager fileManager = new SDMFileManager ();
		public static List<CLLocation> listOfTripLocationCoordinates = new List<CLLocation> ();
		RawGPS rawGPS = new RawGPS ();
		public static double distanceTraveledForCurrentTrip = 0;
		public static double numberHardStarts = 0;
		public static double numberHardStops = 0;
		public static double numberHardAccel = 0;
		const double STARTSPEEDTHRESHOLD = 5;

		public StopScreenn (IntPtr handle) : base (handle)
		{
		}

		public static ArrayList coordList = new ArrayList ();
		double currentMaxAvgAccel;
		double avgaccel;
		double threshold = .35;
		double klowpassfilterfactor = .2;
		//threshold for erratic behavior in G's
		int eventcount = 0;
		//number of events
		bool eventInProgress = false;
		//true if event is in progress
		CLLocationCoordinate2D currentCoord = new CLLocationCoordinate2D ();
		//container for current location
		//list of behavior event coordinates
		private CMMotionManager _motionManager;

		// Returns current Latitude reading with accuracy within 10m
		public double getCurrentLatitude ()
		{
			CLLocationManager myLocMan = new CLLocationManager ();
			myLocMan.DesiredAccuracy = 10;
			if (CLLocationManager.LocationServicesEnabled) {
				myLocMan.StartUpdatingLocation ();
			}
			double latitude = myLocMan.Location.Coordinate.Latitude;
			myLocMan.StartUpdatingLocation ();
			return latitude;

		}

		//Gets the Longitude of the user.
		public double getCurrentLongitude ()
		{
			CLLocationManager myLocMan = new CLLocationManager ();
			myLocMan.DesiredAccuracy = 10;
			if (CLLocationManager.LocationServicesEnabled) {
				myLocMan.StartUpdatingLocation ();
			}
			double longitude = myLocMan.Location.Coordinate.Longitude;
			myLocMan.StartUpdatingLocation ();
			return longitude;
		}

		//Resets the values
		partial void resetMaxValues (NSObject sender)
		{
			currentMaxAvgAccel = 0;
			eventcount = 0;
			this.eventCounter.Text = "0";
			this.latReading.Text = "0";
			this.longReading.Text = "0";
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			rawGPS.createCoordinatesWhenHeadingChangesToAddToList ();
			double currentXAcceleration = 0;
			double currentYAcceleration = 0;
			double currentZAcceleration = 0;
			double lowpassXAcceleration = 0;
			double lowpassYAcceleration = 0;
			double lowpassZAcceleration = 0;
			double speedAtEvent = 0;
			double speedAfterEvent = 0;

			avgaccel = 0;
			currentMaxAvgAccel = 0;

			_motionManager = new CMMotionManager ();
			_motionManager.DeviceMotionUpdateInterval = .5;
			_motionManager.StartDeviceMotionUpdates (NSOperationQueue.CurrentQueue, (data,error) =>
			{

				//lowpassXAcceleration = (currentXAcceleration * klowpassfilterfactor) + (previousLowPassFilteredXAcceleration * (1.0 - klowpassfilterfactor));

				avgaccel = Math.Sqrt ((data.UserAcceleration.X * data.UserAcceleration.X) + 
				                      (data.UserAcceleration.Y * data.UserAcceleration.Y) +
				                      (data.UserAcceleration.Z * data.UserAcceleration.Z));

				if (avgaccel > threshold) {
					eventInProgress = true;
					speedAtEvent = rawGPS.convertToKilometersPerHour (rawGPS.getSpeedInMetersPerSecondUnits());
				} else if ((avgaccel < threshold) && eventInProgress) {
					eventcount++;
					speedAfterEvent = rawGPS.convertToKilometersPerHour (rawGPS.getSpeedInMetersPerSecondUnits());
					this.eventCounter.Text = eventcount.ToString ();
					this.determineHardStoOrHardStart (speedAtEvent, speedAfterEvent);
					this.SpeedAtEventLabel.Text = "Speed At Event: " + speedAtEvent.ToString ();
					this.SpeedAfterEventLabel.Text = "Speed After Event: " + speedAfterEvent.ToString ();
					eventInProgress = false;
					currentCoord.Latitude = getCurrentLatitude ();
					currentCoord.Longitude = getCurrentLongitude ();
					coordList.Add (currentCoord);
					fileManager.addEventToTripEventFile (currentCoord);
					this.latReading.Text = currentCoord.Latitude.ToString ();
					this.longReading.Text = currentCoord.Longitude.ToString ();
				}

				this.avgAcc.Text = avgaccel.ToString ("0.0000");

				if (avgaccel > currentMaxAvgAccel)
					currentMaxAvgAccel = avgaccel;

				this.maxAvgAcc.Text = currentMaxAvgAccel.ToString ("0.0000");
			});

			// Perform any additional setup after loading the view, typically from a nib.
		}

		partial void stopButton (NSObject sender)
		{
			//rawGPS.listOfTripLocationCoordinates.Add (new CLLocation (rawGPS.getCurrentUserLatitude (), rawGPS.getCurrentUserLongitude ()));
			//listOfTripLocationCoordinates = rawGPS.listOfTripLocationCoordinates;
			distanceTraveledForCurrentTrip = rawGPS.convertMetersToKilometers(rawGPS.CalculateDistanceTraveled(rawGPS.listOfTripLocationCoordinates));
		}

		public void determineHardStoOrHardStart(double initialSpeed, double secondSpeed){
			if((secondSpeed > initialSpeed) && (initialSpeed < STARTSPEEDTHRESHOLD)){
				numberHardStarts++;
			}else if (secondSpeed > initialSpeed){
				numberHardAccel++;
			}else if(initialSpeed > secondSpeed){
				numberHardStops++;
			}
		}

		public override void ViewWillAppear (bool animated)
		{
			//add users location when trip starts
			//rawGPS.listOfTripLocationCoordinates.Add (new CLLocation (rawGPS.getCurrentUserLatitude (), rawGPS.getCurrentUserLongitude ()));
			base.ViewWillAppear (animated);
			this.NavigationController.SetNavigationBarHidden (true, animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			_motionManager.StopDeviceMotionUpdates ();
		}

		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();

			ReleaseDesignerOutlets ();
		}
	}
	}