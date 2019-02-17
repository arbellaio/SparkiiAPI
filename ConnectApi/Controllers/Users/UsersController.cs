using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ConnectApi.AppProperties;
using ConnectApi.Dtos.OtpDto;
using ConnectApi.Dtos.UserDto;
using ConnectApi.Helpers.GenerateOTP;
using ConnectApi.Helpers.JsonResponse;
using ConnectApi.Helpers.JwtTokenGen;
using ConnectApi.Helpers.TwilioSendSmsWeb;
using ConnectApi.Models.Users;
using ConnectApi.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ConnectApi.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUserService _service;
        private readonly IConfiguration _config;

        public UsersController(IUserService service, IConfiguration config)
        {
            _service = service ?? throw new ArgumentNullException("User Service");
            _config = config ?? throw new ArgumentNullException("Configuration");
        }


        // Verify Token
        // Login WebAPI
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    LoginResponse =
                        new JsonLoginResponseHandler
                        {
                            Token = null,
                            Response = new JsonResponseHandler { IsSuccess = false }
                        }
                });

            // Service Method To Check if User Exists
            var userExists = await _service.UserExists(userLoginDto.Email.ToLower());
            if (!userExists)
            {
                return BadRequest(new
                {
                    Token = (string)null,
                    Response = new JsonResponseHandler
                    {
                        ErrorMessage = AppStrings.UserDoesntExist,
                        IsSuccess = false,
                    }
                }
                );
            }
            // Service Method To Login User
            var userInDb = await _service.Login(userLoginDto.Email.ToLower(), userLoginDto.Password);
            if (userInDb == null)
            {
                return BadRequest(new
                {
                    Token = (string)null,
                    Response = new JsonResponseHandler
                    {
                        ErrorMessage = AppStrings.EmailOrPasswordInvalid,
                        IsSuccess = false,
                    }
                });
            }

            // Change User Status
            var status = await _service.ChangeUserActiveStatusInDb(userInDb.Id);
            var changeContactStatusResponse = await _service.ChangeStatusForAllContactsWhereUserIsSaved(userInDb.PhoneNumber);
            var notificationSendResponse = await _service.SendPushNotificationToAllActiveContactsOnStatusActive(userInDb.Id);

            var userDto = new UserResponseDto
            {
                Id = userInDb.Id,
                Email = userInDb.Email,
                Password = userInDb.Password,
                Name = userInDb.Name,
                PhoneNumber = userInDb.PhoneNumber,
                UserName = userInDb.UserName,
                Token = userInDb.Token,
                CreatedDate = userInDb.CreatedDate,
                ActiveShowTime = userInDb.ActiveShowTime,
                Contacts = userInDb.Contacts,
                Status = status,
                OneSignalUserId = userInDb.OneSignalUserId,
                TwilioUserId = userInDb.TwilioUserId,
            };

            var jwtTokenHelper = new JwtTokenHelper();
            var jwtToken = jwtTokenHelper.GenerateToken(userInDb);


            //Add Or Retrieve Token against specific User
            await _service.SaveTokenInDb(userInDb.Id, jwtToken);
            return Ok(new
            {
                User = userDto,
                Token = jwtToken,
                Response = new JsonResponseHandler
                {
                    IsSuccess = true,
                }
            }
            );
        }




        //Send OTP Against Phone Number
        [AllowAnonymous]
        [HttpPost("sendotp")]
        public async Task<IActionResult> SendOtpCode([FromBody] GetOTP getOtp)
        {
            int userId = 0;
            int numberOfAttempts = 1;

            var otp = new GenerateOtp();

            if (string.IsNullOrEmpty(getOtp.PhoneNumber))
            {
                return BadRequest(new
                {
                    Response = new JsonResponseHandler
                    {
                        IsSuccess = false,
                        ErrorMessage = AppStrings.PhoneNumberOrCountryCodeEmpty
                    }
                });
            }

            // Check if Phone Number is Digits Only
//            var checkPhoneNumberCorrect = _service.IsDigitsOnly(getOtp.PhoneNumber);
//            if (!checkPhoneNumberCorrect)
//            {
//                return BadRequest(new
//                {
//                    Response = new JsonResponseHandler
//                    {
//                        IsSuccess = false,
//                        ErrorMessage = AppStrings.InvalidPhoneNumberOrCountryCode,
//                    }
//                });
//            }


            // Generating OTP Passcode
            var otpPassCode = otp.GetOtpNumber();
            var otpSms = AppStrings.ConnectApi + otpPassCode;

            //Get UserID with PhoneNumber
            var user = await _service.GetUserByPhoneNumber(getOtp.PhoneNumber);

            if (user != null)
            {
                userId = user.Id;
            }


            //To Save Otp Code Against PhoneNumber 
            await _service.SaveGeneratedOtpAgainstPhoneNumber(otpPassCode, getOtp.PhoneNumber, numberOfAttempts,
                userId);

            //Send OTP Code To Mobile
            var twilioSmsHelper = new TwilioSmsSendHelper();
            var responseTwilio = await twilioSmsHelper.SendSms(otpSms, getOtp.PhoneNumber);


            return Ok(new
            {
                Code = otpPassCode,
                Response = new JsonResponseHandler
                {
                    IsSuccess = true,
                    ErrorMessage = responseTwilio,
                }
            });
        }



        // Resend OTP
        [AllowAnonymous]
        [HttpPost("resendotp")]
        public async Task<IActionResult> ResendOtp([FromBody] GetOTP getOtp)
        {
            int userId = 0;
            var otp = new GenerateOtp();

            if (string.IsNullOrEmpty(getOtp.PhoneNumber))
            {
                return BadRequest(new
                {
                    Response = new JsonResponseHandler
                    {
                        IsSuccess = false,
                        ErrorMessage = AppStrings.PhoneNumberOrCountryCodeEmpty
                    }
                });
            }

            // Generating OTP Passcode
            var otpPassCode = otp.GetOtpNumber();
            var otpSms = AppStrings.ConnectApi + otpPassCode;

            // Get Number Of OTP Attemps
            int numberOfAttempts = await _service.GetNumberOfAttempts(getOtp.PhoneNumber);

            //Get UserID with PhoneNumber
            var user = await _service.GetUserByPhoneNumber(getOtp.PhoneNumber);
            if (user != null)
            {
                userId = user.Id;
            }

            //Get UserOTP with PhoneNumber
            var userOtp = await _service.GetUserOtpInfoByPhoneNumber(getOtp.PhoneNumber);

            // Exceeded OTP Attempts Response
            if (numberOfAttempts > AppConstants.OtpNumberOfAttempts ||
                userOtp.CreatedDate.AddMinutes(AppConstants.DurationUntilOtpCodeActive) < DateTime.UtcNow)
            {
                return BadRequest(new
                {
                    Response = new JsonResponseHandler
                    {
                        ErrorMessage = AppStrings.OtpCodeAttemptsExceeded,
                        IsSuccess = false
                    }
                });
            }

            ++numberOfAttempts;

            //To Save Otp Code Against PhoneNumber 
            await _service.SaveResentOtpAgainstPhoneNumber(otpPassCode, getOtp.PhoneNumber, numberOfAttempts, userId);

            //Send OTP Code To Mobile
            var twilioSmsHelper = new TwilioSmsSendHelper();
            var responseTwilio = await twilioSmsHelper.SendSms(otpSms, getOtp.PhoneNumber);
            return Ok(new
            {
                Code = otpPassCode,
                Response = new JsonResponseHandler
                {
                    IsSuccess = true,
                    ErrorMessage = responseTwilio
                }
            });
        }


        //Verify OtpCode
        [AllowAnonymous]
        [HttpPost("verifyotp")]
        public async Task<IActionResult> VerifyOtpCode([FromBody] GetOTP getOtp)
        {
            // Verify Otp
            var verified = await _service.VerifyOtpCode(getOtp.OtpCode);
            if (!verified)
            {
                return BadRequest(new
                {
                    Response = new JsonResponseHandler { ErrorMessage = AppStrings.WrongOtp, IsSuccess = false, }
                });
            }

            // Check if OTP time is expired
            var checkOtpLimit = await _service.CheckOtpTimeExpired(getOtp.OtpCode);
            if (!checkOtpLimit)
            {
                return BadRequest(new
                {
                    Response = new JsonResponseHandler { ErrorMessage = AppStrings.OtpTimeExpired, IsSuccess = false, }
                });
            }

            //            var user = await _service.GetUserByPhoneNumber(getOtp.PhoneNumber);

            return Ok(new
            {
                Response = new JsonResponseHandler
                {
                    IsSuccess = true,
                }
            });
        }




        // User Registration Controller Method
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto userRegisterDto)
        {
            bool isEmailValid = false;
            userRegisterDto.Email = userRegisterDto.Email.ToLower();
            if (new EmailAddressAttribute().IsValid(userRegisterDto.Email))
            {
                isEmailValid = true;
            }

            if (!isEmailValid)
            {
                return BadRequest(new
                {
                    UserId = "",
                    Token = "",
                    Response = new JsonResponseHandler
                    {
                        ErrorMessage = AppStrings.InvalidEmail,
                        IsSuccess = false
                    }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    UserId = "",
                    Token = "",
                    Response = new JsonResponseHandler
                    {
                        ErrorMessage = AppStrings.IncompleteOrInvalidInformation,
                        IsSuccess = false
                    }
                });
            }

            var user = await _service.GetUserByPhoneNumber(userRegisterDto.PhoneNumber);
            if (user != null)
            {
                return BadRequest(new
                {
                    UserId = "",
                    Token = "",
                    Response = new JsonResponseHandler
                    {
                        IsSuccess = false,
                        ErrorMessage = AppStrings.UserAlreadyExistsWithPhoneNumber
                    }
                });
            }

            if (await _service.UserExists(userRegisterDto.Email))
            {
                return BadRequest(new
                {
                    UserId = "",
                    Token = "",
                    Response = new JsonResponseHandler
                    {
                        IsSuccess = false,
                        ErrorMessage = AppStrings.UserAlreadyExistsWithEmailAddress
                    }
                });
            }

            

            var userToCreate = new User
            {
                UserName = userRegisterDto.Email,
                Password = userRegisterDto.Password,
                Email = userRegisterDto.Email,
                Name = userRegisterDto.Name,
                PhoneNumber = userRegisterDto.PhoneNumber,
                CreatedDate = DateTime.UtcNow.Date,
                Status = userRegisterDto.Status,
                ActiveShowTime = userRegisterDto.ActiveShowTime,
                OneSignalUserId = userRegisterDto.OneSignalUserId,
                TwilioUserId = userRegisterDto.TwilioUserId,
                Token = userRegisterDto.Token,
            };
         
            // Registering User
            var createdUser = await _service.Register(userToCreate, userToCreate.Password);


            //Token Generation for in SignUp
            var jwtTokenHelper = new JwtTokenHelper();
            var tokenStringJwt = jwtTokenHelper.GenerateToken(createdUser);
            await _service.SaveTokenInDb(createdUser.Id, tokenStringJwt);

            return Ok(new
            {
                UserId = createdUser.Id,
                Token = tokenStringJwt,
                Response = new JsonResponseHandler { IsSuccess = true }
            });
        }


    }
}