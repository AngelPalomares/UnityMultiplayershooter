using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public static Testing instance;

    private void Awake()
    {
        instance = this;
    }
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] public int Health;

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(horizontalInput + mouseX, verticalInput + mouseY, 0f) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }
}
