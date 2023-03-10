using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HarvestFloatingText : MonoBehaviour
{
    TextMeshPro FloatingText;
    SpriteRenderer FloatingIcon;

    float timeCount = 0;
    [SerializeField]
    float endingTime;
    [SerializeField]
    float floatingSpeed;

    public void Setup(ItemScriptableObject item, int itemNumber)
    {
        FloatingText = transform.Find("Harvest Floating Text").gameObject.GetComponent<TextMeshPro>();
        FloatingIcon = transform.Find("Harvest Floating Icon").gameObject.GetComponent<SpriteRenderer>();

        gameObject.SetActive(true);

        FloatingText.text = itemNumber + "";
        FloatingIcon.sprite = item.iconSprite;
    }

    void Update()
    {
        if (timeCount <= endingTime)
        {
            // Vertical Movement
            gameObject.transform.position += new Vector3(0, floatingSpeed, 0);

            // Scale
            if (timeCount > 0.8f * endingTime)
            {
                float currentScale = (1f - (timeCount / endingTime)) / 0.2f;
                gameObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            }

            timeCount += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
