using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public float MoveSpeed = 4f;
    public float MouseSensitivity = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
    }

    // Update is called once per frame
    void Update()
    {
        var mouseX = Input.GetAxisRaw("Mouse X");
        var mouseY = -Input.GetAxisRaw("Mouse Y");
        var rot = Camera.main.transform.eulerAngles;
        var rotationVector = new Vector3(mouseY, mouseX, 0);
        rot += rotationVector * MouseSensitivity;
        Camera.main.transform.rotation = Quaternion.Euler(rot);

        var sideMove = Input.GetAxisRaw("Horizontal");
        var forwardMove = Input.GetAxisRaw("Vertical");
        var moveVector = new Vector3(sideMove, 0, forwardMove) * MoveSpeed;
        moveVector = Camera.main.transform.TransformDirection(moveVector);
        moveVector.y = 0;
        transform.Translate(moveVector * Time.deltaTime);
    }
}
