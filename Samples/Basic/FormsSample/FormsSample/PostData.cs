using System;
using Parse.Api.Models;

namespace FormsSample
{
	public class PostData : ParseObject
	{
		public string Id { get; set; }
		public int PhotoWidth { get; set; }
		public int PhotoHeight { get; set; }
	}
}

