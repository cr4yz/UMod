using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolGunHud : GUIMonoBehaviour
{
    [Header("ToolGun Options")]
    [SerializeField]
    private ToolGunHudTool _toolTemplate;
    [SerializeField]
    private ToolGun _toolGun;
    [SerializeField]
    private float _visibleDuration = 3f;

    private List<GameObject> _clones = new List<GameObject>();
    private float _visibleTimer = .1f;

    private void Start()
    {
        _toolTemplate.gameObject.SetActive(false);

        if (!_toolGun)
        {
            _toolGun = GameObject.FindObjectOfType<ToolGun>();
        }

        _toolGun.OnToolChanged.AddListener((newTool) =>
        {
            if(_visibleTimer <= 0)
            {
                LeanTween.scaleX(Root, 1.25f, .15f)
                    .setOnComplete(() =>
                    {
                        LeanTween.scaleX(Root, 1f, .05f);
                    });
            }
            _visibleTimer = _visibleDuration;
        });

        Build();
    }

    protected override void Update()
    {
        base.Update();

        if(_visibleTimer > 0)
        {
            _visibleTimer -= Time.deltaTime;
            if(_visibleTimer <= 0)
            {
                LeanTween.scaleX(Root, 1.25f, .05f)
                    .setOnComplete(() =>
                    {
                        LeanTween.scaleX(Root, 0f, .15f);
                    });
            }
        }
    }

    private void Build()
    {
        foreach(var clone in _clones)
        {
            GameObject.Destroy(clone);
        }
        _clones.Clear();

        foreach (var tool in _toolGun.Tools)
        {
            var clone = GameObject.Instantiate(_toolTemplate, _toolTemplate.transform.parent);
            clone.gameObject.SetActive(true);
            clone.Initialize(tool);
            _clones.Add(clone.gameObject);
        }
    }

}
