using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchBodyType : MonoBehaviour
{
    Rigidbody2D rb;

    private void Start()
    {
        rb = this.gameObject.transform.parent.GetComponent<Rigidbody2D>();
    }

    // 領域の中にプレイヤーが入ってきたら、オブジェクトのボディタイプを変更
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rb.gravityScale = 3;
        }
    }
}
