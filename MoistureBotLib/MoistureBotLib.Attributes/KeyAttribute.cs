using System;

namespace MoistureBot
{
	[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
	public class KeyAttribute : StringAttribute
	{
		public KeyAttribute (string name) : base (name) {}
	}

}

