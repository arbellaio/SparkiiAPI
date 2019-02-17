using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConnectApi.AppProperties;
using ConnectApi.Models.Users;
using OneSignal.RestAPIv3.Client;
using OneSignal.RestAPIv3.Client.Resources;
using OneSignal.RestAPIv3.Client.Resources.Notifications;

namespace ConnectApi.Helpers.OneSignal
{
    public class OneSignalPushNotificationHelper
    {
        public int SendOneSignalPushNotification(List<string> playerIds, User user, string message)
        {
            // OneSignal Notification Generator
            var client = new OneSignalClient(AppConstants.OneSignalApiKey); // Use your Api Key
            var options = new NotificationCreateOptions
            {
                AppId = new Guid(AppConstants.OneSignalAppId),   // Use your AppId
                IncludePlayerIds = playerIds
//                    new List<string>()
//                {
//                    "00000000-0000-0000-0000-000000000000" // Use your playerId
//                }
            };
            options.Headings.Add(LanguageCodes.English, "New Notification!");
            options.Contents.Add(LanguageCodes.English, message+" from "+user.Name);

            var response = client.Notifications.Create(options);
            return response.Recipients;
        }
    }
}
