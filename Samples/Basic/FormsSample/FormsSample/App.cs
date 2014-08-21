using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;

using Youbetme.Communication;
using Youbetme.DataObjects;
using Youbetme.Validation;
using Youbetme.Proxies;

using Parse.Api;
using Parse.Api.Models;

namespace FormsSample
{
	public class App
	{
		static string Token;
		static Customer ContextCustomer;

		static ParseRestClient ParseClient;

		public static Page GetMainPage ()
		{	
			// Initialization 
			Content.Init (Configuration.CdnUri);

			// TODO: Provide your API token and secret below
			// Note: To obtain a valid API token and secret sign up at http://dev.youbetme.com/portal
			// Then contact a Youbetme representative or sent email to support@youbetme.com to request access to the API
			// Once your account has been granted access, use your credentails (email and password) to init the SuperProxy below
			SuperProxy.Init (Configuration.ServiceUri, "api-token-here", "api-secret-here");

			// TODO: Provide your Parse Application Id and Rest API Key below
			// Note: If you need to attach custom data to customer, bet and post objects, we recommend parse.com for storage
			// To obtain a valid Parse Application Id and Rest API key sign up at https://parse.com and create an application (it would take less than a minute)
			// From the popup copy the Application Id and the Rest API Key 
			ParseClient = new ParseRestClient ("app-id-here", "rest-api-key-kere");

			return new ContentPage { 
				Content = new Label {
					Text = "Hello, Forms!",
					VerticalOptions = LayoutOptions.CenterAndExpand,
					HorizontalOptions = LayoutOptions.CenterAndExpand,
				},
			};
		}

		static void SignUp()
		{
			var customer = new Customer ();
			// TODO: Set first, last names, email and password
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
			if (response.Status != Youbetme.Communication.Status.Success) 
			{
				//TODO: Display response.Message
				return;
			}

			ContextCustomer = response.Result;

			// Customer successfully created
		}

		static void LogIn()
		{
			var loginResponse = CustomerProxy.LoginWithPassword ("john.miller@gmail.com", "m1ll3r");

			if (loginResponse.Status != Status.Success)
			{
				//TODO: Display loginResponse.Message
				return;
			}

			Token = loginResponse.Result.Token;

			// Customer successfully logged in 
		}

		static void LoginWithFacebook()
		{
			var network = new SocialNetwork ();
			network.Type = SocialNetworkType.Facebook;
			network.Permission = SocialNetworkPermission.Read;

			//TODO: Set Facebook access token and expiration date
			network.Token = "facebook-access-token-here"; //FBSession.ActiveSession.AccessTokenData.AccessToken;
			network.ExpirationDate = DateTime.UtcNow.AddDays (40); //FBSession.ActiveSession.AccessTokenData.ExpirationDate;

			var loginResponse = CustomerProxy.LoginWithFacebook (network);

			if (loginResponse.Status != Status.Success) 
			{
				//TODO: Show message
				return;
			}

			Token = loginResponse.Result.Token;
		}

		static void GetCustomerFromToken()
		{
			var response = CustomerProxy.GetCustomerFromSession (Token);

			if(response.Status != Status.Success)
			{
				//TODO: Display response.Message
				return;
			}

			ContextCustomer = response.Result;

			// Customer sucessfully retrieved
		}

		static void UpdateContextCustomerAsync()
		{
			var response = CustomerProxy.GetCustomerFromSession (Token);
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			var customer = response.Result;

			// TODO: Provide new value here
			customer.FirstName = "new-first-name-here";

			response = CustomerProxy.UpdateCustomer (Token, customer);
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			customer = response.Result;

			// Customer sucessfully updated
		}

		static void GetCustomerFromId()
		{
			//TODO: Provide customer Id below
			var response = CustomerProxy.GetCustomerFromId (Token, "customer-id-here");
			if (response.Status != Status.Success) 
			{
				//TODO: Display response.Message
				return;
			}

			var customer = response.Result;

			// Customer sucessfully retrieved
		}

