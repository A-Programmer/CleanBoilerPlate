using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Project.Core.Services.SecutiryServices;
using Project.Entities;
using Project.Entities.Common;
using Project.Entities.EntityClasses.IdentityEntities;
using Project.Entities.SharedEntity;

namespace Project.Services.JwtServices
{
    public class JwtService : IJwtService
    {
        private readonly PublicSettings _settings;
        private readonly SignInManager<User> _signInManager;

        public JwtService(IOptionsSnapshot<PublicSettings> settings, SignInManager<User> signInManager)
        {
            _settings = settings.Value;
            this._signInManager = signInManager;
        }

        public async Task<AccessToken> GenerateToken(User user)
        {
            var securityKey = Encoding.UTF8.GetBytes(_settings.JwtOptions.SecretKey);
            var signinCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256Signature);

            var secretKey2 = Encoding.UTF8.GetBytes(_settings.JwtOptions.SecretKey2);
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(secretKey2), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var claims = await _getUserClaims(user);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _settings.JwtOptions.Issuer,
                Audience = _settings.JwtOptions.Audience,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.Now.AddMinutes(_settings.JwtOptions.Expiration),
                SigningCredentials = signinCredentials,
                Subject = new ClaimsIdentity(claims),
                EncryptingCredentials = encryptingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);



            return GenerateAccessToken(securityToken);
        }

        private async Task<IEnumerable<Claim>> _getUserClaims(User user)
        {
            var claims = await _signInManager.ClaimsFactory.CreateAsync(user);
            var claimsList = new List<Claim>(claims.Claims);

            claimsList.Add(new Claim("test", "test"));

            return claimsList;
        }

        public AccessToken GenerateAccessToken(JwtSecurityToken securityToken)
        {
            var accessToken = new AccessToken
            {
                access_token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                token_type = "Bearer",
                expires_in = (int)(securityToken.ValidTo - DateTime.UtcNow).TotalSeconds
            };
            return accessToken;
        }

    }
}
