using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TaskManagement.Data;
using TaskManagement.Model.Common;

namespace TaskManagement.Services
{
    public class Usertokens
    {
        public string User_ID { get; set; }
        public string Role { get; set; }
        public string Role_Type { get; set; }
    }
    public class JwtHandler
    {
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpcontext;
        private readonly DataCon _datacon;

        public JwtHandler(IOptions<AppSettings> appSettings, IHttpContextAccessor httpcontext, DataCon dataCon)
        {
            _datacon = dataCon;
            _appSettings = appSettings.Value;
            _httpcontext = httpcontext ?? throw new ArgumentNullException(nameof(httpcontext));
        }

        public Usertokens GetUsertokens()
        {
            ClaimsIdentity token = _httpcontext.HttpContext?.User.Identity as ClaimsIdentity;
            var usertoken = new Usertokens()
            {
                User_ID = token?.Claims.First(x => x.Type == "User_Id")?.Value,
                Role = token?.Claims.First(x => x.Type == "Role")?.Value,
                Role_Type = token?.Claims.First(x => x.Type == "Role_Type")?.Value
            };
            return usertoken;
        }

        public string Generatetoken(User_Settings user)
        {
            string tokenexpiry = "360";
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("User_ID", user.User_ID.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("Role_Type", user.Role_Type.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(int.Parse(tokenexpiry)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public Usertokens TokenDecrypt(string tokenID)
        {
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? token = handler.ReadToken(tokenID) as JwtSecurityToken;
            var usertoken = new Usertokens()
            {
                User_ID = token?.Claims.First(x => x.Type == "User_ID")?.Value,
                Role = token?.Claims.First(x => x.Type == "role")?.Value,
                Role_Type = token?.Claims.First(x => x.Type == "Role_Type")?.Value
            };
            return usertoken;
        }

        public bool ValidateToken(string jwtToken)
        {
            try
            {
                Usertokens token = TokenDecrypt(jwtToken);
                if (token != null)
                {
                    string currenttoken = "";
                    currenttoken = _datacon.Task_User.FirstOrDefault(x => x.User_ID == int.Parse(token.User_ID)).Token;
                    if (currenttoken == null) return false;
                    return jwtToken == currenttoken;
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
        }
    }
}
