using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace WebApplication1.Helpers
{
    public static class JsonSerializeHelper
    {
        public static string GetSerializedString<T>(T value)
        {
            return JsonSerializer.Serialize(value);
        }

        public static void Serialize<T>(T value, string filepath)
        {
            string jsonString = JsonSerializer.Serialize(value);
            File.WriteAllText(filepath, jsonString);
        }

        public static T Deserialize<T>(string filepath)
        {
            var jsonString = File.ReadAllText(filepath);
            return JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}