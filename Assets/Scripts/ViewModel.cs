using System;
using System.Collections;
using UnityEngine;

public class ViewModel : MonoBehaviour
{
    [SerializeField]
    private GameObject _animationTarget;
    [SerializeField]
    private Transform _barrel;
    [SerializeField]
    private float _swaySpeed = .5f;
    [SerializeField]
    private float _maxSway = .15f;
    [SerializeField]
    private float _swayRecovery = 10f;

    private Vector3 _swayOffset;
    private Vector3 _defaultPosition;

    public Vector3 BarrelPosition => _barrel != null ? _barrel.position : transform.position;
    public Animator Animator { get; private set; }

    private void Awake()
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
        var maxSway = _maxSway / _animationTarget.transform.lossyScale.x;
        var swaySpeed = _swaySpeed / _animationTarget.transform.lossyScale.x;
        var swayRecovery = _swayRecovery / _animationTarget.transform.lossyScale.x;

        if (!GUIManager.Instance.GUIHasCursor())
        {
            var swayX = -Input.GetAxisRaw("Mouse X") * Time.deltaTime;
            _swayOffset.x = Mathf.Clamp(_swayOffset.x + swayX, -maxSway, maxSway);
        }

        _swayOffset = Vector3.Lerp(_swayOffset, Vector3.zero, swayRecovery * Time.deltaTime);
        _animationTarget.transform.localPosition = Vector3.MoveTowards(_animationTarget.transform.localPosition, _defaultPosition + _swayOffset, swaySpeed * Time.deltaTime);
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
            LeanTween.rotateAroundLocal(_animationTarget, Vector3.right, -5f * intensity, .09f).setOnComplete(() =>
            {
                LeanTween.rotateAroundLocal(_animationTarget, Vector3.right, 5f * intensity, .1f).setOnComplete(() =>
                {
                    _kickCanRotate = true;
                });
            });
        }
    }

    public IEnumerator FireProjectile(GameObject projectile, Transform target, float speed, Action finishCallback = null)
    {
        projectile.transform.position = BarrelPosition;
        projectile.transform.LookAt(target.transform.position);

        while (target && Vector3.Distance(projectile.transform.position, target.transform.position) > .1f)
        {
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, target.transform.position, speed * Time.deltaTime);
            yield return null;
        }

        GameObject.Destroy(projectile);

        finishCallback?.Invoke();
    }

    public IEnumerator FireProjectile(GameObject projectile, Vector3 endPoint, float speed, Action finishCallback = null)
    {
        projectile.transform.position = BarrelPosition;
        projectile.transform.LookAt(endPoint);
        var dir = (endPoint - BarrelPosition).normalized;
        var velocity = dir * speed;

        while(Vector3.Distance(projectile.transform.position, endPoint) > .1f)
        {
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, endPoint, speed * Time.deltaTime);
            yield return null;
        }

        GameObject.Destroy(projectile);

        finishCallback?.Invoke();
    }

}
