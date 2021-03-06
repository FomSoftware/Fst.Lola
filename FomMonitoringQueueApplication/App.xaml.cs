﻿using System.ComponentModel;
using System.Windows;

namespace FomMonitoringQueueApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private bool _isExit;
        

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.Closing += MainWindow_Closing;

            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = FomMonitoringQueueApplication.Properties.Resources.Icon1
            };
            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();
            _notifyIcon.Visible = true;

            CreateContextMenu();
            

        }



        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip =
                new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("MainWindow...").Click += (s, e) => ShowMainWindow();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        private void ExitApplication()
        {
            _isExit = true;
            MainWindow.Hide();
            _notifyIcon.Dispose();
            _notifyIcon = null;
            FomMonitoringQueueApplication.MainWindow.ConsumerInitializer.Dispose();
            FomMonitoringQueueApplication.MainWindow.ConsumerInitializer = null;
            Application.Current.Shutdown();
        }

        private void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                MainWindow.Hide(); // A hidden window can be shown again, a closed one not
            }
            else
            {
                FomMonitoringQueueApplication.MainWindow.ConsumerInitializer.Dispose();
                FomMonitoringQueueApplication.MainWindow.ConsumerInitializer = null;
                Application.Current.Shutdown();
            }


        }
    }
}
