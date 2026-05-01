using API.Controllers;
using Business.Common.Constants;
using Business.Common.Interfaces;
using Business.Dtos.Request;
using DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Api.Tests.ControllerTests
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

        #region Get (Paged) Tests
        [Fact]
        public async Task Get_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            int page = 1;
            int size = 10;

            var pagedResponse = new PagedResponse<Post>
            {
                PageNumber = page,
                PageSize = size,
                Data = new List<Post>(), // Lista vacía para evitar nulos
                TotalRecords = 0
            };

            var serviceResponse = new ResponseApi<PagedResponse<Post>>(pagedResponse);

            _serviceMock.Setup(s => s.GetAllPagedAsync(page, size))
                        .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Get(page, size);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // CORRECCIÓN 2: El tipo debe ser exactamente ResponseApi<PagedResponse<Post>>
            var finalResponse = Assert.IsType<ResponseApi<PagedResponse<Post>>>(okResult.Value);

            Assert.True(finalResponse.Succeeded);
            Assert.NotNull(finalResponse.Data);
            Assert.Equal(page, finalResponse.Data.PageNumber);
        }

        [Fact]
        public async Task Get_ShouldReturnInternalServerError_WhenServiceFails()
        {
            // Arrange
            var serviceResponse = new ResponseApi<PagedResponse<Post>>(AppMessages.InternalServerError);

            _serviceMock.Setup(s => s.GetAllPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
                        .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Get(1, 10);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            var finalResponse = Assert.IsType<ResponseApi<PagedResponse<Post>>>(objectResult.Value);

            Assert.False(finalResponse.Succeeded);
            Assert.Equal(AppMessages.InternalServerError, finalResponse.Message);
        }
        #endregion

        #region GetById Tests
        [Fact]
        public async Task GetById_ShouldReturnOk_WhenFound()
        {
            // Arrange
            int id = 1;
            var expectedPost = new PostCreate { Title = "Found Post" };
            var serviceResponse = new ResponseApi<PostCreate>(expectedPost);

            _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var finalResponse = Assert.IsType<ResponseApi<PostCreate>>(okResult.Value);
            Assert.True(finalResponse.Succeeded);

        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            int id = 99;

            var serviceResponse = new ResponseApi<PostCreate>(AppMessages.PostNotFound);

            _serviceMock.Setup(s => s.GetByIdAsync(id))
                        .ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            var finalResponse = Assert.IsType<ResponseApi<PostCreate>>(notFoundResult.Value);

            Assert.False(finalResponse.Succeeded);
            Assert.Equal(AppMessages.PostNotFound, finalResponse.Message);
        }
        #endregion

        #region Delete Tests
        [Fact]
        public async Task Delete_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            int id = 1;
            var serviceResponse = new ResponseApi<bool>(true, AppMessages.PostDeleteSuccess);
            _serviceMock.Setup(s => s.Delete(id)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var finalResponse = Assert.IsType<ResponseApi<bool>>(okResult.Value);
            Assert.True(finalResponse.Succeeded);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenFails()
        {
            // Arrange
            int id = 1;
            var serviceResponse = new ResponseApi<bool>(AppMessages.PostNotFound);
            _serviceMock.Setup(s => s.Delete(id)).ReturnsAsync(serviceResponse);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var finalResponse = Assert.IsType<ResponseApi<bool>>(badRequestResult.Value);
            Assert.False(finalResponse.Succeeded);
        }
        #endregion

    }
}