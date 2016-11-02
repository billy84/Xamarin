//
// Program Change Log, All changes to the version number to be listed below.
//
// Version  By   Date      Heat     Description
// 1.0.1    BP   28/09/14  379848   ABP Surveyor Windows 8 metro application.
// 1.0.2    BP   03/10/14           Display different message if account is disabled.
// 1.0.3    BP   11/10/14           Update to use new file upload web service.
// 1.0.4    BP   30/10/14           Survey dates search, only use enddatetime, start time lookup from 00:00:00
// 1.0.5    BP                      Running mode of the app, new field.
// 1.0.6    BP   31/10/14           When uploading new file, apply new modified date.
// 1.0.7    BP   31/10/14           Live build
// 1.0.8    BP   11/11/14           Compatibility with installers application
// 1.0.9    JM   12/11/14  395146   Remove -1 hour from AX time
// 1.0.10   BP   14/11/14  379848   Contract manager changes.
// 1.0.11   BP   24/11/14  379848   Contract manager changes - rework
// 1.0.12   BP   30/11/14           Include progress status on searches.
// 1.0.13   BP   17/12/14           Return project status on search.
// 1.0.14   BP   05/01/15           Remove "ToLower" from SQL as not valid.
// 1.0.15   BP   05/01/15           Update URL for live.
// 1.0.16   BP   17/01/15           Fix issue with contract manager app search.
// 1.0.18   BP   07/02/15           Installer search
// 1.0.19   BP   23/04/15  402855   Return project name in search results for tool tip.
//                                  Make all text searches wild card without needing a *
//                                  Update for new fields
// 1.0.20   BP   03/09/15           Routine for copying files to local temp area
// 1.0.21   BP   18/10/15  422625   Survey health and safety completed
//                                  Survey Failed reasons.
//                                  HS Incomplete filter for search
//                                  Update URL
// 1.0.22                           Rebuild
// 1.0.23   BP   05/09/16           When checking for survey hscomplete orders being complete, use null as well.
//                                  Do not throw error when clearing temp folder.


using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ANG-ABP-SURVEYOR-APP-CLASS")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ANG-ABP-SURVEYOR-APP-CLASS")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.23.0")]
[assembly: AssemblyFileVersion("1.0.23.0")]
[assembly: ComVisible(false)]