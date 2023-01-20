using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Configurator.Application
{
    public class FunctionApp : IFunctionApp
    {
        private readonly AppSettings _settings;
        //  <add key="BaseURL" value="http://localhost:7071" />
        //  < add key = "BaseURL" value = "http://icodefunctionapp.azurewebsites.net" />

        public FunctionApp(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
        }

        public byte[] GetDecryptedData(string iCodeSerialNumber, byte[] seedData)
        {
            string seedDataStr = HelperFunctions.ByteArrayToString(seedData);
            var baseUrl = GetBaseUrl();
            var completeUrl = $"{baseUrl}/api/iCodeUnlock?code=2JNOZCfh9kaaRPh1HQbPhxWukH3Nn1fe08POjEjEw/KKenGInUfinA==&iCodeSerialNumber={iCodeSerialNumber}&seedData={seedDataStr}";

            return ReturnResponse(completeUrl);
        }

        public byte[] GetApplicationContent(string iCodeSerialNumber)
        {
            var baseUrl = GetBaseUrl();
            var completeUrl = $"{baseUrl}/api/GetApplicationData?code=bLyp68n99feNpHnIIdsMxBiZIgeiHqSdOPeZQnYVtmla2aYxbYIhNw==&iCodeSerialNumber={iCodeSerialNumber}";
            var webResponse = HelperFunctions.GetWebResponse(completeUrl);
            return HelperFunctions.StringToByteArray(webResponse);
        }

        private string GetBaseUrl()
        {
            return _settings.BaseUrl;
        }

        public string GetLookUpData(string lookupType, int offset)
        {
            var baseUrl = GetBaseUrl();
            var completeUrl = $"{baseUrl}/api/GetLookup?code=NGCfDacJCZ0UrfhTGVuDWvJkZH5umcDqu5lhLYdOTLyI86/WOftyow==&lookupType={lookupType}&offset={offset}";
            var webResponse = HelperFunctions.GetWebResponse(completeUrl);
            var iCodeResponse = JsonHelper.ToClass<BaseResponse>(webResponse);
            return iCodeResponse.Response;
        }

        private byte[] ReturnResponse(string completeUrl)
        {
            try
            {
                var webResponse = HelperFunctions.GetWebResponse(completeUrl);
                var iCodeResponse = JsonHelper.ToClass<BaseResponse>(webResponse);
                if (iCodeResponse.Success)
                {
                    return HelperFunctions.StringToByteArray(iCodeResponse.Response);
                }
                else
                {
                    throw new Exception(iCodeResponse.Errors.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception {ex.Message} occurred while invoking API {completeUrl}");
            }
        }
    }
}
