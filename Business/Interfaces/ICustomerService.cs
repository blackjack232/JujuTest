using Business.Dtos;
using DataAccess.Data;
using System.Collections.Generic;

namespace Business.Interfaces
{
    /// <summary>
    /// Define el contrato para los servicios de gestión de clientes (Customers).
    /// Contiene la lógica de negocio requerida para la creación, actualización y eliminación.
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// Recupera la colección completa de clientes registrados en el sistema.
        /// </summary>
        /// <returns>Una enumeración de entidades <see cref="Customer"/>.</returns>
        IEnumerable<Customer> GetAll();

        /// <summary>
        /// Realiza el registro de un nuevo cliente tras validar las reglas de negocio.
        /// </summary>
        /// <remarks>
        /// Regla de negocio: Se debe validar que el nombre del cliente sea único antes de persistir (Punto 2).
        /// </remarks>
        /// <param name="customer">DTO con la información necesaria para la creación.</param>
        /// <returns>La entidad <see cref="Customer"/> creada y con ID asignado.</returns>
        /// <exception cref="System.InvalidOperationException">Lanzada si el nombre ya existe.</exception>
        Customer Create(CustomerCreateDto customer);

        /// <summary>
        /// Actualiza los datos de un cliente existente identificado por su ID (Punto 1).
        /// </summary>
        /// <param name="id">Identificador único del cliente a modificar.</param>
        /// <param name="customer">DTO con los nuevos valores del cliente.</param>
        /// <returns>La entidad <see cref="Customer"/> actualizada.</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Lanzada si el ID no corresponde a un cliente existente.</exception>
        Customer Update(int id, CustomerUpdateDto customer);

        /// <summary>
        /// Elimina un cliente del sistema y procesa la limpieza de datos relacionados (Punto 4).
        /// </summary>
        /// <remarks>
        /// Regla de negocio: Al eliminar un cliente, se deben eliminar primero todos sus posts asociados para mantener la integridad.
        /// </remarks>
        /// <param name="id">Identificador único del cliente a eliminar.</param>
        /// <returns><c>true</c> si la operación fue exitosa; <c>false</c> si el cliente no fue encontrado.</returns>
        bool Delete(int id);
    }
}