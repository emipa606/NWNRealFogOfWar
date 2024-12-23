using UnityEngine;
using Verse;

namespace RimWorldRealFoW;

public class MoteSoundWave : Mote
{
    private float targetSize;

    private float velocity;

    protected override bool EndOfLife => AgeSecs >= targetSize / velocity;


    public override float Alpha
    {
        get
        {
            Mathf.Clamp01(AgeSecs * 10f);
            var num = 1f;
            var num2 = Mathf.Clamp01(1f - (AgeSecs / (targetSize / velocity)));
            return num * num2 * CalculatedIntensity();
        }
    }

    public void Initialize(Vector3 position, float size, float velocity)
    {
        exactPosition = position;
        targetSize = size;
        this.velocity = velocity;
        Scale = 0f;
    }

    protected override void TimeInterval(float deltaTime)
    {
        base.TimeInterval(deltaTime);
        if (Destroyed)
        {
            return;
        }

        var scale = AgeSecs * velocity;
        Scale = scale;
        //this.exactPosition += base.Map.waterInfo.GetWaterMovement(this.exactPosition) * deltaTime;
    }

    public float CalculatedIntensity()
    {
        return Mathf.Sqrt(targetSize) / 10f;
    }

    public float CalculatedShockwaveSpan()
    {
        return Mathf.Min(Mathf.Sqrt(targetSize) * 0.8f, ExactScale.x) / ExactScale.x;
    }
}