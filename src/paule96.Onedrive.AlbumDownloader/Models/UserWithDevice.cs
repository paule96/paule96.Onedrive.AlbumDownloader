using System;
using System.Collections.Generic;
using System.Text;

namespace paule96.Onedrive.AlbumDownloader.Models
{
    public class UserWithDevice
    {
        public Application application { get; set; }
        public Device device { get; set; }
        public User user { get; set; }
        public Onedrivesync oneDriveSync { get; set; }
    }
}
