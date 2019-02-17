using ConnectApi.AppProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OneSignal.RestAPIv3.Client;
using OneSignal.RestAPIv3.Client.Resources;
using OneSignal.RestAPIv3.Client.Resources.Notifications;

namespace ConnectApi.Helpers.GenerateOTP
{
    public class GenerateOtp
    {
        // Otp 4 Digit Number
        public string GetOtpNumber()
        {
            string num = "0123456789";
            int lenght = num.Length;
            string otp = string.Empty;
            int otpDigit = AppConstants.OtpCodeLength;
            string finalDigit;
            int getIndex;
            for (int i = 0; i < otpDigit; i++)
            {
                do
                {
                    getIndex = new Random().Next(0,lenght);
                    finalDigit = num.ToCharArray()[getIndex].ToString();
                } while (otp.IndexOf(finalDigit) != -1);

                otp += finalDigit;
            }

            return otp;
        }

        
        
        //RandomString For Token Generation
        public string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

       
        
      
    }


   
}
