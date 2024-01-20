using MobileBanking.DataAccess;
using MobileBanking.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace MobileBanking.NUnit
{



    [SetUpFixture]
    public class MobileBankingInitialize
    {
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            using (var context = new BankingContext())
            {
                context.TopUpTransactions.RemoveRange(context.TopUpTransactions);
                context.Beneficiaries.RemoveRange(context.Beneficiaries);

                context.Database.ExecuteSqlRaw("TRUNCATE TABLE Users;");

                context.SaveChanges();

                // Seed new data
                SeedUsers(context);
            }
        }

        private void SeedUsers(BankingContext context)
        {
            if (!context.Users.Any())
            {
                var uid = 0;
                var users = new[]
                {
                new User { Username = "SaraAmir", Email = "sara@domain.com", VerificationStatus = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Username = "OmarFaisal", Email = "omar@domain.com", VerificationStatus = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Username = "LaylaHadi", Email = "layla@domain.com", VerificationStatus = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Username = "NourAlHuda", Email = "nour@domain.com", VerificationStatus = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Username = "AdamBakr", Email = "adam@domain.com", VerificationStatus = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Username = "HanaSami", Email = "hana@domain.com", VerificationStatus = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Username = "KhalidYousef", Email = "khalid@domain.com", VerificationStatus = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Username = "MahaWaleed", Email = "maha@domain.com", VerificationStatus = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Username = "SamirNajib", Email = "samir@domain.com", VerificationStatus = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Username = "ZaraFahim", Email = "zara@domain.com", VerificationStatus = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                };

                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }
    }


}
