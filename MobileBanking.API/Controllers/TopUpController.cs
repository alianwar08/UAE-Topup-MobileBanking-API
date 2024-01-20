using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MobileBanking.BusinessLogic;
using MobileBanking.BusinessLogic.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileBanking.API.Controllers
{
    /// <summary>
    /// Controller for managing top-up transactions.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TopUpController : ControllerBase
    {
        private readonly TopUpService _topUpService;
        private readonly ILogger<TopUpController> _logger;

        public TopUpController()
        {
            _topUpService = new TopUpService();
            _logger = ServiceLocator.Current.GetService<ILogger<TopUpController>>();
        }

        /// <summary>
        /// Processes a top-up transaction.
        /// </summary>
        /// <param name="topUpDto">Top-up data transfer object.</param>
        /// <returns>A response indicating the result of the transaction.</returns>
        [HttpPost("TopUp")]
        public async Task<ActionResult<ResponseBO<bool>>> TopUp([FromBody] TopUpDTO topUpDto)
        {
            try
            {
                var response = await _topUpService.ProcessTopUpAsync(topUpDto.UserId, topUpDto.BeneficiaryId, topUpDto.OptionId);
                return response.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in TopUp");
                return new ResponseBO<bool>
                {
                    Status = ResponseBO<bool>.ResponseStatus.Exception,
                    Messages = new List<string> { "An unexpected error occurred during the top-up process." }
                }.ToActionResult();
            }
        }

        /// <summary>
        /// Retrieves available top-up options.
        /// </summary>
        /// <returns>A list of top-up options.</returns>
        [HttpGet("TopUpOptions")]
        public async Task<ActionResult<ResponseBO<List<TopUpOptionDTO>>>> GetTopUpOptions()
        {
            try
            {
                var response = await _topUpService.GetTopUpOptionsAsync();
                return response.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetTopUpOptions");
                return new ResponseBO<List<TopUpOptionDTO>>
                {
                    Status = ResponseBO<List<TopUpOptionDTO>>.ResponseStatus.Exception,
                    Messages = new List<string> { "An unexpected error occurred while retrieving top-up options." }
                }.ToActionResult();
            }
        }

    }
}
