using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLib
{
    public class xConverter
    {
        private static string[] HEX_TABLE = {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0a", "0b", "0c", "0d", "0e", "0f", "10", "11",
            "12", "13", "14", "15", "16", "17", "18", "19", "1a", "1b", "1c", "1d", "1e", "1f", "20", "21", "22", "23",
            "24", "25", "26", "27", "28", "29", "2a", "2b", "2c", "2d", "2e", "2f", "30", "31", "32", "33", "34", "35",
            "36", "37", "38", "39", "3a", "3b", "3c", "3d", "3e", "3f", "40", "41", "42", "43", "44", "45", "46", "47",
            "48", "49", "4a", "4b", "4c", "4d", "4e", "4f", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
            "5a", "5b", "5c", "5d", "5e", "5f", "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6a", "6b",
            "6c", "6d", "6e", "6f", "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7a", "7b", "7c", "7d",
            "7e", "7f", "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8a", "8b", "8c", "8d", "8e", "8f",
            "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9a", "9b", "9c", "9d", "9e", "9f", "a0", "a1",
            "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "aa", "ab", "ac", "ad", "ae", "af", "b0", "b1", "b2", "b3",
            "b4", "b5", "b6", "b7", "b8", "b9", "ba", "bb", "bc", "bd", "be", "bf", "c0", "c1", "c2", "c3", "c4", "c5",
            "c6", "c7", "c8", "c9", "ca", "cb", "cc", "cd", "ce", "cf", "d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7",
            "d8", "d9", "da", "db", "dc", "dd", "de", "df", "e0", "e1", "e2", "e3", "e4", "e5", "e6", "e7", "e8", "e9",
            "ea", "eb", "ec", "ed", "ee", "ef", "f0", "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "fa", "fb",
            "fc", "fd", "fe", "ff"
        };
        //===========================================================================================================================
        public static bool Compare(byte[] data1, byte[] data2, int data_len)
        {
            if (data1 == null || data2 == null || data1.Length < data_len || data2.Length < data_len) return false;
            int len = data2.Length;
            if (data1.Length < len) len = data1.Length;

            for (int i = 0; i < data_len; i++) if (data1[i] != data2[i]) return false;
            return true;
        }

        public static unsafe bool Compare(string str, xContent content)
        {
            if (str == null || str.Length == 0 || str.Length > content.Size) { return false; }

            for (int i = 0; i < str.Length; i++) { if (str[i] != content.Obj[i]) { return false; } }
            return true;
        }

        public static unsafe bool Compare(byte[] data1, void *data2, int data_len)
        {
            byte* ptr = (byte*)data2;
            if (data1 == null || data2 == null || data1.Length < data_len) return false;

            for (int i = 0; i < data_len; i++) if (data1[i] != ptr[i]) return false;
            return true;
        }

        public static unsafe bool Compare(string data1, void* data2, int data_len)
        {
            byte* ptr = (byte*)data2;
            if (data1 == null || data2 == null || data1.Length < data_len) return false;

            for (int i = 0; i < data_len; i++) if (data1[i] != ptr[i]) return false;
            return true;
        }

        public static bool Compare(string data1, string data2)
        {
            if (data1 == null || data2 == null || data1.Length > data2.Length) { return false; }
            for (int i = 0; i < data1.Length; i++) { if (data1[i] != data2[i]) return false; }
            return true;
        }

        public static bool Compare(string str1, string[] str2)
        {
            if (str2 == null || str1 == null) return false;
            for (int i = 0; i < str2.Length; i++) { if(Compare(str1, str2[i])) return true; }
            return false;
        }

        public static bool Compare(string data_in, int first_index, string str)
        {
            if (str == null || data_in == null || str.Length == 0 || first_index > data_in.Length || first_index + str.Length > data_in.Length) return false;
            int accept = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (data_in[i + first_index] == str[i]) accept++;
                else break;
            }
            if (accept == str.Length) return true;
            return false;
        }
        //===============================================================================================================================================================================
        public static string GetRange(string str, int start_index, int count)
        {
            string result = "";
            if (str == null) { return result; }

            int i = start_index;
            while (i < start_index + count && i < str.Length)
            {
                result += str[i];
                i++;
            }
            return result;
        }
        //===============================================================================================================================================================================
        public static string[] Split(string content, string separator)
        {
            if (content == null || separator == null) return null;
            List<string> rows = new List<string>();
            List<byte> data = new List<byte>();

            foreach (byte ch in content)
            {
                data.Add(ch);

                if (data.Count >= separator.Length)
                {
                    int i = separator.Length;
                    int j = data.Count;
                    while (i > 0)
                    {
                        i--;
                        j--;

                        if (separator[i] != data[j]) { goto end_foreach; }
                    }

                    data.RemoveRange(data.Count - separator.Length, separator.Length);
                    rows.Add(Encoding.UTF8.GetString(data.ToArray()));
                    data.Clear();
                }
            end_foreach:;
            }
            return rows.ToArray();
        }
        //===============================================================================================================================================================================
        public static string[] Split(string[] str1, string str2)
        {
            if (str1 == null || str2 == null) return null;
            List<string> rows = new List<string>();
            string TempStr;
            for (int i = 0; i < str1.Length; i++)
            {
                TempStr = "";
                if (str1[i].Length >= str2.Length)
                {
                    for (int j = 0; j < str1[i].Length; j++)
                    {
                        if (j <= str1[i].Length - str2.Length && Compare(str1[i], j, str2)) { if (TempStr.Length > 0) rows.Add(TempStr); j += str2.Length - 1; TempStr = ""; }
                        else TempStr += str1[i][j];
                    }
                    if (TempStr.Length > 0) rows.Add(TempStr);
                }
                else rows.Add(str1[i]);
            }
            return rows.ToArray();
        }
        //===============================================================================================================================================================================
        public static List<byte[]> Split(byte[] str, char separator, char[] ignoring)
        {
            if (str == null) return null;
            List<byte[]> StrOut = new List<byte[]>();
            List<byte> word = new List<byte>();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != separator)
                {
                    if (ignoring != null)
                    {
                        for (int j = 0; j < ignoring.Length; j++)
                        {
                            if (str[i] != ignoring[j]) word.Add(str[i]);
                        }
                    }
                    else word.Add(str[i]);
                }
                else
                {
                    if (word.Count > 0)
                    {
                        StrOut.Add(word.ToArray());
                        word.Clear();
                    }
                }
            }
            if (word.Count > 0) StrOut.Add(word.ToArray());
            return StrOut;
        }
        //===============================================================================================================================================================================       
        public static unsafe string ToStrHex(byte[] In, int InSize)
        {
            if (In != null && In.Length >= InSize && InSize > 0)
            {
                string tmp = "";
                for (int i = 0; i < InSize; i++)
                {
                    tmp += "0x" + HEX_TABLE[In[i]];
                    if (i < InSize - 1) tmp += " ";
                }
                return tmp;
            }
            return null;
        }
        //===============================================================================================================================================================================
        public static unsafe string ToStrHex(string str)
        {
            if (str != null && str.Length > 0)
            {
                string tmp = "";
                for (int i = 0; i < str.Length; i++)
                {
                    tmp += "0x" + HEX_TABLE[str[i]];
                    if (i < str.Length - 1) tmp += " ";
                }
                return tmp;
            }
            return null;
        }
        //===============================================================================================================================================================================
        public static unsafe string ToStrHex(void* InPtr, int InSize)
        {
            if (InPtr != null && InSize > 0)
            {
                string tmp = "";
                byte* In = (byte*)InPtr;

                for (int i = 0; i < InSize; i++)
                {
                    tmp += "0x" + HEX_TABLE[In[i]];
                    if (i < InSize - 1) tmp += " ";
                }
                return tmp;
            }
            return null;
        }
        //===============================================================================================================================================================================
        public static int[] StrHexToInt(string str, string prefix, string separator)
        {
            if (str == null || prefix == null || separator == null) return null;
            string[] InList = Split(new string[] { str }, separator);
            List<int> OutInt = new List<int>();
            if (prefix != null && prefix.Length > 0) InList = Split(InList, prefix);

            if (InList.Length > 1)
            {
                for (int i = 0; i < InList.Length; i++)
                {
                    try { OutInt.Add(Convert.ToInt32(InList[i], 16)); }
                    catch { }
                }
            }
            else
            {
                try { OutInt.Add(Convert.ToInt32(InList[0], 16)); }
                catch { }
            }

            return OutInt.ToArray();
        }

        public static string StrHexToStr(string str, string prefix, string separator)
        {
            if (str == null || prefix == null || separator == null) return null;
            int[] temp = StrHexToInt(str, "0x", " ");
            string tStr = "";
            if (temp.Length > 0)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    tStr += (char)temp[i];
                }
            }
            return tStr;
        }        
        //===============================================================================================================================================================================
        public static unsafe string GetString(void* obj, int offset, int obj_size)
        {
            string str = "";
            byte* ptr = (byte*)obj + offset;
            if (obj == null) { return str; }

            for (int i = 0; i < obj_size; i++) { str += (char)ptr[i]; }

            return str;
        }

        public static string GetString(byte[] data)
        {
            string str = "";

            foreach (byte ch in data)
            {
                str += (char)ch;
            }

            return str;
        }

        public static unsafe string GetString(void* obj, int offset, int obj_size, char end)
        {
            string str = "";
            byte* ptr = (byte*)obj + offset;

            if (obj == null) { return str; }
            for (int i = 0; i < obj_size; i++) { if ((char)ptr[i] == end) { return str; } str += (char)ptr[i]; }

            return str;
        }
        public static unsafe string GetString(void* obj, int obj_size) { return GetString(obj, 0, obj_size); }

        public static unsafe string GetString(xContent content) { return GetString(content.Obj, 0, content.Size); }

        public static unsafe string GetString(void* obj, int obj_size, char end) { return GetString(obj, 0, obj_size, end); }
        //===============================================================================================================================================================================
        public object get_value(TypeCode type, string text)
        {
            object val = 0;
            if (text == null && text.Length == 0) { return val; }
            try
            {
                switch (type)
                {
                    case TypeCode.Byte: return byte.Parse(text);
                    case TypeCode.UInt16: return ushort.Parse(text);
                    case TypeCode.UInt32: return uint.Parse(text);
                    case TypeCode.UInt64: return ulong.Parse(text);

                    case TypeCode.Int16: return short.Parse(text);
                    case TypeCode.Int32: return int.Parse(text);
                    case TypeCode.Int64: return long.Parse(text);

                    case TypeCode.Double: return double.Parse(text);
                    case TypeCode.Decimal: return float.Parse(text);
                }
            }
            catch { }

            return val;
        }
        //===============================================================================================================================================================================
    }
}
