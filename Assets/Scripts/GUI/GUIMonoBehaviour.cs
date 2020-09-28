using TMPro;
using UnityEngine;

public class GUIMonoBehaviour : MonoBehaviour
{

    [Header("GUI")]
    [SerializeField]
    [Tooltip("The root object should not be this!")]
    private GameObject _root;
    [SerializeField]
    private bool _showCursor;
    [SerializeField]
    private KeyCode _toggleKey;

    private TMP_InputField[] _inputFields;

    public bool IsOpen => _root.activeSelf;
    public GameObject Root => _root;

    protected virtual void Awake()
    {
        _inputFields = GetComponentsInChildren<TMP_InputField>();
        GUIManager.Instance.AddGUI(this);
    }

    protected virtual void OnDestroy()
    {
        if (GUIManager.Instance)
        {
            GUIManager.Instance.RemoveGUI(this);
        }
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(_toggleKey))
        {
            Toggle();
        }
    }

    protected virtual void OnOpen() { }
    protected virtual void OnClose() { }

    public void Close()
    {
        if (!_root.activeSelf)
        {
            return;
        }
        _root.gameObject.SetActive(false);
        OnClose();
    }

    public void Open()
    {
        if (_root.activeSelf)
        {
            return;
        }
        _root.gameObject.SetActive(true);
        OnOpen();
    }

    public void Toggle()
    {
        if (_root.activeSelf)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public bool HasCursor()
    {
        if(_root.activeSelf)
        {
            if (_showCursor)
            {
                return true;
            }
            foreach (var inputField in _inputFields)
            {
                if (inputField.isFocused)
                {
                    return true;
                }
            }
        }
        return false;
    }

}
