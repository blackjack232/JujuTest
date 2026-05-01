🚀 API Juju System - Gestión de Publicaciones
📝 Descripción del Proyecto
Sistema especializado en la orquestación de contenidos y gestión de clientes, desarrollado sobre .NET Core 2.1. La solución destaca por su motor de procesamiento por lotes (Bulk Process) con capacidad de filtrado inteligente y validaciones de integridad de datos en tiempo real.

🏛️ Arquitectura de la Solución
El proyecto sigue un patrón de Arquitectura Limpia (Clean Architecture) simplificada, optimizada para la escalabilidad y mantenibilidad.


Desglose de Componentes
Presentación (REST API): Controladores desacoplados que gestionan el ruteo y las respuestas HTTP estandarizadas.

Capa de Servicios (Business Logic): Orquestadores de procesos que aplican reglas de negocio complejas (Mapeo manual, truncado de texto y lógica de categorización).

Capa de Datos (Persistence): Implementación de Repository Pattern sobre EF Core para abstracción total de la base de datos.

⚙️ Principios SOLID Aplicados
La arquitectura se diseñó bajo los cinco principios SOLID para garantizar un código mantenible:

S - Single Responsibility: Los controladores solo manejan rutas, los servicios solo lógica de negocio y los repositorios solo persistencia.

O - Open/Closed: El uso de BaseRepository<TEntity> permite extender funcionalidades para nuevas entidades sin modificar la lógica base.

L - Liskov Substitution: Los repositorios específicos (CustomerRepository, PostRepository) pueden ser tratados como sus interfaces base sin alterar el comportamiento del sistema.

I - Interface Segregation: Se definieron interfaces específicas (ICustomerService, IPostService) para que cada clase implemente solo lo que realmente necesita.

D - Dependency Inversion: Los controladores y servicios dependen de abstracciones (interfaces), no de implementaciones concretas, facilitando el Unit Testing.

🛠️ Stack Tecnológico
Runtime: .NET Core 2.1

ORM: Entity Framework Core con SQL Server

Logging: ILogger para trazabilidad de errores

Documentación: Swagger UI (OpenAPI)

🗄️ Modelo de Datos (ERD)
La estructura de datos garantiza la integridad referencial para el procesamiento masivo.

Customer: Almacena la información del cliente. Se valida la unicidad del nombre antes de la creación.

Post: Almacena las publicaciones asociadas. Incluye validación de tipos (Farándula, Política, Fútbol) y truncado automático de texto.

📂 Estructura del Repositorio
Plaintext
📦 JujuApi
 ┣ 📂 API (Controladores y Configuración)
 ┣ 📂 Business (Servicios, DTOs y Reglas de Negocio)
 ┃ ┣ 📂 Common (Constants, Helpers, Interfaces)
 ┃ ┗ 📂 Services (Implementaciones)
 ┗ 📂 DataAccess (Contexto, Entidades y Repositorios)
📋 Endpoints Principales
Customers:

GET /api/customer: Listado paginado.

POST /api/customer: Creación con validación de nombre.

Posts:

POST /api/post/bulk: Motor de carga masiva con filtrado de clientes inexistentes y tipos no válidos.

DELETE /api/post/{id}: Eliminación física verificada.

🛡️ Manejo de Resultados
Todas las respuestas utilizan el envoltorio ResponseApi<T> para estandarizar la comunicación con el cliente:

JSON
{
  "succeeded": true,
  "message": "Proceso finalizado. Insertados: 10. Omitidos: 2.",
  "data": true
}