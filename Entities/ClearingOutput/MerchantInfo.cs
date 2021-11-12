using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace RB.AuthorisationHold.BLL.Entities
{
    /// <summary>
    /// Merchant Acceptor Info
    /// </summary>
    public class MerchantInfo
    {
        /// <summary>
        /// Merchant category code (is: þjónustukóði) to classify the type of service provided.
        /// </summary>
        [Required]
        [RegularExpression("^[0-9]{4}?$", ErrorMessage = "{0} must be of the expression {1}")]
        [JsonProperty(Required = Required.DisallowNull)]
        public string MCC { get; set; }

        /// <summary>
        /// Agreement number for Merchant or AcceptorId.
        /// Samnings númer hjá VISA/EURO. Fyrirtæki hafa sitt hvort samningsnúmerið hjá Valitor (VISA) og Borgun (EURO).
        /// </summary>
        [RegularExpression("^[0-9]{1,15}$", ErrorMessage = "{0} must be of the expression {1}")]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Id { get; set; }

        /// <summary>
        /// Merchant's acceptor name.
        /// </summary>
        [StringLength(22, ErrorMessage = "{0} must be up to {1} letters")]
        [JsonProperty(Required = Required.DisallowNull)]
        public string Name { get; set; }

        /// <summary>
        /// Merchant's acceptor city.
        /// </summary>
        [StringLength(13, ErrorMessage = "{0} must be up to {1} letters")]
        [JsonProperty(Required = Required.DisallowNull)]
        public string City { get; set; }

        /// <summary>
        /// Merchant's country code.
        /// https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2
        /// </summary>
        [StringLength(2, ErrorMessage = "{0} must be {1} letters", MinimumLength = 2)]
        [RegularExpression("^[a-zA-Z]{2}$")]
        [JsonProperty(Required = Required.DisallowNull)]
        public string CountryCode { get; set; }

    }
}