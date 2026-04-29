using Business.Constants;
using Business.Dtos;
using Business.Interfaces;
using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public PostService(IBaseModel<Post> postRepo, IBaseModel<Customer> customerRepo, ILogger<PostService> logger)
        {
            _postRepo = postRepo;
            _customerRepo = customerRepo;
            _logger = logger;
        }

        /// <summary>
        /// Crea una publicación individual procesando reglas de negocio, constantes y validaciones (Punto 3).
        /// </summary>
        /// <param name="dto">DTO con la información necesaria para crear el Post.</param>
        /// <returns>La entidad <see cref="Post"/> persistida en la base de datos.</returns>
        /// <exception cref="KeyNotFoundException">Se lanza si el CustomerId proporcionado no existe.</exception>
        public Post Create(PostCreateDto dto)
        {
            try
            {
                // 1. Validar existencia de usuario usando el repositorio inyectado
                if (!_customerRepo.GetAll.Any(c => c.CustomerId == dto.CustomerId))
                {
                    _logger.LogWarning(AppMessages.UserNotFound + " ID: {CustomerId}", dto.CustomerId);
                    throw new KeyNotFoundException(AppMessages.UserNotFound);
                }

                var entity = new Post
                {
                    CustomerId = dto.CustomerId,
                    Title = dto.Title,
                    Body = dto.Body,
                    Type = dto.Type,
                    Category = dto.Category
                };

                // 2. Lógica de Body (Truncado a 97 caracteres si supera los 20)
                if (!string.IsNullOrEmpty(entity.Body) && entity.Body.Length > AppConstants.MinBodyThreshold)
                {
                    if (entity.Body.Length > AppConstants.MaxBodyLength)
                    {
                        entity.Body = entity.Body.Substring(0, AppConstants.MaxBodyLength);
                    }
                    entity.Body += "...";
                }

                // 3. Lógica de Categoría (Switch clásico compatible con C# 7.3)
                switch (entity.Type)
                {
                    case AppConstants.TypeFarandula:
                        entity.Category = AppConstants.CategoryFarandula;
                        break;
                    case AppConstants.TypePolitica:
                        entity.Category = AppConstants.CategoryPolitica;
                        break;
                    case AppConstants.TypeFutbol:
                        entity.Category = AppConstants.CategoryFutbol;
                        break;
                    default:
                        // Mantiene la categoría original si no coincide con los tipos definidos
                        break;
                }

                var result = _postRepo.Create(entity);
                _logger.LogInformation(AppMessages.PostCreated, result.PostId);

                return result;
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException))
            {
                _logger.LogError(ex, AppMessages.PostUpdateError);
                throw;
            }
        }

        /// <summary>
        /// Realiza el procesamiento y creación de múltiples publicaciones de forma masiva (Punto 5).
        /// </summary>
        /// <remarks>
        /// Este método garantiza que cada elemento de la lista pase por las mismas 
        /// reglas de validación y negocio que una creación individual.
        /// </remarks>
        /// <param name="entities">Lista de DTOs de tipo <see cref="PostCreateDto"/>.</param>
        public void CreateBulk(List<PostCreateDto> entities)
        {
            if (entities == null || !entities.Any()) return;

            _logger.LogInformation(AppMessages.PostBulkStarted, entities.Count);

            foreach (var postDto in entities)
            {
                try
                {
                    Create(postDto);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, AppMessages.LogErrorBulk);
                }
            }
        }

        /// <summary>
        /// Expone todas las publicaciones registradas como un IQueryable.
        /// </summary>
        /// <returns>Consulta base para las publicaciones.</returns>
        public IQueryable<Post> GetAll() => _postRepo.GetAll;
    }
}
