using System;
using Mono.Addins;

namespace MoistureBot
{
	public class ConsoleCommandAttribute : CustomExtensionAttribute
	{
		public ConsoleCommandAttribute ()
		{
		}
			
		public ConsoleCommandAttribute (
			[NodeAttribute ("Name")] string name,
			[NodeAttribute ("ShortUsage")] string shortUsage,
			[NodeAttribute ("ShortDescription")] string shortDescription,
			[NodeAttribute ("Description")] string description,
			[NodeAttribute ("Usage")] string usage
			)
		{
			Name = name;
			ShortUsage = shortUsage;
			ShortDescription = shortDescription;
			Description = Description;
			Usage = usage;
		}

		[NodeAttribute]
		public string Name { get; set; }

		[NodeAttribute( "Description", typeof(Array)) ]
		public string Description { get; set; }

		[NodeAttribute]
		public string ShortUsage { get; set; }

		[NodeAttribute]
		public string ShortDescription { get; set; }

		[NodeAttribute]
		public string Usage { get; set; }

	}
}

