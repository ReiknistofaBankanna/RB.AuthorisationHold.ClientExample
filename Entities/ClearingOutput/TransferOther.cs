using Newtonsoft.Json;

namespace RB.AuthorisationHold.BLL.Entities
{
	public class TransferOther : TransferSettlement
	{
		/// <summary>
		/// A free test description of the transaction
		/// </summary>  XXXX
		[JsonProperty(Order = 201)]
		public string Description { get; set; }

		/// <summary>   XXXX
		/// A label used by the service provider to categorize the transactions
		/// </summary>
		[JsonProperty(Order = 202)]
		public string Label { get; set; }

		/// <summary>
		/// A free reference that can be used by the service provider
		/// </summary>  XXX
		[JsonProperty(Order = 203)]
		public string ReferenceId { get; set; }

		/// <summary>
		/// If the balance adjustment is derived from another transaction, this id will refer to that transaction
		/// </summary>  XXXXX
		[JsonProperty(Order = 204)]
		public long SourceId { get; set; }
	}
}