using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FWS.utility
{
    internal class WindowFormUtility
    {
       /// <summary>
       /// 检查当前打开的窗体中  目标窗体是否已经打开
       /// </summary>
       /// <param name="WinName"> 窗体名 例：项目名.窗体名</param>
       /// <returns></returns>
        public static bool CheckFormIsExist(string WinName)
        {
            foreach (Window win in Application.Current.Windows) //获取当前应用程序实例化窗口
            {
                string type = win.GetType().ToString();
                if (type == WinName)
                {
                    win.Activate();
                    return true;
                }
            }
            return false;
        }
    }
}
