using DataAccess.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    /// <summary>
    /// Interfaz específica para las operaciones de datos de la entidad Cliente.
    /// Hereda las funcionalidades base de IBaseModel.
    /// </summary>
    public interface ICustomerRepository : IBaseModel<Customer>
    {
        /// <summary>
        /// Obtiene todos los clientes registrados de forma asíncrona.
        /// </summary>
        /// <returns>Una lista de todos los clientes.</returns>
        Task<IEnumerable<Customer>> GetAllAsync();

        /// <summary>
        /// Obtiene un cliente por su identificador único.
        /// </summary>
        /// <param name="id">ID del cliente.</param>
        /// <returns>El cliente encontrado o null.</returns>
        Task<Customer> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un cliente por su ID incluyendo su lista de publicaciones relacionadas (Posts).
        /// </summary>
        /// <param name="id">ID del cliente.</param>
        /// <returns>El cliente con sus publicaciones cargadas.</returns>
        Task<Customer> GetByIdWithPostsAsync(int id);

        /// <summary>
        /// Busca un cliente por su nombre exacto.
        /// </summary>
        /// <param name="name">Nombre del cliente a buscar.</param>
        /// <returns>El cliente que coincida con el nombre.</returns>
        Task<Customer> GetByNameAsync(string name);

        /// <summary>
        /// Registra un nuevo cliente en el contexto de datos.
        /// </summary>
        /// <param name="entity">Entidad cliente a agregar.</param>
        Task AddAsync(Customer entity);

        /// <summary>
        /// Prepara la actualización de un cliente existente.
        /// </summary>
        /// <param name="entity">Entidad con los cambios aplicados.</param>
        Task UpdateAsync(Customer entity);

        /// <summary>
        /// Prepara la eliminación de un cliente del sistema.
        /// </summary>
        /// <param name="entity">Entidad a eliminar.</param>
        Task RemoveAsync(Customer entity);

        /// <summary>
        /// Elimina un rango o colección de publicaciones de forma masiva.
        /// </summary>
        /// <param name="entities">Colección de publicaciones a eliminar.</param>
        Task RemoveWithPostsAsync(int customerId);
        Task<bool> ExistsAnotherNameAsync(string name, int currentId);
    }
}