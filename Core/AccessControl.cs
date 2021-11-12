using Newtonsoft.Json.Linq;
using RB.AuthorisationHold.ClientSample.Entities.Enums;
using RB.AuthorisationHold.ClientSample.Utils;
using RestSharp;
using System;

namespace RB.AuthorisationHold.ClientSample.Core
{
	public class AccessControl
	{
		private readonly AccessToken _token = new();
		private readonly HostDetails _hostDetails;

		public AccessControl()
		{
			_hostDetails = HostDetailsFactory.GetHostDetails(HostType.Test);
			GenerateToken();
		}

		public AccessControl(HostType type)
		{
			_hostDetails = HostDetailsFactory.GetHostDetails(type);
			GenerateToken();
		}

		public HostDetails GetHostDetails()
		{
			return _hostDetails;
		}

		/// <summary>
		/// Athugar hvort að token sé gilt, nær í eftir þörfum, skilar gildu JWT.
		/// </summary>
		/// <returns></returns>
		public string GetAccessToken()
		{
			if (IsTokenValid() == false)
			{
				GenerateToken();
			}
			return _token.accessToken;
		}

		/// <summary>
		/// Athugar hvort að JWT sé gilt eða útrunnið.
		/// </summary>
		/// <returns>true ef það er í lagi, annars false</returns>
		private bool IsTokenValid()
		{
			if (String.IsNullOrEmpty(_token.accessToken) == false && _token.expiresIn > 0)
			{
				var ageInSeconds = Math.Abs((DateTime.Now - _token.tokenCreated).TotalSeconds);
				if (ageInSeconds > _token.expiresIn)
				{
					Logger.Log($"Remaining JWT age = {ageInSeconds} seconds");
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Býr til JWT token.
		/// </summary>
		/// <returns>true ef allt gengur</returns>
		private bool GenerateToken()
		{
			string client_id = _hostDetails.clientId;
			string client_secret = _hostDetails.clientSecret;
			string resource = _hostDetails.resource;

			var client = new RestClient(_hostDetails.tokenAddress);
			var request = new RestRequest(Method.POST);
			request.AddHeader("cache-control", "no-cache");
			request.AddHeader("content-type", "application/x-www-form-urlencoded");
			request.AddParameter("application/x-www-form-urlencoded", $"grant_type=client_credentials&resource={resource}&client_id={client_id}&client_secret={client_secret}", ParameterType.RequestBody);
			IRestResponse response = client.Execute(request);

			try
			{
				dynamic resp = JObject.Parse(response.Content);
				if (resp != null)
				{
					_token.accessToken = resp.access_token;
					_token.tokenType = resp.token_type;
					_token.expiresIn = resp.expires_in;
					_token.tokenCreated = DateTime.Now;
					return true; // Success
				}
			}
			catch (Exception ex)
			{
				Logger.Log(ex.Message, LogType.Error);
				throw;
			}

			throw new Exception("No token available !");
		}
	}
}
