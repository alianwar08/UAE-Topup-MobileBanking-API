using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BalanceService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BalanceServiceController : ControllerBase
    {
        static Dictionary<int, decimal> UserBalance = new Dictionary<int, decimal>();
        [HttpGet("GetBalance/{userId}")]
        public async Task<ActionResult<decimal>> GetBalance(int userId)
        {
            if (UserBalance.ContainsKey(userId))
                return Ok(UserBalance[userId]);
            else
                return Ok(0);
            
            
        }

        [HttpPost("UpdateBalance")]
        public async Task<ActionResult> UpdateBalance(int userId, decimal amount)
        {

            if (UserBalance.Count > 99999)
                throw new Exception("Users limi reached for BalanceService");
            UserBalance[userId] = GetBalance(userId).Result.Value + amount;
            return Ok();

            
        }
    }
}
