using AuthenticationService.DataAccess.DTOs;
using AuthenticationService.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Bussiness.InterfaceServices
{
    public interface IAuthenService
    {
        Task<AuthenResponse> Login(LoginModel loginModel);
        Task<string> Register(RegisterRequest registerRequest);
        Task<TokenModel> RefreshToken(TokenModel tokenModel);
        Task RevokeToken(string username);
        Task<IEnumerable<UserLogin>> GetAll();
    }
}
