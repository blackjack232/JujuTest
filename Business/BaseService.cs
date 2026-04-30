using DataAccess.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public class BaseService<TEntity> where TEntity : class, new()
    {

        protected IBaseModel<TEntity> _BaseModel;

        public BaseService(IBaseModel<TEntity> baseModel)
        {
            _BaseModel = baseModel;
        }

        #region Repository

        /// <summary>
        /// Consulta todas las entidades
        /// </summary>
        public virtual IQueryable<TEntity> GetAll()
        {
            return _BaseModel.GetAll;
        }

        /// <summary>
        /// Crea un entidad (Guarda) de forma asíncrona
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> Create(TEntity entity)
        {
            return await _BaseModel.Create(entity);
        }

        /// <summary>
        /// Actualiza la entidad (GUARDA) de forma asíncrona
        /// </summary>
        /// <remarks>
        /// Nota: Se eliminó el parámetro 'out bool changed' por incompatibilidad con async.
        /// </remarks>
        /// <param name="id">Identificador de la entidad</param>
        /// <param name="editedEntity">Entidad editada</param>
        /// <returns></returns>
        public virtual async Task<TEntity> Update(object id, TEntity editedEntity)
        {
            TEntity originalEntity = await _BaseModel.FindById(id);
            if (originalEntity == null) return null;

            return await _BaseModel.Update(editedEntity, originalEntity);
        }

        /// <summary>
        /// Elimina una entidad (Guarda) de forma asíncrona
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<TEntity> Delete(TEntity entity)
        {
            return await _BaseModel.Delete(entity);
        }

        /// <summary>
        /// Guardar cambios de forma asíncrona
        /// </summary>
        public virtual async Task SaveChanges()
        {
            await _BaseModel.SaveChanges();
        }

        #endregion
    }
}