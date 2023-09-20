using Hypertonic.GridPlacement;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public BoxCollider boxCollider; // 拖拽你的Box Collider到这里
    public Sprite spriteToRender; // 拖拽你想渲染的Sprite到这里

    void Start()
    {
        boxCollider = gameObject.GetComponent<BoxCollider>();

        // 获取Box Collider的尺寸和位置
        Vector3 colliderSize = boxCollider.bounds.size;
        Vector2 colliderCenter = boxCollider.bounds.center;

        // 创建一个新的GameObject作为Sprite
        GameObject spriteObj = new GameObject("GeneratedSprite");
        SpriteRenderer spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = spriteToRender;

        // 设置Sprite的尺寸
        spriteObj.transform.localScale = new Vector3(colliderSize.x, colliderSize.z, 1);
        Vector3 newPosition = gameObject.transform.position;
        //newPosition.y = GridManagerAccessor.GridManager.GetGridPosition().y;
        

        // 定位Sprite到Box Collider的底部
        spriteObj.transform.SetParent(this.transform);
        spriteObj.transform.rotation = Quaternion.Euler(90, 0, 0);
        spriteObj.transform.position = newPosition;

    }
}
