using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Options;

namespace moisturebot
{
	public class ConsoleUtils
	{

		public static void WriteHelp (string description, string usage, OptionSet o) {
			Console.WriteLine ();
			Console.WriteLine ("Description:");
			Console.WriteLine ();
			Console.WriteLine ("  " + description);
			Console.WriteLine ();
			Console.WriteLine ("Usage:");
			Console.WriteLine ();
			Console.WriteLine ("  " + usage);
			Console.WriteLine ();
			Console.WriteLine ("Options:");
			Console.WriteLine ();
			o.WriteOptionDescriptions (Console.Out);
			Console.WriteLine ();
		}

		/// <summary>
		/// Like System.Console.ReadLine(), only with a mask.
		/// </summary>
		/// <param name="mask">a <c>char</c> representing your choice of console mask</param>
		/// <returns>the string the user typed in </returns>
		public static string ReadPassword(char mask)
		{
			const int ENTER = 13, BACKSP = 8, CTRLBACKSP = 127;
			int[] FILTERED = { 0, 27, 9, 10 /*, 32 space, if you care */ }; // const

			var pass = new Stack<char>();
			char chr = (char)0;

			while ((chr = System.Console.ReadKey(true).KeyChar) != ENTER)
			{
				if (chr == BACKSP)
				{
					if (pass.Count > 0)
					{
						System.Console.Write("\b \b");
						pass.Pop();
					}
				}
				else if (chr == CTRLBACKSP)
				{
					while (pass.Count > 0)
					{
						System.Console.Write("\b \b");
						pass.Pop();
					}
				}
				else if (FILTERED.Count(x => chr == x) > 0) { }
				else
				{
					pass.Push((char)chr);
					System.Console.Write(mask);
				}
			}

			System.Console.WriteLine();

			return new string(pass.Reverse().ToArray());
		}

		/// <summary>
		/// Like System.Console.ReadLine(), only with a mask.
		/// </summary>
		/// <returns>the string the user typed in </returns>
		public static string ReadPassword()
		{
			return ReadPassword('\0');
		}
	}
}

