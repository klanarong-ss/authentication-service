using AuthenticationService.Bussiness.common;
using AuthenticationService.Bussiness.InterfaceServices;
using AuthenticationService.DataAccess.ComplexModels;
using AuthenticationService.DataAccess.DTOs;
using AuthenticationService.DataAccess.InterfaceRepositories;
using AuthenticationService.DataAccess.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Bussiness.Services
{
    public class UserLoginService : IUserLoginService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        IHttpContextAccessor _httpContextAccessor;
        public UserLoginService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserProfile> GetUserProfile()
        {
            var identity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userId = identity.FindFirst("UserId");
                var userById = await _unitOfWork.UserLogin.FindByCondition(x => x.UserId == userId.ToString()).FirstAsync();

                var userProfile = new UserProfile
                {
                    Username = userById.Username,
                    Email = ""
                };
                return userProfile;

            }
            else
            {
                return null;
            }
        }
    }
}
