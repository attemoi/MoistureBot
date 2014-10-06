using System;
using Mono.Addins;

namespace MoistureBot
{
	public interface IMoistureBotAddin
	{
		IAddinLogger Logger { get; set; }
		IMoistureBot Bot { get; set; }
	}
}

