using AuthenticationService.DataAccess.DTOs;
using AuthenticationService.DataAccess.Models;
using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthenticationService.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterRequest, UserLogin>();
        }
    }
}
