namespace StudyAPI.Extensions;

public static class ConfigAuthorizationExtensions
{
    public static void AddConfigurarionPolicy(this IServiceCollection services)
    {
        ConfigAuthorization(services);
    }


    //politicas de autorização por Roles.
    private static void ConfigAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            
            options.AddPolicy("SuperAdminOnly", policy => policy
                .RequireRole("Admin")
                .RequireClaim("id", "luiz"));
            
            options.AddPolicy("ExclusiveOnly", policy => policy.RequireAssertion(context =>
                context.User.HasClaim(claim => claim.Type == "id" && claim.Value == "luiz") ||
                context.User.IsInRole("SuperAdmin")));
        });
    }
}

//.RequireClaim("id", "luiz")); Claim do tipo id com valor luiz