using Business.Common.Constants;
using Business.Dtos.Request;
using Business.Services;
using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Business.Tests.ServiceTests
{
    public class PostServiceTests
    {
        private readonly Mock<IPostRepository> _postRepoMock;
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<ILogger<PostService>> _loggerMock;
        private readonly PostService _service;

        public PostServiceTests()
        {
            _postRepoMock = new Mock<IPostRepository>();
            _customerRepoMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<PostService>>();
            _service = new PostService(_postRepoMock.Object, _customerRepoMock.Object, _loggerMock.Object);
        }

        #region Create Tests
        [Fact]
        public async Task Create_ShouldReturnSuccess_WhenDataIsValid()
        {
            // Arrange
            var dto = new PostCreate
            {
                CustomerId = 1,
                Title = "Test",
                Body = "Contenido corto",
                Type = AppConstants.TypeFarandula
            };

            _customerRepoMock.Setup(r => r.FindById(1))
                .ReturnsAsync(new Customer { CustomerId = 1 });

            _postRepoMock.Setup(r => r.Create(It.IsAny<Post>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Post p, CancellationToken ct) =>
                {
                    p.PostId = 100;
                    return p;
                });

            // Act
            var result = await _service.Create(dto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(AppConstants.CategoryFarandula, result.Data.Category);

            _postRepoMock.Verify(r => r.Create(
                It.Is<Post>(p => p.Body == dto.Body && p.Category == AppConstants.CategoryFarandula),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Create_ShouldTruncateBody_WhenBodyIsTooLong()
        {
            // Arrange
            string longBody = new string('a', AppConstants.MaxBodyLength + 10);
            var dto = new PostCreate { CustomerId = 1, Body = longBody, Type = AppConstants.TypePolitica };
            _customerRepoMock.Setup(r => r.FindById(1)).ReturnsAsync(new Customer());
            _postRepoMock.Setup(r => r.Create(It.IsAny<Post>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Post());

            // Act
            await _service.Create(dto);

            // Assert
            _postRepoMock.Verify(r => r.Create(It.Is<Post>(p => p.Body.EndsWith(AppConstants.Ellipsis)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldReturnFailure_WhenCustomerNotFound()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.FindById(It.IsAny<int>())).ReturnsAsync((Customer)null);

            // Act
            var result = await _service.Create(new PostCreate { CustomerId = 99 });

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal(AppMessages.CustomerNotFound, result.Message);
        }
        #endregion

        #region Bulk Tests
        [Fact]
        public async Task CreateBulk_ShouldReturnSuccess_WithCorrectCounts()
        {
            // Arrange
            var list = new List<PostCreate> {
                new PostCreate { CustomerId = 1, Type = AppConstants.TypeFutbol }, // Válido
                new PostCreate { CustomerId = 1, Type = 999 },                      // Tipo inválido
                new PostCreate { CustomerId = 2, Type = AppConstants.TypeFutbol }  // Cliente no existe
            };

            _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Customer());
            _customerRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Customer)null);

            // Act
            var result = await _service.CreateBulk(list);

            // Assert
            Assert.True(result.Succeeded);
            // 1 insertado, 2 omitidos (1 por tipo, 1 por cliente no encontrado)
            string expectedMsg = string.Format(AppMessages.BulkSuccess, 1, 2);
            Assert.Equal(expectedMsg, result.Message);
            _postRepoMock.Verify(r => r.AddRangeAsync(It.Is<List<Post>>(l => l.Count == 1)), Times.Once);
        }

        [Fact]
        public async Task CreateBulk_ShouldReturnError_WhenListIsEmpty()
        {
            var result = await _service.CreateBulk(new List<PostCreate>());
            Assert.False(result.Succeeded);
        }
        #endregion

        #region Pagination Tests
        [Fact]
        public async Task GetAllPagedAsync_ShouldReturnPagedData()
        {
            // Arrange
            var posts = new List<Post> { new Post { PostId = 1 }, new Post { PostId = 2 } };
            var mockQuery = posts.AsQueryable().BuildMock();
            _postRepoMock.Setup(r => r.GetAll).Returns(mockQuery.Object);

            // Act
            var result = await _service.GetAllPagedAsync(1, 10);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(2, result.Data.TotalRecords);
            Assert.Equal(1, result.Data.PageNumber);
        }
        #endregion

        #region GetById & Delete Tests
        [Fact]
        public async Task GetByIdAsync_ShouldReturnFailure_WhenPostNotFound()
        {
            _postRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Post)null);
            var result = await _service.GetByIdAsync(1);
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSuccess_WhenPostExists()
        {
            _postRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Post { Title = "Hola" });
            var result = await _service.GetByIdAsync(1);
            Assert.True(result.Succeeded);
            Assert.Equal("Hola", result.Data.Title);
        }

        [Fact]
        public async Task Delete_ShouldReturnSuccess_WhenPostExists()
        {
            var post = new Post { PostId = 1 };
            _postRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

            var result = await _service.Delete(1);

            Assert.True(result.Succeeded);
            _postRepoMock.Verify(r => r.DeleteAsync(post), Times.Once);
        }
        #endregion

        #region Exception Handling (Coverage for Catch blocks)
        [Fact]
        public async Task Delete_ShouldReturnError_OnException()
        {
            _postRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception());
            var result = await _service.Delete(1);
            Assert.Equal(AppMessages.InternalServerError, result.Message);
        }

        [Fact]
        public async Task Create_ShouldReturnError_OnException()
        {
            _customerRepoMock.Setup(r => r.FindById(It.IsAny<int>())).ThrowsAsync(new Exception());
            var result = await _service.Create(new PostCreate());
            Assert.Equal(AppMessages.InternalServerError, result.Message);
        }
        #endregion
    }
}