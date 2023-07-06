using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotificationService.Entities.NotificationEntities;
using NotificationService.Exceptions;
using NotificationService.Models;
using NotificationService.Models.AppSettings;
using NotificationService.Repositories;
using NotificationService.Services;
using RestSharp;

namespace NotificationService.Hangfire.Jobs;

public class SmsDeliveryProcessingJob
{
    private readonly ILogger<SmsDeliveryProcessingJob> _logger;
    private readonly ISmsRepository _repo;
    private readonly SmsConfig _config;

    public SmsDeliveryProcessingJob(
        ILogger<SmsDeliveryProcessingJob> logger,
        IOptions<SmsConfig> config,
        ISmsRepository repository
        )
    {
        _logger = logger;
        _repo = repository;
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
        SmsNotification sms,
        Recipient recipient,
        PerformContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("SmsDeliveryProcessingJob invoked");
        if (sms == null)
        {
            throw new NullReferenceException("Sms can't be null");
        }

        if (recipient == null)
        {
            _repo.ChangeSmsStatus(sms.Id, EStatus.HasErrors);
            throw new NotFoundException("user not found");
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
        request.AddParameter("to", recipient.PhoneNumber);
        request.AddParameter("msg", sms.Content);

        var response = await client.ExecuteAsync<ErrorResponse>(request, cancellationToken);
        _logger.LogInformation($"Request fired to {baseUrl}");

        var data = response.Data;

        if (data is null) throw new Exception("SMS API responed with null");

        if (!data.errorMsg.IsNullOrEmpty())
        {
            _repo.ChangeSmsStatus(sms.Id, EStatus.HasErrors);
            _logger.LogInformation($"SMS failed to send reason: {data.errorMsg} Code {data.errorCode}");
        }
        else
        {
            _repo.ChangeSmsStatus(sms.Id, EStatus.Send);
            _logger.LogInformation($"Sms sent successfully");
        }
    }
}