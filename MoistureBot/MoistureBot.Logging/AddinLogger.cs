using System;
using MoistureBot;

namespace MoistureBot.Logging
{
	public class AddinLogger : IAddinLogger
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger("AddinLogger");

		#region IAddinLogger implementation
		public void Debug (string message)
		{
			log.Debug (message);
		}
		public void Info (string message)
		{
			log.Info (message);
		}
		public void Warn (string message)
		{
			log.Warn (message);
		}
		public void Error (string message, Exception e)
		{
			log.Error(message, e);
		}
		public void Fatal (string message)
		{
			log.Fatal (message);
		}
		#endregion

	}
}

