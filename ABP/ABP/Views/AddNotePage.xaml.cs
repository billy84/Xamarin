using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Views
{
    public partial class AddNotePage : ContentPage
    {
        public AddNotePage(string ProjectInfo)
        {
            InitializeComponent();
            Title = ProjectInfo;
        }
    }
}
