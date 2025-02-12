using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] float speed;

    [SerializeField] float topMove;
    [SerializeField] float underMove;

    /// <summary>
    /// 一定量上下する床
    /// </summary>
    public enum DIRECTION_TYPE
    {
        STOP,
        TOP,
        BOTTOM,
    }

    [SerializeField] DIRECTION_TYPE direction;

    Vector3 startPosition; // 開始時点のオブジェクトの位置

    void Start()
    {
        // 最初の移動方向
        //direction = DIRECTION_TYPE.TOP;

        // 開始時点のオブジェクトの位置を記憶
        startPosition = transform.position;

        // 移動させる
        Move();
    }

    /// <summary>
    /// 特定の方向に移動する
    /// </summary>
    void Move()
    {
        float goalY = 0.0f; // 目的地点のy座標
        if (direction == DIRECTION_TYPE.TOP)
        {
            goalY = topMove;
            direction = DIRECTION_TYPE.BOTTOM; // 方向を切り替え
        }
        else if (direction == DIRECTION_TYPE.BOTTOM)
        {
            goalY = underMove;
            direction = DIRECTION_TYPE.TOP; // 方向を切り替え
        }

        // 目的地を設定
        Vector3 goalPosition = startPosition + new Vector3(0.0f, goalY, 0.0f);

        // 目的地まで3.0f秒かけて移動
        //this.transform.DOMove(goalPosition, 3.0f);

        // 目的地まで3.0f秒かけて移動（最初と最後をゆっくり移動）
        //this.transform.DOMove(goalPosition, 3.0f).SetEase(Ease.InOutQuad);

        // 目的地まで3.0f秒かけて移動。移動完了後、Moveメソッドを呼ぶ
        this.transform.DOMove(goalPosition, speed).SetEase(Ease.InOutQuad).OnComplete(Move);
    }
}