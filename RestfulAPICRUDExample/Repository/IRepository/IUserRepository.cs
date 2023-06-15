using RestfulAPICRUDExample.Models;
using RestfulAPICRUDExample.Models.DTO;

namespace RestfulAPICRUDExample.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO);
    }
}
