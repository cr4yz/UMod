using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : ItemMonoBehaviour
{

    [Header("Stick Properties")]
    [SerializeField]
    private float _swingCooldown = .55f;
    [SerializeField]
    private float _meleeRange = 1f;
    [SerializeField]
    private float _meleeRadius = .25f;
    [SerializeField]
    private float _hitForce = 10;

    private float _cooldownTimer;

    protected override void OnKeyDown(KeyCode button)
    {
        if(button != KeyCode.Mouse0)
        {
            return;
        }
        TrySwing();
    }

    protected override void Update()
    {
        base.Update();

        _cooldownTimer -= Time.deltaTime;
    }

    protected override void OnAnimationEvent(string eventName)
    {
        if(eventName == "Hit")
        {
            var ray = PlayerCamera.ViewportPointToRay(Vector3.one * .5f);
            if(Physics.SphereCast(ray, _meleeRadius, out RaycastHit hit, _meleeRange))
            {
                if (hit.rigidbody)
                {
                    hit.rigidbody.AddForceAtPosition(ray.direction * _hitForce, hit.point, ForceMode.Force);
                }
            }
        }
    }

    private void TrySwing()
    {
        if(_cooldownTimer > 0)
        {
            return;
        }
        _cooldownTimer = _swingCooldown;
        Swing();
    }

    private void Swing()
    {
        ViewModel.Animator.SetTrigger("Attack");
    }

}
