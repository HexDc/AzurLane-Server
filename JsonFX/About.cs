using System;
using System.Reflection;

namespace JsonFx
{
	/// <summary>
	/// JsonFx metadata
	/// </summary>
	public sealed class About
	{
		#region Fields

		public static readonly About Fx = new About(typeof(About).Assembly);

		public readonly Version Version;
		public readonly string FullName;
		public readonly string Name;
		public readonly string Configuration;
		public readonly string Copyright;
		public readonly string Title;
		public readonly string Description;
		public readonly string Company;

		#endregion Fields

		#region Init

		/// <summary>
		/// Ctor
		/// </summary>
		public About(Assembly assembly)
		{
			AssemblyName name = assembly.GetName();

			this.FullName = assembly.FullName;
			this.Version = name.Version;
			this.Name = name.Name;

			AssemblyCopyrightAttribute copyright = Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute)) as AssemblyCopyrightAttribute;
			this.Copyright = (copyright != null) ? copyright.Copyright : String.Empty;

			AssemblyDescriptionAttribute description = Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute)) as AssemblyDescriptionAttribute;
			this.Description = (description != null) ? description.Description : String.Empty;

			AssemblyTitleAttribute title = Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute)) as AssemblyTitleAttribute;
			this.Title = (title != null) ? title.Title : String.Empty;

			AssemblyCompanyAttribute company = Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute)) as AssemblyCompanyAttribute;
			this.Company = (company != null) ? company.Company : String.Empty;

			AssemblyConfigurationAttribute config = Attribute.GetCustomAttribute(assembly, typeof(AssemblyConfigurationAttribute)) as AssemblyConfigurationAttribute;
			this.Configuration = (config != null) ? config.Configuration : String.Empty;
		}

		#endregion Init
	}
}