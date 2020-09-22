using UnityEngine;

public class TimeStepMonoBehaviour : MonoBehaviour
{

    public float ElapsedTime { get; private set; }
    public float Alpha { get; private set; }
    public float DeltaTime { get; private set; }
    public float FixedDeltaTime { get; private set; }

    private void Awake()
    {
        TimeStep.Instance.OnFrame.AddListener(_OnFrame);
        TimeStep.Instance.OnTick.AddListener(_OnTick);

        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    private void OnDestroy()
    {
        if (TimeStep.Instance)
        {
            TimeStep.Instance.OnFrame.RemoveListener(_OnFrame);
            TimeStep.Instance.OnTick.RemoveListener(_OnTick);
        }

        OnDestroyed();
    }

    private void _OnFrame(float elapsedTime, float deltaTime, float alpha) 
    {
        ElapsedTime = elapsedTime;
        DeltaTime = deltaTime;
        Alpha = alpha;

        OnFrame();
    }

    private void _OnTick(float elapsedTime, float fixedDeltaTime) 
    {
        ElapsedTime = elapsedTime;
        FixedDeltaTime = fixedDeltaTime;

        OnTick();
    }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnDestroyed() { }
    protected virtual void OnFrame() { }
    protected virtual void OnTick() { }

}
