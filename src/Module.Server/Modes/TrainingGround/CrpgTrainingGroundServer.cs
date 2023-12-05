using Crpg.Module.Common;
using Crpg.Module.Rewards;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.ObjectSystem;

namespace Crpg.Module.Modes.TrainingGround;

internal class CrpgTrainingGroundServer : MissionMultiplayerGameModeBase
{
    private readonly MissionScoreboardComponent _scoreboardComponent;
    private readonly CrpgRewardServer _rewardServer;

    private MissionTimer? _rewardTickTimer;

    public override bool IsGameModeHidingAllAgentVisuals => true;
    public override bool IsGameModeUsingOpposingTeams => false;

    public CrpgTrainingGroundServer(
        MissionScoreboardComponent scoreboardComponent,
        CrpgRewardServer rewardServer)
    {
        _scoreboardComponent = scoreboardComponent;
        _rewardServer = rewardServer;
    }

    public override MissionLobbyComponent.MultiplayerGameType GetMissionType()
    {
        return MissionLobbyComponent.MultiplayerGameType.FreeForAll; // Avoids a crash on mission end.
    }

    public override void AfterStart()
    {
        AddTeam();
    }

    public override void OnClearScene()
    {
        // https://forums.taleworlds.com/index.php?threads/missionbehavior-onmissionrestart-is-never-called.458204
        _scoreboardComponent.ClearScores();
        ClearPeerCounts();
    }

    public override void OnMissionTick(float dt)
    {
        RewardUsers();
    }

    public override void OnAgentRemoved(Agent affectedAgent, Agent? affectorAgent, AgentState agentState, KillingBlow blow)
    {
        if (blow.DamageType == DamageTypes.Invalid
            || (agentState != AgentState.Unconscious && agentState != AgentState.Killed)
            || !affectedAgent.IsHuman)
        {
            return;
        }
    }

    protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
        networkPeer.AddComponent<FFAMissionRepresentative>();
    }

    protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        component.Team = Mission.AttackerTeam;
        component.Culture = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
    }

    private void AddTeam()
    {
        BasicCultureObject cultureTeam1 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue());
        Banner bannerTeam1 = new(cultureTeam1.BannerKey, cultureTeam1.BackgroundColor1, cultureTeam1.ForegroundColor1);
        Team team = Mission.Teams.Add(BattleSideEnum.Attacker, cultureTeam1.BackgroundColor1, cultureTeam1.ForegroundColor1, bannerTeam1, false, true);
        team.SetIsEnemyOf(team, true);
    }

    private void RewardUsers()
    {
        _rewardTickTimer ??= new MissionTimer(duration: CrpgServerConfiguration.RewardTick);
        if (_rewardTickTimer.Check(reset: true))
        {
            _ = _rewardServer.UpdateCrpgUsersAsync(
                durationRewarded: _rewardTickTimer.GetTimerDuration() * 0.75f, // Reduce duration reward as multiplier must be int
                durationUpkeep: 0,
                constantMultiplier: 1);
        }
    }
}
