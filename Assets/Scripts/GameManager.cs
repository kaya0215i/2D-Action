using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverText;
    [SerializeField] GameObject gameClearText;

    [SerializeField] Text scoreText;
    [SerializeField] Text levelText;
    [SerializeField] Text hpText;

    [SerializeField] GameObject expBar;
    [SerializeField] GameObject hpBar;

    AudioSource audioSource;

    PlayerManager playerManager;

    static public int nScore;

    private void Start()
    {
        nScore = 0;

        audioSource = GetComponent<AudioSource>();

        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();

        scoreText.text = "Score:" + nScore;
    }

    private void Update()
    {
        //経験値バー管理
        //スケール
        Vector3 expBarScale;

        expBarScale.x = playerManager.exp * (100 / (100 * Mathf.Pow(1.15f, playerManager.level - 1)));
        expBarScale.y = 100.0f;
        expBarScale.z = 1.0f;

        expBar.transform.localScale = expBarScale;

        //ポジション
        Vector3 expBarPos;

        expBarPos.x = (100.0f - expBarScale.x) * -9.54f;
        expBarPos.y = 505.0f;
        expBarPos.z = -9002.0f;

        expBar.transform.localPosition = expBarPos;

        //レベルテキスト
        levelText.text = "Lv." + playerManager.level;

        //プレイヤーHPバー管理
        //スケール
        Vector3 hpBarScale;

        hpBarScale.x = playerManager.hp * (50 / playerManager.maxHp);
        hpBarScale.y = 70.0f;
        hpBarScale.z = 1.0f;

        hpBar.transform.localScale = hpBarScale;

        //ポジション
        Vector3 hpBarPos;

        hpBarPos.x = (100.0f - (hpBarScale.x * 2)) * -4.7721f - 406;
        hpBarPos.y = 446.0f;
        hpBarPos.z = -9002.0f;

        hpBar.transform.localPosition = hpBarPos;

        hpText.text = "" + playerManager.hp + "\n" + playerManager.atk;
    }

    public void AddScore(int value)
    {
        nScore += value;

        scoreText.text = "Score:" + nScore;
    }

    public void GameOver()
    {
        gameOverText.SetActive(true);

        //audioSource.PlayOneShot(gameOverSE);

        Invoke("ResultScene", 1.5f);
    }

    public void GameClear()
    {
        gameClearText.SetActive(true);

        //audioSource.PlayOneShot(gameClearSE);

        Invoke("ResultScene", 1.5f);
    }

    void ResultScene()
    {
        SceneManager.LoadScene("ResultScene");
    }
}
