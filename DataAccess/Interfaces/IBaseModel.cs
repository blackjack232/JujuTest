using System.Linq;

namespace DataAccess.Interfaces
{
    /// <summary>
    /// Interfaz genérica que define el contrato base para los repositorios de datos.
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
        /// Busca una entidad específica mediante su identificador único.
        /// </summary>
        /// <param name="id">Clave primaria de la entidad.</param>
        /// <returns>La instancia de la entidad encontrada o null si no existe.</returns>
        TEntity FindById(object id);

        /// <summary>
        /// Registra una nueva entidad en el contexto de persistencia.
        /// </summary>
        /// <param name="entity">Objeto con los datos a insertar.</param>
        /// <returns>La entidad ya persistida, incluyendo datos generados por la BD (como IDs).</returns>
        TEntity Create(TEntity entity);

        /// <summary>
        /// Actualiza los valores de una entidad comparando su estado actual con el original.
        /// </summary>
        /// <param name="editedEntity">Entidad que contiene las modificaciones realizadas.</param>
        /// <param name="originalEntity">Instancia original recuperada de la base de datos.</param>
        /// <param name="changed">Parámetro de salida que indica si se detectaron cambios reales en las propiedades.</param>
        /// <returns>La entidad actualizada.</returns>
        TEntity Update(TEntity editedEntity, TEntity originalEntity, out bool changed);

        /// <summary>
        /// Remueve una entidad del sistema de persistencia.
        /// </summary>
        /// <param name="entity">Entidad que se desea eliminar.</param>
        /// <returns>La entidad que ha sido marcada para eliminación.</returns>
        TEntity Delete(TEntity entity);

        /// <summary>
        /// Confirma y persiste de manera atómica todos los cambios pendientes en el contexto de datos.
        /// </summary>
        void SaveChanges();
    }
}