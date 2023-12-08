using AutoMapper;
using Crpg.Application.ActivityLogs.Models;
using Crpg.Application.Common.Interfaces;
using Crpg.Application.Common.Mediator;
using Crpg.Application.Common.Results;
using Crpg.Domain.Entities.ActivityLogs;
using Microsoft.EntityFrameworkCore;

namespace Crpg.Application.Characters.Queries;

public record GetUserCharacterStatisticsChartsQuery : IMediatorRequest<IList<ActivityLogViewModel>>
{
    public int CharacterId { get; init; }
    public int UserId { get; init; }

    internal class Handler : IMediatorRequestHandler<GetUserCharacterStatisticsChartsQuery, IList<ActivityLogViewModel>>
    {
        private readonly ICrpgDbContext _db;
        private readonly IMapper _mapper;

        public Handler(ICrpgDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<IList<ActivityLogViewModel>>> Handle(GetUserCharacterStatisticsChartsQuery req,
            CancellationToken cancellationToken)
        {
            var activityLogs = await _db.ActivityLogs
                .Include(l => l.Metadata)
                .Where(l =>
                    l.UserId == req.UserId
                    && l.CreatedAt >= DateTime.UtcNow.AddDays(-14)
                    && l.CreatedAt <= DateTime.UtcNow
                    && l.Type == ActivityLogType.CharacterGain)
                .OrderByDescending(l => l.CreatedAt)
                .Take(1000)
                .ToArrayAsync(cancellationToken);
            return new(_mapper.Map<IList<ActivityLogViewModel>>(activityLogs));
        }
    }
}
