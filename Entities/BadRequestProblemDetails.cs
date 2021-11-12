using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RB.AuthorisationHold.ClientSample.Entities
{
	/// <summary>
	/// Validation errors.
	/// </summary>
	public class BadRequestProblemDetails
	{
		/// <summary>
		/// List of validation errors
		/// </summary>    
		[JsonPropertyName("errors")]
		public IDictionary<string, string[]> Errors { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type", Required = Required.DisallowNull)]
		[Required]
		public string Type { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "errorcode", Required = Required.DisallowNull)]
		public string ErrorCode { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "title", Required = Required.DisallowNull)]
		public string Title { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "status", Required = Required.DisallowNull)]
		public int? Status { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "detail", Required = Required.DisallowNull)]
		public string Detail { get; set; }
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "instance", Required = Required.DisallowNull)]
		public string Instance { get; set; }
	}
}
