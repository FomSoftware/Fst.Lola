using System;
using System.Reflection;
using System.Timers;
using System.Windows;
using FomMonitoringCore.Queue.Events;
using FomMonitoringCore.Queue.QueueConsumer;
using FomMonitoringCore.Service;
using Mapster;

namespace FomMonitoringQueueApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static Timer _aTimerQueue;
        public static ConsumerInitializer ConsumerInitializer;
        private static Timer _aTimerUnknown;
        private static Timer _aTimerNotElaborated;
        private static MyViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MyViewModel();
            DataContext = _viewModel;
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
            if (_aTimerQueue == null)
            {
                _aTimerQueue = new Timer(1800000);
                // Hook up the Elapsed event for the timer. 
                _aTimerQueue.Elapsed += OnTimedQueueEvent;
                _aTimerQueue.AutoReset = true;
                _aTimerQueue.Enabled = true;
            }

            if (_aTimerUnknown == null)
            {
                _aTimerUnknown = new Timer(60000);
                // Hook up the Elapsed event for the timer. 
                _aTimerUnknown.Elapsed += OnTimedUnknownEvent;
                _aTimerUnknown.AutoReset = true;
                _aTimerUnknown.Enabled = true;
            }
        }
        private static void OnTimedQueueEvent(object source, ElapsedEventArgs e)
        {
            ConsumerInitializer?.Reconnect();
        }

        private static void OnTimedUnknownEvent(object source, ElapsedEventArgs e)
        {
            ConsumerInitializer?.ElaborateUnknown();
        }


        private void ConsumerInitializer_MessageLogged(object sender, LoggerEventsQueue e)
        {
            
            LogService.WriteLog(e.Message, e.TypeLevel, e.Exception);
                switch (e.Type)
                {
                    case TypeEvent.Variable:
                       _viewModel.TextVariabili += $"\n {e.Message}";
                    break;
                    case TypeEvent.Messages:
                        _viewModel.TextMessaggi += $"\n {e.Message}";
                        break;
                    case TypeEvent.HistoryBarJobPiece:
                        _viewModel.TextHistoryJobPieceBar += $"\n {e.Message}";
                        break;
                    case TypeEvent.State:
                        _viewModel.TextState += $"\n {e.Message}";
                        break;
                    case TypeEvent.Info:
                        _viewModel.TextInfo += $"\n {e.Message}";
                        break;
                    case TypeEvent.Tool:
                        _viewModel.TextTool += $"\n {e.Message}";
                    break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (e.TypeLevel == LogService.TypeLevel.Error || e.TypeLevel == LogService.TypeLevel.Fatal)
                {
                    _viewModel.TextErrors += $"\n {e.Message} \n {e.Exception}";
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
