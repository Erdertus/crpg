using System.Diagnostics;
using Crpg.Module.GUI.Hud;
using Crpg.Module.Modes.Dtv;
using Crpg.Module.Modes.TrainingGround;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.HUDExtensions;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.KillFeed;

namespace Crpg.Module.GUI.TrainingGround;
public class TrainingGroundHudVm : ViewModel
{
    public struct DuelArenaProperties
    {
        public GameEntity FlagEntity;

        public int Index;

        public TroopType ArenaTroopType;

        public DuelArenaProperties(GameEntity flagEntity, int index, TroopType arenaTroopType)
        {
            FlagEntity = flagEntity;
            Index = index;
            ArenaTroopType = arenaTroopType;
        }
    }

    private const string ArenaFlagTag = "area_flag";
    private const string AremaTypeFlagTagBase = "flag_";
    private readonly CrpgTrainingGroundClient? _client;
    private readonly MissionMultiplayerGameModeBaseClient _gameMode;
    private bool _isMyRepresentativeAssigned;
    private List<DuelArenaProperties> _duelArenaProperties;
    private TextObject _scoreWithSeparatorText;
    private bool _isAgentBuiltForTheFirstTime = true;
    private bool _hasPlayerChangedArenaPreferrence;
    private string _cachedPlayerClassID = string.Empty;
    private bool _showSpawnPoints;
    private Camera _missionCamera;
    private bool _isEnabled;
    private bool _areOngoingDuelsActive;
    private bool _isPlayerInDuel;
    private int _playerBounty;
    private int _playerPreferredArenaType;
    private string _playerScoreText = string.Empty;
    private TrainingGroundMarkersVm _markers;
    private DuelMatchVM _playerDuelMatch;
    private MBBindingList<DuelMatchVM> _ongoingDuels;
    private MBBindingList<MPDuelKillNotificationItemVM> _killNotifications;
    private TimerHudVm _timerVm;

    public TrainingGroundHudVm(Camera missionCamera, Mission mission)
    {
        _missionCamera = missionCamera;
        _client = mission.GetMissionBehavior<CrpgTrainingGroundClient>();
        if (_client != null)
        {
            _client.OnMyRepresentativeAssigned += OnMyRepresentativeAssigned;
        }

        _gameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();
        _playerDuelMatch = new DuelMatchVM();
        _ongoingDuels = new MBBindingList<DuelMatchVM>();
        _duelArenaProperties = new List<DuelArenaProperties>();
        List<GameEntity> list = new List<GameEntity>();
        _timerVm = new TimerHudVm(mission);

        _markers = new TrainingGroundMarkersVm(missionCamera);
        _killNotifications = new MBBindingList<MPDuelKillNotificationItemVM>();
        _scoreWithSeparatorText = new TextObject("{=J5rb5YVV}/ {SCORE}");
        RefreshValues();
    }

    [DataSourceProperty]
    public TimerHudVm Timer
    {
        get => _timerVm;
        set
        {
            if (value != _timerVm)
            {
                _timerVm = value;
                OnPropertyChangedWithValue(value);
            }
        }
    }

    public void Tick(float dt)
    {
        _timerVm.Tick(dt);
        Markers?.Tick(dt);
    }

    [DataSourceProperty]
    public bool IsEnabled
    {
        get
        {
            return _isEnabled;
        }
        set
        {
            if (value != _isEnabled)
            {
                _isEnabled = value;
                OnPropertyChangedWithValue(value, "IsEnabled");
            }
        }
    }

    [DataSourceProperty]
    public bool AreOngoingDuelsActive
    {
        get
        {
            return _areOngoingDuelsActive;
        }
        set
        {
            if (value != _areOngoingDuelsActive)
            {
                _areOngoingDuelsActive = value;
                OnPropertyChangedWithValue(value, "AreOngoingDuelsActive");
            }
        }
    }

    [DataSourceProperty]
    public bool IsPlayerInDuel
    {
        get
        {
            return _isPlayerInDuel;
        }
        set
        {
            if (value != _isPlayerInDuel)
            {
                _isPlayerInDuel = value;
                OnPropertyChangedWithValue(value, "IsPlayerInDuel");
            }
        }
    }

    [DataSourceProperty]
    public int PlayerBounty
    {
        get
        {
            return _playerBounty;
        }
        set
        {
            if (value != _playerBounty)
            {
                _playerBounty = value;
                OnPropertyChangedWithValue(value, "PlayerBounty");
            }
        }
    }

    [DataSourceProperty]
    public int PlayerPrefferedArenaType
    {
        get
        {
            return _playerPreferredArenaType;
        }
        set
        {
            if (value != _playerPreferredArenaType)
            {
                _playerPreferredArenaType = value;
                OnPropertyChangedWithValue(value, "PlayerPrefferedArenaType");
            }
        }
    }

