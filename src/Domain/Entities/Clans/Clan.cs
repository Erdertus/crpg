﻿using Crpg.Domain.Common;
using Crpg.Domain.Entities.Items;

namespace Crpg.Domain.Entities.Clans;

public class Clan : AuditableEntity
{
    public int Id { get; set; }

    /// <summary>
    /// Short name of the clan.
    /// </summary>
    public string Tag { get; set; } = string.Empty;

    /// <summary>
    /// Primary color (ARGB32) of the clan.
    /// </summary>
    public uint PrimaryColor { get; set; }

    /// <summary>
    /// Secondary color (ARGB32) of the clan.
    /// </summary>
    public uint SecondaryColor { get; set; }

    /// <summary>
    /// Full name of the clan.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the clan.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Bannerlord's banner key of the clan.
    /// </summary>
    public string BannerKey { get; set; } = string.Empty;

    /// <summary>
    /// Region of the clan.
    /// </summary>
    public Region Region { get; set; }

    public int ArmoryMinRank { get; set; }
    public int ArmoryUsageCost { get; set; }
    public int ArmoryBorrowFee { get; set; }
    public TimeSpan ArmoryTimeout { get; set; } = TimeSpan.FromDays(3);  // return an item if a borrower is not active for N days
    public IList<ArmoryItem> ArmoryItems { get; set; } = new List<ArmoryItem>();
    public IList<ArmoryBorrow> ArmoryBorrows { get; set; } = new List<ArmoryBorrow>();

    /// <summary>
    /// Discord url of the clan.
    /// </summary>
    public Uri? Discord { get; set; }

    public IList<ClanMember> Members { get; set; } = new List<ClanMember>();
    public IList<ClanInvitation> Invitations { get; set; } = new List<ClanInvitation>();
}
