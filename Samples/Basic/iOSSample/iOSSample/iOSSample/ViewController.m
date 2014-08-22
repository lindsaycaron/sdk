//
//  ViewController.m
//  iOSSample
//
//  Created by Diana Zhivkova on 8/21/14.
//  Copyright (c) 2014 Social Bet Inc. All rights reserved.
//

#import "ViewController.h"
#import "Definitions.h"

@interface ViewController ()

-(NSString*)getSoapAction:(NSString*)operation;
-(NSString*)getJsonFromDictionary:(NSDictionary*)dictionary;
-(NSDictionary*)getDictionary:(NSString*)json;

@end

static NSString* SERVICE_URL = @"http://dev.youbetme.com/svc/service.asmx";
static NSString* SERVICE_NAMESPACE = @"http://www.youbetme.com/";

@implementation ViewController


- (void)viewDidLoad{
    [super viewDidLoad];
    
    [self logIn];
}

- (void)signUp{
    NSMutableDictionary *customer = [[NSMutableDictionary alloc] init];
    // TODO: Set first, last names, email and password
    [customer setObject:@"first-name-here" forKey:@"FirstName"];
    [customer setObject:@"last-name-here" forKey:@"LastName"];
    [customer setObject:@"email-here" forKey:@"EMail"];
    [customer setObject:@"password-here" forKey:@"Password"];

    NSString *customerJson = [self getJsonFromDictionary:customer];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:customerJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
          soapAction:[self getSoapAction: @"CreateNewCustomer"]
            complete:^(NSInteger statusCode, NSString *stringXML) {
                NSDictionary *response = [self getDictionary:[engine stringValue]];
               
                NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                {
                    NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                    return;
                }
                
                NSLog(@"Customer: %@", [response valueForKey:@"Result"]);
                
            } failWithError:^(NSError *error) {
                NSLog(@"%@", error);
            }];
}

- (void)logIn{
    SOAPEngine* engine = [self getEngine];
    
    // TODO: Set username or email and password below
    [engine setValue:@"username-or-email-here" forKey:@"usernameOrEmail"];
    [engine setValue:@"password-here" forKey:@"password"];
    
    [engine requestURL:SERVICE_URL
          soapAction:[self getSoapAction: @"LoginWithPassword"]
            complete:^(NSInteger statusCode, NSString *stringXML) {
                NSDictionary *response = [self getDictionary:[engine stringValue]];
                
                NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                {
                    NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                    return;
                }
                
                NSLog(@"Login Result: %@", [response valueForKey:@"Result"]);
                
                Token = [response valueForKeyPath:@"Result.Token"];
                
                [self getCustomerFromToken];
                
            } failWithError:^(NSError *error) {
                NSLog(@"%@", error);
            }];
}

