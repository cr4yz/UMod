using UnityEngine;
using UnityEngine.Events;

public class ToolGunTool : MonoBehaviour
{

    public UnityEvent OnSelected = new UnityEvent();
    public UnityEvent OnDeselected = new UnityEvent();

    [SerializeField]
    private string _toolName;

    public string ToolName => _toolName;

    private void OnEnable()
    {
        OnSelected?.Invoke();
    }

    private void OnDisable()
    {
        OnDeselected?.Invoke();
    }

}
