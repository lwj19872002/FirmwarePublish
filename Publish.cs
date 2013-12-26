using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FirmwarePublish
{
    class Publish
    {
        public bool DoPublish(string strConfigFile, string strLogFile)
        {
            string strTemp;
            string strSrcHFullPath;
            char[] acTemp = new char[25];
            string strTemp2;
            string strSWVer;
            char[] acSWVer = new char[9];
            string strHWVer;
            char[] acHWVer = new char[5];

            string strTarFileName;
            string strFileType;
            string strTarFilePath;
            string strTarFileSubPath;
            string strSrcBinName;
            string strSrcBinPath;

            string strSrcFileFullName;
            string strTarFileFullName;

            // 获取当前路径
            string strCurrentPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            IniFile ConfigIniFile = new IniFile(strCurrentPath + strConfigFile);
            LogFile RunLog = new LogFile(strCurrentPath + strLogFile);

            string strLogTemp;

            // 如果没有配置文件，生成一个默认的配置文件
            if (!File.Exists(strCurrentPath + strConfigFile))
            {
                ConfigIniFile.WriteValue("Setting", "FileType", ".bin");
                ConfigIniFile.WriteValue("Setting", "SourceBinName", "SourceFile");
                ConfigIniFile.WriteValue("Setting", "TargetBinName", "TargetFile");
                ConfigIniFile.WriteValue("Setting", "SourceBinPath", strCurrentPath);
                ConfigIniFile.WriteValue("Setting", "SourceHFullPath", strCurrentPath + "systemconfig.h");
                ConfigIniFile.WriteValue("Setting", "TargetBinPath", strCurrentPath);
            }

            // 读取配置信息
            strFileType = ConfigIniFile.ReadValue("Setting", "FileType");
            strLogTemp = "FileType=" + strFileType;
            RunLog.WriteLog(strLogTemp);

            strSrcBinName = ConfigIniFile.ReadValue("Setting", "SourceBinName");
            strLogTemp = "SourceBinName=" + strSrcBinName;
            RunLog.WriteLog(strLogTemp);

            strTarFileName = ConfigIniFile.ReadValue("Setting", "TargetBinName");
            strLogTemp = "TargetBinName=" + strTarFileName;
            RunLog.WriteLog(strLogTemp);

            strSrcBinPath = ConfigIniFile.ReadValue("Setting", "SourceBinPath");
            strLogTemp = "SourceBinPath=" + strSrcBinPath;
            RunLog.WriteLog(strLogTemp);

            strSrcHFullPath = ConfigIniFile.ReadValue("Setting", "SourceHFullPath");
            strLogTemp = "SourceHFullPath=" + strSrcHFullPath;
            RunLog.WriteLog(strLogTemp);

            strTarFilePath = ConfigIniFile.ReadValue("Setting", "TargetBinPath");
            strLogTemp = "TargetBinPath=" + strTarFilePath;
            RunLog.WriteLog(strLogTemp);

            // 检查需要读取的H头文件是否存在
            if (!File.Exists(strSrcHFullPath))
            {
                strLogTemp = "The file <" + strSrcHFullPath + "> is not here!";
                RunLog.WriteLog(strLogTemp);
                return false;
            }

            // 解析H头文件，找到需要的信息
            StreamReader SRHFile = new StreamReader(strSrcHFullPath);
            strSWVer = null;
            strHWVer = null;
            while (true)
            {
                strTemp = SRHFile.ReadLine();
                if (strTemp == null)
                {
                    break;
                }

                if (strTemp.Length >= 37)
                {
                    strTemp.CopyTo(0, acTemp, 0, 25);

                    strTemp2 = new string(acTemp);

                    // 找软件版本号
                    if (strTemp2.Equals("#define SYSCFG_SW_VERSION"))
                    {
                        strTemp.CopyTo(27, acSWVer, 0, 9);
                        strSWVer = new string(acSWVer);

                        strLogTemp = "SWVer = " + strSWVer;
                        RunLog.WriteLog(strLogTemp);
                    }

                    // 找硬件版本号
                    if (strTemp2.Equals("#define SYSCFG_HW_CURVER "))
                    {
                        strTemp.CopyTo(35, acHWVer, 0, 5);
                        strHWVer = new string(acHWVer);

                        strLogTemp = "HWVer = " + strHWVer;
                        RunLog.WriteLog(strLogTemp);
                    }
                }
            }

            if ((strSWVer == null) || (strHWVer == null))
            {
                RunLog.WriteLog(@"SWVer or HWVer is null!");
                return false;
            }

            strTarFileSubPath = strSWVer + "_" + strHWVer;
            RunLog.WriteLog("Sub path of target file : " + strTarFileSubPath);

            if (!Directory.Exists(strTarFilePath + strTarFileSubPath))
            {
                Directory.CreateDirectory(strTarFilePath + strTarFileSubPath);
            }

            strSrcFileFullName = strSrcBinPath + strSrcBinName + strFileType;
            strTarFileFullName = strTarFilePath + strTarFileSubPath + "\\" + strTarFileName + strFileType;
            RunLog.WriteLog("Source file full name : " + strSrcFileFullName);
            RunLog.WriteLog("Target file full name : " + strTarFileFullName);

            // 拷贝文件，生成新文件
            File.Copy(strSrcFileFullName, strTarFileFullName, true);

            RunLog.WriteLog(">>>>>>>>>All done!<<<<<<<<<");

            return true;
        }
    }
}
