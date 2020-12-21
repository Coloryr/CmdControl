using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace CmdControl
{
    class ConfigUtils
    {
        public static async void Write(object obj, string local)
        {
            try
            {
                await File.WriteAllTextAsync(local, JsonConvert.SerializeObject(obj, Formatting.Indented), Encoding.UTF8);
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
                    File.WriteAllText(local, JsonConvert.SerializeObject(obj, Formatting.Indented));
                    return obj;
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(File.ReadAllText(local, Encoding.UTF8));
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