    [DataSourceProperty]
    public string PlayerScoreText
    {
        get
        {
            return _playerScoreText;
        }
        set
        {
            if (value != _playerScoreText)
            {
                _playerScoreText = value;
                OnPropertyChangedWithValue(value, "PlayerScoreText");
            }
        }
    }

    [DataSourceProperty]
    public TrainingGroundMarkersVm Markers
    {
        get
        {
            return _markers;
        }
        set
        {
            if (value != _markers)
            {
                _markers = value;
                OnPropertyChangedWithValue(value, "Markers");
            }
        }
    }

    [DataSourceProperty]
    public DuelMatchVM PlayerDuelMatch
    {
        get
        {
            return _playerDuelMatch;
        }
        set
        {
            if (value != _playerDuelMatch)
            {
                _playerDuelMatch = value;
                OnPropertyChangedWithValue(value, "PlayerDuelMatch");
            }
        }
    }

    [DataSourceProperty]
    public MBBindingList<DuelMatchVM> OngoingDuels
    {
        get
        {
            return _ongoingDuels;
        }
        set
        {
            if (value != _ongoingDuels)
            {
                _ongoingDuels = value;
                OnPropertyChangedWithValue(value, "OngoingDuels");
            }
        }
    }

    [DataSourceProperty]
    public MBBindingList<MPDuelKillNotificationItemVM> KillNotifications
    {
        get
        {
            return _killNotifications;
        }
        set
        {
            if (value != _killNotifications)
            {
                _killNotifications = value;
                OnPropertyChangedWithValue(value, "KillNotifications");
            }
        }
    }

    public override void RefreshValues()
    {
        base.RefreshValues();
        PlayerDuelMatch.RefreshValues();
        Markers.RefreshValues();
    }

    private void OnMyRepresentativeAssigned()
    {
        TrainingGroundMissionRepresentative myRepresentative = _client!.MyRepresentative!;
        myRepresentative.OnAgentSpawnedWithoutDuelEvent += OnAgentSpawnedWithoutDuel;
        Markers.RegisterEvents();
        _isMyRepresentativeAssigned = true;
    }

    [Conditional("DEBUG")]
    private void DebugTick()
    {
        if (TaleWorlds.InputSystem.Input.IsKeyReleased(InputKey.Numpad3))
        {
            _showSpawnPoints = !_showSpawnPoints;
        }

        if (!_showSpawnPoints)
        {
            return;
        }

        string expression = "spawnpoint_area(_\\d+)*";
        foreach (GameEntity item in Mission.Current.Scene.FindEntitiesWithTagExpression(expression))
        {
            Vec3 worldPoint = new Vec3(item.GlobalPosition.x, item.GlobalPosition.y, item.GlobalPosition.z);
            Vec3 vec = _missionCamera.WorldPointToViewPortPoint(ref worldPoint);
            vec.y = 1f - vec.y;
            if (vec.z < 0f)
            {
                vec.x = 1f - vec.x;
                vec.y = 1f - vec.y;
                vec.z = 0f;
                float num = 0f;
                num = ((vec.x > num) ? vec.x : num);
                num = ((vec.y > num) ? vec.y : num);
                num = ((vec.z > num) ? vec.z : num);
                vec /= num;
            }

            if (float.IsPositiveInfinity(vec.x))
            {
                vec.x = 1f;
            }
            else if (float.IsNegativeInfinity(vec.x))
            {
                vec.x = 0f;
            }

            if (float.IsPositiveInfinity(vec.y))
            {
                vec.y = 1f;
            }
            else if (float.IsNegativeInfinity(vec.y))
            {
                vec.y = 0f;
            }

            vec.x = MathF.Clamp(vec.x, 0f, 1f) * Screen.RealScreenResolutionWidth;
            vec.y = MathF.Clamp(vec.y, 0f, 1f) * Screen.RealScreenResolutionHeight;
        }
    }

    public override void OnFinalize()
    {
        base.OnFinalize();
        if (_client != null )
        {
            _client.OnMyRepresentativeAssigned -= OnMyRepresentativeAssigned;
        }

        if (_isMyRepresentativeAssigned)
        {
           Markers.UnregisterEvents();
        }
    }

    private void OnDuelPrepStarted(MissionPeer opponentPeer, int duelStartTime)
    {
        PlayerDuelMatch.OnDuelPrepStarted(opponentPeer, duelStartTime);
        AreOngoingDuelsActive = false;
        Markers.IsEnabled = false;
    }

    private void OnAgentSpawnedWithoutDuel()
    {
        Markers.OnAgentSpawnedWithoutDuel();
        AreOngoingDuelsActive = true;
    }

