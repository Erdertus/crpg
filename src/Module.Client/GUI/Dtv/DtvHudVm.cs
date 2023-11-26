﻿using Crpg.Module.GUI.Hud;
using Crpg.Module.Modes.Dtv;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.GUI.Dtv;

internal class DtvHudVm : ViewModel
{
    private readonly CrpgDtvClient _client;
    private BannerHudVm _allyBannerVm;
    private BannerHudVm _enemyBannerVm;
    private TimerHudVm _timerVm;
    private string _waveLabel = string.Empty;
    private string _roundLabel = string.Empty;
    private int _defenderMemberCount;
    private int _attackerMemberCount;
    private int _currentWave;
    private int _currentRound;
    private bool _isGameStarted;

    public DtvHudVm(Mission mission)
    {
        _allyBannerVm = new BannerHudVm(mission, allyBanner: true);
        _enemyBannerVm = new BannerHudVm(mission, allyBanner: false);
        _timerVm = new TimerHudVm(mission);
        _client = mission.GetMissionBehavior<CrpgDtvClient>();
        RefreshValues();
    }

    public override void RefreshValues()
    {
        base.RefreshValues();
        WaveLabel = new TextObject("{=8KlA48nt}Wave").ToString();
        RoundLabel = new TextObject("{=1QzB15at}Round").ToString();
    }

    [DataSourceProperty]
    public BannerHudVm AllyBanner
    {
        get => _allyBannerVm;
        set
        {
            if (value != _allyBannerVm)
            {
                _allyBannerVm = value;
                OnPropertyChangedWithValue(value);
            }
        }
    }

    [DataSourceProperty]
    public BannerHudVm EnemyBanner
    {
        get => _enemyBannerVm;
        set
        {
            if (value != _enemyBannerVm)
            {
                _enemyBannerVm = value;
                OnPropertyChangedWithValue(value);
            }
        }
    }

    [DataSourceProperty]
    public int AttackerMemberCount
    {
        get => _attackerMemberCount;
        set
        {
            if (value != _attackerMemberCount)
            {
                _attackerMemberCount = value;
                OnPropertyChangedWithValue(value);
            }
        }
    }

    [DataSourceProperty]
    public int DefenderMemberCount
    {
        get => _defenderMemberCount;
        set
        {
            if (value != _defenderMemberCount)
            {
                _defenderMemberCount = value;
                OnPropertyChangedWithValue(value);
            }
        }
    }

    [DataSourceProperty]
    public bool IsGameStarted
    {
        get => _isGameStarted;
        set
        {
            if (value == _isGameStarted)
            {
                return;
            }

            _isGameStarted = value;
            OnPropertyChangedWithValue(value);
        }
    }

    [DataSourceProperty]
    public string RoundLabel
    {
        get => _roundLabel;
        set
        {
            if (value != _roundLabel)
            {
                _roundLabel = value;
                OnPropertyChangedWithValue(value, "RoundLabel");
            }
        }
    }

    [DataSourceProperty]
    public string WaveLabel
    {
        get => _waveLabel;
        set
        {
            if (value != _waveLabel)
            {
                _waveLabel = value;
                OnPropertyChangedWithValue(value, "WaveLabel");
            }
        }
    }

    [DataSourceProperty]
    public int CurrentRound
    {
        get => _currentRound;
        set
        {
            if (value != _currentRound)
            {
                _currentRound = value;
                OnPropertyChangedWithValue(value);
            }
        }
    }

    [DataSourceProperty]
    public int CurrentWave
    {
        get => _currentWave;
        set
        {
            if (value != _currentWave)
            {
                _currentWave = value;
                OnPropertyChangedWithValue(value);
            }
        }
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
        IsGameStarted = !_client.IsInWarmup;
        if (Mission.Current.Agents.Count > 0)
        {
            AttackerMemberCount = Mission.Current.AttackerTeam.ActiveAgents.Count;
            DefenderMemberCount = Mission.Current.DefenderTeam.ActiveAgents.Count;
        }
    }

    public void UpdateProgress()
    {
        CurrentWave = _client.CurrentWave;
        CurrentRound = _client.CurrentRound;
    }
}
