﻿using System;

namespace MoistureBot.Config
{
	[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
	public class SectionAttribute : StringAttribute
	{
		public SectionAttribute (string name) : base (name) {}
	}
		
}

