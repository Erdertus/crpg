using Crpg.Module.Common;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Crpg.Module.Modes.TrainingGround;

internal class CrpgTrainingGroundSpawningBehavior : CrpgSpawningBehaviorBase
{
    public CrpgTrainingGroundSpawningBehavior(CrpgConstants constants)
        : base(constants)
    {
    }

    public override void OnTick(float dt)
    {
        SpawnAgents();
        SpawnBotAgents();
    }

    protected override bool IsRoundInProgress()
    {
        return Mission.Current.CurrentState == Mission.State.Continuing;
    }
}
