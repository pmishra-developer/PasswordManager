using System;
using PasswordManager.Services.Contracts;

namespace PasswordManager.Services
{
    public class SampleService : ISampleService
    {
        public string GetCurrentDate()
        {
            return DateTime.Now.ToLongDateString();
        }
    }
}
