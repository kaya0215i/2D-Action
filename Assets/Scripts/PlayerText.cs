using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerText : MonoBehaviour
{
    Transform playerTransform;

    Vector3 playerPos;

    float lifeTime = 0.5f;

    public string showText;
    public Color textColor;

    private void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();

        playerPos = playerTransform.position;
        playerPos.y += 0.7f;

        this.gameObject.GetComponent<Text>().text = showText;
        this.gameObject.GetComponent<Text>().color = textColor;

        Invoke("DestroyThisObject", lifeTime);
    }

    private void Update()
    {
        this.gameObject.transform.position = playerPos;
    }

    private void DestroyThisObject()
    {
        Destroy(this.gameObject);
    }
}
