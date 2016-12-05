using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anglian.Classes
{
    public class SubProjectFileDetail
    {
        public string sProjectNo;

        public string sSubProjectNo;

        public ObservableCollection<SubProjectFile> sfFiles;
    }
}
