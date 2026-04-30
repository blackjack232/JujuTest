using Business.Constants;
using Business.Dtos.Request;
using Business.Interfaces;
using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    /// <summary>
    /// Servicio encargado de la gestión y lógica de negocio de las publicaciones (Posts).
    /// Implementa validaciones de integridad y transformación de datos según requerimientos.
    /// </summary>
    public class PostService : IPostService
    {
        private readonly IBaseModel<Post> _postRepo;
        private readonly IBaseModel<Customer> _customerRepo;
        private readonly ILogger<PostService> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="PostService"/>.
        /// </summary>
        /// <param name="postRepo">Repositorio para la persistencia de publicaciones.</param>
        /// <param name="customerRepo">Repositorio para la verificación de existencia de clientes.</param>
        /// <param name="logger">Instancia de ILogger para el registro de eventos.</param>
        public PostService(IBaseModel<Post> postRepo, IBaseModel<Customer> customerRepo, ILogger<PostService> logger)
        {
            _postRepo = postRepo;
            _customerRepo = customerRepo;
            _logger = logger;
        }

        /// <summary>
        /// Crea una publicación individual procesando reglas de negocio, constantes y validaciones de forma asíncrona (Punto 3).
        /// </summary>
        /// <param name="dto">DTO con la información necesaria para crear el Post.</param>
        /// <returns>La entidad <see cref="Post"/> persistida en la base de datos.</returns>
        /// <exception cref="KeyNotFoundException">Se lanza si el CustomerId proporcionado no existe.</exception>
        public async Task<ResponseApi<Post>> Create(PostCreate dto)
        {
            try
            {
                var customer = await _customerRepo.FindById(dto.CustomerId);
                if (customer == null)
                {
                    _logger.LogWarning(AppMessages.CustomerNotFoundWarning, dto.CustomerId);
                    return new ResponseApi<Post>(AppMessages.CustomerNotFound);
                }

                var post = MapPost(dto);
                var result = await _postRepo.Create(post);

                _logger.LogInformation(AppMessages.PostCreated, result.PostId);
                return new ResponseApi<Post>(result, AppMessages.PostCreatedSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppMessages.PostUpdateError);
                return new ResponseApi<Post>(AppMessages.InternalServerError);
            }
        }

        /// <summary>
        /// Mapea y procesa los datos del DTO a la entidad Post aplicando reglas de formato.
        /// </summary>
        private Post MapPost(PostCreate dto)
        {
            string processedBody = dto.Body;
            if (!string.IsNullOrEmpty(dto.Body) && dto.Body.Length > 20)
            {
                processedBody = dto.Body.Substring(0, Math.Min(dto.Body.Length, 97)) + "...";
            }
            switch (dto.Type)
            {
                case AppConstants.TypeFarandula:
                    dto.Category = AppConstants.CategoryFarandula;
                    break;
                case AppConstants.TypePolitica:
                    dto.Category = AppConstants.CategoryPolitica;
                    break;
                case AppConstants.TypeFutbol:
                    dto.Category = AppConstants.CategoryFutbol;
                    break;
            }

            return new Post
            {
                CustomerId = dto.CustomerId,
                Title = dto.Title,
                Body = processedBody,
                Category = dto.Category,
                Type = dto.Type
            };
        }

        /// <summary>
        /// Realiza el procesamiento y creación de múltiples publicaciones de forma masiva y asíncrona (Punto 5).
        /// </summary>
        /// <remarks>
        /// Este método garantiza que cada elemento de la lista pase por las mismas 
        /// reglas de validación y negocio que una creación individual.
        /// </remarks>
        /// <param name="entities">Lista de DTOs de tipo <see cref="PostCreate"/>.</param>
        public async Task<ResponseApi<bool>> CreateBulk(List<PostCreate> entities)
        {
            if (entities == null || !entities.Any())
                return new ResponseApi<bool>(AppMessages.ValidationError);

            _logger.LogInformation(AppMessages.PostBulkStarted, entities.Count);
            int successCount = 0;

            foreach (var postDto in entities)
            {
                var response = await Create(postDto);
                if (response.Succeeded) successCount++;
            }

            return new ResponseApi<bool>(true, string.Format(AppMessages.PostBulkStarted, successCount));
        }

        /// <summary>
        /// Expone todas las publicaciones registradas como un IQueryable de forma asíncrona.
        /// </summary>
        /// <returns>Consulta base para las publicaciones.</returns>
        /// <summary>
        /// Obtiene una lista paginada de publicaciones de forma asíncrona.
        /// </summary>
        /// <param name="page">Número de la página a recuperar.</param>
        /// <param name="size">Cantidad de registros por página.</param>
        /// <returns>Una respuesta estandarizada con los datos paginados de Post.</returns>
        public async Task<ResponseApi<PagedResponse<Post>>> GetAllPagedAsync(int page, int size)
        {
            try
            {
                var query = _postRepo.GetAll;
                int totalRecords = await query.CountAsync();

                var data = await query
                    .OrderByDescending(p => p.PostId) 
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToListAsync();

                var pagedData = new PagedResponse<Post>
                {
                    Data = data,
                    PageNumber = page,
                    PageSize = size,
                    TotalRecords = totalRecords
                };

                return new ResponseApi<PagedResponse<Post>>(pagedData, AppMessages.PaginationSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppMessages.PostUpdateError);
                return new ResponseApi<PagedResponse<Post>>(AppMessages.InternalServerError);
            }
        }
    }
}