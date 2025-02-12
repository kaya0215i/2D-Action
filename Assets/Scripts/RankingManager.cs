using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    //ランキング表示用テキストプレハブ
    [SerializeField] GameObject rankItemPrefab;

    //ランキングテキストの親のゲームオブジェクト
    [SerializeField] GameObject parentGameObject;

    public void LoadRanking(int stageId)
    {
        StartCoroutine(GetRanking(stageId));
    }

    public void UpdateRanking(string userName, int stageId, int score)
    {
        StartCoroutine(AddHighScore(userName, stageId, score));
    }

    IEnumerator GetRanking(int stageId)
    {
        UnityWebRequest request = UnityWebRequest.Get("https://functionappge202409.azurewebsites.net/api/scores/ranking/get?stageId=" + stageId);

        //リクエストヘッダを指定
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-functions-key", "YFwhJFJ10Wvq0HMpXD5O9fG1KIh0o9iRPkjeLecgeQwzAzFuTPvD_w==");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            List<Ranking> rankingList = JsonConvert.DeserializeObject<List<Ranking>>(json);

            foreach (Ranking ranking in rankingList)
            {
                GameObject textObject = Instantiate(
                    rankItemPrefab,
                    parentGameObject.transform.position,
                    Quaternion.identity,
                    parentGameObject.transform
                );
                textObject.GetComponent<Text>().text = ranking.GetRankText();
            }
        }
        else
        {
            Debug.Log("Error : GetHiscore UnityWebRequest Failed.");
        }
    }

    IEnumerator AddHighScore(string userName, int stageId, int score)
    {
        //ハイスコアのインスタンスを作成してjsonシリアライズ
        User user = new User(userName, stageId, score);
        string json = JsonConvert.SerializeObject(user);

        Debug.Log(json);

        UnityWebRequest request = UnityWebRequest.Post(
            "https://functionappge202409.azurewebsites.net/api/scores/add",
            json,
            "application/json"
        );

        request.SetRequestHeader("x-functions-key", "YFwhJFJ10Wvq0HMpXD5O9fG1KIh0o9iRPkjeLecgeQwzAzFuTPvD_w==");

        Debug.Log("ハイスコアを送信します");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error : AddHiscore UnityWebRequest Failed.");
        }
        else
        {
            Debug.Log("ハイスコア送信完了");
        }
    }
}


