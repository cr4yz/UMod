using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityGun : ItemMonoBehaviour
{

    private Rigidbody _grabbedObject;

    protected override void OnDisable()
    {
        base.OnDisable();

        _grabbedObject = null;
    }

    protected override void OnKeyDown(KeyCode button)
    {
        var ray = PlayerCamera.ViewportPointToRay(Vector3.one * .5f);

        if (button == KeyCode.Mouse0)
        {
            if(Physics.Raycast(ray, out RaycastHit hit)
                && hit.rigidbody
                && !hit.rigidbody.CompareTag("Player"))
            {
                _grabbedObject = hit.rigidbody;
            }
        }
        else if(button == KeyCode.Mouse1 && _grabbedObject)
        {
            _grabbedObject.velocity = PlayerCamera.transform.forward * 30f;
            _grabbedObject = null;
        }
    }

    protected override void OnKeyUp(KeyCode button)
    {
        if(button == KeyCode.Mouse0)
        {
            _grabbedObject = null;
        }
    }

    private void FixedUpdate()
    {
        if (_grabbedObject)
        {
            var targetPosition = PlayerCamera.transform.position + PlayerCamera.transform.forward * 2f;
            var forceDir = targetPosition - _grabbedObject.position;
            var force = forceDir / Time.fixedDeltaTime * 0.3f / _grabbedObject.mass;
            _grabbedObject.velocity = force;
            _grabbedObject.transform.Rotate(PlayerCamera.transform.forward, 20f * Time.fixedDeltaTime);
        }
    }

}
