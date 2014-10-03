using System;
using Mono.Addins;

namespace moisturebot
{
	public interface IMoistureBotAddin
	{		
		void Initialize (IMoistureBot bot);
	}

}

