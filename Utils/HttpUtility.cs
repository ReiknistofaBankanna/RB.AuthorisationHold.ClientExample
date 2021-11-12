using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RB.AuthorisationHold.ClientSample.Utils
{
	public class HttpUtility
	{
		/// <summary>
		/// Posts payload to specific url address.
		/// </summary>
		/// <param name="requestUri">A string that represents the request Uri</param>
		/// <param name="payload">Payload</param>
		/// <param name="headers">List of headers with values</param>
		/// <param name="mediaType">The media type to use for the content.</param>
		/// <returns></returns>
		internal static async Task<string> SendRequestAsync(HttpMethod method, string requestUri, string payload, Dictionary<string, string> headers = null, string mediaType = "text/xml")
		{
			HttpClient httpClient = new();
			using HttpContent content = new StringContent(payload, Encoding.UTF8, mediaType);
			using HttpRequestMessage request = new(method, requestUri);
			if (headers != null)
			{
				foreach (var header in headers)
				{
					if (header.Key != "Content-Type")
					{
						request.Headers.Add(header.Key, header.Value);
					}
				}
			}

			request.Content = content;
			using HttpResponseMessage response = httpClient.Send(request, HttpCompletionOption.ResponseHeadersRead);
			return await response.Content.ReadAsStringAsync();
		}
	}
}
