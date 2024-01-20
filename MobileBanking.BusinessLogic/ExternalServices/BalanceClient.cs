using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MobileBanking.BusinessLogic.ExternalServices
{
    public class BalanceClient
    {
        private static  HttpClient HttpClient = new HttpClient();
        private static  string _serviceBaseUrl;

        static BalanceClient()
        {
            var configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();

            _serviceBaseUrl = configuration["BalanceServiceAPIUrl"];
            HttpClient.BaseAddress = new Uri(_serviceBaseUrl);
        }

        public async Task<decimal> GetBalanceAsync(int userId)
        {
            var response = await HttpClient.GetAsync($"{_serviceBaseUrl}/GetBalance/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<decimal>();
        }

        public async Task<bool> UpdateBalanceAsync(int userId, decimal amount)
        {
            var response = await HttpClient.PostAsJsonAsync($"{_serviceBaseUrl}/UpdateBalance", new { UserId = userId, Amount = amount });
            return response.IsSuccessStatusCode;
        }
    }
}
