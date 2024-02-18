using UnityEngine;

public class ChickenInitializer : MonoBehaviour
{
    public Sprite[] spriteOptions; // �洢��ѡ���Sprite
    public float[] customRotationYOptions; // �洢��ѡ���Y����ת�Ƕ�
    Vector3 customRotation = new Vector3(0f, 0f, 0f); // �̶��ĳ���Ƕ�
    public float minScale = 1f; // ��С����ֵ
    public float maxScale = 1f; // �������ֵ

    void Start()
    {
        InitializeChicken();
    }

    void Update()
    {
        // ��Update�����ù̶�����
        transform.eulerAngles = customRotation;
    }

    void InitializeChicken()
    {
        // ���ѡ��һ��Sprite
        int randomIndex = Random.Range(0, spriteOptions.Length);
        Sprite selectedSprite = spriteOptions[randomIndex];

        // ��ȡԭʼ�����ϵ�SpriteRenderer���
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            // ���û��SpriteRenderer����������һ��
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // ����Sprite
        spriteRenderer.sprite = selectedSprite;
        customRotation.y = customRotationYOptions[Random.Range(0, customRotationYOptions.Length)];

        float randomScale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(randomScale, randomScale, 1f);
    }
}