		static void GetRecentFriends()
		{
			var filter = new CustomerFilter ();
			filter.CustomerId = ContextCustomer.Id;
			filter.PageNumber = 0;
			filter.PageSize = 50;
			// Note: The parameter below is optional and is needed in case you want to search by first, last name or email
			// filter.Criteria = "John";

			var response = CustomerProxy.GetRecentFriends (Token, filter);
			if (response.Status != Status.Success) 
			{
				//TODO: Display response.Message
				return;
			}

			var friends = response.Result;

			// Recent friends sucessfully retrieved
		}

		static void GetFriends()
		{
			var filter = new CustomerFilter ();
			filter.CustomerId = ContextCustomer.Id;
			filter.PageNumber = 0;
			filter.PageSize = 50;
			// Note: The parameter below is optional and is needed in case you want to search by first, last name or email
			// filter.Criteria = "John";

			var response = CustomerProxy.GetFriends (Token, filter);
			if (response.Status != Status.Success) 
			{
				//TODO: Display response.Message
				return;
			}

			var friends = response.Result;

			// Friends sucessfully retrieved
		}

		static void GetFollowers()
		{
			var filter = new CustomerFilter ();
			filter.CustomerId = ContextCustomer.Id;
			filter.PageNumber = 0;
			filter.PageSize = 50;
			// Note: The parameter below is optional and is needed in case you want to search by first, last name or email
			// filter.Criteria = "John";

			var response = CustomerProxy.GetFollowers (Token, filter);
			if (response.Status != Status.Success) 
			{
				//TODO: Display response.Message
				return;
			}

			var followers = response.Result;

			// Followers sucessfully retrieved
		}

		static void GetFollowees()
		{
			var filter = new CustomerFilter ();
			filter.CustomerId = ContextCustomer.Id;
			filter.PageNumber = 0;
			filter.PageSize = 50;
			// Note: The parameter below is optional and is needed in case you want to search by first, last name or email
			// filter.Criteria = "John";

			var response = CustomerProxy.GetFollowees (Token, filter);
			if (response.Status != Status.Success) 
			{
				//TODO: Display response.Message
				return;
			}

			var followees = response.Result;

			// Followees sucessfully retrieved
		}

		static void PerformSearchAsync()
		{
			var filter = new CustomerFilter ();
			filter.CustomerId = ContextCustomer.Id;
			filter.PageNumber = 0;
			filter.PageSize = 50;
			// Note: The parameter below is required
			// TODO: Provide search criteria below
			filter.Criteria = "Sarah Johnson";

			CustomerProxy.PerformSearchCompleted += HandlePerformSearchCompleted;
			CustomerProxy.PerformSearchAsync (Token, filter, null);
		}

		static void HandlePerformSearchCompleted (Response<List<CustomerLight>> response, object asyncState = null)
		{ 
			if (response.Status != Status.Success) 
			{
				//TODO: Display response.Message
				return;
			}

			var customers = response.Result;

			// Search sucessfully performed
		}

		static void GetBetSuggestions()
		{
			var response = BetSuggestionProxy.GetSuggestions (Token);
			if (response.Status != Status.Success) 
			{
				//TODO: Display response.Message
				return;
			}

			var suggestions = response.Result;

			// Note: Use the line below to get bet title suggestions
			var betTitles = suggestions.Where (it => it.Type == SuggestionType.CustomBetTitle).ToList ();

			// Note: Use the line below to get bet terms suggestions
			var betTerms = suggestions.Where (it => it.Type == SuggestionType.Terms).ToList ();

			// Note: Use the line below to get bet outcome suggestions
			var betOutcome = suggestions.Where (it => it.Type == SuggestionType.Outcome).ToList ();

			// Note: Use the line below to get prediction suggestions
			var predictions = suggestions.Where (it => it.Type == SuggestionType.PredictionTitle).ToList ();
		}
	
