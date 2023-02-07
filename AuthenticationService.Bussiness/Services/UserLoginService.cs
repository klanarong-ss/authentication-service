using AuthenticationService.Bussiness.common;
using AuthenticationService.Bussiness.InterfaceServices;
using AuthenticationService.DataAccess.ComplexModels;
using AuthenticationService.DataAccess.DTOs;
using AuthenticationService.DataAccess.InterfaceRepositories;
using AuthenticationService.DataAccess.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Bussiness.Services
{
    public class UserLoginService : IUserLoginService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public UserLoginService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserLogin>> GetAll()
        {
            return await _unitOfWork.UserLogin.FindAll().ToListAsync();
        }

        public async Task<AuthenticationResponse> Login(AuthenticateRequest authenticateRequest)
        {
            var user = await _unitOfWork.UserLogin.FindByCondition(x => x.Username == authenticateRequest.Username).FirstOrDefaultAsync();
            if (user == null)
                throw new Exception(ResponseMessage.UserNotFound);


            if (VerifyPassword(authenticateRequest.Password, user.Password, user.Salt))
            {
                var _profile = new AuthenticationResponse
                {
                    Username = user.Username,
                    token = GenerateJwtToken(user)
                };

                return _profile;
            }
            else
            {
                throw new Exception(ResponseMessage.UserOrPasswordIncorrect);
            }
        }

        public async Task<string> Register(UserLoginDto newUser)
        {
            var checkUser = await _unitOfWork.UserLogin.FindByCondition(x => x.Username == newUser.Username).FirstOrDefaultAsync();
            if (checkUser != null)
                throw new Exception($"Username {newUser.Username} is already taken");

            var user = _mapper.Map<UserLogin>(newUser);

            byte[] salt = RandomNumberGenerator.GetBytes(64);
            user.UserId = Guid.NewGuid().ToString();
            user.Password = HashPasword(newUser.Password, out salt);
            user.Salt = salt;
            user.CreatedDate = DateTime.Now;
            user.IsActive = true;

            _unitOfWork.UserLogin.Create(user);

            return ResponseMessage.Success;
        }

        string HashPasword(string password, out byte[] salt)
        {
            const int keySize = 64;
            const int iterations = 350000;
            HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

            salt = RandomNumberGenerator.GetBytes(64);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            return Convert.ToHexString(hash);
        }
        bool VerifyPassword(string password, string hash, byte[] salt)
        {
            const int keySize = 64;
            const int iterations = 350000;
            HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                hashAlgorithm,
                keySize);

            return hashToCompare.SequenceEqual(Convert.FromHexString(hash));
        }
        private string GenerateJwtToken(UserLogin user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _configuration.GetSection("JWT:Secret").Value;
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserId.ToString()),
                                                     new Claim(ClaimTypes.Name,user.Username),
                                                     new Claim(ClaimTypes.Role,"admin"),
                                                    }),
                Expires = DateTime.UtcNow.AddDays(1),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
