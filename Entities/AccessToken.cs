using System;

namespace RB.AuthorisationHold.ClientSample
{
	public class AccessToken
	{
		public string accessToken = "";
		public string tokenType = "";
		public int expiresIn = 0;
		public DateTime tokenCreated = DateTime.MinValue;
	}
}
