using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class PropSpawnerTagButton : MonoBehaviour
{

    public UnityEvent OnEnabled = new UnityEvent();
    public UnityEvent OnDisabled = new UnityEvent();

    [SerializeField]
    private Toggle _toggle;
    [SerializeField]
    private TMP_Text _tagText;

    private bool _enabled;

    private void Awake()
    {
        _toggle = GetComponentInChildren<Toggle>();
    }

    private void OnDestroy()
    {
        OnEnabled.RemoveAllListeners();
        OnDisabled.RemoveAllListeners();
    }

    public void Initialize(string tag)
    {
        _tagText.text = tag;
        _toggle.isOn = false;
        _enabled = false;
        _toggle.onValueChanged.AddListener((value) =>
        {
            if (value && !_enabled)
            {
                OnEnabled.Invoke();
            }
            else if(!value && _enabled)
            {
                OnDisabled.Invoke();
            }
            _enabled = value;
        });
    }
}
