using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KameraKontrol : MonoBehaviour
{
    [SerializeField] Transform TargetLihat;

    [SerializeField] float kecepatanRotasi = 2f;
    [SerializeField] float jarak = 5f;

    [SerializeField] float minAngle = -45;
    [SerializeField] float maxAngle = 45;

    [SerializeField] Vector2 Offset;

    float rotX;
    float rotY;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        rotY += Input.GetAxis("Mouse X") * kecepatanRotasi;

        rotX += Input.GetAxis("Mouse Y") * kecepatanRotasi;
        rotX = Mathf.Clamp(rotX, minAngle, maxAngle);

        var targetRotasi = Quaternion.Euler(rotX, rotY, 0);
        var posisiFokus = TargetLihat.position + new Vector3 (Offset.x, Offset.y);

        transform.position = posisiFokus - targetRotasi * new Vector3(0, 0, jarak);
        transform.rotation = targetRotasi;
    }
}
