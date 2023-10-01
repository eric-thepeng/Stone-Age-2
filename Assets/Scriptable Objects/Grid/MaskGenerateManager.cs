using UnityEngine;

public class MaskGenerateManager : MonoBehaviour
{
    public GameObject defaultSizePrefab;
    public GameObject spriteToRenderPrefab; // 假设你有一个预制体用于实例化 spriteToRender

    private GridOperationManager gridOperationManager;

    void Start()
    {
        gridOperationManager = FindObjectOfType<GridOperationManager>();
        //Invoke("SetupMask", 1f);
    }

    void Update()
    {

    }

    public void SetupMask()
    {
        spriteToRenderPrefab = defaultSizePrefab;
        gridOperationManager = FindObjectOfType<GridOperationManager>();

        int AmountOfCellsX = gridOperationManager._gridSettings.AmountOfCellsX;
        int AmountOfCellsY = gridOperationManager._gridSettings.AmountOfCellsY;

        Vector3 GridPosition = gridOperationManager._gridSettings.GridPosition;

        GameObject defaultSizeObject = Instantiate(defaultSizePrefab, transform);
        BoxCollider boxCollider = defaultSizeObject.GetComponent<BoxCollider>();
        Vector3 boxSize = boxCollider.size;

        for (int indX = 0; indX < AmountOfCellsX; indX++)
        {
            for (int indY = 0; indY < AmountOfCellsY; indY++)
            {
                Debug.Log(indX + " " + indY);
                Vector2 _posVec2 = gridOperationManager.GetWorldPositionFromCellIndex(new Vector2(indX, indY));
                Vector3 _posVec3 = new Vector3(_posVec2.x, GridPosition.y, _posVec2.y);

                if (IsPositionBlocked(_posVec3, boxSize))
                {
                    Debug.Log("is blocked");
                    Instantiate(spriteToRenderPrefab, _posVec3, Quaternion.identity, transform);
                }
            }
        }

        Destroy(defaultSizeObject);
    }

    private bool IsPositionBlocked(Vector3 position, Vector3 size)
    {
        // 射线的方向是向下
        Vector3 direction = Vector3.up;

        // 射线的长度是盒子的高度的一半
        float rayLength = 1 * 1f;

        // 如果射线与障碍物碰撞，返回true
        return Physics.Raycast(position, direction, rayLength);
    }

}
