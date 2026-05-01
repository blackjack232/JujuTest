using Business.Dtos.Request;
using DataAccess.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Common.Interfaces
{
    /// <summary>
    /// Interfaz para la gestión de publicaciones (Posts) de forma asíncrona.
    /// Define las reglas de negocio para la creación y validación de contenido.
    /// </summary>
    public interface IPostService
    {
        /// <summary>
        /// Obtiene todos los posts de la base de datos de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación, con una colección de entidades <see cref="Post"/>.</returns>
        Task<ResponseApi<PagedResponse<Post>>> GetAllPagedAsync(int page, int size);

        /// <summary>
        /// Crea un post aplicando validaciones de usuario, truncado de texto
        /// y categorización automática por tipo de forma asíncrona (Punto 3).
        /// </summary>
        /// <param name="entity">DTO con la información para crear el Post.</param>
        /// <returns>Una tarea que representa la operación. El resultado contiene el post creado con las modificaciones aplicadas.</returns>
        Task<ResponseApi<Post>> Create(PostCreate dto);

        /// <summary>
        /// Permite la creación de múltiples posts de forma masiva y asíncrona (Punto 5).
        /// </summary>
        /// <remarks>
        /// Se utiliza Task para permitir que el procesamiento por lotes no bloquee el hilo principal.
        /// </remarks>
        /// <param name="entities">Lista de DTOs de posts a procesar.</param>
        /// <returns>Una tarea que representa la operación de creación masiva.</returns>
        Task<ResponseApi<bool>> CreateBulk(IEnumerable<PostCreate> entities);

        /// <summary>
        /// Obtiene un post por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseApi<PostCreate>> GetByIdAsync(int id);
        /// <summary>
        /// Elimina un post por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseApi<bool>> Delete(int id);
    }
}