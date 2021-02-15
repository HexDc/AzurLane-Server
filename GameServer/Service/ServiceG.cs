using System;
using System.IO;

namespace Service
{
    partial class ServiceG
    {

        public ServiceG(string[] args)
        {
            m_arguments = args;
        }

        public void ServiceMainProc()
        {
            if (m_arguments.Length > 0)
            {
                if (m_arguments[0] == "-c")
                {
                    Debug("service : console\r\n");

                    string fullPath = Directory.GetCurrentDirectory() + "\\" + m_arguments[1];
                    m_arguments[1] = fullPath;

                    ServiceStart();
                }
                else
                {
                    Debug("USAGE : \r\n");
                    Debug("	    -c------------Run as service\r\n");
                    Debug("EXAMPLE : \r\n");
                    Debug("GameServer.exe -c GameServer.ini");
                }
            }
            else
            {
                Debug("WRONG EXCUTE\n");
            }
        }

        private bool ServiceStart()
        {
            switch (m_arguments.Length)
            {
                case 2:
                    m_iniFile = m_arguments[1];
                    break;
                case 3:
                    m_iniFile = m_arguments[1];
                    m_logFile = m_arguments[1];
                    break;
                default:
                    return false;
            }

            Debug("----------------------------------------------------\r\n");
            Debug("-- start service GameServer\r\n");
            Debug(DateTime.Now.ToString("-- [yyyy-MM-dd-HH-mm-ss]\n"));
            Debug("----------------------------------------------------\r\n");

            if (new Manager.Manager().Run(m_iniFile))
            {
                Debug(">> Service Start - Success\r\n");
                return true;
            }
            else
            {
                Debug(">> Service Start - Failed\r\n");
                return false;
            }
        }

        public void Destroy()
        {

        }

        public void Debug(string msg)
        {
            Console.Write(msg);
        }
    }

    partial class ServiceG
    {
        string[] m_arguments;

        string m_iniFile;
        string m_logFile;
    }
}
