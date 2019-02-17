using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConnectApi.AppProperties;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;

namespace ConnectApi.Helpers.TwilioSendSmsWeb
{
    public class TwilioSmsSendHelper
    {
        public async Task<string> SendSms(string messageContent,string phoneNumberTo)
        {
            try
            {
                // Find your Account Sid and Token at twilio.com/console
                const string accountSid = AppConstants.TwilioAccountSid;
                const string authToken = AppConstants.TwilioAuthToken;
                const string phoneNumberFrom = AppConstants.TwilioFromPhoneNumber; 

                TwilioClient.Init(accountSid, authToken);

                var message = await MessageResource.CreateAsync(
                    body: messageContent,
                    from: new Twilio.Types.PhoneNumber(phoneNumberFrom),
                    to: new Twilio.Types.PhoneNumber(phoneNumberTo)
                );

                Console.WriteLine(message.Sid);
                return messageContent;
            }
            catch (ApiException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine($"Twilio Error {e.Code} - {e.MoreInfo}");
                return $"Twilio Error {e.Message} - {e.Code}";
            }

        }
    }
}
