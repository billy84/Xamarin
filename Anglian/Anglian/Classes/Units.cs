using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Anglian.Classes
{
    public class Units : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Unit field
        /// </summary>
        public int UnitNo { get; set; }

        /// <summary>
        /// Design field
        /// </summary>
        public string Design { get; set; }

        /// <summary>
        /// Location field
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Is Installed
        /// </summary>
        private bool _Installed;
        public bool Installed
        {
            get { return _Installed; }
            set
            {
                _Installed = value;
                RaisePropertyChanged("Installed");
            }
        }
    }
}
