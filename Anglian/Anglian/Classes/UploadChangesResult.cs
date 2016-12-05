using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
namespace Anglian.Classes
{
    public class UploadChangesResult
    {
        public DateTime ActivitiesTable_ModDate;

        public ObservableCollection<RealtimeNoteKeyValues> NoteValues;

        public DateTime ProjTable_ModDate;

        public bool bSuccessfull;
    }
}
