using System;
using System.Windows;

namespace VideoSearch.Utils
{
    public class ExceptionUtils
    {
        public static void ShowMessage(String caption, Exception exception)
        {
            if(exception != null)
            {
                String strMsg = exception.Message;

                if(strMsg != null && !strMsg.Contains("Thread was being aborted"))
                    MessageBox.Show(exception.Message, caption);
            }
        }
    }
}
