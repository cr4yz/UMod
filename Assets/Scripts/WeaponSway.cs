using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float SwaySpeed = 1f;
    public float MaxSway = 15f;

    void Update()
    {
        var pos = Quaternion.Euler(new Vector3(0, transform.localRotation.eulerAngles.y, 0));
        var moveX = Input.GetAxis("Mouse X") * SwaySpeed;
        moveX = Mathf.Clamp(moveX, -MaxSway, MaxSway);
        var targetPos = Quaternion.Lerp(pos, Quaternion.Euler(0, moveX, 0), 0.2f);
        transform.localRotation = Quaternion.Euler(targetPos.eulerAngles);
    }
}