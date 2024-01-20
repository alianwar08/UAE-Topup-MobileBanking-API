using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MobileBanking.BusinessLogic.DTOs;
using MobileBanking.DataAccess;
using MobileBanking.DataAccess.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBanking.BusinessLogic
{
    public class BeneficiaryService
    {
        private readonly BankingContext _dbContext;
        private readonly ILogger<BeneficiaryService> _logger;

        public BeneficiaryService()
        {
            _dbContext = new BankingContext();
            _logger = ServiceLocator.Current.GetRequiredService<ILogger<BeneficiaryService>>();
        }

        public async Task<ResponseBO<List<BeneficiaryDTO>>> GetBeneficiariesAsync(int userId)
        {
            var response = new ResponseBO<List<BeneficiaryDTO>>();
            try
            {
                if (!await IsValidUser(userId))
                {
                    response.AddError("Invalid User ID.");
                    return response;
                }

                var beneficiaries = await _dbContext.Beneficiaries
                    .Where(b => b.UserID == userId && b.IsActive)
                    .Select(b => new BeneficiaryDTO
                    {
                        BeneficiaryID = b.BeneficiaryID,
                        Nickname = b.Nickname,
                        PhoneNumber = b.PhoneNumber,
                        UserID = b.UserID
                       
                    })
                    .ToListAsync();

                if (beneficiaries == null || beneficiaries.Count == 0)
                {
                    response.AddError("No beneficiaries found for the specified user.");
                    return response;
                }

                response.AddSuccess("Beneficiaries retrieved successfully.");
                response.Data = beneficiaries;
            }
            catch (Exception ex)
            {
                response.AddException("An error occurred while retrieving beneficiaries.");
                LogException(ex);
            }

            return response;
        }

        public async Task<ResponseBO> AddBeneficiaryAsync(BeneficiaryDTO beneficiaryDto)
        {
            var response = new ResponseBO();

            try
            {
                var beneficiary = new Beneficiary
                {
                    UserID = beneficiaryDto.UserID,
                    Nickname = beneficiaryDto.Nickname,
                    PhoneNumber = beneficiaryDto.PhoneNumber,
                    CreatedAt = DateTime.UtcNow.Date,
                    IsActive = true
                };

                if (!await ValidateBeneficiary(beneficiary, response))
                {
                    return response;
                }

                _dbContext.Beneficiaries.Add(beneficiary);
                await _dbContext.SaveChangesAsync();

                response.AddSuccess("Beneficiary added successfully.");
            }
            catch (Exception ex)
            {
                response.AddException("An error occurred while adding the beneficiary.");
                LogException(ex);
            }

            return response;
        }

        private async Task<bool> ValidateBeneficiary(Beneficiary beneficiary, ResponseBO response)
        {
            if (beneficiary == null)
            {
                response.AddError("Beneficiary information is null.");
                return false;
            }

            if (!await IsValidUser(beneficiary.UserID))
            {
                response.AddError("Invalid User ID.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(beneficiary.Nickname) || beneficiary.Nickname.Length > 20)
            {
                response.AddError("Beneficiary nickname is required and must be 20 characters or less.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(beneficiary.PhoneNumber) || beneficiary.PhoneNumber.Length > 15)
            {
                response.AddError("Beneficiary phone number is required and must be 15 characters or less.");
                return false;
            }

            if (await IsBeneficiaryLimitReached(beneficiary.UserID))
            {
                response.AddError("Maximum number of active beneficiaries reached.");
                return false;
            }

            return true;
        }

        private async Task<bool> IsValidUser(int userId)
        {
            return await _dbContext.Users.AnyAsync(u => u.UserID == userId);
        }

        private async Task<bool> IsBeneficiaryLimitReached(int userId)
        {
            var activeBeneficiaryCount = await _dbContext.Beneficiaries
                .CountAsync(b => b.UserID == userId && b.IsActive);
            return activeBeneficiaryCount >= 5;
        }

        private void LogException(Exception ex)
        {
            if (_logger != null)
            {
                _logger.LogError(ex, "An exception occurred in BeneficiaryService.");
            }
        }
    }
}
