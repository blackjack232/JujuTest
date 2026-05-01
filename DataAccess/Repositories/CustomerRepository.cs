using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    /// <summary>
    /// Implementación del repositorio para la entidad Cliente.
    /// Proporciona acceso a datos especializado y aprovecha la funcionalidad base.
    /// </summary>
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="CustomerRepository"/>.
        /// </summary>
        /// <param name="context">Contexto de base de datos inyectado.</param>
        public CustomerRepository(JujuTestContext context) : base(context)
        {
        }

        /// <summary>
        /// Registra un nuevo cliente en el contexto de datos. Este método es esencial para la creación de nuevos registros de clientes en el sistema. Al agregar un cliente, se prepara la entidad para su inserción en la base de datos, pero la operación no se ejecuta hasta que se llame a SaveChangesAsync en el contexto. Esto permite realizar múltiples operaciones de creación, actualización o eliminación en una sola transacción, optimizando el rendimiento y garantizando la integridad de los datos.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task AddAsync(Customer entity)
        {
            await Create(entity);
            
        }

        /// <summary>
        /// Obtiene todos los clientes registrados de forma asíncrona. Este método es esencial para operaciones de lectura masiva, como mostrar la lista completa de clientes en una interfaz de usuario o para procesos de exportación de datos.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Recupera un cliente específico utilizando su identificador único. Este método es fundamental para operaciones de lectura individual, como mostrar detalles de un cliente o validar su existencia antes de realizar actualizaciones o eliminaciones.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Customer> GetByIdAsync(int id)
        {
            return await FindById(id);
        }

        /// <summary>
        /// Obtiene un cliente por su ID incluyendo su lista de publicaciones relacionadas (Posts).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Customer> GetByIdWithPostsAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        /// <summary>
        /// Busca un cliente por su nombre exacto. Este método es útil para validar la unicidad del nombre o para recuperar un cliente específico basado en su nombre.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Customer> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
        }

        /// <summary>
        /// Prepara la eliminación de un cliente del sistema. Este método marca la entidad para eliminación, pero no ejecuta la operación en la base de datos hasta que se llame a SaveChangesAsync en el contexto.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task RemoveAsync(Customer entity)
        {
            await Delete(entity);
        }

        /// <summary>
        /// Elimina un rango o colección de publicaciones de forma masiva.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task RemoveRangeAsync(IEnumerable<Post> entities)
        {
            _context.Set<Post>().RemoveRange(entities);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Prepara la actualización de un cliente existente. Localiza la entidad original para que el rastreador de cambios (ChangeTracker)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Customer entity)
        {

            var original = await FindById(entity.CustomerId);
            if (original != null)
            {
                await Update(entity, original);
            }
        }
        public async Task<bool> ExistsAnotherNameAsync(string name, int currentId)
        {
            return await _dbSet.AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.CustomerId != currentId);
        }
        /// <summary>
        /// Elimina un cliente junto con todas sus publicaciones asociadas.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task RemoveWithPostsAsync(int customerId)
        {
            var associatedPosts = await _context.Set<Post>()
                .Where(p => p.CustomerId == customerId)
                .ToListAsync();
            if (associatedPosts.Any())
            {
                _context.Set<Post>().RemoveRange(associatedPosts);
            }

            var customer = await FindById(customerId);
            if (customer != null)
            {
                await Delete(customer);
            }
        }

    }
}