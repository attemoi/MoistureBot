using System;

namespace MoistureBot.Config
{
	[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
	public class NameAttribute : StringAttribute
	{
		public NameAttribute (string name) : base (name) {}
	}
}

