using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using paule96.Onedrive.AlbumDownloader.Models;

namespace paule96.Onedrive.AlbumDownloader
{
    public interface IGraphSessionService
    {
        IPublicClientApplication App { get; }
        AuthenticationResult AuthResult { get; }
        User CurrentUser { get; }
        Task<string> GetHttpContentWithToken(string url, AuthenticationResult authResult);
        Task<IEnumerable<T>> GetListContent<T>(string url) where T : class;
        Task<IEnumerable<T>> GetListContent<T>(string url, AuthenticationResult authResult) where T : class;
        Task<T> GetSingleContent<T>(string url) where T : class;
        Task<T> GetSingleContent<T>(string url, AuthenticationResult authResult) where T : class;
        Task<AuthenticationResult> Login();
        Task RenewSession();
    }
}