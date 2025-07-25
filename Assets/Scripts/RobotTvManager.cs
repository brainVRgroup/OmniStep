using Elisu.TTSLocalizationKit;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RobotTvManager : TextAndAudioManager<RobotTvManager>
{
    [SerializeField] Animator animator;

    public async Task ToggleAsync()
    {
        await animator.SetTriggerAsync("Toggle");
    }
}
