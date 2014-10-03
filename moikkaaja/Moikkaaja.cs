using System;
using Mono.Addins;
using moisturebot.lib;

[assembly:Addin]
[assembly:AddinDependency ("Moisture-bot", "1.0")]

namespace moisturebot
{

	[Extension]
	public class Moikkaaja: IChatCommand
	{
		public void MessageReceived (string content)
		{
			Console.WriteLine ("MOIMOIMOIMOI!");
		}
	}
}