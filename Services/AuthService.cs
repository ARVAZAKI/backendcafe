using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using backendcafe.Data;
using backendcafe.DTO;
using backendcafe.Models;
using backendcafe.Utils;

namespace backendcafe.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<UserDTO> Register(RegisterDTO registerDto)
        {
            if (await UserExists(registerDto.Username))
                throw new Exception("Username already exists");

            var user = new User
            {
                Username = registerDto.Username.ToLower(),
                Email = registerDto.Email.ToLower(),
                PasswordHash = HashUtility.ComputeHash(registerDto.Password),
                Role = Role.Admin
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = _tokenService.CreateToken(user)
            };
        }

        public async Task<UserDTO> Login(LoginDTO loginDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(x => x.Username == loginDto.Username.ToLower());

            if (user == null)
                throw new Exception("Username not found");

            if (!VerifyPasswordHash(loginDto.Password, user.PasswordHash))
                throw new Exception("Invalid password");

            return new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = _tokenService.CreateToken(user)
            };
        }

        public async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username.ToLower());
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            string computedHash = HashUtility.ComputeHash(password);
            return computedHash == storedHash;
        }
    }
}