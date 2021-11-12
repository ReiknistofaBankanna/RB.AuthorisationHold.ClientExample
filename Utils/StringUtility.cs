using RB.AuthorisationHold.ClientSample.Entities.Enums;
using RB.AuthorisationHold.ClientSample.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace RB.AuthorisationHold.BLL.Utils
{
	public class StringUtility
	{
		/// <summary>
		/// Býr til random streng.
		/// </summary>
		/// <param name="length"></param>
		/// <param name="bNumeric"></param>
		/// <returns></returns>
		public static string RandomString(int length, RandomStringType type)
		{
			const string space = "   ";
			string AB = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ" + space;
			Random rnd = new();
			StringBuilder sb = new(length);
			for (int i = 0; i < length; i++)
			{
				if (type == RandomStringType.Numeric)
				{
					// x er í 0-9
					sb.Append(rnd.Next(10));
				}
				else if (type == RandomStringType.Alphabetic)
				{
					// x er í A-Z
					sb.Append(AB[10 + rnd.Next(AB.Length - 10 - space.Length)]);
				}
				else if (type == RandomStringType.SpacedAlphabetic)
				{
					// x er stafur til að byrja með, annars A-Z eða bil
					if (i > 0)
					{
						sb.Append(AB[10 + rnd.Next(AB.Length - 10)]);
					}
					else
					{
						sb.Append(AB[10 + rnd.Next(AB.Length - 10 - space.Length)]);
					}
				}
				else if (type == RandomStringType.AlphaNum)
				{
					// x er í 0-9 eða A-Z
					sb.Append(AB[rnd.Next(AB.Length - space.Length)]);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="upperBound">Efri mörk tölu sem á að búa til.</param>
		/// <param name="lowerBound">Neðri mörk tölu sem á að búa til.</param>
		/// <param name="bFraction">true ef sýna á 2 aukastafi.</param>
		/// <returns></returns>
		public static string RandomAmount(int upperBound, int lowerBound = 0, bool bFraction = false)
		{
			Random rnd = new();

			int value = rnd.Next(lowerBound, upperBound);
			int fraction = rnd.Next(1, 99);
			bool bUseDot = true;

			string result;
			if (bFraction)
			{
				result = string.Format("{0:F2}", (decimal)value + (fraction / 100m));
				if (bUseDot && result.Contains(","))
				{
					result = result.Replace(',', '.');
				}
			}
			else
			{
				result = value.ToString();
			}

			if (Decimal.TryParse(result, out decimal temp))
			{
				temp.ToString(new CultureInfo("en-US"));
			}

			return value.ToString(); // Just in case ...
		}

		/// The replacement token dictionary. Note that token names are case-sensitive.
		/// </param>
		/// <param name="content">
		/// The content that will be searched and replaced with an appropriate matching token.
		/// </param>
		/// <returns>A string with matched token keys replaced.</returns>
		/// <example>
		/// var replacementTokens = new Dictionary&lt;string, string&gt;
		/// {
		///     { &quot;{Name}&quot;,  &quot;Jon&quot; },
		///     { &quot;{Fax}&quot;,   &quot;(999) 999-9999&quot;       },
		///     { &quot;{Phone}&quot;, &quot;(888) 888-8888&quot;       }
		/// };
		/// </example>
		public static string ReplaceTokens(Dictionary<string, string> tokens, string content)
		{
			foreach (var token in tokens.Keys)
			{
				content = content.Replace(token, tokens[token]);
			}

			return content;
		}

		/// <summary>
		/// Makes json string printable.
		/// </summary>
		/// <param name="json">input string to beautify</param>
		/// <returns></returns>
		public static string PrettifyJson(string json)
		{
			string result = json;

			try
			{
				if (String.IsNullOrEmpty(json) == false)
				{
					var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
					result = JsonSerializer.Serialize(jsonElement, new() { WriteIndented = true });
				}
			}
			catch (Exception ex)
			{
				Logger.Log(ex.Message);
			}

			return result;

		}
	}
}
