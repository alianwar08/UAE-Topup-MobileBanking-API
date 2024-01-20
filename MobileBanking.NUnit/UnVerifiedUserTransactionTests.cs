using System.Text;
using MobileBanking.DataAccess;
using MobileBanking.DataAccess.Entities;
using MobileBanking.BusinessLogic.DTOs;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using MobileBanking.BusinessLogic;
using Microsoft.EntityFrameworkCore;



namespace MobileBanking.NUnit
{
    [TestFixture]
    public class UnVerifiedUserTransactionTests
    {
        private readonly string BankingApiBaseUrl;
        private readonly HttpClient BankingHttpClient = new HttpClient();
        private readonly HttpClient BalanceHttpClient = new HttpClient();
        private BankingContext _context = new BankingContext();

        bool AlwayFail = false;

        static readonly int AED5 = 1;
        static readonly int AED10 = 2;
        static readonly int AED20 = 3;
        static readonly int AED30 = 4;
        static readonly int AED50 = 5;
        static readonly int AED75 = 6;
        static readonly int AED100 = 7;


        User User01_UnVerified;
        Beneficiary[] User01_Beneficiaries;

        public UnVerifiedUserTransactionTests()
        {
            var configuration = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();

            BankingApiBaseUrl = configuration["BankingApiBaseUrl"];
            BankingHttpClient.BaseAddress = new Uri(BankingApiBaseUrl);

            var BalanceApiBaseUrl = configuration["BalanceApiBaseUrl"];
            BalanceHttpClient.BaseAddress = new Uri(BalanceApiBaseUrl);
        }


        [SetUp]
        public void Initialize()
        {
            _context = new BankingContext();


            _context.Database.ExecuteSqlRaw("TRUNCATE TABLE TopUpTransactions;");
            _context.Database.ExecuteSqlRaw("TRUNCATE TABLE Beneficiaries;");
            _context.Database.ExecuteSqlRaw("TRUNCATE TABLE Users;");


            _context.SaveChanges();
            User01_UnVerified = null;
            User01_Beneficiaries = null;

            // Add a UnVerified user
            User01_UnVerified = new User { Username = "User01", Email = "User01@domain.com", VerificationStatus = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

            _context.Users.Add(User01_UnVerified);
            _context.SaveChanges();
            LinkedList<Beneficiary> ben = new LinkedList<Beneficiary>();

            // Add 5 beneficiaries for the user
            for (int i = 1; i <= 31; i++)
            {
                var beneficiary = new Beneficiary { IsActive = true, Nickname = "NICK0" + i, PhoneNumber = "056553127X", UserID = User01_UnVerified.UserID };
                _context.Beneficiaries.Add(beneficiary);
                ben.AddLast(beneficiary);
                _context.SaveChanges();
            }

            User01_Beneficiaries = ben.ToArray();

        }

        [Test, Order(0)]
        public async Task Test000_UnVerified_OverBalance_Fail()
        {
            await UpdateBalance(User01_UnVerified.UserID, 10);
            var response = (await PerformTopUp(User01_UnVerified.UserID, User01_Beneficiaries[3].BeneficiaryID, AED100));
            Assert.That(!AlwayFail && response.Status == ResponseBO.ResponseStatus.Error, response.ToString());
        }

        [Test, Order(1)]
        public async Task Test001_UnVerified_UnderBalanceUnderLimit_Success()
        {
            await UpdateBalance(User01_UnVerified.UserID, 4000);
            var response = (await PerformTopUp(User01_UnVerified.UserID, User01_Beneficiaries[0].BeneficiaryID, AED100));
            Assert.That(!AlwayFail && response.Status == ResponseBO.ResponseStatus.Success, response.ToString());
        }


        [Test, Order(2)]
        public async Task Test002_UnVerified_OverIndividualMonthlyLimit_Fail()
        {
            var response = (await PerformTopUp(User01_UnVerified.UserID, User01_Beneficiaries[0].BeneficiaryID, AED100));
            response = (await PerformTopUp(User01_UnVerified.UserID, User01_Beneficiaries[0].BeneficiaryID, AED100));
            response = (await PerformTopUp(User01_UnVerified.UserID, User01_Beneficiaries[0].BeneficiaryID, AED100));
            response = (await PerformTopUp(User01_UnVerified.UserID, User01_Beneficiaries[0].BeneficiaryID, AED100));
            response = (await PerformTopUp(User01_UnVerified.UserID, User01_Beneficiaries[0].BeneficiaryID, AED75));
            response = (await PerformTopUp(User01_UnVerified.UserID, User01_Beneficiaries[0].BeneficiaryID, AED30));

            Assert.That(!AlwayFail && response.Status == ResponseBO.ResponseStatus.Error, response.ToString());
        }


        [Test, Order(3)]
        public async Task Test003_UnVerified_OverNetLimit_Fail()
        {

            for (int i = 0; i <= 29; i++)
            {
                var r = (await PerformTopUp(User01_UnVerified.UserID, User01_Beneficiaries[i].BeneficiaryID, AED100));

            }

            var response = (await PerformTopUp(User01_UnVerified.UserID, User01_Beneficiaries[6].BeneficiaryID, AED5));
            Assert.That(!AlwayFail && response.Status == ResponseBO.ResponseStatus.Error, response.ToString());
        }


        private async Task<ResponseBO<bool>> PerformTopUp(int UserId, int beneficiaryId, int optionId)
        {
            var topUpDto = new TopUpDTO
            {
                UserId = UserId,
                BeneficiaryId = beneficiaryId,
                OptionId = optionId
            };

            var json = JsonSerializer.Serialize(topUpDto);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await BankingHttpClient.PostAsync("TopUp/TopUp", data);

            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };


            var responseBody = await response.Content.ReadAsStringAsync();
            var responseBO = JsonSerializer.Deserialize<ResponseBO<bool>>(responseBody, options);

            return responseBO;
        }


        private async Task<bool> UpdateBalance(int UserId, decimal amount)
        {

            var response = await BalanceHttpClient.PostAsync($"UpdateBalance/?userId={UserId}&amount={amount}", null);
            return response.IsSuccessStatusCode;
        }


        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            BankingHttpClient.Dispose();
            BalanceHttpClient.Dispose();
            _context.Dispose();
        }

        [TearDown]
        public void TearDown()
        {

            _context.Dispose();
        }
    }
}