		static void CreateBet()
		{
			var filter = new CustomerFilter ();
			filter.CustomerId = ContextCustomer.Id;
			filter.PageNumber = 0;
			filter.PageSize = 50;

			var getFriendsResponse = CustomerProxy.GetFriends (Token, filter);

			if (getFriendsResponse.Status != Status.Success) 
			{
				//TODO: Display getFriendsResponse.Message
				return;
			}

			var friends = getFriendsResponse.Result;

			var bet = new Bet ();

			// Specify the bet maker Id
			bet.CustomerId = ContextCustomer.Id;

			// Specify bet type 
			bet.Type = BetType.Custom;

			bet.BetTitle = "My First Bet";
			bet.BetTerms = "Gets created";
			bet.BetOutcome = "High Five";

			// Create the bet maker team
			var betMakerTeam = new Team ();
			betMakerTeam.Side = TeamSide.BetMaker;

			// Add the bet maker to the team
			var betMaker = new Member ();
			betMaker.CustomerId = ContextCustomer.Id;
			betMaker.BetStatus = BetStatus.Active;
			betMakerTeam.Members.Add (betMaker);

			// Optionally add a teammate
			var teammate = new Member ();
			teammate.CustomerId = friends [0].Id;
			betMakerTeam.Members.Add (teammate);

			bet.Teams.Add (betMakerTeam);

			// Create the opponent team
			var opponentTeam = new Team ();
			opponentTeam.Side = TeamSide.Opponent;

			// Add the first opponent
			var opponent1 = new Member ();
			opponent1.CustomerId = friends [1].Id;
			opponentTeam.Members.Add (opponent1);

			// Opptionally add another opponent
			var opponent2 = new Member ();
			opponent2.CustomerId = friends [2].Id;
			opponentTeam.Members.Add (opponent2);

			bet.Teams.Add (opponentTeam);

			// Optionally add a team of spectators
			var audience = new Team ();
			audience.Side = TeamSide.Audience;

			// Add as many spectators as needed
			var spectator1 = new Member ();
			spectator1.CustomerId = friends [3].Id;
			audience.Members.Add (spectator1);

			bet.Teams.Add (audience);

			// Note: Optionally add bet location
			var location = new LocationData ();
			// TODO: populate location properties with real values
			// bet.Location.Latitude = 0;
			// bet.Location.Longitude = 0;
			// bet.Location.Altitude = 0;
			bet.Location = location;

			// Note: Set "uploadInProgress" to true if you're about to submit a photo attached to the bet
			var response = BetProxy.CreateNewBet (Token, bet, true);
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			bet = response.Result;

			// Note: The call below is needed if you want to attach a photo to the bet immeadiately after creating it
			// Note: You need to set the postCreateUpload parameter to true
			// TODO: Set data to PNG image byet array
			var data = new byte[10];
			ImageProxy.UploadBetImageAsync (Token, bet.Id, data, true, null);
		}
	
		static void GetPendingBetsAsync()
		{
			BetProxy.GetBetsLightCompleted += HandleGetBetsLightCompleted;

			var filter = new BetFilter ();
			filter.CustomerId = ContextCustomer.Id;
			filter.Statuses = new BetStatus[] { BetStatus.Pending };
			filter.PageNumber = 0;
			filter.PageSize = 50;

			BetProxy.GetBetsLightAsync (Token, filter, null);
		}

		static void GetActiveBetsAsync()
		{
			BetProxy.GetBetsLightCompleted += HandleGetBetsLightCompleted;

			var filter = new BetFilter ();
			filter.CustomerId = ContextCustomer.Id;
			filter.Statuses = new BetStatus[] { BetStatus.Active };
			filter.PageNumber = 0;
			filter.PageSize = 50;

			BetProxy.GetBetsLightAsync (Token, filter, null);
		}

