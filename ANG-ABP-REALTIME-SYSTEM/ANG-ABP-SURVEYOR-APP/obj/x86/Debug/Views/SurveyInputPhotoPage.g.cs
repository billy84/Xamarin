﻿

#pragma checksum "C:\Development\ABP\ANG-ABP-REALTIME-SYSTEM\ANG-ABP-SURVEYOR-APP\Views\SurveyInputPhotoPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E06591ED9D4FDFB8D2F295109F856BBE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ANG_ABP_SURVEYOR_APP.Views
{
    partial class SurveyInputPhotoPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 11 "..\..\..\Views\SurveyInputPhotoPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.SurveyInputPhotoPage_Loaded;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 90 "..\..\..\Views\SurveyInputPhotoPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnSelectPhoto_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 91 "..\..\..\Views\SurveyInputPhotoPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnTakePhoto_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 92 "..\..\..\Views\SurveyInputPhotoPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnSaveChanges_Click;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 78 "..\..\..\Views\SurveyInputPhotoPage.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.imgPic_Tapped;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 82 "..\..\..\Views\SurveyInputPhotoPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.btnRemoveItem_Click;
                 #line default
                 #line hidden
                break;
            case 7:
                #line 83 "..\..\..\Views\SurveyInputPhotoPage.xaml"
                ((global::Windows.UI.Xaml.Controls.TextBox)(target)).TextChanged += this.txtPicNotes_TextChanged;
                 #line default
                 #line hidden
                break;
            case 8:
                #line 41 "..\..\..\Views\SurveyInputPhotoPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.backButton_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


