using API.Controllers;
using Business.Constants;
using Business.Dtos.Request;
using Business.Interfaces;
using DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Project.Tests.ControllerTests
{
    public class PostControllerTests
    {
        private readonly Mock<IPostService> _serviceMock;
        private readonly PostController _controller;

        public PostControllerTests()
        {
            _serviceMock = new Mock<IPostService>();
            _controller = new PostController(_serviceMock.Object);
        }

        #region Create (Individual) Tests
        [Fact]
        public async Task Create_ShouldReturnCreated_WhenSuccessful()
        {
            // Arrange
            var dto = new PostCreate { Title = "Test Title", Body = "Test Body", CustomerId = 1 };
            var expectedPost = new Post { PostId = 1, Title = "Test Title" };
            var serviceResponse = new ResponseApi<Post>(expectedPost, AppMessages.PostCreatedSuccess);

            _serviceMock.Setup(s => s.Create(dto)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode); // Validamos el código 201
            var finalResponse = Assert.IsType<ResponseApi<Post>>(objectResult.Value);
            Assert.True(finalResponse.Succeeded);
            Assert.Equal(expectedPost.PostId, finalResponse.Data.PostId);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenCustomerNotFound()
        {
            // Arrange
            var dto = new PostCreate { CustomerId = 999 };
            var serviceResponse = new ResponseApi<Post>(AppMessages.CustomerNotFound);

            _serviceMock.Setup(s => s.Create(dto)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var finalResponse = Assert.IsType<ResponseApi<Post>>(badRequestResult.Value);
            Assert.False(finalResponse.Succeeded);
            Assert.Equal(AppMessages.CustomerNotFound, finalResponse.Message);
        }
        #endregion

        #region CreateBulk Tests
        [Fact]
        public async Task CreateBulk_ShouldReturnOk_WhenListIsProcessed()
        {
            // Arrange
            var dtos = new List<PostCreate>
            {
                new PostCreate { Title = "Post 1" },
                new PostCreate { Title = "Post 2" }
            };
            var serviceResponse = new ResponseApi<bool>(true, string.Format(AppMessages.PostBulkStarted, dtos.Count));

            _serviceMock.Setup(s => s.CreateBulk(dtos)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.CreateBulk(dtos);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var finalResponse = Assert.IsType<ResponseApi<bool>>(okResult.Value);
            Assert.True(finalResponse.Succeeded);
            Assert.Contains(dtos.Count.ToString(), finalResponse.Message);
        }

        [Fact]
        public async Task CreateBulk_ShouldReturnBadRequest_WhenServiceReturnsError()
        {
            // Arrange
            var dtos = new List<PostCreate> { new PostCreate() };
            var serviceResponse = new ResponseApi<bool>(AppMessages.ValidationError);

            _serviceMock.Setup(s => s.CreateBulk(dtos)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.CreateBulk(dtos);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var finalResponse = Assert.IsType<ResponseApi<bool>>(badRequestResult.Value);
            Assert.False(finalResponse.Succeeded);
            Assert.Equal(AppMessages.ValidationError, finalResponse.Message);
        }
        #endregion

    
    }
}