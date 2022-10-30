using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBall : MonoBehaviour
{
    [SerializeField] Ball ball;
    [SerializeField] float speed = 15f;

    public bool IsMoving => this.transform.position == ball.Position;
    void Update()
    {
        if (this.transform.position == ball.Position)
            return;

        // camera follow the ball
        this.transform.position = Vector3.Lerp(
            this.transform.position,
            ball.Position,
            Time.deltaTime * speed
            );

        if (Vector3.Distance(transform.position, ball.Position) < 1f)
        {
            transform.position = ball.Position;
        }
    }
}
