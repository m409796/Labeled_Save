using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    Transform parentAfterDrag;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(GameManager.Instance.interactCursor, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.SetCursor(GameManager.Instance.regularCursor, Vector2.zero, CursorMode.Auto);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("begin");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("drag begin");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("end begin");
        transform.SetParent(parentAfterDrag);
        Cursor.SetCursor(GameManager.Instance.regularCursor, Vector2.zero, CursorMode.Auto);
    }
}
