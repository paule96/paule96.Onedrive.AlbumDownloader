using paule96.Onedrive.AlbumDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace paule96.Onedrive.AlbumDownloader
{
    public class FileService
    {
        public FileService(GraphSessionService graphSession)
        {
            GraphSession = graphSession;
        }

        public GraphSessionService GraphSession { get; }

        public async Task<File> GetFileMetadata(string driveId, string fileId)
        {
            var url = $"{GraphSessionService.baseUrl}/drives/{driveId}/items/{fileId}";
            return await GraphSession.GetSingleContent<File>(url);
        }

        public async Task DownloadFiles(string outputFolder, IEnumerable<File> files)
        {
            foreach (var file in files)
            {
                await DownloadFile(outputFolder, file);
            }
        }

        public async Task DownloadFiles(string outputFolder, IEnumerable<File> files, bool makeFileNamesUnique)
        {
            if (makeFileNamesUnique)
            {
                for (int i = 0; i < files.Count(); i++)
                {
                    var element = files.ElementAt(i);
                    element.name = $"{i.ToString()}_{element.name}";
                }
            }
            await DownloadFiles(outputFolder, files);
        }

        public async Task DownloadFile(string outputFolder, File file)
        {
            var newDownloadUrl = (await GetFileMetadata(file.parentReference.driveId, file.id)).microsoftgraphdownloadUrl;
            await DownloadFile(outputFolder + file.name, newDownloadUrl);
        }

        private Task DownloadFile(string outputPath, string url)
        {
            var client = new System.Net.WebClient();
            return client.DownloadFileTaskAsync(url, outputPath);
        }
    }
}
