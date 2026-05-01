using DataAccess.Data;
using DataAccess.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    /// <summary>
    /// Implementación del repositorio para la entidad Publicación (Post).
    /// Esta clase gestiona las operaciones de persistencia específicas para las publicaciones,
    /// extendiendo las funcionalidades básicas del repositorio genérico.
    /// </summary>
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PostRepository"/>.
        /// </summary>
        /// <param name="context">Contexto de base de datos <see cref="JujuTestContext"/> que será compartido por la unidad de trabajo.</param>
        public PostRepository(JujuTestContext context) : base(context)
        {
        }

        /// <summary>
        /// Registra una colección de publicaciones de forma masiva en el contexto de datos.
        /// Este método es ideal para procesos de creación por lote (Bulk Creation).
        /// </summary>
        /// <param name="entities">Colección de entidades <see cref="Post"/> que se desean agregar.</param>
        /// <returns>Una tarea que representa la operación de agregado masivo.</returns>
        public async Task AddRangeAsync(IEnumerable<Post> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina una publicación de la base de datos por su identificador único.
        /// </summary>
        /// <param name="id">ID de la publicación a eliminar.</param>
        /// <returns>True si la operación se completó con éxito.</returns>
        public async Task DeleteAsync(Post entity)
        {

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();

        }

        /// <summary>
        /// Recupera una publicación específica utilizando su identificador único.
        /// </summary>
        /// <param name="id">El identificador primario de la publicación.</param>
        /// <returns>La entidad <see cref="Post"/> encontrada o null si no existe una coincidencia.</returns>
        public async Task<Post> GetByIdAsync(int id)
        {
            return await FindById(id);
        }

        /// <summary>
        /// Prepara la actualización de una publicación existente.
        /// Localiza la entidad original para que el rastreador de cambios (ChangeTracker) 
        /// pueda procesar la actualización de valores correctamente.
        /// </summary>
        /// <param name="entity">Entidad con los nuevos datos que se desean persistir.</param>
        /// <returns>Una tarea que representa la operación de preparación de actualización.</returns>
        public async Task UpdateAsync(Post entity)
        {
            var original = await FindById(entity.PostId);

            if (original != null)
            {
                await Update(entity, original);
            }
        }
    }
}