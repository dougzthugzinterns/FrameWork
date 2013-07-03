using MonoTouch.UIKit;
using System;
using MonoTouch.Foundation;
using MonoTouch.CoreLocation;
namespace FrameWorkApp
{
	public partial class MainViewController : UIViewController
	{
		SDMFileManager fileManager= new SDMFileManager();

		public MainViewController (IntPtr handle) : base (handle)
		{
			// Custom initialization
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}
		#region View lifecycle

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//Alert User of Previous Trip in Progress was Saved
			UILocalNotification notification = new UILocalNotification();

			//Phone Crashed during a Trip in Progress, write data recovered to trip log.
			if (fileManager.currentTripInProgress()) {
				notification.AlertAction = "Trip Data Recovered!";
				notification.AlertBody = "We detected your phone has shut down during a trip, " +
					"but good news we managed to recover your data up to that point your phone shut down.";
				//Read Data from Recovered File.
				fileManager.addDataToTripLogFile(new Trip(fileManager.getDateOfLastPointEnteredInCurrentTrip(), fileManager.readDataFromTripEventFile().Length));
				fileManager.clearCurrentTripEventFile();
				fileManager.clearCurrentTripDistanceFile ();
				//Display Alert
				UIApplication.SharedApplication.ScheduleLocalNotification(notification);
			} 
		

		}
	

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}
		#endregion

		partial void showInfo (NSObject sender)
		{
		}
	}
}
