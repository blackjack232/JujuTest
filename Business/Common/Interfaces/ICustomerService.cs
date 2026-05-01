using Business.Common.Dtos.Request;
using DataAccess.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Common.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de gestión de clientes.
    /// Define las operaciones de negocio que retornan un envoltorio ResponseApi.
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// Obtiene todos los clientes registrados.
        /// </summary>
        /// <returns>Una respuesta estandarizada con la lista de entidades Customer.</returns>
        Task<ResponseApi<IEnumerable<Customer>>> GetAll();

        /// <summary>
        /// Obtiene una lista paginada de clientes.
        /// </summary>
        /// <param name="page">Número de página actual.</param>
        /// <param name="size">Cantidad de registros por página.</param>
        /// <returns>Una respuesta estandarizada con los datos de paginación.</returns>
        Task<ResponseApi<PagedResponse<Customer>>> GetPagedCostumersAsync(int page, int size);

        /// <summary>
        /// Obtiene un cliente, busca por ID.
        /// </summary>
        Task<ResponseApi<CustomerCreate>> GetByIdAsync(int id);

        /// <summary>
        /// Crea un nuevo cliente en el sistema.
        /// </summary>
        /// <param name="dto">Datos del cliente a crear.</param>
        /// <returns>Una respuesta con el cliente creado o el detalle del error.</returns>
        Task<ResponseApi<Customer>> Create(CustomerCreate dto);

        /// <summary>
        /// Actualiza la información de un cliente existente.
        /// </summary>
        /// <param name="id">Identificador único del cliente.</param>
        /// <param name="dto">Nuevos datos para actualizar.</param>
        /// <returns>Una respuesta con la entidad actualizada.</returns>
        Task<ResponseApi<Customer>> Update(int id, CustomerUpdate dto);

        /// <summary>
        /// Elimina un cliente y sus dependencias asociadas.
        /// </summary>
        /// <param name="id">ID del cliente a eliminar.</param>
        /// <returns>Una respuesta indicando si la operación fue exitosa.</returns>
        Task<ResponseApi<bool>> Delete(int id);
    }
}