// This file has been autogenerated from parsing an Objective-C header file added in Xcode.

using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace FrameWorkApp
{
	public partial class MenuScreenTiles : UIViewController
	{
		public MenuScreenTiles (IntPtr handle) : base (handle)
		{
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			this.NavigationController.SetNavigationBarHidden (true,false);
		}


		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			this.NavigationController.SetNavigationBarHidden (false,false);

		}
		partial void menuToHome (NSObject sender)
		{
			DismissViewController(true,null);
		}
	}
}
