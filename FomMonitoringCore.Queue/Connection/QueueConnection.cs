using System;
using System.Configuration;
using System.Threading;
using RabbitMQ.Client;

namespace FomMonitoringCore.Queue.Connection
{
    public class QueueConnection :  IQueueConnection
    {
        public IConnection Connection { get; set; }
        public IModel ChannelVariableList { get; set; }
        public IModel ChannelInfo { get; set; }
        public IModel ChannelState { get; set; }
        public IModel ChannelMessages { get; set; }
        public IModel ChannelTool { get; set; }
        public IModel ChannelHistoryJobPieceBar { get; set; }

        public QueueConnection()
        {
            Connect();
        }

        private void Connect()
        {
            var rabbitHost = ConfigurationManager.AppSettings["RabbitMqHost"];
            var rabbitUsername = ConfigurationManager.AppSettings["RabbitMqUsername"];
            var rabbitPassword = ConfigurationManager.AppSettings["RabbitMqPassword"];

            var factory = new ConnectionFactory
            {
                HostName = rabbitHost,
                UserName = rabbitUsername,
                Password = rabbitPassword,
                AutomaticRecoveryEnabled = true,
                RequestedHeartbeat = 30

            };

            Connection = factory.CreateConnection();
            Connection.ConnectionShutdown += Connection_ConnectionShutdown;
            ChannelVariableList = Connection.CreateModel();
            ChannelInfo = Connection.CreateModel();
            ChannelState = Connection.CreateModel();
            ChannelMessages = Connection.CreateModel();
            ChannelTool = Connection.CreateModel();
            ChannelHistoryJobPieceBar = Connection.CreateModel();


            ChannelVariableList.QueueDeclare("VariableList",
                true,
                false,
                false,
                null);
            ChannelInfo.QueueDeclare("Info",
                true,
                false,
                false,
                null);
            ChannelState.QueueDeclare("State",
                true,
                false,
                false,
                null);
            ChannelMessages.QueueDeclare("Messages",
                true,
                false,
                false,
                null);
            ChannelTool.QueueDeclare("Tool",
                true,
                false,
                false,
                null);
            ChannelHistoryJobPieceBar.QueueDeclare("HistoryJobPieceBar",
                true,
                false,
                false,
                null);
        }

        private void Cleanup()
        {
                ChannelVariableList = Connection.CreateModel();
                ChannelInfo = Connection.CreateModel();
                ChannelState = Connection.CreateModel();
                ChannelMessages = Connection.CreateModel();
                ChannelTool = Connection.CreateModel();
                ChannelHistoryJobPieceBar = Connection.CreateModel();


                if (ChannelVariableList != null && ChannelVariableList.IsOpen)
                {
                    ChannelVariableList.Close();
                    ChannelVariableList.Dispose();
                    ChannelVariableList = null;
                }


                if (ChannelInfo != null && ChannelInfo.IsOpen)
                {
                    ChannelInfo.Close();
                    ChannelInfo.Dispose();
                    ChannelInfo = null;
                }

                if (ChannelState != null && ChannelState.IsOpen)
                {
                    ChannelState.Close();
                    ChannelState.Dispose();
                    ChannelState = null;
                }

                if (ChannelMessages != null && ChannelMessages.IsOpen)
                {
                    ChannelMessages.Close();
                    ChannelMessages.Dispose();
                    ChannelMessages = null;
                }

                if (ChannelTool != null && ChannelTool.IsOpen)
                {
                    ChannelTool.Close();
                    ChannelTool.Dispose();
                    ChannelTool = null;
                }

                if (ChannelHistoryJobPieceBar != null && ChannelHistoryJobPieceBar.IsOpen)
                {
                    ChannelHistoryJobPieceBar.Close();
                    ChannelHistoryJobPieceBar.Dispose();
                    ChannelHistoryJobPieceBar = null;
                }


                if (Connection != null && Connection.IsOpen)
                {
                    Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                }
            
        }


        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Connection broke!");

            Reconnect();
        }

        public void Reconnect()
        {
            Cleanup();

            var mres = new ManualResetEventSlim(false); // state is initially false

            while (!mres.Wait(3000)) // loop until state is true, checking every 3s
            {
                try
                {
                    Connect();
                    
                    mres.Set(); // state set to true - breaks out of loop
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void Dispose()
        {
            Cleanup();
        }
    }
}
