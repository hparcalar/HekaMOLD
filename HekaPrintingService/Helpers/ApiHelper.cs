using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Text;

namespace HekaPrintingService.Helpers
{
    public class ApiHelper
    {
        HttpClient _httpClient;
        string baseUrl;

        public ApiHelper(string baseUrl)
        {
            this.baseUrl = baseUrl;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public void AddHeader(string header, string value)
        {
            if (_httpClient != null)
            {
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header, value);
            }
        }

        public async Task<T> GetData<T>(string target)
        {
            var httpResponse = await _httpClient.GetAsync($"{baseUrl}{target}");

            var content = await httpResponse.Content.ReadAsStringAsync();
            var returnItem = JsonConvert.DeserializeObject<T>(content);

            return returnItem;
        }

        public async Task PostData<T>(string target, T data)
        {
            await _httpClient.PostAsync($"{baseUrl}{target}",
                new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
        }

        public async Task PutData<T>(string target, T data)
        {
            var putStr = JsonConvert.SerializeObject(data);
            putStr = putStr.Replace("Do", "do");

            var xx = await _httpClient.PutAsync($"{baseUrl}{target}",
                new StringContent(putStr, Encoding.UTF8, "application/json"));
        }
    }
}
