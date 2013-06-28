// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>


@interface TripSummaryScreen : UIViewController {
	UIButton *_TripSummaryGoogleMapButton;
}

@property (nonatomic, retain) IBOutlet UIButton *TripSummaryGoogleMapButton;
@property (retain, nonatomic) IBOutlet UILabel *tripSummaryEventsLabel;

- (IBAction)toHome:(id)sender;

@end
