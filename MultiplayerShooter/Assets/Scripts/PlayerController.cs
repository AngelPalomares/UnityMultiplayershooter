using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform ViewPoint;
    public float MouseSensitivity = 1f;
    private float VerticalRotation;
    private Vector2 MouseInput;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        MouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + MouseInput.x,transform.rotation.eulerAngles.z);

        VerticalRotation += MouseInput.y;
        VerticalRotation = Mathf.Clamp(VerticalRotation, -60f, 60f);

        ViewPoint.rotation = Quaternion.Euler(-VerticalRotation, ViewPoint.rotation.eulerAngles.y, ViewPoint.rotation.eulerAngles.z);

    }
}
