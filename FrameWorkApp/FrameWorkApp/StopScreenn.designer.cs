// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace FrameWorkApp
{
	[Register ("StopScreenn")]
	partial class StopScreenn
	{
		[Outlet]
		MonoTouch.UIKit.UILabel avgAcc { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel eventCounter { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel latReading { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel longReading { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel maxAvgAcc { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel SpeedAfterEventLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel SpeedAtEventLabel { get; set; }

		[Action ("resetMaxValues:")]
		partial void resetMaxValues (MonoTouch.Foundation.NSObject sender);

		[Action ("stopButton:")]
		partial void stopButton (MonoTouch.Foundation.NSObject sender);
		
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

			if (latReading != null) {
				latReading.Dispose ();
				latReading = null;
			}

			if (longReading != null) {
				longReading.Dispose ();
				longReading = null;
			}

			if (maxAvgAcc != null) {
				maxAvgAcc.Dispose ();
				maxAvgAcc = null;
			}

			if (SpeedAtEventLabel != null) {
				SpeedAtEventLabel.Dispose ();
				SpeedAtEventLabel = null;
			}

			if (SpeedAfterEventLabel != null) {
				SpeedAfterEventLabel.Dispose ();
				SpeedAfterEventLabel = null;
			}
		}
	}
}
