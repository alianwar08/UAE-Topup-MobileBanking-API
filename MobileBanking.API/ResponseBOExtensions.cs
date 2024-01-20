using Microsoft.AspNetCore.Mvc;
using MobileBanking.BusinessLogic;

namespace MobileBanking.API
{

    public static class ResponseBOExtensions
    {
        public static ActionResult<ResponseBO<T>> ToActionResult<T>(this ResponseBO<T> response)
        {
            switch (response.Status)
            {
                case ResponseBO<T>.ResponseStatus.Success:
                    return new OkObjectResult(response); 

                case ResponseBO<T>.ResponseStatus.Error:
                    return new BadRequestObjectResult(response);

                case ResponseBO<T>.ResponseStatus.Exception:
                    return new ObjectResult(response) { StatusCode = 500 }; 

                default:
                    return new ObjectResult(response) { StatusCode = 500 }; 

            }
        }

        public static ActionResult<ResponseBO> ToActionResult(this ResponseBO response)
        {
            switch (response.Status)
            {
                case ResponseBO.ResponseStatus.Success:
                    return new OkObjectResult(response);

                case ResponseBO.ResponseStatus.Error:
                    return new BadRequestObjectResult(response);

                case ResponseBO.ResponseStatus.Exception:
                    return new ObjectResult(response) { StatusCode = 500 };

                default:
                    return new ObjectResult(response) { StatusCode = 500 };

            }
        }
    }

}
