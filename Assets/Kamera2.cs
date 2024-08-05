using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamera2 : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Konfigurasi Kamera dengan Player")]
    public float distance = 10.0f;
    public float smoothSpeed = 0.125f;
    public float lookSpeed = 2.0f;
    public float minYAngle = -40f;
    public float maxYAngle = 80f;

    [Header("Kamera Jika ada Obstacle")]
    public LayerMask obstacleLayer;
    public float jarakMenjauh = 0.5f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    [Header("Musuh")]
    public float radiusMusuh = 10.0f;
    public float kecepatanLihatMusuh = 2.0f;
    public float faktorLihatTengah = 0.5f;

    private Transform musuhTerdekat;
    private Vector3 desiredPosition;
    private Quaternion targetRotation;

    void LateUpdate()
    {
        yaw += lookSpeed * Input.GetAxis("Mouse X");
        pitch -= lookSpeed * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredOffset = rotation * new Vector3(0, 0, -distance);
        desiredPosition = player.position + desiredOffset;

        RaycastHit hit;
        if (Physics.Raycast(player.position, desiredOffset.normalized, out hit, distance, obstacleLayer))
        {
            desiredPosition = player.position + (hit.distance - jarakMenjauh) * desiredOffset.normalized;
        }

        musuhTerdekat = TemukanMusuhTerdekat();

        if (musuhTerdekat != null)
        {
            Vector3 tengahTitik = Vector3.Lerp(player.position, musuhTerdekat.position, faktorLihatTengah);
            Vector3 directionToCenter = tengahTitik - transform.position;
            targetRotation = Quaternion.LookRotation(directionToCenter);

            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * kecepatanLihatMusuh);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            targetRotation = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed);
        }
    }

    Transform TemukanMusuhTerdekat()
    {
        Collider[] colliders = Physics.OverlapSphere(player.position, radiusMusuh);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector3.Distance(player.position, collider.transform.position);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = collider.transform;
                }
            }
        }

        return closestEnemy;
    }
}