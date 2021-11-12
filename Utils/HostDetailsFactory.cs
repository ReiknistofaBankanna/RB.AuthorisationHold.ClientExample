using RB.AuthorisationHold.ClientSample.Entities.Enums;
using System.Collections.Generic;

namespace RB.AuthorisationHold.ClientSample
{
	public class HostDetails
	{
		public string baseAddress;
		public string clientId;
		public string clientSecret;
		public string resource;
		public string tokenAddress;
	}

	public class HostDetailsFactory
	{
		private static readonly Dictionary<HostType, HostDetails> hosts = new()
		{
			{
				HostType.Test,
				new()
				{
					baseAddress = "https://.../AuthorisationHold/",
					clientId = "aha12345-1234-1234-1234-123456789012",
					clientSecret = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
					resource = "urn:api.AAAAAAAAAAAAAA",
					tokenAddress = "https://sts..."
				}
			},
			{
				HostType.Prod,
				new()
				{
					baseAddress = "https://.../AuthorisationHold/",
					clientId = "aha12345-1234-1234-1234-123456789012",
					clientSecret = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
					resource = "urn:api.AAAAAAAAAAAAAA",
					tokenAddress = "https://sts..."
				}
			}
		};

		public static HostDetails GetHostDetails(HostType type)
		{
			return HostDetailsFactory.hosts[type];
		}
	}
}