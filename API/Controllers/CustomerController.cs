using Business.Dtos.Request;
using Business.Helpers;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Clientes (Customers).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene una lista paginada de clientes validando parámetros con AppConstants.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var (validPage, validSize) = PaginationHelper.Validate(page, size);

            var response = await _service.GetPagedCostumersAsync(validPage, validSize);
            return Ok(response);
        }

        /// <summary>
        /// Crea un nuevo registro de cliente.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerCreate entity)
        {
            var response = await _service.Create(entity);

            if (!response.Succeeded)
                return BadRequest(response);

            return StatusCode(201, response);
        }

        /// <summary>
        /// Actualiza la información de un cliente existente.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdate entity)
        {
            var response = await _service.Update(id, entity);

            if (!response.Succeeded)
                return NotFound(response);

            return Ok(response);
        }

        /// <summary>
        /// Elimina un cliente y sus registros relacionados.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _service.Delete(id);

            if (!response.Succeeded)
                return NotFound(response);

            return Ok(response);
        }
    }
}