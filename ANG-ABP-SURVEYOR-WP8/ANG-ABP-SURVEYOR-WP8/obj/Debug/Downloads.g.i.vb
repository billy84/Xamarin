﻿

#ExternalChecksum("C:\Development\ABP\ANG-ABP-SURVEYOR-WP8\ANG-ABP-SURVEYOR-WP8\Downloads.xaml", "{406ea660-64cf-4c82-b6f0-42d48172a799}", "CBEC1C087D98DC115926B49A7FDC8FE1")
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On


Namespace Global.ANG_ABP_SURVEYOR_WP8

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>  _
    Partial Class Downloads
        Inherits Global.Windows.UI.Xaml.Controls.Page

        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks", "4.0.0.0")>  _
        private WithEvents LayoutRoot As Global.Windows.UI.Xaml.Controls.Grid
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks", "4.0.0.0")>  _
        private WithEvents ContentRoot As Global.Windows.UI.Xaml.Controls.Grid
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks", "4.0.0.0")>  _
        private WithEvents lvDownloads As Global.Windows.UI.Xaml.Controls.ListView

        Private _contentLoaded As Boolean

        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks", "4.0.0.0")>  _
        <Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
        Public Sub InitializeComponent()
            If _contentLoaded Then
                Return
            End If
            _contentLoaded = true

            Dim uri As New Global.System.Uri("ms-appx:///Downloads.xaml")
            Global.Windows.UI.Xaml.Application.LoadComponent(Me, uri, Global.Windows.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application)

            LayoutRoot = CType(Me.FindName("LayoutRoot"), Global.Windows.UI.Xaml.Controls.Grid)
            ContentRoot = CType(Me.FindName("ContentRoot"), Global.Windows.UI.Xaml.Controls.Grid)
            lvDownloads = CType(Me.FindName("lvDownloads"), Global.Windows.UI.Xaml.Controls.ListView)
        End Sub
    End Class

End Namespace


