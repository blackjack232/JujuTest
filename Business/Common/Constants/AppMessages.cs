namespace Business.Common.Constants
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

        // Mensajes de Validación para Clientes
        public const string CustomerNameRequired = "El nombre del cliente es obligatorio.";
        public const string CustomerNameMaxLength = "El nombre del cliente no puede exceder los 100 caracteres.";

        // Mensajes de Validación para Posts
        public const string PostCustomerRequired = "Debe seleccionar un cliente válido para la publicación.";
        public const string PostTitleRequired = "El título de la publicación es obligatorio.";
        public const string PostTitleMaxLength = "El título no puede superar los 100 caracteres.";
        public const string PostBodyRequired = "El contenido de la publicación (Body) no puede estar vacío.";
        public const string PostTypeRange = "El tipo de post debe ser: 1 (Farándula), 2 (Política) o 3 (Futbol).";
        public const string PostTypeInvalidWarning = "Omitiendo Post: Tipo {0} no válido.";


        // Mensajes de Éxito (Para ResponseApi.Message)
        public const string CustomerListSuccess = "Lista de clientes recuperada exitosamente.";
        public const string CustomerCreatedSuccess = "El cliente ha sido registrado correctamente.";
        public const string CustomerUpdatedSuccess = "Los datos del cliente se actualizaron con éxito.";
        public const string CustomerDeletedSuccess = "Cliente y sus publicaciones asociadas han sido eliminados.";
        public const string PaginationSuccess = "Consulta paginada completada.";
        public const string PostCreatedSuccess = "El post ha sido creado correctamente.";
    

        // Mensajes de Error de Negocio (Para ResponseApi.Message)
        public const string InternalServerError = "Ocurrió un error inesperado en el servidor. Intente más tarde.";
        public const string CustomerDuplicateNameWarning = "Intento de actualización con nombre duplicado: {Name}";

        // Mensajes para GetById y Delete
        public const string PostNotFound = "La publicación con ID {0} no existe.";
        public const string PostGetSuccess = "Publicación recuperada con éxito.";
        public const string PostDeleteSuccess = "La publicación ha sido eliminada correctamente.";
        public const string PostDeleteError = "Error al intentar eliminar la publicación.";
    }
}
