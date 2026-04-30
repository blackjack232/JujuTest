# JujuTest
Prueba tecnica JUJU
Aquí tienes el contenido en texto plano para que lo copies directamente a tu archivo README.md.

ProjectAPI - Gestión de Publicaciones y Clientes
Esta es una API RESTful desarrollada con .NET Core 2.1 diseñada para gestionar la interacción entre clientes y sus publicaciones. El sistema implementa una arquitectura robusta, respuestas estandarizadas y un enfoque orientado a pruebas unitarias.

Características Principales
Arquitectura en Capas: Separación de responsabilidades entre API, Business y DataAccess.

Respuesta Estándar (ResponseApi): Todas las respuestas del servidor siguen una estructura consistente para facilitar la integración con el Frontend.

Paginación de Datos: Implementación de paginación eficiente en los endpoints de lectura para manejar grandes volúmenes de datos.

Procesamiento Masivo: Capacidad de creación de publicaciones en lote mediante el endpoint CreateBulk.

Validaciones de Negocio: Lógica centralizada para el manejo de duplicados, integridad de datos y formato.

Tecnologías y Herramientas
Framework: .NET Core 2.1

Base de Datos: SQL Server

ORM: Entity Framework Core

Documentación: Swagger / Swashbuckle

Pruebas: xUnit, Moq y MockQueryable

Logging: Serilog

Estructura del Proyecto
API/: Controladores, configuración de middleware (Startup/Program) y documentación.

Business/: Servicios, interfaces de negocio, DTOs y lógica de validación.

DataAccess/: Contexto de base de datos, entidades de dominio y repositorios base.

Configuración e Instalación
Clonar el repositorio:
git clone [URL-DE-TU-REPOSITORIO]

Configurar la base de datos:
Actualiza la cadena de conexión en el archivo API/appsettings.json:
"ConnectionStrings": {
"DefaultConnection": "Server=TU_SERVIDOR;Database=ProjectDB;Trusted_Connection=True;"
}

Ejecutar migraciones:
dotnet ef database update

Iniciar la aplicación:
dotnet run --project API

Documentación de la API
Una vez iniciada la API, puedes acceder a la interfaz de Swagger para explorar y probar los endpoints en:
http://localhost:[PUERTO]/swagger

Ejemplo de respuesta exitosa:
{
"succeeded": true,
"message": "Operación realizada con éxito",
"data": { ... }
}

Pruebas Unitarias
El proyecto incluye una suite de pruebas para asegurar la calidad del código. Para ejecutarlas, usa el comando:
dotnet test

Desarrollado por: Fredy Alexander España Garcia
Fecha: Abril 2026