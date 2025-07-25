using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class AnimatorExtensions
{
    public static async Task SetTriggerAsync(this Animator animator, string triggerName, int layerIndex = 0, CancellationToken cancellationToken = default)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        int currentStateHash = stateInfo.shortNameHash;

        // Set the animation trigger
        animator.SetTrigger(triggerName);

        // Wait until the new state is entered (i.e., the animation starts)
        while (animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash == currentStateHash || animator.IsInTransition(layerIndex))
        {
            cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation
            await Task.Yield(); // Non-blocking wait for the next frame
        }

        // Once the animation starts, wait until it finishes
        while (animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < 1.0f || animator.IsInTransition(layerIndex))
        {
            cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation
            await Task.Yield(); // Non-blocking wait for the next frame
        }
    }


}
