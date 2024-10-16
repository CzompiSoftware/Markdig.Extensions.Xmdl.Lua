using System;
using System.Text;

namespace Markdig.Extensions.Xmdl.Lua
{
    internal class Crc
    {
        internal static string Parse(string script)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(script);
            ushort crc = 0xFFFF; // Initialize CRC value
            foreach (byte b in bytes)
            {
                crc ^= (ushort)(b << 8);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                    {
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }
            }

            return $"{crc:X4}";
        }
    }
}