using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomMonitoringQueueApplication
{
    sealed class MyViewModel : INotifyPropertyChanged
    {
        private string _info = string.Empty;
        private string _variabili = string.Empty;
        private string _message = string.Empty;
        private string _stati = string.Empty;
        private string _historyJobPieceBar = string.Empty;
        private string _tools = string.Empty;
        private string _errors = string.Empty;

        public string TextErrors
        {
            get { return _errors; }
            set
            {
                if (_errors != value)
                {
                    _errors = value;
                    OnPropertyChange("TextErrors");
                }
            }
        }

        public string TextTool
        {
            get { return _tools; }
            set
            {
                if (_tools != value)
                {
                    _tools = value;
                    OnPropertyChange("TextTool");
                }
            }
        }

        public string TextInfo
        {
            get { return _info; }
            set
            {
                if (_info != value)
                {
                    _info = value;
                    OnPropertyChange("TextInfo");
                }
            }
        }
        public string TextVariabili
        {
            get { return _variabili; }
            set
            {
                if (_variabili != value)
                {
                    _variabili = value;
                    OnPropertyChange("TextVariabili");
                }
            }
        }

        public string TextMessaggi
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChange("TextMessaggi");
                }
            }
        }

        public string TextHistoryJobPieceBar
        {
            get { return _historyJobPieceBar; }
            set
            {
                if (_historyJobPieceBar != value)
                {
                    _historyJobPieceBar = value;
                    OnPropertyChange("TextHistoryJobPieceBar");
                }
            }
        }

        public string TextState
        {
            get { return _stati; }
            set
            {
                if (_stati != value)
                {
                    _stati = value;
                    OnPropertyChange("TextState");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
