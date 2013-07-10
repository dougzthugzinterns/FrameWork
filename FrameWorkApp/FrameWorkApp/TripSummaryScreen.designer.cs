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
		MonoTouch.UIKit.UILabel distanceLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel fastAccelsLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel hardBrakesLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel numHardStartLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel pointsEarnedLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel sharpTurnLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel totalBreakAcessLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel tripSummaryEventsLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton TripSummaryGoogleMapButton { get; set; }

		[Action ("toHome:")]
		partial void toHome (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (distanceLabel != null) {
				distanceLabel.Dispose ();
				distanceLabel = null;
			}

			if (tripSummaryEventsLabel != null) {
				tripSummaryEventsLabel.Dispose ();
				tripSummaryEventsLabel = null;
			}

			if (sharpTurnLabel != null) {
				sharpTurnLabel.Dispose ();
				sharpTurnLabel = null;
			}

			if (hardBrakesLabel != null) {
				hardBrakesLabel.Dispose ();
				hardBrakesLabel = null;
			}

			if (fastAccelsLabel != null) {
				fastAccelsLabel.Dispose ();
				fastAccelsLabel = null;
			}

			if (totalBreakAcessLabel != null) {
				totalBreakAcessLabel.Dispose ();
				totalBreakAcessLabel = null;
			}

			if (pointsEarnedLabel != null) {
				pointsEarnedLabel.Dispose ();
				pointsEarnedLabel = null;
			}

			if (TripSummaryGoogleMapButton != null) {
				TripSummaryGoogleMapButton.Dispose ();
				TripSummaryGoogleMapButton = null;
			}

			if (numHardStartLabel != null) {
				numHardStartLabel.Dispose ();
				numHardStartLabel = null;
			}
		}
	}
}
