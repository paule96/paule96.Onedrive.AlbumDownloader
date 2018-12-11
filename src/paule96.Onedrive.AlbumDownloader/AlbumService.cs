using Newtonsoft.Json.Linq;
using paule96.Onedrive.AlbumDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace paule96.Onedrive.AlbumDownloader
{
    public class AlbumService
    {        
        public AlbumService(GraphSessionService graphSession)
        {
            GraphSession = graphSession;
        }

        public GraphSessionService GraphSession { get; }

        public async Task<IEnumerable<Album>> GetAlbums()
        {            
            var url = $"{GraphSessionService.baseUrl}/me/drive/items/{GraphSession.CurrentUser.id}!0:/SkyDriveCache/Albums:/children";
            return await GraphSession.GetListContent<Album>(url);            
        }

        public async Task<IEnumerable<File>> GetAlbumItems(string albumId)
        {
            var url = $"{GraphSessionService.baseUrl}/drives/{GraphSession.CurrentUser.id}/items/{albumId}/children";
            return await GraphSession.GetListContent<File>(url);
        }
    }
}
