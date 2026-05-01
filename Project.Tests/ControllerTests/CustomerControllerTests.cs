using API.Controllers;
using Business.Common.Constants;
using Business.Common.Dtos.Request;
using Business.Common.Interfaces;
using DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Project.Tests.ControllerTests
{
    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerService> _serviceMock;
        private readonly CustomerController _controller;

        public CustomerControllerTests()
        {
            _serviceMock = new Mock<ICustomerService>();
            _controller = new CustomerController(_serviceMock.Object);
        }

        #region Get Tests
        [Fact]
        public async Task Get_ShouldReturnOk_WithPagedData()
        {
            // Arrange
            int page = 1;
            int size = 10;
            var pagedData = new PagedResponse<Customer>
            {
                Data = new List<Customer> { new Customer { CustomerId = 1, Name = "Test" } },
                PageSize = size
            };

            var serviceResponse = new ResponseApi<PagedResponse<Customer>>(pagedData, AppMessages.PaginationSuccess);

            _serviceMock.Setup(s => s.GetPagedCostumersAsync(page, size))
                        .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Get(page, size);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var finalResponse = Assert.IsType<ResponseApi<PagedResponse<Customer>>>(okResult.Value);
            Assert.True(finalResponse.Succeeded);
            Assert.Equal(pagedData, finalResponse.Data);
        }
        #endregion

        #region Create Tests
        [Fact]
        public async Task Create_ShouldReturnCreated_WhenSuccessful()
        {
            // Arrange
            var dto = new CustomerCreate { Name = "New Customer" };
            var createdCustomer = new Customer { CustomerId = 1, Name = "New Customer" };
            var serviceResponse = new ResponseApi<Customer>(createdCustomer, AppMessages.CustomerCreatedSuccess);

            _serviceMock.Setup(s => s.Create(dto)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);
            var finalResponse = Assert.IsType<ResponseApi<Customer>>(objectResult.Value);
            Assert.True(finalResponse.Succeeded);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenCustomerExists()
        {
            // Arrange
            var dto = new CustomerCreate { Name = "Existing" };
            var serviceResponse = new ResponseApi<Customer>(AppMessages.CustomerExists);

            _serviceMock.Setup(s => s.Create(dto)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var finalResponse = Assert.IsType<ResponseApi<Customer>>(badRequestResult.Value);
            Assert.False(finalResponse.Succeeded);
            Assert.Equal(AppMessages.CustomerExists, finalResponse.Message);
        }
        #endregion

        #region Update Tests
        [Fact]
        public async Task Update_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            int id = 1;
            var dto = new CustomerUpdate { Name = "Updated" };
            var updatedCustomer = new Customer { CustomerId = id, Name = "Updated" };
            var serviceResponse = new ResponseApi<Customer>(updatedCustomer, AppMessages.CustomerUpdatedSuccess);

            _serviceMock.Setup(s => s.Update(id, dto)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var finalResponse = Assert.IsType<ResponseApi<Customer>>(okResult.Value);
            Assert.True(finalResponse.Succeeded);
        }

        [Fact]
        public async Task Update_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            int id = 99;
            var dto = new CustomerUpdate { Name = "No exist" };
            var serviceResponse = new ResponseApi<Customer>(AppMessages.CustomerNotFound);

            _serviceMock.Setup(s => s.Update(id, dto)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Update(id, dto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var finalResponse = Assert.IsType<ResponseApi<Customer>>(notFoundResult.Value);
            Assert.False(finalResponse.Succeeded);
        }
        #endregion

        #region Delete Tests
        [Fact]
        public async Task Delete_ShouldReturnOk_WhenDeletedSuccessfully()
        {
            // Arrange
            int id = 1;
            var serviceResponse = new ResponseApi<bool>(true, AppMessages.CustomerDeletedSuccess);
            _serviceMock.Setup(s => s.Delete(id)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var finalResponse = Assert.IsType<ResponseApi<bool>>(okResult.Value);
            Assert.True(finalResponse.Succeeded);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenDeleteFails()
        {
            // Arrange
            int id = 1;
            var serviceResponse = new ResponseApi<bool>(AppMessages.CustomerNotFound);
            _serviceMock.Setup(s => s.Delete(id)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var finalResponse = Assert.IsType<ResponseApi<bool>>(notFoundResult.Value);
            Assert.False(finalResponse.Succeeded);
        }
        #endregion
    }
}