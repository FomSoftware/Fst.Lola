using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using FomMonitoringCore.Service;
using FomMonitoringCoreQueue.Events;
using FomMonitoringCoreQueue.QueueConsumer;
using Mapster;

namespace FomMonitoringQueueApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static Timer _aTimer;
        public static ConsumerInitializer ConsumerInitializer;

        public MainWindow()
        {
            InitializeComponent();

            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetAssembly(typeof(FomMonitoringCore.Framework.Config.MapsterConfig)));
            Init();
            SetTimer();
        }

        private void Init()
        {
            if (ConsumerInitializer == null)
            {

                ConsumerInitializer = new ConsumerInitializer();
                ConsumerInitializer.MessageLogged += ConsumerInitializer_MessageLogged;
            }
        }

        private void SetTimer()
        {
            if (_aTimer == null)
            {
                _aTimer = new Timer(180000);
                // Hook up the Elapsed event for the timer. 
                _aTimer.Elapsed += OnTimedEvent;
                _aTimer.AutoReset = true;
                _aTimer.Enabled = true;
            }
        }
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            ConsumerInitializer?.Reconnect();
        }

        public void UpdateTextBlock(TextBlock textBlock, string message)
        {
            Dispatcher?.BeginInvoke(new Action(() =>
            {
                textBlock.Text += $"\n {message}";
                textBlock.UpdateLayout();
            }), DispatcherPriority.Send);
        }

        private void ConsumerInitializer_MessageLogged(object sender, LoggerEventsQueue e)
        {
            
            LogService.WriteLog(e.Message, e.TypeLevel, e.Exception);
                switch (e.Type)
                {
                    case TypeEvent.Variable:
                        Task.Factory.StartNew(() => UpdateTextBlock(TextVariabili, e.Message));

                        break;
                    case TypeEvent.Messages:

                        Panel.Dispatcher?.BeginInvoke(DispatcherPriority.Send, new Action(() => {
                            TextMessaggi.Text += $"\n {e.Message}";
                        })).Wait();
                        break;
                    case TypeEvent.HistoryBarJobPiece:

                        Panel.Dispatcher?.BeginInvoke(DispatcherPriority.Send, new Action(() => {
                            TextHistoryJobPieceBar.Text += $"\n {e.Message}";
                            TextHistoryJobPieceBar.UpdateLayout();
                        }));
                        break;
                    case TypeEvent.State:
                        Panel.Dispatcher?.BeginInvoke( DispatcherPriority.Send, new Action(() => {
                            TextState.Text += $"\n {e.Message}";
                        })).Wait();
                        break;
                    case TypeEvent.Info:
                        Panel.Dispatcher?.BeginInvoke(DispatcherPriority.Send, new Action(() => {
                            TextInfo.Text += $"\n {e.Message}";
                            TextInfo.UpdateLayout();
                        }));
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
            

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextInfo.UpdateLayout();
            TextState.UpdateLayout();
            TextHistoryJobPieceBar.UpdateLayout();
            TextMessaggi.UpdateLayout();
            TextVariabili.UpdateLayout();
            TextTool.UpdateLayout();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ConsumerInitializer.Reconnect();
        }
    }
}
