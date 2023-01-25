using Microsoft.EntityFrameworkCore;
using OB_Net_Core_Tutorial.DAL.Models;
using Type = OB_Net_Core_Tutorial.DAL.Models.Type;

namespace OB_Net_Core_Tutorial.DAL.Repository
{
    public class TypeRepository : BaseRepository<Type>
    {
        public TypeRepository(DbContext dbContext) : base(dbContext) { }
    }
}
