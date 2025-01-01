using TMPro;
using UnityEngine;

public class PlayStateState_LevelEnd : PlayStateStateBase
{
    private int levelUpScore;
    private TextMeshProUGUI levelText;
    private RectTransform levelRtf;
    private Vector3 centerPos;
    private RectTransform levelMarker;



    public PlayStateState_LevelEnd(PlayStateStateMachine stateMachine) : base(stateMachine)
    {
        levelText = GM.levelText;
        levelRtf = GM.levelRtf;
        centerPos = GM.centerPos_World;
        levelMarker = GM.levelMarkerRtf;
    }



    public override void Enter()
    {
        //���x�����㏸����X�R�A���Z�o
        levelUpScore = GM.level * GM.levelUpSpan;

        //��Q�������𖳌���
        TM.isCreateObstacle = false;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //���x���e�L�X�g�̈ʒu�E�傫���E�F����
        int shortageScore = levelUpScore - GM.score;
        levelRtf.position = Vector2.Lerp(centerPos, levelMarker.position, (shortageScore - 150) / 75f);
        levelText.fontSize = Mathf.Lerp(300, 72, (shortageScore - 150) / 75f);
        levelText.color = Color.Lerp(Color.clear, Color.black, (shortageScore - 50) / 50f);

        //�X�R�A�����x���㏸��𖞂����΃X�e�[�g�J��
        if (GM.score > levelUpScore)
            stateMachine.ChangeState(stateMachine.state_LevelStart);
    }



    public override void Exit()
    {

    }
}
