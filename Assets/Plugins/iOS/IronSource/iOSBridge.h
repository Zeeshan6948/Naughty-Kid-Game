//
//  iOSBridge.h
//  iOSBridge
//
//  Created by Supersonic.
//  Copyright (c) 2015 Supersonic. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <IronSource/IronSource.h>

static NSString *  GitHash = @"3807e96";

@interface iOSBridge : NSObject<ISRewardedVideoDelegate,ISDemandOnlyRewardedVideoDelegate, ISInterstitialDelegate,ISDemandOnlyInterstitialDelegate, ISOfferwallDelegate, ISRewardedInterstitialDelegate, ISBannerDelegate, ISSegmentDelegate>

@end


