using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MobileBanking.BusinessLogic;
using MobileBanking.BusinessLogic.DTOs;
using MobileBanking.BusinessLogic.ExternalServices;
using MobileBanking.DataAccess;
using MobileBanking.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileBanking.BusinessLogic
{
    public class TopUpService
    {
        private readonly BankingContext _dbContext;
        private readonly BalanceClient _balanceClient;
        private readonly ILogger<TopUpService> _logger;

        public TopUpService()
        {
            _dbContext = new BankingContext();
            _logger = ServiceLocator.Current.GetRequiredService<ILogger<TopUpService>>();
            _balanceClient = new BalanceClient();
        }

        public async Task<ResponseBO<List<TopUpOptionDTO>>> GetTopUpOptionsAsync()
        {
            var response = new ResponseBO<List<TopUpOptionDTO>>();
            try
            {
                var topUpOptions = await _dbContext.TopUpOptions
                                .Select(o => new TopUpOptionDTO
                                {
                                    OptionID = o.OptionID,
                                    Amount = o.Amount,
                                    Description = o.Description
                                })
                                .ToListAsync();

                if (topUpOptions == null || topUpOptions.Count == 0)
                {
                    response.AddError("No top-up options found.");
                    return response;
                }

                response.AddSuccess("Top-up options retrieved successfully.");
                response.Data = topUpOptions;
            }
            catch (Exception ex)
            {
                response.AddException("An error occurred while retrieving top-up options.");
                LogException(ex);
            }

            return response;
        }


        public async Task<ResponseBO<bool>> ProcessTopUpAsync(int userId, int beneficiaryId, int optionId)
        {
            var response = new ResponseBO<bool>();

            try
            {
                var user = await _dbContext.Users.FindAsync(userId);
                if (user == null)
                {
                    response.AddError("User not found.");
                    return response;
                }

                // Is beneficiary valid
                var beneficiary = await _dbContext.Beneficiaries
                    .Where(b=> b.IsActive==true && b.UserID==userId && b.BeneficiaryID==beneficiaryId).FirstOrDefaultAsync();
                if (beneficiary == null)
                {
                    response.AddError("Beneficiary not found.");
                    return response;
                }

                // Is Valid Option 
                var option = await _dbContext.TopUpOptions.Where(o => o.OptionID == optionId).FirstOrDefaultAsync();
                if (option == null)
                {
                    response.AddError("Topup Option not found.");
                    return response;
                }

                var amount = option.Amount;

                // Is topup amount is valid
                var IsValidTopUpAmountRS = await IsValidTopUpAmount(userId, beneficiaryId, amount);
                if (!IsValidTopUpAmountRS.Data)
                {
                    response.AddError("Invalid top-up amount based on user's verification status or monthly limit.");
                    response.ImportMessages(IsValidTopUpAmountRS.Messages, ResponseBO<bool>.ResponseStatus.Error);
                    return response;
                }


                var fee = 1M;

                decimal currentBalance = await _balanceClient.GetBalanceAsync(userId);
                if (amount + fee > currentBalance)
                {
                    response.AddError("Insufficient balance for the top-up.");
                    return response;
                }


                // Deducting balance
                bool balanceUpdated = await _balanceClient.UpdateBalanceAsync(userId, -1* (amount+ fee)); //amount + 1AED fee
                if (!balanceUpdated)
                {
                    response.AddError("Unable to update user balance.");
                    return response;
                }

                // Record the top-up transaction
                await RecordTopUpTransaction(userId, beneficiaryId, amount, fee);

                response.AddSuccess("Top-up processed successfully.");
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.AddException("An error occurred while processing the top-up.");
                LogException(ex);
            }

            return response;
        }

        private async Task<ResponseBO<bool>> IsValidTopUpAmount(int userId, int beneficiaryId, decimal amount)
        {
            var response = new ResponseBO<bool>();

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                response.AddError("User not found.");
                return response;
            }

            // Retrieve limits based on verification status
            var individualLimit = user.VerificationStatus ? 1000m : 500m;
            var totalMonthlyLimit = 3000m;

            // Calculate total amount topped up for this beneficiary in the current month
            var beneficiaryTotalThisMonth = await _dbContext.TopUpTransactions
                .Where(t => t.BeneficiaryID == beneficiaryId &&
                            t.TransactionDate.Month == DateTime.Now.Month &&
                            t.TransactionDate.Year == DateTime.Now.Year)
                .SumAsync(t => t.Amount);

            if (beneficiaryTotalThisMonth + amount > individualLimit)
            {
                response.AddError($"Top-up amount exceeds the monthly limit of AED {individualLimit} for this beneficiary.");
                return response;
            }

            // Calculate total amount topped up for all beneficiaries in the current month
            var userTotalThisMonth = await _dbContext.TopUpTransactions
                .Where(t => t.UserID == userId &&
                            t.TransactionDate.Month == DateTime.Now.Month &&
                            t.TransactionDate.Year == DateTime.Now.Year)
                .SumAsync(t => t.Amount);

            if (userTotalThisMonth + amount > totalMonthlyLimit)
            {
                response.AddError($"Top-up amount exceeds the total monthly limit of AED {totalMonthlyLimit} for all beneficiaries.");
                return response;
            }

            //response.AddSuccess("Top-up amount is within the permitted limits.");
            response.Data = true;
            return response;
        }

        private async Task RecordTopUpTransaction(int userId, int beneficiaryId, decimal amount, decimal fee)
        {
            var transaction = new TopUpTransaction
            {
                UserID = userId,
                BeneficiaryID = beneficiaryId,
                Amount = amount,
                TransactionDate = DateTime.Now,
                Status = "Completed",
                Fee = fee 
            };

            _dbContext.TopUpTransactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
        }

        private void LogException(Exception ex)
        {
            if (_logger != null)
            {
                _logger.LogError(ex, "An exception occurred in TopUpService.");
            }
        }

    }
}



