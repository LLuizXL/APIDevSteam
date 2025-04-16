
using APIDevSteam.Data;
using APIDevSteam.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args); //Builder: Variável de Serviços


// Add services to the container.


//Adicionar Serviço de Banco de Dados.
builder.Services.AddDbContext<APIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



//Adicionar o Serviço de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Adicionar o serviço de autenticação
// Serviço de EndPoints do Identity Framework
builder.Services.AddIdentityApiEndpoints<Usuario>(options => // Vai fazer os endpoints a partir da Nossa model de Usuário
{
    options.SignIn.RequireConfirmedEmail = false; // Exige confirmação de email
    options.SignIn.RequireConfirmedAccount = false; // Exige confirmação de conta
    options.User.RequireUniqueEmail = true; // Exige email único
    options.Password.RequireNonAlphanumeric = false; // Exige caracteres não alfanuméricos
    options.Password.RequireLowercase = false; // Exige letras minúsculas
    options.Password.RequireUppercase = false; // Exige letras maiúsculas
    options.Password.RequireDigit = false; // Exige dígitos numéricos
    options.Password.RequiredLength = 4; // Exige comprimento mínimo da senha
})
.AddRoles<IdentityRole>() // Adicionando o serviço de roles
.AddEntityFrameworkStores<APIContext>() // Adicionando o serviço de EntityFramework
.AddDefaultTokenProviders(); // Adiocionando o provedor de tokens padrão


// Swagger com Autenticação JWT Bearer
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "APIDevSteam", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Adicionar Serviços de Autenticação e Autorização
builder.Services.AddAuthentication(); //Prova que realmente é você/autentica seu token
builder.Services.AddAuthorization(); //Te da a permissão de acessar determinado Endpoint

var app = builder.Build(); //Cria o aplicativo, a build

// Configure the HTTP request pipeline.

app.UseSwagger(); // Ativa o Swagger
app.UseSwaggerUI(); // Ativa a UI (Interface) do Swagger

app.UseHttpsRedirection(); // Redireciona requisições HTTP para HTTPS.

app.UseCors("AllowAll");

app.UseAuthentication(); // Utiliza a Autenticação
app.UseAuthorization(); // Utiliza a Autorização


app.MapControllers(); // Mapeia os Controladores
app.MapGroup("/Usuario").MapIdentityApi<Usuario>(); // Mapeia o grupo de endpoints de autenticação

app.Run();
