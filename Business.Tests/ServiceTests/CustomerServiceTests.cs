using Business.Common.Constants;
using Business.Dtos.Request;
using Business.Services;
using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Business.Tests.ServiceTests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<ILogger<CustomerService>> _loggerMock;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _customerRepoMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<CustomerService>>();

            _service = new CustomerService(_customerRepoMock.Object, _loggerMock.Object);
        }

        #region GetAll Tests
        [Fact]
        public async Task GetAll_ShouldReturnSuccessResponse_WithList()
        {
            // 1. Arrange
            var customerList = new List<Customer>
                    {
                        new Customer { CustomerId = 1, Name = "Cliente A" }
                    };

            _customerRepoMock.Setup(r => r.GetAllAsync())
                             .ReturnsAsync(customerList);

            // 2. Act
            var result = await _service.GetAll();

            // 3. Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal("Cliente A", result.Data.First().Name);
            Assert.Equal(AppMessages.CustomerListSuccess, result.Message);
        }
        #endregion

        #region Create Tests
        [Fact]
        public async Task Create_ShouldReturnFailure_WhenNameExists()
        {

            var dto = new CustomerCreate { Name = "Juan" };
            var existingCustomer = new Customer { CustomerId = 1, Name = "Juan" };
            _customerRepoMock.Setup(r => r.GetByNameAsync(It.Is<string>(s => s == dto.Name)))
                             .ReturnsAsync(existingCustomer);
            var result = await _service.Create(dto);

            Assert.False(result.Succeeded);
            Assert.Equal(AppMessages.CustomerExists, result.Message);

            _customerRepoMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task Create_ShouldReturnSuccess_WhenSuccessful()
        {
            // Arrange
            var customers = new List<Customer>().AsQueryable().BuildMock();
            _customerRepoMock.Setup(r => r.GetAll).Returns(customers.Object);

            var dto = new CustomerCreate { Name = "Nuevo" };
            var expected = new Customer { CustomerId = 1, Name = "Nuevo" };

            _customerRepoMock.Setup(r => r.Create(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                             .ReturnsAsync(expected);

            // Act
            var result = await _service.Create(dto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(expected.Name, result.Data.Name);
            Assert.Equal(AppMessages.CustomerCreatedSuccess, result.Message);
        }
        #endregion

        #region Update Tests
        [Fact]
        public async Task Update_ShouldReturnFailure_WhenCustomerDoesNotExist()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.FindById(1)).ReturnsAsync((Customer)null);

            // Act
            var result = await _service.Update(1, new CustomerUpdate());

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(AppMessages.CustomerNotFound, result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnSuccess_WhenSuccessful()
        {
            // 1. Arrange
            int customerId = 1;
            var original = new Customer { CustomerId = customerId, Name = "Viejo" };
            var dto = new CustomerUpdate { Name = "Nuevo" };

            _customerRepoMock.Setup(r => r.GetByIdAsync(customerId))
                             .ReturnsAsync(original);
            _customerRepoMock.Setup(r => r.ExistsAnotherNameAsync(dto.Name, customerId))
                             .ReturnsAsync(false);
            _customerRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Customer>()))
                             .Returns(Task.CompletedTask);

            // 2. Act
            var result = await _service.Update(customerId, dto);

            // 3. Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Nuevo", result.Data.Name);
            Assert.Equal(AppMessages.CustomerUpdatedSuccess, result.Message);

            _customerRepoMock.Verify(r => r.UpdateAsync(It.Is<Customer>(c => c.Name == "Nuevo")), Times.Once);
        }
        #endregion

        #region Delete Tests
        [Fact]
        public async Task Delete_ShouldReturnFailure_WhenNotFound()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.FindById(1)).ReturnsAsync((Customer)null);

            // Act
            var result = await _service.Delete(1);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(AppMessages.CustomerNotFound, result.Message);
        }

        [Fact]
        public async Task Delete_ShouldRemovePostsAndCustomer_WhenSuccessful()
        {
            // 1. Arrange
            int customerId = 1;
            var customer = new Customer { CustomerId = customerId, Name = "Juan" };

            _customerRepoMock.Setup(r => r.GetByIdAsync(customerId))
                             .ReturnsAsync(customer);
            _customerRepoMock.Setup(r => r.RemoveWithPostsAsync(customerId))
                             .Returns(Task.CompletedTask);

            // 2. Act
            var result = await _service.Delete(customerId);

            // 3. Assert
            Assert.True(result.Succeeded);
            Assert.True(result.Data);
            Assert.Equal(AppMessages.CustomerDeletedSuccess, result.Message);

            _customerRepoMock.Verify(r => r.RemoveWithPostsAsync(customerId), Times.Once);
        }
        #endregion

        #region GetById Tests
        [Fact]
        public async Task GetByIdAsync_ShouldReturnSuccess_WhenCustomerExists()
        {
            // 1. Arrange
            int customerId = 1;
            var customer = new Customer { CustomerId = customerId, Name = "Cliente Prueba" };

            // Configuramos el Mock para que devuelva el cliente encontrado
            _customerRepoMock.Setup(r => r.GetByIdAsync(customerId))
                             .ReturnsAsync(customer);

            // 2. Act
            var result = await _service.GetByIdAsync(customerId);

            // 3. Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal("Cliente Prueba", result.Data.Name);
            Assert.Equal(AppMessages.PaginationSuccess, result.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFailure_WhenCustomerDoesNotExist()
        {
            // 1. Arrange
            int customerId = 99;

            // Configuramos el Mock para que devuelva null (no encontrado)
            _customerRepoMock.Setup(r => r.GetByIdAsync(customerId))
                             .ReturnsAsync((Customer)null);

            // 2. Act
            var result = await _service.GetByIdAsync(customerId);

            // 3. Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);

            // Verificamos que el mensaje contenga el ID (según tu string.Format)
            var expectedMessage = string.Format(AppMessages.CustomerNotFound, customerId);
            Assert.Equal(expectedMessage, result.Message);
        }
        #endregion

    }
}