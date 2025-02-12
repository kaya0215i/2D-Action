using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleArea : MonoBehaviour
{
    GameObject invisibleBlocks;

    private void Start()
    {
        invisibleBlocks = GameObject.Find("StageMapInvisible");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            invisibleBlocks.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            invisibleBlocks.SetActive(true);
        }
    }
}
