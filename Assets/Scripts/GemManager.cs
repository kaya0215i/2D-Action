using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemManager : MonoBehaviour
{
    [SerializeField] int expVolume = 0;

    Transform playerPos;

    private float lifeTime = 30;

    private void Start()
    {
        playerPos = GameObject.Find("Player").GetComponent<Transform>();

        Invoke("DestroyThisObject", lifeTime);
    }

    private void Update()
    {
        if (this.transform.position.x - 1.2f < playerPos.position.x && playerPos.position.x < this.transform.position.x + 1.2f &&
            this.transform.position.y - 1.2f < playerPos.position.y && playerPos.position.y < this.transform.position.y + 1.2f)
        {
            float step = 3 * Time.deltaTime;

            this.transform.position = Vector3.MoveTowards(this.transform.position, playerPos.position, step);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerManager>().exp += expVolume;

            Destroy(this.gameObject);
        }
    }

    private void DestroyThisObject()
    {
        Destroy(this.gameObject);
    }
}
