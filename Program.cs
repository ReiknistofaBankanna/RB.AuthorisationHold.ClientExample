using RB.AuthorisationHold.ClientSample.Core;
using RB.AuthorisationHold.ClientSample.Entities.Enums;
using RB.AuthorisationHold.ClientSample.Utils;
using RB.AuthorisationHold.BLL.Utils;
using System;

namespace RB.AuthorisationHold.ClientSample
{
	class Program
	{
		static void Main()
		{
			DisplayTokenLoop(HostType.Test);
		}

		/// <summary>
		/// Sýnir JWT token og random amount og externalId.
		/// </summary>
		public static void DisplayTokenLoop(HostType type)
		{
			AccessControl tester = new(type);

			ConsoleKeyInfo input;
			do
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("Press any key to regenerate, ESC to quit\n");
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine($"externalId = {StringUtility.RandomString(15, RandomStringType.Numeric)}\n");
				Console.WriteLine($"amount = {StringUtility.RandomAmount(upperBound: 1000, lowerBound: 100)}\n");

				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("JWT = \"" + tester.GetAccessToken() + "\"");

				input = Console.ReadKey();

				Console.Clear();
			} while (input.Key != ConsoleKey.Escape);
		}
	}
}