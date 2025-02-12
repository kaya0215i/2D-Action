using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    public string userName = "";
    public int stageId = 0;
    public int score = 0;

    public User(string userName, int stageId, int score)
    {
        this.userName = userName;
        this.stageId = stageId;
        this.score = score;
    }
}
