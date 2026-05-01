using Business.Common.Constants;
using Business.Common.Dtos.Request;
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

namespace Project.Tests.ServiceTests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<IPostRepository> _postRepoMock;
        private readonly Mock<ILogger<CustomerService>> _loggerMock;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _customerRepoMock = new Mock<ICustomerRepository>();
            _postRepoMock = new Mock<IPostRepository>();
            _loggerMock = new Mock<ILogger<CustomerService>>();

            _service = new CustomerService(
                _customerRepoMock.Object,
                _postRepoMock.Object,
                _loggerMock.Object);
        }

        #region GetAll Tests
        [Fact]
        public async Task GetAll_ShouldReturnSuccessResponse_WithList()
        {
            // Arrange
            var customerList = new List<Customer> { new Customer { Name = "Cliente A" } };
            var mockQueryable = customerList.AsQueryable().BuildMock();
            _customerRepoMock.Setup(r => r.GetAll).Returns(mockQueryable.Object);

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.True(result.Succeeded);
            Assert.Single(result.Data);
            Assert.Equal("Cliente A", result.Data.First().Name);
        }
        #endregion

        #region Create Tests
        [Fact]
        public async Task Create_ShouldReturnFailure_WhenNameExists()
        {
            // Arrange
            var existing = new List<Customer> { new Customer { Name = "Juan" } }.AsQueryable().BuildMock();
            _customerRepoMock.Setup(r => r.GetAll).Returns(existing.Object);
            var dto = new CustomerCreate { Name = "Juan" };

            // Act
            var result = await _service.Create(dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(AppMessages.CustomerExists, result.Message);
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
            // Arrange
            var original = new Customer { CustomerId = 1, Name = "Viejo" };
            var dto = new CustomerUpdate { Name = "Nuevo" };
            _customerRepoMock.Setup(r => r.FindById(1)).ReturnsAsync(original);
            _customerRepoMock.Setup(r => r.Update(It.IsAny<Customer>(), It.IsAny<Customer>()))
                             .ReturnsAsync(original);

            // Act
            var result = await _service.Update(1, dto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Nuevo", result.Data.Name);
            Assert.Equal(AppMessages.CustomerUpdatedSuccess, result.Message);
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
            // Arrange
            var customer = new Customer { CustomerId = 1 };
            var postsList = new List<Post> {
                new Post { PostId = 10, CustomerId = 1 },
                new Post { PostId = 11, CustomerId = 1 }
            };
            var postsMock = postsList.AsQueryable().BuildMock();

            _customerRepoMock.Setup(r => r.FindById(1)).ReturnsAsync(customer);
            _customerRepoMock.Setup(r => r.Delete(customer)).ReturnsAsync(customer);
            _postRepoMock.Setup(r => r.GetAll).Returns(postsMock.Object);
            _postRepoMock.Setup(r => r.Delete(It.IsAny<Post>())).ReturnsAsync((Post p) => p);

            // Act
            var result = await _service.Delete(1);

            // Assert
            Assert.True(result.Succeeded);
            Assert.True(result.Data); // El Data es un booleano en Delete
            _postRepoMock.Verify(r => r.Delete(It.IsAny<Post>()), Times.Exactly(2));
            _customerRepoMock.Verify(r => r.Delete(customer), Times.Once);
        }
        #endregion
    }
}