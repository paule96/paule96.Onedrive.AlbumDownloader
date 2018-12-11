using System;
using System.Collections.Generic;
using System.Text;

namespace paule96.Onedrive.AlbumDownloader.Models
{
    public class User
    {
        public string displayName { get; set; }
        public string surname { get; set; }
        public string givenName { get; set; }
        public string id { get; set; }
        public string userPrincipalName { get; set; }
        public object[] businessPhones { get; set; }
        public object jobTitle { get; set; }
        public object mail { get; set; }
        public object mobilePhone { get; set; }
        public object officeLocation { get; set; }
        public object preferredLanguage { get; set; }
    }
}
