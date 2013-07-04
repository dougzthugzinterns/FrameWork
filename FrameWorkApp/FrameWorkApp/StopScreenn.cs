using System;
using System.Drawing;
using System.Collections;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreMotion;
using MonoTouch.CoreLocation;

namespace FrameWorkApp
{
	public partial class StopScreenn : UIViewController
	{
		public static SDMFileManager fileManager = new SDMFileManager ();
		RawGPS rawGPS = new RawGPS ();
		public static double distanceTraveledForCurrentTrip = 0;
		public static double numberHardStarts = 0;
		public static double numberHardStops = 0;
		public static double numberHardAccel = 0;
		const double STARTSPEEDTHRESHOLD = 5;
		const double SPEED_THRESHOLD_BRAKING = -10;
		const double SPEED_THRESHOLD_TURNING = -18;
		const double SPEED_THRESHOLD_ACCEL = 100;
		const double SPEED_THRESHOLD_STARTS = 105;

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

			double speedAtEvent = 0;
			double speedAfterEvent = 0;
			int pollCount = 0;
			double speedDiff = 0;

			avgaccel = 0;
			currentMaxAvgAccel = 0;

			_motionManager = new CMMotionManager ();
			_motionManager.DeviceMotionUpdateInterval = .25;
			_motionManager.StartDeviceMotionUpdates (NSOperationQueue.CurrentQueue, (data,error) =>
			{

				//lowpassXAcceleration = (currentXAcceleration * klowpassfilterfactor) + (previousLowPassFilteredXAcceleration * (1.0 - klowpassfilterfactor));
				if(pollCount != 16){
				avgaccel = Math.Sqrt ((data.UserAcceleration.X * data.UserAcceleration.X) + 
					(data.UserAcceleration.Y * data.UserAcceleration.Y) +
					(data.UserAcceleration.Z * data.UserAcceleration.Z));
				}else{
					avgaccel = 0;
				}
				if (avgaccel > threshold) {
					eventInProgress = true;
					Console.WriteLine("Event is in progress.");
					if(pollCount == 0){
						speedAtEvent = rawGPS.convertToKilometersPerHour(rawGPS.getSpeedInMetersPerSecondUnits());
						Console.WriteLine("Event Initial Speed: " + speedAtEvent);
					}
					pollCount++;
				}
				else if ((avgaccel < threshold) && eventInProgress) {
					Console.WriteLine("Event has ended.");
					eventcount++;
					speedAfterEvent = rawGPS.convertToKilometersPerHour(rawGPS.getSpeedInMetersPerSecondUnits());
					Console.WriteLine("Speed After Event: " + speedAfterEvent);
					this.eventCounter.Text = eventcount.ToString ();
					this.determineHardStoOrHardStart(speedAtEvent, speedAfterEvent);
					this.SpeedAtEventLabel.Text = "Speed At Event: " + speedAtEvent.ToString();
					this.SpeedAfterEventLabel.Text = "Speed After Event: " + speedAfterEvent.ToString();
					speedDiff = speedAfterEvent - speedAtEvent;
					Console.WriteLine("Speed Difference: "+speedDiff);
					Console.WriteLine("----------------------------------");
					this.speedDiffLabel.Text = speedDiff.ToString();
					eventInProgress = false;
					currentCoord.Latitude = rawGPS.getCurrentUserLatitude();
					currentCoord.Longitude = rawGPS.getCurrentUserLongitude();
					coordList.Add (currentCoord);
					fileManager.addEventToTripEventFile (currentCoord);
					this.latReading.Text = currentCoord.Latitude.ToString ();
					this.longReading.Text = currentCoord.Longitude.ToString ();
					pollCount = 0;
					speedAtEvent = 0;
					speedAfterEvent = 0;
				}

				this.avgAcc.Text = avgaccel.ToString ("0.0000");

				if (avgaccel > currentMaxAvgAccel)
					currentMaxAvgAccel = avgaccel;

				this.maxAvgAcc.Text = currentMaxAvgAccel.ToString ("0.0000");
			});
				/*
				if (avgaccel >= G_RATING_BRAKING) {
					if(!eventInProgress){
						speedAtEvent = rawGPS.convertToKilometersPerHour(rawGPS.getSpeedInMetersPerSecondUnits());
					}
					eventInProgress = true;

					//brake shit
				} else if ((avgaccel < G_RATING_BRAKING) && (avgaccel >= G_RATING_TURNING)) {
					if(!eventInProgress){
						speedAtEvent = rawGPS.convertToKilometersPerHour(rawGPS.getSpeedInMetersPerSecondUnits());
					}
					eventInProgress = true;
					//turn shit
				} else if ((avgaccel < G_RATING_TURNING) && (avgaccel >= G_RATING_ACCEL)) {
					if(!eventInProgress){
						speedAtEvent = rawGPS.convertToKilometersPerHour(rawGPS.getSpeedInMetersPerSecondUnits());
					}
					eventInProgress = true;
					//accel shit
				} else if ((avgaccel < G_RATING_ACCEL) && (avgaccel >= G_RATING_STARTS)) {
					if(!eventInProgress){
						speedAtEvent = rawGPS.convertToKilometersPerHour(rawGPS.getSpeedInMetersPerSecondUnits());
					}
					eventInProgress = true;
					//start shit
				} else if(eventInProgress == true){
					eventcount++;
					speedAfterEvent = rawGPS.convertToKilometersPerHour(rawGPS.getSpeedInMetersPerSecondUnits());
					this.determineHardStoOrHardStart(speedAtEvent, speedAfterEvent);
					this.eventCounter.Text = eventcount.ToString ();
					this.SpeedAtEventLabel.Text = "Speed At Event: " + speedAtEvent.ToString();
					this.SpeedAfterEventLabel.Text = "Speed After Event: " + speedAfterEvent.ToString();
					eventInProgress = false;
					currentCoord.Latitude = rawGPS.getCurrentUserLatitude();
					currentCoord.Longitude = rawGPS.getCurrentUserLongitude();
					coordList.Add (currentCoord);
					fileManager.addEventToTripEventFile (currentCoord);
					this.latReading.Text = currentCoord.Latitude.ToString ();
					this.longReading.Text = currentCoord.Longitude.ToString ();
				}
			});
			*/
			// Perform any additional setup after loading the view, typically from a nib.
		}

		partial void stopButton (NSObject sender)
		{
			distanceTraveledForCurrentTrip = rawGPS.convertMetersToKilometers(rawGPS.CalculateDistanceTraveled(rawGPS.listOfTripLocationCoordinates));
			rawGPS.stopGPSReadings();
		}

		public void determineHardStoOrHardStart(double initialSpeed, double secondSpeed){
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