using System;
using Mono.Options;

namespace moisturebot
{
	public interface IOptions
	{
		void Parse(string[] args);
		void WriteHelp();
	}

}

