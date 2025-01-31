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

    
    //�X�L���J���[
    public Color skinColor;

    //�X�L���̃}�X�N��L�������邩
    public bool isEnabledMask;

    //UI�̃J���[
    public Color UIColor;
}
