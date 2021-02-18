using System;
using Command;
using SNetwork;
using System.Data.SqlClient;

namespace Manager
{
    public partial class Manager
    {
        public bool Run(string scriptName)
        {
            m_szINI = scriptName;
            if (!LoadINI()) return false;
            if (!CreateDB()) return false;
            if (!ListenNetwork()) return false;
            if (!CmdNetwork()) return false;
            return true;
        }

        private bool CmdNetwork()
        {
            ProCommand cmd = new ProCommand(20000);

            if (m_cmdUse)
            {
                if (!cmd.OnStart())
                {
                    Information("Processing Command ..");
                    Information("failed\n");
                    return false;
                }
                else
                {
                    Information("Processing Command ..");
                    Information("done\n");
                    return true;

                }
            }
            else
            {
                Information("ProCommand OFF\n");
                return true;
            }
        }

        private bool ListenNetwork()
        {
            Network network = new Network(m_port);
            if (network.Start())
            {
                Information("Listening TCP ..");

                Information("done\n");
                return true;
            }
            Information("Listening TCP ..");

            Information("failed\n");
            return false;
        }

        private bool LoadINI()
        {
            //---------------------------------
            //        ini Information
            //---------------------------------

            IniFile ini = new IniFile();
            try
            {
                ini.Load(m_szINI);
                m_port = int.TryParse(ini["Default"]["Port"].ToString(), out int _) ? ini["Default"]["Port"].ToInt() : 80;

                m_dbIP = ini["SQL"]["IP"].ToString();
                m_dbName = ini["SQL"]["Database"].ToString();
                m_dbID = ini["SQL"]["ID"].ToString();
                m_dbPW = ini["SQL"]["PW"].ToString();
                m_dbPort = int.TryParse(ini["SQL"]["Port"].ToString(), out int _) ? ini["SQL"]["Port"].ToInt() : 1433;
                m_cmdPort = int.TryParse(ini["Command"]["Port"].ToString(), out int _) ? ini["Command"]["Port"].ToInt() : 20000;
                m_cmdUse = ini["Command"]["UseCmd"].ToBool();

            }
            catch (Exception e)
            {
                Information("Failed To Load INI File {0}", e.Message);
                return false;
            }

            return true;
        }

        private bool CreateDB()
        {
            Information("Create DatabasePool..");
            //--------------------------------
            //     DB Connection
            //--------------------------------
            string connectstrings = $"Data Source={m_dbIP},{m_dbPort}; Initial Catalog={m_dbName}; User ID={m_dbID}; Password={m_dbPW}";
            try
            {
                using (SqlConnection con = new SqlConnection(connectstrings))
                {

                }
            }
            catch (TimeoutException e)
            {
                Information(e.Message);
                return false;
            }
            catch (Exception e)
            {
                Information(e.Message);
                return false;
            }
            finally
            {
            }
            Information("done\n");

            return true;
        }

        public void Information(string format, params object[] args)
        {
            Console.Write($"{format}", args);
        }
    }

    public partial class Manager
    {
        private string m_szINI;

        private int m_port;

        private string m_dbIP;
        private string m_dbName;
        private string m_dbID;
        private string m_dbPW;
        private int m_dbPort;

        private int m_cmdPort;
        private bool m_cmdUse;
    }
}
