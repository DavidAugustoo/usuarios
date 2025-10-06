using AutoMapper;
using FCG.API.Models;
using FCG.Application.DTOs;
using FCG.Domain.Entities;

namespace FCG.Application.Mappings
{
    public class EntitiesToDTOMappingProfile : Profile
    {
        public EntitiesToDTOMappingProfile()
        {
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailUsuario.EmailAddress))
                .ReverseMap();
            CreateMap<Usuario, UsuarioViewModel>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailUsuario.EmailAddress))
                .ReverseMap();
        }
    }
}
