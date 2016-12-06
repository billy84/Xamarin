using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;
using Anglian.Models;
using Xamarin.Forms;
using Anglian.Engine;
namespace Anglian.Views
{
    public partial class AddNotePage : ContentPage
    {
        private List<cProjectNotesTable> m_cProjectNotes = null;
        private SurveyInputResult m_cProjectData = null;
        public AddNotePage(SurveyInputResult ProjectInfo)
        {
            InitializeComponent();
            m_cProjectData = ProjectInfo;
            Title = ProjectInfo.ProjectNo + " - " + ProjectInfo.ProjectName;
            this.m_cProjectNotes = Main.p_cDataAccess.GetSubProjectNotesData(ProjectInfo.SubProjectNo);
        }
        /// <summary>
        /// Add new note.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddNewNote_Click(object sender, EventArgs e)
        {

            try
            {
                this.txtNewNote.Text = this.txtNewNote.Text.Trim();
                if (this.txtNewNote.Text.Length > 0)
                {
                    string sNoteText = this.txtNewNote.Text;

                    //v1.0.1 - Add notes the notes collection
                    cProjectNotesTable cNote = Settings.ReturnNoteObject(
                        this.m_cProjectData.SubProjectNo, 
                        sNoteText, DateTime.Now, 
                        Settings.p_sProjectNoteType_General);
                    this.m_cProjectNotes.Add(cNote);

                    this.txtNewNote.Text = String.Empty;
                    this.txtNewNote.Focus();

                    this.RefreshNotesList();
                }


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }
        /// <summary>
        /// Refresh notes list.
        /// </summary>
        private void RefreshNotesList()
        {

            try
            {

                //Create new instance, we need to bind to list view.
                List<NotesHistory> cNotes = new List<NotesHistory>();
                NotesHistory cNote = null;

                //v1.0.1 - Order by input date time.
                var oResult = (from oCols in this.m_cProjectNotes
                               orderby oCols.InputDateTime descending
                               select oCols);


                foreach (cProjectNotesTable cProjNote in oResult)
                {
                    cNote = new NotesHistory();
                    cNote.InputDateTime = cProjNote.InputDateTime;
                    cNote.NoteText = cProjNote.NoteText;
                    cNote.UserName = cProjNote.UserName;
                    cNotes.Add(cNote);

                }

                //v1.0.1 - Update notes.
                this.lvNotes.ItemsSource = null;
                this.lvNotes.ItemsSource = cNotes;
                //this.lvNotes.UpdateLayout();

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }
    }
}
