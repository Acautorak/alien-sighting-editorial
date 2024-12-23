using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    [SerializeField] private bool isLeft;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject != null)
        {
            RectTransform droppedRectTransform = droppedObject.GetComponent<RectTransform>();
            droppedRectTransform.SetParent(transform);
            droppedRectTransform.anchoredPosition = Vector2.zero;
            NotificationBuss.Publish(EventNames.OnCaseDropped, isLeft);
        }
    }

}
