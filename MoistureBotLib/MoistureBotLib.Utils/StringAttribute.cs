using System;

namespace MoistureBot
{

	public class StringAttribute : System.Attribute
	{
		private string _value;

		public StringAttribute(string name)
		{
			_value = name;
		}

		public string Value
		{
			get { return _value; }
		}
	}

	public class SectionAttribute : StringAttribute
	{
		public SectionAttribute (string name) : base (name) {}
	}

	public class KeyAttribute : StringAttribute
	{
		public KeyAttribute (string name) : base (name) {}
	}
		
}

