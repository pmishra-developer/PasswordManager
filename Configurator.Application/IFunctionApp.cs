using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurator.Application
{
    public interface IFunctionApp
    {
        public byte[] GetDecryptedData(string iCodeSerialNumber, byte[] seedData);
        public byte[] GetApplicationContent(string iCodeSerialNumber);
        public string GetLookUpData(string lookupType, int offset);
    }
}
