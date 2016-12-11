using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace Anglian.Service
{
    public interface IMain
    {
        Task<bool> CopyFile(string v_sFromFile, string v_sToFile);
        string GetAppResourceValue(string v_sResourceName);
        Task<object> ReturnImageFromFile(string v_sImageFilePath);
        decimal ReturnHeightBitmapImage(object bitmapImage);
        Task<object> ReadAndResizeImageFile(string v_sFilePath, Size v_szSize);
        decimal ReturnWidthBitmapImage(object bitmapImage);
        Task<bool> CopyAndConvertImage(object v_sfFrom, object v_sfTo, Size v_szSize);
    }
}
