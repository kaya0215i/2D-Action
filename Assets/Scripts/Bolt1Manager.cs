using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt1Manager : MonoBehaviour
{
    public enum TARGET_TYPE
    {
        PLAYER,
        ENEMY,
    }

    public TARGET_TYPE targetType;

    public enum BOLT_TYPE
    {
        NORMAL,
        HORMING,
    }

    public BOLT_TYPE boltType;

    // The target marker.
    private Transform target;

    // Speed in units per sec.
    public float speed;

    public float atk;

    [SerializeField] GameObject endAmimation;

    public float scale = 1;

    private float lifeTime = 5;

    private void Start()
    {
        this.transform.localScale = new Vector3(scale, scale, scale);

        switch (targetType)
        {
            case TARGET_TYPE.PLAYER:
                target = GameObject.Find("Player").GetComponent<Transform>();

                break;

            case TARGET_TYPE.ENEMY:
                target = GameObject.Find("Target").GetComponent<Transform>();

                break;
        }

        switch (boltType)
        {
            case BOLT_TYPE.NORMAL:
                Vector3 dir = (target.position - this.transform.position);

                

                this.transform.rotation = Quaternion.FromToRotation(Vector3.left, dir);

                break;

            case BOLT_TYPE.HORMING:

                break;
        }

        Invoke("DestroyThisObject", lifeTime);
    }

    private void Update()
    {
        switch (boltType)
        {
            case BOLT_TYPE.NORMAL:
                Vector3 velocity = gameObject.transform.rotation * new Vector3(speed, 0, 0);
                gameObject.transform.position -= velocity * Time.deltaTime;

                break;

            case BOLT_TYPE.HORMING:
                // The step size is equal to speed times frame time.
                float step = speed * Time.deltaTime;

                // Move our position a step closer to the target.
                this.transform.position = Vector3.MoveTowards(this.transform.position, target.position, step);

                Vector3 dir = (target.position - this.transform.position);

                this.transform.rotation = Quaternion.FromToRotation(Vector3.left, dir);

                break;
        } 
    }

    private void DestroyThisObject()
    {
        Instantiate(endAmimation, this.gameObject.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    public GameObject FindClosestEnemy()
    {
        // EnemyのTagを持つゲームオブジェクトの配列
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
        // 最も近い位置に存在するEnemy
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        // 最も近かったEnemyを返す
        return closest;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (targetType)
        {
            case TARGET_TYPE.PLAYER:
                if (collision.gameObject.CompareTag("Player"))
                {
                    collision.gameObject.GetComponent<PlayerManager>().AttackToPlayer(atk);
                    DestroyThisObject();
                }

                break;

            case TARGET_TYPE.ENEMY:
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    collision.gameObject.GetComponent<EnemyInfo>().AttackToEnemy(atk);
                    DestroyThisObject();
                }

                break;
        }
    }
}
