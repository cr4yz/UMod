using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeStep : SingletonComponent<TimeStep>
{

    // Elapsed Time, Delta Time
    public UnityEvent<float, float> OnTick = new UnityEvent<float, float>();
    // Elapsed Time, Delta Time, Alpha
    public UnityEvent<float, float, float> OnFrame = new UnityEvent<float, float, float>();
    public int TickRate = 100;

    private float _elapsedTime;
    private float _accumulator;
    private float _alpha;

    void Update()
    {
        var fixedDeltaTime = 1f / TickRate;
        var newTime = Time.realtimeSinceStartup;
        var frameTime = Mathf.Min(newTime - _elapsedTime, fixedDeltaTime);
        
        _elapsedTime = newTime;
        _accumulator += frameTime;

        while (_accumulator >= fixedDeltaTime)
        {
            _accumulator -= fixedDeltaTime;
            OnTick?.Invoke(_elapsedTime, fixedDeltaTime);
        }
        _alpha = _accumulator / fixedDeltaTime;

        OnFrame?.Invoke(_elapsedTime, Time.deltaTime, _alpha);
    }

}
