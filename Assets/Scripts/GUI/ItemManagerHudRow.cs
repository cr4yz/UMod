using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemManagerHudRow : MonoBehaviour
{

    [SerializeField]
    private TMP_Text _slotNumberText;
    [SerializeField]
    private GameObject _itemLabelTemplate;

    private void Start()
    {
        _itemLabelTemplate.gameObject.SetActive(false);
    }

    public void Initialize(int rowNumber, IEnumerable<ItemMonoBehaviour> items)
    {
        _slotNumberText.text = rowNumber.ToString();
        foreach (var item in items)
        {
            var clone = GameObject.Instantiate(_itemLabelTemplate, _itemLabelTemplate.transform.parent);
            clone.GetComponentInChildren<TMP_Text>().text = item.ItemName;
            clone.gameObject.SetActive(true);
            var img = clone.GetComponent<Image>();
            item.OnEquipped.AddListener(() =>
            {
                LeanTween.alpha(img.rectTransform, 1, 0.05f);
            });
            item.OnUnequipped.AddListener(() =>
            {
                LeanTween.alpha(img.rectTransform, 0, 0.05f);
            });
        }
    }

}
