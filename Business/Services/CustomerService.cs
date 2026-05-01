using Business.Common.Constants;
using Business.Common.Interfaces;
using Business.Dtos.Request;
using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    /// <summary>
    /// Proporciona servicios de lógica de negocio para la gestión de clientes (Customers).
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerService> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="CustomerService"/>.
        /// </summary>
        /// <param name="customerRepository">Repositorio para el acceso a datos de clientes.</param>
        /// <param name="postRepository">Repositorio para el acceso a datos de publicaciones.</param>
        /// <param name="logger">Servicio de registro de eventos (logs).</param>
        public CustomerService(
            ICustomerRepository customerRepository,
            IPostRepository postRepository,
            ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene la lista completa de clientes registrados de forma asíncrona.
        /// </summary>
        /// <returns>Una respuesta de API que contiene la colección de clientes.</returns>
        public async Task<ResponseApi<IEnumerable<Customer>>> GetAll()
        {
            _logger.LogInformation(AppMessages.CustomerGetAllLog);
            var data = await _customerRepository.GetAllAsync();
            return new ResponseApi<IEnumerable<Customer>>(data, AppMessages.CustomerListSuccess);
        }

        /// <summary>
        /// Obtiene una lista paginada de clientes según los parámetros especificados.
        /// </summary>
        /// <param name="page">Número de la página a consultar.</param>
        /// <param name="size">Cantidad de registros por página.</param>
        /// <returns>Una respuesta de API con los datos paginados y metadatos de navegación.</returns>
        public async Task<ResponseApi<PagedResponse<Customer>>> GetPagedCostumersAsync(int page, int size)
        {
            var query = _customerRepository.GetAll;
            int totalRecords = await query.CountAsync();

            var data = await query
                .OrderBy(c => c.CustomerId)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            var pagedData = new PagedResponse<Customer>
            {
                Data = data,
                PageNumber = page,
                PageSize = size,
                TotalRecords = totalRecords
            };

            return new ResponseApi<PagedResponse<Customer>>(pagedData, AppMessages.PaginationSuccess);
        }
        /// <summary>
        /// Recupera la información de un cliente específico mediante su identificador único.
        /// </summary>
        /// <remarks>
        /// Este método busca en el repositorio de clientes. Si el registro no existe, 
        /// devuelve una respuesta fallida y registra una advertencia en el log.
        /// </remarks>
        /// <param name="id">Identificador único del cliente a consultar.</param>
        /// <returns>
        /// Un objeto <see cref="ResponseApi{CustomerCreate}"/> que contiene el DTO del cliente si se encuentra, 
        /// o un mensaje de error en caso de no existir o fallar la operación.
        /// </returns>
        public async Task<ResponseApi<CustomerCreate>> GetByIdAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);

                if (customer == null)
                {
                    _logger.LogWarning(AppMessages.CustomerNotFoundWarning, id);
                    return new ResponseApi<CustomerCreate>(string.Format(AppMessages.CustomerNotFound, id));
                }

                var response = new CustomerCreate
                {
                    Name = customer.Name
                };
                return new ResponseApi<CustomerCreate>(response, AppMessages.PaginationSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppMessages.CustomerError);
                return new ResponseApi<CustomerCreate>(AppMessages.InternalServerError);
            }
        }
        /// <summary>
        /// Crea un nuevo cliente en el sistema validando que el nombre sea único.
        /// </summary>
        /// <param name="dto">Objeto con la información del cliente a crear.</param>
        /// <returns>Una respuesta de API con el cliente creado o un mensaje de error si el nombre ya existe.</returns>
        public async Task<ResponseApi<Customer>> Create(CustomerCreate dto)
        {
            try
            {
                var existing = await _customerRepository.GetByNameAsync(dto.Name);
                if (existing != null)
                {
                    _logger.LogWarning(AppMessages.CustomerExistsWarning, dto.Name);
                    return new ResponseApi<Customer>(AppMessages.CustomerExists);
                }

                var entity = new Customer { Name = dto.Name };
                await _customerRepository.AddAsync(entity);


                _logger.LogInformation(AppMessages.CustomerCreatedLog, entity.Name, entity.CustomerId);
                return new ResponseApi<Customer>(entity, AppMessages.CustomerCreatedSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppMessages.CustomerError);
                return new ResponseApi<Customer>(AppMessages.InternalServerError);
            }
        }

        /// <summary>
        /// Actualiza la información de un cliente existente.
        /// </summary>
        /// <param name="id">Identificador único del cliente.</param>
        /// <param name="dto">Objeto con la información actualizada.</param>
        /// <returns>La entidad actualizada o un error si no se encuentra el cliente o el nombre ya está en uso.</returns>
        public async Task<ResponseApi<Customer>> Update(int id, CustomerUpdate dto)
        {
            try
            {
                var original = await _customerRepository.GetByIdAsync(id);
                if (original == null)
                {
                    _logger.LogWarning(AppMessages.CustomerNotFoundWarning, id);
                    return new ResponseApi<Customer>(AppMessages.CustomerNotFound);
                }

                var nameExists = await _customerRepository.ExistsAnotherNameAsync(dto.Name, id);
                if (nameExists)
                {
                    _logger.LogWarning(AppMessages.CustomerDuplicateNameWarning, dto.Name);
                    return new ResponseApi<Customer>(AppMessages.CustomerExists);
                }

                original.Name = dto.Name;
                await _customerRepository.UpdateAsync(original);


                _logger.LogInformation(AppMessages.CustomerUpdateLog, id);
                return new ResponseApi<Customer>(original, AppMessages.CustomerUpdatedSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppMessages.CustomerError);
                return new ResponseApi<Customer>(AppMessages.InternalServerError);
            }
        }

        /// <summary>
        /// Elimina un cliente y todas sus publicaciones asociadas de forma atómica.
        /// </summary>
        /// <param name="id">Identificador único del cliente a eliminar.</param>
        /// <returns>Verdadero si la eliminación fue exitosa, o un error si el cliente no existe.</returns>
        public async Task<ResponseApi<bool>> Delete(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    _logger.LogWarning(AppMessages.CustomerNotFoundWarning, id);
                    return new ResponseApi<bool>(AppMessages.CustomerNotFound);
                }

                await _customerRepository.RemoveWithPostsAsync(id);

                _logger.LogInformation(AppMessages.CustomerDeleteLog, id);
                return new ResponseApi<bool>(true, AppMessages.CustomerDeletedSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppMessages.CustomerError);
                return new ResponseApi<bool>(AppMessages.InternalServerError);
            }
        }

    }
}