    private void OnDuelStarted(MissionPeer firstPeer, MissionPeer secondPeer, int flagIndex)
    {
        Markers.OnDuelStarted(firstPeer, secondPeer);
        DuelArenaProperties duelArenaProperties = _duelArenaProperties.First((DuelArenaProperties f) => f.Index == flagIndex);
        if (firstPeer == _client?.MyRepresentative?.MissionPeer || secondPeer == _client?.MyRepresentative?.MissionPeer)
        {
            AreOngoingDuelsActive = false;
            IsPlayerInDuel = true;
            PlayerDuelMatch.OnDuelStarted(firstPeer, secondPeer, (int)duelArenaProperties.ArenaTroopType);
        }
        else
        {
            DuelMatchVM duelMatchVM = new DuelMatchVM();
            duelMatchVM.OnDuelStarted(firstPeer, secondPeer, (int)duelArenaProperties.ArenaTroopType);
            OngoingDuels.Add(duelMatchVM);
        }
    }

    private void OnDuelEnded(MissionPeer winnerPeer)
    {
        if (PlayerDuelMatch.FirstPlayerPeer == winnerPeer || PlayerDuelMatch.SecondPlayerPeer == winnerPeer)
        {
            AreOngoingDuelsActive = true;
            IsPlayerInDuel = false;
            Markers.IsEnabled = true;
            Markers.SetMarkerOfPeerEnabled(PlayerDuelMatch.FirstPlayerPeer, isEnabled: true);
            Markers.SetMarkerOfPeerEnabled(PlayerDuelMatch.SecondPlayerPeer, isEnabled: true);
            PlayerDuelMatch.OnDuelEnded();
        }

        DuelMatchVM duelMatchVM = OngoingDuels.FirstOrDefault((DuelMatchVM d) => d.FirstPlayerPeer == winnerPeer || d.SecondPlayerPeer == winnerPeer);
        if (duelMatchVM != null)
        {
            Markers.SetMarkerOfPeerEnabled(duelMatchVM.FirstPlayerPeer, isEnabled: true);
            Markers.SetMarkerOfPeerEnabled(duelMatchVM.SecondPlayerPeer, isEnabled: true);
            OngoingDuels.Remove(duelMatchVM);
        }
    }

    private void OnDuelRoundEnded(MissionPeer winnerPeer)
    {
        if (PlayerDuelMatch.FirstPlayerPeer == winnerPeer || PlayerDuelMatch.SecondPlayerPeer == winnerPeer)
        {
            PlayerDuelMatch.OnPeerScored(winnerPeer);
            KillNotifications.Add(new MPDuelKillNotificationItemVM(PlayerDuelMatch.FirstPlayerPeer, PlayerDuelMatch.SecondPlayerPeer, PlayerDuelMatch.FirstPlayerScore, PlayerDuelMatch.SecondPlayerScore, (TroopType)PlayerDuelMatch.ArenaType, RemoveKillNotification));
            return;
        }

        DuelMatchVM duelMatchVM = OngoingDuels.FirstOrDefault((DuelMatchVM d) => d.FirstPlayerPeer == winnerPeer || d.SecondPlayerPeer == winnerPeer);
        if (duelMatchVM != null)
        {
            duelMatchVM.OnPeerScored(winnerPeer);
            KillNotifications.Add(new MPDuelKillNotificationItemVM(duelMatchVM.FirstPlayerPeer, duelMatchVM.SecondPlayerPeer, duelMatchVM.FirstPlayerScore, duelMatchVM.SecondPlayerScore, (TroopType)duelMatchVM.ArenaType, RemoveKillNotification));
        }
    }

    private void RemoveKillNotification(MPDuelKillNotificationItemVM item)
    {
        KillNotifications.Remove(item);
    }

    public void OnScreenResolutionChanged()
    {
        Markers.UpdateScreenCenter();
    }

    public void OnMainAgentRemoved()
    {
        if (!PlayerDuelMatch.IsEnabled)
        {
            Markers.IsEnabled = false;
            AreOngoingDuelsActive = false;
        }
    }

    public void OnMainAgentBuild()
    {
        if (!PlayerDuelMatch.IsEnabled)
        {
            Markers.IsEnabled = true;
            AreOngoingDuelsActive = true;
        }

        string stringId = MultiplayerClassDivisions.GetMPHeroClassForPeer(_client?.MyRepresentative?.MissionPeer).StringId;
        if (_isAgentBuiltForTheFirstTime || (stringId != _cachedPlayerClassID && !_hasPlayerChangedArenaPreferrence))
        {
            Markers.OnAgentBuiltForTheFirstTime();
            _isAgentBuiltForTheFirstTime = false;
            _cachedPlayerClassID = stringId;
        }
    }

}
