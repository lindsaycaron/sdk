//
//  BetStatus.h
//  iOSSample
//
//  Created by Diana Zhivkova on 8/21/14.
//  Copyright (c) 2014 Social Bet Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef enum {
    StatusFailure = 0,
    StatusSuccess = 1,
    StatusNewerApi = 2,
    StatusAuthenticationFailed = 3,
    StatusCanceled = 4,
    StatusUnderMaintenance = 5
} Status;

typedef enum {
    BetStatusPending  = 0,
    BetStatusActive = 1,
    BetStatusPayup = 2,
    BetStatusPaid = 3,
    BetStatusRejected = 4,
    BetStatusInProgress = 5,
    BetStatusWelched = 6
} BetStatus;

typedef enum
{
    BetPrivacyAll = 0,
    BetPrivacyFriends = 1,
    BetPrivacyPrivate = 2
} BetPrivacy;

typedef enum
{
    BetTypeCustom = 0,
    BetTypeSports = 1,
    BetTypePrediction = 2
} BetType;

typedef enum
{
    TeamSideBetMaker = 0,
    TeamSideOpponent = 1,
    TeamSideAudience = 255
}TeamSide;

typedef enum
{
    TeamStatusNone = 0,
    TeamStatusLost = 1,
    TeamStatusWon = 2
} TeamStatus;

typedef enum
{
    BetSuggestionCustomBetTitle = 0,
    BetSuggestionPredictionTitle = 1,
    BetSuggestionOutcome = 2,
    BetSuggestionTerms = 3
} BetSuggestionType;

typedef enum
{
    CustomerStatusActive = 0,
    CustomerStatusInvited = 1,
    CustomerStatusDeleted = 2
} CustomerStatus;

typedef enum
{
    FollowerStatusNone = 0,
    FollowerStatusFollow = 1,
    FollowerStatusUnfollow = 2
} FollowerStatus;

typedef enum
{
    FriendshipStatusNone = 0,
    FriendshipStatusPending = 1,
    FriendshipStatusAccepted = 2,
    FriendshipStatusRejected = 3,
    FriendshipStatusRemoved = 4
} FriendshipStatus;

typedef enum
{
    MemberNotificationStatusNone = 0, // Notifications read.
    MemberNotificationStatusPending = 1, // Notification(s) pending.
} MemberNotificationStatus;

typedef enum
{
    NewsFeedItemTypeBetPending = 0,
    NewsFeedItemTypeBetActive = 1,
    NewsFeedItemTypeBetRejected = 2,
    NewsFeedItemTypeBetWon = 3,
    NewsFeedItemTypeBetLost = 4,
    NewsFeedItemTypeBetPaid = 5,
    NewsFeedItemTypeBetWelched = 6,
    NewsFeedItemTypeBetPhoto = 7,
    NewsFeedItemTypePostText = 8,
    NewsFeedItemTypePostPhoto = 9,
    NewsFeedItemTypePredictionMade = 10,
    NewsFeedItemTypeHumor = 11,
    NewsFeedItemTypeSoccer = 12,
    NewsFeedItemTypeNBA = 13,
    NewsFeedItemTypeNFL = 14,
    NewsFeedItemTypePGA = 15,
    NewsFeedItemTypeMLB = 16,
    NewsFeedItemTypeNASCAR = 17,
    NewsFeedItemTypeInviteFacebookFriends = 18,
    NewsFeedItemTypeInvitePhonebookContacts = 19,
    NewsFeedItemTypeAddProfilePhoto = 20,
    NewsFeedItemTypeAddMobileNumber = 21,
    NewsFeedItemTypeNewCustomBet = 22,
    NewsFeedItemTypeNewSportBet = 23,
    NewsFeedItemTypeNewPrediction = 24,
    NewsFeedItemTypeFindFriends = 25
} NewsFeedItemType;

typedef enum
{
    NewsFeedItemStatusRecent = 0,
    NewsFeedItemStatusOld = 1
} NewsFeedItemStatus;

typedef enum
{
    NewsTypePublication,
    NewsTypeHumor
} NewsType;

typedef enum
{
    iOS = 0,
    Android = 1
} DeviceOperatingSystem;

typedef enum
{
    Facebook = 0,
    Twitter = 1,
    Tumblr = 2
} SocialNetworkType;

typedef enum
{
    SocialNetworkPermissionRead = 0,
    SocialNetworkPermissionWrite = 1
} SocialNetworkPermission;

typedef enum
{
    BetLineTypeNone,
    BetLineTypeHomeOU,
    BetLineTypeAwayOU,
    BetLineTypeOver,
    BetLineTypeUnder,
    BetLineTypeAwayWin,
    BetLineTypeHomeWin
} BetLineType;


#define SportEventStatePending = "PENDING"
#define SportEventStateCircled = "CIRCLED"
#define SportEventStatePostpone = "POSTPONE"
#define SportEventStateSuspended = "SUSPENDED"
#define SportEventStateRainDelay = "RAIN DELAY"
#define SportEventStateDelay = "DELAY"
#define SportEventStateCancelled = "CANCELLED"
#define SportEventStateStarted = "STARTED"
#define SportEventStateFinal = "FINAL"

#define BetLineTypePrevious = "PREVIOUS"
#define BetLineTypeCurrent = "CURRENT"