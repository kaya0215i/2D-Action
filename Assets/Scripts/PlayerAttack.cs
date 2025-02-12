using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    BoxCollider2D myBoxCollider;
    Transform myTransform;

    public enum ATTACK_TYPE
    {
        NONE,
        WEAK_ATTACK,
        STRONG_ATTACK,
    }

    ATTACK_TYPE attackType = ATTACK_TYPE.NONE;

    private void Start()
    {
        myBoxCollider = this.gameObject.GetComponent<BoxCollider2D>();
        myTransform = this.gameObject.transform;
        ResetAll();
    }

    private void FixedUpdate()
    {
        Vector3 angle = Vector3.zero;
        Vector2 colliderOffSet = Vector2.zero;
        Vector2 colliderSize = Vector2.zero;

        switch (attackType)
        {
            case ATTACK_TYPE.WEAK_ATTACK:
                //開始角度(0, 0, 77.0f);
                //終了角度(0, 0, -94.0f);

                //コライダーの開始オフセット(0.2640479f, 0.3873976f);
                //コライダーの開始サイズ(0.2483976f, 0.8040488f);

                //コライダーの終了オフセット(0.2640479f, 0.4867948f);
                //コライダーの終了サイズ(0.2483976f, 1.002843f);

                angle.z = -171.0f / 0.3f * Time.fixedDeltaTime;
                colliderOffSet.y = 0.0993972f / 0.3f * Time.fixedDeltaTime;
                colliderSize.y = 0.1987942f / 0.3f * Time.fixedDeltaTime;

                myTransform.localEulerAngles += angle;
                myBoxCollider.offset += colliderOffSet;
                myBoxCollider.size += colliderSize;

                if(myBoxCollider.size.y > 1.0f)
                {
                    attackType = ATTACK_TYPE.NONE;
                    ResetAll();
                }

                break;

            case ATTACK_TYPE.STRONG_ATTACK:
                //開始角度(0, 0, 125.0f);
                //終了角度(0, 0, -112.0f);

                //コライダーの開始オフセット(0.09725648f, 0.5769879f);
                //コライダーの開始サイズ(0.2042446f, 0.5715969f);

                //コライダーの終了オフセット(0.2471149f, 0.6228468f);
                //コライダーの終了サイズ(0.2366455f, 0.7550337f);

                angle.z = -237.0f / 0.2f * Time.fixedDeltaTime;
                colliderOffSet.x = 0.14985842f / 0.2f * Time.fixedDeltaTime;
                colliderOffSet.y = 0.0458589f / 0.2f * Time.fixedDeltaTime;
                colliderSize.x = 0.0324009f / 0.2f * Time.fixedDeltaTime;
                colliderSize.y = 0.1834368f / 0.2f * Time.fixedDeltaTime;

                myTransform.localEulerAngles += angle;
                myBoxCollider.offset += colliderOffSet;
                myBoxCollider.size += colliderSize;

                if (myBoxCollider.size.y > 0.755f)
                {
                    attackType = ATTACK_TYPE.NONE;
                    ResetAll();
                }

                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //攻撃判定
        if (collision.gameObject.CompareTag("Enemy"))
        {
            switch (attackType)
            {
                case ATTACK_TYPE.WEAK_ATTACK:
                    collision.GetComponent<EnemyInfo>().AttackToEnemy(this.gameObject.transform.parent.GetComponent<PlayerManager>().atk);

                    break;

                case ATTACK_TYPE.STRONG_ATTACK:
                    collision.GetComponent<EnemyInfo>().AttackToEnemy(this.gameObject.transform.parent.GetComponent<PlayerManager>().atk * 2.3f);

                    break;
            }
        }
    }

    public void StartAttackCollider(ATTACK_TYPE type)
    {
        switch (type)
        {
            case ATTACK_TYPE.WEAK_ATTACK:
                myTransform.localEulerAngles = new Vector3(0, 0, 77.0f);
                myBoxCollider.offset = new Vector2(0.2640479f, 0.3873976f);
                myBoxCollider.size = new Vector2(0.2483976f, 0.8040488f);

                attackType = type;

                break;

            case ATTACK_TYPE.STRONG_ATTACK:
                myTransform.localEulerAngles = new Vector3(0, 0, 125.0f);
                myBoxCollider.offset = new Vector2(0.09725648f, 0.5769879f);
                myBoxCollider.size = new Vector2(0.2042446f, 0.5715969f);

                StartCoroutine(ChangeStrongAttack(ATTACK_TYPE.STRONG_ATTACK));

                break;
        }
    }

    public void EndAttackCollider()
    {
        attackType = ATTACK_TYPE.NONE;
    }

    IEnumerator ChangeStrongAttack(ATTACK_TYPE type)
    {
        yield return new WaitForSeconds(0.35f);

        attackType = type;
    }

    private void ResetAll()
    {
        myTransform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        myBoxCollider.offset = new Vector2(0.0f, 0.0f);
        myBoxCollider.size = new Vector2(0.0f, 0.0f);
    }
}
