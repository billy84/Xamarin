using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ANG_ABP_INSTALLER_APP.Views
{
    public sealed partial class InstallerSearch : UserControl
    {

        /// <summary>
        /// Event handler for when an installer has been selected.
        /// </summary>
        public event EventHandler<ANG_ABP_SURVEYOR_APP_CLASS.Model.cInstallersTable> InstallerSelected;

        public InstallerSearch()
        {
            this.InitializeComponent();
        }



        /// <summary>
        /// v1.0.9 - Installers name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInstallerName_TextChanged(object sender, TextChangedEventArgs e)
        {


            try
            {

                this.ProcessSearch();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        /// <summary>
        /// Process search
        /// </summary>
        private void ProcessSearch()
        {

            try
            {

                if (this.txtInstallerName.Text.Length > 0)
                {
                    this.lvInstallers.ItemsSource = cMain.p_cDataAccess.SearchInstallers(this.txtInstallerName.Text);

                }
                else
                {
                    this.lvInstallers.ItemsSource = cMain.p_cDataAccess.FetchAllInstallers();

                }
                

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }

        /// <summary>
        /// Installer selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvInstallers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {

                foreach (ANG_ABP_SURVEYOR_APP_CLASS.Model.cInstallersTable cInstaller in e.AddedItems)
                {

                    this.InstallerSelected(this, cInstaller);
                }

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }

        /// <summary>
        /// Control loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {

                this.ProcessSearch();

            }
            catch (Exception ex)
            {
                cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
    }
}
