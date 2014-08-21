using System;
using Parse.Api.Models;

namespace FormsSample
{
	class BetData : ParseObject
	{
		public string Id { get; set;}
		public DateTime AcceptBetDueDate { get; set; }
	}
}

