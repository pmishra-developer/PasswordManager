﻿using Newtonsoft.Json;
using System.Collections.Generic;
namespace Configurator.Application
{
    public class JsonHelper
    {
        public static string FromClass<T>(T data, bool isEmptyToNull = false,
            JsonSerializerSettings jsonSettings = null)
        {
            var response = string.Empty;

            if (!EqualityComparer<T>.Default.Equals(data, default(T)))
                response = JsonConvert.SerializeObject(data, jsonSettings);

            return isEmptyToNull ? (response == "{}" ? "null" : response) : response;
        }

        public static T ToClass<T>(string data, JsonSerializerSettings jsonSettings = null)
        {
            var response = default(T);

            if (!string.IsNullOrEmpty(data))
                response = jsonSettings == null
                    ? JsonConvert.DeserializeObject<T>(data)
                    : JsonConvert.DeserializeObject<T>(data, jsonSettings);

            return response;
        }
    }
}
