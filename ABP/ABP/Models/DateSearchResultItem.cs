using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABP.Models
{
    public class DateSearchResultItem
    {
        public DateSearchResultItem()
        {

        }

        public string Address { get; set; }
        public string Postcode { get; set; }
        public string SurveyPlanDate { get; set; }
        public string ProgressStatus { get; set; }
        public string ConfirmedFlag { get; set; }
    }
}
