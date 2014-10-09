using System;
using System.Reflection;

namespace MoistureBot
{
	public static class EnumUtils
	{
		public static string GetValue<T>(Enum value) where T : StringAttribute
		{
			string output = null;
			Type type = value.GetType();

			FieldInfo fi = type.GetField(value.ToString());
			T[] attrs =
				fi.GetCustomAttributes(typeof(T),
					false) as T[];
			if (attrs.Length > 0)
			{
				output = attrs[0].Value;
			}

			return output;
		}

	}
}

