using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CF2025.Base.DAL
{
    public class CommonDefine
    {
        public CommonDefine()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 取报表服务器地址
        /// </summary>
        /// <returns></returns>
        public static string getReportServerUrl()
        {
            return ConfigurationManager.AppSettings.Get("ReportServer").ToString();

        }

        /// <summary>
        /// 取报表项目名
        /// </summary>
        /// <returns></returns>
        public static string getReportProjectName()
        {
            return ConfigurationManager.AppSettings.Get("ReportProjectName").ToString();
        }




    }
}
