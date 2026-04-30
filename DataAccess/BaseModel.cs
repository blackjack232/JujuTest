using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess
{
    /// <summary>
    /// Implementación genérica del patrón Repositorio (BaseModel) de forma asíncrona.
    /// Proporciona operaciones CRUD base para cualquier entidad del sistema utilizando Entity Framework Core.
    /// </summary>
    /// <typeparam name="TEntity">Tipo de la entidad que debe ser una clase y permitir instanciación.</typeparam>
    public class BaseModel<TEntity> : IBaseModel<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// Contexto de base de datos principal.
        /// </summary>
        protected readonly JujuTestContext _context;

        /// <summary>
        /// Set de datos específico para la entidad <typeparamref name="TEntity"/>.
        /// </summary>
        protected readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="BaseModel{TEntity}"/>.
        /// </summary>
        /// <param name="context">Contexto de base de datos inyectado mediante DI.</param>
        public BaseModel(JujuTestContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        /// <summary>
        /// Expone la tabla completa como una consulta de tipo <see cref="IQueryable{TEntity}"/>.
        /// Permite aplicar filtros adicionales (Where, OrderBy) antes de ejecutar la consulta en la BD de forma diferida.
        /// </summary>
        /// <remarks>
        /// Se utiliza AsNoTracking() por defecto para mejorar significativamente el rendimiento en lecturas masivas.
        /// </remarks>
        public virtual IQueryable<TEntity> GetAll => _dbSet.AsNoTracking();

        /// <summary>
        /// Busca una entidad específica por su clave primaria de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único (Primary Key) de la entidad.</param>
        /// <returns>Una tarea que representa la operación. El resultado contiene la entidad encontrada o null si no existe.</returns>
        public virtual async Task<TEntity> FindById(object id) => await _dbSet.FindAsync(id);

        /// <summary>
        /// Agrega una nueva entidad al contexto y persiste los cambios de forma asíncrona.
        /// </summary>
        /// <param name="entity">Instancia de la entidad a crear.</param>
        /// <returns>Una tarea que representa la operación. El resultado contiene la entidad persistida con sus campos actualizados (como IDs autoincrementales).</returns>
        public virtual async Task<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        /// <summary>
        /// Actualiza los valores de una entidad existente comparándola con su estado original de forma asíncrona.
        /// </summary>
        /// <remarks>
        /// Utiliza <c>CurrentValues.SetValues</c> para realizar una actualización eficiente. 
        /// Nota: Se eliminó el parámetro 'out' por incompatibilidad con async.
        /// </remarks>
        /// <param name="editedEntity">Entidad que contiene los nuevos cambios.</param>
        /// <param name="originalEntity">Entidad rastreada por el contexto que será actualizada.</param>
        /// <returns>Una tarea que representa la operación. El resultado contiene la entidad actualizada.</returns>
        public virtual async Task<TEntity> Update(TEntity editedEntity, TEntity originalEntity)
        {
            // Mapea los valores de la entidad editada a la original
            _context.Entry(originalEntity).CurrentValues.SetValues(editedEntity);

            await _context.SaveChangesAsync();
            return originalEntity;
        }

        /// <summary>
        /// Elimina una entidad del contexto y persiste el cambio en la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="entity">Entidad a eliminar.</param>
        /// <returns>Una tarea que representa la operación. El resultado contiene la instancia de la entidad que fue removida.</returns>
        public virtual async Task<TEntity> Delete(TEntity entity)
        {
            // Verificación de rastreo para evitar errores en eliminaciones de entidades desconectadas
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Ejecuta manualmente la persistencia de todos los cambios rastreados en el contexto de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación de guardado.</returns>
        public virtual async Task SaveChanges(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);
    }
}