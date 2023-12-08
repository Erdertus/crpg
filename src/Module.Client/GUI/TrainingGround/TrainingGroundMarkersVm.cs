using Crpg.Module.Modes.TrainingGround;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace Crpg.Module.GUI.TrainingGround;
public class TrainingGroundMarkersVm : ViewModel
{
    private class PeerMarkerDistanceComparer : IComparer<TrainingGroundPeerMarkerVm>
    {
        public int Compare(TrainingGroundPeerMarkerVm x, TrainingGroundPeerMarkerVm y)
        {
            return y.Distance.CompareTo(x.Distance);
        }
    }

    private const float FocusScreenDistanceThreshold = 350f;
    private bool _hasEnteredLobby;
    private Camera _missionCamera;
    private TrainingGroundPeerMarkerVm? _previousFocusTarget;
    private TrainingGroundPeerMarkerVm? _currentFocusTarget;
    private PeerMarkerDistanceComparer _distanceComparer;
    private readonly Dictionary<MissionPeer, TrainingGroundPeerMarkerVm> _targetPeersToMarkersDictionary;
    private readonly CrpgTrainingGroundClient _client;
    private Vec2 _screenCenter;
    private Dictionary<MissionPeer, bool> _targetPeersInDuelDictionary;
    private int _playerPreferredArenaType;
    private bool _isPlayerFocused;
    private bool _isEnabled;
    private MBBindingList<TrainingGroundPeerMarkerVm> _targets;

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
                UpdateTargetsEnabled(value);
            }
        }
    }

    [DataSourceProperty]
    public MBBindingList<TrainingGroundPeerMarkerVm> Targets
    {
        get
        {
            return _targets;
        }
        set
        {
            if (value != _targets)
            {
                _targets = value;
                OnPropertyChangedWithValue(value, "Targets");
            }
        }
    }

    public TrainingGroundMarkersVm(Camera missionCamera)
    {
        _missionCamera = missionCamera;
        _client = Mission.Current.GetMissionBehavior<CrpgTrainingGroundClient>();
        _targets = new MBBindingList<TrainingGroundPeerMarkerVm>();
        _targetPeersToMarkersDictionary = new Dictionary<MissionPeer, TrainingGroundPeerMarkerVm>();
        _targetPeersInDuelDictionary = new Dictionary<MissionPeer, bool>();
        _distanceComparer = new PeerMarkerDistanceComparer();
        UpdateScreenCenter();
        RefreshValues();
    }

    public override void RefreshValues()
    {
        base.RefreshValues();
        Targets.ApplyActionOnAllItems(delegate (TrainingGroundPeerMarkerVm t)
        {
            t.RefreshValues();
        });
    }

    public void UpdateScreenCenter()
    {
        _screenCenter = new Vec2(Screen.RealScreenResolutionWidth / 2f, Screen.RealScreenResolutionHeight / 2f);
    }

    public void Tick(float dt)
    {
        if (_hasEnteredLobby && GameNetwork.MyPeer != null)
        {
            OnRefreshPeerMarkers();
            UpdateTargets(dt);
        }
    }

    public void RegisterEvents()
    {
    }

    public void UnregisterEvents()
    {
    }

    private void UpdateTargets(float dt)
    {
        if (_currentFocusTarget != null)
        {
            _previousFocusTarget = _currentFocusTarget;
            _currentFocusTarget = null;
            if (_isPlayerFocused)
            {
                _previousFocusTarget.IsFocused = false;
            }
        }

        if (_client.MyRepresentative?.MissionPeer.ControlledAgent == null)
        {
            return;
        }

        float num = float.MaxValue;
        foreach (TrainingGroundPeerMarkerVm target in Targets)
        {
            target.OnTick(dt);
            if (target.IsEnabled)
            {
                if (!target.HasSentDuelRequest && !target.HasDuelRequestForPlayer && target.TargetPeer.ControlledAgent != null)
                {
                    target.PreferredArenaType = _playerPreferredArenaType;
                }

                target.UpdateScreenPosition(_missionCamera);
                float num2 = target.ScreenPosition.Distance(_screenCenter);
                if (!_isPlayerFocused && target.WSign >= 0 && num2 < 350f && num2 < num)
                {
                    num = num2;
                    _currentFocusTarget = target;
                }
            }
        }

        Targets.Sort(_distanceComparer);
        if (_client.MyRepresentative == null)
        {
            return;
        }

        if (_currentFocusTarget != null && _currentFocusTarget.TargetPeer.ControlledAgent != null)
        {
            _client.MyRepresentative.OnObjectFocused(_currentFocusTarget.TargetPeer.ControlledAgent);
            if (_previousFocusTarget != null && _currentFocusTarget.TargetPeer != _previousFocusTarget.TargetPeer)
            {
                _previousFocusTarget.IsFocused = false;
            }

            _currentFocusTarget.IsFocused = true;

            return;
        }

        if (_previousFocusTarget != null)
        {
            _previousFocusTarget.IsFocused = false;
        }

        if (_currentFocusTarget == null)
        {
            _client.MyRepresentative.OnObjectFocusLost();
        }
    }

    private void OnRefreshPeerMarkers()
    {
        List<TrainingGroundPeerMarkerVm> list = Targets.ToList();
        foreach (MissionPeer item in VirtualPlayer.Peers<MissionPeer>())
        {
            if (item?.Team == null || !item.IsControlledAgentActive || item.IsMine)
            {
                continue;
            }

            if (!_targetPeersToMarkersDictionary.ContainsKey(item))
            {
                TrainingGroundPeerMarkerVm missionDuelPeerMarkerVM = new(item);
                Targets.Add(missionDuelPeerMarkerVM);
                _targetPeersToMarkersDictionary.Add(item, missionDuelPeerMarkerVM);
                OnPeerEquipmentRefreshed(item);
                if (_targetPeersInDuelDictionary.ContainsKey(item))
                {
                    missionDuelPeerMarkerVM.UpdateCurentDuelStatus(_targetPeersInDuelDictionary[item]);
                }
            }
            else
            {
                list.Remove(_targetPeersToMarkersDictionary[item]);
            }

            if (!_targetPeersInDuelDictionary.ContainsKey(item))
            {
                _targetPeersInDuelDictionary.Add(item, value: false);
            }
        }

        foreach (TrainingGroundPeerMarkerVm item2 in list)
        {
            Targets.Remove(item2);
            _targetPeersToMarkersDictionary.Remove(item2.TargetPeer);
        }
    }

    private void UpdateTargetsEnabled(bool isEnabled)
    {
        foreach (TrainingGroundPeerMarkerVm target in Targets)
        {
            target.IsEnabled = !target.IsInDuel && isEnabled;
        }
    }

    private void OnDuelRequestSent(MissionPeer targetPeer)
    {
        foreach (TrainingGroundPeerMarkerVm target in Targets)
        {
            if (target.TargetPeer == targetPeer)
            {
                target.HasSentDuelRequest = true;
            }
        }
    }

    private void OnDuelRequested(MissionPeer targetPeer, TroopType troopType)
    {
        TrainingGroundPeerMarkerVm missionDuelPeerMarkerVM = Targets.FirstOrDefault((TrainingGroundPeerMarkerVm t) => t.TargetPeer == targetPeer);
        if (missionDuelPeerMarkerVM != null)
        {
            missionDuelPeerMarkerVM.HasDuelRequestForPlayer = true;
            missionDuelPeerMarkerVM.PreferredArenaType = (int)troopType;
        }
    }

    public void OnAgentSpawnedWithoutDuel()
    {
        _hasEnteredLobby = true;
        IsEnabled = true;
    }

    public void OnAgentBuiltForTheFirstTime()
    {
        _playerPreferredArenaType = (int)MultiplayerDuelVM.GetAgentDefaultPreferredArenaType(Agent.Main);
    }

    public void OnDuelStarted(MissionPeer firstPeer, MissionPeer secondPeer)
    {
        if (_client?.MyRepresentative?.MissionPeer == firstPeer || _client?.MyRepresentative?.MissionPeer == secondPeer)
        {
            IsEnabled = false;
        }

        foreach (TrainingGroundPeerMarkerVm target in Targets)
        {
            if (target.TargetPeer == firstPeer || target.TargetPeer == secondPeer)
            {
                target.OnDuelStarted();
            }
        }

        _targetPeersInDuelDictionary[firstPeer] = true;
        _targetPeersInDuelDictionary[secondPeer] = true;
    }

    public void SetMarkerOfPeerEnabled(MissionPeer peer, bool isEnabled)
    {
        if (peer != null)
        {
            if (_targetPeersToMarkersDictionary.ContainsKey(peer))
            {
                _targetPeersToMarkersDictionary[peer].UpdateCurentDuelStatus(!isEnabled);
            }

            if (_targetPeersInDuelDictionary.ContainsKey(peer))
            {
                _targetPeersInDuelDictionary[peer] = !isEnabled;
            }
        }
    }

    public void OnFocusGained()
    {
        _isPlayerFocused = true;
    }

    public void OnFocusLost()
    {
        _isPlayerFocused = false;
    }

    public void OnPeerEquipmentRefreshed(MissionPeer peer)
    {
        if (_targetPeersToMarkersDictionary.ContainsKey(peer))
        {
        }
    }
}
