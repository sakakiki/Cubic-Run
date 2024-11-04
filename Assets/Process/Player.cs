using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isJump;
    public GameObject SkinDefault;
    public GameObject SkinAttack;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        #region ジャンプ処理
        if (!isJump)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                rb.velocity += Vector2.up * (Input.GetKey(KeyCode.DownArrow) ? 20 : 30);
                isJump = true;
            }
        }
        #endregion

        #region しゃがみ・急降下処理
        // カーソルキー下の押下を検知
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.localScale = new Vector2(1, 0.5f);

            if (isJump)
            {
                rb.velocity += Vector2.down * 30;
            }
        }

        // カーソルキー下の離上を検知
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            transform.localScale = Vector2.one;
        }
        #endregion

        #region 攻撃（変形）処理
        SkinDefault.SetActive(!Input.GetKey(KeyCode.RightArrow));
        SkinAttack.SetActive(Input.GetKey(KeyCode.RightArrow));
        #endregion
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ジャンプ中判定解除
        isJump = false;
    }
}
