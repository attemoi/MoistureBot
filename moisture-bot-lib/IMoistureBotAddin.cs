using System;
using Mono.Addins;

namespace moisturebot
{
	public abstract class IMoistureBotAddin
	{		
		public IMoistureBot Bot { get; internal set; }

		public void Initialize ( IMoistureBot bot )
		{
			this.Bot = bot;
		}

	}

}

