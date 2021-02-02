using UnityEngine;
using Unity.Collections;

public struct SampledAnimationCurve : System.IDisposable
{
    [ReadOnly] NativeArray<float> AnimationCurveLUT;
    private readonly int maxIndex;
    public SampledAnimationCurve(AnimationCurve ac, int samples)
    {
        maxIndex = samples - 1;
        AnimationCurveLUT = new NativeArray<float>(samples, Allocator.Persistent);
        float multiplier = 1f / samples;
        for (int i = 0; i < samples; i++)
        {
            AnimationCurveLUT[i] = ac.Evaluate(i * multiplier);
            
        }
    }

    public void Dispose()
    {
        AnimationCurveLUT.Dispose();
    }

    public float Evaluate(float time)
    {
        time = time >= 0 ? time : 0;
        time = time <= 1 ? time : 1;
        return AnimationCurveLUT[(int)(Mathf.Clamp01(time) * maxIndex)];
    }
}
