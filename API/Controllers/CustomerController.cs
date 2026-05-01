using Business.Common.Constants;
using Business.Common.Helpers;
using Business.Common.Interfaces;
using Business.Dtos.Request;
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
        /// Obtiene una lista paginada de clientes, aplicando validaciones de paginación y retornando una respuesta estandarizada con los datos de paginación.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var (validPage, validSize) = PaginationHelper.Validate(page, size);

            var response = await _service.GetPagedCostumersAsync(validPage, validSize);
            return Ok(response);
        }
        /// <summary>
        /// Obtiene un cliente por su ID, validando la existencia del cliente antes de retornar la información.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);

            if (!response.Succeeded)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Crea un nuevo cliente aplicando validaciones y reglas de negocio, como la longitud máxima del nombre.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerCreate entity)
        {
            var response = await _service.Create(entity);

            if (!response.Succeeded)
                return BadRequest(response);

            return StatusCode(AppConstants.StatusCreated, response);
        }

        /// <summary>
        /// Actualiza la información de un cliente existente, validando la existencia del cliente antes de la actualización.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdate entity)
        {
            var response = await _service.Update(id, entity);

            if (!response.Succeeded)
                return NotFound(response);

            return Ok(response);
        }

        /// <summary>
        /// Elimina un cliente y sus dependencias asociadas, validando la existencia del cliente antes de la eliminación.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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