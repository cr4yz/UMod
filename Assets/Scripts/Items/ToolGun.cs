using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ToolGun : ItemMonoBehaviour
{

    public UnityEvent<ToolGunTool> OnToolChanged = new UnityEvent<ToolGunTool>();

    private ToolGunTool _activeTool;
    private Camera _playerCamera;

    public ToolGunTool[] Tools { get; private set; }

    private void Awake()
    {
        Tools = GetComponentsInChildren<ToolGunTool>();
        foreach (var tool in Tools)
        {
            tool.gameObject.SetActive(false);
        }
        SelectNextTool();
        _playerCamera = Camera.allCameras.First(x => x.CompareTag("Player"));
    }

    protected override void OnKeyDown(KeyCode button)
    {
        if (!_activeTool)
        {
            return;
        }
        ViewModel.Kick();
        _activeTool.HandleKeyDown(this, button, _playerCamera.ViewportPointToRay(Vector3.one * .5f));
    }

    protected override void OnKeyUp(KeyCode button)
    {
        if (!_activeTool)
        {
            return;
        }
        _activeTool.HandleKeyUp(this, button, _playerCamera.ViewportPointToRay(Vector3.one * .5f));
    }

    protected override void OnScrollDelta(float delta)
    {
        SelectNextTool(delta > 0);
    }

    private void SelectNextTool(bool reverse = false)
    {
        if(Tools.Length == 0)
        {
            return;
        }

        ToolGunTool nextTool = null;
        if (_activeTool)
        {
            var list = reverse ? Tools.Reverse() : Tools;
            nextTool = list
                .SkipWhile(x => x != _activeTool)
                .Skip(1)
                .FirstOrDefault();
            _activeTool.gameObject.SetActive(false);
        }

        if (!nextTool)
        {
            nextTool = Tools[reverse ? Tools.Length - 1 : 0];
        }

        nextTool.gameObject.SetActive(true);
        _activeTool = nextTool;
        OnToolChanged?.Invoke(_activeTool);

        ViewModel.Kick(.5f);
    }

}
