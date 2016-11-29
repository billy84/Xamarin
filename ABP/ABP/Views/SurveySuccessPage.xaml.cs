using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ABP.WcfProxys;
using ABP.Models;
using ABP.TableModels;

namespace ABP.Views
{
    public partial class SurveySuccessPage : TabbedPage
    {
        private cProjectTable m_cProjectData = null;
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
                Text = "Copy",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "copy"),
                Command = new Command(() => CopyBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Paste",
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
                Text = "Copy",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "copy"),
                Command = new Command(() => CopyBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Paste",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "save"),
                Command = new Command(() => SaveBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Note",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "upload"),
                Command = new Command(() => UploadBtn_Tapped())
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
            // navigation camera
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new PhotoScreenPage(Title)));
        }
        private void UploadBtn_Tapped()
        {
            // add note
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new AddNotePage(Title)));
        }
        private async void SaveBtn_Tapped()
        {
            try
            {
                if (cMain.p_cSurveyInputCopiedSelections != null)
                {
                    if (cMain.p_cSurveyInputCopiedSelections.ProjectNo.Equals(this.m_cProjectData.ProjectNo, StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        StringBuilder sbMsg = new StringBuilder();
                        sbMsg.Append("Are you sure you want to paste in the selections from the following sub-project:");
                        sbMsg.Append(Environment.NewLine);
                        sbMsg.Append(Environment.NewLine);
                        sbMsg.Append(cMain.p_cSurveyInputCopiedSelections.SubProjectNo);
                        sbMsg.Append(Environment.NewLine);
                        sbMsg.Append(cMain.p_cSurveyInputCopiedSelections.DeliveryStreet);
                        var answer = await DisplayAlert("Paste Selections", sbMsg.ToString(), "Yes", "No");
                        if (answer == true)
                        {
                            int? iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPropertyType));
                            this.cmbPropertyType.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;
                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbFloorLevel));
                            this.cmbFloorLevel.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;
                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbInstallationType));
                            this.cmbInstallationType.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbAsbestosPresumed));
                            this.cmbAsbestosPresumed.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbAccessEquipment));
                            this.cmbAccessEquipment.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPermanentGasVent));
                            this.cmbPermanentGasVent.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbWindowboard));
                            this.cmbWindowboard.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbStructuralFaults));
                            this.cmbStructuralFaults.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbServicesToMove));
                            this.cmbServicesToMove.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbDisabledAdaptionsRqd));
                            this.cmbDisabledAdaptionsRqd.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbDoorChoiceFormRcvd));
                            this.cmbDoorChoiceFormRcvd.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbInternalDamage));
                            this.cmbInternalDamage.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbWorkAccessRestrictions));
                            this.cmbWorkAccessRestrictions.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPublicProtection));
                            this.cmbPublicProtection.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void CopyBtn_Tapped()
        {
            try
            {
                cMain.p_cSurveyInputCopiedSelections = new cProjectTable();
                cMain.p_cSurveyInputCopiedSelections.ProjectNo = this.m_cProjectData.ProjectNo;
                cMain.p_cSurveyInputCopiedSelections.ProjectName = this.m_cProjectData.ProjectName;
                cMain.p_cSurveyInputCopiedSelections.SubProjectNo = this.m_cProjectData.SubProjectNo;
                cMain.p_cSurveyInputCopiedSelections.SubProjectName = this.m_cProjectData.SubProjectName;
                cMain.p_cSurveyInputCopiedSelections.DeliveryStreet = this.m_cProjectData.DeliveryStreet;

                cMain.p_cSurveyInputCopiedSelections.PropertyType = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPropertyType));
                cMain.p_cSurveyInputCopiedSelections.ABPAXFloorLevel = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbFloorLevel));
                cMain.p_cSurveyInputCopiedSelections.ABPAXInstallationType = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbInstallationType));
                cMain.p_cSurveyInputCopiedSelections.ABPAXAsbestosPresumed = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbAsbestosPresumed));
                cMain.p_cSurveyInputCopiedSelections.ABPAXAccessEquipment = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbAccessEquipment));
                cMain.p_cSurveyInputCopiedSelections.ABPAXPermanentGasVent = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPermanentGasVent));
                cMain.p_cSurveyInputCopiedSelections.ABPAXWindowBoard = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbWindowboard));
                cMain.p_cSurveyInputCopiedSelections.ABPAXStructuralFaults = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbStructuralFaults));
                cMain.p_cSurveyInputCopiedSelections.ABPAXServicesToMove = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbServicesToMove));
                cMain.p_cSurveyInputCopiedSelections.DisabledAdaptionsRequired = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbDisabledAdaptionsRqd));
                cMain.p_cSurveyInputCopiedSelections.MxmDoorChoiceFormReceived = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbDoorChoiceFormRcvd));
                cMain.p_cSurveyInputCopiedSelections.ABPAXInternalDamage = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbInternalDamage));
                cMain.p_cSurveyInputCopiedSelections.ABPAXWorkAccessRestrictions = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbWorkAccessRestrictions));
                cMain.p_cSurveyInputCopiedSelections.ABPAXPublicProtection = Convert.ToInt32(cMain.ReturnComboSelectedTagValue(this.cmbPublicProtection));
            }
            catch (Exception ex)
            {

            }
        }
    }
}
