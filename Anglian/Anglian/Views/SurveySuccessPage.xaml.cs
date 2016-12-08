using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anglian.Classes;
using Anglian.Engine;
using Anglian.Models;
using Xamarin.Forms;

namespace Anglian.Views
{
    public partial class SurveySuccessPage : TabbedPage
    {
        private cProjectTable m_cProjectData = null;
        private SurveyInputResult m_surveyInputResult = null;
        private List<cUpdatesTable> m_cUpdateLog = null;
        private bool m_bUserAppliedSurveyDate = true;
        private List<cProjectNotesTable> m_cProjectNotes = null;
        public SurveySuccessPage(SurveyInputResult ProjectInfo)
        {
            InitializeComponent();
            m_surveyInputResult = ProjectInfo;
            Title = ProjectInfo.ProjectNo + " - " + ProjectInfo.ProjectName;
            AddToolBarForCustomerPage();
            m_cProjectData = Main.p_cDataAccess.GetSubProjectProjectData(ProjectInfo.SubProjectNo);
            m_cProjectNotes = Main.p_cDataAccess.GetSubProjectNotesData(ProjectInfo.SubProjectNo);
            CurrentPageChanged += SurveySuccessPage_CurrentPageChanged;
            PopulateDropDowns();
            DisplayProjectDetails(m_cProjectData);
        }
        /// <summary>
        /// Update special note.
        /// </summary>
        /// <param name="v_sNoteText"></param>
        private void UpdateSpecialNotes(cProjectTable v_sProjectData)
        {

            try
            {

                if (String.IsNullOrEmpty(v_sProjectData.SpecialResidentNote) == true)
                {
                    this.txtSpecialResidentNote.Text = String.Empty;
                    //this.txtSpecialNoteFlyout.Text = String.Empty;
                }
                else
                {
                    this.txtSpecialResidentNote.Text = v_sProjectData.SpecialResidentNote;
                    //this.txtSpecialNoteFlyout.Text = v_sProjectData.SpecialResidentNote;

                }

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
        /// <summary>
        /// Populate drop downs.
        /// </summary>
        private void PopulateDropDowns()
        {

            try
            {

                this.UpdateDropDown(this.cmbPropertyType, "MxmPropertyType");
                this.UpdateDropDown(this.cmbFloorLevel, "ABPAXFloorLevel");
                this.UpdateDropDown(this.cmbInstallationType, "ABPAXInstallationType");
                this.UpdateDropDown(this.cmbAsbestosPresumed, "ABPAXAsbestosPresumed");
                this.UpdateDropDown(this.cmbAccessEquipment, "ABPAXAccessEquipment");
                this.UpdateDropDown(this.cmbPermanentGasVent, "ABPAXPermanentGasVent");
                this.UpdateDropDown(this.cmbWindowboard, "ABPAXWindowBoard");
                this.UpdateDropDown(this.cmbStructuralFaults, "ABPAXStructuralFaults");
                this.UpdateDropDown(this.cmbServicesToMove, "ABPAXServicesToMove");
                this.UpdateDropDown(this.cmbDoorChoiceFormRcvd, "MxmDoorChoiceFormReceived");
                this.UpdateDropDown(this.cmbDisabledAdaptionsRqd, "MxmDisabledAdaptionsRequired");

                this.UpdateDropDown(this.cmbInternalDamage, "ABPAXInternDamage");
                this.UpdateDropDown(this.cmbWorkAccessRestrictions, "ABPAXWrkAccRestrictions");
                this.UpdateDropDown(this.cmbPublicProtection, "ABPAXPublicProtect");

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }

        /// <summary>
        /// Update drop down list.
        /// </summary>
        /// <param name="v_sControlName"></param>
        /// <param name="v_sFieldName"></param>
        private void UpdateDropDown(Picker v_sControlName, string v_sFieldName)
        {

            try
            {

                //Try and locate the combo box on screen.
                Picker cmbCombo = v_sControlName;
                if (cmbCombo != null)
                {


                    //Add please choose to the list.
                    //ComboBoxItem cmbItem = new ComboBoxItem();
                    //cmbItem.Content = Settings.p_sPleaseChoose;
                    //cmbItem.Tag = -1;

                    cmbCombo.Items.Add(Settings.p_sPleaseChoose);

                    //Fetch enumerators and populate.
                    List<cBaseEnumsTable> oEnums = Main.p_cDataAccess.GetEnumsForField(v_sFieldName);
                    foreach (cBaseEnumsTable cEnum in oEnums)
                    {
                        if (cEnum.EnumName.Length > 0)
                        {

                            //cmbItem = new ComboBoxItem();
                            //cmbItem.Content = cEnum.EnumName;
                            //cmbItem.Tag = cEnum.EnumValue;

                            cmbCombo.Items.Add(cEnum.EnumName);
                        }

                    }

                    //Select first item in list.
                    cmbCombo.SelectedIndex = 0;
                    //cmbCombo.Tag = 0; //v1.0.2 - Set combo tag to initial selected index.

                }


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }


        }
        /// <summary>
        /// Display project details on screen.
        /// </summary>
        private void DisplayProjectDetails(cProjectTable v_cProjectData)
        {

            try
            {
                Title = this.m_cProjectData.SubProjectNo + Main.ReturnAddress(this.m_cProjectData);
                //Sub project details.
                //this.tbSubProjectID.Text = this.m_cProjectData.SubProjectNo;
                //this.tbAddress.Text = cMain.ReturnAddress(this.m_cProjectData);

                //Resident details
                this.txtResidentName.Text = Settings.ReturnString(v_cProjectData.ResidentName);
                this.txtResidentTelNo.Text = Settings.ReturnString(v_cProjectData.ResidentTelNo);
                this.txtReplacementType.Text = Settings.ReturnString(v_cProjectData.MxmProjDescription);
                this.txtResidentMobileNo.Text = Settings.ReturnString(v_cProjectData.ResidentMobileNo);
                this.txtAltContactTelNo.Text = Settings.ReturnString(v_cProjectData.AlternativeContactTelNo);
                this.txtAltContactName.Text = Settings.ReturnString(v_cProjectData.AlternativeContactName);
                this.txtAltContactMobNo.Text = Settings.ReturnString(v_cProjectData.AlternativeContactMobileNo);
                this.UpdateSpecialNotes(v_cProjectData);

                //Drop down details
                this.SelectDropDown(this.cmbPropertyType, v_cProjectData.PropertyType, this.m_cProjectData.PropertyType);
                this.SelectDropDown(this.cmbFloorLevel, v_cProjectData.ABPAXFloorLevel, this.m_cProjectData.ABPAXFloorLevel);
                this.SelectDropDown(this.cmbInstallationType, v_cProjectData.ABPAXInstallationType, this.m_cProjectData.ABPAXInstallationType);
                this.SelectDropDown(this.cmbAsbestosPresumed, v_cProjectData.ABPAXAsbestosPresumed, this.m_cProjectData.ABPAXAsbestosPresumed);
                this.SelectDropDown(this.cmbAccessEquipment, v_cProjectData.ABPAXAccessEquipment, this.m_cProjectData.ABPAXAccessEquipment);
                this.SelectDropDown(this.cmbPermanentGasVent, v_cProjectData.ABPAXPermanentGasVent, this.m_cProjectData.ABPAXPermanentGasVent);
                this.SelectDropDown(this.cmbWindowboard, v_cProjectData.ABPAXWindowBoard, this.m_cProjectData.ABPAXWindowBoard);
                this.SelectDropDown(this.cmbStructuralFaults, v_cProjectData.ABPAXStructuralFaults, this.m_cProjectData.ABPAXStructuralFaults);
                this.SelectDropDown(this.cmbServicesToMove, v_cProjectData.ABPAXServicesToMove, this.m_cProjectData.ABPAXServicesToMove);
                this.SelectDropDown(this.cmbDisabledAdaptionsRqd, v_cProjectData.DisabledAdaptionsRequired, this.m_cProjectData.DisabledAdaptionsRequired);
                this.SelectDropDown(this.cmbDoorChoiceFormRcvd, v_cProjectData.MxmDoorChoiceFormReceived, this.m_cProjectData.MxmDoorChoiceFormReceived);

                this.SelectDropDown(this.cmbInternalDamage, v_cProjectData.ABPAXInternalDamage, this.m_cProjectData.ABPAXInternalDamage);
                this.SelectDropDown(this.cmbWorkAccessRestrictions, v_cProjectData.ABPAXWorkAccessRestrictions, this.m_cProjectData.ABPAXWorkAccessRestrictions);
                this.SelectDropDown(this.cmbPublicProtection, v_cProjectData.ABPAXPublicProtection, this.m_cProjectData.ABPAXPublicProtection);



                //Display correct survey input date.
                if (v_cProjectData.MXM1002TrfDate.HasValue == true)
                {
                    this.DisplaySurveyDateOnSurveyButton(v_cProjectData.MXM1002TrfDate);
                }
                else
                {
                    this.DisplaySurveyDateOnSurveyButton(v_cProjectData.EndDateTime);
                }

                //v1.0.21 - If incomplete, do not allow
                if (v_cProjectData.ABPAWHealthSafetyInComplete.HasValue == true)
                {
                    if (v_cProjectData.ABPAWHealthSafetyInComplete.Value == (int)Settings.YesNoBaseEnum.Yes)
                    {

                        //this.btnResidentSignature.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        this.btnSurveyedDate.IsEnabled = false;


                    }

                    //Update page title.
                    //this.Title = "Surveyed Input Detail";

                }


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
        /// <summary>
        /// Update drop down with value.
        /// </summary>
        /// <param name="v_sControlID"></param>
        /// <param name="v_sValue"></param>
        private void SelectDropDown(Picker v_sControlName, int? v_iValue, int? v_iMainValue)
        {

            try
            {

                //If null we go no further.
                if (v_iValue.HasValue == false)
                {
                    return;
                }

                //Try and locate the combo box on screen.
                Picker cmbCombo = v_sControlName;
                if (cmbCombo != null)
                {


                    int iIndex = 0;

                    //Loop through combo box items and find one matching passed values.
                    foreach (string cbItem in cmbCombo.Items)
                    {

                        //v1.0.2 - This is the original value, not a restore value.
                        if (cmbCombo.SelectedIndex == v_iMainValue)
                        {

                            //v1.0.2 - Update initial index
                            cmbCombo.SelectedIndex = (int)v_iMainValue;
                            //cmbCombo.Tag = iIndex;

                        }


                        //If match then select index.
                        if (cmbCombo.SelectedIndex == v_iValue)
                        {

                            cmbCombo.SelectedIndex = iIndex;

                        }

                        //Count through items.
                        iIndex++;

                    }

                }


            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
        }
        /// <summary>
        /// Display survey date on survey date button.
        /// </summary>
        /// <param name="v_dSurveyDate"></param>
        private void DisplaySurveyDateOnSurveyButton(DateTime? v_dSurveyDate)
        {

            try
            {

                if (v_dSurveyDate.HasValue == true)
                {
                    this.btnSurveyedDate.Date = (DateTime)v_dSurveyDate;
                    //this.btnSurveyedDate.Content = "Surveyed date: " + cMain.ReturnDisplayDate(v_dSurveyDate.Value);
                }
                else
                {
                    this.btnSurveyedDate.Date = DateTime.Now;
                    //this.btnSurveyedDate.Content = "Surveyed date: N\\A";
                }

                //this.btnSurveyedDate.Date = (DateTime)v_dSurveyDate;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }
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
                Text = "Note",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "note"),
                Command = new Command(() => NoteBtn_Tapped())
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
                Text = "Note",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "note"),
                Command = new Command(() => NoteBtn_Tapped())
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
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "paste"),
                Command = new Command(() => PasteBtn_Tapped())
            });
            this.ToolbarItems.Add(new ToolbarItem()
            {
                Text = "Save",
                Icon = String.Format("{0}{1}.png", Device.OnPlatform("Icons/", "", "Assets/Icons/"), "save"),
                Command = new Command(() => SaveBtn_Tapped())
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
        private async void SaveBtn_Tapped()
        {
            bool bSaveOK = false;
            try
            {

                bSaveOK = await this.SaveProject();

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);

            }

        }
        /// <summary>
        /// Logs all the changes for saving
        /// </summary>
        /// <returns></returns>
        private bool LogChangesForSaving(bool v_bCheckingForChangesOnly)
        {

            try
            {


                //Create new log table.
                this.m_cUpdateLog = new List<cUpdatesTable>();

                //Check resident name has changed.
                if (this.txtResidentName.Text != this.m_cProjectData.ResidentName)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.ResidentName = this.txtResidentName.Text; }
                    this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmResidentName", this.txtResidentName.Text);
                }

                //Check resident telephone number has changed.
                if (this.txtResidentTelNo.Text != this.m_cProjectData.ResidentTelNo)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.ResidentTelNo = this.txtResidentTelNo.Text; }
                    this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmTelephoneNo", this.txtResidentTelNo.Text);
                }

                //Check resident mobile number has changed.
                if (this.txtResidentMobileNo.Text != this.m_cProjectData.ResidentMobileNo)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.ResidentMobileNo = this.txtResidentMobileNo.Text; }
                    this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmResidentMobileNo", this.txtResidentMobileNo.Text);
                }

