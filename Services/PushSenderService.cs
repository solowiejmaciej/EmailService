﻿using System.Text.Json;
using AuthService.Services;
using AuthService.UserContext;
using Google.Apis.Auth.OAuth2;
using Google.Apis.FirebaseCloudMessaging.v1;
using Google.Apis.FirebaseCloudMessaging.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using NotificationService.Models;
using NotificationService.Models.AppSettings;

namespace NotificationService.Services
{
    public class PushSenderService : IPushSenderService
    {
        private readonly IOptions<GoogleFirebaseConfig> _config;
        private readonly IUserService _userService;

        public PushSenderService(IOptions<GoogleFirebaseConfig> config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }

        public void SendPushNow(PushRequest pushRequest, string userId)
        {
            FirebaseCloudMessagingService firebaseCloudMessagingService = new FirebaseCloudMessagingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromJson(JsonSerializer.Serialize(_config.Value))
            });

            string deviceId = _userService.GetById(userId).DeviceId;
            var pushNotification = new SendMessageRequest()
            {
                Message = new Message()
                {
                    Token = deviceId,
                    Notification = new Notification()
                    {
                        Title = pushRequest.PushTitle,
                        Body = pushRequest.PushContent
                    }
                },
            };
            try
            {
                firebaseCloudMessagingService.Projects.Messages.Send(pushNotification, $"projects/{_config.Value.project_id}").Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wystąpił błąd podczas wysyłania powiadomienia push: " + ex.Message);
            }
        }
    }

    public interface IPushSenderService
    {
        void SendPushNow(PushRequest pushRequest, string id);
    }
}