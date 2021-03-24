using System;
using System.IO;
using System.Text;

namespace Chikatto.Bancho.Serialization.Extensions
{
    public static class Extension
    {
        public static byte[] GetBytes(this short obj) => BitConverter.GetBytes(obj);
        public static byte[] GetBytes(this ushort obj) => BitConverter.GetBytes(obj);
        
        public static byte[] GetBytes(this int obj) => BitConverter.GetBytes(obj);
        public static byte[] GetBytes(this uint obj) => BitConverter.GetBytes(obj);
        public static byte[] GetBytes(this float obj) => BitConverter.GetBytes(obj);
        
        public static byte[] GetBytes(this long obj) => BitConverter.GetBytes(obj);
        public static byte[] GetBytes(this ulong obj) => BitConverter.GetBytes(obj);
        public static byte[] GetBytes(this double obj) => BitConverter.GetBytes(obj);

        public static byte[] GetBytes(this string obj) => Encoding.UTF8.GetBytes(obj);
    }
}