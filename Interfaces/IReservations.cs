using System.Threading.Tasks;

namespace RB.AuthorisationHold.ClientSample.Interfaces
{
	interface IReservations
	{
		Task<string> GetReservationsByAgreementId(string agreementId);
		Task<string> CreateReservation(string withdrawalAgreementId, string cardHolderId, string reserveAmount, string externalId);
		Task<string> GetReservationsByExternalId(string externalId);
		Task<string> CreateOrReplaceReservation(string withdrawalAgreementId, string cardHolderId, string reserveAmount, string externalId);
		Task<string> DeleteReservationByExternalId(string externalId);
		Task<string> ConfirmReservation(string withdrawalAgreementId, string cardHolderId, string reserveAmount, string externalId);
	}
}