- (void)logInWithFacebook{

    NSMutableDictionary *socialNetwork = [[NSMutableDictionary alloc] init];
    // TODO: Set first, last names, email and password
    [socialNetwork setObject:[NSNumber numberWithInt:Facebook] forKey:@"Type"];
    [socialNetwork setObject:[NSNumber numberWithInt:SocialNetworkPermissionRead] forKey:@"Permission"];
    [socialNetwork setObject:@"Facebook-access-token-here" forKey:@"Token"];
    [socialNetwork setObject:[NSDate date] forKey:@"ExpirationDate"];
    
    NSString *networkJson = [self getJsonFromDictionary:socialNetwork];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:networkJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"LoginWithFacebook"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Login Result: %@", [response valueForKey:@"Result"]);
                  
                  Token = [response valueForKeyPath:@"Result.Token"];
                  
                  [self getCustomerFromToken];
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

- (void)getCustomerFromToken{
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetCustomerFromSession"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Customer: %@", [response valueForKey:@"Result"]);
                  
                  ContextCustomer = [response valueForKey:@"Result"];
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

- (void)getCustomerFromId{
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    
    // TODO: Set below customer Id
    [engine setValue:@"customer-id-here" forKey:@"customerId"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetCustomerFromId"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Customer: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)updateCustomer{
    NSMutableDictionary *temp = [ContextCustomer mutableCopy];
    // TODO: Set below first name
    [temp setValue:@"new-first-name-here" forKey:@"FirstName"];
    
    NSString *customerJson = [self getJsonFromDictionary:temp];
    
    SOAPEngine* engine = [self getEngine];
    [engine setValue:Token forKey:@"token"];
    [engine setValue:customerJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"UpdateCustomer"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Customer: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
   
}

-(void)getRecentFriends{
    NSMutableDictionary *filter = [[NSMutableDictionary alloc] init];
    [filter setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    // Note: Specify page number and size
    [filter setObject:[NSNumber numberWithInt:0] forKey:@"PageNumber"];
    [filter setObject:[NSNumber numberWithInt:50] forKey:@"PageSize"];
    // Note: The parameter below is optional and is needed in case you want to search by first, last name or email
    // [filter setObject:@"Sarah Johnson" forKey:@"Criteria"];
    
    NSString *customerJson = [self getJsonFromDictionary:filter];
    
    SOAPEngine* engine = [self getEngine];

    [engine setValue:Token forKey:@"token"];
    [engine setValue:customerJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetRecentFriendsLight"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Recent Friends: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)getFriends{
    NSMutableDictionary *filter = [[NSMutableDictionary alloc] init];
    [filter setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    // Note: Specify page number and size
    [filter setObject:[NSNumber numberWithInt:0] forKey:@"PageNumber"];
    [filter setObject:[NSNumber numberWithInt:50] forKey:@"PageSize"];
    // Note: The parameter below is optional and is needed in case you want to search by first, last name or email
    // [filter setObject:@"Sarah Johnson" forKey:@"Criteria"];
    
    NSString *customerJson = [self getJsonFromDictionary:filter];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    [engine setValue:customerJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetFriendsLight"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Friends: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)getFollowers{
    NSMutableDictionary *filter = [[NSMutableDictionary alloc] init];
    [filter setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    // Note: Specify page number and size
    [filter setObject:[NSNumber numberWithInt:0] forKey:@"PageNumber"];
    [filter setObject:[NSNumber numberWithInt:50] forKey:@"PageSize"];
    // Note: The parameter below is optional and is needed in case you want to search by first, last name or email
    // [filter setObject:@"Sarah Johnson" forKey:@"Criteria"];
    
    NSString *customerJson = [self getJsonFromDictionary:filter];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    [engine setValue:customerJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetFollowersLight"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Followers: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)getFollowees{
    NSMutableDictionary *filter = [[NSMutableDictionary alloc] init];
    [filter setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    // Note: Specify page number and size
    [filter setObject:[NSNumber numberWithInt:0] forKey:@"PageNumber"];
    [filter setObject:[NSNumber numberWithInt:50] forKey:@"PageSize"];
    // Note: The parameter below is optional and is needed in case you want to search by first, last name or email
    // [filter setObject:@"Sarah Johnson" forKey:@"Criteria"];
    
    NSString *customerJson = [self getJsonFromDictionary:filter];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    [engine setValue:customerJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetFollowingLight"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Followeеs: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)performCustomerSearch{
    NSMutableDictionary *filter = [[NSMutableDictionary alloc] init];
    [filter setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    // Note: Specify page number and size
    [filter setObject:[NSNumber numberWithInt:0] forKey:@"PageNumber"];
    [filter setObject:[NSNumber numberWithInt:50] forKey:@"PageSize"];
	// Note: The parameter below is required
    [filter setObject:@"Sarah Johnson" forKey:@"Criteria"];
    
    NSString *customerJson = [self getJsonFromDictionary:filter];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    [engine setValue:customerJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"PerformCustomerSearch"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Customers: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)getBetSuggestions{
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetBetSuggestions"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Suggestions: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)getPendingBets{
    NSMutableDictionary *filter = [[NSMutableDictionary alloc] init];
    [filter setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    NSArray *statuses = [NSArray arrayWithObject:[NSNumber numberWithInt:BetStatusPending]];
    [filter setObject:statuses forKey:@"Statuses"];
    // Note: Specify page number and size
    [filter setObject:[NSNumber numberWithInt:0] forKey:@"PageNumber"];
    [filter setObject:[NSNumber numberWithInt:50] forKey:@"PageSize"];
    
    NSString *customerJson = [self getJsonFromDictionary:filter];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    [engine setValue:customerJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetBetsLight"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Pending Bets: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)getActiveBets{
    NSMutableDictionary *filter = [[NSMutableDictionary alloc] init];
    [filter setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    NSArray *statuses = [NSArray arrayWithObject:[NSNumber numberWithInt:BetStatusActive]];
    [filter setObject:statuses forKey:@"Statuses"];
    // Note: Specify page number and size
    [filter setObject:[NSNumber numberWithInt:0] forKey:@"PageNumber"];
    [filter setObject:[NSNumber numberWithInt:50] forKey:@"PageSize"];
    
    NSString *customerJson = [self getJsonFromDictionary:filter];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    [engine setValue:customerJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetBetsLight"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Active Bets: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)getPayUpBets{
    NSMutableDictionary *filter = [[NSMutableDictionary alloc] init];
    [filter setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    NSArray *statuses = [NSArray arrayWithObject:[NSNumber numberWithInt:BetStatusPayup]];
    [filter setObject:statuses forKey:@"Statuses"];
    // Note: Specify page number and size
    [filter setObject:[NSNumber numberWithInt:0] forKey:@"PageNumber"];
    [filter setObject:[NSNumber numberWithInt:50] forKey:@"PageSize"];
    
    NSString *customerJson = [self getJsonFromDictionary:filter];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    [engine setValue:customerJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetBetsLight"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Pay Up Bets: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)getClosedBets{
    NSMutableDictionary *filter = [[NSMutableDictionary alloc] init];
    [filter setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    NSArray *statuses = [NSArray arrayWithObjects:[NSNumber numberWithInt:BetStatusPaid], [NSNumber numberWithInt:BetStatusWelched], nil];
    [filter setObject:statuses forKey:@"Statuses"];
    // Note: Specify page number and size
    [filter setObject:[NSNumber numberWithInt:0] forKey:@"PageNumber"];
    [filter setObject:[NSNumber numberWithInt:50] forKey:@"PageSize"];
    
    NSString *customerJson = [self getJsonFromDictionary:filter];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    [engine setValue:customerJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetBetsLight"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Closed Bets: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)getPublicBets{
    NSMutableDictionary *filter = [[NSMutableDictionary alloc] init];
    // Note: Optionally use the line below to limit bets by status
    NSArray *statuses = [NSArray arrayWithObjects:[NSNumber numberWithInt:BetStatusActive], nil];
    [filter setObject:statuses forKey:@"Statuses"];
    // Note: Optionally use the line below to limit bets by types
    NSArray *types = [NSArray arrayWithObjects:[NSNumber numberWithInt:BetTypeCustom], [NSNumber numberWithInt:BetTypeSports], nil];
    [filter setObject:types forKey:@"Types"];
    // Note: Specify page number and size
    [filter setObject:[NSNumber numberWithInt:0] forKey:@"PageNumber"];
    [filter setObject:[NSNumber numberWithInt:50] forKey:@"PageSize"];
    
    NSString *customerJson = [self getJsonFromDictionary:filter];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    [engine setValue:customerJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetBets"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Public Bets: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)createBet{
    NSMutableDictionary *bet = [[NSMutableDictionary alloc] init];
    
    // Specify the bet maker Id
    [bet setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    
    // Specify bet type
    [bet setObject:[NSNumber numberWithInt:BetTypeCustom] forKey:@"Type"];
    
    [bet setObject:@"My First Bet" forKey:@"BetTitle"];
    [bet setObject:@"Gets created" forKey:@"BetTerms"];
    [bet setObject:@"High Five" forKey:@"BetOutcome"];
    
    NSMutableArray *teams = [[NSMutableArray alloc] init];
    
    // Create the bet maker team
    NSMutableDictionary *team =  [[NSMutableDictionary alloc] init];
    [team setObject:[NSNumber numberWithInt:TeamSideBetMaker] forKey:@"Side"];

    NSMutableArray *members = [[NSMutableArray alloc] init];
    
    // Add the bet maker to the team
    NSMutableDictionary *member =  [[NSMutableDictionary alloc] init];
    [member setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    [member setObject:[NSNumber numberWithInt:BetStatusActive] forKey:@"BetStatus"];
    [members addObject:member];
    
    // Optionally add a teammate
    member =  [[NSMutableDictionary alloc] init];
    [member setObject:@"teammate-id-here" forKey:@"CustomerId"];

    [members addObject:member];
    
    [team setObject:members forKey:@"Members"];
    
    [teams addObject:team];
    
    // Create the opponent team
    team =  [[NSMutableDictionary alloc] init];
    [team setObject:[NSNumber numberWithInt:TeamSideOpponent] forKey:@"Side"];
    
    members = [[NSMutableArray alloc] init];
    
    // Add the first opponent
    member =  [[NSMutableDictionary alloc] init];
    [member setObject:@"opponent1-id-here" forKey:@"CustomerId"];
    [members addObject:member];
    
    // Optionally add another opponent
    member =  [[NSMutableDictionary alloc] init];
    [member setObject:@"opponent2-id-here" forKey:@"CustomerId"];
    [members addObject:member];
    
    [team setObject:members forKey:@"Members"];
    
    [teams addObject:team];
    
    // Optionally add a team of spectators
    
    team =  [[NSMutableDictionary alloc] init];
    [team setObject:[NSNumber numberWithInt:TeamSideAudience] forKey:@"Side"];
    
    members = [[NSMutableArray alloc] init];
    
    // Add as many spectators as needed
    member =  [[NSMutableDictionary alloc] init];
    [member setObject:@"spectator-id-here" forKey:@"CustomerId"];
    [members addObject:member];
    
    [team setObject:members forKey:@"Members"];
    
    [teams addObject:team];

    [bet setObject:teams forKey:@"Teams"];
    
    // Optionally add bet location
    NSMutableDictionary *location = [[NSMutableDictionary alloc] init];
    //[location setObject:0 forKey:@"Latitude"];
    //[location setObject:0 forKey:@"Longitude"];
    //[location setObject:0 forKey:@"Altitude"];
    [bet setObject:location forKey:@"Location"];
    
    NSString *betJson = [self getJsonFromDictionary:bet];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    [engine setValue:betJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"CreateNewBet"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Bet: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)acceptBet{
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    // TODO: Provide betId below
    [engine setValue:@"bet-id-here" forKey:@"betId"];
    [engine setValue:[ContextCustomer valueForKey:@"Id"] forKey:@"customerId"];
    [engine setValue:[NSNumber numberWithInt:BetStatusActive] forKey:@"status"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"SetMemberStatus"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Bet: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];

}

-(void)denyBet{
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    // TODO: Provide betId below
    [engine setValue:@"bet-id-here" forKey:@"betId"];
    [engine setValue:[ContextCustomer valueForKey:@"Id"] forKey:@"customerId"];
    [engine setValue:[NSNumber numberWithInt:BetStatusRejected] forKey:@"status"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"SetMemberStatus"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Bet: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
    
}

-(void)cancelBet{
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    // TODO: Provide betId below
    [engine setValue:@"bet-id-here" forKey:@"betId"];
    [engine setValue:[ContextCustomer valueForKey:@"Id"] forKey:@"customerId"];
    [engine setValue:[NSNumber numberWithInt:BetStatusRejected] forKey:@"status"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"SetMemberStatus"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Bet: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];

}

-(void)closeBet{
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    // TODO: Provide betId below
    [engine setValue:@"bet-id-here" forKey:@"betId"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetBet"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Bet: %@", [response valueForKey:@"Result"]);
                  
                  NSMutableDictionary *bet = [[response valueForKey:@"Result"] mutableCopy];
                  
                  NSArray *teams = [bet valueForKey:@"Teams"];
                  
                  NSMutableDictionary *betMakerTeam, *opponentTeam, *audienceTeam;
                  NSMutableArray *temp = [[NSMutableArray alloc] init];
                  for(int index = 0; index < teams.count; index++)
                  {
                      if([teams[index] valueForKey: @"Side"] == [NSNumber numberWithInt:TeamSideBetMaker])
                      {
                          betMakerTeam = [teams[index] mutableCopy];
                          continue;
                      }
                      
                      if([teams[index] valueForKey: @"Side"] == [NSNumber numberWithInt:TeamSideOpponent])
                      {
                          opponentTeam = [teams[index] mutableCopy];
                          continue;
                      }
                      
                      if([teams[index] valueForKey: @"Side"] == [NSNumber numberWithInt:TeamSideAudience])
                      {
                          audienceTeam = [teams[index] mutableCopy];
                          continue;
                      }
                  }
                  
                  if(betMakerTeam == nil || opponentTeam == nil){
                      // TODO: Report error
                      return;
                  }
                  
                  // TODO: Set winner and loser
                  [betMakerTeam setObject:[NSNumber numberWithInt:TeamStatusWon] forKey:@"Status"];
                  [temp addObject:betMakerTeam];
                  [opponentTeam setObject:[NSNumber numberWithInt:TeamStatusLost] forKey:@"Status"];
                  [temp addObject:opponentTeam];
                  if(audienceTeam != nil) [temp addObject:audienceTeam];

                  [bet setObject:temp forKey:@"Teams"];

                  // Note: Set whether the bet has been paid, welched or is to be paid by changing its status
                  [bet setObject:[NSNumber numberWithInt:BetStatusPayup] forKey:@"Status"];
                  
                  NSString* betJson = [self getJsonFromDictionary:bet];
                  
                  SOAPEngine* engine = [self getEngine];
                  
                  [engine setValue:Token forKey:@"token"];
                  [engine setValue:betJson forKey:@"request"];
                  
                  [engine requestURL:SERVICE_URL
                          soapAction:[self getSoapAction: @"CloseBet"]
                            complete:^(NSInteger statusCode, NSString *stringXML) {
                                NSDictionary *response = [self getDictionary:[engine stringValue]];
                                
                                NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                                if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                                {
                                    NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                                    return;
                                }
                                
                                NSLog(@"Bet: %@", [response valueForKey:@"Result"]);
                                
                            } failWithError:^(NSError *error) {
                                NSLog(@"%@", error);
                            }];
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];

}

-(void)postText{
    NSMutableDictionary *post = [[NSMutableDictionary alloc] init];
    [post setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    // TODO: Set below betId
    [post setObject:@"bet-id-here" forKey:@"BetId"];
    [post setObject:@"Hey! That's a really cool bet you created" forKey:@"Text"];
    
    NSString *postJson = [self getJsonFromDictionary:post];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    [engine setValue:postJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"CreateNewPost"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Post: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)postPhoto{
    NSMutableDictionary *post = [[NSMutableDictionary alloc] init];
    [post setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    // TODO: Set below betId
    [post setObject:@"bet-id-here" forKey:@"BetId"];
    // TODO: Set PNG image data below
    NSData *data = [[NSData alloc] init];
    NSString *dataString = [data base64EncodedStringWithOptions:NSDataBase64EncodingEndLineWithCarriageReturn];
    [post setObject:dataString forKey:@"ImageData"];
    
    NSString *postJson = [self getJsonFromDictionary:post];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    [engine setValue:postJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"CreateNewPost"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Post: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)uploadBetPhoto{
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    // TODO: Set below betId
    [engine setValue:@"bet-id-here" forKey:@"betId"];
    // TODO: Set PNG image data below
    NSData *data = [[NSData alloc] init];
    NSString *dataString = [data base64EncodedStringWithOptions:NSDataBase64EncodingEndLineWithCarriageReturn];
    [engine setValue:dataString forKey:@"bytes"];
    [engine setValue:@"false" forKey:@"postCreateUpload"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"UploadBetImage"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Filename: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
}

-(void)getNewsFeed{
    NSMutableDictionary *filter = [[NSMutableDictionary alloc] init];
    [filter setObject:[ContextCustomer valueForKey:@"Id"] forKey:@"CustomerId"];
    // TODO: Set below betId
    [filter setObject:[NSNumber numberWithInt:0] forKey:@"PageNumber"];
    [filter setObject:[NSNumber numberWithInt:50]  forKey:@"PageSize"];
    
    NSString *filterJson = [self getJsonFromDictionary:filter];
    
    SOAPEngine* engine = [self getEngine];
    
    [engine setValue:Token forKey:@"token"];
    [engine setValue:filterJson forKey:@"request"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"GetFeed"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"News Stories: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];

}

-(void)setDevice{
    SOAPEngine* engine = [self getEngine];
    
    // TODO: Provide deviceToken below
    [engine setValue:@"device-token-here" forKey:@"deviceToken"];
    [engine setValue:[NSNumber numberWithInt:iOS] forKey:@"system"];
    [engine setValue:[ContextCustomer valueForKey:@"Id"] forKey:@"customerId"];
    
    [engine requestURL:SERVICE_URL
            soapAction:[self getSoapAction: @"SetDevice"]
              complete:^(NSInteger statusCode, NSString *stringXML) {
                  NSDictionary *response = [self getDictionary:[engine stringValue]];
                  
                  NSNumber *status = (NSNumber*)[response valueForKey:@"Status"];
                  if(![status isEqualToNumber: [NSNumber numberWithInt:StatusSuccess]])
                  {
                      NSLog(@"Backend Error: %@", [response valueForKey:@"Message"]);
                      return;
                  }
                  
                  NSLog(@"Device: %@", [response valueForKey:@"Result"]);
                  
              } failWithError:^(NSError *error) {
                  NSLog(@"%@", error);
              }];
    
}


-(SOAPEngine*)getEngine
{
    SOAPEngine *engine = [[SOAPEngine alloc] init];
    engine.userAgent = @"SOAPEngine";
    engine.actionNamespaceSlash = YES;
    engine.delegate = self;
    
    // license for com.prioregroup.soapengine-sample bundle
    // *** not required on ios-simulator ***
    engine.licenseKey = @"eJJDzkPK9Xx+p5cOH7w0Q+AvPdgK1fzWWuUpMaYCq3r1mwf36Ocw6dn0+CLjRaOiSjfXaFQBWMi+TxCpxVF/FA==";
    
    // TODO: Provide your API token and secret below
    // Note: To obtain a valid API token and secret sign up at http://dev.youbetme.com/portal
    // Then contact a Youbetme representative or sent email to support@youbetme.com to request access to the API
    // Once your account has been granted access, use your credentails (email and password) to init the SuperProxy below
    engine.authorizationMethod = SOAP_AUTH_BASIC;
    engine.username = @"";
    engine.password = @"";
    
    return engine;
}

-(NSString*)getSoapAction:(NSString*)operation{
    return [NSString stringWithFormat:@"%@%@", SERVICE_NAMESPACE, operation];
}

-(NSString*)getJsonFromDictionary:(NSDictionary *)dictionary{
    NSError *error;
    // Pass 0 if you don't care about the readability of the generated string
    NSData *data = [NSJSONSerialization dataWithJSONObject:dictionary
                                                   options:0
                                                     error:&error];
    if (error != nil){
        NSLog(@"Error: %@", error);
        return nil;
    }
    
    return [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
}

-(NSString*)getJsonFromArray:(NSArray *)array{
    NSError *error;
    // Pass 0 if you don't care about the readability of the generated string
    NSData *data = [NSJSONSerialization dataWithJSONObject:array
                                                   options:0
                                                     error:&error];
    if (error != nil){
        NSLog(@"Error: %@", error);
        return nil;
    }
    
    return [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
}

-(NSDictionary*)getDictionary:(NSString *)json{
    NSData* data = [json dataUsingEncoding:NSUTF8StringEncoding];
    
    NSError *error;
    NSDictionary *dictionary = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if(error != nil){
        NSLog(@"Error: %@", error);
        return nil;
    }
    
    return dictionary;
}

- (void)didReceiveMemoryWarning{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

@end
