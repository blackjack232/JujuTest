using DataAccess.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    /// <summary>
    /// Interfaz específica para las operaciones de datos de la entidad Publicación (Post).
    /// Hereda las funcionalidades base de IBaseModel.
    /// </summary>
    public interface IPostRepository : IBaseModel<Post>
    {
        /// <summary>
        /// Obtiene una publicación específica por su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">ID de la publicación.</param>
        /// <returns>La publicación encontrada o null.</returns>
        Task<Post> GetByIdAsync(int id);

        /// <summary>
        /// Prepara la actualización de una publicación existente en el contexto de datos.
        /// </summary>
        /// <param name="entity">Entidad con los datos actualizados.</param>
        Task UpdateAsync(Post entity);

        /// <summary>
        /// Registra una colección de publicaciones de forma masiva en la base de datos.
        /// Útil para operaciones de inserción por lote (Bulk Insert).
        /// </summary>
        /// <param name="entities">Lista de publicaciones a registrar.</param>
        Task AddRangeAsync(IEnumerable<Post> entities);

        /// <summary>
        /// Elimina un Post específico por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Post id);
        
    }
}