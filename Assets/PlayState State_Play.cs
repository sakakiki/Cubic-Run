using TMPro;

public class PlayStateState_Play : PlayStateStateBase
{
    private TextMeshProUGUI score;
    private PlayerStateMachine playerStateMachine;



    public PlayStateState_Play(PlayStateStateMachine stateMachine) : base(stateMachine)
    {
        score = GameManager.Instance.score;
        playerStateMachine = playerCon.stateMachine;
    }



    public override void Enter()
    {

    }



    public override void Update(float deltaTime)
    {
        //�v���C���ԉ��Z
        playTime += deltaTime;

        //�ړ����̒n�`���Ǘ�
        TM.ManageMovingTerrain();

        //�X�R�A�e�L�X�g�X�V
        score.SetText("" + (int)(playTime * 100));

        //�v���C���[��GameOver�X�e�[�g�Ȃ�GameOver�X�e�[�g�ɑJ��
        if (playerStateMachine.currentState == playerStateMachine.state_GameOver)
            stateMachine.ChangeState(stateMachine.state_GameOver);
    }



    public override void Exit()
    {

    }
}
