using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DongleDemo
{
   public class CFSystem
    {
        #region 
        //==================================================
        // 功能描述: 获取产品信息
        // 创建日期: 2012-02-21
        // 返回值：0 成功，其他值失败
        // 作者:
        // 修改日期:
        // 修改者:  
        // 修改说明:
        //===================================================
        [DllImport(@"CFSystemInfo.dll", CallingConvention = CallingConvention.Cdecl)]
       private static extern void CF_GetProductInfo(ref CFProductInfo productInfo);
        #endregion

       [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct CFProductInfo
        {
            public UInt32 nProductID;                //产品ID 6291458
            public UInt32 nUserID;                   //用户ID 268435455
            public UInt32 wszKey;                    //用户密钥
            public UInt32 nKeyID;                    //返回产品KeyID
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public UInt16[] tryTimeEnd;            //返回试用期截止时间，与返回值的成功与否无关，有值就表示成功
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public UInt16[] updateTimeEnd;         //返回免费升级截止时间，与返回值的成功与否无关，有值就表示成功
              [MarshalAs(UnmanagedType.ByValArray, SizeConst = 496)]
            public byte[] pUserData;   //返回用户区数据

            public int nUserDataLen;              //返回用户区数据实际长度
            public int retcode;
        }

     

       #region
       public static CFProductInfo GetCFProductInfo(CFProductInfo cfproductInfo)
       {

           int size = Marshal.SizeOf(typeof(CFProductInfo));
           IntPtr productInfoSize = Marshal.AllocHGlobal(size);
           cfproductInfo = (CFProductInfo)Marshal.PtrToStructure((IntPtr)productInfoSize, typeof(CFProductInfo));
           cfproductInfo.nProductID = 6291458;
           cfproductInfo.nUserID = 268435455;
           cfproductInfo.wszKey = 0;
           cfproductInfo.nKeyID = 0;
           cfproductInfo.tryTimeEnd = new UInt16[32];
           cfproductInfo.updateTimeEnd = new UInt16[32];
           cfproductInfo.pUserData = new byte[496];
           cfproductInfo.nUserDataLen = 0;
           cfproductInfo.retcode = -1;
           CF_GetProductInfo(ref cfproductInfo);
           return cfproductInfo;
       }
       #endregion

    }
}
