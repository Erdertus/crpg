using System;
using System.Collections.Generic;
using System.Linq;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Modes.TrainingGround;
public class TrainingGroundMissionRepresentative : MissionRepresentativeBase
{
    public const int DuelPrepTime = 3;
    public event Action OnAgentSpawnedWithoutDuelEvent = default!;
    private IFocusable? _focusedObject;

    public int Bounty { get; private set; }

    public int Score { get; private set; }

    public int NumberOfWins { get; private set; }

    private bool _isInDuel
    {
        get
        {
            if (MissionPeer != null && MissionPeer.Team != null)
            {
                return MissionPeer.Team.IsDefender;
            }

            return false;
        }
    }

    public override void Initialize()
    {
        Mission.Current.SetMissionMode(MissionMode.Duel, atStart: true);
    }

    public void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
    {
        if (GameNetwork.IsClient)
        {
            GameNetwork.NetworkMessageHandlerRegisterer networkMessageHandlerRegisterer = new(mode);
        }
    }

    public void OnInteraction()
    {
        if (_focusedObject != null)
        {
            Debug.Print("Interacted with: " + _focusedObject.GetDescriptionText());
            Agent focusedAgent;
            if ((focusedAgent = (Agent)_focusedObject) != null)
            {
                if (focusedAgent.IsActive())
                {
                    Debug.Print("Interacted with agent!");
                }
            }
        }
    }

    public void OnObjectFocused(IFocusable focusedObject)
    {
        _focusedObject = focusedObject;
    }

    public void OnObjectFocusLost()
    {
        _focusedObject = null;
    }

    public override void OnAgentSpawned()
    {
        OnAgentSpawnedWithoutDuelEvent?.Invoke();
    }

}
