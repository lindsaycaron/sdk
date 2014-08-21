using System;
using Youbetme.Communication;
using Youbetme.DataObjects;
using Youbetme.Proxies;
using Youbetme.Validation;
using System.Collections.Generic;

namespace Youbetme.Helpers
{
	public class FriendsHelper
	{

		public static void GetIdFromEmailAsync (string email, Action<string> onSuccess, Action<string> onFail)
		{
			var filter = new CustomerFilter ();
			filter.CustomerId = LoginHelper.LocalCustomer.Id;
			filter.PageNumber = 0;
			filter.PageSize = 50;
			// Note: The parameter below is required
			filter.Criteria = email;
			FriendsHelper.onSuccess = onSuccess;
			FriendsHelper.onFail = onFail;

			CustomerProxy.PerformSearchCompleted += handlePerformSearchCompleted;
			CustomerProxy.PerformSearchAsync (LoginHelper.SessionToken, filter, null);
		}

		private static Action<string> onSuccess;
		private static Action<string> onFail;

		private static void handlePerformSearchCompleted (Response<List<CustomerLight>> response, object asyncState)
		{
			CustomerProxy.PerformSearchCompleted -= handlePerformSearchCompleted;
			if (response.Status != Status.Success) {
				var errorMsg = "UnKnown Error";
				if (string.IsNullOrWhiteSpace (response.Message) == false)
					errorMsg = response.Message;
				if (onFail != null)
					onFail (errorMsg);
				return;
			}

			var customers = response.Result;

			// Search sucessfully performed
			if (customers.Count > 0 && onSuccess != null)
				onSuccess (customers [0].Id);
		}

	}
}

