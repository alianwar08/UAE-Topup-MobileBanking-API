using System.Text;
using MobileBanking.DataAccess;
using MobileBanking.DataAccess.Entities;
using MobileBanking.BusinessLogic.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using MobileBanking.BusinessLogic;

namespace MobileBanking.NUnit
{
    [TestFixture]
    public class BeneficiaryTests
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly string BankingApiBaseUrl;

        static BeneficiaryTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            BankingApiBaseUrl = configuration["BankingApiBaseUrl"];
            HttpClient.BaseAddress = new Uri(BankingApiBaseUrl);
        }


        [OneTimeTearDown]
        public void Cleanup()
        {
            HttpClient.Dispose();
           
        }

        [Test]
        public async Task AddBeneficiaryTest()
        {
            int validUserId = 1; //  this is a valid user ID
            int invalidUserId = 999; // this is an invalid user ID

            // Test adding valid beneficiaries
            for (int i = 1; i <= 5; i++)
            {
                await AddBeneficiary(validUserId, $"Beneficiary{i}", $"050123456{i}", true);
            }

            // Test adding a sixth beneficiary, should fail
            await AddBeneficiary(validUserId, "Beneficiary6", "0501234566", false);

            // Test adding a beneficiary with an invalid user ID
            await AddBeneficiary(invalidUserId, "InvalidUserBeneficiary", "0501234567", false);

            // Test adding a beneficiary with too long nickname
            await AddBeneficiary(validUserId, new string('A', 21), "0501234568", false);
        }

        private async Task AddBeneficiary(int userId, string nickname, string phoneNumber, bool shouldSucceed)
        {
            var beneficiary = new BeneficiaryDTO
            {
                UserID = userId,
                Nickname = nickname,
                PhoneNumber = phoneNumber
            };

            var json = JsonSerializer.Serialize(beneficiary);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync("Beneficiary/AddBeneficiary", data);

            if (shouldSucceed)
            {
                Assert.That(response.IsSuccessStatusCode, $"Failed to add valid beneficiary: {nickname}");
            }
            else
            {
                Assert.That(!response.IsSuccessStatusCode, $"Incorrectly added invalid beneficiary: {nickname}");
            }
        }

        [Test]
        public async Task GetBeneficiariesTest()
        {
            int validUserId = 1;

            var response = await HttpClient.GetAsync($"Beneficiary/GetBeneficiaries?userId={validUserId}");
            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };


            var responseBody = await response.Content.ReadAsStringAsync();
            var beneficiaries = JsonSerializer.Deserialize<ResponseBO<List<BeneficiaryDTO>>>(responseBody, options);

            Assert.That(beneficiaries!=null, "Failed to retrieve beneficiaries");
            Assert.That(beneficiaries.Status == ResponseBO.ResponseStatus.Success && beneficiaries.Data.Count() == 5, beneficiaries.ToString());
        }


        [Test]
        public async Task GetTopUpOptionsTest()
        {
            var response = await HttpClient.GetAsync("TopUp/TopUpOptions");
            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };

            var responseBody = await response.Content.ReadAsStringAsync();
            var topUpOptions = JsonSerializer.Deserialize<ResponseBO<List<TopUpOptionDTO>>>(responseBody, options);

            Assert.That(topUpOptions!=null, "Failed to retrieve top-up options");
            Assert.That(topUpOptions.Data.Count > 0, topUpOptions.ToString());

        }
    }
}
