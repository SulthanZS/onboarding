using AutoMapper;
using AutoMapper.QueryableExtensions;
using OB_Net_Core_Tutorial.BLL.DTO;
using OB_Net_Core_Tutorial.BLL.Interface;
using OB_Net_Core_Tutorial.BLL.Service;
using OB_Net_Core_Tutorial.DAL.Models;
using OB_Net_Core_Tutorial.DAL.Repositories;
using OBNetCoreTutorial.BLL.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace OB_Net_Core_Tutorial.BLL.Service
{
    public class CarService : ICarService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IRedisService _redisService;
        private readonly IPublisherService _publisherService;

        public CarService(IUnitOfWork unitOfWork, IRedisService redisService, IPublisherService publisherService) 
        {
            _unitOfWork = unitOfWork;
            _redisService = redisService;
            _publisherService = publisherService;
        }

        public IEnumerable<Car> GetAllCar() 
        {
            return  _unitOfWork.CarRepository.GetAll();
        }

        public Car GetCarById(Guid id) 
        {
            var car = _redisService.GetValue<Car>($"car:{id}");

            if (car == null)
            {
                var theCar = _unitOfWork.CarRepository.GetByIdGuid(id);
                if (theCar != null)
                {
                    _redisService.SaveAsync($"car:{id}", theCar);
                    return theCar;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return car;
            }
        }

        public async Task CreateCarAsync(CarDTO carDTO) 
        {
            if(carDTO.Name == null || carDTO.Name == "")
            {
                throw new NullReferenceException();
            }
            else if (carDTO.TypeId == null || carDTO.TypeId == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                throw new DbUpdateException();
            }
            else
            {
                Car newCar = new Car();
                newCar.Name = carDTO.Name;
                newCar.TypeId = carDTO.TypeId;

                await _unitOfWork.CarRepository.AddAsync(newCar);
                await _unitOfWork.SaveAsync();

                await _publisherService.SendCarToEventHub(newCar);
            }


        }

        public async Task<IActionResult> EditCar(Guid id, CarDTO carDTO) 
        {
            var Editcar = _unitOfWork.CarRepository.GetByIdGuid(id);

            if (Editcar == null)
            {
                return new NotFoundResult();
            }
            else if (carDTO.Name == null || carDTO.Name == "")
            {
                throw new NullReferenceException();
            }
            else if (carDTO.TypeId == null || carDTO.TypeId == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                throw new DbUpdateException();
            }
            else
            {
                Editcar.Name = carDTO.Name;
                Editcar.TypeId = carDTO.TypeId;

                _unitOfWork.CarRepository.Edit(Editcar);
                _unitOfWork.Save();

                return new OkObjectResult(Editcar);
            }

        }

        public async Task<IActionResult> DeleteCar(Guid id) 
        {
            var car = _unitOfWork.CarRepository.GetByIdGuid(id);
            if (car == null) 
            {
                return new NotFoundResult();
            }
            else
            {
                _unitOfWork.CarRepository.Delete(car);
                _unitOfWork.Save();

                await _redisService.DeleteAsync($"car:{id}");
                return new OkResult();
            }
        }


    }
}
