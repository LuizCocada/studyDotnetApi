using AutoMapper;
using StudyAPI.DTOs.CategoryDTO;
using StudyAPI.DTOs.ProductDTO;
using StudyAPI.Models;

namespace StudyAPI.DTOs.Mappings;

public class ProdutoDtoMappingProfile : Profile
{
    public ProdutoDtoMappingProfile()
    {
        CreateMap<Produto, ProdutoDTO>().ReverseMap();//mapeamento bi-latetal
        CreateMap<Categoria, CategoriaDto>().ReverseMap(); //no momento produto esta usando extensions manual.
        CreateMap<Produto, ProdutoDtoUpdateRequest>().ReverseMap();
        CreateMap<Produto, ProdutoDtoUpdateResponse>().ReverseMap();
    }
}