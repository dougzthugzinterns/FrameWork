// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace FrameWorkApp
{
	[Register ("StatsScreen")]
	partial class StatsScreen
	{
		[Outlet]
		MonoTouch.UIKit.UILabel totalDistanceLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel totalNumberOfEventsLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel totalPointsLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (totalDistanceLabel != null) {
				totalDistanceLabel.Dispose ();
				totalDistanceLabel = null;
			}

			if (totalNumberOfEventsLabel != null) {
				totalNumberOfEventsLabel.Dispose ();
				totalNumberOfEventsLabel = null;
			}

			if (totalPointsLabel != null) {
				totalPointsLabel.Dispose ();
				totalPointsLabel = null;
			}
		}
	}
}
