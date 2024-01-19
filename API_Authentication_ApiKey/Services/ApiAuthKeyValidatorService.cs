namespace API_Authentication_ApiKey.Services
{
    public class ApiAuthKeyValidatorService : IApiAuthKeyValidatorService
    {
        IConfiguration _configuration;
        /// <summary>
        /// Inject the IConfiguration to read ApiAuthKey from appsettings.json
        /// </summary>
        /// <param name="configuration"></param>
        public ApiAuthKeyValidatorService(IConfiguration configuration)
        {
            _configuration = configuration;           
        }
        void IApiAuthKeyValidatorService.ValidateApiAuthKey(string apiAuthKey, out bool isValid)
        {
            isValid = false;
            // Make apiAuthKey is not Null or EMpty

            if (string.IsNullOrEmpty(apiAuthKey))
                isValid = false;

            // Read the Key from the appsettings.json
            string? key = _configuration.GetValue<string>("ApiAuthKey");
            // Make sure that 
            if (string.IsNullOrEmpty(key))
                isValid = false;
            if (key.Equals(apiAuthKey))
                isValid = true;
           
        }
    }
}
