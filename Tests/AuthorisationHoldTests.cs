using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RB.AuthorisationHold.ClientSample.Entities;
using RB.AuthorisationHold.ClientSample.Entities.Enums;
using RB.AuthorisationHold.ClientSample.Utils;
using RB.AuthorisationHold.BLL.Entities;
using RB.AuthorisationHold.BLL.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace RB.AuthorisationHold.ClientSample.Tests
{
	public class AuthorisationHoldTests
	{
		private readonly ITestOutputHelper _output;
		private readonly AuthorisationHold _authorisationHold;
		private readonly string _baseFilePath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\Data\\";

		public AuthorisationHoldTests(ITestOutputHelper output)
		{
			_output = output;
			_authorisationHold = new AuthorisationHold(HostType.Test);
		}

		[Fact]
		[Trait("Category", "I")]
		public void TestPingSuccess()
		{
			// Arrange
			string expected = "OK";
			string actual;

			// Act
			actual = _authorisationHold.Ping();
			_output.WriteLine(actual);

			// Assert
			Assert.Contains(expected, actual);
		}

		[Theory]
		[Trait("Category", "I")]
		[InlineData("12345678901234A")]
		public void TestGetReservationsByExternalIdEmpty(string externalId)
		{
			// Arrange
			string expected = "[]";
			string actual;

			// Act
			actual = _authorisationHold.GetReservationsByExternalId(externalId).Result;
			_output.WriteLine(actual);

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[Trait("Category", "I")]
		[InlineData("015126000020")]
		public void TestGetReservationsByAgreementIdSuccess(string withdrawalAgreementId)
		{
			// Arrange
			string actual;

			// Act
			actual = _authorisationHold.GetReservationsByAgreementId(withdrawalAgreementId).Result;
			dynamic resp = JObject.Parse(actual);
			_output.WriteLine($"rowCount = {resp.rowCount}");


			// Assert
			Assert.True(resp.rowCount > 0);
		}


		[Theory]
		[Trait("Category", "I")]
		[InlineData("015126000020", "2104258379")]
		[InlineData("015126000011", "0209806999")]
		public void TestCreateReservationSuccess(string withdrawalAgreementId, string cardHolderId)
		{
			// Arrange
			BaseReservationResponse goodResponse = new();

			// Act
			object obj = CreateReservationWrapper(withdrawalAgreementId, cardHolderId);
			if (obj is BaseReservationResponse temp)
			{
				goodResponse = temp;
			}

			// Assert
			Assert.Equal("00", goodResponse.ResponseCode);
			Assert.Equal(16, goodResponse.OperationReference.Length);
		}

		[Theory]
		[Trait("Category", "I")]
		[InlineData("012326012345", "2104258379")]
		public void TestCreateReservationNotFound(string withdrawalAgreementId, string cardHolderId)
		{
			// Arrange
			BadRequestProblemDetails badResponse = new();

			// Act
			object obj = CreateReservationWrapper(withdrawalAgreementId, cardHolderId);
			if (obj is BadRequestProblemDetails temp)
			{
				badResponse = temp;
			}

			// Assert
			Assert.Equal("52", badResponse.ErrorCode);
			Assert.Equal("Business logic error", badResponse.Title);
			Assert.Equal(409, badResponse.Status);
			Assert.Contains("This account does not exist", badResponse.Detail);
		}

		[Theory(Skip = "Á eftir að gera")]
		[Trait("Category", "I")]
		[InlineData("012326012345", "2104258379", "123456789012345")]
		public void TestTodoSkip(string withdrawalAgreementId, string cardHolderId, string externalId)
		{
			Logger.Log(_authorisationHold.CreateOrReplaceReservation(withdrawalAgreementId, cardHolderId, "123.0", externalId).Result);
			Logger.Log(_authorisationHold.ConfirmReservation(withdrawalAgreementId, cardHolderId, "123.0", externalId).Result);
			Logger.Log(_authorisationHold.DeleteReservationByExternalId(externalId).Result);
			Logger.Log(_authorisationHold.CreateCreditTranfer(withdrawalAgreementId, cardHolderId, "1.0", externalId).Result);
		}

		[Theory]
		[InlineData("OfflineSettlements", ".zip")]
		[InlineData("OfflineSettlements", ".json")]
		[Trait("Category", "I")]
		[Conditional("DEBUG")]
		public void TestPostSettlementsSuccess(string baseName, string extension)
		{
			// Arrange
			string inputFileName = String.Format("{0}{1}{2}", _baseFilePath, baseName, extension);

			// Act / Assert
			try
			{
				_authorisationHold.PostSettlements(inputFileName, "Fyrsti hluti");
			}
			catch (Exception)
			{
				throw;
			}
		}

		[Theory]
		[InlineData("Bull", ".bin")]
		[InlineData("BuildJsonFileNULL", ".json")]
		[Trait("Category", "I")]
		public async void TestPostSettlementsFailure(string baseName, string extension)
		{
			// Arrange
			string inputFileName = String.Format("{0}{1}{2}", _baseFilePath, baseName, extension);

			// Act and Assert #1
			try
			{
				_authorisationHold.PostSettlements(inputFileName);
				throw new Exception("Should not happen");
			}
			catch (Exception ex)
			{
				Assert.Contains("The remote server returned an error: (500) Internal Server Error.", ex.Message);
				_output.WriteLine(ex.Message);
			}
			
			// Act and Assert #2
			try
			{
				await _authorisationHold.PostSettlements_Basic(inputFileName);
				throw new Exception("Should not happen");
			}
			catch (Exception ex)
			{
				Assert.Contains("Response status code does not indicate success", ex.Message);
				_output.WriteLine(ex.Message);
			}

			// Act and Assert #3
			try
			{
				_authorisationHold.PostSettlements_RestSharp(inputFileName);
				throw new Exception("Should not happen");
			}
			catch (Exception ex)
			{
				Assert.True(ex.Message.Contains("Bad Request") || ex.Message.Contains("Internal Server Error"));
				_output.WriteLine(ex.Message);
			}
		}

		[Theory]
		[Trait("Category", "I")]
		[InlineData(true)]
		public void TestBuildJsonFile(bool allowNull)
		{
			// Arrange
			string baseName = "BuildJsonFile";
			string outputFileName = String.Format("{0}{1}{2}{3}", _baseFilePath, baseName, allowNull ? "NULL" : "", ".json");
			string inputFileName = outputFileName;

			var transferSettlementList = new List<TransferSettlement>();
			var transferOtherList = new List<TransferOther>();

			for (int i = 0; i < 5; i++)
			{
				TransferSettlement tc = new()
				{
					CardId = "0000000017",
					WithdrawalAgreementId = "015126000011",
					Amount = 111,
					CurrencyCode = "ISK",
					SourceAmount = 111,
					SourceCurrencyCode = "ISK",
					ExternalId = StringUtility.RandomString(15, RandomStringType.Numeric),
					ExpiryDate = DateTime.Today.AddDays(6),
					CardHolderId = "0209806999",
					CardNumberMasked = "426562******" + StringUtility.RandomString(4, RandomStringType.Numeric),
					WalletType = "103",
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

				transferSettlementList.Add(tc);
			}

			for (int i = 0; i < 2; i++)
			{
				TransferOther tc = new()
				{
					CardId = "0000000017",
					WithdrawalAgreementId = "015126000011",
					Amount = 111,
					CurrencyCode = "ISK",
					SourceAmount = 111,
					SourceCurrencyCode = "ISK",
					ExternalId = StringUtility.RandomString(15, RandomStringType.Numeric),
					ExpiryDate = DateTime.Today.AddDays(6),
					CardHolderId = "0209806999",
					CardNumberMasked = "426562******" + StringUtility.RandomString(4, RandomStringType.Numeric),
					WalletType = "103",
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

				transferOtherList.Add(tc);
			}

			// Act
			var jsonSettings = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented
			};

			if (allowNull)
			{
				jsonSettings.NullValueHandling = NullValueHandling.Include;
			}
			else
			{
				jsonSettings.NullValueHandling = NullValueHandling.Ignore;
			}

			StringBuilder jsonStringBuilder = new();
			jsonStringBuilder.Append("{\n");                        // Start of JSON
			jsonStringBuilder.Append("\"TransferSettlements\": ");
			jsonStringBuilder.Append(JsonConvert.SerializeObject(transferSettlementList, jsonSettings));

			jsonStringBuilder.Append(",\n");
			jsonStringBuilder.Append("\"TransferOthers\": ");
			jsonStringBuilder.Append(JsonConvert.SerializeObject(transferOtherList, jsonSettings));
			jsonStringBuilder.Append("\n}");                        // End of JSON
			File.Delete(outputFileName);
			File.WriteAllText(outputFileName, StringUtility.PrettifyJson(jsonStringBuilder.ToString()), Encoding.UTF8);

			// Assert
			Assert.True(File.Exists(inputFileName));
			dynamic resp = JObject.Parse(File.ReadAllText(inputFileName));
			Assert.Equal(5, resp.TransferSettlements.Count);
			Assert.Equal(2, resp.TransferOthers.Count);
		}

		#region CreateReservationWrapper
		/// <summary>
		/// Hjúpar createReservation fallið.
		/// Ef allt gengur þá skilar það BaseReservationResponse hlut.
		/// Villur skila BadRequestProblemDetails hlut eða NULL í versta falli.
		/// </summary>
		/// <param name="tester">Handfang á rest köllin</param>
		/// <returns>BaseReservationResponse eða BadRequestProblemDetails eða NULL</returns>
		private object CreateReservationWrapper(string withdrawalAgreementId, string cardHolderId)
		{
			string reserveAmount = StringUtility.RandomAmount(upperBound: 1000, lowerBound: 100);
			string externalId = StringUtility.RandomString(15, RandomStringType.Numeric);

			try
			{
				string result = _authorisationHold.CreateReservation(withdrawalAgreementId, cardHolderId, reserveAmount, externalId).Result;

				Logger.Log(result, LogType.Info);

				dynamic peek = JObject.Parse(result);
				if (peek.responseCode != null)
				{
					// SUCCESS
					// TODO: bæta við success svarkóðum
					var goodResponse = JsonConvert.DeserializeObject<BaseReservationResponse>(result);
					if (goodResponse.ResponseCode != null)
					{
						return goodResponse;
					}
				}
				else
				{
					// FAILURE
					var badResponse = JsonConvert.DeserializeObject<BadRequestProblemDetails>(result);

					if (badResponse.Title != null)
					{
						StringBuilder sbError = new("Decoded\n-------\n");
						sbError.AppendLine($"Type = \"{badResponse.Type}\"");
						sbError.AppendLine($"ErrorCode = \"{badResponse.ErrorCode}\"");
						sbError.AppendLine($"Title = \"{badResponse.Title}\"");
						sbError.AppendFormat("Status = \"{0}\"\n", badResponse?.Status);
						sbError.AppendLine($"Detail = \"{badResponse.Detail}\"");
						sbError.AppendLine($"Instance = \"{badResponse.Instance}\"");
						
						if (badResponse.Errors != null)
						{
							int i = 1;
							foreach (var error in badResponse.Errors)
							{
								sbError.AppendFormat("Error {0}: {1} - ", i, error.Key);
								foreach (var message in error.Value)
								{
									sbError.AppendFormat("{0} ", message);
								}
								sbError.Append('\n');
								i++;
							}
						}

						Logger.Log(sbError.ToString(), LogType.Debug);
					}

					return badResponse;
				}
			}
			catch (Exception ex)
			{
				Logger.Log(ex.Message, LogType.Error);
			}

			return null;
		}
		#endregion
	}
}