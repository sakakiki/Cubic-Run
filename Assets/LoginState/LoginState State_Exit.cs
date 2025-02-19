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
        //経過時間のリセット
        elapsedTime = 0;
    }

    public override void Update()
    {
        //経過時間の加算
        elapsedTime += Time.deltaTime;

        //画面カバーの透明度変更
        screenCover.color = Color.white - Color.black * elapsedTime/2;

        //2秒経過すればステート遷移
        if (elapsedTime > 2)
            gameStateMachine.ChangeState(gameStateMachine.state_Menu);
    }

    public override void Exit() { }
}
