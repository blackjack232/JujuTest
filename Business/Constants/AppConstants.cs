namespace Business.Constants
{
    public static class AppConstants
    {
        public const int MaxBodyLength = 97;
        public const int MinBodyThreshold = 20;
        public const string CategoryFarandula = "Farándula";
        public const string CategoryPolitica = "Política";
        public const string CategoryFutbol = "Futbol";

        public const int TypeFarandula = 1;
        public const int TypePolitica = 2;
        public const int TypeFutbol = 3;

        // Paginación por defecto
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;

        // Validaciones de negocio
        public const int CustomerNameMaxLength = 100;
    }
}
