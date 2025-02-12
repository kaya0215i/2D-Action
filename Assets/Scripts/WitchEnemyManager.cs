using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WitchEnemyManager : MonoBehaviour
{
    private Transform playerPos;
    private Animator animator;

    [SerializeField] GameObject bolt1;

    bool actionCoolTime = false;

    [SerializeField] float scale = 1;
    [SerializeField] int nDirection = 1;

    [SerializeField] bool isHoming = false;

    [SerializeField] Transform[] tpPoint;

    private int lastPoint;

    public enum DIRECTION_TYPE
    {
        RIGHT,
        LEFT,
    }

    DIRECTION_TYPE direction = DIRECTION_TYPE.LEFT;

    private void Start()
    {
        playerPos = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();

        lastPoint = 0;
    }

    private void Update()
    { 
        if(playerPos.position.x <= this.transform.position.x)
        {
            direction = DIRECTION_TYPE.LEFT;
        }
        else if(this.transform.position.x < playerPos.position.x)
        {
            direction = DIRECTION_TYPE.RIGHT;
        }

        if (direction == DIRECTION_TYPE.RIGHT)
        {
            transform.localScale = new Vector3(nDirection * -scale, scale, scale);
        }
        else if (direction == DIRECTION_TYPE.LEFT)
        {
            transform.localScale = new Vector3(nDirection * scale, scale, scale);
        }

        if(this.gameObject.GetComponent<EnemyInfo>().hp <= 1000)
        {
            isHoming = true;
            this.gameObject.GetComponent<EnemyInfo>().atk = 35;
            this.gameObject.GetComponent<EnemyInfo>().color = Color.yellow;
        }
        if(this.gameObject.GetComponent<EnemyInfo>().hp <= 100)
        {
            this.gameObject.GetComponent<EnemyInfo>().atk = 50;
        }


        if (!actionCoolTime)
        {
            if (this.transform.position.x - 20.0f < playerPos.position.x && playerPos.position.x < this.transform.position.x + 20.0f &&
                this.transform.position.y - 20.0f < playerPos.position.y && playerPos.position.y < this.transform.position.y + 20.0f)
            {
                StartCoroutine(Action());
            }
        }
    }

    IEnumerator Action()
    {
        actionCoolTime = true;


        int rnd = Random.Range(0, 3);

        if(rnd == 0)
        {
            Teleport();
        }
        else if(rnd > 0)
        {
            Attack();
        }

        yield return new WaitForSeconds(2.5f);

        actionCoolTime = false;
    }

    private void Attack()
    {
        animator.SetTrigger("attack");

        Vector2 myTrans = this.transform.position;

        switch (direction)
        {
            case DIRECTION_TYPE.RIGHT:
                myTrans.x += 1;

                break;

            case DIRECTION_TYPE.LEFT:
                myTrans.x += -1;

                break;
        }

        myTrans.y += 2;

        GameObject bolt1Config = Instantiate(bolt1, myTrans, Quaternion.identity);

        bolt1Config.GetComponent<Bolt1Manager>().targetType = Bolt1Manager.TARGET_TYPE.PLAYER;
        if(isHoming)
        {
            bolt1Config.GetComponent<Bolt1Manager>().boltType = Bolt1Manager.BOLT_TYPE.HORMING;
        }
        else if(!isHoming)
        {
            bolt1Config.GetComponent<Bolt1Manager>().boltType = Bolt1Manager.BOLT_TYPE.NORMAL;
        }
        bolt1Config.GetComponent<Bolt1Manager>().speed = 3;
        bolt1Config.GetComponent<Bolt1Manager>().atk = this.gameObject.GetComponent<EnemyInfo>().atk;
    }

    private void Teleport()
    {
        Transform tp;

        int n = Random.Range(0, tpPoint.Length);

        if(n == lastPoint)
        {
            n++;

            if(n > tpPoint.Length - 1)
            {
                n = 0;
            }
        }

        tp = tpPoint[n];

        lastPoint = n;

        this.transform.position = tp.position;
    }
}
