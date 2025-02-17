using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudyAPI.Context;
using StudyAPI.Extensions;
using StudyAPI.Filters;
using StudyAPI.Models;
using StudyAPI.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options =>
    {
        options.Filters.Add(typeof(ApiExceptionFilter)); //filtro de exceção
    }).AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; })
    .AddNewtonsoftJson(); //adiciona possibilidade de usar HTTPPath


builder.Services.AddAplication(); //injeção de dependências
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "OrigensComAcessoPermitido", policy =>
    {
        policy.WithOrigins("http://localhost:xxxx")
            .WithMethods("Get", "Post")
            .AllowAnyHeader();
    });
});


// //habilitar autenticação JWT no swagger
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo { Title = "studyapi", Version = "v1" });
    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT"
    });
    config.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
});

//gerar tabelaas de identidade
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


//receber e validar token
var secretKrey = builder.Configuration["JWT:SecretKey"] ?? throw new ArgumentException("Invalid secret key");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"], //host recebe o token
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"], //host que gera o token
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKrey))
    };
});

builder.Services.AddConfigurarionPolicy(); //configurações de autorização por roles

//rate limiting nivel de Controller
builder.Services.AddRateLimiter(rateLimiterOption =>
{
    rateLimiterOption.AddFixedWindowLimiter(policyName: "fixedwindow", options =>
    {
        options.PermitLimit = 1;
        options.Window = TimeSpan.FromSeconds(5);
        options.QueueLimit = 2;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    rateLimiterOption.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

//rate limiting nivel Global
builder.Services.AddRateLimiter(rateLimiterOption =>
{
    rateLimiterOption.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    rateLimiterOption.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext =>
        RateLimitPartition.GetFixedWindowLimiter(partitionKey: httpcontext.User.Identity?.Name ??
                                                               httpcontext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 5,
                QueueLimit = 2,
                Window = TimeSpan.FromSeconds(10)
            }
        )
    );
});

string? mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnection,
        ServerVersion.AutoDetect(mySqlConnection)));

builder.Services.AddScoped<ITokenServices, TokenServices>(); //serviço de token
builder.Services.AddScoped<ApiLoggingFilter>(); //filtro de log

var app = builder.Build();

//desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler(); //middlawre to global exception handling in development
}

app.UseHttpsRedirection(); //middleware to redirect HTTP request to HTTPS
app.UseStaticFiles(); //middleware to handle static files
app.UseRouting(); //middleware to handle routing

app.UseRateLimiter(); //middleware to handle rate limiter
app.UseCors(); //middleware to handle CORS default

app.UseAuthorization(); //middleware to handle authorization
app.MapControllers(); //middleware to map controllers routes

app.Run();