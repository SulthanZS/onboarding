using System.Linq.Expressions;
using Xunit;
using MockQueryable.Moq;
using Moq;
using OB_Net_Core_Tutorial.BLL.Interface;
using OB_Net_Core_Tutorial.BLL.Service;
using OB_Net_Core_Tutorial.DAL.Repositories;
using OB_Net_Core_Tutorial.DAL.Models;
using OB_Net_Core_Tutorial.BLL.DTO;
using OB_Net_Core_Tutorial.BLL.Test.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace OB_Net_Core_Tutorial.BLL.TEST
{
    public class CarServiceTest
    {
        private IEnumerable<Car> cars;
        private Mock<IRedisService> redis;
        private Mock<IUnitOfWork> uow;
        private Mock<IPublisherService> publisher;

        public CarServiceTest()
        {
            cars = CommonHelper.LoadDataFromFile<IEnumerable<Car>>(@"MockData\Car.json");
            uow = MockUnitOfWork();
            redis = MockRedis();
            publisher = MockPublisher();
        }


        private CarService CreateCarService()
        {
            return new CarService(uow.Object, redis.Object, publisher.Object);
        }

        #region method mock depedencies


        private Mock<IUnitOfWork> MockUnitOfWork()
        {
            var carsQueryable = cars.AsQueryable();

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork
                .Setup(u => u.CarRepository.GetAll())
                .Returns(carsQueryable.BuildMock());

            mockUnitOfWork
                .Setup(u => u.CarRepository.IsExist(It.IsAny<Expression<Func<Car, bool>>>()))
                .Returns((Expression<Func<Car, bool>> condition) => carsQueryable.Any(condition));

            mockUnitOfWork
               .Setup(u => u.CarRepository.GetSingleAsync(It.IsAny<Expression<Func<Car, bool>>>()))
               .ReturnsAsync((Expression<Func<Car, bool>> condition) => carsQueryable.FirstOrDefault(condition));

            mockUnitOfWork
               .Setup(u => u.CarRepository.GetByIdGuid(It.Is<Guid>(u => u.Equals(Guid.Parse("2865FABE-D428-4153-5B41-08DAFA39475E")))))
               .Returns(cars.FirstOrDefault(x => x.Id == Guid.Parse("2865FABE-D428-4153-5B41-08DAFA39475E")));

            mockUnitOfWork
               .Setup(u => u.CarRepository.AddAsync(It.IsAny<Car>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((Car car, CancellationToken token) =>
               {
                   car.Id = Guid.NewGuid();
                   return car;
               });

            mockUnitOfWork
                .Setup(u => u.CarRepository.Delete(It.IsAny<Expression<Func<Car, bool>>>()))
                .Verifiable();


            mockUnitOfWork
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            return mockUnitOfWork;
        }


        private Mock<IRedisService> MockRedis()
        {
            var mockRedis = new Mock<IRedisService>();
            var id = Guid.Parse("2865FABE-D428-4153-5B41-08DAFA39475E");

            mockRedis
                .Setup(x => x.GetAsync<Car>(It.Is<string>(x => x.Equals("car:2865FABE-D428-4153-5B41-08DAFA39475E"))))
                .ReturnsAsync(cars.FirstOrDefault(x => x.Id == Guid.Parse("2865FABE-D428-4153-5B41-08DAFA39475E")))
                .Verifiable();

            mockRedis
                .Setup(x => x.GetValue<Car>(It.Is<string>(x => x.Equals($"car:{id}"))))
                .Returns(cars.FirstOrDefault(x => x.Id == Guid.Parse("2865FABE-D428-4153-5B41-08DAFA39475E")))
                .Verifiable();

            mockRedis
                .Setup(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            mockRedis
              .Setup(x => x.DeleteAsync(It.IsAny<string>())).Verifiable();

            return mockRedis;
        }

        private Mock<IPublisherService> MockPublisher()
        {
            var mockPublisher = new Mock<IPublisherService>();

            mockPublisher
                .Setup(x => x.SendCarToEventHub(It.IsAny<Car>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            return mockPublisher;
        }


        #endregion method mock depedencies
        [Fact]
        public async Task GetAll_Success()
        {
            //arrange
            var expected = cars;

            var svc = CreateCarService();

            // act
            var actual =  svc.GetAllCar();

            // assert      
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("2865FABE-D428-4153-5B41-08DAFA39475E")]
        public async Task GetById_Success(string Id)
        {

            //arrange
            var id = Guid.Parse(Id);
            var expected = cars.First(x => x.Id == id);

            var svc = CreateCarService();

            //act
            var actual = svc.GetCarById(id);

            //assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("2865FABE-D428-4153-5B41-08DAFA39475E")]
        public async Task GetById_InRedis_Success(string Id)
        {

            //arrange
            var id = Guid.Parse(Id);
            var expected = cars.First(x => x.Id == id);

            var svc = CreateCarService();

            //act
            var actual =  svc.GetCarById(id);

            //assert
            actual.Should().BeEquivalentTo(expected);

            redis.Verify(x => x.GetValue<Car>($"car:{id}"), Times.Once);
            redis.Verify(x => x.SaveAsync($"car:{id}", It.IsAny<Car>()), Times.Never);
        }

        [Theory]
        [InlineData("2865FABE-D428-4153-5B41-08DAFA39475F")]
        public async Task GetById_NotFound(string Id)
        {

            //arrange
            var id = Guid.Parse(Id);

            var svc = CreateCarService();

            //act
            var actual = svc.GetCarById(id);

            //assert
            actual.Should().BeNull();
            redis.Verify(x => x.GetValue<Car>($"car:{id}"), Times.Once);
            uow.Verify(x => x.CarRepository.GetByIdGuid(id), Times.Once);
            redis.Verify(x => x.SaveAsync($"car:{id}", It.IsAny<Car>()), Times.Never);
        }

        [Fact]
        public async Task CreateCar_Success()
        {
            //arrange
            var dto = new CarDTO
            {
                Name = "TestTest",
                TypeId = Guid.Parse("00000000-0000-0000-0000-000000000002")
            };

            var svc = CreateCarService();

            //act
            Func<Task> act = async () => { await svc.CreateCarAsync(dto); };
            await act.Should().NotThrowAsync<Exception>();

            //assert
            uow.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateCar_NoName()
        {
            //arrange
            var dto = new CarDTO
            {
                TypeId = Guid.Parse("00000000-0000-0000-0000-000000000002")
            };

            var svc = CreateCarService();

            //act
            Func<Task> act = async () => { await svc.CreateCarAsync(dto); };

            //assert
            await act.Should().ThrowAsync<NullReferenceException>();
            uow.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateCar_NoTypeId()
        {
            //arrange
            var dto = new CarDTO
            {
                Name = "TestCreateNoTypeId"
            };

            var svc = CreateCarService();

            //act
            Func<Task> act = async () => { await svc.CreateCarAsync(dto); };

            //assert
            await act.Should().ThrowAsync<DbUpdateException>();
            uow.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [InlineData("2865FABE-D428-4153-5B41-08DAFA39475E", "TestCar1Edit", "00000000-0000-0000-0000-000000000002")]
        public async Task EditCar_Success(string Id, string Name, string TypeId)
        {
            //arrange
            var dto = new CarDTO
            {
                Name = Name,
                TypeId = Guid.Parse(TypeId)
            };

            var id = Guid.Parse(Id);

            var svc = CreateCarService();

            //act
            Func<Task> act = async () => { await svc.EditCar(id, dto); };
            await act.Should().NotThrowAsync<Exception>();

            //assert
            uow.Verify(x => x.CarRepository.GetByIdGuid(id), Times.Once);
            uow.Verify(x => x.Save(), Times.Once);
        }

        [Theory]
        [InlineData("2865FABE-D428-4153-5B41-08DAFA39475B", "TestCar1Edit", "00000000-0000-0000-0000-000000000002")]
        public async Task EditCar_CarNotFound(string Id, string Name, string TypeId)
        {
            //arrange
            var dto = new CarDTO
            {
                Name = Name,
                TypeId = Guid.Parse(TypeId)
            };

            var id = Guid.Parse(Id);

            var svc = CreateCarService();

            //act
            Func<Task> act = async () => { await svc.EditCar(id, dto); };
            await act.Should().NotThrowAsync<Exception>();

            //assert
            uow.Verify(x => x.CarRepository.GetByIdGuid(id), Times.Once);
            uow.Verify(x => x.Save(), Times.Never);
        }

        [Theory]
        [InlineData("2865FABE-D428-4153-5B41-08DAFA39475E", "00000000-0000-0000-0000-000000000002")]
        public async Task EditCar_NoName(string Id, string TypeId)
        {
            //arrange
            var dto = new CarDTO
            {
                TypeId = Guid.Parse(TypeId)
            };

            var id = Guid.Parse(Id);

            var svc = CreateCarService();

            //act
            Func<Task> act = async () => { await svc.EditCar(id, dto); };

            //assert
            await act.Should().ThrowAsync<NullReferenceException>();
            uow.Verify(x => x.CarRepository.GetByIdGuid(id), Times.Once);  
            uow.Verify(x => x.Save(), Times.Never);
        }

        [Theory]
        [InlineData("2865FABE-D428-4153-5B41-08DAFA39475E", "TestCar1Edit")]
        public async Task EditCar_NoTypeId(string Id, string Name)
        {
            //arrange
            var dto = new CarDTO
            {
                Name = Name
            };

            var id = Guid.Parse(Id);

            var svc = CreateCarService();

            //act
            Func<Task> act = async () => { await svc.EditCar(id, dto); };

            //assert
            await act.Should().ThrowAsync<DbUpdateException>();
            uow.Verify(x => x.CarRepository.GetByIdGuid(id), Times.Once);
            uow.Verify(x => x.Save(), Times.Never);
        }

        [Theory]
        [InlineData("2865FABE-D428-4153-5B41-08DAFA39475E")]
        public async Task Delete_Success(string Id)
        {
            //arrange
            var id = Guid.Parse(Id);

            var svc = CreateCarService();

            //act
            Func<Task> act = async () => { svc.DeleteCar(id); };
            await act.Should().NotThrowAsync<Exception>();

            //assert
            uow.Verify(x => x.CarRepository.GetByIdGuid(id), Times.Once);
            uow.Verify(x => x.Save(), Times.Once);
            redis.Verify(x => x.DeleteAsync(It.Is<string>(y => y.Equals($"car:{id}"))), Times.Once);
        }

        [Theory]
        [InlineData("2865FABE-D428-4153-5B41-08DAFA39475B")]
        public async Task DeleteCarNotFound(string Id)
        {
            //arrange
            var id = Guid.Parse(Id);

            var svc = CreateCarService();

            //act
            Func<Task> act = async () => { svc.DeleteCar(id); };
            await act.Should().NotThrowAsync<Exception>();

            //assert
            uow.Verify(x => x.CarRepository.GetByIdGuid(id), Times.Once);
            uow.Verify(x => x.Save(), Times.Never);
            redis.Verify(x => x.DeleteAsync(It.Is<string>(y => y.Equals($"car:{id}"))), Times.Never);
        }
    }
}
