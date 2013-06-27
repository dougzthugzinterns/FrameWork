// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <UIKit/UIKit.h>
#import <MapKit/MapKit.h>
#import <Foundation/Foundation.h>
#import <CoreGraphics/CoreGraphics.h>


@interface StopScreenn : UIViewController {
	UILabel *_latReading;
	UILabel *_longReading;
	UILabel *_avgAcc;
	UILabel *_maxAvgAcc;
	UILabel *_eventCounter;
}

@property (nonatomic, retain) IBOutlet UILabel *latReading;

@property (nonatomic, retain) IBOutlet UILabel *longReading;

@property (nonatomic, retain) IBOutlet UILabel *avgAcc;

@property (nonatomic, retain) IBOutlet UILabel *maxAvgAcc;

@property (nonatomic, retain) IBOutlet UILabel *eventCounter;

- (IBAction)resetMaxValues:(id)sender;

@end
