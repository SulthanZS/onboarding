using Microsoft.EntityFrameworkCore;
using OB_Net_Core_Tutorial.DAL.Models;
using OB_Net_Core_Tutorial.DAL.Repository;

namespace OB_Net_Core_Tutorial.DAL.Repository
{
	public class CarRepository : BaseRepository<Car>
	{
		public CarRepository(DbContext dbContext) : base(dbContext) { }
	}
}
