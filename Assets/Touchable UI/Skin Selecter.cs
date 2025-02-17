using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SkinSelecter : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private GameManager GM;
    [SerializeField] private InputManager IM;
    [SerializeField] private SkinDataBase skinDataBase;
    [SerializeField] public Sprite squar;
    [SerializeField] public Sprite cicle;
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
        //�t���O�̏�����
        isStop = false;
        isActive = false;

        //OK�{�^���̗L����
        buttonOK.SetActive(true);
    }



    public void OnPointerDown(PointerEventData eventData)
    {
        //��A�N�e�B�u��ԂȂ牽�����Ȃ�
        if (!isActive) return;

        //��]��Ԃɕω�
        isDragged = true;

        //�{�^���̖�����
        IM.InputUISetActive_Skin(false);

        //���ݒl�̋L����Queue�ւ̊i�[
        currentPointY = eventData.position.y;
        currentTime = Time.time;
        pointQueue.Enqueue(currentPointY);
        timeQueue.Enqueue(currentTime);
    }



    public void OnDrag(PointerEventData eventData)
    {
        //��A�N�e�B�u��ԂȂ牽�����Ȃ�
        if (!isActive) return;

        //��A�N�e�B�u��Ԃ̊ԂɃh���b�O���J�n����Ă���Ȃ�J�n�����̎��s
        if (!isDragged)
        {
            //��]��Ԃɕω�
            isDragged = true;

            //�{�^���̖�����
            IM.InputUISetActive_Skin(false);

            //���ݒl�̋L����Queue�ւ̊i�[
            currentPointY = eventData.position.y;
            currentTime = Time.time;
            pointQueue.Enqueue(currentPointY);
            timeQueue.Enqueue(currentTime);
        }

        //��~��Ԃ�����
        isStop = false;

        //�^�b�v�ʒu
        float newScreenPointY = eventData.position.y;

        //�z�C�[���̉�]
        wheelEulerAnglesX += (newScreenPointY - currentPointY) / Screen.height * 160;
        wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;

        //���ݒl�̋L����Queue�ւ̊i�[
        currentPointY = newScreenPointY;
        currentTime = Time.time;
        pointQueue.Enqueue(currentPointY);
        timeQueue.Enqueue(currentTime);

        //Queue���s�v�f�[�^�폜
        while (currentTime - timeQueue.Peek() > 0.1)
        {
            pointQueue.Dequeue();
            timeQueue.Dequeue();
        }
    }



    public void OnPointerUp(PointerEventData eventData)
    {
        //��A�N�e�B�u��ԂȂ牽�����Ȃ�
        if (!isActive) return;

        //��]��Ԃ��I��
        isDragged = false;

        //�X�i�b�v��ԉ���
        isSnapping = false;

        //���x�t�^
        if (timeQueue.Count >= 2)
            angleVelocityX = (currentPointY - pointQueue.Dequeue()) / (currentTime - timeQueue.Dequeue()) / Screen.height * 64;
        else angleVelocityX = 0;
        if (angleVelocityX > 1800) 
            angleVelocityX = 1800;

        //Queue���f�[�^�폜
        timeQueue.Clear();
        pointQueue.Clear();
    }



    private void Update()
    {
        //��]���܂��͔�A�N�e�B�u��ԂȂ�
        if (!isStop || !isActive)
        {
            //�p�l���̓����x�E�X�P�[���ύX
            for (int ID = 0; ID < panels_sprite.Length; ID++)
            {
                //���O�v�Z
                //�p�l���̊p�x���x�[�X�Ƀz�C�[���̃��[���h���W�ŕ␳��������
                float temp = (1 - panels_tf[ID].forward.z + (wheelTf.position.z - 90) / 25) * 5;

                //�����x�ύX
                Color tempColor_white = Color.white - Color.black * temp;
                Color tempColor_black = Color.black - Color.black * temp;
                panels_sprite[ID].color = tempColor_white;
                panels_skinName[ID].color = tempColor_black;
                if (GM.isSkinUnlocked[ID] || ID % 8 != 7)
                {
                    panels_skinModel[ID].color = skinDataBase.skinData[ID].skinColor - Color.black * temp;
                    panels_skinEyes_L[ID].color = tempColor_white;
                    panels_skinEyes_R[ID].color = tempColor_white;
                }
                //���b�N����Crystal�X�L���̂ݗ�O����
                else
                {
                    panels_skinModel[ID].color = tempColor_black;
                    panels_skinModelText[ID/8].color = tempColor_white * 0.5f;
                }
                //���b�N���̃p�l���̏���
                if (!GM.isSkinUnlocked[ID])
                {
                    panels_lockPanel[ID].color = tempColor_black * 0.8f;
                    panels_lockText[ID].color = tempColor_white;
                    panels_lockText[ID].outlineColor = tempColor_black;
                }

                //�X�P�[���ύX
                panels_tf[ID].localScale = Vector3.one * (1 - temp / 5);
            }
        }

        //��A�N�e�B�u��ԂȂ�c��̏������΂�
        if (!isActive) return;

        //��]�ʕ␳
        while (wheelEulerAnglesX > 360)
            wheelEulerAnglesX -= 360;
        while (wheelEulerAnglesX < 0)
            wheelEulerAnglesX += 360;

        //�z�C�[�������݂̃X�L���p�l���̈ʒu�̊O�܂ŉ�]������
        while (Mathf.Abs(wheelEulerAnglesX - 22.5f * frontSkinID) > 11.25)
        {
            //�t�����g�̃X�L��ID���X�V
            frontSkinID += (int)Mathf.Sign(wheelEulerAnglesX - 22.5f * frontSkinID);

            //����炷����

            /* �����Ńp�l���̍X�V����������΃X�L���𑝂₹��i���̏����̕ύX���K�v�j */
        }
        //�X�L��ID��␳
        if (frontSkinID < 0) frontSkinID += 16;
        else if (frontSkinID > 15) frontSkinID -= 16;

        //�h���b�O���͎c��̏������΂�
        if (isDragged) return;

        //�������Ȃ����]
        if (Mathf.Abs(angleVelocityX) > 15)
        {
            //�����������Ă������]���~�i�t��]�h�~�j
            if (Time.deltaTime * 2 > 1)
                angleVelocityX = 0;

            //��]������
            angleVelocityX *= (1 - Time.deltaTime * 2);

            //�z�C�[������]
            wheelEulerAnglesX += angleVelocityX * Time.deltaTime;
            wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;
        }

        //�X�i�b�v����
        else
        {
            //�J�n����
            if (!isSnapping)
            {
                //�X�i�b�v����J�n
                isSnapping = true;

                //��]�����ɕ␳�������ڕW��]�ʂ��Z�o
                targetAngleX = Mathf.Floor(wheelEulerAnglesX / 22.5f) * 22.5f;
                if (wheelEulerAnglesX + angleVelocityX/5 - targetAngleX > 11.25f)
                    targetAngleX += 22.5f;

                //��]���~
                angleVelocityX = 0;
            }

            //��]���K�v�Ȃ�
            if (Mathf.Abs(wheelEulerAnglesX - targetAngleX) > 1)
            {
                //��ԉ�]
                wheelEulerAnglesX = Mathf.Lerp(wheelEulerAnglesX, targetAngleX, Time.deltaTime * 10);
                wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;
            }

            //��~����
            else if (!isStop)
            {
                //��~��Ԃ�
                isStop = true;

                //�X�L���ύX����
                GM.ChangePlayerSkin(frontSkinID);

                //�X�L�����L���Ȃ�
                if (GM.isSkinUnlocked[frontSkinID])
                {

                    //OK�{�^���̗L����
                    buttonOK.SetActive(true);
                }
                //�X�L���������Ȃ�OK�{�^���̖�����
                else buttonOK.SetActive(false);

                //�p�l���̃X�P�[���ύX
                panels_tf[frontSkinID].localScale = Vector3.one * 1.03f;

                //�{�^���̗L����
                IM.InputUISetActive_Skin(true);
            }
        }
    }



    //�g�p���Ă���X�L���ɃZ���N�^�[�̐ݒ�����킹��
    public void SetWheelAngle(int usingSkinID)
    {
        //�X�L��ID���i�[
        frontSkinID = usingSkinID;

        //�Z���N�^�[����]
        targetAngleX = 22.5f * usingSkinID;
        wheelEulerAnglesX = 22.5f * usingSkinID;
        wheelTf.localEulerAngles = wheelEulerAnglesX * Vector3.right;
    }



    //�X�L���p�l���𐶐��i�S�ă��b�N��ԂŐ����j
    public void CreateSkinPanel()
    {
        for (int ID = 0; ID < skinDataBase.skinData.Count; ID++)
        {
            //���b�N����������ݒ�
            panels_lockText[ID].SetText(GM.GetUnlockSkinCondition(ID));

            if (ID % 8 != 7)
            {
                //�X�L�����\��
                panels_skinName[ID].SetText(skinDataBase.skinData[ID].name);

                //�`�̐ݒ�
                panels_skinModel[ID].sprite = skinDataBase.skinData[ID].bodyType == SkinData.BodyType.Cube ? squar : cicle;
            }
            //Crystal�X�L���͗�O����
            else
            {
                //���O���B��
                panels_skinName[ID].SetText("�H�H�H");

                //�e�N�X�`�����}�X�N���Ȃ�
                panels_skinModel[ID].maskInteraction = SpriteMaskInteraction.None;

                //�ڂ𓧖���
                panels_skinEyes_L[ID].color = Color.clear;
                panels_skinEyes_R[ID].color = Color.clear;

                //�e�N�X�`�����"?"��\��
                panels_skinModelText[ID / 8].SetText("?");
            }
        }
    }



    //�p�l�����A�����b�N��Ԃ�
    public void UnlockPanel(int skinID)
    {
        //���b�N�����������\����
        panels_lockText[skinID].SetText("");

        //Crystal�X�L���̂ݗ�O����
        if (skinID % 8 == 7)
        {
            //���O��\��
            panels_skinName[skinID].SetText(skinDataBase.skinData[skinID].name);

            //�e�N�X�`�����}�X�N
            panels_skinModel[skinID].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

            //�e�N�X�`�����"?"���\����
            panels_skinModelText[skinID/8].SetText("");
        }
    }
}
