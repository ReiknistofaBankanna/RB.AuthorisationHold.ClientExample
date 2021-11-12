using Newtonsoft.Json;
using RB.AuthorisationHold.ClientSample.Core;
using RB.AuthorisationHold.ClientSample.Entities.Enums;
using RB.AuthorisationHold.ClientSample.Interfaces;
using RB.AuthorisationHold.ClientSample.Utils;
using RB.AuthorisationHold.BLL.Entities;
using RB.AuthorisationHold.BLL.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RB.AuthorisationHold.ClientSample
{
	public class AuthorisationHold : IReservations, ISettlements, ICreditTransfers, IPing
	{
		private readonly AccessControl _access = new();

		public AuthorisationHold(HostType type)
		{
			_access = new AccessControl(type);
		}

		#region Wrapper function for create/alter/confirm
		/// <summary>
		/// Hjúpur fyrir 4 föll: CreateReservation, ConfirmReservation, CreateOrReplaceReservation og CreateCreditTranfer.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="withdrawalAgreementId"></param>
		/// <param name="cardHolderId"></param>
		/// <param name="reserveAmount"></param>
		/// <param name="externalId"></param>
		/// <returns></returns>
		private async Task<string> PerformReservation(ReservationType type, string withdrawalAgreementId, string cardHolderId, string reserveAmount, string externalId)
		{
			string result = "";
			string sUrl = _access.GetHostDetails().baseAddress + "v1/Reservations/";
			HttpMethod method = HttpMethod.Post;

			if (type == ReservationType.ReserveOrReplace)
			{
				sUrl += externalId;
				method = HttpMethod.Put;
			}
			else if (type == ReservationType.Confirm)
			{
				sUrl += externalId + "/confirm";
			}
			else if (type == ReservationType.Deposit)
			{
				sUrl = _access.GetHostDetails().baseAddress + "v1/credittransfers/";
			}

			Logger.Log("POST: " + sUrl);
			Dictionary<string, string> headers = new() {
					{ "Content-Type", "application/json" },
					{ "Authorization", "Bearer " + _access.GetAccessToken() },
					{ "ExternalUserName", "ABB" },
					{ "ExternalApplication", "RDK" },
					{ "ExternalReference", DateTime.Now.Ticks.ToString()}
				};

			try
			{
				bool rawTemplate = false;
				string payload = "";

				if (rawTemplate)
				{
					string template = @"{ ""CardId"": ""{cardId}"",
										  ""WithdrawalAgreementId"": ""{withdrawalAgreementId}"",
										  ""Amount"": ""{amount}"",
										  ""CurrencyCode"": ""ISK"",
										  ""ExternalId"": ""{externalId}"",
										  ""SourceAmount"": ""{amount}"",
										  ""SourceCurrencyCode"": ""ISK"",
										  ""AuthorisationTransactionId"": ""{authorisationTransactionId}"",
										  ""RequestType"": ""00"",
										  ""CardHolderId"": ""{cardHolderId}"",
										  ""CardNumberMasked"": ""{cardNumberMasked}"",
										  ""IsPartialAllowed"": false,
										  ""MerchantInfo"": {
											""MCC"": ""6011"",
											""Id"": ""0501"",
											""Name"": ""Tonastodin"",
											""City"": ""Reykjvik"",
											""CountryCode"": ""IS""
										  }
										}";

					var parameters = new Dictionary<string, string>
					{
						{ "{cardId}", "00000" + StringUtility.RandomString(5, RandomStringType.Numeric) },
						{ "{withdrawalAgreementId}", withdrawalAgreementId },
						{ "{amount}", reserveAmount },
						{ "{externalId}", externalId },
						{ "{authorisationTransactionId}", StringUtility.RandomString(20, RandomStringType.Numeric) },
						{ "{cardHolderId}", cardHolderId },
						{ "{cardNumberMasked}", "426562******" + StringUtility.RandomString(4, RandomStringType.Numeric) }
					};

					payload = StringUtility.ReplaceTokens(parameters, template);
					payload = StringUtility.PrettifyJson(payload);
					Logger.Log("PerformReservation\n-----------------\n" + payload);
				}
				else
				{
					_ = decimal.TryParse(reserveAmount, out decimal dReserveAmount);
					PaymentReservation pr = new()
					{
						CardId = "00000" + StringUtility.RandomString(5, RandomStringType.Numeric),
						WithdrawalAgreementId = withdrawalAgreementId,
						Amount = dReserveAmount,
						CurrencyCode = "ISK",
						SourceAmount = dReserveAmount,
						SourceCurrencyCode = "ISK",
						ExternalId = externalId,
						CardHolderId = cardHolderId,
						CardNumberMasked = "426562******" + StringUtility.RandomString(4, RandomStringType.Numeric),
						AuthorisationTransactionId = StringUtility.RandomString(20, RandomStringType.Numeric),
						IsPartialAllowed = false,
						RequestType = "00",
						MerchantInfo = new MerchantInfo
						{
							MCC = "6011",
							Id = "0001234",
							Name = "Kvitt",
							City = "Reykjavík",
							CountryCode = "IS"
						}
					};

					payload = JsonConvert.SerializeObject(pr, new JsonSerializerSettings
					{
						Formatting = Formatting.Indented,
						NullValueHandling = NullValueHandling.Ignore
					});
				}

				result = await HttpUtility.SendRequestAsync(method, sUrl, payload, headers, "application/json");
				result = StringUtility.PrettifyJson(result);
			}
			catch (Exception ex)
			{
				Logger.Log(ex.Message);
			}

			return await Task.FromResult(result);
		}
		#endregion


		/// <summary>
		/// GET /v1/reservations, get all unconfirmetd reservations by agreement id.
		/// </summary>
		/// <param name="agreementId"></param>
		/// <returns></returns>
		public async Task<string> GetReservationsByAgreementId(string agreementId)
		{
			string result = "";
			string sUrl = $"{_access.GetHostDetails().baseAddress}v1/Reservations?agreementId={agreementId}";
			Logger.Log("GET: " + sUrl);
			var client = new RestClient(sUrl);
			var request = new RestRequest(Method.GET);

			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Authorization", "Bearer " + _access.GetAccessToken());
			request.AddHeader("ExternalUserName", "ABB");
			request.AddHeader("ExternalApplication", "RDK");
			request.AddHeader("ExternalReference", DateTime.Now.Ticks.ToString());

			try
			{
				IRestResponse response = await client.ExecuteAsync(request);

				if (response.Content.Length > 0)
				{
					result = StringUtility.PrettifyJson(response.Content);
				}
				else
				{
					result = "No data returned";
				}
			}
			catch (Exception ex)
			{
				Logger.Log(ex.StackTrace);
			}

			return await Task.FromResult(result);
		}

		/// <summary>
		///  POST /v1/reservations, create reservation payment
		/// </summary>
		/// <param name="withdrawalAgreementId"></param>
		/// <param name="cardHolderId"></param>
		/// <param name="reserveAmount"></param>
		/// <param name="externalId"></param>
		/// <returns></returns>
		public Task<string> CreateReservation(string withdrawalAgreementId, string cardHolderId, string reserveAmount, string externalId)
		{
			return PerformReservation(ReservationType.Reserve, withdrawalAgreementId, cardHolderId, reserveAmount, externalId);
		}

		/// <summary>
		/// GET /v1/reservations/{id}, reservation history by externalId.
		/// </summary>
		/// <param name="externalId"></param>
		/// <returns></returns>
		public async Task<string> GetReservationsByExternalId(string externalId)
		{
			string result = "";
			string sUrl = $"{_access.GetHostDetails().baseAddress}v1/Reservations/{externalId}";
			Logger.Log("GET: " + sUrl);
			var client = new RestClient(sUrl);
			var request = new RestRequest(Method.GET);

			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Authorization", "Bearer " + _access.GetAccessToken());
			request.AddHeader("ExternalUserName", "ABB");
			request.AddHeader("ExternalApplication", "RDK");
			request.AddHeader("ExternalReference", DateTime.Now.Ticks.ToString());

			try
			{
				IRestResponse response;
				response = await client.ExecuteAsync(request);

				if (response.Content.Length > 0)
				{
					result = StringUtility.PrettifyJson(response.Content);
				}
				else
				{
					result = "No data returned";
				}
			}
			catch (Exception ex)
			{
				Logger.Log(ex.StackTrace);
			}

			return await Task.FromResult(result);
		}


		/// <summary>
		/// PUT /v1/reservations/{id}, create or replace reservation payment.
		/// </summary>
		/// <param name="withdrawalAgreementId"></param>
		/// <param name="cardHolderId"></param>
		/// <param name="reserveAmount"></param>
		/// <param name="externalId"></param>
		/// <returns></returns>
		public Task<string> CreateOrReplaceReservation(string withdrawalAgreementId, string cardHolderId, string reserveAmount, string externalId)
		{
			return PerformReservation(ReservationType.ReserveOrReplace, withdrawalAgreementId, cardHolderId, reserveAmount, externalId);
		}

		/// <summary>
		/// DELETE /v1/Reservations/{id}, cancel a previously reserved payment.
		/// </summary>
		/// <param name="externalId"></param>
		/// <returns></returns>
		public async Task<string> DeleteReservationByExternalId(string externalId)
		{
			string result = "";
			string sUrl = $"{_access.GetHostDetails().baseAddress}v1/Reservations/{externalId}";
			Logger.Log("DELETE: " + sUrl);
			var client = new RestClient(sUrl);
			var request = new RestRequest(Method.DELETE);

			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Authorization", "Bearer " + _access.GetAccessToken());
			request.AddHeader("ExternalUserName", "ABB");
			request.AddHeader("ExternalApplication", "RDK");
			request.AddHeader("ExternalReference", DateTime.Now.Ticks.ToString());

			try
			{
				IRestResponse response;
				response = await client.ExecuteAsync(request);

				if (response.Content.Length > 0)
				{
					result = StringUtility.PrettifyJson(response.Content);
				}
				else
				{
					result = "No data returned";
				}
			}
			catch (Exception ex)
			{
				Logger.Log(ex.StackTrace);
			}

			return await Task.FromResult(result);
		}

		/// <summary>
		/// POST /v1/reservation/{id}/confirm, confirm reserved payment
		/// </summary>
		/// <param name="withdrawalAgreementId"></param>
		/// <param name="cardHolderId"></param>
		/// <param name="reserveAmount"></param>
		/// <param name="externalId"></param>
		/// <returns></returns>
		public Task<string> ConfirmReservation(string withdrawalAgreementId, string cardHolderId, string reserveAmount, string externalId)
		{
			return PerformReservation(ReservationType.Confirm, withdrawalAgreementId, cardHolderId, reserveAmount, externalId);
		}

		/// <summary>
		/// POST /v1/credittransfers, creates deposit on the client account.
		/// </summary>
		/// <param name="withdrawalAgreementId"></param>
		/// <param name="cardHolderId"></param>
		/// <param name="reserveAmount"></param>
		/// <param name="externalId"></param>
		/// <returns></returns>
		public Task<string> CreateCreditTranfer(string withdrawalAgreementId, string cardHolderId, string reserveAmount, string externalId)
		{
			return PerformReservation(ReservationType.Deposit, withdrawalAgreementId, cardHolderId, reserveAmount, externalId);
		}

		#region Alternative methods to upload file
		public async Task PostSettlements_Basic(string filePath)
		{
			var client = new HttpClient
			{
				BaseAddress = new(_access.GetHostDetails().baseAddress + "v1/settlements")
			};

			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"File [{filePath}] not found.");
			}
			using var form = new MultipartFormDataContent();
			using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
			fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
			form.Add(fileContent, "file", Path.GetFileName(filePath));

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _access.GetAccessToken());

			try
			{
				var response = await client.PostAsync("", form);
				response.EnsureSuccessStatusCode();
				Logger.Log($"Successful upload of file. Http status code is {response.StatusCode}.");
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void PostSettlements_RestSharp(string filePath)
		{
			var client = new RestClient(_access.GetHostDetails().baseAddress + "v1/settlements")
			{
				Timeout = -1
			};
			var request = new RestRequest(Method.POST);
			request.AddHeader("Content-Type", "multipart/form-data");
			request.AddHeader("Authorization", $"Bearer {_access.GetAccessToken()}");
			request.AlwaysMultipartFormData = true;
			request.AddFile(filePath, filePath);

			IRestResponse restResponse = client.Execute(request);
			if (restResponse.IsSuccessful == false)
			{
				throw new HttpRequestException($"{restResponse.StatusDescription} {restResponse.Content}");
			}
			Logger.Log(restResponse.Content);
		}
		#endregion

		/// <summary>
		/// POST /v1/settlements, upload settlements file.
		/// Throws 500 error if invalid input.
		/// </summary>
		/// <param name="fileName">Fully qualified path to the file to upload</param>
		/// <param name="comment"></param>
		public void PostSettlements(string fileName, string comment = null)
		{
			string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
			byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

			string sUrl = _access.GetHostDetails().baseAddress + "v1/settlements";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sUrl);
			request.Method = "POST";
			request.KeepAlive = false;
			request.Headers.Add("Authorization", "Bearer " + _access.GetAccessToken());
			request.Accept = "application/json, application/zip, application/xml";
			request.ContentType = "multipart/form-data; boundary=" + boundary;
			request.ProtocolVersion = new Version(1, 1);

			Stream rs = request.GetRequestStream();

			rs.Write(boundarybytes, 0, boundarybytes.Length);

			if (String.IsNullOrEmpty(comment) == false)
			{
				string header = $"Content-Disposition: form-data; name=\"{comment}\"; filename=\"{new FileInfo(fileName).Name}\"\r\nContent-Type: application/{Path.GetExtension(fileName).ToLower().Replace(".", "")}\r\n\r\n";
				byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
				rs.Write(headerbytes, 0, headerbytes.Length);
			}

			FileStream fileStream = new(fileName, FileMode.Open, FileAccess.Read);
			byte[] buffer = new byte[4096];
			int bytesRead;
			while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
			{
				rs.Write(buffer, 0, bytesRead);
			}
			fileStream.Close();

			byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
			rs.Write(trailer, 0, trailer.Length);
			rs.Close();

			using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			using Stream stream = response.GetResponseStream();
			using StreamReader reader = new(stream);

			string html = reader.ReadToEnd();
			for (int i = 1; i < response.Headers.Count; ++i)
			{
				Logger.Log(response.Headers[i]);

				String resp = response.Headers[i].ToString();
				resp = response.Headers[i-1].ToString() + resp;
				System.Diagnostics.Debug.WriteLine(response.ProtocolVersion);
				Logger.Log(response.ProtocolVersion.ToString());
			}
		}

		/// <summary>
		/// GET /ping, to check if the api is up and running.
		/// </summary>
		/// <returns>Returns "OK" if successful</returns>
		public string Ping()
		{
			var client = new RestClient(_access.GetHostDetails().baseAddress + "ping");
			var request = new RestRequest(Method.GET);
			string result = "";

			try
			{
				IRestResponse response = client.Execute(request);
				if (response.IsSuccessful)
				{
					result = $"{_access.GetHostDetails().baseAddress}{MethodBase.GetCurrentMethod().Name}: {response.Content}";
				}
				else
				{
					result = response.ErrorMessage;
				}
			}
			catch (Exception ex)
			{
				Logger.Log(ex.StackTrace);
			}

			return result;
		}
	}
}