		static void GetPayUpBetsAsync()
		{
			BetProxy.GetBetsLightCompleted += HandleGetBetsLightCompleted;

			var filter = new BetFilter ();
			filter.CustomerId = ContextCustomer.Id;
			filter.Statuses = new BetStatus[] { BetStatus.Payup };
			filter.PageNumber = 0;
			filter.PageSize = 50;

			BetProxy.GetBetsLightAsync (Token, filter, null);
		}

		static void GetClosedBetsAsync()
		{
			BetProxy.GetBetsLightCompleted += HandleGetBetsLightCompleted;

			var filter = new BetFilter ();
			filter.CustomerId = ContextCustomer.Id;
			filter.Statuses = new BetStatus[] { BetStatus.Paid, BetStatus.Welched };
			filter.PageNumber = 0;
			filter.PageSize = 50;

			BetProxy.GetBetsLightAsync (Token, filter, null);
		}

		static void HandleGetBetsLightCompleted (Response<List<BetLight>> response, object asyncState = null)
		{
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			var bets = response.Result;

			// Bets sucessfully retrieved
		}

		static void UploadBetPhoto()
		{
			// TODO: Set data to PNG image byte array
			var data = new byte[10];

			ImageProxy.UploadBetImageCompleted += HandleUploadBetImageCompleted;

			// TODO: Provide bet Id below
			ImageProxy.UploadBetImageAsync (Token, "bet-id-here", data, false, null);
		}

		static void HandleUploadBetImageCompleted (Response<string> response, object asyncState = null)
		{
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			// Image successfully uploaded
		}

		static void AcceptBetAsync()
		{
			// TODO: Provide bet Id below
			var response = BetProxy.GetBet (Token, "bet-id-here");
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			var bet = response.Result;

			BetProxy.SetMemberStatusCompleted += HandleSetMemberStatusCompleted;
			BetProxy.SetMemberStatusAsync(Token, ContextCustomer.Id, bet.Id, BetStatus.Active, null);
		}

		static void DenyBetAsync()
		{
			// TODO: Provide bet Id below
			var response = BetProxy.GetBet (Token, "bet-id-here");
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			var bet = response.Result;

			BetProxy.SetMemberStatusCompleted += HandleSetMemberStatusCompleted;
			BetProxy.SetMemberStatusAsync(Token, ContextCustomer.Id, bet.Id, BetStatus.Rejected, null);
		}

		static void CancelBetAsync()
		{
			// TODO: Provide bet Id below
			var response = BetProxy.GetBet (Token, "bet-id-here");
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			var bet = response.Result;

			BetProxy.SetMemberStatusCompleted += HandleSetMemberStatusCompleted;
			BetProxy.SetMemberStatusAsync(Token, ContextCustomer.Id, bet.Id, BetStatus.Rejected, null);
		}

		static void HandleSetMemberStatusCompleted (Response<Bet> response, object asyncState = null)
		{
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Mesage
				return;
			}

			// Member status successfully changed
		}
	
		static void CloseBetAsync()
		{
			// TODO: Provide bet Id below
			var response = BetProxy.GetBet (Token, "bet-id-here");
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			var bet = response.Result;

			var betMakerTeam = bet.GetTeamBySide (TeamSide.BetMaker);
			var opponentTeam = bet.GetTeamBySide (TeamSide.Opponent);

			// Note: Set winner and loser
			betMakerTeam.Status = TeamStatus.Won;
			opponentTeam.Status = TeamStatus.Lost;

			// Note: Set whether the bet has been paid or welched by changing its status
			bet.Status = BetStatus.Paid;

			BetProxy.CloseBetCompleted += HandleCloseBetCompleted;
			BetProxy.CloseBetAsync(Token, bet, null);
		}
	
		static void HandleCloseBetCompleted (Response<Bet> response, object asyncState = null)
		{
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			// Bet sucessfully closed
		}

