using UnityEngine;

public class ViewModel : MonoBehaviour
{
    [SerializeField]
    private GameObject _animationTarget;
    [SerializeField]
    private Transform _barrel;
    [SerializeField]
    private float _swaySpeed = 2;
    [SerializeField]
    private float _maxSway = 6;
    [SerializeField]
    private float _swayRecovery = 6f;

    private Vector3 _swayOffset;
    private Vector3 _defaultPosition;

    public Vector3 BarrelPosition => _barrel != null ? _barrel.position : transform.position;
    public Animator Animator { get; private set; }

    private void Start()
    {
        if (!_animationTarget)
        {
            _animationTarget = gameObject;
        }
        Animator = GetComponentInChildren<Animator>();
        _defaultPosition = _animationTarget.transform.localPosition;
    }

    private void Update()
    {
        var swayX = -Input.GetAxisRaw("Mouse X") * Time.deltaTime;
        _swayOffset.x = Mathf.Clamp(_swayOffset.x + swayX, -_maxSway, _maxSway);
        _swayOffset = Vector3.Lerp(_swayOffset, Vector3.zero, _swayRecovery * Time.deltaTime);

        _animationTarget.transform.localPosition = Vector3.MoveTowards(_animationTarget.transform.localPosition, _defaultPosition + _swayOffset, _swaySpeed * Time.deltaTime);
    }

    private bool _kickCanRotate = true;
    public void Kick(float intensity = 1f)
    {
        LeanTween.moveLocalZ(_animationTarget, _defaultPosition.z - .1f * intensity, .09f).setOnComplete(() =>
        {
            LeanTween.moveLocalZ(_animationTarget, _defaultPosition.z, .1f).setEase(LeanTweenType.easeOutQuint);
        }).setEase(LeanTweenType.easeOutCubic);
        if (_kickCanRotate)
        {
            _kickCanRotate = false;
            LeanTween.rotateAroundLocal(_animationTarget, Vector3.right, 5f * intensity, .09f).setOnComplete(() =>
            {
                LeanTween.rotateAroundLocal(_animationTarget, Vector3.right, -5f * intensity, .1f).setOnComplete(() =>
                {
                    _kickCanRotate = true;
                });
            });
        }
    }

}
