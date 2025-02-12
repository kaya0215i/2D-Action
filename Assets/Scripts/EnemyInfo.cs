using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    [SerializeField] GameObject enemyDeathEffect;
    [SerializeField] GameObject expGem;

    [SerializeField] int gemAmount;

    GameManager gameManager;

    [SerializeField] public float hp;
    [SerializeField] public float atk;
    [SerializeField] public int scoreVolue;

    public enum COLOR_TYPE
    {
        NORMAL,
        GREEN,
        BLUE,
        YELLOW,
    }

    [SerializeField] COLOR_TYPE colorType;

    public Color color;

    private bool attackCoolTime = false;

    private void Start()
    {
        switch (colorType)
        {
            case COLOR_TYPE.NORMAL:
                color = Color.white;

                break;

            case COLOR_TYPE.GREEN:
                color = Color.green;

                break;

            case COLOR_TYPE.BLUE:
                color = Color.blue;

                break;

            case COLOR_TYPE.YELLOW:
                color = Color.yellow;

                break;
        }

        var children = GetChildrenRecursive(this.transform, false);

        for (var i = 0; i < children.Length; i++)
        {
            SpriteRenderer spriteRenderer = children[i].GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
        }
    }

    private void Update()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (hp == 0)
        {
            DestroyEnemy();
        }
    }

    public void AttackToEnemy(float damageVolume)
    {
        StartCoroutine(ChangeColor());

        Debug.Log("Damage:" + damageVolume);

        hp -= damageVolume;

        if(hp < 0)
        {
            hp = 0;
        }
    }

    IEnumerator ChangeColor()
    {
        // 子オブジェクトを取得する
        var children = GetChildrenRecursive(this.transform, false);

        for (var i = 0; i < children.Length; i++)
        {
            SpriteRenderer spriteRenderer = children[i].GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.red;
        }

        yield return new WaitForSeconds(0.2f);

        for (var i = 0; i < children.Length; i++)
        {
            SpriteRenderer spriteRenderer = children[i].GetComponent<SpriteRenderer>();

            spriteRenderer.color = color;
        }
    }

    // parent直下の子オブジェクトを再帰的に取得する
    private static Transform[] GetChildrenRecursive(Transform parent, bool includeParent = true)
    {
        // 親を含む子オブジェクトを再帰的に取得
        // trueを指定しないと非アクティブなオブジェクトを取得できないことに注意
        var parentAndChildren = parent.GetComponentsInChildren<Transform>(true);

        if (includeParent)
        {
            // 親を含む場合はそのまま返す
            return parentAndChildren;
        }

        // 子オブジェクトの格納用配列作成
        var children = new Transform[parentAndChildren.Length - 1];

        // 親を除く子オブジェクトを結果にコピー
        Array.Copy(parentAndChildren, 1, children, 0, children.Length);

        // 子オブジェクトが再帰的に格納された配列
        return children;
    }

    public void DestroyEnemy()
    {
        Instantiate(enemyDeathEffect, this.transform.position, this.transform.rotation);

        for (int i = 0; i < gemAmount; i++)
        {
            Vector2 add = new Vector2 (0, 300);

            add.x = UnityEngine.Random.Range(-1.0f, 1.0f) * 80;

            GameObject gem = Instantiate(expGem, this.transform.position, Quaternion.identity);
            gem.GetComponent<Rigidbody2D>().AddForce(add);
        }
        gameManager.AddScore(scoreVolue);
        Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !attackCoolTime)
        {
            attackCoolTime = true;

            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            collision.gameObject.GetComponent<PlayerManager>().AttackToPlayer(atk);

            StartCoroutine(AttackCoolTime());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            DestroyEnemy();
        }
    }

    IEnumerator AttackCoolTime()
    {
        yield return new WaitForSeconds(2.0f); ;
        
        attackCoolTime = false;
    }
}
