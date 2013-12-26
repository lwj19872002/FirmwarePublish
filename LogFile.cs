using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FirmwarePublish
{
    class LogFile
    {
        private string m_strFileFullName;

        public LogFile(string strFileFullName)
        {
            m_strFileFullName = strFileFullName;
        }

        public void WriteLog(string strMSG)
        {
            StreamWriter SW = new StreamWriter(m_strFileFullName, true);

            SW.WriteLine(DateTime.Now.ToLocalTime().ToString() + ": " + strMSG);
            SW.Flush();

            SW.Close();
        }
    }
}
