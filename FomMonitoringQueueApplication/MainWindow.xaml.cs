using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Events;
using FomMonitoringCoreQueue.QueueConsumer;

namespace FomMonitoringQueueApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ConsumerInitializer ConsumerInitializer;
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            ConsumerInitializer = new ConsumerInitializer();
            ConsumerInitializer.MessageLogged += ConsumerInitializer_MessageLogged;
        }
        private void ConsumerInitializer_MessageLogged(object sender, LoggerEventsQueue e)
        {
            new Thread(()=>
            {
                LogService.WriteLog(e.Message, e.TypeLevel, e.Exception);
                switch (e.Type)
                {
                    case TypeEvent.Variable:
                        TextVariabili.Dispatcher?.Invoke(() =>
                        {
                            TextVariabili.Text += $"\n {e.Message}";
                            TextVariabili.UpdateLayout();
                        });

                        break;
                    case TypeEvent.Messages:
                        TextMessaggi.Dispatcher?.Invoke(() => {
                            TextMessaggi.Text += $"\n {e.Message}";
                            TextMessaggi.UpdateLayout();
                        });
                        break;
                    case TypeEvent.HistoryBarJobPiece:
                        TextHistoryJobPieceBar.Dispatcher?.Invoke(() =>
                        {
                            TextHistoryJobPieceBar.Text += $"\n {e.Message}";
                            TextHistoryJobPieceBar.UpdateLayout();
                        });
                        break;
                    case TypeEvent.State:
                        TextState.Dispatcher?.Invoke(() => {
                            TextState.Text += $"\n {e.Message}";
                            TextState.UpdateLayout();
                        });
                        break;
                    case TypeEvent.Info:
                        TextInfo.Dispatcher?.Invoke(() =>
                        {
                            TextInfo.Text += $"\n {e.Message}";
                            TextInfo.UpdateLayout();
                        }
                        );
                        break;
                    case TypeEvent.Tool:
                        TextTool.Dispatcher?.Invoke(() => {
                            TextTool.Text += $"\n {e.Message}";
                            TextTool.UpdateLayout();
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }).Start();
            

        }
    }
}
