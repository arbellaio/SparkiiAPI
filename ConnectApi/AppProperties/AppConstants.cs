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
        public string Secret { get; set; }



        //One API Key and AppId
        public const string OneSignalAppId = "";
        public const string OneSignalApiKey = "";

        public const string TwilioAccountSid = ""; //"";
        public const string TwilioAuthToken = "";//"";
        public const string TwilioFromPhoneNumber = ""; //"";



    }
}