		static void GetPublicBetsAsync()
		{
			BetProxy.GetBetsCompleted += HandleGetBetsCompleted;

			var filter = new BetFilter ();
			// Note: Optionally use the line below to limit bets by status
			filter.Statuses = new BetStatus[] { BetStatus.Active };
			// Note: Optionally use the line below to limit bets by type
			filter.Types = new BetType[] { BetType.Custom, BetType.Sports };
			// Note: Specify page number and size
			filter.PageNumber = 0;
			filter.PageSize = 50;

			BetProxy.GetBetsAsync (Token, filter, null);
		}

		static void HandleGetBetsCompleted (Response<List<Bet>> response, object asyncState = null)
		{
			if (response.Status != Status.Success) 
			{
				// TODO: display message
				return;
			}

			// Public bets sucessfully retrieved
		}
	
		static void GetNewsFeedAsync()
		{
			var filter = new NewsFeedItemFilter ();
			filter.CustomerId = ContextCustomer.Id;
			filter.PageNumber = 0;
			filter.PageSize = 50;

			NewsFeedItemProxy.GetNewsFeedItemsCompleted += HandleGetNewsFeedItemsCompleted;
			NewsFeedItemProxy.GetNewsFeedItemsAsync (Token, filter, null);
		}

		static void HandleGetNewsFeedItemsCompleted (Response<List<NewsFeedItem>> response, object asyncState = null)
		{
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			// News feed sucessfully retrieved
		}
	
		static void PostText()
		{
			// TODO: Provide bet Id below
			var response = BetProxy.GetBet (Token, "bet-id-here");
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			var bet = response.Result;

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

			PostProxy.CreateCompleted += HandlePostTextCompleted;
			PostProxy.CreateAsync (Token, post, null);
		}

