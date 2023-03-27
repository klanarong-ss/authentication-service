using AuthenticationService.Bussiness.common;
using AuthenticationService.Bussiness.InterfaceServices;
using AuthenticationService.DataAccess.ComplexModels;
using AuthenticationService.DataAccess.DTOs;
using AuthenticationService.DataAccess.InterfaceRepositories;
using AuthenticationService.DataAccess.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    public class AuthenService : IAuthenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        IHttpContextAccessor _httpContextAccessor;
        public AuthenService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
           _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthenResponse> Login(LoginModel loginModel)
        {
            try
            {
                var user = await _unitOfWork.UserLogin.FindByCondition(x => x.Username == loginModel.Username).FirstOrDefaultAsync();
                if (user == null)
                    throw new Exception(ResponseMessage.UserNotFound);


                if (VerifyPassword(loginModel.Password, user.Password, user.Salt))
                {
                    var refreshTokenExpire = Convert.ToInt64(_configuration.GetSection("JWT:RefreshTokenValidityInDays").Value);
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginModel.Username),
                    new Claim(ClaimTypes.Role, UserRoles.Admin),
                    new Claim("UserId",user.UserId)
                };

                    var _profile = new AuthenResponse
                    {
                        Username = user.Username,
                        AccessToken = GenerateJwtToken(claims),
                        RefreshToken = GenerateRefreshToken(),
                        RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenExpire)
                    };

                    user.LastLogin = DateTime.Now;
                    user.RefreshToken = _profile.RefreshToken;
                    user.RefreshTokenExpiryTime = _profile.RefreshTokenExpiryTime;

                    _unitOfWork.UserLogin.Update(user);

                    return _profile;
                }
                else
                {
                    throw new Exception(ResponseMessage.UserOrPasswordIncorrect);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<string> Register(RegisterRequest registerRequest)
        {
            var checkUser = await _unitOfWork.UserLogin.FindByCondition(x => x.Username == registerRequest.Username).FirstOrDefaultAsync();
            if (checkUser != null)
                throw new Exception($"Username {registerRequest.Username} is already taken");

            var user = _mapper.Map<UserLogin>(registerRequest);

            byte[] salt = RandomNumberGenerator.GetBytes(64);
            user.UserId = Guid.NewGuid().ToString();
            user.Password = HashPasword(registerRequest.Password, out salt);
            user.Salt = salt;
            user.CreatedDate = DateTime.Now;
            user.IsActive = true;

            _unitOfWork.UserLogin.Create(user);

            return ResponseMessage.RegisterSuccess;
        }

        public async Task<TokenModel> RefreshToken(TokenModel tokenModel)
        {
            string accessToken = tokenModel.AccessToken;
            string refreshToken = tokenModel.RefreshToken;
            var principal = GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default
            var user = await _unitOfWork.UserLogin.FindByCondition(x => x.Username == username).FirstOrDefaultAsync();

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new Exception("Invalid client request");

            var newAccessToken = GenerateJwtToken(principal.Claims);
            var newRefreshToken = GenerateRefreshToken();
            
            user.RefreshToken = newRefreshToken;
            _unitOfWork.UserLogin.Update(user);

            var newToken = new TokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
            return newToken;
        }

        public async Task RevokeToken(string username)
        {
            var user = await _unitOfWork.UserLogin.FindByCondition(x => x.Username == username).FirstOrDefaultAsync();
            if (user is null) 
                throw new Exception(ResponseMessage.InvalidUserName);

            user.RefreshToken = null;
            _unitOfWork.UserLogin.Update(user) ;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value)),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        private string HashPasword(string? password, out byte[] salt)
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

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        bool VerifyPassword(string? password, string hash, byte[] salt)
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

        private string GenerateJwtToken(IEnumerable<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenExpire = Convert.ToInt64(_configuration.GetSection("JWT:TokenValidityInMinutes").Value);
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value));
            var signinCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(tokenExpire),
                SigningCredentials = signinCredentials,
                Issuer = "Issuer",
                Audience = "Audience"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<IEnumerable<UserLogin>> GetAll()
        {
            var a = await _unitOfWork.UserLogin.FindAll().ToListAsync();
            return a;
        }

        public async Task<string> Logout()
        {
            var userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            var user = await _unitOfWork.UserLogin.FindByCondition(x => x.UserId == userId).FirstOrDefaultAsync();

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            _unitOfWork.UserLogin.Update(user);

            return ResponseMessage.LogoutSuccess;
        }
    }
}
