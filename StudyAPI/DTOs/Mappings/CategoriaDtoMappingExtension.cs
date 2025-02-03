using StudyAPI.Domain;
using StudyAPI.DTOs.CategoryDTO;

namespace StudyAPI.DTOs.Mappings;

public static class CategoriaDtoMappingExtension
{
    //ALTERNATIVA AO USO DO AUTO-MAPPER
    //PODEMOS USAR ESTA:MANUAL OU A BIBLIOTECA AUTO-MAPPER
    public static CategoriaDto? ToCaregoriaDto(this Categoria? categoria)
    {
        return new CategoriaDto()
        {
            CategoriaId = categoria.CategoriaId,
            Nome = categoria.Nome,
            ImageUrl = categoria.ImageUrl
        };
    }

    public static Categoria? ToCategoria(this CategoriaDto categoriaDto)
    {
        return new Categoria()
        {
            CategoriaId = categoriaDto.CategoriaId,
            Nome = categoriaDto.Nome,
            ImageUrl = categoriaDto.ImageUrl
        };
    }

    public static IEnumerable<CategoriaDto> ToCategoriaDtoList(this IEnumerable<Categoria> categorias)
    {
        return categorias.Select(categoria => new CategoriaDto
        {
            CategoriaId = categoria.CategoriaId,
            Nome = categoria.Nome,
            ImageUrl = categoria.ImageUrl
        }).ToList();
    }
}