using System;
using Mono.Addins;

namespace MoistureBot.ExtensionPoints
{

	/// <summary>
	/// This is an extension point for a single instance addin intended for internal use. 
	/// This should not be extended by external addins.
	/// </summary>
	[TypeExtensionPoint]
	public interface ILogger
	{
		void Debug(string message);

		void Info(string message);

		void Warn(string message);

		void Error(string message, Exception e);

		void Fatal(string message);
	}
}

