using UnityEngine;
using UnityEngine.Events;

public class TimeStep : SingletonComponent<TimeStep>
{
    // Elapsed Time, Delta Time
    public UnityEvent<float, float> OnTick = new UnityEvent<float, float>();
    // Elapsed Time, Delta Time, Alpha
    public UnityEvent<float, float, float> OnFrame = new UnityEvent<float, float, float>();
    public int TickRate = 100;

    private float _accumulator;

    public float ElapsedTime { get; private set; }
    public float DeltaTime { get; private set; }
    public float FixedDeltaTime { get; private set; }
    public float Alpha { get; private set; }

    void Update()
    {
        DeltaTime = Time.deltaTime;
        FixedDeltaTime = 1f / TickRate;

        var newTime = Time.realtimeSinceStartup;
        var frameTime = Mathf.Min(newTime - ElapsedTime, FixedDeltaTime);

        ElapsedTime = newTime;
        _accumulator += frameTime;

        while (_accumulator >= FixedDeltaTime)
        {
            _accumulator -= FixedDeltaTime;
            OnTick?.Invoke(ElapsedTime, FixedDeltaTime);
        }
        Alpha = _accumulator / FixedDeltaTime;

        OnFrame?.Invoke(ElapsedTime, DeltaTime, Alpha);
    }

}
