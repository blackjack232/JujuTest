using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    /// <summary>
    /// Interfaz genérica que define el contrato base para los repositorios de datos de forma asíncrona.
    /// Sigue el patrón Repository para desacoplar la lógica de negocio del acceso a datos.
    /// </summary>
    /// <typeparam name="TEntity">La entidad de base de datos sobre la que se operará.</typeparam>
    public interface IBaseModel<TEntity> where TEntity : class
    {
        /// <summary>
        /// Obtiene una consulta de tipo <see cref="IQueryable{TEntity}"/> que representa todos los registros.
        /// </summary>
        /// <remarks>
        /// Se utiliza IQueryable para permitir la ejecución diferida (Deferred Execution), 
        /// lo que facilita el filtrado y ordenamiento eficiente en el lado del servidor (SQL).
        /// </remarks>
        IQueryable<TEntity> GetAll { get; }

        /// <summary>
        /// Busca una entidad específica mediante su identificador único de forma asíncrona.
        /// </summary>
        /// <param name="id">Clave primaria de la entidad.</param>
        /// <returns>Una tarea que representa la operación. El resultado contiene la instancia de la entidad encontrada o null si no existe.</returns>
        Task<TEntity> FindById(object id);

        /// <summary>
        /// Registra una nueva entidad en el contexto de persistencia de forma asíncrona.
        /// </summary>
        /// <param name="entity">Objeto con los datos a insertar.</param>
        /// <returns>Una tarea que representa la operación. El resultado contiene la entidad ya persistida, incluyendo datos generados por la BD (como IDs).</returns>
        Task<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza los valores de una entidad comparando su estado actual con el original de forma asíncrona.
        /// </summary>
        /// <remarks>
        /// Nota: El parámetro 'out' no es compatible con métodos async. 
        /// Se recomienda manejar la lógica de 'changed' dentro de la implementación o devolver un objeto compuesto.
        /// </remarks>
        /// <param name="editedEntity">Entidad que contiene las modificaciones realizadas.</param>
        /// <param name="originalEntity">Instancia original recuperada de la base de datos.</param>
        /// <returns>Una tarea que representa la operación. El resultado contiene la entidad actualizada.</returns>
        Task<TEntity> Update(TEntity editedEntity, TEntity originalEntity);

        /// <summary>
        /// Remueve una entidad del sistema de persistencia de forma asíncrona.
        /// </summary>
        /// <param name="entity">Entidad que se desea eliminar.</param>
        /// <returns>Una tarea que representa la operación. El resultado contiene la entidad que ha sido marcada para eliminación.</returns>
        Task<TEntity> Delete(TEntity entity);

        /// <summary>
        /// Confirma y persiste de manera atómica todos los cambios pendientes en el contexto de datos de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación de guardado.</returns>
        Task SaveChanges(CancellationToken cancellationToken = default);
    }
}