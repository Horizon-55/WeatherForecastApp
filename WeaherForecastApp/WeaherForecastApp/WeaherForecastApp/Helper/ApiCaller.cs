using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WeaherForecastApp.Helper
{
    public class ApiCaller
    {
        public static async Task<ApiResponce> Get(string url, string authId = null)
        {
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(authId))
                    client.DefaultRequestHeaders.Add("Authorization", authId);
                var request = await client.GetAsync(url);
                if (request.IsSuccessStatusCode)
                {
                    return new ApiResponce { Rensponce = await request.Content.ReadAsStringAsync() };
                } else 
                    return new ApiResponce { ErrorMessage = request.ReasonPhrase };
            }
        }
    }

    public class ApiResponce
    {
        public bool Successful => ErrorMessage == null;
        public string ErrorMessage { get; set; }
        public string Rensponce { get; set; }
    }
}
