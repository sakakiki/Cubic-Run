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
        #region �W�����v����
        if (!isJump)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                rb.linearVelocity += Vector2.up * (Input.GetKey(KeyCode.DownArrow) ? 20 : 30);
                isJump = true;
            }
        }
        #endregion

        #region ���Ⴊ�݁E�}�~������
        // �J�[�\���L�[���̉��������m
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.localScale = new Vector2(1, 0.5f);

            if (isJump)
            {
                rb.linearVelocity += Vector2.down * 30;
            }
        }

        // �J�[�\���L�[���̗�������m
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            transform.localScale = Vector2.one;
        }
        #endregion

        #region �U���i�ό`�j����
        SkinDefault.SetActive(!Input.GetKey(KeyCode.RightArrow));
        SkinAttack.SetActive(Input.GetKey(KeyCode.RightArrow));
        #endregion
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �W�����v���������
        isJump = false;
    }
}
