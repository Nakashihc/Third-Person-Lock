using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class AutoTargetLock : MonoBehaviour
{
    [Header("Kamera")]
    [SerializeField] private Camera KameraUtama;
    [SerializeField] private CinemachineFreeLook kameraCinemachine;

    [Header("Aim Icon")]
    [SerializeField] private Image aimIcon;

    [Header("Settings")]
    [SerializeField] private string tagMusuh;
    [SerializeField] private Vector2 TargetOffset;
    [SerializeField] private float JarakMin;
    [SerializeField] private float JarakMax;

    public bool MusuhDiTargetkan;
    private float maxAngle;
    public Transform currentTarget;
    private float mouseX;
    private float mouseY;

    private List<Transform> targetsInRange = new List<Transform>();
    private int currentTargetIndex = 0;

    void Start()
    {
        maxAngle = 90f;
        kameraCinemachine.m_XAxis.m_InputAxisName = "";
        kameraCinemachine.m_YAxis.m_InputAxisName = "";
        UpdateTargetsInRange();
    }

    void Update()
    {
        UpdateTargetsInRange();

        if (MusuhDiTargetkan)
        {
            if (currentTarget)
            {
                float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
                if (distanceToTarget > JarakMax)
                {
                    MusuhDiTargetkan = false;
                    currentTarget = null;
                    UpdateTargetsInRange();
                }
                else
                {
                    NewInputTarget(currentTarget);
                }
            }
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }

        if (aimIcon)
            aimIcon.gameObject.SetActive(MusuhDiTargetkan);

        kameraCinemachine.m_XAxis.m_InputAxisValue = mouseX;
        kameraCinemachine.m_YAxis.m_InputAxisValue = mouseY;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (MusuhDiTargetkan)
            {
                SwitchTarget();
            }
            else
            {
                AssignTarget();
            }
        }
    }

    private void UpdateTargetsInRange()
    {
        targetsInRange.Clear();
        GameObject[] gos = GameObject.FindGameObjectsWithTag(tagMusuh);

        foreach (GameObject go in gos)
        {
            float distance = Vector3.Distance(transform.position, go.transform.position);
            if (distance >= JarakMin && distance <= JarakMax)
            {
                targetsInRange.Add(go.transform);
            }
        }

        if (targetsInRange.Count > 0)
        {
            if (!MusuhDiTargetkan)
            {
                currentTargetIndex = 0; // Focus on the first target
                currentTarget = targetsInRange[currentTargetIndex];
                MusuhDiTargetkan = true;
            }
        }
        else
        {
            MusuhDiTargetkan = false;
            currentTarget = null;
        }
    }

    private void AssignTarget()
    {
        if (targetsInRange.Count > 0)
        {
            currentTarget = targetsInRange[currentTargetIndex];
            MusuhDiTargetkan = true;
        }
    }

    private void SwitchTarget()
    {
        if (targetsInRange.Count > 0)
        {
            currentTargetIndex = (currentTargetIndex + 1) % targetsInRange.Count;
            currentTarget = targetsInRange[currentTargetIndex];
        }
    }

    private void NewInputTarget(Transform target)
    {
        if (!currentTarget) return;

        Vector3 viewPos = KameraUtama.WorldToViewportPoint(target.position);

        if (aimIcon)
            aimIcon.transform.position = KameraUtama.WorldToScreenPoint(target.position);

        if ((target.position - transform.position).magnitude < JarakMin) return;
        mouseX = (viewPos.x - 0.5f + TargetOffset.x) * 3f;
        mouseY = (viewPos.y - 0.5f + TargetOffset.y) * 3f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, JarakMax);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, JarakMin);
    }
}
