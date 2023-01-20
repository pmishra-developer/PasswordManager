using System.Net;

namespace Configurator.Application
{
    public class HelperFunctions
    {
        public static byte[] StringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("Cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        public static string ByteArrayToString(byte[] seedData)
        {
            string tempStr = BitConverter.ToString(seedData);
            return tempStr.Replace("-", string.Empty);
        }

        public static string GetWebResponse(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            try
            {
                WebRequest request = WebRequest.Create(url);
                using WebResponse response = request.GetResponse();

                var responseStream = response.GetResponseStream();
                using var reader = new StreamReader(responseStream);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static void DisplayError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
