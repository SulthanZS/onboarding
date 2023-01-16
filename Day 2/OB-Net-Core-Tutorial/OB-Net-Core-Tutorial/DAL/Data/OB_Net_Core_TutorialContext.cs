using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OB_Net_Core_Tutorial.DAL.Models;

namespace OB_Net_Core_Tutorial.DAL.Data
{
    public class OB_Net_Core_TutorialContext : DbContext
    {
        public OB_Net_Core_TutorialContext (DbContextOptions<OB_Net_Core_TutorialContext> options)
            : base(options)
        {
        }
        public DbSet<OB_Net_Core_Tutorial.DAL.Models.Car> Car { get; set; } = default!;
        public DbSet<OB_Net_Core_Tutorial.DAL.Models.Type> Type { get; set; } = default!;
    }
}
