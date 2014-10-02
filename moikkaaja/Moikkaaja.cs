using System;
using Mono.Addins;
using moisturebot.contracts;

[assembly:Addin]
[assembly:AddinDependency ("Moisture-bot", "1.0")]

[Extension]
public class Moikkaaja: IChatCommand
{
	public void MessageReceived (string content)
	{
		Console.WriteLine ("MOIMOIMOIMOI!");
	}
}