using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkinSelecter : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Transform wheelTf;
    [SerializeField] private SpriteRenderer[] panels_sprite;
    [SerializeField] private Transform[] panels_tf;
    private float wheelEulerAnglesX;
    private bool isDragged;
    private float angleVelocityX;
    private float currentPointY;
    private float currentTime;
    private Queue<float> pointQueue = new Queue<float>();
    private Queue<float> timeQueue = new Queue<float>();
    private bool isSnapping;
    private float targetAngleX;
    [SerializeField] private int frontSkinID;
    public bool isStop;
    public bool isActive;



    public void OnEnable()
    {
        //フラグの初期化
        isStop = false;
        isActive = false;
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        //非アクティブ状態なら何もしない
        if (!isActive) return;

        //回転状態に変化
        isDragged = true;

        //現在値の記憶とQueueへの格納
        currentPointY = eventData.position.y;
        currentTime = Time.time;
        pointQueue.Enqueue(currentPointY);
        timeQueue.Enqueue(currentTime);
    }



    public void OnDrag(PointerEventData eventData)
    {
        //非アクティブ状態なら何もしない
        if (!isActive) return;

        //停止状態を解除
        isStop = false;

        //タップ位置
        float newScreenPointY = eventData.position.y;

        //ホイールの回転
        wheelEulerAnglesX += (newScreenPointY - currentPointY) / Screen.height * 160;
        wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;

        //現在値の記憶とQueueへの格納
        currentPointY = newScreenPointY;
        currentTime = Time.time;
        pointQueue.Enqueue(currentPointY);
        timeQueue.Enqueue(currentTime);

        //Queue内不要データ削除
        while (currentTime - timeQueue.Peek() > 0.1)
        {
            pointQueue.Dequeue();
            timeQueue.Dequeue();
        }
    }



    public void OnPointerUp(PointerEventData eventData)
    {
        //非アクティブ状態なら何もしない
        if (!isActive) return;

        //回転状態を終了
        isDragged = false;

        //スナップ状態解除
        isSnapping = false;

        //速度付与
        if (timeQueue.Count >= 2)
            angleVelocityX = (currentPointY - pointQueue.Dequeue()) / (currentTime - timeQueue.Dequeue()) / Screen.height * 64;
        else angleVelocityX = 0;
        if (angleVelocityX > 1800) 
            angleVelocityX = 1800;

        //Queue内データ削除
        timeQueue.Clear();
        pointQueue.Clear();
    }



    private void Update()
    {
        //回転中または非アクティブ状態なら
        if (!isStop || !isActive)
        {
            //パネルの透明度・スケール変更
            for (int ID = 0; ID < panels_sprite.Length; ID++)
            {
                //事前計算
                //パネルの角度をベースにホイールのワールド座標で補正をかける
                float temp = (1 - panels_tf[ID].forward.z + (wheelTf.position.z - 90) / 25) * 5;

                //透明度変更
                panels_sprite[ID].color = Color.white - Color.black * temp;

                //スケール変更
                panels_tf[ID].localScale = Vector3.one * (1 - temp / 5);
            }
        }

        //非アクティブ状態なら残りの処理を飛ばす
        if (!isActive) return;

        //回転量補正
        while (wheelEulerAnglesX > 360)
            wheelEulerAnglesX -= 360;
        while (wheelEulerAnglesX < 0)
            wheelEulerAnglesX += 360;

        //ホイールが現在のスキンパネルの位置の外まで回転したら
        while (Mathf.Abs(wheelEulerAnglesX - 22.5f * frontSkinID) > 11.25)
        {
            //フロントのスキンIDを更新
            frontSkinID += (int)Mathf.Sign(wheelEulerAnglesX - 22.5f * frontSkinID);

            //音を鳴らす処理

            /* ここでパネルの更新処理をすればスキンを増やせる（他の処理の変更も必要） */
        }
        //スキンIDを補正
        if (frontSkinID < 0) frontSkinID += 16;
        else if (frontSkinID > 15) frontSkinID -= 16;

        //ドラッグ中は残りの処理を飛ばす
        if (isDragged) return;

        //減速しながら回転
        if (Mathf.Abs(angleVelocityX) > 15)
        {
            //処理落ちしていたら回転を停止（逆回転防止）
            if (Time.deltaTime * 2 > 1)
                angleVelocityX = 0;

            //回転を減速
            angleVelocityX *= (1 - Time.deltaTime * 2);

            //ホイールを回転
            wheelEulerAnglesX += angleVelocityX * Time.deltaTime;
            wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;
        }

        //スナップ処理
        else
        {
            //開始処理
            if (!isSnapping)
            {
                //スナップ動作開始
                isSnapping = true;

                //回転方向に補正をかけつつ目標回転量を算出
                targetAngleX = Mathf.Floor(wheelEulerAnglesX / 22.5f) * 22.5f;
                if (wheelEulerAnglesX + angleVelocityX/5 - targetAngleX > 11.25f)
                    targetAngleX += 22.5f;

                //回転を停止
                angleVelocityX = 0;
            }

            //回転が必要なら
            if (Mathf.Abs(wheelEulerAnglesX - targetAngleX) > 1)
            {
                //補間回転
                wheelEulerAnglesX = Mathf.Lerp(wheelEulerAnglesX, targetAngleX, Time.deltaTime * 10);
                wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;
            }

            //停止処理
            else if (!isStop)
            {
                //停止状態に
                isStop = true;

                //スキン変更処理
                GameManager.Instance.ChangePlayerSkin(frontSkinID);

                //パネルのスケール変更
                panels_tf[frontSkinID].localScale = Vector3.one * 1.03f;
            }
        }
    }



    //使用しているスキンにセレクターの設定を合わせる
    public void SetWheelAngle(int usingSkinID)
    {
        //スキンIDを格納
        frontSkinID = usingSkinID;

        //セレクターを回転
        wheelEulerAnglesX = 22.5f * usingSkinID;
        wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;
    }
}
