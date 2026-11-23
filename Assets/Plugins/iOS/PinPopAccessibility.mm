#import <UIKit/UIKit.h>
extern "C" int PinPopReducedMotion() { return UIAccessibilityIsReduceMotionEnabled() ? 1 : 0; }
