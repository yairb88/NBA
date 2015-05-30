using log4net;
using log4net.Config;
using NbaMainWindow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


namespace Nba
{
    class Program
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));


       [STAThread]
       public static void Main(string[] args)
        {
            Log4NetInitializer.Init();
            log.InfoFormat("Udi Kabudi Industries proud to present");
            Application app = new Application();
            app.Run(new MainWindow());
        }

    }
}
