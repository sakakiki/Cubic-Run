using System.Collections.Generic;
using UnityEngine;

public class SkinDataBase : MonoBehaviour
{
    //���g�̃C���X�^���X
    public static SkinDataBase Instance;

    //�X�L���f�[�^�̃��X�g
    public List<SkinData> skinData = new List<SkinData>();



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
