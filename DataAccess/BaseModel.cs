using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DataAccess
{
    /// <summary>
    /// Implementación genérica del patrón Repositorio (BaseModel).
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
        /// Permite aplicar filtros adicionales (Where, OrderBy) antes de ejecutar la consulta en la BD.
        /// </summary>
        public virtual IQueryable<TEntity> GetAll => _dbSet;

        /// <summary>
        /// Busca una entidad específica por su clave primaria.
        /// </summary>
        /// <param name="id">Identificador único (Primary Key) de la entidad.</param>
        /// <returns>La entidad encontrada o null si no existe.</returns>
        public virtual TEntity FindById(object id) => _dbSet.Find(id);

        /// <summary>
        /// Agrega una nueva entidad al contexto y persiste los cambios inmediatamente.
        /// </summary>
        /// <param name="entity">Instancia de la entidad a crear.</param>
        /// <returns>La entidad persistida con sus campos actualizados (como IDs autoincrementales).</returns>
        public virtual TEntity Create(TEntity entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Actualiza los valores de una entidad existente comparándola con su estado original.
        /// </summary>
        /// <remarks>
        /// Utiliza <c>CurrentValues.SetValues</c> para realizar una actualización eficiente de las propiedades.
        /// </remarks>
        /// <param name="editedEntity">Entidad que contiene los nuevos cambios.</param>
        /// <param name="originalEntity">Entidad rastreada por el contexto que será actualizada.</param>
        /// <param name="changed">Parámetro de salida que indica si hubo cambios reales en los datos.</param>
        /// <returns>La entidad actualizada.</returns>
        public virtual TEntity Update(TEntity editedEntity, TEntity originalEntity, out bool changed)
        {
            // Mapea los valores de la entidad editada a la original
            _context.Entry(originalEntity).CurrentValues.SetValues(editedEntity);

            // Verifica si el estado cambió a Modified tras el set de valores
            changed = _context.Entry(originalEntity).State == EntityState.Modified;

            _context.SaveChanges();
            return originalEntity;
        }

        /// <summary>
        /// Elimina una entidad del contexto y persiste el cambio en la base de datos.
        /// </summary>
        /// <param name="entity">Entidad a eliminar.</param>
        /// <returns>La instancia de la entidad que fue removida.</returns>
        public virtual TEntity Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Ejecuta manualmente la persistencia de todos los cambios rastreados en el contexto.
        /// </summary>
        public virtual void SaveChanges() => _context.SaveChanges();
    }
}