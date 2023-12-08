using Crpg.Module.Modes.TrainingGround;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.TwoDimension;

namespace Crpg.Module.GUI.TrainingGround;

internal class TrainingGroundHudUiHandler : MissionView
{
    private CrpgTrainingGroundClient? _client;
    private TrainingGroundHudVm? _dataSource;
    private GauntletLayer? _gauntletLayer;
    private SpriteCategory? _mpMissionCategory;

    public TrainingGroundHudUiHandler()
    {
        ViewOrderPriority = 15;
    }

    public override void OnMissionScreenInitialize()
    {
        base.OnMissionScreenInitialize();
        _mpMissionCategory = UIResourceManager.SpriteData.SpriteCategories["ui_mpmission"];
        _mpMissionCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
        _dataSource = new TrainingGroundHudVm(MissionScreen.CombatCamera, Mission);
        _gauntletLayer = new GauntletLayer(ViewOrderPriority);
        _gauntletLayer.LoadMovie("TrainingGroundHud", _dataSource);
        MissionScreen.AddLayer(_gauntletLayer);
    }

    public override void AfterStart()
    {
        base.AfterStart();
        _client = Mission.GetMissionBehavior<CrpgTrainingGroundClient>();
    }

    public override void OnMissionScreenFinalize()
    {
        MissionScreen.RemoveLayer(_gauntletLayer);
        _dataSource!.OnFinalize();
        _mpMissionCategory?.Unload();
        base.OnMissionScreenFinalize();
    }

    public override void OnMissionScreenTick(float dt)
    {
        base.OnMissionScreenTick(dt);
        _dataSource!.Tick(dt);
        TrainingGroundMissionRepresentative? myRepresentative = _client?.MyRepresentative;
        if (myRepresentative?.ControlledAgent != null && Input.IsGameKeyReleased(13))
        {
            _client!.MyRepresentative!.OnInteraction();
        }
    }

    public override void OnFocusGained(Agent agent, IFocusable focusableObject, bool isInteractable)
    {
        base.OnFocusGained(agent, focusableObject, isInteractable);
        if (focusableObject is not Agent)
        {
            _dataSource!.Markers.OnFocusGained();
        }
    }

    public override void OnFocusLost(Agent agent, IFocusable focusableObject)
    {
        base.OnFocusLost(agent, focusableObject);
        if (focusableObject is not Agent)
        {
            _dataSource!.Markers.OnFocusLost();
        }
    }
}
