using UnityEngine;

public class ChickenInitializer : MonoBehaviour
{
    public Sprite[] spriteOptions; // 存储可选择的Sprite
    public float[] customRotationYOptions; // 存储可选择的Y轴旋转角度
    Vector3 customRotation = new Vector3(0f, 0f, 0f); // 固定的朝向角度
    public float minScale = 1f; // 最小缩放值
    public float maxScale = 1f; // 最大缩放值

    void Start()
    {
        InitializeChicken();
    }

    void Update()
    {
        // 在Update中设置固定朝向
        transform.eulerAngles = customRotation;
    }

    void InitializeChicken()
    {
        // 随机选择一个Sprite
        int randomIndex = Random.Range(0, spriteOptions.Length);
        Sprite selectedSprite = spriteOptions[randomIndex];

        // 获取原始对象上的SpriteRenderer组件
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            // 如果没有SpriteRenderer组件，则添加一个
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // 设置Sprite
        spriteRenderer.sprite = selectedSprite;
        customRotation.y = customRotationYOptions[Random.Range(0, customRotationYOptions.Length)];

        float randomScale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(randomScale, randomScale, 1f);
    }
}