using System;
using Mono.Addins;

namespace MoistureBot
{
	public interface IMoistureBotAddin
	{		
		IMoistureBot Bot { get; set; }
	}

}

