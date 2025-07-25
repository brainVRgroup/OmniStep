using System;
using System.Threading.Tasks;
using UnityEngine;

public static class AsyncLerper
{
    public static async Task LerpAsync(float duration, float startValue, float endValue, Action<float> onUpdate)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calculate the interpolated value
            float t = elapsedTime / duration;
            float currentValue = Mathf.Lerp(startValue, endValue, t);

            // Invoke the lambda to update the value
            onUpdate(currentValue);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            await Task.Yield();
        }

        // Ensure the final value is set to the end value
        onUpdate(endValue);
    }
}
