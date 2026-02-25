namespace Backend.Foundation.Template.Infrastructure.Messaging;

internal sealed class MessagingOptions
{
    public const string SectionName = "Messaging";

    public string Provider { get; set; } = "None";
}
