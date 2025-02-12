using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdEnemyManager : MonoBehaviour
{
    [SerializeField] float scale = 0.0f;
    [SerializeField] float flyingRangeX = 0.0f;
    [SerializeField] float flyingRangeY = 0.0f;
    [SerializeField] int nDirection = 1;
    [SerializeField] float speed = 1;

    float time = 0.0f;

    float xPos = 0.0f;
    float yPos = 0.0f;

    public enum DIRECTION_TYPE
    {
        RIGHT,
        LEFT,
    }

    DIRECTION_TYPE direction = DIRECTION_TYPE.LEFT;

    Rigidbody2D myRigidbody;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();

        transform.localScale = new Vector3(nDirection * scale, scale, scale);

        xPos = this.transform.position.x;
        yPos = this.transform.position.y;
    }

    private void Update()
    {
        time += Time.deltaTime;
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

        this.transform.position = new Vector3(Mathf.Sin(time * speed) * flyingRangeX + xPos, Mathf.Sin(time * (speed * 1.5f)) * flyingRangeY + yPos, -1);
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
}
