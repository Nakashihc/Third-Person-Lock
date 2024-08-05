using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarakterBergerakAutoLock : MonoBehaviour
{
    public float Jalan = 2f;
    public float Lari = 6f;
    public Animator animator;
    public Transform Kamera;
    public AutoTargetLock autoTargetLock;

    private void Update()
    {
        if (autoTargetLock.MusuhDiTargetkan)
        {
            // Liat Musuh
            Transform targetMusuh = autoTargetLock.currentTarget;
            if (targetMusuh != null)
            {
                Vector3 directionToTarget = (targetMusuh.position - transform.position).normalized;
                directionToTarget.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Jalan * Time.deltaTime * 100);
            }
            animator.SetBool("LockTarget", true);

            HandleMovement();
        }
        else
        {
            animator.SetBool("LockTarget", false);

            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 forward = Kamera.forward;
        Vector3 right = Kamera.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 movement = (forward * moveVertical + right * moveHorizontal).normalized;

        float currentSpeed = Jalan;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed = Lari;
            animator.SetBool("Lari", true);
        }
        else
        {
            animator.SetBool("Lari", false);
        }

        transform.Translate(movement * currentSpeed * Time.deltaTime, Space.World);

        if (movement != Vector3.zero)
        {
            animator.SetBool("Jalan", true);

            // Animasi W A S D Waktu Ngunci Target
            if (Input.GetKey(KeyCode.D))
            {
                animator.SetBool("DiamLock", false);
                animator.ResetTrigger("Mundur");
                animator.ResetTrigger("Kiri");
                animator.ResetTrigger("JalanLock");
                animator.ResetTrigger("Diamm");
                animator.SetTrigger("Kanan");
            }
            else if (Input.GetKey(KeyCode.A))
            {
                animator.SetBool("DiamLock", false);
                animator.ResetTrigger("Mundur");
                animator.ResetTrigger("Kanan");
                animator.ResetTrigger("JalanLock");
                animator.ResetTrigger("Diamm");
                animator.SetTrigger("Kiri");
            }
            else if (Input.GetKey(KeyCode.S))
            {
                animator.SetBool("DiamLock", false);
                animator.ResetTrigger("Kiri");
                animator.ResetTrigger("Kanan");
                animator.ResetTrigger("JalanLock");
                animator.ResetTrigger("Diamm");
                animator.SetTrigger("Mundur");
            }
            else if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool("DiamLock", false);
                animator.ResetTrigger("Kiri");
                animator.ResetTrigger("Kanan");
                animator.ResetTrigger("Mundur");
                animator.ResetTrigger("Diamm");
                animator.SetTrigger("JalanLock");
            }
            else
            {
                animator.SetBool("DiamLock", true);
                animator.ResetTrigger("Mundur");
                animator.ResetTrigger("Kanan");
                animator.ResetTrigger("Mundur");
                animator.ResetTrigger("JalanLock");
                animator.SetTrigger("Diamm");
            }

            // Bergerak Tidak Mengunci Target
            if (!autoTargetLock.MusuhDiTargetkan)
            {
                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, currentSpeed * Time.deltaTime * 100);
            }
        }
        else
        {
            animator.SetBool("Jalan", false);
        }
    }
}
