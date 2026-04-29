//using Business;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Linq;
//using CustomerEntity = DataAccess.Data.Customer;

//namespace API.Controllers.Customer
//{
//    [Route("[controller]")]
//    public class CustomerController : ControllerBase
//    {
//        private BaseService<CustomerEntity> CustomerService;
//        public CustomerController(BaseService<CustomerEntity> customerService)
//        {
//            CustomerService = customerService;
//        }


//        [HttpGet()]
//        public IQueryable<CustomerEntity> GetAll()
//        {
//            return CustomerService.GetAll();
//        }


//        [HttpPost()]
//        public CustomerEntity Create([FromBodyAttribute] CustomerEntity entity)
//        {
//            return CreateCustomer(entity);
//        }

//        private CustomerEntity CreateCustomer(CustomerEntity entity)
//        {
//            throw new Exception("");
//            return CustomerService.Create(entity);
//        }

//        //[HttpPut()]
//        //public CustomerEntity Update(CustomerEntity entity)
//        //{
//        //    return CustomerService.Update(entity.CustomerId, entity, out bool changed);
//        //}
//        [HttpPut("{id}")]
//        public IActionResult Update(int id, [FromBody] CustomerEntity entity)
//        {
//            if (entity == null || id != entity.CustomerId) return BadRequest();

//            var updated = CustomerService.Update(id, entity, out bool changed);
//            if (updated == null) return NotFound();

//            return Ok(updated);
//        }

//        [HttpDelete()]
//        public CustomerEntity Delete([FromBodyAttribute] CustomerEntity entity)
//        {
//            return CustomerService.Delete(entity);
//        }
//    }
//}


using Business.Dtos;
using Business.Interfaces;
using DataAccess.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Clientes (Customers).
    /// Proporciona endpoints para operaciones CRUD utilizando DTOs para transferencia de datos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        /// <summary>
        /// Constructor del controlador.
        /// </summary>
        /// <param name="service">Instancia del servicio de negocio inyectada.</param>
        public CustomerController(ICustomerService service) => _service = service;

        /// <summary>
        /// Obtiene la lista completa de clientes registrados.
        /// </summary>
        /// <returns>Una colección de entidades de tipo Customer.</returns>
        /// <response code="200">Retorna la lista de clientes.</response>
        [HttpGet]
        public IActionResult Get() => Ok(_service.GetAll());

        /// <summary>
        /// Crea un nuevo registro de cliente en el sistema.
        /// </summary>
        /// <remarks>
        /// El nombre del cliente debe ser único; de lo contrario, el servicio lanzará una excepción.
        /// </remarks>
        /// <param name="entity">Objeto DTO con los datos necesarios para la creación.</param>
        /// <returns>El cliente recién creado con su ID asignado.</returns>
        /// <response code="200">Si el cliente se creó exitosamente.</response>
        /// <response code="400">Si hay un error de validación o el nombre ya existe.</response>
        [HttpPost]
        public IActionResult Create([FromBody] CustomerCreateDto entity)
        {
            try
            {
                return Ok(_service.Create(entity));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza la información de un cliente existente.
        /// </summary>
        /// <param name="id">Identificador único del cliente a modificar.</param>
        /// <param name="entity">Objeto DTO con los nuevos datos del cliente.</param>
        /// <returns>La entidad del cliente actualizada.</returns>
        /// <response code="200">Si la actualización fue exitosa.</response>
        /// <response code="404">Si el cliente con el ID proporcionado no existe.</response>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CustomerUpdateDto entity)
        {
            var result = _service.Update(id, entity);
            return result == null ? (IActionResult)NotFound() : Ok(result);
        }

        /// <summary>
        /// Elimina un cliente y todos sus registros relacionados (Posts).
        /// </summary>
        /// <param name="id">Identificador único del cliente a eliminar.</param>
        /// <returns>Sin contenido si se eliminó correctamente.</returns>
        /// <response code="204">Operación exitosa, el registro fue eliminado.</response>
        /// <response code="404">Si el cliente no fue encontrado.</response>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return _service.Delete(id) ? (IActionResult)NoContent() : NotFound();
        }
    }
}
