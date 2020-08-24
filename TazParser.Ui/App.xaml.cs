using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Taz.Services.Business;
using Taz.Services.Interfaces;

namespace TazParser.Ui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if DEBUG
            SimpleIoc.Default.Register<IEmailService, EmailServiceDebug>();
#else
            SimpleIoc.Default.Register<IEmailService, EmailService>();
#endif

            //var window = new MainWindow();// { DataContext = mainWindowViewModel };
            //window.Show();
        }
    }
}
