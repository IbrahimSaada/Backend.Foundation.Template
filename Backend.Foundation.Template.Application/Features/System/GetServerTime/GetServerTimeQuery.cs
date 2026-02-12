using Backend.Foundation.Template.Application.Contracts.Requests;

namespace Backend.Foundation.Template.Application.Features.System.GetServerTime;

public sealed record GetServerTimeQuery : IQuery<DateTime>;
