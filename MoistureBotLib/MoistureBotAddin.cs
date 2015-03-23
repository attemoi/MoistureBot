using System;
using MoistureBot.ExtensionPoints;

namespace MoistureBot
{
	public abstract class MoistureBotAddin
	{
		public ILogger Logger;
		public IConfig Config;
		public IMoistureBot Bot;

		public void initAddin() {}
	}
}

