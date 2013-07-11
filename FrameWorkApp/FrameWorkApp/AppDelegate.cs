using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Google.Maps;
using System.Collections;
using MonoTouch.CoreLocation;

namespace FrameWorkApp
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		ArrayList coordList = new ArrayList ();
		// class-level declarations
		//MapServices.ProvideAPIKey ("AIzaSyCgSqffPgmaQlNENxSehjScWg2VLDVxJ-w");
		public override UIWindow Window {
			get;
			set;
		}

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			MapServices.ProvideAPIKey ("AIzaSyCgSqffPgmaQlNENxSehjScWg2VLDVxJ-w");
			// check for a notification
			if (options != null) {
				// check for a local notification
				if (options.ContainsKey (UIApplication.LaunchOptionsLocalNotificationKey)) {
					var localNotification = options [UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
					if (localNotification != null) {
						new UIAlertView (localNotification.AlertAction, localNotification.AlertBody, null, "OK", null).Show ();
						// reset our badge
						UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
					}
				}
			}
			return true;
		}

		public override void ReceivedLocalNotification (UIApplication application, UILocalNotification notification)
		{
			// show an alert
		
			new UIAlertView (notification.AlertAction, notification.AlertBody, null, "OK", null).Show ();
			
			// reset our badge
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
		}

		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation (UIApplication application)
		{
		}

		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
		}

		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
			if (CLLocationManager.Status == CLAuthorizationStatus.Denied) {
				new UIAlertView ("Location Services must be enabled to use application!", "We noticed you have disabled location services for this application. Please enable these before continuing. Please enable these before starting a new trip.", null, "OK", null).Show ();
			}
		}

		// This method is called when the application is about to terminate. Save data, if needed. 
		public override void WillTerminate (UIApplication application)
		{
		}
	}
}