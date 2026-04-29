using Business.Dtos;
using DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Interfaces
{
    /// <summary>
    /// Interfaz para la gestión de publicaciones (Posts).
    /// Define las reglas de negocio para la creación y validación de contenido.
    /// </summary>
    public interface IPostService
    {
        /// <summary>
        /// Obtiene todos los posts de la base de datos.
        /// </summary>
        /// <returns>IQueryable para permitir filtrado eficiente.</returns>
        IQueryable<Post> GetAll();

        /// <summary>
        /// Crea un post aplicando validaciones de usuario, truncado de texto
        /// y categorización automática por tipo (Punto 3).
        /// </summary>
        /// <param name="entity">Entidad Post a crear.</param>
        /// <returns>El post creado con las modificaciones aplicadas.</returns>
        Post Create(PostCreateDto entity);

        /// <summary>
        /// Permite la creación de múltiples posts en una sola transacción (Punto 5).
        /// </summary>
        /// <param name="entities">Lista de posts a procesar.</param>
        void CreateBulk(List<PostCreateDto> entities);
    }
}
