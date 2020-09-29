using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ItemManager : MonoBehaviour
{

    public UnityEvent<ItemMonoBehaviour> OnItemEquipped = new UnityEvent<ItemMonoBehaviour>();

    private ItemMonoBehaviour _activeItem;

    private Dictionary<KeyCode, int> _itemSlots = new Dictionary<KeyCode, int>
    {
        { KeyCode.Alpha1, 1 },
        { KeyCode.Alpha2, 2 },
        { KeyCode.Alpha3, 3 },
        { KeyCode.Alpha4, 4 },
        { KeyCode.Alpha5, 5 },
        { KeyCode.Alpha6, 6 }
    };

    public ItemMonoBehaviour[] Items { get; private set; }

    private void Awake()
    {
        Items = GetComponentsInChildren<ItemMonoBehaviour>();
        foreach(var item in Items)
        {
            item.gameObject.SetActive(false);
        }
        StartCoroutine(EquipNextItem());
    }

    private void Update()
    {
        foreach(var kvp in _itemSlots)
        {
            if (Input.GetKeyDown(kvp.Key))
            {
                StartCoroutine(EquipNextItem(kvp.Value));
            }
        }
    }

    private IEnumerator EquipNextItem(int slot = -1)
    {
        ItemMonoBehaviour nextItem = null;

        if (_activeItem && _activeItem.SlotNumber == slot)
        {
            nextItem = Items
                .SkipWhile(x => x != _activeItem)
                .Skip(1)
                .FirstOrDefault(x => x.SlotNumber == slot);
        }

        if (!nextItem)
        {
            nextItem = Items.FirstOrDefault(x => slot == -1 || x.SlotNumber == slot);
            if (!nextItem)
            {
                yield break;
            }
        }

        if (_activeItem)
        {
            yield return _activeItem.Hide();
        }

        _activeItem = nextItem;
        _activeItem.gameObject.SetActive(true);
        OnItemEquipped?.Invoke(_activeItem);
        yield return _activeItem.Show();
    }

}
