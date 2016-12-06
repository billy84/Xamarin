using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anglian.Classes
{
    public class NotesHistory
    {
        /// <summary>
        /// 
        /// </summary>
        public string NoteText { get; set; }

        /// <summary>
        /// Date time note added
        /// </summary>
        public DateTime InputDateTime { get; set; }


        /// <summary>
        /// Name of user who created.
        /// </summary>        
        public string UserName { get; set; }

        /// <summary>
        /// List view with
        /// </summary>
        public double ListViewWidth { get; set; }

    }
}
