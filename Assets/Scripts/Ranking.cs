using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranking
{
    public long ranking = 0;
    public int userId = 0;
    public string userName = "";
    public int stageId = 0;
    public int score = 0;

    public Ranking() { }

    public string GetRankText()
    {
        return $"{this.ranking}ˆÊ {this.userName}\n     {this.score}“_";
    }
}
