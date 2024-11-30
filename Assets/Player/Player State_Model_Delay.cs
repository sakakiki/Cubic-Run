public class PlayerState_Model_Delay : PlayerStateBase_Model
{
    public float delayTime;
    public PlayerStateBase nextState;

    public PlayerState_Model_Delay(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        //�w�莞�ԑҋ@��ɃX�e�[�g�J��
        if (elapsedTime > delayTime)
            stateMachine.ChangeState(nextState);
    }

    public override void Exit() { }
}
