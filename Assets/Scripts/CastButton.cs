using UnityEngine;
using UnityEngine.EventSystems;

public class CastButton : MonoBehaviour, IPointerDownHandler
{
    public GameManager gameManager;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Tell GameManager that the player wants to start aiming
        gameManager.BeginAim();
    }
}
