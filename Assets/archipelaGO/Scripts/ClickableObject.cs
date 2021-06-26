using UnityEngine;
using UnityEngine.EventSystems;

namespace archipelaGO.WorldMap
{
    public abstract class ClickableObject : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData pointerEventData) =>
            OnObjectClicked();
        
        protected abstract void OnObjectClicked();
    }
}