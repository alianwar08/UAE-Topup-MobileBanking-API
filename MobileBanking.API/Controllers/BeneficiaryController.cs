using Microsoft.AspNetCore.Mvc;
using MobileBanking.BusinessLogic;
using MobileBanking.BusinessLogic.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileBanking.API.Controllers
{
    /// <summary>
    /// Controller for managing beneficiaries.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class BeneficiaryController : ControllerBase
    {
        private readonly BeneficiaryService _beneficiaryService;
        private readonly ILogger<BeneficiaryController> _logger;

        public BeneficiaryController()
        {
            _beneficiaryService = new BeneficiaryService();
            _logger = ServiceLocator.Current.GetService<ILogger<BeneficiaryController>>();
        }

        /// <summary>
        /// Adds a new beneficiary.
        /// </summary>
        /// <param name="beneficiaryDto">Beneficiary data transfer object.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpPost("AddBeneficiary")]
        public async Task<ActionResult<ResponseBO>> AddBeneficiary([FromBody] BeneficiaryDTO beneficiaryDto)
        {
            try
            {
                var response = await _beneficiaryService.AddBeneficiaryAsync(beneficiaryDto);
                return response.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in AddBeneficiary");
                return new ResponseBO
                {
                    Status = ResponseBO.ResponseStatus.Exception,
                    Messages = new List<string> { "An unexpected error occurred." }
                }.ToActionResult();
            }
        }

        /// <summary>
        /// Retrieves the list of beneficiaries for a user.
        /// </summary>
        /// <param name="userId">The user's identifier.</param>
        /// <returns>A list of beneficiaries.</returns>
        [HttpGet("GetBeneficiaries")]
        public async Task<ActionResult<ResponseBO<List<BeneficiaryDTO>>>> GetBeneficiaries(int userId)
        {
            try
            {
                var response = await _beneficiaryService.GetBeneficiariesAsync(userId);
                return response.ToActionResult();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in GetBeneficiaries");
                return new ResponseBO<List<BeneficiaryDTO>>
                {
                    Status = ResponseBO<List<BeneficiaryDTO>>.ResponseStatus.Exception,
                    Messages = new List<string> { "An unexpected error occurred." }
                }.ToActionResult();
            }
        }

    }
}
