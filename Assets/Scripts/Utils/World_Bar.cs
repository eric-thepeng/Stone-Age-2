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
     * Bar in the World, great for quickly making a health bar
     * */
    public class World_Bar {
        
        private GameObject gameObject;
        private Transform transform;
        private Transform background;
        private Transform bar;

        public static int GetSortingOrder(Vector3 position, int offset, int baseSortingOrder = 5000) {
            return (int)(baseSortingOrder - position.y) + offset;
        }

        public class Outline {
            public float size = 1f;
            public Color color = Color.black;
        }

        private void SetupParent(Transform parent, Vector3 localPosition) {
            gameObject = new GameObject("World_Bar");
            transform = gameObject.transform;
            transform.SetParent(parent);
            transform.localPosition = localPosition;
        }
        public void SetRotation(float rotation) {
            transform.localEulerAngles = new Vector3(0, 0, rotation);
        }
        public void SetSize(float sizeRatio) {
            bar.localScale = new Vector3(sizeRatio, 1, 1);
        }
        public void SetColor(Color color) {
            bar.Find("BarIn").GetComponent<SpriteRenderer>().color = color;
        }
        public void Show() {
            gameObject.SetActive(true);
        }
        public void Hide() {
            gameObject.SetActive(false);
        }
        public Button_Sprite AddButton(System.Action ClickFunc, System.Action MouseOverOnceFunc, System.Action MouseOutOnceFunc) {
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
            if (gameObject != null) {
                Object.Destroy(gameObject);
            }
        }

    }
    
}
