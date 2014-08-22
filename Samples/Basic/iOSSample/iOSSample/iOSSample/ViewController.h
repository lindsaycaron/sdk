//
//  ViewController.h
//  iOSSample
//
//  Created by Diana Zhivkova on 8/21/14.
//  Copyright (c) 2014 Social Bet Inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <SOAPEngine/SOAPEngine.h>

@interface ViewController : UIViewController<SOAPEngineDelegate>
{
    NSString* Token;
    NSDictionary* ContextCustomer;
}

@end
