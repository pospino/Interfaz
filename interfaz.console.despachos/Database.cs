using FluentData;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace interfaz.console
{
	public class Database : IDisposable
	{
		private static IDbContext context;

		public static IDbContext Context
		{
			get
			{
				if (Database.context == null)
				{
					Database.context = (new DbContext()).ConnectionStringName("MySql", new MySqlProvider());
				}
				return Database.context;
			}
		}

		static Database()
		{
			Database.context = null;
		}

		public Database()
		{
		}

		public void Dispose()
		{
			if (Database.context != null)
			{
				Database.context.Dispose();
			}
		}

	
	}
}