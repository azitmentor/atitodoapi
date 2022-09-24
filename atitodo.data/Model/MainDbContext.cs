using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Atitodo.Data.Model
{
	public class MainDbContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
	{
		public DbSet<t_todo> t_todo { get; set; }

		public DbSet<t_user> t_user { get; set; }

		public DbSet<t_accesslink> t_accesslink { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			=> optionsBuilder.UseNpgsql("Host=172.17.0.1;Database=atitodo");

		public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
		{

		}

		public MainDbContext()
		{
		}
	}
}
