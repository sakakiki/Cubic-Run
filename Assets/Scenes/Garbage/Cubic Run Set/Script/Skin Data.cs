using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewSkinData",menuName = "�X�L���f�[�^/SkinData")]
public class SkinData : ScriptableObject
{
    new public string name;

    public enum BodyType
    {
        Cube,
        Ball
    }

    public BodyType bodyType;

    public Color color;
}