		static void HandlePostTextCompleted (Response<Post> resposne, object asyncState = null)
		{
			if (resposne.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			// Text sucessfully posted
		}

		static void PostPhoto()
		{
			// TODO: Provide bet Id below
			var response = BetProxy.GetBet (Token, "bet-id-here");
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			var bet = response.Result;

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

			PostProxy.CreateCompleted += HandlePostPhotoCompleted;
			PostProxy.CreateAsync (Token, post, null);
		}

		static void HandlePostPhotoCompleted (Response<Post> resposne, object asyncState = null)
		{
			if (resposne.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			// Photo sucessfully posted
		}

		static void SetDevice()
		{
			// TODO: Set below the device token or registration id
			var deviceToken = "device-token-here";

			// TODO: Set below the device OS
			var system = DeviceOperatingSystem.iOS;

			CustomerProxy.BeginSetDevice (deviceToken, system, ContextCustomer.Id, HandleSetDeviceTokenCompleted, null);
		}

		static void HandleSetDeviceTokenCompleted(Response<Youbetme.DataObjects.Device> response, object asyncState)
		{
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}
		}
	
		static void CreateCustomerWithContextData()
		{
			var customer = new Customer ();
			// TODO: Set first, last names, email and password below
			customer.FirstName = "first-name-here";
			customer.LastName = "last-name-here";
			customer.EMail = "email-here";
			customer.PasswordHash = "plain-text-password-here";

			var result = CustomerValidator.Validate (customer);
			if (!result.IsValid) 
			{
				//TODO: Display response.Message
				return;
			}

			var response = CustomerProxy.CreateCustomer (customer);
			if (response.Status != Youbetme.Communication.Status.Success) 
			{
				//TODO: Display response.Message
				return;
			}

			// Customer successfully created

			customer = response.Result;

			var data = new CustomerData ();
			data.Id = customer.Id;
			data.City = "Seattle";

			var parseResponse = ParseClient.CreateObject (data);
			if (parseResponse.Exception != null) 
			{
				// TODO: Display message
				return;
			}

			// Customer data successfully created
		}

		static void UpdateCustomerWithContextData()
		{
			// TODO: Provide Youbetme customer Id below
			var getResponse = ParseClient.GetObjects<CustomerData> (new { Id = "youbetme-customer-id-here" });
			if (getResponse.Exception != null) 
			{
				// TODO: Display message
				return;
			}

			var customerData = getResponse.Results [0];
			customerData.City = "San Francisco";

			var updateResponse = ParseClient.Update (customerData);
			if (updateResponse.Exception != null) 
			{
				// TODO: Display message
				return;
			}

			// Customer data sucessfully updated
		}

		static void CreateBetWithContextData()
		{
			var bet = new Bet ();

			// Specify the bet maker Id
			bet.CustomerId = ContextCustomer.Id;

			// Specify bet type 
			bet.Type = BetType.Custom;

			bet.BetTitle = "My Firts Bet With Custom Data";
			bet.BetTerms = "Gets Created";
			bet.BetOutcome = "Drinks";

			var team = new Team ();
			team.Side = TeamSide.BetMaker;
			var member = new Member ();
			member.CustomerId = ContextCustomer.Id;
			member.BetStatus = BetStatus.Active;
			team.Members.Add (member);
			bet.Teams.Add (team);

			team = new Team ();
			team.Side = TeamSide.Opponent;
			member = new Member ();
			// TODO: Provide opponent customer Id below
			member.CustomerId = "opponent-id-here";
			member.BetStatus = BetStatus.Pending;
			team.Members.Add (member);
			bet.Teams.Add (team);

			var result = BetValidator.Validate (bet);
			if (!result.IsValid) 
			{
				// TODO: Display result.Message
				return;
			}

			var response = BetProxy.CreateNewBet (Token, bet, false);
			if (response.Status != Status.Success) 
			{
				// TODO: Display response.Message
				return;
			}

			bet = response.Result;

			// Bet sucessfully created

			var data = new BetData ();
			data.Id = bet.Id;
			data.AcceptBetDueDate = DateTime.UtcNow.AddDays (1);

			var parseResponse = ParseClient.CreateObject (data);

			if (parseResponse.Exception != null) 
			{
				// TODO: Display message
				return;
			}

			// Bet data successfully created
		}

		static void GetBetContextData()
		{
			// TODO: Provide Youbetme bet Id below
			var parseResponse = ParseClient.GetObjects<BetData> (new { Id = "youbetme-bet-id-here" });
			if (parseResponse.Exception != null) 
			{
				// TODO: Display message
				return;
			}

			var betData = parseResponse.Results [0];

			// Bet data sucessfully retrieved
		}
	
		static void CreatePostWithContextData()
		{
			var post = new Post ();
			post.CustomerId = ContextCustomer.Id;
			// TODO: Provide bet Id below
			post.BetId = "bet-id-here";
			// TODO: Set PNG image data below 
			//post.ImageData = data;

			var result = PostValidator.Validate (post);
			if (!result.IsValid) 
			{
				//TODO: Display response.Message
				return;
			}

			PostProxy.CreateCompleted += HandleCustomPostCreateCompleted;
			PostProxy.CreateAsync (Token, post, null);
		}

		static void HandleCustomPostCreateCompleted (Response<Post> response, object asyncState = null)
		{
			if (response.Status != Status.Success) 
			{
				// TODO: Display resposne.Message
				return;
			}

			// Post successfully created

			var post = response.Result;

			var data = new PostData ();
			data.Id = post.Id;
			data.PhotoWidth = 270;
			data.PhotoHeight = 180;

			var parseResponse = ParseClient.CreateObject (data);
			if (parseResponse.Exception != null) 
			{
				// TODO: Display message
				return;
			}

			// Post data successfully created
		}

		static void DeletePostContextData()
		{
			// TODO: Provide Youbetme post Id below
			var getResponse = ParseClient.GetObjects<PostData> (new { Id = "youbetme-post-id-here" });
			if (getResponse.Exception != null) 
			{
				// TODO: Display message
				return;
			}

			var postData = getResponse.Results [0];

			var deleteResponse = ParseClient.DeleteObject (postData);
			if (deleteResponse.Exception != null) 
			{
				// TODO: Display message
				return;
			}

			// Post data successfully deleted
		}
	}
}

