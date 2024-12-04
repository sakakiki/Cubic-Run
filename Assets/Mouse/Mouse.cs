using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clip;
    private Transform tf;
    [SerializeField] private GameObject touchObject;

    void Start()
    {
        Cursor.visible = false;
        tf = transform;
    }

    void Update()
    {
        tf.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        tf.localScale = Input.GetKey(KeyCode.Mouse0) ? Vector3.one : Vector3.one * 1.1f;
        touchObject.SetActive(Input.GetKey(KeyCode.Mouse0));
        if (Input.GetKeyDown(KeyCode.Mouse0)) audioSource.PlayOneShot(clip);
    }
}
