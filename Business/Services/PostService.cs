using Business.Common.Constants;
using Business.Common.Interfaces;
using Business.Dtos.Request;
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
        private readonly IPostRepository _postRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly ILogger<PostService> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="PostService"/>.
        /// </summary>
        /// <param name="postRepo">Repositorio para la persistencia de publicaciones.</param>
        /// <param name="customerRepo">Repositorio para la verificación de existencia de clientes.</param>
        /// <param name="logger">Instancia de ILogger para el registro de eventos.</param>
        public PostService(IPostRepository postRepo, ICustomerRepository customerRepo, ILogger<PostService> logger)
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
            if (!string.IsNullOrEmpty(dto.Body) && dto.Body.Length > AppConstants.MinBodyThreshold)
            {
                processedBody = dto.Body.Substring(0, Math.Min(dto.Body.Length, AppConstants.MaxBodyLength)) + AppConstants.Ellipsis;
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
        /// Realiza el procesamiento y creación de múltiples publicaciones de forma masiva y asíncrona.
        /// </summary>
        /// <remarks>
        /// Este método garantiza que cada elemento de la lista pase por las mismas 
        /// reglas de validación y negocio que una creación individual.
        /// </remarks>
        /// <param name="entities">Lista de DTOs de tipo <see cref="PostCreate"/>.</param>
        public async Task<ResponseApi<bool>> CreateBulk(IEnumerable<PostCreate> entities)
        {
            if (entities == null || !entities.Any())
                return new ResponseApi<bool>(AppMessages.ValidationError);

            try
            {
                _logger.LogInformation(AppMessages.PostBulkStarted, entities.Count());
                var (postsToInsert, skippedCount) = await ProcessEntities(entities);
                if (postsToInsert.Any())
                {
                    await ExecuteBulkInsert(postsToInsert);
                }

                string message = string.Format(AppMessages.BulkSuccess, postsToInsert.Count, skippedCount);
                return new ResponseApi<bool>(true, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppMessages.LogErrorBulk);
                return new ResponseApi<bool>(AppMessages.InternalServerError);
            }
        }

        /// <summary>
        /// Itera sobre las entidades, aplica validaciones y retorna las procesadas.
        /// </summary>
        private async Task<(List<Post> posts, int skipped)> ProcessEntities(IEnumerable<PostCreate> entities)
        {
            var postsToInsert = new List<Post>();
            int skippedCount = 0;

            foreach (var dto in entities)
            {
                if (!IsValidType(dto.Type))
                {
                    _logger.LogWarning(AppMessages.PostTypeInvalidWarning, dto.Type);
                    skippedCount++;
                    continue;
                }
                var customer = await _customerRepo.GetByIdAsync(dto.CustomerId);
                if (customer == null)
                {
                    _logger.LogWarning(AppMessages.CustomerNotFoundWarning, dto.CustomerId);
                    skippedCount++;
                    continue;
                }
                postsToInsert.Add(MapPost(dto));
            }

            return (postsToInsert, skippedCount);
        }

        /// <summary>
        /// Verifica si el tipo de publicación está dentro de los permitidos.
        /// </summary>
        private bool IsValidType(int type)
        {
            return type == AppConstants.TypeFarandula ||
                   type == AppConstants.TypePolitica ||
                   type == AppConstants.TypeFutbol;
        }

        /// <summary>
        /// Se encarga exclusivamente de la comunicación con el repositorio.
        /// </summary>
        private async Task ExecuteBulkInsert(List<Post> posts)
        {
            await _postRepo.AddRangeAsync(posts);
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
        /// <summary>
        /// Recupera una publicación específica por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResponseApi<PostCreate>> GetByIdAsync(int id)
        {
            try
            {
                var post = await _postRepo.GetByIdAsync(id);

                if (post == null)
                {
                    _logger.LogWarning(AppMessages.PostNotFound, id);
                    return new ResponseApi<PostCreate>(string.Format(AppMessages.PostNotFound, id));
                }

                var response = new PostCreate
                {
                    CustomerId = post.CustomerId,
                    Title = post.Title,
                    Body = post.Body,
                    Category = post.Category,
                    Type = post.Type
                };
                return new ResponseApi<PostCreate>(response, AppMessages.PostGetSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener post {0}", id);
                return new ResponseApi<PostCreate>(AppMessages.InternalServerError);
            }
        }
        /// <summary>
        /// Elimina una publicación de la base de datos tras verificar su existencia.
        /// </summary>
        public async Task<ResponseApi<bool>> Delete(int id)
        {
            try
            {
                var exists = await _postRepo.GetByIdAsync(id);
                if (exists == null)
                {
                    _logger.LogWarning(AppMessages.PostNotFound, id);
                    return new ResponseApi<bool>(string.Format(AppMessages.PostNotFound, id));
                }

                await _postRepo.DeleteAsync(exists);


                _logger.LogInformation("Post {0} eliminado con éxito.", id);
                return new ResponseApi<bool>(true, AppMessages.PostDeleteSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppMessages.PostDeleteError);
                return new ResponseApi<bool>(AppMessages.InternalServerError);
            }
        }
    }
}