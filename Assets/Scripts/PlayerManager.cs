using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] LayerMask blockLayer;
    [SerializeField] GameManager gameManager;

    Animator animator;

    [SerializeField] GameObject playerText;
    [SerializeField] GameObject UICanvas;

    [SerializeField] GameObject bolt1;

    Rigidbody2D myRigidbody;

    public enum DIRECTION_TYPE
    {
        STOP,
        RIGHT,
        LEFT,
    }

    DIRECTION_TYPE direction = DIRECTION_TYPE.STOP;

    float move;
    float jumpPowr = 350;
    float inputHorizontal = 0.0f;
    
    bool isDeath = false;
    public bool isAttack = false;
    bool isDamage = false;
    bool isDamageAnim = false;
    bool isDash = false;
    public bool isBolt = false;

    int nDirection = 1;

    public int level;
    public float exp;
    public float maxHp;
    public float hp;
    public float hpBuff;
    public float atk;
    public float atkBuff;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        level = 1;
        exp = 0;
        hpBuff = 0;
        atkBuff = 0;
        StatusAdjustment();
        hp = maxHp;
    }

    private void Update()
    {
        if (isDeath)
        {
            return;
        }

        inputHorizontal = 0.0f;

        if (!isAttack)
        {
            inputHorizontal = Input.GetAxis("Horizontal");
            inputHorizontal = Mathf.Clamp(inputHorizontal, -0.1f, 0.1f);

            animator.SetFloat("speed", Mathf.Abs(inputHorizontal));

            //ジャンプ
            //地面についていたら
            if (IsGround())
            {
                animator.SetBool("IsJumping", false);
                isDamageAnim = false;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump();
                }
            }
            //空中にいたら
            else
            {
                if (!isDamageAnim && !isDash)
                {
                    animator.SetBool("IsJumping", true);
                }
                else
                {
                    animator.SetBool("IsJumping", false);
                }
            }

            //ダッシュ
            if (Input.GetKeyDown(KeyCode.F) && !isDash)
            {
                StartCoroutine(Dash());
            }

            //攻撃
            //弱
            if (Input.GetMouseButtonDown(0))
            {
                WeakAttack();
            }
            //強
            else if (Input.GetMouseButtonDown(1))
            {
                StrongAttack();
            }
        }

        //止まっている
        if (inputHorizontal == 0)
        {
            direction = DIRECTION_TYPE.STOP;
        }
        //右
        else if (inputHorizontal > 0)
        {
            direction = DIRECTION_TYPE.RIGHT;
            transform.localScale = new Vector3(1, 1, 1);
        }
        //左
        else if (inputHorizontal < 0)
        {
            direction = DIRECTION_TYPE.LEFT;
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //レベルアップ
        if (exp >= 100 * Mathf.Pow(1.15f, level - 1))
        {
            exp -= 100 * Mathf.Pow(1.15f, level - 1);
            level++;

            StatusAdjustment();
            hp += maxHp * 0.5f;

            if(hp > maxHp)
            {
                hp = maxHp;
            }

            GameObject text= Instantiate(playerText, this.transform.position, Quaternion.identity, UICanvas.transform);
            text.GetComponent<PlayerText>().showText = "Level Up!";
            text.GetComponent<PlayerText>().textColor = Color.yellow;
        }

        //死亡
        if(hp == 0)
        {
            PlayerDeath();
        }
    }

    private void FixedUpdate()
    {
        if (isDeath)
        {
            return;
        }

        switch (direction)
        {
            case DIRECTION_TYPE.STOP:
                move = 0;
                break;

            case DIRECTION_TYPE.RIGHT:
                move = 3;
                nDirection = 1;
                break;

            case DIRECTION_TYPE.LEFT:
                move = -3;
                nDirection = -1;
                break;
        }

        myRigidbody.velocity = new Vector2(move, myRigidbody.velocity.y);

        if(isAttack)
        {
            myRigidbody.velocity = new Vector2(0, 0);
            myRigidbody.gravityScale = 1;
        }
        else if (!isAttack)
        {   
            myRigidbody.gravityScale = 1.5f;
        }
    }

    public void StatusAdjustment()
    {
        maxHp = (50 * Mathf.Pow(1.25f, level - 1)) + hpBuff;
        atk = (10 * Mathf.Pow(1.25f, level - 1)) + atkBuff;
    }

    void Jump()
    {
        myRigidbody.AddForce(Vector2.up * jumpPowr);
    }

    IEnumerator Dash()
    {
        isDash = true;

        animator.SetBool("IsDash", true);

        GameObject.Find("DashCool").GetComponent<AbilityCoolDown>().StartCoolDownTime();

        this.gameObject.layer = 9;

        myRigidbody.velocity = new Vector2(0, 0);
        myRigidbody.MovePosition(new Vector2(this.transform.position.x + (nDirection * 3.0f), this.transform.position.y));

        Invoke("LayerChange", 0.21f);

        yield return new WaitForSeconds(1.5f);

        isDash = false;
        GameObject.Find("DashCool").GetComponent<AbilityCoolDown>().EndCoolDownTime();
    }

    public void LayerChange()
    {
        this.gameObject.layer = 3;
        animator.SetBool("IsDash", false);
    }

    void WeakAttack()
    {
        isAttack = true;

        if(isBolt)
        {
            Vector2 myPos = this.transform.position;

            myPos.x += 0.5f * nDirection;
            myPos.y += -0.2f;

            GameObject bolt1Config = Instantiate(bolt1, myPos, Quaternion.identity);

            bolt1Config.GetComponent<Bolt1Manager>().scale = 0.75f;
            bolt1Config.GetComponent<Bolt1Manager>().targetType = Bolt1Manager.TARGET_TYPE.ENEMY;
            bolt1Config.GetComponent<Bolt1Manager>().boltType = Bolt1Manager.BOLT_TYPE.NORMAL;
            bolt1Config.GetComponent<Bolt1Manager>().speed = 3;
            bolt1Config.GetComponent<Bolt1Manager>().atk = atk * 0.75f;
        }

        GameObject.Find("Attack").GetComponent<PlayerAttack>().StartAttackCollider(PlayerAttack.ATTACK_TYPE.WEAK_ATTACK);

        animator.SetBool("IsJumping", false);
        animator.SetTrigger("WeakAttack");
    }

    void StrongAttack()
    {
        isAttack = true;

        GameObject.Find("Attack").GetComponent<PlayerAttack>().StartAttackCollider(PlayerAttack.ATTACK_TYPE.STRONG_ATTACK);

        animator.SetBool("IsJumping", false);
        animator.SetTrigger("StrongAttack");
    }

    public void FalseIsAttack()
    {
        isAttack = false;
        GameObject.Find("Attack").GetComponent<PlayerAttack>().EndAttackCollider();
    }

    bool IsGround()
    {
        Vector3 startPoint = transform.position + new Vector3(nDirection * 0.0f, -5f, 0) * 0.1f;
        Vector3 leftEndPoint = transform.position + new Vector3(nDirection * -1.5f, -8f, 0) * 0.1f;
        Vector3 rightEndPoint = transform.position + new Vector3(nDirection * 1.5f, -8f, 0) * 0.1f;


        Debug.DrawLine(startPoint, leftEndPoint);
        Debug.DrawLine(startPoint, rightEndPoint);

        return Physics2D.Linecast(startPoint, leftEndPoint, blockLayer) ||
               Physics2D.Linecast(startPoint, rightEndPoint, blockLayer);
    }

    bool IsWall()
    {
        Vector3 startPoint = transform.position + new Vector3(0, -2.3f, 0) * 0.1f;
        Vector3 endPoint = transform.position;

        switch (direction)
        {
            case DIRECTION_TYPE.RIGHT:
                endPoint = startPoint + new Vector3(0.25f, 0.0f, 0.0f);
                break;

            case DIRECTION_TYPE.LEFT:
                endPoint = startPoint + new Vector3(-0.25f, 0.0f, 0.0f);
                break;
        }

        Debug.DrawLine(startPoint, endPoint);

        return Physics2D.Linecast(startPoint, endPoint, blockLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDeath)
        {
            return;
        }

        //トラップ接触判定
        if (collision.gameObject.CompareTag("Trap"))
        {
            AttackToPlayer(hp * 0.7f);
        }

        if (collision.gameObject.CompareTag("OutZone"))
        {
            PlayerDeath();
        }

        //クリア判定
        if (collision.gameObject.CompareTag("Finish"))
        {
            gameManager.GameClear();
        }

        //アイテム取得判定
        if (collision.gameObject.CompareTag("Item"))
        {
            //audioSource.PlayOneShot(enemyKill);
            collision.GetComponent<ItemManager>().GetItem();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //トラップ接触判定
        if (collision.gameObject.CompareTag("TriggerTrap"))
        {
            AttackToPlayer(hp * 0.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //接敵判定
        if (collision.gameObject.CompareTag("Enemy"))
        {
            
        }
    }

    public void AttackToPlayer(float damageVolume)
    {
        if (isDeath)
        {
            return;
        }

        if (isDamage)
        {
            return;
        }

        FalseIsAttack();

        animator.SetBool("IsJumping", false);

        isDamage =true;
        isDamageAnim = true;

        animator.SetTrigger("AttackHit");

        hp -= damageVolume;

        if (hp < 0)
        {
            hp = 0;
        }

        StartCoroutine(InvincibleTime());
    }

    IEnumerator InvincibleTime()
    {
        yield return new WaitForSeconds(0.5f);
        isDamage = false;
    }

    public void PlayerDeath()
    {
        StopAllCoroutines();

        isDeath = true;

        animator.SetBool("IsJumping", false);
        animator.SetBool("IsDash", false);
        animator.SetBool("IsDead", true);

        animator.Play("Death NoEffect");

        myRigidbody.velocity = new Vector2(0, 0);

        gameManager.GameOver();
    }
}
