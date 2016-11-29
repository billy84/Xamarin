using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ABP.Views
{
    public partial class SurveySuccessPage : TabbedPage
    {
        public SurveySuccessPage(string ProjectInfo)
        {
            InitializeComponent();
            Title = ProjectInfo;
            AddToolBarForCustomerPage();
            this.CurrentPageChanged += SurveySuccessPage_CurrentPageChanged;
        }
        private void AddToolBarForCustomerPage()
        {
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Camera",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "camera"),
                Command = new Command(() => CameraBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Edit",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "find"),
                Command = new Command(() => EditBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Save",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "save"),
                Command = new Command(() => SaveBtn_Tapped())
            });
        }
        private void AddToolBarForHealthPage()
        {
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Camera",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "camera"),
                Command = new Command(() => CameraBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Edit",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "find"),
                Command = new Command(() => EditBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Save",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "save"),
                Command = new Command(() => SaveBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Copy",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "copy"),
                Command = new Command(() => CopyBtn_Tapped())
            });
        }
        private void SurveySuccessPage_CurrentPageChanged(object sender, EventArgs e)
        {
            var i = this.Children.IndexOf(this.CurrentPage);
            if (i == 0)
            {
                this.ToolbarItems.Clear();
                AddToolBarForCustomerPage();
            }
            else
            {
                this.ToolbarItems.Clear();
                AddToolBarForHealthPage();
            }
            //throw new NotImplementedException();
        }

        private void CameraBtn_Tapped()
        {

        }
        private void EditBtn_Tapped()
        {

        }
        private void SaveBtn_Tapped()
        {

        }
        private void CopyBtn_Tapped()
        {

        }
    }
}
