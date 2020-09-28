using UnityEngine;

public class PhysGun : ItemMonoBehaviour
{
    [Header("PhysGun Properties")]
    [SerializeField]
    private float _maxGrabDistance = 40f;
    [SerializeField]
    private float _minGrabDistance = 1f;
    [SerializeField]
    private LineRenderer _pickLine;

    private Rigidbody _grabbedObject;
    private float _pickDistance;
    private Vector3 _pickOffset;
    private Quaternion _rotationOffset;
    private Vector3 _pickTargetPosition;
    private Vector3 _pickForce;

    private void Start()
    {
        if (!_pickLine)
        {
            var obj = new GameObject("PhysGun Pick Line");
            _pickLine = obj.AddComponent<LineRenderer>();
            _pickLine.startWidth = 0.02f;
            _pickLine.endWidth = 0.02f;
            _pickLine.useWorldSpace = true;
            _pickLine.gameObject.SetActive(false);
        }
    }

    protected override void OnKeyDown(KeyCode button)
    {
        switch (button) 
        {
            case KeyCode.Mouse0:
                Grab();
                ViewModel.Kick();
                break;
        }
    }

    protected override void OnKeyUp(KeyCode button)
    {
        switch (button)
        {
            case KeyCode.Mouse0:
            case KeyCode.Mouse1:
                if (_grabbedObject)
                {
                    Release(button == KeyCode.Mouse1);
                    ViewModel.Kick(-.2f);
                }
                break;
        }
    }

    protected override void OnScrollDelta(float delta)
    {
        _pickDistance = Mathf.Clamp(_pickDistance + delta, _minGrabDistance, _maxGrabDistance);
        ViewModel.Kick(.2f * delta < 0 ? -1 : 1);
    }

    private void LateUpdate()
    {
        if (_grabbedObject)
        {
            var midpoint = PlayerCamera.transform.position + PlayerCamera.transform.forward * _pickDistance * .5f;
            DrawQuadraticBezierCurve(_pickLine, ViewModel.BarrelPosition, midpoint, _grabbedObject.position + _grabbedObject.transform.TransformVector(_pickOffset));
        }
    }

    private void FixedUpdate()
    {
        if (_grabbedObject != null)
        {
            var ray = PlayerCamera.ViewportPointToRay(Vector3.one * .5f);
            _pickTargetPosition = (ray.origin + ray.direction * _pickDistance) - _grabbedObject.transform.TransformVector(_pickOffset);
            var forceDir = _pickTargetPosition - _grabbedObject.position;
            _pickForce = forceDir / Time.fixedDeltaTime * 0.3f / _grabbedObject.mass;
            _grabbedObject.velocity = _pickForce;
            _grabbedObject.transform.rotation = PlayerCamera.transform.rotation * _rotationOffset;
            
        }
    }

    private void Grab()
    {
        var ray = PlayerCamera.ViewportPointToRay(Vector3.one * .5f);
        if(Physics.Raycast(ray, out RaycastHit hit, _maxGrabDistance, layerMask: 1 << 0)
            && hit.rigidbody != null)
        {
            _rotationOffset = Quaternion.Inverse(PlayerCamera.transform.rotation) * hit.rigidbody.rotation;
            _pickOffset = hit.transform.InverseTransformVector(hit.point - hit.transform.position);
            _pickDistance = hit.distance;
            _grabbedObject = hit.rigidbody;
            _grabbedObject.collisionDetectionMode = CollisionDetectionMode.Continuous;
            _grabbedObject.useGravity = false;
            _grabbedObject.freezeRotation = true;
            _grabbedObject.isKinematic = false;
            _pickLine.gameObject.SetActive(true);
        }
    }

    private void Release(bool setKinematic = false)
    {
        _grabbedObject.collisionDetectionMode = CollisionDetectionMode.Discrete;
        _grabbedObject.useGravity = true;
        _grabbedObject.freezeRotation = false;
        _grabbedObject.isKinematic = setKinematic;
        _grabbedObject = null;
        _pickLine.gameObject.SetActive(false);
    }

    // https://www.codinblack.com/how-to-draw-lines-circles-or-anything-else-using-linerenderer/
    void DrawQuadraticBezierCurve(LineRenderer line, Vector3 point0, Vector3 point1, Vector3 point2)
    {
        line.positionCount = 10;
        float t = 0f;
        Vector3 B = new Vector3(0, 0, 0);
        for (int i = 0; i < line.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
            line.SetPosition(i, B);
            t += (1 / (float)line.positionCount);
        }
    }

}
