// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace FrameWorkApp
{
	[Register ("StopScreen")]
	partial class StopScreen
	{
		[Outlet]
		MonoTouch.UIKit.UILabel avgAcc { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel eventCounter { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel currentSpeedLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel maxAvgAcc { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel SpeedAfterEventLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel SpeedAtEventLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel speedDiffLabel { get; set; }

		[Action ("stopButton:")]
		partial void stopButton (MonoTouch.Foundation.NSObject sender);

		[Action ("resetMaxValues:")]
		partial void resetMaxValuesOnLabelsOnStopScreen (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (avgAcc != null) {
				avgAcc.Dispose ();
				avgAcc = null;
			}

			if (eventCounter != null) {
				eventCounter.Dispose ();
				eventCounter = null;
			}

			if (currentSpeedLabel != null) {
				currentSpeedLabel.Dispose ();
				currentSpeedLabel = null;
			}

			if (maxAvgAcc != null) {
				maxAvgAcc.Dispose ();
				maxAvgAcc = null;
			}

			if (SpeedAfterEventLabel != null) {
				SpeedAfterEventLabel.Dispose ();
				SpeedAfterEventLabel = null;
			}

			if (SpeedAtEventLabel != null) {
				SpeedAtEventLabel.Dispose ();
				SpeedAtEventLabel = null;
			}

			if (speedDiffLabel != null) {
				speedDiffLabel.Dispose ();
				speedDiffLabel = null;
			}
		}
	}
}
