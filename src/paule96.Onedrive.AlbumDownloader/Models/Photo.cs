using System;
using System.Collections.Generic;
using System.Text;

namespace paule96.Onedrive.AlbumDownloader.Models
{
    public class Photo
    {
        public string cameraMake { get; set; }
        public string cameraModel { get; set; }
        public float exposureDenominator { get; set; }
        public float exposureNumerator { get; set; }
        public float focalLength { get; set; }
        public float fNumber { get; set; }
        public DateTime takenDateTime { get; set; }
        public int iso { get; set; }
    }
}
