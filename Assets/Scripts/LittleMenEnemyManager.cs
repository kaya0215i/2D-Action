using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleMenEnemyManager : MonoBehaviour
{
    [SerializeField] LayerMask blockLayer;

    [SerializeField] float scale = 0.0f;
    [SerializeField] float lineStartPoint = 0.0f;
    [SerializeField] int nDirection = 1;
    [SerializeField] float speed = 1;

    Animator animator;

    public enum DIRECTION_TYPE
    {
        RIGHT,
        LEFT,
    }

    DIRECTION_TYPE direction = DIRECTION_TYPE.RIGHT;

    Rigidbody2D myRigidbody;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();

        direction = DIRECTION_TYPE.RIGHT;
        transform.localScale = new Vector3(nDirection * scale, scale, scale);

        animator = GetComponent<Animator>();

        animator.SetBool("isMoving", true);
    }

    void Update()
    {
        if (!IsGround())
        {
            ChangeDirection();
        }
        if (IsWall())
        {
            ChangeDirection();
        }
    }

    private void FixedUpdate()
    {
        float move = 0;

        switch (direction)
        {
            case DIRECTION_TYPE.RIGHT:
                move = 1 * speed;
                break;

            case DIRECTION_TYPE.LEFT:
                move = -1 * speed;
                break;
        }

        myRigidbody.velocity = new Vector2(move, myRigidbody.velocity.y);
    }

    bool IsGround()
    {
        Vector3 startPoint = transform.position + transform.right * nDirection * lineStartPoint * transform.localScale.x;
        Vector3 endPoint = startPoint - transform.up * 1.0f;

        Debug.DrawLine(startPoint, endPoint);

        return Physics2D.Linecast(startPoint, endPoint, blockLayer);
    }

    bool IsWall()
    {
        Vector3 startPoint = transform.position + transform.right * nDirection * lineStartPoint * transform.localScale.x;
        Vector3 endPoint = new Vector3(0.0f, 0.0f, 0.0f);

        switch (direction)
        {
            case DIRECTION_TYPE.RIGHT:
                endPoint = startPoint + new Vector3(0.1f, 0.0f, 0.0f);
                break;

            case DIRECTION_TYPE.LEFT:
                endPoint = startPoint + new Vector3(-0.1f, 0.0f, 0.0f);
                break;
        }

        Debug.DrawLine(startPoint, endPoint);

        return Physics2D.Linecast(startPoint, endPoint, blockLayer);
    }

    void ChangeDirection()
    {
        if (direction == DIRECTION_TYPE.RIGHT)
        {
            direction = DIRECTION_TYPE.LEFT;
            transform.localScale = new Vector3(nDirection * -scale, scale, scale);
        }
        else if (direction == DIRECTION_TYPE.LEFT)
        {
            direction = DIRECTION_TYPE.RIGHT;
            transform.localScale = new Vector3(nDirection * scale, scale, scale);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ChangeDirection();
        }
    }
}
