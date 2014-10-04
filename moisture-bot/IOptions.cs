using System;
using Mono.Options;

namespace moisturebot
{
	public interface IOptionParser
	{
		void Parse(string[] args);
		void WriteHelp();
	}

}

