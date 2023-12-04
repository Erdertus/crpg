using Crpg.Module.Common;
using Crpg.Module.Notifications;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;

#if CRPG_SERVER
using Crpg.Module.Api;
using Crpg.Module.Common.ChatCommands;
using Crpg.Module.Rewards;
#else
using Crpg.Module.GUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
#endif

namespace Crpg.Module.Modes.TrainingGround;

[ViewCreatorModule] // Exposes methods with ViewMethod attribute.
internal class CrpgTrainingGroundGameMode : MissionBasedMultiplayerGameMode
{
    public const string GameName = "cRPGTrainingGround";

    private static CrpgConstants _constants = default!; // Static so it's accessible from the views.

    public CrpgTrainingGroundGameMode(CrpgConstants constants)
        : base(GameName)
    {
        _constants = constants;
    }

#if CRPG_CLIENT
    // Used by MissionState.OpenNew that finds all methods having a ViewMethod attribute contained in class
    // having a ViewCreatorModule attribute.
    [ViewMethod(GameName)]
    public static MissionView[] OpenCrpgTrainingGround(Mission mission)
    {
        CrpgExperienceTable experienceTable = new(_constants);
        MissionMultiplayerGameModeBaseClient gameModeClient = mission.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
        MissionView crpgEscapeMenu = ViewCreatorManager.CreateMissionView<CrpgMissionMultiplayerEscapeMenu>(isNetwork: false, null, "Training Ground", gameModeClient);

        return new[]
        {
            ViewCreator.CreateMissionServerStatusUIHandler(),
            ViewCreator.CreateMissionMultiplayerPreloadView(mission),
            ViewCreator.CreateMissionKillNotificationUIHandler(),
            ViewCreator.CreateMissionAgentStatusUIHandler(mission),
            ViewCreator.CreateMissionMainAgentEquipmentController(mission),
            ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission),
            crpgEscapeMenu,
            ViewCreator.CreateMultiplayerEndOfBattleUIHandler(),
            ViewCreator.CreateMissionScoreBoardUIHandler(mission, true),
            ViewCreator.CreateLobbyEquipmentUIHandler(),
            ViewCreator.CreatePollProgressUIHandler(),
            ViewCreator.CreateOptionsUIHandler(),
            ViewCreator.CreateMissionMainAgentEquipDropView(mission),
            ViewCreator.CreateMissionBoundaryCrossingView(),
            new MissionBoundaryWallView(),
            new MissionItemContourControllerView(),
            new MissionAgentContourControllerView(),
            ViewCreator.CreateMissionFlagMarkerUIHandler(), // Draw flags but also player names when pressing ALT.
            new CrpgAgentHud(experienceTable),
            // Draw flags but also player names when pressing ALT. (Native: CreateMissionFlagMarkerUIHandler)
            ViewCreatorManager.CreateMissionView<CrpgMarkerUiHandler>(isNetwork: false, null, gameModeClient),
        };
    }
#endif

    public override void StartMultiplayerGame(string scene)
    {
        var lobbyComponent = MissionLobbyComponent.CreateBehavior();

#if CRPG_SERVER
        ICrpgClient crpgClient = CrpgClient.Create();
        ChatBox chatBox = Game.Current.GetGameHandler<ChatBox>();
        CrpgRewardServer rewardServer = new(crpgClient, _constants, null, enableTeamHitCompensations: false, enableRating: false);
        CrpgTrainingGroundServer server = new(rewardServer);
#endif
        CrpgTrainingGroundClient client = new();
        MissionState.OpenNew(
            Name,
            new MissionInitializerRecord(scene),
            _ =>
                new MissionBehavior[]
                {
                    lobbyComponent,
#if CRPG_CLIENT
                    new CrpgUserManagerClient(), // Needs to be loaded before the Client mission part.
#endif
                    client,
                    new MultiplayerTimerComponent(), // round timer
                    new CrpgNotificationComponent(), // Inherits the MultiplayerGameNotificationsComponent component.
                    new MultiplayerMissionAgentVisualSpawnComponent(), // expose method to spawn an agent
                    new ConsoleMatchStartEndHandler(),
                    new MissionLobbyEquipmentNetworkComponent(), // logic to change troop or perks
                    new MultiplayerTeamSelectComponent(),
                    new MissionHardBorderPlacer(),
                    new MissionBoundaryPlacer(), // set walkable boundaries
                    new MissionBoundaryCrossingHandler(), // kills agent out of mission boundaries
                    new MultiplayerPollComponent(), // poll logic to kick player, ban player, change game
                    new MissionOptionsComponent(),
                    new CrpgScoreboardComponent(new CrpgDuelScoreboardData()), // score board
                    new MultiplayerPreloadHelper(),
#if CRPG_SERVER
                    server,
                    // SpawnFrameBehaviour: where to spawn, SpawningBehaviour: when to spawn
                    new SpawnComponent(new DuelSpawnFrameBehavior(), new CrpgTrainingGroundSpawningBehavior(_constants, server)),
                    new MultiplayerAdminComponent(), // admin UI to kick player or restart game
                    new MissionAgentPanicHandler(),
                    new AgentHumanAILogic(), // bot intelligence
                    new EquipmentControllerLeaveLogic(),
                    new CrpgUserManagerServer(crpgClient, _constants),
                    new ChatCommandsComponent(chatBox, crpgClient),
                    new CrpgActivityLogsBehavior(null, chatBox, crpgClient),
                    new ServerMetricsBehavior(),
                    new NotAllPlayersReadyComponent(),
                    new RemoveIpFromFirewallBehavior(),
                    new PopulationBasedEntityVisibilityBehavior(lobbyComponent),
#else
                    new MultiplayerAchievementComponent(),
                    new MissionMatchHistoryComponent(),
                    new MissionRecentPlayersComponent(),
#endif
                });
    }
}
