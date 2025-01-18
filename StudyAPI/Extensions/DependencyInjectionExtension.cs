using StudyAPI.Repositories;
using StudyAPI.Repositories.IRepositorys;

namespace StudyAPI.Extensions;

public static class DependencyInjectionExtension
{
    public static void AddAplication(this IServiceCollection services)
    {
        AddUseCases(services);
    }

    public static void AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)) ;
    }
}