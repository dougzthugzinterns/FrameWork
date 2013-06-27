// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace FrameWorkApp
{
	[Register ("TripSummaryScreen")]
	partial class TripSummaryScreen
	{
		[Outlet]
		MonoTouch.UIKit.UIButton TripSummaryGoogleMapButton { get; set; }

		[Action ("toHome:")]
		partial void toHome (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (TripSummaryGoogleMapButton != null) {
				TripSummaryGoogleMapButton.Dispose ();
				TripSummaryGoogleMapButton = null;
			}
		}
	}
}
