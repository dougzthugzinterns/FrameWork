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
		public StopScreenn (IntPtr handle) : base (handle)
		{
		}
		public static ArrayList coordList = new ArrayList();
		double currentMaxAvgAccel;
		double avgaccel;
		double threshold = .5;
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

			avgaccel = 0;
			currentMaxAvgAccel = 0;

			_motionManager = new CMMotionManager ();
			_motionManager.DeviceMotionUpdateInterval = .5;
			_motionManager.StartDeviceMotionUpdates (NSOperationQueue.CurrentQueue, (data,error) =>
			{

				//UIAccelerationValue lowPassFilteredXAcceleration = (currentXAcceleration * kLowPassFilteringFactor) + (previousLowPassFilteredXAcceleration * (1.0 - kLowPassFilteringFactor));


				avgaccel = Math.Sqrt ((data.UserAcceleration.X * data.UserAcceleration.X) + 
				                      (data.UserAcceleration.Y * data.UserAcceleration.Y) +
				                      (data.UserAcceleration.Z * data.UserAcceleration.Z));

				if (avgaccel > threshold) {
					eventInProgress = true;
				} else if ((avgaccel < threshold) && eventInProgress) {
					eventcount++;
					this.eventCounter.Text = eventcount.ToString ();
					eventInProgress = false;
					currentCoord.Latitude = getCurrentLatitude ();
					currentCoord.Longitude = getCurrentLongitude ();
					coordList.Add (currentCoord);

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

		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();

			ReleaseDesignerOutlets ();
		}

	}
}