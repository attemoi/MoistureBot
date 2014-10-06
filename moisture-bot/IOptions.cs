using System;
using Mono.Options;

namespace MoistureBot
{
	public interface IOptionParser
	{
		void Parse(string[] args);
		void WriteHelp();
	}

}

