using UnityEngine;

public class PlayerState_Model_Skin : PlayerStateBase_Model
{
    private float waitTimer;
    private float targetDirection;

    public PlayerState_Model_Skin(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        base.Enter();

        //�J�n5�b��ɏu��
        eyeCloseTimer = 5;

        //�^�C�}�[������
        waitTimer = Random.Range(1f, 2.5f);
    }

    public override void Update()
    {
        base.Update();

        //�Q�[���X�e�[�g��SkinToMenu�Ȃ�X�e�[�g�J��
        if (gameStateMachine.currentState == gameStateMachine.state_SkinToMenu)
            stateMachine.ChangeState(stateMachine.state_Model_SkinToMenu);
    }

    public override void Exit()
    {

    }
}
