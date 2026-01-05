using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using SimpleShopApi.BLL;
using SimpleShopApi.Models.Requests;
using SimpleShopApi.Models.Responses;
using SimpleShopApi.Utils;

namespace SimpleShopApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController(ApiKeyValidator apiKeyValidator, UserService userService) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request, [FromQuery] string apiKey)
        {
            try
            {
                if (!await apiKeyValidator.IsValidAsync(apiKey))
                    return Forbid("Invalid api key");

                var res = await userService.CreateUserAsync(request);
                if (res is Success<object> success)
                    return Ok(success.Value);
                else
                    return BadRequest(res);
            }
            catch
            {
                return BadRequest(ServiceResponse.Failed("Undefined error"));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, [FromQuery] string apiKey)
        {
            try
            {
                if (!await apiKeyValidator.IsValidAsync(apiKey))
                    return Forbid("Invalid api key");

                var res = await userService.LoginUserAsync(request);
                if (res is Success)
                    return Ok(res);
                else
                    return BadRequest(res);
            }
            catch
            {
                return BadRequest(ServiceResponse.Failed("Undefined error"));
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUser([FromHeader(Name = "Authorization")] string token, [FromQuery] string apiKey)
        {
            try
            {
                if (!await apiKeyValidator.IsValidAsync(apiKey))
                    return Forbid("Invalid api key");

                var res = await userService.GetUserAsync(token);
                if (res is Success)
                    return Ok(res);
                else
                    return BadRequest(res);
            }
            catch
            {
                return BadRequest(ServiceResponse.Failed("Undefined error"));
            }
        }

        [HttpPatch("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromHeader(Name = "Authorization")] string token, [FromBody] UpdateUserRequest request, [FromQuery] string apiKey)
        {
            try
            {
                if (!await apiKeyValidator.IsValidAsync(apiKey))
                    return Forbid("Invalid api key");

                var res = await userService.UpdateUserAsync(token, request);
                if (res is Success)
                    return Ok(res);
                else
                    return BadRequest(res);
            }
            catch
            {
                return BadRequest(ServiceResponse.Failed("Undefined error"));
            }
        }

        [HttpPatch("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromHeader(Name = "Authorization")] string token, [FromBody] ChangePasswordRequest request, [FromQuery] string apiKey)
        {
            try
            {
                if (!await apiKeyValidator.IsValidAsync(apiKey))
                    return Forbid("Invalid api key");

                var res = await userService.ChangePasswordAsync(token, request);
                if (res is Success)
                    return Ok(res);
                else
                    return BadRequest(res);
            }
            catch
            {
                return BadRequest(ServiceResponse.Failed("Undefined error"));
            }
        }

        [HttpPatch("sendResetPasswordCode")]
        public async Task<IActionResult> ResetPassword([FromBody] string email,[FromQuery] string apiKey)
        {
            try
            {
                if (!await apiKeyValidator.IsValidAsync(apiKey))
                    return Forbid("Invalid api key");

                var res = await userService.SendResetPasswordCodeAsync(email);
                if (res is Success)
                    return Ok(res);
                else
                    return BadRequest(res);
            }
            catch
            {
                return BadRequest(ServiceResponse.Failed("Undefined error"));
            }
        }

        [HttpPatch("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, [FromQuery] string apiKey)
        {
            try
            {
                if (!await apiKeyValidator.IsValidAsync(apiKey))
                    return Forbid("Invalid api key");

                var res = await userService.ResetPasswordAsync(request);
                if (res is Success)
                    return Ok(res);
                else
                    return BadRequest(res);
            }
            catch
            {
                return BadRequest(ServiceResponse.Failed("Undefined error"));
            }
        }

        [HttpPatch("sendConfirmEmailCode")]
        [Authorize]
        public async Task<IActionResult> ConfirmEmail([FromHeader(Name = "Authorization")] string token, [FromQuery] string apiKey)
        {
            try
            {
                if (!await apiKeyValidator.IsValidAsync(apiKey))
                    return Forbid("Invalid api key");

                var res = await userService.SendConfirmEmailCodeAsync(token);
                if (res is Success)
                    return Ok(res);
                else
                    return BadRequest(res);
            }
            catch
            {
                return BadRequest(ServiceResponse.Failed("Undefined error"));
            }
        }

        [HttpPatch("confirmEmail")]
        [Authorize]
        public async Task<IActionResult> ConfirmEmail([FromHeader(Name = "Authorization")] string token, [FromQuery] string apiKey,[FromBody] string code)
        {
            try
            {
                if (!await apiKeyValidator.IsValidAsync(apiKey))
                    return Forbid("Invalid api key");

                var res = await userService.ConfirmEmailAsync(token, code);
                if (res is Success)
                    return Ok(res);
                else
                    return BadRequest(res);
            }
            catch
            {
                return BadRequest(ServiceResponse.Failed("Undefined error"));
            }
        }
    }
}
