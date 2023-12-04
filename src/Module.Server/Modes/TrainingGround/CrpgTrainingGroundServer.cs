using System.Xml.Serialization;
using Crpg.Module.Rewards;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace Crpg.Module.Modes.TrainingGround;

internal class CrpgTrainingGroundServer : MissionMultiplayerGameModeBase
{
    private readonly CrpgRewardServer _rewardServer;
    private bool _gameStarted;


    public CrpgTrainingGroundServer(CrpgRewardServer rewardServer)
    {
        _rewardServer = rewardServer;
    }

    public override bool IsGameModeHidingAllAgentVisuals => true;
    public override bool IsGameModeUsingOpposingTeams => false;
    public override bool AllowCustomPlayerBanners() => false;
    public override bool UseRoundController() => false;

    private CrpgTrainingGroundSpawningBehavior SpawningBehavior => (CrpgTrainingGroundSpawningBehavior)SpawnComponent.SpawningBehavior;

    public override MissionLobbyComponent.MultiplayerGameType GetMissionType()
    {
        return MissionLobbyComponent.MultiplayerGameType.Battle;
    }

    public override void AfterStart()
    {
        base.AfterStart();
    }

    public override void OnBehaviorInitialize()
    {
        base.OnBehaviorInitialize();
    }

    public override void OnRemoveBehavior()
    {
        base.OnRemoveBehavior();
    }

    public override bool CheckForWarmupEnd()
    {
        return true;
    }

    public override void OnPeerChangedTeam(NetworkCommunicator networkPeer, Team oldTeam, Team newTeam)
    {
        var missionPeer = networkPeer.GetComponent<MissionPeer>();
        if (missionPeer == null || newTeam == Mission.SpectatorTeam)
        {
            return;
        }

        missionPeer.Team = Mission.DefenderTeam;
    }

    public override void OnAgentBuild(Agent agent, Banner banner)
    {
        base.OnAgentBuild(agent, banner);
        // Synchronize health with all clients to make the spectator health bar work.
        agent.UpdateSyncHealthToAllClients(true);
    }

    public override void OnMissionTick(float dt)
    {
        base.OnMissionTick(dt);
        if (MissionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Playing
            || !CanGameModeSystemsTickThisFrame)
        {
            return;
        }
    }

    protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
        if (!_gameStarted)
        {
            return;
        }
    }

    private void SendDataToPeers(GameNetworkMessage message)
    {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage(message);
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }
}
