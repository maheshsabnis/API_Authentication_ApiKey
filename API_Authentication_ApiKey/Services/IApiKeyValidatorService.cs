namespace API_Authentication_ApiKey.Services
{
    public interface IApiAuthKeyValidatorService
    {
        void ValidateApiAuthKey(string apiAuthKey, out bool isValid);
    }
}
