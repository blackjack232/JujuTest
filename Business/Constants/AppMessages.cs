namespace Business.Constants
{
    public static class AppMessages
    {
        public const string CustomerExists = "El nombre del cliente ya existe en el sistema.";
        public const string CustomerNotFound = "El cliente solicitado no fue encontrado.";
        public const string UserNotFound = "El usuario asociado a la publicación no existe.";
        public const string BulkSuccess = "Se han procesado {0} registros exitosamente.";
        public const string ValidationError = "Se presentaron errores de validación en los datos.";


        public const string PostCreated = "Publicación creada exitosamente con ID: {0}";
        public const string PostBulkStarted = "Iniciando proceso de creación masiva para {0} publicaciones.";
        public const string PostUpdateError = "Error crítico al procesar la publicación.";
        public const string LogGetAllPosts = "Consultando todas las publicaciones desde el servicio.";
        public const string LogErrorBulk = "Error en registro individual durante carga masiva.";

        public const string CustomerCreatedLog = "Cliente '{0}' creado exitosamente con ID: {1}";
        public const string CustomerUpdateLog = "Cliente con ID {0} actualizado correctamente.";
        public const string CustomerDeleteLog = "Cliente con ID {0} y sus datos relacionados han sido eliminados.";
        public const string CustomerDeletePostsLog = "Eliminando {0} publicaciones asociadas al cliente ID {1}.";
        public const string CustomerGetAllLog = "Consultando la lista completa de clientes.";
        public const string CustomerExistsWarning = "Intento de creación fallido: El cliente con nombre '{0}' ya existe.";
        public const string CustomerNotFoundWarning = "Intento de operación fallido: No se encontró el cliente con ID {0}";
        public const string CustomerError = "Error crítico en el servicio de Clientes.";

        // En Business.Constants.AppMessages
        public const string HostStartingLog = "Iniciando el servidor de la API...";
        public const string HostFatalError = "El servidor se detuvo debido a un fallo crítico.";
    }
}
