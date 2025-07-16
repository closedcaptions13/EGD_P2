using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private int num;

    [SerializeField] private QuizManager quizManager;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

            if (eventData.pointerDrag.GetComponent<DragAndDrop>().GetItemNum() == num)
            {
                quizManager.UpdateFinalScore();
            }
        }
    }
}
