using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Ball ball;
    [SerializeField] GameObject arrow;
    [SerializeField] Image aim;
    [SerializeField] LineRenderer line;
    [SerializeField] TMP_Text shootCountText;
    [SerializeField] LayerMask ballLayer;
    [SerializeField] LayerMask rayLayer;
    [SerializeField] FollowBall cameraPivot;
    [SerializeField] Camera cam;
    [SerializeField] Vector2 camSensitiviy;
    [SerializeField] float shootforce;

    private AudioSource hitSound;

    Vector3 lastMousePosition;
    float ballDistance;
    bool isShooting;
    Vector3 forceDirection;
    float forceFactor;

    Renderer[] arrowRends;
    int shootCount = 0;

    public int ShootCount { get => shootCount; }

    private void Start()
    {
        ballDistance = Vector3.Distance(
            cam.transform.position, ball.Position) + 1;

        arrowRends = arrow.GetComponentsInChildren<Renderer>();

        arrow.SetActive(false);
        shootCountText.text = "Shoot : " + shootCount;

        line.enabled = false;
    }
    void Update()
    {

        if (ball.IsMoving || ball.IsTeleporting)
            return;

        if (!cameraPivot.IsMoving && aim.gameObject.activeInHierarchy == false)
        {
            aim.gameObject.SetActive(true);
            var rect = aim.GetComponent<RectTransform>();
            rect.anchoredPosition = cam.WorldToScreenPoint(ball.Position);
        }

        if (this.transform.position != ball.Position)
        {

            this.transform.position = ball.Position;

            aim.gameObject.SetActive(true);
            var rect = aim.GetComponent<RectTransform>();
            rect.anchoredPosition = cam.WorldToScreenPoint(ball.Position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, ballDistance, ballLayer))
            {
                isShooting = true;
                arrow.SetActive(true);

                line.enabled = true;

            }
        }


        // shooting mode
        if (Input.GetMouseButton(0) && isShooting == true)
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, ballDistance * 2, rayLayer))
            {
                Debug.DrawLine(ball.Position, hit.point);

                var forceVector = ball.Position - hit.point;
                forceVector = new Vector3(forceVector.x, 0, forceVector.z);
                forceDirection = forceVector.normalized;
                var forceMagnitude = forceVector.magnitude;

                Debug.Log(forceMagnitude);
                forceMagnitude = Mathf.Clamp(forceMagnitude, 0, 5);

                forceFactor = forceMagnitude / 5;
            }

            //arrow
            this.transform.LookAt(this.transform.position + forceDirection);
            arrow.transform.localScale = new Vector3(
                1 + 0.5f * forceFactor,
                1 + 0.5f * forceFactor,
                1 + 2 * forceFactor
            );

            foreach (var rend in arrowRends)
            {
                rend.material.color = Color.Lerp(Color.white, Color.red, forceFactor);
            }

            //aim
            var rect = aim.GetComponent<RectTransform>();
            rect.anchoredPosition = Input.mousePosition;

            //line
            var ballScreenPos = cam.WorldToScreenPoint(ball.Position);
            line.SetPositions(new Vector3[] { ballScreenPos, Input.mousePosition });


        }

        // camera mode
        if (Input.GetMouseButton(0) && isShooting == false)
        {
            var current = cam.ScreenToViewportPoint(Input.mousePosition);
            var last = cam.ScreenToViewportPoint(lastMousePosition);

            var delta = current - last;

            // rotate ball horizontal
            cameraPivot.transform.RotateAround(
                ball.Position,
                Vector3.up,
                delta.x * camSensitiviy.x);

            // rotate ball vertical
            cameraPivot.transform.RotateAround(
                ball.Position,
                cam.transform.right,
                -delta.y * camSensitiviy.y);

            var angle = Vector3.SignedAngle(
                Vector3.up,
                cam.transform.up,
                cam.transform.right);

            // batas rotasi sudut kamera
            if (angle < 3 || angle > 65)
            {
                cameraPivot.transform.RotateAround(
                   ball.Position,
                   cam.transform.right,
                   delta.y * camSensitiviy.y);
            }

        }

        if (Input.GetMouseButtonUp(0) & isShooting)
        {
            ball.AddForce(forceDirection * shootforce * forceFactor);
            shootCount += 1;
            shootCountText.text = "Shoot : " + shootCount;
            forceFactor = 0;
            forceDirection = Vector3.zero;
            isShooting = false;
            arrow.SetActive(false);

            aim.gameObject.SetActive(false);
            line.enabled = false;

            hitSound = GetComponent<AudioSource>();
            hitSound.Play();
        }
        lastMousePosition = Input.mousePosition;
    }
}
