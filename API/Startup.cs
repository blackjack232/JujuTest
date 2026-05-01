using Business.Common.Interfaces;
using Business.Common.Validators;
using Business.Services;
using DataAccess.Data;
using DataAccess.Interfaces;
using DataAccess.Repositories;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System.Linq;

namespace ProjectAPI.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Servicios                      
            services.AddScoped<JujuTestContext, JujuTestContext>();

            //Agregar cadena de conexion al contexto
            services.AddDbContext<JujuTestContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Development")));

            services.AddScoped(typeof(IBaseModel<>), typeof(BaseRepository<>));

            // Inyección de Servicios de Negocio
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IPostService, PostService>();
            // Inyección de repositorio
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(dispose: true);
            });
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation(fv =>
                {
                    // Registra todos los validadores que estén en el mismo proyecto que 'CustomerCreateDtoValidator'
                    fv.RegisterValidatorsFromAssemblyContaining<CustomerCreateDtoValidator>();

                    // Opcional: Esto hace que las validaciones de [Attributes] de .NET también funcionen
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = true;
                });
            //Intercepta y llena los errores de fluent validate para devolverlos en un formato personalizado
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    var response = new ResponseApi<object>(errors, "Se encontraron errores de validación");

                    return new BadRequestObjectResult(response);
                };
            });


            // ======== CONFIGURACIÓN DE SWAGGER =========
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "TestAPI", Version = "v1" });
                });
            services.AddHttpContextAccessor();
            services.AddSession();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // ======== CONFIGURACIÓN DE SWAGGER =========
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "TestAPI v1");
            });

            app.UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();


            app.UseSession();
            app.UseMvc();
        }
    }
}