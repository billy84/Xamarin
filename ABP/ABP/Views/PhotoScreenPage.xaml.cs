using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Views
{
    public partial class PhotoScreenPage : ContentPage
    {
        public PhotoScreenPage(string ProjectInfo)
        {
            InitializeComponent();
            this.Title = ProjectInfo;
        }
    }
}
