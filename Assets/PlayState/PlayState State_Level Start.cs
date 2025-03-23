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

        //BGM�̐؂�ւ�
        if (GM.level <= 5)
        {
            if (AudioManager.Instance.audioSource_BGM.clip != AudioManager.Instance.BGM_Play_1)
            {
                AudioManager.Instance.audioSource_BGM.clip = AudioManager.Instance.BGM_Play_1;
                AudioManager.Instance.audioSource_BGM.Play();
            }
            AudioManager.Instance.SetBGMSpeed(Mathf.Lerp(0.9f, 1.1f, (GM.level - 1) / 4f));
        }
        else if (6 <= GM.level && GM.level <= 10)
        {
            if (AudioManager.Instance.audioSource_BGM.clip != AudioManager.Instance.BGM_Play_2)
            {
                AudioManager.Instance.audioSource_BGM.clip = AudioManager.Instance.BGM_Play_2;
                AudioManager.Instance.audioSource_BGM.Play();
            }
            AudioManager.Instance.SetBGMSpeed(Mathf.Lerp(1.05f, 1.25f, (GM.level - 6) / 4f));
        }
        else if (11 <= GM.level && GM.level <= 15)
        {
            if (AudioManager.Instance.audioSource_BGM.clip != AudioManager.Instance.BGM_Play_3)
            {
                AudioManager.Instance.audioSource_BGM.clip = AudioManager.Instance.BGM_Play_3;
                AudioManager.Instance.audioSource_BGM.Play();
            }
            AudioManager.Instance.SetBGMSpeed(Mathf.Lerp(1.1f, 1.25f, (GM.level - 11) / 4f));
        }
        else if (16 <= GM.level && GM.level <= 20)
        {
            if (AudioManager.Instance.audioSource_BGM.clip != AudioManager.Instance.BGM_Play_4)
            {
                AudioManager.Instance.audioSource_BGM.clip = AudioManager.Instance.BGM_Play_4;
                AudioManager.Instance.audioSource_BGM.Play();
            }
            AudioManager.Instance.SetBGMSpeed(Mathf.Lerp(1.1f, 1.2f, (GM.level - 16) / 4f));
        }
        else if (21 <= GM.level)
        {
            if (AudioManager.Instance.audioSource_BGM.clip != AudioManager.Instance.BGM_Play_5)
            {
                AudioManager.Instance.audioSource_BGM.clip = AudioManager.Instance.BGM_Play_5;
                AudioManager.Instance.audioSource_BGM.Play();
                AudioManager.Instance.SetBGMSpeed(1);
            }
        }

        //�g���[�j���O���[�h�Ȃ��Q�������I����x�点��
        if (GM.isTraining)
            ((PlayStateState_Play)stateMachine.state_Play).levelEndScore += 250;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //���x���e�L�X�g�̈ʒu�E�傫���E�F����
        int shortageScore = levelStartScore - scoreCorrection - GM.score;
        levelTf.position = Vector3.Lerp(levelMarker.position, centerPos, shortageScore / 75f);
        levelText.fontSize = Mathf.Lerp(72, 300, shortageScore / 75f);
        levelText.color = Color.Lerp(Color.black, Color.clear, (shortageScore - 200) / 50f);

        //�X�R�A����𖞂����΃X�e�[�g�J��
        if (GM.score > levelStartScore - scoreCorrection)
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
