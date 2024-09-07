using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    #region 入力取得用変数
    [System.NonSerialized] public bool isInputUpPush;
    [System.NonSerialized] public bool isInputDownPush;
    [System.NonSerialized] public bool isInputDownHold;
    [System.NonSerialized] public bool isInputDownRelease;
    [System.NonSerialized] public bool isInputRightPush;
    [System.NonSerialized] public bool isInputRightHold;
    [System.NonSerialized] public bool isInputRightRelease;
    #endregion

    #region オブジェクト登録
    [SerializeField] private GameObject SkinDefault;
    [SerializeField] private GameObject SkinAttack;
    [SerializeField] private GameObject TriggerFoot;
    [SerializeField] private GameObject TriggerFront;
    [SerializeField] private GameObject EyeDefault_Right;
    [SerializeField] private GameObject EyeDefault_Left;
    [SerializeField] private GameObject EyeGameOver;

    [SerializeField] private ParticleSystem runningParticle;
    #endregion

    #region プレイヤーステート
    public enum PlayerState
    {
        Run,
        Jump,
        Squat,
        Attack,
        Pause,
        GameOver
    }

    [System.NonSerialized] public PlayerState currentPlayerState;
    [System.NonSerialized] public PlayerState previousPlayerState;
    private bool playerStateEnter = true;

    private void ChangePlayerState(PlayerState newPlayerState)
    {
        previousPlayerState = currentPlayerState;
        currentPlayerState = newPlayerState;
        if (previousPlayerState == PlayerState.Attack)
        {
            SkinDefault.SetActive(true);
            SkinAttack.SetActive(false);
            EyeDefault_Right.SetActive(true);
        }
        playerStateEnter = true;
    }
    #endregion

    private Rigidbody2D rb;
    private bool isTouchGround;
    private bool isHit;
    [System.NonSerialized] public bool isInTunnel;
    private bool isInCave;
    private List<GameObject> HitObjects = new List<GameObject>();
    private Vector2 jumpScale = new Vector2(0.9f, 1.1f);
    private Vector2 squatScale = new Vector2(1.4f, 0.5f);
    private Vector2 fallScale = new Vector2(1.2f, 0.8f);
    private Vector2 tunnelScale = new Vector2(1.3f, 0.6f);
    private Vector2 tunnelEyeScale = new Vector2(0.91f, 1.33f);
    [System.NonSerialized] public bool isGameOver;
    private float tunnelStart;
    private float tunnelEnd;
    private float caveStart;
    private float caveEnd;



    private void OnEnable()
    {
        #region 初期化
        currentPlayerState = PlayerState.Jump;
        gameObject.layer = 0;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.constraints = 
            RigidbodyConstraints2D.FreezePositionX |
            RigidbodyConstraints2D.FreezeRotation;
        isHit = false;
        isInTunnel = false;
        isGameOver = false;
        EyeDefault_Left.SetActive(true);
        EyeDefault_Right.SetActive(true);
        EyeGameOver.SetActive(false);
        transform.eulerAngles = Vector3.zero;
        #endregion
    }


    void Update()
    {
        #region 変数の取得
        isTouchGround = TriggerFoot.GetComponent<TriggerTouchCheck>().isTouch;
        isHit = TriggerFront.GetComponent<TriggerTouchCheck>().isTouch;
        HitObjects = TriggerFront.GetComponent<TriggerTouchCheck>().HitObjects;
        tunnelStart = TriggerFoot.GetComponent<TriggerTouchCheck>().tunnelStart;
        tunnelEnd = TriggerFoot.GetComponent<TriggerTouchCheck>().tunnelEnd;
        caveStart = TriggerFoot.GetComponent<TriggerTouchCheck>().caveStart;
        caveEnd = TriggerFoot.GetComponent <TriggerTouchCheck>().caveEnd;
        #endregion

        #region 入力の取得
        isInputUpPush = Input.GetKeyDown(KeyCode.UpArrow);
        isInputDownPush = Input.GetKeyDown(KeyCode.DownArrow);
        isInputDownHold = Input.GetKey(KeyCode.DownArrow);
        isInputDownRelease = Input.GetKeyUp(KeyCode.DownArrow);
        isInputRightPush = Input.GetKeyDown(KeyCode.RightArrow);
        isInputRightHold = Input.GetKey(KeyCode.RightArrow);
        isInputRightRelease = Input.GetKeyUp(KeyCode.RightArrow);
        #endregion

        isInTunnel = tunnelStart < transform.position.x + 0.5f && transform.position.x - 0.3 < tunnelEnd;
        isInCave = caveStart < transform.position.x && transform.position.x - 0.5f < caveEnd;

        switch (currentPlayerState)
        {
            case PlayerState.Run:
                if (playerStateEnter)
                {
                    playerStateEnter = false;
                    transform.localScale = Vector2.one;
                }

                if (isHit) ChangePlayerState(PlayerState.GameOver);
                else if (!isTouchGround) ChangePlayerState(PlayerState.Jump);
                else if (isInputUpPush) rb.velocity = Vector2.up * 30;
                else if (isInputDownPush) ChangePlayerState(PlayerState.Squat);
                else if (isInputRightPush) ChangePlayerState (PlayerState.Attack);
                break;



            case PlayerState.Squat:
                if (playerStateEnter)
                {
                    playerStateEnter = false;
                    transform.localScale = squatScale;
                }

                if (isHit) ChangePlayerState(PlayerState.GameOver);
                else if(!isTouchGround) ChangePlayerState(isInTunnel ? PlayerState.GameOver : PlayerState.Jump);
                else if (isInputUpPush) rb.velocity = Vector2.up * 20;
                else if (isInputRightPush) ChangePlayerState(isInTunnel ? PlayerState.GameOver : PlayerState.Attack);
                else if (isInputDownRelease) 
                    ChangePlayerState(isInTunnel ? PlayerState.GameOver : (isInputRightHold ? PlayerState.Attack : PlayerState.Run));
                break;



            case PlayerState.Jump:
                transform.localScale = isInputDownHold ? fallScale : jumpScale; 
                rb.gravityScale = isInputDownHold ? 13 : 10;

                if(isInCave && !isInputDownHold && transform.position.y > 1.65f)
                {
                    transform.localPosition = Vector2.up * 1.65f;
                    if (rb.velocity.y > 0) rb.velocity = Vector2.zero;
                }

                if (isInputDownPush && rb.velocity.y > -25) rb.velocity = Vector2.down * 25;

                if (isHit || transform.position.y < -10) ChangePlayerState(PlayerState.GameOver);
                else if (isTouchGround)
                {
                    rb.gravityScale = 10;
                    if (isInputDownHold) ChangePlayerState(PlayerState.Squat);
                    else if(isInputRightHold) ChangePlayerState(PlayerState.Attack);
                    else ChangePlayerState(PlayerState.Run);
                }
                break;



            case PlayerState.Attack:
                if (playerStateEnter)
                {
                    playerStateEnter = false;
                    transform.localScale = Vector3.one;
                    SkinDefault.SetActive(false);
                    SkinAttack.SetActive(true);
                    EyeDefault_Right.SetActive(false);
                }
                while (HitObjects.Count > 0 && isHit)
                {
                    if (HitObjects[0].tag == "Enemy")
                    {
                        Rigidbody2D HitRb = HitObjects[0].GetComponent<Rigidbody2D>();
                        HitRb.isKinematic = false;
                        HitRb.velocity = Vector2.up * 10;
                        HitObjects[0].GetComponent<BoxCollider2D>().enabled = false;
                    }
                    else
                    {
                        ChangePlayerState(PlayerState.GameOver);
                        break;
                    }
                }
                if (!isTouchGround) ChangePlayerState(PlayerState.Jump);
                else if (isInputUpPush) rb.velocity = Vector2.up * 30;
                else if (isInputDownPush) ChangePlayerState (PlayerState.Squat);
                else if (isInputRightRelease) ChangePlayerState(isInputDownHold ? PlayerState.Squat : PlayerState.Run);
                break;



            case PlayerState.Pause:
                if (GameManagerScript.Instance.currentPlayState != GameManagerScript.PlayState.Pause)
                {
                    if (isInputDownHold) ChangePlayerState(PlayerState.Squat);
                    else if (isInTunnel)
                    {
                        ChangePlayerState(PlayerState.GameOver);
                        previousPlayerState = PlayerState.Squat;
                    }
                    else if (isInputRightHold) ChangePlayerState(PlayerState.Attack);
                    else if (isTouchGround)ChangePlayerState(PlayerState.Run);
                    else ChangePlayerState(PlayerState.Jump);
                }
                break;
        }

        if(currentPlayerState != PlayerState.Pause && GameManagerScript.Instance.currentPlayState == GameManagerScript.PlayState.Pause)
            ChangePlayerState(PlayerState.Pause);

        if (currentPlayerState == PlayerState.GameOver && playerStateEnter)
        {
            playerStateEnter = false;
            transform.localScale = (previousPlayerState == PlayerState.Squat && isInTunnel) ? tunnelScale : Vector2.one;
            EyeGameOver.transform.localScale = (previousPlayerState == PlayerState.Squat && isInTunnel) ? tunnelEyeScale : Vector2.one;
            EyeDefault_Left.SetActive(false);
            EyeDefault_Right.SetActive(false);
            EyeGameOver.SetActive(true);

            if ((previousPlayerState != PlayerState.Squat || !isInTunnel) && HitObjects.Count > 0)
                transform.position = Vector3.up * transform.position.y + Vector3.right * (HitObjects[HitObjects.Count -1].transform.parent.position.x - 0.5f);
            }
        }
}
