using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ConnectApi.AppProperties
{
    public class AppConstants
    {
        // All the Constants Used in Web App
        public const int OtpCodeLength = 6; // OTP Code Length
        public const int OtpNumberOfAttempts = 3; // Can Ask for New OTP this many times
        public const int DurationUntilOtpCodeActive = 8; // Duration until OTP Code stays Active before expiring


        // JWT Token Constants
        public const string AppSecret = "AppSettings:Secret";
        public const string AppSecretKey = "SparkiiSecretKey";
        public const int TokenHandlerSize = 1;
        public const int TokenExpireTimeDuration= 9999;

       

        //One API Key and AppId
        public const string OneSignalAppId = "3399eeba-1516-4e78-8867-d2eee5a0a4ed";
        public const string OneSignalApiKey = "YWNmNGQwMjMtODQ1MC00MDUwLWExNWYtYTZhMThkOTNhYWNm";

        public const string TwilioAccountSid = "AC987493e37ffee3357e84002d6920d119"; //"ACa02f9462920d0d1bb1c7f90552dfbf5b";
        public const string TwilioAuthToken = "75867b180dc29766cd7b9ea6cb543833";//"23b62f41a2ad00ffbc64e1f345ae6b45";
        public const string TwilioFromPhoneNumber = "+13345390248"; //"+13145820655";



    }
}
