/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading the Code Monkey Utilities
    I hope you find them useful in your projects
    If you have any questions use the contact form
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using UnityEngine;

namespace CodeMonkey.Utils {

    /*
     * Sprite in the World
     * */
    public class World_Sprite {
        
        private const int sortingOrderDefault = 5000;

        public GameObject gameObject;
        public Transform transform;
        private SpriteRenderer spriteRenderer;
        

        public static int GetSortingOrder(Vector3 position, int offset, int baseSortingOrder = sortingOrderDefault) {
            return (int)(baseSortingOrder - position.y) + offset;
        }




        public World_Sprite(Transform parent, Vector3 localPosition, Vector3 localScale, Sprite sprite, Color color, int sortingOrderOffset) {
            int sortingOrder = GetSortingOrder(localPosition, sortingOrderOffset);
            gameObject = UtilsClass.CreateWorldSprite(parent, "Sprite", sprite, localPosition, localScale, sortingOrder, color);
            transform = gameObject.transform;
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }
        public void SetSortingOrderOffset(int sortingOrderOffset) {
            SetSortingOrder(GetSortingOrder(gameObject.transform.position, sortingOrderOffset));
        }
        public void SetSortingOrder(int sortingOrder) {
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        }
        public void SetLocalScale(Vector3 localScale) {
            transform.localScale = localScale;
        }
        public void SetPosition(Vector3 localPosition) {
            transform.localPosition = localPosition;
        }
        public void SetColor(Color color) {
            spriteRenderer.color = color;
        }
        public void SetSprite(Sprite sprite) {
            spriteRenderer.sprite = sprite;
        }
        public void Show() {
            gameObject.SetActive(true);
        }
        public void Hide() {
            gameObject.SetActive(false);
        }
        public Button_Sprite AddButton(System.Action ClickFunc, System.Action MouseOverOnceFunc, System.Action MouseOutOnceFunc) {
            gameObject.AddComponent<BoxCollider2D>();
            Button_Sprite buttonSprite = gameObject.AddComponent<Button_Sprite>();
            if (ClickFunc != null)
                buttonSprite.ClickFunc = ClickFunc;
            if (MouseOverOnceFunc != null)
                buttonSprite.MouseOverOnceFunc = MouseOverOnceFunc;
            if (MouseOutOnceFunc != null)
                buttonSprite.MouseOutOnceFunc = MouseOutOnceFunc;
            return buttonSprite;
        }
        public void DestroySelf() {
            Object.Destroy(gameObject);
        }

    }
}