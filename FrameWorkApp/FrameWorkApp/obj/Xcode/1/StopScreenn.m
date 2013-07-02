// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import "StopScreenn.h"

@implementation StopScreenn

@synthesize avgAcc = _avgAcc;
@synthesize eventCounter = _eventCounter;
@synthesize latReading = _latReading;
@synthesize longReading = _longReading;
@synthesize maxAvgAcc = _maxAvgAcc;

- (IBAction)resetMaxValues:(id)sender {
}

- (IBAction)stopButton:(id)sender {
}

- (void)dealloc {
    [_SpeedAtEventLabel release];
    [_SpeedAfterEventLabel release];
    [super dealloc];
}
@end
