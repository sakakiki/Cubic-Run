using UnityEngine;

public class LoginStateState_Exit : LoginStateStateBase
{
    private float elapsedTime;
    private SpriteRenderer screenCover;

    public LoginStateState_Exit(LoginStateStateMachine stateMachine) : base(stateMachine)
    {
        screenCover = GM.loginScreenCover;
    }

    public override void Enter()
    {
        //�o�ߎ��Ԃ̃��Z�b�g
        elapsedTime = 0;
    }

    public override void Update()
    {
        //�o�ߎ��Ԃ̉��Z
        elapsedTime += Time.deltaTime;

        //��ʃJ�o�[�̓����x�ύX
        screenCover.color = Color.white - Color.black * elapsedTime/2;

        //2�b�o�߂���΃X�e�[�g�J��
        if (elapsedTime > 2)
            gameStateMachine.ChangeState(gameStateMachine.state_Menu);
    }

    public override void Exit() { }
}
