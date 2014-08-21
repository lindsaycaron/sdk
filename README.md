#SDK - Xamarin#

Integrating Youbetme into your own mobile app is easy with the Youbetme SDK. To do so with Xamarin you can use either Xamarin Studio or Visual Studio (http://xamarin.com/platform). The Youbetme SDK consists of four C# PCLs (portable class libraries) targeting PCL profile 7 (.NET Framework 4.5, Windows 8, Windows Store, Xamarin.Android and Xamarin.iOS). Each library contains a logically related subset of types that you’ll need to interact with the Youbetme backend:

- **Youbetme.DataObjects**
Contains all domain objects like customers, bets, posts, sports teams and events, friends, followers, news feed stories, reminders and more. 

- **Youbetme.Validation**
Contains a few validation utility types that you will need to ensure the data you submit to the Youbetme backend is valid. The content you can create include customers, bets and posts. For each one of these types a validator is available.

- **Youbetme.Communication** 
Contains the generic Response<T> type which carries data and useful information when communicating with the backend (status report, important messages, and the data result). 

- **Youbetme.Proxies**
These are the types that will submit content and queries for you to the Youbetme backend API. They take care of data serialization and deserialization and provide both blocking and async calls.

##Getting Started##

To integrate Youbetme into your app, reference the Youbetme SDK libraries and add the using directives below to the AppDelegate of your project:

```
using Youbetme.Communication;
using Youbetme.DataObjects;
using Youbetme.Proxies;
using Youbetme.Validation;
```

To initialize the SDK add the following piece of code to the AppDelegate of your app, providing your api token and secret (the email and password you use to login to this portal):

```
Content.Init (Configuration.CdnUri);
SuperProxy.Init (Configuration.ServiceUri, "api-token-here", "api-secret-here");
```

##Creating a Customer##

Before you start querying the API you need a customer on whose behalf you will be running operations. In the documentation below the customer on whose behalf you run queries will be known as the “context customer”. To create a new customer, use the lines below:

```
var customer = new Customer ();
customer.FirstName = "first-name-here";
customer.LastName = "last-name-here";
customer.EMail = "email-here";
customer.PasswordHash = "plain-text-password-here";

var result = CustomerValidator.Validate (customer);
if (!result.IsValid)
{
     //TODO: Display result.Message
     return;
}

var response = CustomerProxy.CreateCustomer (customer);
if (response.Status != Status.Success)
{
     //TODO: Display response.Message
     return;
}

customer = response.Result;
```

##Logging the Customer In##

Once you have created a new customer, call the method below to log in to Youbetme and receive an access token. You will need this token to later authenticate your requests against the backend. Store it to a convenient variable for later use:

```
var loginResponse = CustomerProxy.LoginWithPassword ("john.miller@gmail.com", "m1ll3r");

if (loginResponse.Status != Status.Success)
{
     //TODO: Display loginResponse.Message
     return;
}

Token = loginResponse.Result.Token;

```

##Retrieving Customer Data##

Use the lines below to retrieve the customer data. All you need to provide is the access token you received after the successful login. Storing the data to a convenient variable would make it available for later use:

```
var response = CustomerProxy.GetCustomerFromSession (Token);

if(response.Status != Status.Success)
{
     //TODO: Display response.Message
     return;
}

Customer = response.Result;
```

##Retrieving a Customer’s Friends, Followers and Followees##

To learn how to retrieve customer’s friends, followers or followees (i.e. subscriptions, those that the customer is following) refer to the GetFriends, GetFollowers and GetFollowees methods of the App type in the FormsSample project. 


##Searching for Customers##

To learn how to specify criteria and perform searches in the Youbetme customers database refer to the PerformSearch method of the App type in the FormsSample project.


##Bets##

- **New Bets**: Refer to the CreateBet method of the App type in the portable FormsSample project.

- **Accepting Bets**: The newly created bet is in Pending state and it will remain so until every bet member accepts the bet. Refer to the AcceptBetAsync method of the App type in the FormsSample project.

- **Closing Bets**: Once a bet has been accepted by all members it becomes Active. Every bet member can close the bet by specifying the winning team and whether the bet has been paid. Refer to the CloseBetAsync method of the App type in the FormsSample project. If a bet is to be paid it will enter the PayUp state; otherwise it will become either Paid or Welched. 

- **Retrieving Bets**: Refer to the GetPendingBetsAsync, GetActiveBetsAsync, GetPayUpBetsAsync and GetClosedBetsAsync of the App type from the FormsSample project. 

- **Posting Text**: Youbetme customers are able to post comments below bets. To comment on a bet, use the lines below:

```
var post = new Post ();
post.BetId = bet.Id;
post.CustomerId = ContextCustomer.Id;
post.Text = "Hey! That's a really cool bet you created";

var result = PostValidator.Validate (post);
if (!result.IsValid) 
{
	// TODO: Display result.Message
	return;
} 

PostProxy.CreateAsync (Token, post, null, null);
```

- **Posting Photos**: To post a photo below a bet, use the code below:

```
var post = new Post ();
post.BetId = bet.Id;
post.CustomerId = ContextCustomer.Id;
// TODO: Set PNG image data below
// post.ImageData = data;

var result = PostValidator.Validate (post);
if (!result.IsValid)
{
     // TODO: Display result.Message
     return;
}

PostProxy.CreateAsync (Token, post, post.ImageData, null);
```

- **Storing Custom Data**: To see how to store custom bet, user and post data refer to the CreateCustomerWithContextData, CreateBetWithContextData and CreatePostWithContextData methods of the App type in the FormsSample project. 

##News Feed##

To see how to get the customer’s news feed stories refers to the GetNewsFeedAsync method of the App type in the FormsSample project.

