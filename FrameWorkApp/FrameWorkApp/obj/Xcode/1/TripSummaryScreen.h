// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>


@interface TripSummaryScreen : UIViewController {
	UILabel *_distanceLabel;
	UILabel *_tripSummaryEventsLabel;
	UILabel *_sharpTurnLabel;
	UILabel *_hardBrakesLabel;
	UILabel *_fastAccelsLabel;
	UILabel *_totalBreakAcessLabel;
	UILabel *_pointsEarnedLabel;
	UIButton *_TripSummaryGoogleMapButton;
}

@property (nonatomic, retain) IBOutlet UILabel *distanceLabel;

@property (nonatomic, retain) IBOutlet UILabel *tripSummaryEventsLabel;

@property (nonatomic, retain) IBOutlet UILabel *sharpTurnLabel;

@property (nonatomic, retain) IBOutlet UILabel *hardBrakesLabel;

@property (nonatomic, retain) IBOutlet UILabel *fastAccelsLabel;

@property (nonatomic, retain) IBOutlet UILabel *totalBreakAcessLabel;

@property (nonatomic, retain) IBOutlet UILabel *pointsEarnedLabel;

@property (nonatomic, retain) IBOutlet UIButton *TripSummaryGoogleMapButton;

- (IBAction)toHome:(id)sender;

@end
