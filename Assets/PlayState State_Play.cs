using TMPro;

public class PlayStateState_Play : PlayStateStateBase
{
    private PlayerStateMachine playerStateMachine;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI levelText;



    public PlayStateState_Play(PlayStateStateMachine stateMachine) : base(stateMachine)
    {
        playerStateMachine = playerCon.stateMachine;
        scoreText = GameManager.Instance.scoreText;
        levelText = GameManager.Instance.levelText;
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

        //�X�R�A�X�V
        GM.score = (int)(playTime * 100);
        scoreText.SetText("" + GM.score);

        //���x���X�V
        GM.level = GM.score / 1000 + 1;
        levelText.SetText("Lv." + GM.level);

        //�v���C���[��GameOver�X�e�[�g�Ȃ�GameOver�X�e�[�g�ɑJ��
        if (playerStateMachine.currentState == playerStateMachine.state_GameOver)
            stateMachine.ChangeState(stateMachine.state_GameOver);
    }



    public override void Exit()
    {

    }
}
