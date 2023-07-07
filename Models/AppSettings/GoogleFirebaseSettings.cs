using System.ComponentModel.DataAnnotations;

namespace NotificationService.Models.AppSettings;

public class GoogleFirebaseSettings
{
    [Required(AllowEmptyStrings = false)]
    public string? type { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? project_id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? private_key_id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? private_key { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? client_email { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? client_id { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? auth_uri { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? token_uri { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? auth_provider_x509_cert_url { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? client_x509_cert_url { get; set; }

    [Required(AllowEmptyStrings = false)]
    public string? universe_domain { get; set; }
}