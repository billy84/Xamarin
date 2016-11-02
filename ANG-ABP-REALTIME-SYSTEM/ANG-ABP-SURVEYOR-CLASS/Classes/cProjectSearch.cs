using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ANG_ABP_SURVEYOR_APP_CLASS.Classes
{
    public class cProjectSearch : NotifyPropertyChangedBase

    {

        /// <summary>
        /// Project number field
        /// </summary>
        public string ProjectNo { get; set; }

        /// <summary>
        /// Project name field
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Project status field
        /// </summary>
        public string ProjectStatus { get; set; }

        /// <summary>
        /// Sub project quantity
        /// </summary>
        public int SubProjectQty { get; set; }

     

        /// <summary>
        /// Background field
        /// </summary>
        public string Background { get; set; }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// v1.0.2 - Sub project quantity text display.
        /// </summary>
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


        /// <summary>
        /// Is Enabled
        /// </summary>
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



        /// <summary>
        /// List view width
        /// </summary>
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
