using Business.Constants;
using Business.Dtos;
using Business.Interfaces;
using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IEnumerable<Customer> GetAll()
        {
            _logger.LogInformation(AppMessages.CustomerGetAllLog);
            return _customerRepo.GetAll.ToList();
        }

        public Customer Create(CustomerCreateDto dto)
        {
            try
            {
                if (_customerRepo.GetAll.Any(c => c.Name.ToLower().Trim() == dto.Name.ToLower().Trim()))
                {
                    _logger.LogWarning(AppMessages.CustomerExistsWarning, dto.Name);
                    throw new InvalidOperationException(AppMessages.CustomerExists);
                }

                var entity = new Customer { Name = dto.Name };
                var result = _customerRepo.Create(entity);

                _logger.LogInformation(AppMessages.CustomerCreatedLog, result.Name, result.CustomerId);
                return result;
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppMessages.CustomerError);
                throw;
            }
        }

        public Customer Update(int id, CustomerUpdateDto dto)
        {
            try
            {
                var original = _customerRepo.FindById(id);

                if (original == null)
                {
                    _logger.LogWarning(AppMessages.CustomerNotFoundWarning, id);
                    throw new KeyNotFoundException(AppMessages.CustomerNotFound);
                }

                original.Name = dto.Name;
                var result = _customerRepo.Update(original, original, out _);

                _logger.LogInformation(AppMessages.CustomerUpdateLog, id);
                return result;
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException))
            {
                _logger.LogError(ex, AppMessages.CustomerError);
                throw;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var customer = _customerRepo.FindById(id);
                if (customer == null)
                {
                    _logger.LogWarning(AppMessages.CustomerNotFoundWarning, id);
                    return false;
                }

                var associatedPosts = _postRepo.GetAll.Where(p => p.CustomerId == id).ToList();

                if (associatedPosts.Any())
                {
                    _logger.LogInformation(AppMessages.CustomerDeletePostsLog, associatedPosts.Count, id);
                    foreach (var post in associatedPosts)
                    {
                        _postRepo.Delete(post);
                    }
                }

                _customerRepo.Delete(customer);
                _logger.LogInformation(AppMessages.CustomerDeleteLog, id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, AppMessages.CustomerError);
                throw;
            }
        }
    }
}