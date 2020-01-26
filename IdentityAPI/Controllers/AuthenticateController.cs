using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityAPI.CustomExceptions;
using IdentityAPI.Data;
using IdentityAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace IdentityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {

        #region Variables

        private UserManager<AppUser> _userManager;
        //  private IServiceProvider _serviceProvider;
        private IConfiguration _configuration;
        #endregion

        #region Constructor
        public AuthenticateController(UserManager<AppUser> userManager,
                                      IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        #endregion


        #region Actions
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegistrationDTO signUp)
        {
            //1. check if user exist in DB
            var user = await _userManager.FindByEmailAsync(signUp.Email);

            if (user != null)
            {
                return Conflict("Email ID is already registered");
            }
            var applicationUser = new AppUser()
            {
                Email = signUp.Email,
                UserName = signUp.Email,
            };

            //2. if not exist then create
            var result = await _userManager.CreateAsync(applicationUser, signUp.Password);

            //3. Generate Email confirmation URL
            if (result == null || !result.Succeeded)
            {
                return BadRequest("Not created in database");
            }

            //4. send an email with email confirmation url
            var emailConfirmationToken = _userManager.GenerateEmailConfirmationTokenAsync(applicationUser).Result;

            var baseUrl = _configuration.GetValue<string>("BaseUrl");
            var emailConfirmationUrl = "";
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                emailConfirmationUrl = baseUrl +
                                      "/api/Authenticate/EmailConfirm?Email=" +
                                      applicationUser.Email + "&Token=" + emailConfirmationToken;
            }

            return Ok("Created Successfully!");
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginDTO login)
        {
            var user = await _userManager.FindByNameAsync(login.Username);

            if (await _userManager.CheckPasswordAsync(user, login.Password))
            {
                throw new Exception();
            }

            if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
            {

                var authClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var secureKey = _configuration.GetValue<string>("Secure_Machine_Key");
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secureKey));

                var token = new JwtSecurityToken(
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            return Unauthorized();
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPassword)
        {
            //1. Get user using email

            var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
            if (user == null)
            {
                return BadRequest("Email is not present in the database");
            }

            //2. Get reset token for this user
            var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            //3.Send  reset password link with token through email
            //1.  Generate reset password link

            var link = _configuration.GetValue<string>("Base_Url") + "/api/Authenticate/ResetPassword?EmailId=" +
                                                                    user.Email +
                                                                    "&Token=" + resetPasswordToken;

            return Ok("Password reset link has been sent to your registered email");

        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPassword)
        {

            //1. Get user using email

            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
            {
                return BadRequest("Invalid link as email does not exist in the database");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password has been changed successfully");
            }

            return BadRequest("Password is not changed. try again");

        }

        #endregion
    }
}