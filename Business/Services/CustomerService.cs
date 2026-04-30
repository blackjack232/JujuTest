using Business.Constants;
using Business.Dtos.Request;
using Business.Interfaces;
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
    /// Proporciona la lógica de negocio para la gestión de clientes (Customers).
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly IBaseModel<Customer> _customerRepo;
        private readonly IBaseModel<Post> _postRepo;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            IBaseModel<Customer> customerRepo,
            IBaseModel<Post> postRepo,
            ILogger<CustomerService> logger)
        {
            _customerRepo = customerRepo;
            _postRepo = postRepo;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los clientes registrados en el sistema de forma asíncrona.
        /// </summary>
        public async Task<ResponseApi<IEnumerable<Customer>>> GetAll()
        {
            _logger.LogInformation(AppMessages.CustomerGetAllLog);
            var data = await _customerRepo.GetAll.ToListAsync();
            return new ResponseApi<IEnumerable<Customer>>(data, AppMessages.CustomerListSuccess);
        }

        /// <summary>
        /// Obtiene una respuesta paginada de clientes utilizando constantes globales.
        /// </summary>
        public async Task<ResponseApi<PagedResponse<Customer>>> GetPagedCostumersAsync(int page, int size)
        {
            var query = _customerRepo.GetAll;
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
        /// Crea un nuevo cliente validando unicidad mediante AppMessages.
        /// </summary>
        public async Task<ResponseApi<Customer>> Create(CustomerCreate dto)
        {
            try
            {
                var normalizedName = dto.Name?.Trim();

                if (await _customerRepo.GetAll.AnyAsync(c => c.Name == normalizedName))
                {
                    _logger.LogWarning(AppMessages.CustomerExistsWarning, dto.Name);
                    return new ResponseApi<Customer>(AppMessages.CustomerExists);
                }

                var entity = new Customer { Name = dto.Name };
                var result = await _customerRepo.Create(entity);

                _logger.LogInformation(AppMessages.CustomerCreatedLog, result.Name, result.CustomerId);
                return new ResponseApi<Customer>(result, AppMessages.CustomerCreatedSuccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppMessages.CustomerError);
                return new ResponseApi<Customer>(AppMessages.InternalServerError);
            }
        }

        /// <summary>
        /// Actualiza un cliente existente manejando mensajes de error centralizados.
        /// </summary>
        public async Task<ResponseApi<Customer>> Update(int id, CustomerUpdate dto)
        {
            var original = await _customerRepo.FindById(id);

            if (original == null)
            {
                _logger.LogWarning(AppMessages.CustomerNotFoundWarning, id);
                return new ResponseApi<Customer>(AppMessages.CustomerNotFound);
            }

            original.Name = dto.Name;
            var result = await _customerRepo.Update(original, original);

            _logger.LogInformation(AppMessages.CustomerUpdateLog, id);
            return new ResponseApi<Customer>(result, AppMessages.CustomerUpdatedSuccess);
        }

        /// <summary>
        /// Elimina un cliente y limpia publicaciones asociadas usando AppMessages para el log.
        /// </summary>
        public async Task<ResponseApi<bool>> Delete(int id)
        {
            var customer = await _customerRepo.FindById(id);
            if (customer == null)
            {
                _logger.LogWarning(AppMessages.CustomerNotFoundWarning, id);
                return new ResponseApi<bool>(AppMessages.CustomerNotFound);
            }

            var associatedPosts = await _postRepo.GetAll
                .Where(p => p.CustomerId == id)
                .ToListAsync();

            foreach (var post in associatedPosts)
            {
                await _postRepo.Delete(post);
            }

            await _customerRepo.Delete(customer);
            _logger.LogInformation(AppMessages.CustomerDeleteLog, id);

            return new ResponseApi<bool>(true, AppMessages.CustomerDeletedSuccess);
        }
    }
}