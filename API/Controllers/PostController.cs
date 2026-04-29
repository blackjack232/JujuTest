

using Business.Constants;
using Business.Dtos;
using Business.Interfaces;
using DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Publicaciones (Posts).
    /// Maneja la lógica de creación individual y masiva bajo reglas de negocio específicas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _service;

        /// <summary>
        /// Constructor del controlador de Publicaciones.
        /// </summary>
        /// <param name="service">Interfaz del servicio de publicaciones inyectada.</param>
        public PostController(IPostService service) => _service = service;

        /// <summary>
        /// Crea una nueva publicación aplicando validaciones de contenido.
        /// </summary>
        /// <remarks>
        /// Reglas aplicadas:
        /// 1. Valida que el CustomerId exista.
        /// 2. Si el cuerpo supera los 20 caracteres, se trunca a 97 y se añade "...".
        /// 3. Asigna categorías automáticas: 1-Farándula, 2-Política, 3-Futbol.
        /// </remarks>
        /// <param name="entity">DTO con la información de la publicación.</param>
        /// <returns>La entidad Post creada y procesada.</returns>
        /// <response code="200">Retorna el objeto creado exitosamente.</response>
        /// <response code="400">Si el usuario no existe o hay errores de validación.</response>
        [HttpPost]
        public IActionResult Create([FromBody] PostCreateDto entity)
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
        /// Realiza la creación masiva de múltiples publicaciones (Punto 5).
        /// </summary>
        /// <remarks>
        /// Este método itera sobre una lista de DTOs y aplica las reglas de negocio individualmente a cada uno.
        /// </remarks>
        /// <param name="dtos">Lista de objetos PostCreateDto a procesar.</param>
        /// <returns>Mensaje de éxito con la cantidad de registros procesados.</returns>
        /// <response code="200">Si todos los registros se procesaron correctamente.</response>
        /// <response code="400">Si la lista es nula, vacía o ocurre un error en el proceso.</response>
        [HttpPost("Bulk")]
        public IActionResult CreateBulk([FromBody] List<PostCreateDto> dtos)
        {
            if (dtos == null || dtos.Count == 0)
                return BadRequest(AppMessages.ValidationError);

            try
            {
                foreach (var dto in dtos)
                {
                    _service.Create(dto);
                }

                return Ok(string.Format(AppMessages.BulkSuccess, dtos.Count));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}