using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Camera mainCanera;

    [SerializeField] GameObject rankingManager;

    public void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void StartButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void EndButton()
    {
        Application.Quit();
    }

    public void ControlKeyButton()
    {
        StartCoroutine(CameraControl1());
    }

    public void ShowRankingButton()
    {
        LoadRankingButton();

        StartCoroutine(CameraControl2());
    }

    public void LoadRankingButton()
    {
        foreach (Transform child in GameObject.Find("RankingList").transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        rankingManager.GetComponent<RankingManager>().LoadRanking(1);
    }

    public void ReturnKeyButton(int num)
    {
        switch (num)
        {
            case 1:
                StartCoroutine(CameraControl1());
                break;

            case 2:
                StartCoroutine(CameraControl2());
                break;
        }
    }

    IEnumerator CameraControl1()
    {
        Vector2 pos = mainCanera.transform.position;

        for (int i = 0; i < 60; i++)
        {
            pos.x += 0.2795f;
            mainCanera.transform.position = pos;
            yield return new WaitForSeconds(0.015f);
        }
    }

    

    IEnumerator CameraControl2()
    {
        Vector2 pos = mainCanera.transform.position;

        for (int i = 0; i < 60; i++)
        {
            pos.x -= 0.2795f;
            mainCanera.transform.position = pos;
            yield return new WaitForSeconds(0.015f);
        }
    }  
}