                //Check resident alternate contact name has changed.
                if (this.txtAltContactName.Text != this.m_cProjectData.AlternativeContactName)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.AlternativeContactName = this.txtAltContactName.Text; }
                    this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmAlternativeContactName", this.txtAltContactName.Text);
                }

                //Check resident alternate contact telephone number has changed.
                if (this.txtAltContactTelNo.Text != this.m_cProjectData.AlternativeContactTelNo)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.AlternativeContactTelNo = this.txtAltContactTelNo.Text; }
                    this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmAlternativeContactTelNo", this.txtAltContactTelNo.Text);
                }

                //Check resident alternate contact mobile number has changed.
                if (this.txtAltContactMobNo.Text != this.m_cProjectData.AlternativeContactMobileNo)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.AlternativeContactMobileNo = this.txtAltContactMobNo.Text; }
                    this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmAlternativeContactMobileNo", this.txtAltContactMobNo.Text);
                }

                //Check replacement type text has changed.
                if (this.txtReplacementType.Text != this.m_cProjectData.MxmProjDescription)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.MxmProjDescription = this.txtReplacementType.Text; }
                    this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmProjDescription", this.txtReplacementType.Text);
                }

                //Special Resident note.
                if (this.txtSpecialResidentNote.Text != this.m_cProjectData.SpecialResidentNote)
                {
                    if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.SpecialResidentNote = this.txtSpecialResidentNote.Text; }
                    this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmSpecialResidentNote", this.txtSpecialResidentNote.Text);
                }

                //Log combo box changes.

                int? iValue = this.LogComboValueChange(this.cmbPropertyType, "MxmPropertyType", this.m_cProjectData.PropertyType);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.PropertyType = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbFloorLevel, "ABPAXFloorLevel", this.m_cProjectData.ABPAXFloorLevel);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXFloorLevel = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbInstallationType, "ABPAXInstallationType", this.m_cProjectData.ABPAXInstallationType);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXInstallationType = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbAsbestosPresumed, "ABPAXAsbestosPresumed", this.m_cProjectData.ABPAXAsbestosPresumed);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXAsbestosPresumed = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbAccessEquipment, "ABPAXAccessEquipment", this.m_cProjectData.ABPAXAccessEquipment);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXAccessEquipment = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbPermanentGasVent, "ABPAXPermanentGasVent", this.m_cProjectData.ABPAXPermanentGasVent);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXPermanentGasVent = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbWindowboard, "ABPAXWindowBoard", this.m_cProjectData.ABPAXWindowBoard);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXWindowBoard = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbStructuralFaults, "ABPAXStructuralFaults", this.m_cProjectData.ABPAXStructuralFaults);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXStructuralFaults = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbServicesToMove, "ABPAXServicesToMove", this.m_cProjectData.ABPAXServicesToMove);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXServicesToMove = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbDisabledAdaptionsRqd, "MxmDisabledAdaptionsRequired", this.m_cProjectData.DisabledAdaptionsRequired);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.DisabledAdaptionsRequired = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbDoorChoiceFormRcvd, "MxmDoorChoiceFormReceived", this.m_cProjectData.MxmDoorChoiceFormReceived);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.MxmDoorChoiceFormReceived = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbInternalDamage, "ABPAXInternDamage", this.m_cProjectData.ABPAXInternalDamage);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXInternalDamage = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbWorkAccessRestrictions, "ABPAXWrkAccRestrictions", this.m_cProjectData.ABPAXWorkAccessRestrictions);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXWorkAccessRestrictions = iValue;
                }

                iValue = this.LogComboValueChange(this.cmbPublicProtection, "ABPAXPublicProtect", this.m_cProjectData.ABPAXPublicProtection);
                if (v_bCheckingForChangesOnly == false)
                {
                    this.m_cProjectData.ABPAXPublicProtection = iValue;
                }


                //v1.0.21 - Save HS incomplete flag               
                if (this.m_cProjectData.ABPAWHealthSafetyInComplete.HasValue == true)
                {
                    if (this.m_cProjectData.ABPAWHealthSafetyInComplete.Value == (int)Settings.YesNoBaseEnum.Yes)
                    {

                        //This is not a change the user has made so do not include when checking changes only.
                        if (v_bCheckingForChangesOnly == false)
                        {

                            this.m_cProjectData.ABPAWHealthSafetyInComplete = (int)Settings.YesNoBaseEnum.No;
                            this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "ABPAWHealthSafetyInComplete", Convert.ToString((int)Settings.YesNoBaseEnum.No));

                        }


                    }

                }


                //v1.0.2 - Decide if we need to check survey input date.
                bool bCheckSurveyInputDate = true;
                if (v_bCheckingForChangesOnly == true)
                {
                    if (this.m_bUserAppliedSurveyDate == false)
                    {
                        bCheckSurveyInputDate = false;

                    }

                }

                //v1.0.2 - Only check survey date if flagged
                if (bCheckSurveyInputDate == true)
                {

                    //Update key fields
                    DateTime dSurveyDate = DateTime.MinValue;
                    dSurveyDate = btnSurveyedDate.Date;
                    if (dSurveyDate.Date == null)
                    {
                        throw new Exception("No survey date set for sub project " + this.m_cProjectData.SubProjectNo);
                    }
                    else
                    {

                        //v1.0.2 - Decide if we need to update survey dates.
                        bool bUpdateSurveyorDetails = true;
                        if (this.m_cProjectData.MXM1002TrfDate.HasValue == true)
                        {
                            if (this.m_bUserAppliedSurveyDate == false)
                            {
                                if (this.m_cProjectData.MXM1002TrfDate.Value.Equals(dSurveyDate) == true)
                                {
                                    bUpdateSurveyorDetails = false;

                                }

                            }

                        }


                        if (bUpdateSurveyorDetails == true)
                        {

                            //Log survey date.
                            if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.MXM1002TrfDate = dSurveyDate; }
                            this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "Mxm1002TrfDate", dSurveyDate.ToString());

                            //v1.0.2 - Update surveyor details
                            if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.SurveyorName = Session.CurrentUserName; }
                            this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmSurveyorName", this.m_cProjectData.SurveyorName);

                            if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.SurveyorProfile = Session.CurrentUserName; }
                            this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmSurveyorProfile", this.m_cProjectData.SurveyorProfile);

                            if (v_bCheckingForChangesOnly == false) { this.m_cProjectData.SurveyorPCTag = Settings.GetMachineName(); }
                            this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, "MxmSurveyorPCTag", this.m_cProjectData.SurveyorPCTag);
                        }
                    }

                }


                return true;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }
        }

        /// <summary>
        /// Log combo box value change.
        /// </summary>
        /// <param name="v_sControlName"></param>
        /// <param name="v_sFieldName"></param>
        /// <param name="v_iOldValue"></param>
        /// <returns></returns>
        private int? LogComboValueChange(Picker v_sControlName, string v_sFieldName, int? v_iOldValue)
        {

            int? iRtnValue = v_iOldValue;
            int? iNewValue = null;
            string sValue = string.Empty;

            try
            {
                Picker cmbCombo = v_sControlName;
                if (cmbCombo != null)
                {

                    sValue = Main.ReturnComboSelectedTagValue(cmbCombo);
                    iNewValue = Convert.ToInt32(sValue);

                    if (cmbCombo.SelectedIndex != Convert.ToInt32(sValue))
                    {

                        if (iNewValue != v_iOldValue)
                        {
                            iRtnValue = iNewValue;
                            this.m_cUpdateLog = Settings.AddToUpdatesList(this.m_cUpdateLog, this.m_cProjectData.SubProjectNo, v_sFieldName, iNewValue.ToString());

                        }

                    }


                }

                return iRtnValue;

            }
            catch (Exception ex)
            {
                throw new Exception("Field:" + v_sFieldName + ",Control:" + v_sControlName + ",Error:" + ex.Message);

            }

        }
        /// <summary>
        /// v1.0.2 - Save project
        /// </summary>
        /// <returns></returns>
        private async Task<bool> SaveProject()
        {


            bool bValidationOK = false;
            bool bSaveOK = true;
            try
            {

                bValidationOK = await this.IsSaveValidationOK();
                if (bValidationOK == false)
                {
                    bSaveOK = false;

                }
                else
                {

                    bool bLoggedOK = this.LogChangesForSaving(false);
                    if (bLoggedOK == false)
                    {
                        bSaveOK = false;

                    }
                    else
                    {

                        if (this.m_cUpdateLog.Count > 0)
                        {

                            //Save details away.
                            bSaveOK = Main.p_cDataAccess.UpdateProjectTable(this.m_cProjectData);
                            if (bSaveOK == true)
                            {

                                //Save log changes for syncing.
                                bSaveOK = Main.p_cDataAccess.AddUpdatesToUpdateTable(this.m_cUpdateLog);
                                if (bSaveOK == false)
                                {
                                    bSaveOK = false;

                                }

                            }

                        }

                        //v1.0.1 - Save project notes.
                        //bSaveOK = this.SaveProjectNotes();
                        //if (bSaveOK == false)
                        //{
                          //  bSaveOK = false;
                        //}

                    }


                    if (bSaveOK == false)
                    {
                        await DisplayAlert("Error Saving", "An error has occurred when trying to save your changes, please try again.", "OK");

                    }

                }

                return bSaveOK;
            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }
        }
        /// <summary>
        /// Is combo value selected
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsComboValueSelected(Picker v_sControl, string v_sSelectionType)
        {

            string sValue = string.Empty;
            string sMsg = string.Empty;
            try
            {

                //Try and locate the combo box on screen.
                Picker cmbCombo = v_sControl;//this.FindByName<Picker>(v_sControlName);
                if (cmbCombo != null)
                {

                    //Check floor level is selected.
                    sValue = Main.ReturnComboSelectedTagValue(cmbCombo);
                    if (Convert.ToInt32(sValue) == -1)
                    {
                        sMsg = "You must select a " + v_sSelectionType + " option before you can update the sub projects survey details.";
                        await DisplayAlert("Selection Required.", sMsg, "OK");
                        cmbCombo.Focus();
                        return false;

                    }

                    return true;
                }
                else
                {
                    throw new Exception("Control " + v_sSelectionType + " is not a combobox.");
                }

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }


        }
        /// <summary>
        /// Check if survey input passes save validation.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsSaveValidationOK()
        {

            bool bComboOK = false;
            try
            {


                bComboOK = await this.IsComboValueSelected(this.cmbPropertyType, "Property Type");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbFloorLevel, "Floor Level");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbInstallationType, "Installation Type");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbAsbestosPresumed, "Asbestos Presumed");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbAccessEquipment, "Access Equipment");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbPermanentGasVent, "Permanent Gas Vent");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbWindowboard, "Window Board");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbStructuralFaults, "Structural Faults");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbServicesToMove, "Services to move");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbDoorChoiceFormRcvd, "Door choice form received");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbDisabledAdaptionsRqd, "Disabled adaption's");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbInternalDamage, "Internal damage");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbWorkAccessRestrictions, "Work access restrictions");
                if (bComboOK == false) { return false; }

                bComboOK = await this.IsComboValueSelected(this.cmbPublicProtection, "Public protection");
                if (bComboOK == false) { return false; }

                //v1.0.4 - Check if other is selected, if so make sure notes is attached.
                if (this.HasOtherBeenSelected() == true)
                {
                    if (this.m_cProjectNotes.Count() == 0)
                    {

                        await DisplayAlert("Note Required", "You have selected Other on one of the drop downs, you need to add a note.", "OK");
                        return false;

                    }

                }

                bool bSurveyDateOK = await this.IsSurveyDateSet();
                if (bSurveyDateOK == false) { return false; }

                return true;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }

        }
        /// <summary>
        /// Is survey date set.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsSurveyDateSet()
        {

            try
            {

                if (this.btnSurveyedDate.Date != null)
                {
                    DateTime dSurveyDate = DateTime.Now;
                    dSurveyDate = btnSurveyedDate.Date;
                    return true;

                }

                await DisplayAlert("Survey Date Required.", "You have not set a survey date for this sub project, please amend and try-again.", "OK");
                btnSurveyedDate.Focus();
                return false;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }
        }
        /// <summary>
        /// v1.0.4 - Check if OTHER has been selected on combo
        /// </summary>
        /// <param name="v_sComboBoxControlID"></param>
        /// <returns></returns>
        private bool HasOtherBeenSelectedOnCombo(Picker v_sComboBoxControlID)
        {

            try
            {

                Picker cmbItem = v_sComboBoxControlID;
                if (cmbItem != null)
                {

                    string sValue = Main.ReturnComboSelectedItemText(cmbItem);
                    if (sValue.Equals("Other", StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        return true;
                    }


                }

                return false;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), v_sComboBoxControlID);
                return false;

            }

        }
        /// <summary>
        /// v1.0.4 - Has OTHER been selected on any of the drop downs.
        /// </summary>
        /// <returns></returns>
        private bool HasOtherBeenSelected()
        {

            try
            {
                bool bOtherSelected = false;

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbPropertyType);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbFloorLevel);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbInstallationType);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbAsbestosPresumed);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbAccessEquipment);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbPermanentGasVent);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbWindowboard);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbStructuralFaults);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbServicesToMove);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbDoorChoiceFormRcvd);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbDisabledAdaptionsRqd);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbInternalDamage);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbWorkAccessRestrictions);
                if (bOtherSelected == true) { return true; }

                bOtherSelected = this.HasOtherBeenSelectedOnCombo(this.cmbPublicProtection);
                if (bOtherSelected == true) { return true; }

                return false;

            }
            catch (Exception ex)
            {
                //cMain.ReportError(ex, cMain.GetCallerMethodName(), string.Empty);
                return false;

            }

        }

        private void CameraBtn_Tapped()
        {
            // navigation camera
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new PhotoScreenPage(m_surveyInputResult)));
        }
        private void NoteBtn_Tapped()
        {
            // add note
            Device.BeginInvokeOnMainThread(() => Navigation.PushAsync(new AddNotePage(m_surveyInputResult)));
        }
        private async void PasteBtn_Tapped()
        {
            try
            {
                if (Main.p_cSurveyInputCopiedSelections != null)
                {
                    if (Main.p_cSurveyInputCopiedSelections.ProjectNo.Equals(this.m_cProjectData.ProjectNo, StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        StringBuilder sbMsg = new StringBuilder();
                        sbMsg.Append("Are you sure you want to paste in the selections from the following sub-project:");
                        sbMsg.Append(Environment.NewLine);
                        sbMsg.Append(Environment.NewLine);
                        sbMsg.Append(Main.p_cSurveyInputCopiedSelections.SubProjectNo);
                        sbMsg.Append(Environment.NewLine);
                        sbMsg.Append(Main.p_cSurveyInputCopiedSelections.DeliveryStreet);
                        var answer = await DisplayAlert("Paste Selections", sbMsg.ToString(), "Yes", "No");
                        if (answer == true)
                        {
                            int? iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbPropertyType));
                            this.cmbPropertyType.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;
                            iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbFloorLevel));
                            this.cmbFloorLevel.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;
                            iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbInstallationType));
                            this.cmbInstallationType.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbAsbestosPresumed));
                            this.cmbAsbestosPresumed.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbAccessEquipment));
                            this.cmbAccessEquipment.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbPermanentGasVent));
                            this.cmbPermanentGasVent.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbWindowboard));
                            this.cmbWindowboard.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbStructuralFaults));
                            this.cmbStructuralFaults.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbServicesToMove));
                            this.cmbServicesToMove.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbDisabledAdaptionsRqd));
                            this.cmbDisabledAdaptionsRqd.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbDoorChoiceFormRcvd));
                            this.cmbDoorChoiceFormRcvd.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbInternalDamage));
                            this.cmbInternalDamage.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbWorkAccessRestrictions));
                            this.cmbWorkAccessRestrictions.SelectedIndex = iCurValue == null ? 0 : (int)iCurValue;

                            iCurValue = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbPublicProtection));
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
                Main.p_cSurveyInputCopiedSelections = new cProjectTable();
                Main.p_cSurveyInputCopiedSelections.ProjectNo = this.m_cProjectData.ProjectNo;
                Main.p_cSurveyInputCopiedSelections.ProjectName = this.m_cProjectData.ProjectName;
                Main.p_cSurveyInputCopiedSelections.SubProjectNo = this.m_cProjectData.SubProjectNo;
                Main.p_cSurveyInputCopiedSelections.SubProjectName = this.m_cProjectData.SubProjectName;
                Main.p_cSurveyInputCopiedSelections.DeliveryStreet = this.m_cProjectData.DeliveryStreet;

                Main.p_cSurveyInputCopiedSelections.PropertyType = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbPropertyType));
                Main.p_cSurveyInputCopiedSelections.ABPAXFloorLevel = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbFloorLevel));
                Main.p_cSurveyInputCopiedSelections.ABPAXInstallationType = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbInstallationType));
                Main.p_cSurveyInputCopiedSelections.ABPAXAsbestosPresumed = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbAsbestosPresumed));
                Main.p_cSurveyInputCopiedSelections.ABPAXAccessEquipment = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbAccessEquipment));
                Main.p_cSurveyInputCopiedSelections.ABPAXPermanentGasVent = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbPermanentGasVent));
                Main.p_cSurveyInputCopiedSelections.ABPAXWindowBoard = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbWindowboard));
                Main.p_cSurveyInputCopiedSelections.ABPAXStructuralFaults = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbStructuralFaults));
                Main.p_cSurveyInputCopiedSelections.ABPAXServicesToMove = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbServicesToMove));
                Main.p_cSurveyInputCopiedSelections.DisabledAdaptionsRequired = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbDisabledAdaptionsRqd));
                Main.p_cSurveyInputCopiedSelections.MxmDoorChoiceFormReceived = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbDoorChoiceFormRcvd));
                Main.p_cSurveyInputCopiedSelections.ABPAXInternalDamage = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbInternalDamage));
                Main.p_cSurveyInputCopiedSelections.ABPAXWorkAccessRestrictions = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbWorkAccessRestrictions));
                Main.p_cSurveyInputCopiedSelections.ABPAXPublicProtection = Convert.ToInt32(Main.ReturnComboSelectedTagValue(this.cmbPublicProtection));
            }
            catch (Exception ex)
            {

            }
        }
    }
}
