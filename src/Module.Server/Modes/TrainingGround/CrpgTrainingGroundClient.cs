using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.MissionRepresentatives;

namespace Crpg.Module.Modes.TrainingGround;

internal class CrpgTrainingGroundClient : MissionMultiplayerGameModeBaseClient
{
    public TrainingGroundMissionRepresentative? MyRepresentative { get; private set; }
    public event Action OnMyRepresentativeAssigned = default!;
    public override bool IsGameModeUsingGold => false;
    public override bool IsGameModeTactical => false;
    public override bool IsGameModeUsingRoundCountdown => false;
    public override MissionLobbyComponent.MultiplayerGameType GameType => MissionLobbyComponent.MultiplayerGameType.FreeForAll;

    public override void OnBehaviorInitialize()
    {
        base.OnBehaviorInitialize();
        MissionNetworkComponent.OnMyClientSynchronized += OnMyClientSynchronized;
    }

    public override void AfterStart()
    {
        Mission.SetMissionMode(MissionMode.Battle, true);
    }

    public override int GetGoldAmount()
    {
        return 0;
    }

    public override void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount)
    {
    }

    public override void OnRemoveBehavior()
    {
        MissionNetworkComponent.OnMyClientSynchronized -= OnMyClientSynchronized;
        base.OnRemoveBehavior();
    }

    private void OnMyClientSynchronized()
    {
        MyRepresentative = GameNetwork.MyPeer.GetComponent<TrainingGroundMissionRepresentative>();
        OnMyRepresentativeAssigned?.Invoke();
    }
}
