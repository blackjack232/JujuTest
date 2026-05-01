using DataAccess.Context;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    /// <summary>
    /// Repositorio base genérico que proporciona operaciones CRUD fundamentales utilizando Entity Framework Core.
    /// </summary>
    /// <typeparam name="TEntity">El tipo de entidad de dominio con el que operará el repositorio.</typeparam>
    public class BaseRepository<TEntity> : IBaseModel<TEntity> where TEntity : class
    {
        protected readonly JujuTestContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(JujuTestContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        /// <summary>
        /// Expone todas las entidades como una consulta de solo lectura.
        /// </summary>
        public IQueryable<TEntity> GetAll => _dbSet.AsNoTracking();

        /// <summary>
        /// Agrega una nueva entidad y persiste los cambios en la base de datos.
        /// </summary>
        public async Task<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        /// <summary>
        /// Busca una entidad por su clave primaria.
        /// </summary>
        public async Task<TEntity> FindById(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Actualiza los valores de una entidad rastreada y guarda los cambios.
        /// </summary>
        public async Task<TEntity> Update(TEntity editedEntity, TEntity originalEntity)
        {
            _context.Entry(originalEntity).CurrentValues.SetValues(editedEntity);
            await _context.SaveChangesAsync();
            return originalEntity;
        }

        /// <summary>
        /// Elimina físicamente una entidad del contexto y de la base de datos.
        /// </summary>
        public async Task<TEntity> Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Guarda manualmente los cambios pendientes en el contexto.
        /// </summary>
        public async Task SaveChanges(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}