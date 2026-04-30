using Business.Constants;

namespace Business.Helpers
{
    public static class PaginationHelper
    {
        /// <summary>
        /// Normaliza los valores de paginación basándose en las constantes globales.
        /// </summary>
        public static (int page, int size) Validate(int page, int size)
        {
            if (page < 1) page = AppConstants.DefaultPageNumber;

            if (size < 1 || size > AppConstants.MaxPageSize)
                size = AppConstants.DefaultPageSize;

            return (page, size);
        }
    }
}

