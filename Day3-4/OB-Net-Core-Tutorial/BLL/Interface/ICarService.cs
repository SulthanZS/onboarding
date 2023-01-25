using Microsoft.AspNetCore.Mvc;
using OB_Net_Core_Tutorial.BLL.DTO;
using OB_Net_Core_Tutorial.DAL.Models;

namespace OBNetCoreTutorial.BLL.Interface
{
    public interface ICarService
    {
        IEnumerable<Car> GetAllCar();
        Car GetCarById(Guid id);
        Task CreateCarAsync(CarDTO carDTO);
        Task<IActionResult> EditCar(Guid id, CarDTO carDTO);
        Task<IActionResult> DeleteCar(Guid id);
    }
}
