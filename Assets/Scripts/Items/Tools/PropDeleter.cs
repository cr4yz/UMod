using System.Collections.Generic;
using UnityEngine;

public class PropDeleter : ToolGunTool
{

    private List<GameObject> _destroyOnDisable = new List<GameObject>();

    protected override void OnDisable()
    {
        base.OnDisable();

        foreach(var obj in _destroyOnDisable)
        {
            GameObject.Destroy(obj);
        }
        _destroyOnDisable.Clear();
    }

    public override void HandleKeyDown(ToolGun toolGun, KeyCode button, Ray eyeRay)
    {
        if(Physics.Raycast(eyeRay, out RaycastHit hit))
        {
            if (hit.rigidbody)
            {
                var prop = hit.rigidbody.GetComponentInParent<PropRoot>();
                if (!prop)
                {
                    return;
                }
                var projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                projectile.transform.localScale = Vector3.one * .1f;
                _destroyOnDisable.Add(projectile);
                _destroyOnDisable.Add(hit.transform.gameObject);
                StartCoroutine(toolGun.ViewModel.FireProjectile(projectile, hit.transform, 40f, () =>
                {
                    if (!prop.transform)
                    {
                        return;
                    }
                    LeanTween.scale(prop.transform.gameObject, prop.transform.localScale * 1.25f, .05f)
                        .setOnComplete(() =>
                        {
                            LeanTween.scale(prop.transform.gameObject, Vector3.zero, .15f)
                                .setOnComplete(() =>
                                {
                                    GameObject.Destroy(prop.gameObject);
                                });
                        });
                })); 
            }
        }
    }

}
