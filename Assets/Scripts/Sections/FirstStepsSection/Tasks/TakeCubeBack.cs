using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TakeCubeBack : SectionTaskBase
{
    [SerializeField] XRGrabInteractable theCube;
    protected override async Task RunTask(CancellationToken cancellationToken)
    {
        var robotInstrucions = RobotController.SetInstructionsAsync("table-room-entry", cancellationToken);
        await WaitForEvent(theCube.firstSelectEntered, cancellationToken);
        await robotInstrucions;
    }
}
