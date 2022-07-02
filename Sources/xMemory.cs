using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using xLib.Sources;
using xLib.Transceiver;

namespace xLib
{
    public class xMemory
    {
        public static unsafe int Copy(void* Out, void* In, int InSize, int Offset)
        {
            if (Out != null && In != null && InSize > 0)
            {
                byte* OutPtr = (byte*)Out;
                byte* InPtr = (byte*)In;
                for (int i = 0; i < InSize; i++)
                {
                    OutPtr[i + Offset] = InPtr[i];
                }
                return InSize;
            }
            return 0;
        }

        public static unsafe int Copy(byte[] Out, byte[] In, int Offset)
        {
            if (Out != null && In != null)
            {
                fixed (byte* ptr = Out)
                {
                    return Copy(ptr, In, In.Length, Offset);
                }
            }
            return 0;
        }

        public static unsafe int Copy(void* Out, void* In, int Size)
        {
            return Copy(Out, In, Size, 0);
        }

        public static unsafe int Copy(byte[] Out, void* In, int Size, int Offset)
        {
            if (Out != null && In != null)
            {
                fixed (byte* ptr = Out)
                {
                    return Copy(ptr, In, Size, Offset);
                }
            }
            return 0;
        }

        public static unsafe int Copy(byte[] Out, string In, int Offset)
        {
            if (Out != null && In != null && In.Length > 0 && (Out.Length >= (In.Length + Offset)))
            {
                for (int i = 0; i < In.Length; i++)
                {
                    Out[i + Offset] = (byte)In[i];
                }
                return In.Length;
            }
            return 0;
        }

        public static unsafe int Copy(void* Out, string Str, int Offset)
        {
            if (Out != null && Str != null && Str.Length > 0)
            {
                byte* OutPtr = (byte*)Out;
                for (int i = 0; i < Str.Length; i++)
                {
                    OutPtr[i + Offset] = (byte)Str[i];
                }
                return Str.Length;
            }
            return 0;
        }

        public static unsafe int Copy(void* Out, byte[] In, int InSize, int Offset)
        {
            if (Out != null && In != null && InSize > 0)
            {
                byte* OutPtr = (byte*)Out;
                for (int i = 0; i < InSize; i++)
                {
                    OutPtr[i + Offset] = In[i];
                }
                return InSize;
            }
            return 0;
        }

        public static unsafe bool Compare(void* in_1, int offset_1, void *in_2, int offset_2, int count)
        {
            if (in_1 != null && in_2 != null && count > 0)
            {
                byte* ptr_1 = (byte*)in_1;
                byte* ptr_2 = (byte*)in_2;
                for (int i = 0; i < count; i++)
                {
                    if (ptr_1[offset_1 + i] != ptr_2[offset_2 + i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static unsafe string ToString(void* obj, int obj_size)
        {
            string str = "";
            char* ptr;

            if (obj == null || obj_size <= 0 ) { return str; }

            ptr = (char*)obj;
            for (uint i = 0; i < obj_size; i++)
            {
                str += ptr[i];
            }

            return str;
        }
    }
}
