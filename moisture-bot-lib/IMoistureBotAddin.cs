using System;
using Mono.Addins;

namespace moisturebot
{
	public interface IMoistureBotAddin
	{		
		IMoistureBot Bot { get; set; }
	}

}

