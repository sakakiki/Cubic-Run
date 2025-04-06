using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    public static int RankingCount = 10;   //�����L���O�̕\����

    private static float rankingUpdateSpan = 60;   //�n�C�X�R�A�����L���O�X�V�̍ŒZ�Ԋu
    private static float rankingUpdateTime_highScore = 0;
    private static float rankingUpdateTime_playerScore = 0;

    public enum RankingType
    {
        HighScore,
        PlayerScore
    }

    public static List<(string name, int score, int experience, int skin)> rankingList_highScore
        = new List<(string name, int score, int experience, int skin)>();
    public static List<(string name, int score, int experience, int skin)> rankingList_playerScore
        = new List<(string name, int score, int experience, int skin)>();
    public static (int userRank, float userPercentile) userRankData_highScore = (0, 100);
    public static (int userRank, float userPercentile) userRankData_playerScore = (0, 100);



    //�����L���O�X�V�̕K�v�����m�F
    public static async Task CheckUpdateNecessity(RankingType rankingType)
    {
        //�O�񃉃��L���O�X�V����w�莞�Ԉȏ�o�߂��Ă��Ȃ���Ή������Ȃ�
        switch (rankingType)
        {
            case RankingType.HighScore:
                if (Time.time - rankingUpdateTime_highScore < rankingUpdateSpan) return;
                break;

            case RankingType.PlayerScore:
                if (Time.time - rankingUpdateTime_playerScore < rankingUpdateSpan) return;
                break;
        }

        //�����L���O�̍X�V
        await UpdateRanking(rankingType);
    }



    //�����L���O���擾���X�V
    public static async Task<bool> UpdateRanking(RankingType rankingType)
    {
        switch (rankingType)
        {
            case RankingType.HighScore:

                //�g�b�v10�̃��X�g���擾
                List<(string name, int score, int experience, int skin)> newList_highScore
                    = await FirestoreManager.Instance.GetTop10Ranking("highScore");

                //����Ɏ擾�ł��Ă���΍X�V
                if (newList_highScore.Count == RankingCount)
                    RankingManager.rankingList_highScore = newList_highScore;

                //���[�U�[�̏��ʂƏ�ʃp�[�Z���g�̎擾
                (int rank, float percentile) newUserData_highScore =
                    await FirestoreManager.Instance.GetUserRanking("highScore", GameManager.Instance.highScore);

                //����Ɏ擾�ł��Ă���΍X�V
                if (newUserData_highScore.rank != -1)
                    userRankData_highScore = newUserData_highScore;
                //�����łȂ���΃G���[��Ԃ�
                else return false;

                //�����L���O�X�V���Ԃ��L��
                rankingUpdateTime_highScore = Time.time;

                return true;



            case RankingType.PlayerScore:

                //�g�b�v10�̃��X�g���擾
                List<(string name, int score, int experience, int skin)> newList_playerScore
                    = await FirestoreManager.Instance.GetTop10Ranking("playerScore");

                //����Ɏ擾�ł��Ă���΍X�V
                if (newList_playerScore.Count == RankingCount)
                    rankingList_playerScore = newList_playerScore;

                //���[�U�[�̏��ʂƏ�ʃp�[�Z���g�̎擾
                (int rank, float percentile) newUserData_playerScore =
                    await FirestoreManager.Instance.GetUserRanking("playerScore", GameManager.Instance.GetPlayerScore());

                //����Ɏ擾�ł��Ă���΍X�V
                if (newUserData_playerScore.rank != -1)
                    userRankData_playerScore = newUserData_playerScore;
                //�����łȂ���΃G���[��Ԃ�
                else return false;

                //�����L���O�X�V���Ԃ��L��
                rankingUpdateTime_playerScore = Time.time;

                return true;

            default: return false;
        }
    }
}
