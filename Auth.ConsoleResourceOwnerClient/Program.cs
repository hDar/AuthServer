using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Auth.ConsoleResourceOwnerClient
{
    public class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();


        private static async Task MainAsync()
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);               
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "console.ro.client", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("straximages.dev.team.au@gmail.com", "Straximages#2017", "auth_api internal_auth_api");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);                
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var userId = "0d706984-f418-4139-933e-2e34d9e65aad";
            //var response = await client.GetAsync("http://localhost:5001/api/User/Get?userId=" + userId);

            var response = await client.GetAsync("http://localhost:5001/api/User/GetAll");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();                
                Console.WriteLine(JsonConvert.DeserializeObject(content));
                Console.WriteLine("\n\n");
            }

            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
    }
}
