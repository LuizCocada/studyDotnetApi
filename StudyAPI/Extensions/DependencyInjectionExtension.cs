using StudyAPI.DTOs.Mappings;
using StudyAPI.Repositories;
using StudyAPI.Repositories.IRepositorys;

namespace StudyAPI.Extensions;

public static class DependencyInjectionExtension
{
    public static void AddAplication(this IServiceCollection services)
    {
        AddRepositories(services);
        AddAutoMapper(services);
    }

    private static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ProdutoDtoMappingProfile));
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}