using System;
using System.Threading.Tasks;
using Youbetme.Communication;
using Youbetme.DataObjects;
using Youbetme.Proxies;
using Youbetme.Validation;
using System.Collections.Generic;


namespace Youbetme.Helpers
{
	public class LoginHelper
	{
		public static string SessionToken;
		public static Customer LocalCustomer;

		public static bool UserIsLoggedIn {
			get {
				return (
				    LocalCustomer != null
				    && string.IsNullOrWhiteSpace (SessionToken) == false);
			}
		}

		public const string AppId = "YouFlapMe";

		private static LoginHelper _singleton;

		private static LoginHelper singleton {
			get {
				if (_singleton == null)
					_singleton = new LoginHelper ();
				return _singleton;
			}
		}


		public static void SignUpAsync (string firstName, string lastName, string email, string password, Action onSuccess, Action<string> onFail)
		{
			Task.Factory.StartNew (() => {
				singleton.signUp (firstName, lastName, email, password, onSuccess, onFail);
			});
		}

		private void signUp (string firstName, string lastName, string email, string password, Action onSuccess, Action<string> onFail)
		{
			var customer = new Customer ();
			customer.FirstName = firstName;
			customer.LastName = lastName;
			customer.EMail = email;
			customer.PasswordHash = password;
			//customer.Context ["AppId"] = LoginHelper.AppId;

			var result = CustomerValidator.Validate (customer);

			var error = "Unknown Error";

			if (!result.IsValid) {
				if (result.Message != null)
					error = result.Message;
				if (onFail != null)
					onFail (error);
				return;
			}

			var response = CustomerProxy.CreateCustomer (customer);

			if (response.Status != Youbetme.Communication.Status.Success) {
				if (response.Message != null)
					error = response.Message;
				if (onFail != null)
					onFail (error);
				return;
			}

			LocalCustomer = response.Result;
			if (onSuccess != null)
				onSuccess ();
		}

		public static void LoginAsync (string email, string password, Action onSuccess, Action<string> onFail)
		{
			Content.Init (Configuration.CdnUri);

			Task.Factory.StartNew (() => {
				singleton.logIn (email, password, onSuccess, onFail);
			});
		}

		private void logIn (string email, string password, Action onSuccess, Action<string> onFail)
		{
			var loginResponse = CustomerProxy.LoginWithPassword (email, password);

			if (loginResponse.Status != Status.Success) {
				var error = "Unknown Error";
				if (loginResponse.Message != null)
					error = loginResponse.Message;
				if (onFail != null)
					onFail (error);
				return;
			}

			SessionToken = loginResponse.Result.Token;
			var customerResponce = CustomerProxy.GetCustomerFromSession (SessionToken);
			LocalCustomer = customerResponce.Result;
		
			if (onSuccess != null)
				onSuccess ();
		}

		public static void LoginWithFacebookAsync (Action onSuccess, Action<string> onFail)
		{
			Task.Factory.StartNew (() => {
				singleton.loginWithFacebook (onSuccess, onFail);
			});
		}

		private void loginWithFacebook (Action onSuccess, Action<string> onFail)
		{
			var network = new SocialNetwork ();
			network.Type = SocialNetworkType.Facebook;
			network.Permission = SocialNetworkPermission.Read;

			//TODO: Set Facebook access token and expiration date
			//network.Token = FBSession.ActiveSession.AccessTokenData.AccessToken;
			//network.ExpirationDate = FBSession.ActiveSession.AccessTokenData.ExpirationDate;

			var loginResponse = CustomerProxy.LoginWithFacebook (network);

			if (loginResponse.Status != Status.Success) {
				var error = "Unknown Error";
				if (loginResponse.Message != null)
					error = loginResponse.Message;
				if (onFail != null)
					onFail (error);
				return;
			}

			SessionToken = loginResponse.Result.Token;
			if (onSuccess != null)
				onSuccess ();
		}

		public static void GetCustomerFromTokenAsync (string token, Action onSuccess, Action<string> onFail)
		{
			if (string.IsNullOrWhiteSpace (token)) {
				if (onFail != null)
					onFail ("Token was blank");
				return;
			}

			Task.Factory.StartNew (() => {
				singleton.getCustomerFromToken (token, onSuccess, onFail);
			});
		}

		void getCustomerFromToken (string token, Action onSuccess, Action<string> onFail)
		{
			var response = CustomerProxy.GetCustomerFromSession (token);

			if (response.Status != Status.Success) {
				var error = "Unknown Error";
				if (string.IsNullOrWhiteSpace (response.Message) == false)
					error = response.Message;
				if (onFail != null)
					onFail (error);
				return;
			}

			LocalCustomer = response.Result;
			SessionToken = token;

			// Customer sucessfully retrieved
			if (onSuccess != null)
				onSuccess ();
		}

		static void UpdateContextCustomerAsync ()
		{
			var response = CustomerProxy.GetCustomerFromSession (SessionToken);
			if (response.Status != Status.Success) {
				// TODO: Display response.Message
				return;
			}

			var customer = response.Result;

			customer.FirstName = "new-first-name-here";

			response = CustomerProxy.UpdateCustomer (SessionToken, customer);
			if (response.Status != Status.Success) {
				// TODO: Display response.Message
				return;
			}

			customer = response.Result;

			// Customer sucessfully updated
		}

		static void GetCustomerFromId ()
		{
			var response = CustomerProxy.GetCustomerFromId (SessionToken, "customer-id-here");
			if (response.Status != Status.Success) {
				//TODO: Display response.Message
				return;
			}

			var customer = response.Result;

			// Customer sucessfully retrieved
		}
	}
}

