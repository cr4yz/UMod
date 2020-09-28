using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManagerHud : GUIMonoBehaviour
{

    [SerializeField]
    private float _visibleDuration = 3f;
    [SerializeField]
    private ItemManager _itemManager;
    [SerializeField]
    private ItemManagerHudRow _hudRowTemplate;

    private List<GameObject> _clones = new List<GameObject>();
    private float _visibleTimer = .1f;

    private void Start()
    {
        _hudRowTemplate.gameObject.SetActive(false);
        _itemManager.OnItemEquipped.AddListener((item) =>
        {
            if(_visibleTimer <= 0)
            {
                LeanTween.scaleX(Root, 1.25f, .15f).setOnComplete(() =>
                {
                    LeanTween.scaleX(Root, 1f, .1f);
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
                LeanTween.scaleX(Root, 1.25f, .1f).setOnComplete(() =>
                {
                    LeanTween.scaleX(Root, 0f, .25f);
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

        for (int i = 1; i <= 6; i++)
        {
            var items = _itemManager.Items.Where(x => x.SlotNumber == i);
            if(items.Count() > 0)
            {
                var clone = GameObject.Instantiate(_hudRowTemplate, _hudRowTemplate.transform.parent);
                clone.Initialize(i, items);
                clone.gameObject.SetActive(true);
                _clones.Add(clone.gameObject);
            }
        }
    }

}
