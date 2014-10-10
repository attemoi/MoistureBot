using System;

namespace MoistureBot.Config
{
	[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
	public class DefaultValueAttribute : StringAttribute
	{
		public DefaultValueAttribute (string name) : base (name) {}
	}
}

