using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewSkinData",menuName = "�X�L���f�[�^")]
public class SkinData : ScriptableObject
{
    //�X�L����
    new public string name;


    //�{�f�B�̌`
    public enum BodyType
    {
        Cube,
        Sphere
    }

    public BodyType bodyType;


    //�{�̓��̐F
    public Color mainColor;
}
