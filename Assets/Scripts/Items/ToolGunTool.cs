using UnityEngine;
using UnityEngine.Events;

public class ToolGunTool : MonoBehaviour
{

    public UnityEvent OnSelected = new UnityEvent();
    public UnityEvent OnDeselected = new UnityEvent();

    [SerializeField]
    private string _toolName;

    public string ToolName => _toolName;

    protected virtual void OnEnable()
    {
        OnSelected?.Invoke();
    }

    protected virtual void OnDisable()
    {
        OnDeselected?.Invoke();
    }

    public virtual void HandleKeyDown(ToolGun toolGun, KeyCode button, Ray eyeRay) { }
    public virtual void HandleKeyUp(ToolGun toolGun, KeyCode button, Ray eyeRay) { }

}
