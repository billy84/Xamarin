using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anglian.Classes
{
    public class FetchSurveyFailedReasonsResult
    {
        public DateTime bLastUpdateDate;

        public bool bSuccessfull;
        public ObservableCollection<SurveyFailedReason> sfrReasons;
    }
}
