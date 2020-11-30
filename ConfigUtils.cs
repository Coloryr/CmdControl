using System;
using System.IO;
using System.Text.Json;

namespace CmdControl
{
    class ConfigUtils
    {
        public static async void Write(object obj, string local)
        {
            try
            {
                await File.WriteAllTextAsync(local, JsonSerializer.Serialize(obj));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static T Read<T>(string local, T obj)
        {
            try
            {
                if (!File.Exists(local))
                {
                    File.WriteAllText(local, JsonSerializer.Serialize(obj));
                    return obj;
                }
                else
                {
                    return JsonSerializer.Deserialize<T>(File.ReadAllText(local));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return obj;
        }
    }
}
