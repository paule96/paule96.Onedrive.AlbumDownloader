using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using paule96.Onedrive.AlbumDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace paule96.Onedrive.AlbumDownloader
{
    public class GraphSessionService
    {
        public const string baseUrl = "https://graph.microsoft.com/v1.0";
        public const string defaultOdataJsonValueToken = "value";
        public const string defaultOdataSkipLink = "@odata.nextLink";
        private readonly string currentProfil = $"{baseUrl}/me";

        public GraphSessionService(string clientId)
        {
            App = new PublicClientApplication(clientId);
        }
        private static readonly string[] scopes = new string[] {
            "user.read",
            "Files.Read",
            "Files.Read.All",
            "Files.Read.Selected",
            "Files.ReadWrite",
            "Files.ReadWrite.All",
            "Files.ReadWrite.AppFolder",
            "Files.ReadWrite.Selected",
            "User.Read"
        };
        public IPublicClientApplication App { get; private set; }
        public User CurrentUser { get; set; }
        private AuthenticationResult authResult;
        public AuthenticationResult AuthResult
        {
            get
            {
                if (authResult == null)
                {
                    throw new Exception("Please call Login first!");
                }
                if (authResult.ExtendedExpiresOn < DateTimeOffset.Now.AddMinutes(20))
                {
                    RenewSession().GetAwaiter().GetResult();
                }
                return authResult;
            }
            private set { authResult = value; }
        }
        /// <summary>
        /// Login an user, so the program can access the graphapi with the user token.
        /// This creates an <see cref="AuthenticationResult"/> you can use in you requests.
        /// </summary>
        /// <returns></returns>
        public async Task<AuthenticationResult> Login()
        {
            var result = await CreateAuthResult(App);
            CurrentUser = await GetCurrentUser();
            return result;

        }

        private async Task<User> GetCurrentUser()
        {
            return JsonConvert.DeserializeObject<User>(await GetHttpContentWithToken(currentProfil, authResult));
        }

        public async Task<IEnumerable<T>> GetListContent<T>(string url) where T : class
        {
            var result = JObject.Parse(await GetHttpContentWithToken(url, AuthResult));
            if (result.ContainsKey(defaultOdataJsonValueToken) == false)
            {
                throw new Exception("This request doesn't return a list.");
            }
            var typedResultList = result[defaultOdataJsonValueToken]
                .Children()
                .Select(d => d.ToObject<T>());
            if (result.ContainsKey("@odata.nextLink"))
            {
                typedResultList = typedResultList.Concat(await GetListContent<T>(result["@odata.nextLink"].Value<string>()));
            }
            return typedResultList;
        }

        public async Task<T> GetSingleContent<T>(string url) where T : class
        {
            return JsonConvert.DeserializeObject<T>(await GetHttpContentWithToken(url, AuthResult));
        }
        public static async Task<string> GetHttpContentWithToken(string url, AuthenticationResult authResult)
        {
            var httpClient = new System.Net.Http.HttpClient();
            var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
            //Add the token in Authorization header
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            var response = await httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public async Task RenewSession()
        {
            AuthResult = await CreateAuthResult(App);
        }

        private static async Task<AuthenticationResult> CreateAuthResult(IPublicClientApplication app)
        {
            AuthenticationResult authResult = null;
            // This opens a browser window and ask for a microsoft account the frist time
            var accounts = await app.GetAccountsAsync();
            try
            {
                authResult = await app.AcquireTokenSilentAsync(scopes, accounts.FirstOrDefault());
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenAsync to acquire a token
                System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");
                try
                {
                    authResult = await app.AcquireTokenAsync(scopes);
                }
                catch (MsalException msalex)
                {
                    throw new Exception($"Error Acquiring Token:{System.Environment.NewLine}{msalex}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}");
            }
            if (authResult != null)
            {
                return authResult;
            }
            throw new Exception("Error while creating a token.");
        }
    }
}
