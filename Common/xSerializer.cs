using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace xLib.Common
{
    public static class xSerializer
    {
        public static xAction<string> Trace;

        public static bool SaveObject(object arg, string file_name)
        {
            if (file_name == null || arg == null) return false;
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream Stream = new FileStream(file_name, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(Stream, arg);
                }
            }
            catch(Exception e)
            {
                Trace?.Invoke(e.ToString());
                return false;
            }
            return true;
        }

        public static object OpenObject(string file_name)
        {
            if (file_name == null) return null;
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (FileStream Stream = new FileStream(file_name, FileMode.Open))
                {
                    return formatter.Deserialize(Stream);
                }
            }
            catch (Exception e)
            {
                Trace?.Invoke(e.ToString());
                return null;
            }
        }
        /*
        public static object OpenJson(string file_name, object arg)
        {
            
            if (file_name == null) return null;
            try
            {               
                using (FileStream Stream = new FileStream(file_name, FileMode.Open))
                {
                    return JsonSerializer.Deserialize<>();
                }                
                return null;
            }
            catch (Exception e)
            {
                Trace?.Invoke(e.ToString());
                return null;
            }
            
        }
        */
    }
}
