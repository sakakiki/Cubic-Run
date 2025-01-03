using UnityEngine;
using TMPro;

public class PlayStateState_LevelStart : PlayStateStateBase
{
    private int levelStartScore;
    private TextMeshProUGUI levelText;
    private Transform levelTf;
    private Vector3 centerPos;
    private RectTransform levelMarker;



    public PlayStateState_LevelStart(PlayStateStateMachine stateMachine) : base(stateMachine)
    {
        levelText = GameManager.Instance.levelText;
        levelTf = GM.levelTf;
        centerPos = GM.centerPos_World;
        levelMarker = GM.levelMarkerRtf_Play;
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
            GM.level * GM.levelUpSpan - (int)(300 + 3200 / TM.moveSpeed);
        /*
            �l�̏ڍ�
            GM.level * GM.levelUpSpan�@�c�c���x���㏸�̃^�C�~���O�i�X�R�A���Z�j
            300�@�c�c�I�����o�Ɏg�p���鎞�ԁi�X�R�A���Z�j
            3200 / TM.moveSpeed�@�c�c��Q������ʊO�Ɉړ�����̂ɂ����鎞�ԁi�X�R�A���Z�j
        */
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //���x���e�L�X�g�̈ʒu�E�傫���E�F����
        int shortageScore = levelStartScore - GM.score;
        levelTf.position = Vector3.Lerp(levelMarker.position, centerPos, shortageScore / 75f);
        levelText.fontSize = Mathf.Lerp(72, 300, shortageScore / 75f);
        levelText.color = Color.Lerp(Color.black, Color.clear, (shortageScore - 200) / 50f);

        //�X�R�A����𖞂����΃X�e�[�g�J��
        if (GM.score > levelStartScore)
            stateMachine.ChangeState(stateMachine.state_Play);
    }



    public override void Exit()
    {
        //���x���̃e�L�X�g��Z���W���C��
        Vector3 levelPos = levelTf.position;
        levelPos.z = GM.scoreSetTf.position.z;
        levelTf.position = levelPos;
    }
}
