using UnityEngine;

public class PlayerState_Model_Skin : PlayerStateBase_Model
{
    private float waitTimer;
    private float targetDirection;

    public PlayerState_Model_Skin(PlayerStateMachine stateMachine) : base(stateMachine){ }

    public override void Enter()
    {
        base.Enter();

        //開始5秒後に瞬き
        eyeCloseTimer = 5;

        //タイマー初期化
        waitTimer = Random.Range(1f, 2.5f);
    }

    public override void Update()
    {
        base.Update();

        //ゲームステートがSkinToMenuならステート遷移
        if (gameStateMachine.currentState == gameStateMachine.state_SkinToMenu)
            stateMachine.ChangeState(stateMachine.state_Model_SkinToMenu);
    }

    public override void Exit()
    {

    }
}
