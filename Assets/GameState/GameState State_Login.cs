using UnityEngine;

public class GameStateState_Login : GameStateStateBase
{
    private LoginStateStateMachine loginStateMachine;

    private SpriteRenderer screenCover;



    public GameStateState_Login(GameStateStateMachine stateMachine) : base(stateMachine)
    {
        //ログインステートマシンの作成
        loginStateMachine = new LoginStateStateMachine(stateMachine);
        
        screenCover = GM.frontScreenCover;
    }



    public override void Enter()
    {
        //ログインステートマシンの初期化
        loginStateMachine.Initialize(loginStateMachine.state_Login);

        //障害物の生成を有効化
        TM.isCreateObstacle = true;

        //ログイン画面の画面カバーを白に
        screenCover.color = Color.white;

        //ローディングキューブを表示
        GM.loadingCube.SetActive(true);
    }



    public override void Update(float deltaTime)
    {
        //ステートマシンのUpdateを実行
        loginStateMachine.Update();

        //地形の管理
        TM.ManageMovingTerrain();

        //ローディングキューブの回転
        GM.loadingCube.transform.Rotate(Vector3.up, 180 * deltaTime);
    }



    public override void Exit()
    {
        //ログイン画面の画面カバーを透明に
        screenCover.color = Color.clear;
    }
}
