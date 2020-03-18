using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Demo6
{
    class Md5
    {
        public static string GetMd5(string str)
        {
            /// <summary>
            /// MD5加密
            /// </summary>
            //创建MD5的对象
            MD5 myMd5 = MD5.Create();
            ///将用户输入的字符串转换为字节数组
            byte[] buffer = System.Text.Encoding.Default.GetBytes(str);
            //调用MD5加密的方法，将MD5加密后的的字节数组存储到Md5buffer字节数组中
            byte[] Md5buffer = myMd5.ComputeHash(buffer);

            //将加密后的字节数组转换为字符串, 是将字节数组中的每个元素都转换为字符串。
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Md5buffer.Length; i++)
            {
                sb.Append(Md5buffer[i].ToString("x2"));  //x-->将10进制转换为16进制。2-->每次都是两位数输出。
            }
            return sb.ToString();
        }
    }
}
