using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace RB.AuthorisationHold.BLL.Entities
{
    /// <summary>
    /// Sjá nánari upplýsingar um stöðluð snið á https://en.wikipedia.org/wiki/ISO_8583
    /// </summary>
    public class PaymentReservation
    {
        /// <summary>
        /// Unique identifier for card, can be cardid, panid or rbsnumber.
        /// </summary>
        [StringLength(10, ErrorMessage = "{0} must be up to {1} letters", MinimumLength = 1)]
        [JsonProperty(Required = Required.DisallowNull, Order = 1)]
        public string CardId { get; set; }

        /// <summary>
        ///  Maskað kortanúmer 
        /// </summary>
        [StringLength(16, ErrorMessage = "{0} must be {1} digits", MinimumLength = 16)]
        [JsonProperty(Order = 2)]
        public string CardNumberMasked { get; set; }

        /// <summary>
        /// Bank account number.
        /// </summary>
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(12, ErrorMessage = "{0} must be {1} digits", MinimumLength = 12)]
        [JsonProperty(Required = Required.DisallowNull, Order = 3)]
        public string WithdrawalAgreementId { get; set; }

        /// <summary>
        ///  CardHolderId 
        /// </summary>
        [Required]
        [StringLength(10, ErrorMessage = "{0} must be {1} digits", MinimumLength = 10)]
        [JsonProperty(Required = Required.DisallowNull, Order = 4)]
        public string CardHolderId { get; set; }

        /// <summary>   
        /// Amount - uppgjörsupphæð
        /// </summary>
        [Required]
        [RegularExpression(@"^[0-9]{1,15}(.[0-9]{1,3})?$", ErrorMessage = "{0} must have follow the rule of {1} where fields after comma are optional.")]
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "{0} must be greater than 0.")]
        [JsonProperty(Required = Required.DisallowNull, Order = 5)]
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency Code of purchase https://en.wikipedia.org/wiki/ISO_4217
        /// Currency of the debit card account
        /// </summary>
        [Required]
        [RegularExpression("^[a-zA-Z]{3}$")]
        [JsonProperty(Required = Required.DisallowNull, Order = 6)]
        public string CurrencyCode { get; set; }

        /// <summary>
		/// Purchase amount in card currency (in card currency, ISK for current cards).
		/// LCYAmount in FinancialEvents.
        /// </summary>        
        [RegularExpression(@"^[0-9]{1,15}(.[0-9]{1,3})?$", ErrorMessage = "{0} must have follow the rule of {1} where fields after comma are optional.")]
        [JsonProperty(Order = 7)]
        public decimal SourceAmount { get; set; }

        /// <summary>
		/// Currency code of withdrawal in source currency, ISK for now. 
		/// https://en.wikipedia.org/wiki/ISO_4217
        /// </summary>
        [RegularExpression("^[a-zA-Z]{3}$")]
        [JsonProperty(Required = Required.DisallowNull, Order = 8)]
        public string SourceCurrencyCode { get; set; }

        /// <summary>
		/// Lifecycle Identification, unique identify for transaction, i.e. TransactionLifeCycleId
		/// Transaction Identifier, Visa generated identifier that is unique for each original transaction.
		/// It links original authorization requests to subsequent messages, such as reversals,
        /// </summary>
        [Required]
        [StringLength(30, ErrorMessage = "{0} must be up to {1} letters", MinimumLength = 3)]
        [JsonProperty(Required = Required.DisallowNull, Order = 9)]
        public string ExternalId { get; set; }


        /// <summary>
        /// RequestType
        /// </summary>
        [StringLength(2, ErrorMessage = "{0} must be up to {1} letters", MinimumLength = 2)]
        [JsonProperty(Order = 10)]
        public string RequestType { get; set; }

        /// <summary>
        /// IsPartialAllowed, True ef partial reserve allowed. = PartialAllowed
        /// </summary>
        [JsonProperty(Order = 11)]
        public bool IsPartialAllowed { get; set; }

        /// <summary>
        /// Payment reservation expiry date (Default 6 days) 
        /// </summary>
        [DataType(DataType.Date)]
        [JsonProperty(Order = 12)]
        public DateTime ExpiryDate { get; set; }


        /// <summary>
		/// AuthTransId, Unique id of the authorization request within Valitor (paired authorization).
        /// </summary>
        [StringLength(20, ErrorMessage = "{0} must be up to {1} letters", MinimumLength = 6)]
        [JsonProperty(Order = 13)]
        public string AuthorisationTransactionId { get; set; }

        /// <summary>
        ///Wallet type. Example: 103 = Apple Pay, 216 Google Pay, 217 Samsung Pay 
        [JsonProperty(Order = 14)]
        public string WalletType { get; set; }

        /// <summary>
        /// Merchant info innheldur 5 svæði:  =  MCC, AcceptorId, AcceptorName, AcceptorCity, AcceptorCountry
        /// Merchant MCC (is: þjónustukóði) to classify the type of service provided.        
        /// Merchant Id
        /// Merchant name
        /// Merchant City
        /// Merchant CountryCode 
        /// </summary>
        [JsonProperty(Order = 300)]
        public MerchantInfo MerchantInfo { get; set; }
    }
}