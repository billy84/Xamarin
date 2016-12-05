using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Anglian.Classes
{
    public class ProjectSearch : NotifyPropertyChangedBase
    {
        public string ProjectNo { get; set; }
        public string ProjectName { get; set; }
        public string ProjectStatus { get; set; }
        public int SubProjectQty { get; set; }
        public string Background { get; set; }

        private string _Status;
        public string Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                RaisePropertyChanged("Status");
            }
        }

        private string _SubProjectQtyDisplay;
        public string SubProjectQtyDisplay
        {
            get { return _SubProjectQtyDisplay; }
            set
            {
                _SubProjectQtyDisplay = value;
                RaisePropertyChanged("SubProjectQtyDisplay");
            }
        }
        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
                RaisePropertyChanged("IsEnabled");
            }
        }
        public double ListViewWidth { get; set; }
    }
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
