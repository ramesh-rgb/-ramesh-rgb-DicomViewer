using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ImageViewer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {  

        private void Application_StartUp(object sender, StartupEventArgs e)
        {

            if(e.Args.Length>0)
            {
                
                MainWindow main = new MainWindow(e.Args[0]);
                ApplicationContext.mainWindow=main;
                main.Show();
            }
            else
            {
                MainWindow main = new MainWindow();
                main.Show();
            }
        }
    }
}


