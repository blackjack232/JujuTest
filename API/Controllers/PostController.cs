using Business.Constants;
using Business.Dtos.Request;
using Business.Helpers;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Publicaciones (Posts).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _service;

        public PostController(IPostService service)
        {
            _service = service;
        }

        /// <summary>
        /// Crea una nueva publicación aplicando validaciones y reglas de negocio.
        /// </summary>
        /// <remarks>
        /// Reglas: Truncado de texto > 20 caracteres y categorización automática por Type.
        /// </remarks>
        /// <param name="entity">DTO del post.</param>
        /// <response code="201">Post creado con éxito.</response>
        /// <response code="400">Error de validación o cliente no existe.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostCreate entity)
        {
            var response = await _service.Create(entity);

            if (!response.Succeeded)
                return BadRequest(response);

            return StatusCode(201, response);
        }

        /// <summary>
        /// Realiza la creación masiva de publicaciones.
        /// </summary>
        /// <param name="dtos">Lista de objetos a procesar.</param>
        /// <response code="200">Proceso masivo finalizado.</response>
        /// <response code="400">Lista vacía o error en el proceso.</response>
        [HttpPost("Bulk")]
        public async Task<IActionResult> CreateBulk([FromBody] List<PostCreate> dtos)
        {
            var response = await _service.CreateBulk(dtos);

            if (!response.Succeeded)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Obtiene todas las publicaciones de forma paginada.
        /// </summary>
        /// <param name="page">Número de página (Por defecto 1).</param>
        /// <param name="size">Registros por página (Por defecto 10).</param>
        /// <returns>Objeto ResponseApi con la lista paginada de posts.</returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = AppConstants.DefaultPageNumber, [FromQuery] int size = AppConstants.DefaultPageSize)
        {
            var (validPage, validSize) = PaginationHelper.Validate(page, size);

            var response = await _service.GetAllPagedAsync(validPage, validSize);

            if (!response.Succeeded)
                return StatusCode(500, response);

            return Ok(response);
        }
    }
}