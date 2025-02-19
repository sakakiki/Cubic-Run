using UnityEngine;

public class GameStateState_Login : GameStateStateBase
{
    private LoginStateStateMachine loginStateMachine;

    private SpriteRenderer screenCover;



    public GameStateState_Login(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        //���O�C���X�e�[�g�}�V���̍쐬�Ə�����
        loginStateMachine = new LoginStateStateMachine(stateMachine);
        loginStateMachine.Initialize(loginStateMachine.state_Select);
        
        screenCover = GM.frontScreenCover;
    }



    public override void Enter()
    {
        //��Q���̐�����L����
        TM.isCreateObstacle = true;

        //���O�C����ʂ̉�ʃJ�o�[�𔒂�
        screenCover.color = Color.white;
    }



    public override void Update(float deltaTime)
    {
        //�X�e�[�g�}�V����Update�����s
        loginStateMachine.Update();

        //�n�`�̊Ǘ�
        TM.ManageMovingTerrain();
    }



    public override void Exit()
    {
        //���O�C����ʂ̉�ʃJ�o�[�𓧖���
        screenCover.color = Color.clear;
    }
}
