using AgendaApi.Controllers;
using AgendaApi.Data;
using AgendaApi.Data.Repository.Implementations;
using AgendaApi.Data.Repository.Interfaces;
using AgendaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Xunit;

namespace AgendaTest
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AgendaContext>(dbContextOptions => dbContextOptions.UseSqlite(
                "Data Source=ConsultaAlumnos.db"));
            services.AddAuthentication("Bearer") //"Bearer" es el tipo de auntenticaci�n que tenemos que elegir despu�s en PostMan para pasarle el token
            .AddJwtBearer(options => //Ac� definimos la configuraci�n de la autenticaci�n. le decimos qu� cosas queremos comprobar. La fecha de expiraci�n se valida por defecto.
                            {
                                options.TokenValidationParameters = new()
                                {
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateIssuerSigningKey = true,
                                    ValidIssuer = "https://localhost:52852",
                                    ValidAudience = "agendaapi",
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("thisisthesecretforgeneratingakey(mustbeatleast32bitlong)"))
                                };
                            }
            );
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
    public class AuthenticationControllerTest
    {
        private readonly IUserRepository _userRepository;
        private readonly ConfigurationManager _configuration;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTest(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _configuration = new ConfigurationManager();
            _controller = new AuthenticationController(_configuration, _userRepository);

        }

        [Fact]
        public void AuthenticateForbidenUser()
        {
            //Preparar la prueba
            AuthenticationRequestBody requestBody = new AuthenticationRequestBody()
            {
                Password = "Contrase�aFalsa",
                UserName = "NombreFalso"
            };

            //Ejecutar la prueba
            var response = (ForbidResult)_controller.Autenticar(requestBody);

            //Verificar
            Assert.IsType<ForbidResult>(response);
        }

        [Fact]
        public void Authenticate�lowUser()
        {
            //Preparar la prueba
            AuthenticationRequestBody requestBody = new AuthenticationRequestBody()
            {
                Password = "Pa$$w0rd",
                UserName = "karenpiola"
            };

            //Ejecutar la prueba
            var response = (OkObjectResult)_controller.Autenticar(requestBody);

            //Verificar
            Assert.IsType<OkObjectResult>(response);
        }
    }
}