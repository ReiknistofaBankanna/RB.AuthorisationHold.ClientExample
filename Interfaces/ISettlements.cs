using System.Threading.Tasks;

namespace RB.AuthorisationHold.ClientSample.Interfaces
{
	interface ISettlements
	{
		void PostSettlements(string fileName, string comment = null);
		void PostSettlements_RestSharp(string filePath);	// Alternative method without comment
		Task PostSettlements_Basic(string filePath);		// Alternative method without comment
	}
}
