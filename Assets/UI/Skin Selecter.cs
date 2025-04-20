using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SkinSelecter : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private GameManager GM;
    [SerializeField] private InputManager IM;
    [SerializeField] private AudioManager AM;
    [SerializeField] private SkinDataBase skinDataBase;
    [SerializeField] private Transform wheelTf;
    [SerializeField] private SpriteRenderer[] panels_sprite;
    [SerializeField] private Transform[] panels_tf;
    [SerializeField] private TextMeshProUGUI[] panels_skinName;
    [SerializeField] private SpriteRenderer[] panels_skinModel;
    [SerializeField] private SpriteRenderer[] panels_skinEyes_L;
    [SerializeField] private SpriteRenderer[] panels_skinEyes_R;
    [SerializeField] private TextMeshProUGUI[] panels_skinModelText;
    [SerializeField] private SpriteRenderer[] panels_lockPanel;
    [SerializeField] private TextMeshProUGUI[] panels_lockText;
    [SerializeField] GameObject buttonOK;
    private float wheelEulerAnglesX;
    private bool isDragged;
    private float angleVelocityX;
    private float currentPointY;
    private float currentTime;
    private Queue<float> pointQueue = new Queue<float>();
    private Queue<float> timeQueue = new Queue<float>();
    private bool isSnapping;
    private float targetAngleX;
    private int frontSkinID;
    public bool isStop;
    public bool isActive;






    public void OnEnable()
    {
        //フラグの初期化
        isStop = false;
        isActive = false;

        //OKボタンの有効化
        buttonOK.SetActive(true);
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        //非アクティブ状態なら何もしない
        if (!isActive) return;

        //回転状態に変化
        isDragged = true;

        //ボタンの無効化
        IM.InputUISetActive_Skin(false);

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

        //非アクティブ状態の間にドラッグが開始されているなら開始処理の実行
        if (!isDragged)
        {
            //回転状態に変化
            isDragged = true;

            //ボタンの無効化
            IM.InputUISetActive_Skin(false);

            //現在値の記憶とQueueへの格納
            currentPointY = eventData.position.y;
            currentTime = Time.time;
            pointQueue.Enqueue(currentPointY);
            timeQueue.Enqueue(currentTime);
        }

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
            for (int ID = frontSkinID - 2; ID <= frontSkinID + 2; ID++)
            {
                int panelID = ID;

                //スキンIDを補正
                if (panelID < 0) panelID += 16;
                else if (panelID > 15) panelID -= 16;

                //事前計算
                //パネルの角度をベースにホイールのワールド座標で補正をかける
                float temp = (1 - panels_tf[panelID].forward.z + (wheelTf.position.z - 90) / 25) * 5;

                //透明度変更
                Color tempColor_white = Color.white - Color.black * temp;
                Color tempColor_black = Color.black - Color.black * temp;
                panels_sprite[panelID].color = tempColor_white;
                panels_skinName[panelID].color = tempColor_black;
                if (GM.isSkinUnlocked[panelID] || panelID % 8 != 7)
                {
                    panels_skinModel[panelID].color = skinDataBase.skinData[panelID].skinColor - Color.black * temp;
                    panels_skinEyes_L[panelID].color = tempColor_white;
                    panels_skinEyes_R[panelID].color = tempColor_white;
                }
                //ロック中のCrystalスキンのみ例外処理
                else
                {
                    panels_skinModel[panelID].color = tempColor_black;
                    panels_skinModelText[panelID/8].color = tempColor_white * 0.5f;
                }
                //ロック中のパネルの処理
                if (!GM.isSkinUnlocked[panelID])
                {
                    panels_lockPanel[panelID].color = tempColor_black * 0.8f;
                    panels_lockText[panelID].color = tempColor_white;
                    panels_lockText[panelID].outlineColor = tempColor_black;
                }

                //スケール変更
                panels_tf[panelID].localScale = Vector3.one * (1 - temp / 5);
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
            //回転の端は例外として処理
            if (wheelEulerAnglesX > 348.75 && frontSkinID == 0)
                break;

            //フロントのスキンIDを更新
            float misalignment = wheelEulerAnglesX - 22.5f * frontSkinID;
            frontSkinID += (int)(Mathf.Sign(misalignment) * Mathf.Sign(180 - Mathf.Abs(misalignment)));

            //スキンIDを補正
            if (frontSkinID < 0) frontSkinID += 16;
            else if (frontSkinID > 15) frontSkinID -= 16;

            //音を鳴らす処理
            AM.PlaySE(AM.SE_SkinSelecter);

            /* ここでパネルの更新処理をすればスキンを増やせる（他の処理の変更も必要） */
        }

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
                GM.ChangePlayerSkin(frontSkinID);

                //スキンが有効なら
                if (GM.isSkinUnlocked[frontSkinID])
                {

                    //OKボタンの有効化
                    buttonOK.SetActive(true);
                }
                //スキンが無効ならOKボタンの無効化
                else buttonOK.SetActive(false);

                //パネルのスケール変更
                panels_tf[frontSkinID].localScale = Vector3.one * 1.03f;

                //ボタンの有効化
                IM.InputUISetActive_Skin(true);
            }
        }
    }



    //使用しているスキンにセレクターの設定を合わせる
    public void SetWheelAngle(int usingSkinID)
    {
        //スキンIDを格納
        frontSkinID = usingSkinID;

        //セレクターを回転
        targetAngleX = 22.5f * usingSkinID;
        wheelEulerAnglesX = 22.5f * usingSkinID;
        wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;
    }



    //スキンパネルを生成（全てロック状態で生成）
    public void CreateSkinPanel()
    {
        for (int ID = 0; ID < skinDataBase.skinData.Count; ID++)
        {
            //ロック解除条件を設定
            panels_lockText[ID].SetText(GM.GetUnlockSkinCondition(ID));

            if (ID % 8 != 7)
            {
                //スキン名表示
                panels_skinName[ID].SetText(skinDataBase.skinData[ID].name);

                //形の設定
                panels_skinModel[ID].sprite = 
                    skinDataBase.skinData[ID].bodyType == SkinData.BodyType.Cube ? GM.squarSprite : GM.cicleSprite;
            }
            //Crystalスキンは例外処理
            else
            {
                //名前を隠す
                panels_skinName[ID].SetText("？？？");

                //テクスチャをマスクしない
                panels_skinModel[ID].maskInteraction = SpriteMaskInteraction.None;

                //目を透明化
                panels_skinEyes_L[ID].color = Color.clear;
                panels_skinEyes_R[ID].color = Color.clear;

                //テクスチャ上の"?"を表示
                panels_skinModelText[ID / 8].SetText("?");
            }
        }
    }



    //パネルをアンロック状態に
    public void UnlockPanel(int skinID)
    {
        //ロック解除条件を非表示に
        panels_lockText[skinID].SetText("");

        //Crystalスキンのみ例外処理
        if (skinID % 8 == 7)
        {
            //名前を表示
            panels_skinName[skinID].SetText(skinDataBase.skinData[skinID].name);

            //テクスチャをマスク
            panels_skinModel[skinID].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            //テクスチャ上の"?"を非表示に
            panels_skinModelText[skinID / 8].SetText("");
        }
    }
}
