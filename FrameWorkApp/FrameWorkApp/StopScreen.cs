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
	public partial class StopScreen : UIViewController
	{
		public static SDMFileManager fileManager = new SDMFileManager ();
		public static List<CLLocation> listOfTripLocationCoordinates = new List<CLLocation> ();
		private RawGPS rawGPS;
		public static double distanceTraveledForCurrentTrip = 0;
		public static double numberHardStarts = 0;
		public static double numberHardStops = 0;
		public static double numberHardAccel = 0;
		const double STARTSPEEDTHRESHOLD = 5;
		const double SPEED_THRESHOLD_BRAKING = -10;
		const double SPEED_THRESHOLD_TURNING = -18;
		const double SPEED_THRESHOLD_ACCEL = 100;
		const double SPEED_THRESHOLD_STARTS = 105;
		const double G_FORCE_THRESHOLD = .35;
		double currentMaxNormalizedGForce;
		double normalizedGForce;
		int eventCount = 0;		//number of events
		bool eventInProgress;		//true if event is in progress
		CLLocationCoordinate2D currentCoord;		//container for current location
		private CMMotionManager _motionManager;

		//Type Codes for Markers
		const int UNKNOWN_EVENT_TYPE= 0;
		const int HARD_BRAKE_TYPE= 1;
		const int HARD_ACCEL_TYPE= 2;

		public StopScreen (IntPtr handle) : base (handle)
		{
			rawGPS = new RawGPS ();
			eventInProgress = false;
			currentCoord = new CLLocationCoordinate2D ();
		}

		partial void resetMaxValuesOnLabelsOnStopScreen (NSObject sender)
		{
			currentMaxNormalizedGForce = 0;
			eventCount = 0;
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

		private double determineNormalizedGForce(double xAccelration, double yAcceleration, double zAcceleration){
			return Math.Sqrt ((xAccelration * xAccelration) + 
			                  (yAcceleration * yAcceleration) +
			                  (zAcceleration * zAcceleration));
		}


		private void handleAccelerometerEvents(CMDeviceMotion data, NSError error){
			double speedAtEvent = 0;
			double speedAfterEvent = 0;
			int pollCountOnEventTriggered = 0;
			double startAndStopSpeedDifference = 0;
			normalizedGForce = 0;
			currentMaxNormalizedGForce = 0;
			//lowpassXAcceleration = (currentXAcceleration * klowpassfilterfactor) + (previousLowPassFilteredXAcceleration * (1.0 - klowpassfilterfactor));
			normalizedGForce = determineNormalizedGForce(data.UserAcceleration.X, data.UserAcceleration.Y, data.UserAcceleration.Z);
			if (normalizedGForce > G_FORCE_THRESHOLD) {
				eventInProgress = true;
				Console.WriteLine("Event is in progress.");
				if(pollCountOnEventTriggered == 0){
					speedAtEvent = rawGPS.convertToKilometersPerHour(rawGPS.getSpeedInMetersPerSecondUnits());
					Console.WriteLine("Event Initial Speed: " + speedAtEvent);
				}
				pollCountOnEventTriggered++;
			}
			else if ((normalizedGForce < G_FORCE_THRESHOLD) && eventInProgress) {
				Console.WriteLine("Event has ended.");
				eventCount++;
				speedAfterEvent = rawGPS.convertToKilometersPerHour(rawGPS.getSpeedInMetersPerSecondUnits());
				Console.WriteLine("Speed After Event: " + speedAfterEvent);
				this.eventCounter.Text = eventCount.ToString ();
				this.determineHardStoOrHardStart(speedAtEvent, speedAfterEvent);
				this.SpeedAtEventLabel.Text = "Speed At Event: " + speedAtEvent.ToString();
				this.SpeedAfterEventLabel.Text = "Speed After Event: " + speedAfterEvent.ToString();
				startAndStopSpeedDifference = speedAfterEvent - speedAtEvent;
				Console.WriteLine("Speed Difference: "+startAndStopSpeedDifference);
				Console.WriteLine("----------------------------------");
				this.speedDiffLabel.Text = startAndStopSpeedDifference.ToString();
				eventInProgress = false;

				//Get GPS
				currentCoord.Latitude = rawGPS.getCurrentUserLatitude();
				currentCoord.Longitude = rawGPS.getCurrentUserLongitude();
				//Create new Event and add to Text File
				Event newEvent = new Event (currentCoord, UNKNOWN_EVENT_TYPE);
				fileManager.addEventToTripEventFile (newEvent);

				this.latReading.Text = currentCoord.Latitude.ToString ();
				this.longReading.Text = currentCoord.Longitude.ToString ();
				pollCountOnEventTriggered = 0;
				speedAtEvent = 0;
				speedAfterEvent = 0;
			}
			this.avgAcc.Text = normalizedGForce.ToString ("0.0000");
			if (normalizedGForce > currentMaxNormalizedGForce)
				currentMaxNormalizedGForce = normalizedGForce;
			this.maxAvgAcc.Text = currentMaxNormalizedGForce.ToString ("0.0000");			
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			_motionManager = new CMMotionManager ();
			if (CLLocationManager.Status == CLAuthorizationStatus.Authorized) {
				rawGPS.createCoordinatesWhenHeadingChangesToAddToList ();
				_motionManager.DeviceMotionUpdateInterval = .5;
				_motionManager.StartDeviceMotionUpdates (NSOperationQueue.CurrentQueue, (data,error) =>
				{
					handleAccelerometerEvents (data, error);
				});
			} else {
				new UIAlertView("Location Services must be enabled to use application!","We noticed you have disabled location services for this application. Please enable these before continuing. Please enable these before starting a new trip.", null, "OK", null).Show();
			}
			// Perform any additional setup after loading the view, typically from a nib.
		}

		partial void stopButton (NSObject sender)
		{
			rawGPS.listOfRawGPSTripLocationCoordinates.Add (new CLLocation (rawGPS.getCurrentUserLatitude (), rawGPS.getCurrentUserLongitude ()));
			listOfTripLocationCoordinates = rawGPS.listOfRawGPSTripLocationCoordinates;
			distanceTraveledForCurrentTrip = rawGPS.convertMetersToKilometers(rawGPS.CalculateDistanceTraveled(rawGPS.listOfRawGPSTripLocationCoordinates));
			rawGPS.stopGPSReadings();
		}

		private void determineHardStoOrHardStart(double initialSpeed, double secondSpeed){
			/*
			if((secondSpeed > initialSpeed) && (initialSpeed < STARTSPEEDTHRESHOLD)){
				numberHardStarts++;
			}else if (secondSpeed > initialSpeed){
				numberHardAccel++;
			}else if(initialSpeed > secondSpeed){
				if(turn critiera is met){

				}else{
					numberHardStops++;
				}
			}
			*/

			if (secondSpeed > initialSpeed) { //speeding up
				if (initialSpeed < STARTSPEEDTHRESHOLD) { //if your initial speed is below 5km
					if((secondSpeed - initialSpeed) > SPEED_THRESHOLD_STARTS){ //and finally if youre going fast enough
						numberHardStarts++; //hard start bitch
						Console.WriteLine ("Hard start recorded.");
					}else{
						Console.WriteLine ("Not a hard enough start.");
					}
				} else {
					if((secondSpeed - initialSpeed) > SPEED_THRESHOLD_ACCEL){
						numberHardAccel++;
						Console.WriteLine ("Hard acceleration recorded.");
					}else{
						Console.WriteLine ("Not a hard enough acceleration.");
					}
				}
			} else if (secondSpeed < initialSpeed) {
				if((secondSpeed - initialSpeed) < SPEED_THRESHOLD_BRAKING){
					numberHardStops++;
					Console.WriteLine ("Hard stop recorded.");
				}else{
					Console.WriteLine ("Not a hard enough brake.");
				}
			}

		}

		public override void ViewWillAppear (bool animated)
		{
			//add users location when trip starts
			if(CLLocationManager.Status == CLAuthorizationStatus.Authorized){
				rawGPS.listOfRawGPSTripLocationCoordinates.Add (new CLLocation (rawGPS.getCurrentUserLatitude (), rawGPS.getCurrentUserLongitude ()));
			}
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