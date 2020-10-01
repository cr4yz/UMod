using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class PropSpawner : GUIMonoBehaviour
{

    [SerializeField]
    private PropDatabase _propDatabase;
    [SerializeField]
    private PropSpawnerTagButton _tagButtonTemplate;
    [SerializeField]
    private PropSpawnerPropButton _propButtonTemplate;

    private bool _loaded;
    private List<string> _selectedTags = new List<string>();
    private List<string> _tags = new List<string>();
    private List<PropSpawnerTagButton> _tagButtons = new List<PropSpawnerTagButton>();
    private List<PropSpawnerPropButton> _propButtons = new List<PropSpawnerPropButton>();

    protected override void OnOpen()
    {
        if (!_loaded)
        {
            _tagButtonTemplate.gameObject.SetActive(false);
            _propButtonTemplate.gameObject.SetActive(false);
            LoadDatabase(_propDatabase);
        }
    }

    private void LoadDatabase(PropDatabase database)
    {
        foreach(var btn in _propButtons)
        {
            GameObject.Destroy(btn.gameObject);
        }
        foreach (var btn in _tagButtons)
        {
            GameObject.Destroy(btn.gameObject);
        }
        _propButtons.Clear();
        _tagButtons.Clear();
        _tags.Clear();
        _selectedTags.Clear();
        foreach (var prop in database.Props)
        {
            if (!string.IsNullOrEmpty(prop.Tags))
            {
                prop.Tags = prop.Tags.ToLower();
                var tags = prop.Tags.Split(',');
                foreach (var tag in tags)
                {
                    if (!_tags.Contains(tag))
                    {
                        _tags.Add(tag);
                        AddTag(tag);
                    }
                }
            }
            AddProp(prop);
        }
        _loaded = true;
    }

    private void SpawnProp(Prop prop)
    {
        if(prop.Object is GameObject gameObj)
        {
            var clone = GameObject.Instantiate<GameObject>(gameObj);
            var rb = clone.GetComponentInChildren<Rigidbody>();
            clone.AddComponent<PropRoot>();
            if(rb == null)
            {
                rb = clone.AddComponent<Rigidbody>();
            }
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            var col = clone.GetComponentInChildren<Collider>();
            if(col == null)
            {
                foreach (var mf in clone.GetComponentsInChildren<MeshFilter>())
                {
                    mf.gameObject.AddComponent<MeshCollider>().convex = true;
                }
            }
            Bounds b = new Bounds();
            foreach(var collider in clone.GetComponentsInChildren<Collider>())
            {
                b.Encapsulate(collider.bounds);
                if(collider is MeshCollider mc)
                {
                    mc.convex = true;
                }
            }
            var playerCamera = Camera.allCameras.FirstOrDefault(x => x.CompareTag("Player"));
            clone.transform.position = playerCamera.transform.position + playerCamera.transform.forward * b.size.magnitude;
        }
    }

    private void AddProp(Prop prop)
    {
        var clone = GameObject.Instantiate(_propButtonTemplate, _propButtonTemplate.transform.parent);
        clone.gameObject.SetActive(true);
        clone.Initialize(prop);
        _propButtons.Add(clone);
        clone.OnClicked.AddListener(() =>
        {
            SpawnProp(prop);
        });
    }

    private void AddTag(string tag)
    {
        var clone = GameObject.Instantiate(_tagButtonTemplate, _tagButtonTemplate.transform.parent);
        clone.gameObject.SetActive(true);
        clone.Initialize(tag);
        _tagButtons.Add(clone);
        clone.OnEnabled.AddListener(() =>
        {
            _selectedTags.Add(tag);
            UpdatePropList();
        });
        clone.OnDisabled.AddListener(() =>
        {
            _selectedTags.Remove(tag);
            UpdatePropList();
        });
    }

    private void UpdatePropList()
    {
        foreach(var propButton in _propButtons)
        {
            propButton.gameObject.SetActive(ShouldShow(_selectedTags, propButton));
        }

        bool ShouldShow(List<string> tags, PropSpawnerPropButton button)
        {
            if(tags.Count == 0)
            {
                return true;
            }
            foreach(var tag in button.Tags)
            {
                if (tags.Contains(tag))
                {
                    return true;
                }
            }
            return false;
        }
    }

}
