// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <UIKit/UIKit.h>
#import <MapKit/MapKit.h>
#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>


@interface TripSummaryScreen : UIViewController {
	UILabel *_distanceLabel;
	UILabel *_fastAccelsLabel;
	UILabel *_hardBrakesLabel;
	UILabel *_numHardStartLabel;
	UILabel *_pointsEarnedLabel;
	UILabel *_sharpTurnLabel;
	UILabel *_totalBreakAcessLabel;
	UILabel *_tripSummaryEventsLabel;
	UIButton *_TripSummaryGoogleMapButton;
}

@property (nonatomic, retain) IBOutlet UILabel *distanceLabel;

@property (nonatomic, retain) IBOutlet UILabel *fastAccelsLabel;

@property (nonatomic, retain) IBOutlet UILabel *hardBrakesLabel;

@property (nonatomic, retain) IBOutlet UILabel *numHardStartLabel;

@property (nonatomic, retain) IBOutlet UILabel *pointsEarnedLabel;

@property (nonatomic, retain) IBOutlet UILabel *sharpTurnLabel;

@property (nonatomic, retain) IBOutlet UILabel *totalBreakAcessLabel;

@property (nonatomic, retain) IBOutlet UILabel *tripSummaryEventsLabel;

@property (nonatomic, retain) IBOutlet UIButton *TripSummaryGoogleMapButton;

- (IBAction)toHome:(id)sender;

@end
