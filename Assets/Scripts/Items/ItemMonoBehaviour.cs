using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemMonoBehaviour : MonoBehaviour
{
    [Header("Item Properties")]
    [SerializeField]
    private string _itemName;
    [SerializeField]
    [Range(1, 6)]
    private int _slotNumber = 1;
    [SerializeField]
    protected ViewModel ViewModel;
    [SerializeField]
    private GameObject _worldModelPrefab;

    private AnimationEventListener _eventListener;
    protected Camera PlayerCamera;

    private KeyCode[] _itemInputs = new KeyCode[]
    {
        KeyCode.Mouse0,
        KeyCode.Mouse1
    };
    private List<KeyCode> _keysDown = new List<KeyCode>();

    public int SlotNumber => _slotNumber;

    private void Awake()
    {
        PlayerCamera = Camera.allCameras.FirstOrDefault(x => x.CompareTag("Player"));
        _eventListener = GetComponentInChildren<AnimationEventListener>();
        if (_eventListener)
        {
            _eventListener.OnEvent.AddListener(OnAnimationEvent);
        }
    }

    private void OnDisable()
    {
        ReleaseAllInput();
    }

    public IEnumerator Show()
    {
        bool complete = false;
        LeanTween.moveLocalY(gameObject, 0f, .25f).setOnComplete(() =>
        {
            gameObject.SetActive(true);
            complete = true;
        }).setEase(LeanTweenType.easeOutBack);
        while (!complete)
        {
            yield return null;
        }
    }

    public IEnumerator Hide()
    {
        bool complete = false;
        LeanTween.moveLocalY(gameObject, -2f, .25f).setOnComplete(() =>
        {
            gameObject.SetActive(false);
            complete = true;
        }).setEase(LeanTweenType.easeOutBack);
        while (!complete)
        {
            yield return null;
        }
    }

    private void ReleaseAllInput()
    {
        foreach (var key in _keysDown)
        {
            OnKeyUp(key);
        }
        _keysDown.Clear();
    }

    protected virtual void OnAnimationEvent(string eventName)
    {

    }

    protected virtual void Update()
    {
        if (GUIManager.Instance.GUIHasCursor())
        {
            ReleaseAllInput();
            return;
        }

        if(Input.mouseScrollDelta.y != 0)
        {
            OnScrollDelta(Input.mouseScrollDelta.y);
        }

        foreach(var key in _itemInputs)
        {
            if (Input.GetKeyDown(key))
            {
                _keysDown.Add(key);
                OnKeyDown(key);
            }
            else if (Input.GetKey(key))
            {
                OnKeyHold(key);
            }
            if (Input.GetKeyUp(key))
            {
                _keysDown.Remove(key);
                OnKeyUp(key);
            }
        }
    }

    protected virtual void OnKeyDown(KeyCode button) { }
    protected virtual void OnKeyHold(KeyCode button) { }
    protected virtual void OnKeyUp(KeyCode button) { }
    protected virtual void OnScrollDelta(float delta) { }

}
