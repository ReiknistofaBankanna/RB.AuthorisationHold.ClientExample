using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace RB.AuthorisationHold.BLL.Entities
{
	/// <summary>
	/// TransferSettlement
	/// TransferAndReleaseReserve 
	///     Same as TransferSettlement, ReleaseReserve is always true and NewReserveAmount is always 0
	/// TransferAndReplaceReserve
	///     Same as TransferSettlement, ReleaseReserve is always true and NewReserveAmount > 0.  Previous reserve is relasead and new created.
	/// </summary>
	public class TransferSettlement : PaymentReservation
	{
		/// <summary>
		/// AuthorizationId
		/// </summary>
		[JsonProperty(Order = 105)]
		public string AuthorisationId { get; set; }

		/// <summary>
		/// Transaction text key.
		/// For TransferSettlement: SALE05, REFUND, ...
		/// For TransferOther: ATMFEE, INNBOU, ...
		/// </summary>
		[JsonProperty(Order = 106)]
		public string TransactionKey { get; set; }

		/// <summary>
		/// The date and time of the transaction. For settlement transactions (sale, refund) this is the merchant date and time. 
		/// Format yyyy-MM-ddTHH:mm:ss
		/// </summary>
		[JsonProperty(Order = 107)]
		public DateTime TransactionDate { get; set; }

		/// <summary>
		/// The date and time the transaction was processed by VALITOR.  
		/// Format yyyy-MM-ddTHH:mm:ss.
		/// </summary>
		[JsonProperty(Order = 108)]
		public DateTime RegistrationDate { get; set; }

		/// <summary>
		/// The date and time the transaction was processed by the card scheme. 
		/// For settlement transactions (sale, refund) this is the merchant date and time. 
		/// Format yyyy-MM-ddTHH:mm:ss
		/// </summary>
		[JsonProperty(Order = 109)]
		public DateTime ProcessingDate { get; set; }

		/// <summary>
		/// New reserve amount in card currency.
		/// </summary>
		[RegularExpression("^[0-9]{1,18}(.[0-9]{1,3})?$", ErrorMessage = "{0} must have follow the rule of {1} where fields after comma are optional.")]
		[JsonProperty(Order = 112)]
		public decimal NewReserveAmount { get; set; } = 0;

		/// <summary>
		/// Is this OCT settlement record
		/// </summary>
		[JsonProperty(Order = 112)]
		public bool IsOriginalCreditTransfer { get; set; }
	}
}
