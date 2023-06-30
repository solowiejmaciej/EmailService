using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Entities;
using NotificationService.Models.AppSettings;
using NotificationService.Services;

using RestSharp;

namespace NotificationService.Hangfire.Jobs;

public class SmsDeliveryProcessingJob
{
    private readonly ILogger<SmsDeliveryProcessingJob> _logger;
    private readonly ISmsService _smsService;
    private readonly SmsConfig _config;

    public SmsDeliveryProcessingJob(
        ILogger<SmsDeliveryProcessingJob> logger,
        IOptions<SmsConfig> config,
        ISmsService smsService
        )
    {
        _logger = logger;
        _smsService = smsService;
        _config = config.Value;
    }

    private class ErrorResponse
    {
        public int errorCode { get; set; }
        public string errorMsg { get; set; }
    }

    [AutomaticRetry(Attempts = 0)]
    [JobDisplayName("SmsDeliveryProcessingJob")]
    [Queue(HangfireQueues.DEFAULT)]
    public async Task Send(
        Sms sms,
        PerformContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("SmsDeliveryProcessingJob invoked");
        if (sms == null)
        {
            throw new NullReferenceException("Sms can't be null");
        }

        var baseUrl = _config.ApiUrl;

        var options = new RestClientOptions(baseUrl);
        var client = new RestClient(options);
        var request = new RestRequest("/sms", Method.Post);

        _logger.LogInformation("Creating request");

        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("key", _config.Key);
        request.AddParameter("password", _config.Password);
        request.AddParameter("from", _config.SenderName);
        request.AddParameter("to", sms.To);
        request.AddParameter("msg", sms.Body);

        var response = await client.ExecuteAsync<ErrorResponse>(request, cancellationToken);
        _logger.LogInformation($"Request fired to {baseUrl}");

        var data = response.Data;
        if (!data.errorMsg.IsNullOrEmpty())
        {
            _smsService.ChangeSmsStatus(sms.Id, SmsStatus.HasErrors);
            _logger.LogInformation($"SMS failed to send reason: {data.errorMsg} Code{data.errorCode}");
        }
        else
        {
            _smsService.ChangeSmsStatus(sms.Id, SmsStatus.Send);
            _logger.LogInformation($"Sms sent successfully");
        }
    }
}