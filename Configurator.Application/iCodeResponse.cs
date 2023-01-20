using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Application
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public List<string> Errors { get; }
        public string Response { get; set; }
        private BaseResponse(bool success = true, List<string>? errors = null)
        {
            Success = success;
            Errors = errors ?? new List<string>();
        }

        public static BaseResponse Failed(string errorMessage)
        {
            return new BaseResponse(false, new List<string> { errorMessage });
        }

        public static BaseResponse Failed(List<string> errors)
        {
            return new BaseResponse(false, errors);
        }

        public static BaseResponse Successful()
        {
            return new BaseResponse(true);
        }
    }
}
