using System;

namespace MoistureBot
{
	public static class ConsoleMessage
	{

		public static string ExtraParametersNotAllowed( string commandName ) { 
			return "This command doesn't allow extra parameters. See '" + commandName + "' for more information";
		}

		public static string InvalidNumberOfParameters( string commandName ) { 
			return "Invalid number of parameters. Type '" + commandName + "'  for more information.";
		}
			
		public static string InvalidParameters( string commandName ) {
			return "Invalid parameters. Type '" + commandName + "' for more information.";
		}

		public static string SEE_STATUS = "Type 'status' to see bot status.";

	}
}			

