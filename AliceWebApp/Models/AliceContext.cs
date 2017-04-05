using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AliceWebApp.Models
{
    public class AliceContext : DbContext
    {
        public AliceContext(DbContextOptions<AliceContext> opts) 
            :base(opts)
        {

        }

        public DbSet<Character> Characters { get; set; }

    }
}
