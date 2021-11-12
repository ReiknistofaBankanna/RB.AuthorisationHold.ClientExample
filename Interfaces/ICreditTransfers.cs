using System.Threading.Tasks;

namespace RB.AuthorisationHold.ClientSample.Interfaces
{
	interface ICreditTransfers
	{
		Task<string> CreateCreditTranfer(string withdrawalAgreementId, string cardHolderId, string reserveAmount, string externalId);
	}
}
