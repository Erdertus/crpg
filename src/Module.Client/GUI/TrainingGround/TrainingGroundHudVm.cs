using Crpg.Module.GUI.Hud;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.GUI.TrainingGround;

internal class TrainingGroundHudVm : ViewModel
{
    private TimerHudVm _timerVm;
    public TrainingGroundHudVm(Mission mission)
    {
        _timerVm = new TimerHudVm(mission);
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
    }
}
