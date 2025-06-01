using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Model_Dragged : PlayerStateBase_Model
{
    private Camera mainCam;
    private Transform mainCamTf;
    private Vector3 startScreenPoint;
    private Vector3 touchOffset;
    private float currentTime;
    private Vector2 currentPos;
    private Queue<float> timeQueue = new Queue<float>();
    private Queue<Vector2> posQueue = new Queue<Vector2>();
    private float startAngle;
    private Transform playerCenterTf;
    private Vector2 centerOffset;

    public PlayerState_Model_Dragged(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        mainCam = Camera.main;
        mainCamTf = mainCam.transform;
        playerCenterTf = playerCon.centerTf;
    }

    public override void Enter()
    {
        base.Enter();

        //物理演算を無効化
        rb.isKinematic = true;

        //運動を停止
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        //回転量を記憶
        startAngle = tf.eulerAngles.z;

        //スケール変更
        tf.localScale = Vector3.one * 1.6f;

        //目を開ける
        eyeTf.localScale = Vector3.one;
        eyeCloseTimer = 5;

        //驚いた目に
        playerCon.eyeDragged.SetActive(true);

        //ドラッグ開始時の位置・タッチ位置のズレを記憶
        startScreenPoint = mainCam.WorldToScreenPoint(tf.position);
        touchOffset = tf.position - mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, startScreenPoint.z));

        //プレイヤーの座標と中心の位置のズレを記憶
        centerOffset = playerCenterTf.position - tf.position;

        //SE再生
        AudioManager.Instance.PlaySE(AudioManager.SE.Player_Hold);
    }

    public override void Update()
    {
        //経過時間加算
        elapsedTime += Time.deltaTime;

        //タッチ（マウス）の座標をもとに移動先の位置を計算
        Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, startScreenPoint.z);
        Vector3 newPosition = mainCam.ScreenToWorldPoint(currentScreenPoint) + touchOffset;

        //画面外に出た場合は補正
        if (newPosition.y + centerOffset.y - tf.localScale.y / 2 < mainCamTf.position.y) 
            newPosition.y = mainCamTf.position.y - centerOffset.y + tf.localScale.y / 2;
        if (newPosition.x + centerOffset.x - tf.localScale.x / 2 < -5)
            newPosition.x = -5 - centerOffset.x + tf.localScale.x / 2;
        else if (newPosition.x + centerOffset.x + tf.localScale.x / 2 > 15)
            newPosition.x = 15 - centerOffset.x - tf.localScale.x / 2;

        //現在値の記憶とQueueへの格納
        currentTime = Time.time;
        currentPos = newPosition;
        timeQueue.Enqueue(currentTime);
        posQueue.Enqueue(currentPos);

        //位置と回転を変更
        rb.position = newPosition;
        rb.SetRotation(Mathf.Sin(Mathf.PI * elapsedTime * 4) * 2 + startAngle);

        //Queue内不要データ削除
        while (Time.time - timeQueue.Peek() > 0.1)
        {
            timeQueue.Dequeue();
            posQueue.Dequeue();
        }

        //ゲームステートが遷移したならステート遷移
        if (gameStateMachine.currentState == gameStateMachine.state_MenuToPlay ||
            gameStateMachine.currentState == gameStateMachine.state_MenuToTutorial ||
            gameStateMachine.currentState == gameStateMachine.state_ResultToPlay)
            stateMachine.ChangeState(stateMachine.state_Model_MenuToPlay);
        else if (gameStateMachine.currentState == gameStateMachine.state_MenuToSkin)
            stateMachine.ChangeState(stateMachine.state_Model_MenuToSkin);
    }

    public override void Exit()
    {
        //スケール変更
        tf.localScale = Vector3.one * 1.5f;

        //驚いた目を戻す
        playerCon.eyeDragged.SetActive(false);

        //物理演算の有効化
        rb.isKinematic = false; 

        //回転を有効化
        rb.freezeRotation = false;

        //速度付与
        rb.velocity = (currentPos - posQueue.Dequeue()) / (currentTime - timeQueue.Dequeue()) / 2;

        //Queue内データ削除
        timeQueue.Clear();
        posQueue.Clear();
    }
}
