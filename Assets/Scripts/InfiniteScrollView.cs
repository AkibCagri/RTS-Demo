using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfiniteScrollView : MonoBehaviour, IScrollHandler
{
    [SerializeField] List<RectTransform> itemTransforms = new List<RectTransform>();
    [SerializeField] List<RectTransform> itemPool = new List<RectTransform>();
    [SerializeField] float spacing;
    [SerializeField] RectTransform topPoint, bottomPoint;

    public void OnScroll(PointerEventData eventData)
    {
        RectTransform removedItem = null;
        RectTransform addedItem = null;
        foreach (var item in itemTransforms)
        {
            item.anchoredPosition += Vector2.up * eventData.scrollDelta.y * 50;
            // If current item anchored position y is greater than top point anchored position y, current item is selected as removed item and added top of the item pool.
            // And bottom item in the item pool is selected as added item.
            if (item.anchoredPosition.y > topPoint.anchoredPosition.y)
            {
                float difference = item.anchoredPosition.y - topPoint.anchoredPosition.y;
                Vector2 newAnchoredPos = bottomPoint.anchoredPosition + Vector2.up * difference;
                itemPool.Insert(0, item);
                removedItem = item;
                item.gameObject.SetActive(false);
                addedItem = itemPool[itemPool.Count - 1];
                addedItem.anchoredPosition = newAnchoredPos;
                addedItem.gameObject.SetActive(true);

            }
            // If current item anchored position y is lower than bottom point anchored position y, current item is selected as removed item and added botom of the item pool.
            // And top item in the item pool is selected as added item.
            else if (item.anchoredPosition.y < bottomPoint.anchoredPosition.y)
            {
                float difference = item.anchoredPosition.y - bottomPoint.anchoredPosition.y;
                Vector2 newAnchoredPos = topPoint.anchoredPosition + Vector2.up * difference;
                itemPool.Add(item);
                removedItem = item;
                item.gameObject.SetActive(false);
                addedItem = itemPool[0];
                addedItem.anchoredPosition = newAnchoredPos;
                addedItem.gameObject.SetActive(true);
            }
        }

        // if removed item is not null, removed item is removed from item transforms.
        if (removedItem != null)
        {
            itemTransforms.Remove(removedItem);
        }

        // if added item is not null, added item is added to item transforms and removed from item pool.
        if (addedItem != null)
        {
            itemTransforms.Add(addedItem);
            itemPool.Remove(addedItem);
        }
    }
}
