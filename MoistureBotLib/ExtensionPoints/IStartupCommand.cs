using System;
using Mono.Addins;

namespace MoistureBot.ExtensionPoints
{
	/// <summary>
	/// Extension point for initializing addins during on program startup.
	/// </summary>
	[TypeExtensionPoint]
	public interface IStartupCommand
	{
		/// <summary>
		/// Called when the program has started
		/// </summary>
		void ProgramStarted();
	}
		
}
