using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nba
{
    
    public static  class Log4NetInitializer
    {
        public static void Init ()
        {
            FileInfo fi = new FileInfo("Log4NetConfiguration.xml");
            XmlConfigurator.Configure(fi);
        }   
    }
}
