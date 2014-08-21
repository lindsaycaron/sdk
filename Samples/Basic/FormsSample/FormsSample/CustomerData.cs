using System;
using Parse.Api.Models;

namespace FormsSample
{
	public class CustomerData : ParseObject
	{
		public string Id { get; set; }
		public string City { get; set; }
	}
}

