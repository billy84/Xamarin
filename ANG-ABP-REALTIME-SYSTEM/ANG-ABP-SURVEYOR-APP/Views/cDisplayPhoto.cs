using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace ANG_ABP_SURVEYOR_APP.Views
{
    class cDisplayPhoto
    {

        /// <summary>
        /// IDKey field
        /// </summary>
        public int IDKey { get; set; }

        /// <summary>
        /// SubProject number field
        /// </summary>
        public string SubProjectNo { get; set; }

        /// <summary>
        /// File Note text field
        /// </summary>
        public string NoteText { get; set; }

        /// <summary>
        /// File path field
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// URI to the file path.
        /// </summary>
        public Uri FileUri { get; set; }

        /// <summary>
        /// Image Source
        /// </summary>
        public WriteableBitmap ImageSource { get; set; }

        /// <summary>
        /// Height to display grid
        /// </summary>
        public double GridHeight { get; set; }

        /// <summary>
        /// Width to display grid
        /// </summary>
        public double GridWidth { get; set; }

        /// <summary>
        /// Unique ID
        /// </summary>
        public int UniqueID { get; set; }

        /// <summary>
        /// Height of original image.
        /// </summary>
        public decimal ImageHeight { get; set; }

        /// <summary>
        /// Width of original image.
        /// </summary>
        public decimal ImageWidth { get; set; }

    }
}
