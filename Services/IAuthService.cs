using System.Threading.Tasks;
using backendcafe.DTO;
using backendcafe.Models;

namespace backendcafe.Services
{
    public interface IAuthService
    {
        Task<UserDTO> Register(RegisterDTO registerDto);
        Task<UserDTO> Login(LoginDTO loginDto);
        Task<bool> UserExists(string username);
    }
}