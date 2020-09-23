using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PropSpawnerPropButton : MonoBehaviour
{

    public UnityEvent OnClicked = new UnityEvent();

    [SerializeField]
    private TMP_Text _nameText;
    [SerializeField]
    private Button _button;

    public Prop Prop { get; private set; }
    public List<string> Tags { get; private set; }

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnDestroy()
    {
        OnClicked.RemoveAllListeners();
    }

    public void Initialize(Prop prop)
    {
        Prop = prop;
        _nameText.text = prop.Name;
        _button.onClick.AddListener(() =>
        {
            OnClicked.Invoke();
        });
        if (!string.IsNullOrEmpty(prop.Tags))
        {
            Tags = prop.Tags.Split(',').ToList();
        }
        else
        {
            Tags = new List<string>();
        }
    }
}
