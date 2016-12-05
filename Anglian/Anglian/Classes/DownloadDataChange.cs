using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anglian.Classes
{
    public class DownloadDataChange
    {
        public DateTime ActivitiesTable_ModDate;

        public DateTime Delivery_ModDate;

        public ObservableCollection<RealtimeNoteKeyValues> Notes;

        public DateTime ProjTable_ModDate;

        public ObservableCollection<UnitDetails> Units;

        public string sProjectNo;

        public string sSubProjectNo;
    }
}
