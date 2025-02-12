using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] GameObject rankingManager;

    [SerializeField] InputField nameInputField;

    [SerializeField] Text resultScoreText;

    [SerializeField] GameObject postButton;
    [SerializeField] GameObject postedText;

    private float colorR;
    private float colorG;
    private float colorB;

    private float time;

    private void Start()
    {
        resultScoreText.text = "Score:" + GameManager.nScore;
    }

    void Update()
    {
        time += Time.deltaTime;

        if (time >= 0.1)
        {
            time = 0;

            colorR = Random.Range(0.0f, 1.0f);
            colorG = Random.Range(0.0f, 1.0f);
            colorB = Random.Range(0.0f, 1.0f);

            resultScoreText.color = new Color(colorR, colorG, colorB, 1.0f);
        }
    }

    public void GoTitleButton()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void SetUserNameAndUpdateScore()
    {
        postButton.SetActive(false);
        postedText.SetActive(true);

        rankingManager.GetComponent<RankingManager>().UpdateRanking(nameInputField.text, 1, GameManager.nScore);
    }
}

