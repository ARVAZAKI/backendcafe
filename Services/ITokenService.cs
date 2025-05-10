namespace backendcafe.Services
{
    public interface ITokenService
    {
        string CreateToken(Models.User user);
    }
}