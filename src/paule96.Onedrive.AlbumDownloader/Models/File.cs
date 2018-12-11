using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace paule96.Onedrive.AlbumDownloader.Models
{
    public class File
    {
        [JsonProperty("@microsoft.graph.downloadUrl")]
        public string microsoftgraphdownloadUrl { get; set; }
        public DateTime createdDateTime { get; set; }
        public string cTag { get; set; }
        public string eTag { get; set; }
        public string id { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string name { get; set; }
        public int size { get; set; }
        public string webUrl { get; set; }
        public UserWithDevice createdBy { get; set; }
        public UserWithDevice lastModifiedBy { get; set; }
        public Parentreference parentReference { get; set; }
        public File file { get; set; }
        public Filesysteminfo fileSystemInfo { get; set; }
        public Image image { get; set; }
        public Photo photo { get; set; }
        public Shared shared { get; set; }
    }
}
