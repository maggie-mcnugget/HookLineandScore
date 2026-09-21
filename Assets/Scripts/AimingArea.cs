using UnityEngine;
using UnityEngine.EventSystems;

public class AimingArea : MonoBehaviour, IDragHandler
{
    public GameManager gameManager;

    public void OnDrag(PointerEventData eventData)
    {
        gameManager.UpdateAim(eventData.position);
    }
}