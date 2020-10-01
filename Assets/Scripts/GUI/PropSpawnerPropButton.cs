using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PropSpawnerPropButton : MonoBehaviour
{

    public UnityEvent OnClicked = new UnityEvent();

    [SerializeField]
    private TMP_Text _nameText;
    [SerializeField]
    private Button _button;
    [SerializeField]
    private RawImage _previewImage;
    [SerializeField]
    private Camera _propCamera;

    public Prop Prop { get; private set; }
    public List<string> Tags { get; private set; }

    private void Awake()
    {
        _propCamera.gameObject.SetActive(false);
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

        if(prop.Object is GameObject gameObj
            && _previewImage)
        {
            _previewImage.color = Color.white;
            _propCamera.gameObject.SetActive(true);
            var previewObject = GameObject.Instantiate<GameObject>(gameObj);
            foreach(var rb in previewObject.GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = true;
            }
            RuntimePreviewGenerator.PreviewRenderCamera = _propCamera;
            RuntimePreviewGenerator.OrthographicMode = true;
            _previewImage.texture = RuntimePreviewGenerator.GenerateModelPreview(previewObject.transform, 1024, 1024);
            GameObject.Destroy(previewObject);
            _propCamera.gameObject.SetActive(false);
        }

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
