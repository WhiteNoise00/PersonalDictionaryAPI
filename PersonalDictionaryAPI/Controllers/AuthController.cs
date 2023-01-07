
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalDictionaryAPI.Data;
using PersonalDictionaryAPI.Models;
using PersonalDictionaryAPI.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;


namespace PersonalDictionaryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        public static UserData user_data = new UserData();
        private readonly IConfiguration _configuration;
        private readonly APIDbContext _db;

        public AuthController(IConfiguration configuration, APIDbContext db)
        {
            _configuration = configuration;
            _db = db;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserViewModel>> Register(UserViewModel data)
        {
            CreatePasswordHash(data.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user_data.UserName = data.UserName;
            user_data.PasswordHash = passwordHash;
            user_data.PasswordSalt = passwordSalt;

            User check_user = await _db.Users.FirstOrDefaultAsync(u=>u.UserName==data.UserName);
            if (check_user!=null)
            {
                return BadRequest("The user already exists.");
            }
            else
            {
                try
                {
                    User user = new User
                    {
                        UserName = data.UserName,
                        Password = user_data.PasswordHash,
                        PasswordSalt = passwordSalt
                    };
                    _db.Users.Add(user);
                    _db.SaveChanges();
                    return StatusCode(StatusCodes.Status201Created);
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                } 
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserViewModel>> Login(UserViewModel data)
        {
            User user= await _db.Users.FirstOrDefaultAsync(u => u.UserName == data.UserName);
            if (user.UserName != data.UserName)
            {
                return BadRequest("User "+user.UserName+"not found.");
            }

            if (!VerifyPasswordHash(data.Password, user.Password, user.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            UserData user_data = new UserData
            {
                UserName = user.UserName,
                PasswordHash = user.Password,
                PasswordSalt = user.PasswordSalt
            };

            string token = CreateToken(user_data);
            RefreshToken refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken);
            return Ok(user_data.PasswordHash);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!user_data.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            else if (user_data.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = CreateToken(user_data);
            RefreshToken newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken);

            return Ok(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            RefreshToken refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            CookieOptions cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user_data.RefreshToken = newRefreshToken.Token;
            user_data.TokenCreated = newRefreshToken.Created;
            user_data.TokenExpires = newRefreshToken.Expires;
        }

        private string CreateToken(UserData user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.Add(TimeSpan.FromMinutes(15)),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (HMACSHA512 hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
