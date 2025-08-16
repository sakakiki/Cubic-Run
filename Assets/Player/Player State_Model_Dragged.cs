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

        //�������Z�𖳌���
        rb.isKinematic = true;

        //�^�����~
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0;

        //��]�ʂ��L��
        startAngle = tf.eulerAngles.z;

        //�X�P�[���ύX
        tf.localScale = Vector3.one * 1.6f;

        //�ڂ��J����
        eyeTf.localScale = Vector3.one;
        eyeCloseTimer = 5;

        //�������ڂ�
        playerCon.eyeDragged.SetActive(true);

        //�h���b�O�J�n���̈ʒu�E�^�b�`�ʒu�̃Y�����L��
        startScreenPoint = mainCam.WorldToScreenPoint(tf.position);
        touchOffset = tf.position - mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, startScreenPoint.z));

        //�v���C���[�̍��W�ƒ��S�̈ʒu�̃Y�����L��
        centerOffset = playerCenterTf.position - tf.position;

        //SE�Đ�
        AudioManager.Instance.PlaySE(AudioManager.SE.Player_Hold);
    }

    public override void Update()
    {
        //�o�ߎ��ԉ��Z
        elapsedTime += Time.deltaTime;

        //�^�b�`�i�}�E�X�j�̍��W�����ƂɈړ���̈ʒu���v�Z
        Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, startScreenPoint.z);
        Vector3 newPosition = mainCam.ScreenToWorldPoint(currentScreenPoint) + touchOffset;

        //��ʊO�ɏo���ꍇ�͕␳
        if (newPosition.y + centerOffset.y - tf.localScale.y / 2 < mainCamTf.position.y) 
            newPosition.y = mainCamTf.position.y - centerOffset.y + tf.localScale.y / 2;
        if (newPosition.x + centerOffset.x - tf.localScale.x / 2 < -5)
            newPosition.x = -5 - centerOffset.x + tf.localScale.x / 2;
        else if (newPosition.x + centerOffset.x + tf.localScale.x / 2 > 15)
            newPosition.x = 15 - centerOffset.x - tf.localScale.x / 2;

        //���ݒl�̋L����Queue�ւ̊i�[
        currentTime = Time.time;
        currentPos = newPosition;
        timeQueue.Enqueue(currentTime);
        posQueue.Enqueue(currentPos);

        //�ʒu�Ɖ�]��ύX
        rb.position = newPosition;
        rb.SetRotation(Mathf.Sin(Mathf.PI * elapsedTime * 4) * 2 + startAngle);

        //Queue���s�v�f�[�^�폜
        while (Time.time - timeQueue.Peek() > 0.1)
        {
            timeQueue.Dequeue();
            posQueue.Dequeue();
        }

        //�Q�[���X�e�[�g���J�ڂ����Ȃ�X�e�[�g�J��
        if (gameStateMachine.currentState == gameStateMachine.state_MenuToPlay ||
            gameStateMachine.currentState == gameStateMachine.state_MenuToTutorial ||
            gameStateMachine.currentState == gameStateMachine.state_ResultToPlay)
            stateMachine.ChangeState(stateMachine.state_Model_MenuToPlay);
        else if (gameStateMachine.currentState == gameStateMachine.state_MenuToSkin)
            stateMachine.ChangeState(stateMachine.state_Model_MenuToSkin);
    }

    public override void Exit()
    {
        //�X�P�[���ύX
        tf.localScale = Vector3.one * 1.5f;

        //�������ڂ�߂�
        playerCon.eyeDragged.SetActive(false);

        //�������Z�̗L����
        rb.isKinematic = false; 

        //��]��L����
        rb.freezeRotation = false;

        //���x�t�^
        rb.linearVelocity = (currentPos - posQueue.Dequeue()) / (currentTime - timeQueue.Dequeue()) / 2;

        //Queue���f�[�^�폜
        timeQueue.Clear();
        posQueue.Clear();
    }
}
