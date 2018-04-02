using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAnalysisBlock.Services
{
    public static class NotificationService
    {
        public static NotificationHubClient Hub = NotificationHubClient.CreateClientFromConnectionString(
            EnvironmentVariables.NotificationHubConnection, EnvironmentVariables.NotificationHubName);

        public static Task<string> CreateRegistrationIdInNotificationHubAsync() => Hub.CreateRegistrationIdAsync();

        public static async Task<string> DeleteAllRegistrationsByChannelAsync(string handle)
        {
            string newRegistrationId = null;
            var registrations = await Hub.GetRegistrationsByChannelAsync(handle, 100);

            foreach (RegistrationDescription registration in registrations)
            {
                if (newRegistrationId == null)
                {
                    newRegistrationId = registration.RegistrationId;
                }
                else
                {
                    await Hub.DeleteRegistrationAsync(registration);
                }
            }

            return newRegistrationId;
        }
    }
}
