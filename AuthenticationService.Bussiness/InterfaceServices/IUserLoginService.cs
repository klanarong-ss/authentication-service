using AuthenticationService.DataAccess.ComplexModels;
using AuthenticationService.DataAccess.DTOs;
using AuthenticationService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Bussiness.InterfaceServices
{
    public interface IUserLoginService
    {
        Task<IEnumerable<UserLogin>> GetAll();
        Task<AuthenticationResponse> Login(AuthenticateRequest authenticateRequest);
        Task<string> Register(UserLoginDto newUser);
    }
}
