using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolGunHudTool : MonoBehaviour
{

    [SerializeField]
    private TMP_Text _toolName;
    [SerializeField]
    private Image _labelBackground;

    public void Initialize(ToolGunTool tool)
    {
        _toolName.text = tool.ToolName;
        _labelBackground.color = new Color()
        {
            r = _labelBackground.color.r,
            g = _labelBackground.color.g,
            b = _labelBackground.color.b,
            a = 0
        };
        tool.OnSelected.AddListener(() =>
        {
            LeanTween.alpha(_labelBackground.rectTransform, 1, 0.15f);
        });
        tool.OnDeselected.AddListener(() =>
        {
            LeanTween.alpha(_labelBackground.rectTransform, 0, 0.15f);
        });
    }

}
