using System;
using System.Collections.Generic;
using System.Text;

namespace paule96.Onedrive.AlbumDownloader.Models
{
    public class Album
    {
        public DateTime createdDateTime { get; set; }
        public string cTag { get; set; }
        public string eTag { get; set; }
        public string id { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public int size { get; set; }
        public string webUrl { get; set; }
        public Bundle bundle { get; set; }
        public UserWithDevice createdBy { get; set; }
        public UserWithDevice lastModifiedBy { get; set; }
        public Parentreference parentReference { get; set; }
        public Filesysteminfo fileSystemInfo { get; set; }
        public Shared shared { get; set; }
    }
}
