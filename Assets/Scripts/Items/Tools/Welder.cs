using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Welder : ToolGunTool
{

    private class SingleWeld
    {
        public Rigidbody A;
        public Rigidbody B;
        public FixedJoint Joint;

        public bool IsWelded(Rigidbody a, Rigidbody b)
        {
            return (a == A && b == B) || (a == B && b == A);
        }
    }

    private Vector3 _firstAnchor;
    private Rigidbody _firstRigidbody;
    private List<SingleWeld> _welds = new List<SingleWeld>();

    protected override void OnDisable()
    {
        base.OnDisable();

        _firstRigidbody = null;
    }

    public override void HandleKeyDown(ToolGun toolGun, KeyCode button, Ray eyeRay)
    {
        if(!Physics.Raycast(eyeRay, out RaycastHit hit)
            || !hit.rigidbody)
        {
            return;
        }

        var projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.localScale = Vector3.one * .15f;

        if(button == KeyCode.Mouse0)
        {
            projectile.GetComponent<Renderer>().material.color = Color.green;

            if (_firstRigidbody && hit.rigidbody != _firstRigidbody)
            {
                Weld(_firstRigidbody, hit.rigidbody, _firstAnchor, hit.point);
                _firstRigidbody = null;
            }
            else
            {
                _firstAnchor = hit.point;
                _firstRigidbody = hit.rigidbody;
            }
        }
        else if(button == KeyCode.Mouse1)
        {
            projectile.GetComponent<Renderer>().material.color = Color.red;
            _firstRigidbody = null;
            Unweld(hit.rigidbody);
        }

        StartCoroutine(toolGun.ViewModel.FireProjectile(projectile, hit.transform, 40f, () =>
        {
            if (!hit.transform)
            {
                return;
            }
            var originalScale = hit.transform.localScale;
            LeanTween.scale(hit.transform.gameObject, originalScale * 1.25f, .05f)
                .setOnComplete(() =>
                {
                    LeanTween.scale(hit.transform.gameObject, originalScale, .15f);
                });
        }));
    }

    private void Weld(Rigidbody a, Rigidbody b, Vector3 anchorA, Vector3 anchorB)
    {
        foreach(var weld in _welds)
        {
            if(weld.IsWelded(a, b))
            {
                return;
            }
        }
        var fixedJoint = a.gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = b;
        fixedJoint.anchor = anchorA;
        fixedJoint.connectedAnchor = anchorB;
        _welds.Add(new SingleWeld()
        {
            A = a,
            B = b,
            Joint = fixedJoint
        });
    }

    private void Unweld(Rigidbody rigidbody)
    {
        for (int i = _welds.Count - 1; i >= 0; i--)
        {
            var weld = _welds[i];
            if (weld.A == rigidbody || weld.B == rigidbody)
            {
                GameObject.Destroy(weld.Joint);
                _welds.RemoveAt(i);
            }
        }
    }

}
