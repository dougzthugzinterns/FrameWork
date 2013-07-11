// This file has been autogenerated from parsing an Objective-C header file added in Xcode.
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using MonoTouch.CoreLocation;

namespace FrameWorkApp
{
	public partial class TripSummaryScreen : UIViewController
	{
		SDMFileManager fileManager = new SDMFileManager();
		RawGPS rawGPS = new RawGPS();
		private static Event[] importedGpsEvents;
		double totalDistance;
		int pointChange=0;
		public TripSummaryScreen (IntPtr handle) : base (handle)
		{
			User currentUser = fileManager.readUserFile ();
			totalDistance = rawGPS.convertMetersToKilometers(rawGPS.CalculateDistanceTraveled(new List<CLLocation>(fileManager.readDataFromTripDistanceFile())));

			//Add Recent trip to History
			importedGpsEvents=fileManager.readDataFromTripEventFile ();
			fileManager.addDataToTripLogFile(new Trip(DateTime.Now, importedGpsEvents.Length));

			//Clear Current Trip
			fileManager.clearCurrentTripEventFile();
			fileManager.clearCurrentTripDistanceFile();

			//Update User Data
			pointChange=currentUser.updateData (totalDistance, importedGpsEvents.Length);

			fileManager.updateUserFile (currentUser);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//Updates tripSummaryEventLabel displaying events from this trip
			distanceLabel.Text = rawGPS.convertMetersToKilometers(rawGPS.CalculateDistanceTraveled(new List<CLLocation>(fileManager.readDataFromTripDistanceFile()))).ToString();
			hardBrakesLabel.Text = StopScreen.numberHardStops.ToString ();
			numHardStartLabel.Text = StopScreen.numberHardStarts.ToString ();
			fastAccelsLabel.Text = StopScreen.numberHardStarts.ToString ();
			double totalNumberofEvents = (StopScreen.numberHardStops) + (StopScreen.numberHardStarts);
			totalBreakAcessLabel.Text = totalNumberofEvents.ToString ();
			pointsEarnedLabel.Text = pointChange.ToString ();
			//tripSummaryEventsLabel.Text = StopScreenn.fileManager.readDataFromTripEventFile ().Length.ToString();
			tripSummaryEventsLabel.Text = importedGpsEvents.Length.ToString();
			distanceLabel.Text = totalDistance.ToString ();
		}

		partial void toHome (NSObject sender)
		{
			DismissModalViewControllerAnimated(true);
			StopScreen.fileManager.clearCurrentTripEventFile();
			StopScreen.fileManager.clearCurrentTripDistanceFile();
		}

		public static Event[] getEvents ()
		{
			return importedGpsEvents;
		}
	}
}