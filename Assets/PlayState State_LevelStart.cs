using UnityEngine;
using TMPro;

public class PlayStateState_LevelStart : PlayStateStateBase
{
    private PlayerStateMachine playerStateMachine;
    private int levelStartScore;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI levelText;



    public PlayStateState_LevelStart(PlayStateStateMachine stateMachine) : base(stateMachine)
    {
        playerStateMachine = playerCon.stateMachine;
        scoreText = GameManager.Instance.scoreText;
        levelText = GameManager.Instance.levelText;
    }



    public override void Enter()
    {
        //��Q���������J�n����X�R�A���Z�o
        levelStartScore = GM.level * GM.levelUpSpan + 300;

        //���x���㏸�E�X�V
        GM.level++;
        levelText.SetText("Lv." + GM.level);

        //���x�X�V
        TM.moveSpeed = 5 + Mathf.Pow(GM.level, 0.7f) * 3;

        //��Q���������I������X�R�A���Z�o
        ((PlayStateState_Play)stateMachine.state_Play).levelEndScore = 
            GM.level * GM.levelUpSpan - (int)(300 + 2400 / TM.moveSpeed);

        //�v���C���[��GameOver�X�e�[�g�Ȃ�GameOver�X�e�[�g�ɑJ��
        if (playerStateMachine.currentState == playerStateMachine.state_GameOver)
            stateMachine.ChangeState(stateMachine.state_GameOver);
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

        //�X�R�A����𖞂����΃X�e�[�g�J��
        if (GM.score > levelStartScore)
            stateMachine.ChangeState(stateMachine.state_Play);
    }



    public override void Exit()
    {

    }
}
