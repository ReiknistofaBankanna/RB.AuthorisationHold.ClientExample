using Newtonsoft.Json;

namespace RB.AuthorisationHold.ClientSample.Entities
{
    public class BaseReservationResponse
    {
        /// <summary>
        /// Operation reference
        /// </summary>
        /// <example>B9K15AHPRA2JYS6S</example>
        [JsonProperty(Required = Required.DisallowNull)]
        public string OperationReference { get; set; }

        /// <summary>
        /// ExternalId 
        /// </summary>
        /// <example>107123414129873</example>
        [JsonProperty(Required = Required.DisallowNull)]
        public string ExternalId { get; set; }

        /// <summary>
        /// Available amount if request has set IsPartialAllowed=True
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        public decimal? AvailableAmount { get; set; }

        /// <summary>
        /// Response Code, reserved or rejected
        /// </summary>
        /// <example>00</example>
        [JsonProperty(Required = Required.DisallowNull)]
        public string ResponseCode { get; set; }

        // TODO: Check if this is really necessary...
        /// <summary>
        /// ResponseText from Sopra
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        public string ResponseText { get; internal set; }
    }
}