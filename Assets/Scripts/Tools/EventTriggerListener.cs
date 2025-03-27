using UnityEngine;
using UnityEngine.EventSystems;


public delegate void ColliderEventDelegate(Collider col);
public delegate void ColliderEventDelegate2D(Collider2D col);
public delegate void CollisionEventDelegate2D(Collision2D col);
public delegate void PointerEventDelegate(GameObject go, PointerEventData eventData);
public delegate void BaseEventDelegate(GameObject go, BaseEventData eventData);
public delegate void AxisEventDelegate(GameObject go, AxisEventData eventData);
public delegate void UnityActionInt(int arg);
public delegate void UnityActionFloat(float arg);
public delegate void UnityActionString(string arg);
public delegate void UnityActionBool(bool arg);

public class EventTriggerListener : EventTrigger
{
    //带参数是为了方便取得绑定了UI事件的对象    
    public event ColliderEventDelegate onTriggerEnter;
    public event ColliderEventDelegate onTriggerExit;
    public event ColliderEventDelegate onTriggerStay;
    public event ColliderEventDelegate onCollisionEnter;
    public event ColliderEventDelegate onCollisionExit;
    public event ColliderEventDelegate onCollisionStay;
    public event ColliderEventDelegate2D onTriggerEnter2D;
    public event ColliderEventDelegate2D onTriggerExit2D;
    public event ColliderEventDelegate2D onTriggerStay2D;
    public event CollisionEventDelegate2D onCollisionEnter2D;
    public event CollisionEventDelegate2D onCollisionExit2D;
    public event CollisionEventDelegate2D onCollisionStay2D;
    public event PointerEventDelegate onPointerEnter;
    public event PointerEventDelegate onPointerExit;
    public event PointerEventDelegate onPointerDown;
    public event PointerEventDelegate onPointerUp;
    public event PointerEventDelegate onPointerClick;
    public event PointerEventDelegate onInitializePotentialDrag;
    public event PointerEventDelegate onBeginDrag;
    public event PointerEventDelegate onDrag;
    public event PointerEventDelegate onEndDrag;
    public event PointerEventDelegate onDrop;
    public event PointerEventDelegate onScroll;
    public event BaseEventDelegate onUpdateSelected;
    public event BaseEventDelegate onSelect;
    public event BaseEventDelegate onDeselect;
    public event AxisEventDelegate onMove;
    public event BaseEventDelegate onSubmit;
    public event BaseEventDelegate onCancel;

    public static EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (onTriggerEnter != null) onTriggerEnter(other);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (onTriggerEnter2D != null) onTriggerEnter2D(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (onTriggerExit != null) onTriggerExit(other);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (onTriggerExit2D != null) onTriggerExit2D(other);

    }

    public void OnTriggerStay(Collider other)
    {
        if (onTriggerStay != null) onTriggerStay(other);
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (onTriggerStay2D != null) onTriggerStay2D(other);
    }

    public void OnCollisionEnter(Collision other)
    {
        if (onCollisionEnter != null) onCollisionEnter(other.collider);
    }
    public void OnCollisionEnter2D(Collision2D other)
    {

        if (onCollisionEnter2D != null) onCollisionEnter2D(other);
    }

    public void OnCollisionExit(Collision other)
    {
        if (onCollisionExit != null) onCollisionExit(other.collider);
    }

    public void OnCollisionExit2D(Collision2D other)
    {

        if (onCollisionExit2D != null) onCollisionExit2D(other);
    }


    public void OnCollisionStay(Collision other)
    {
        if (onCollisionStay != null) onCollisionStay(other.collider);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (onCollisionStay2D != null) onCollisionStay2D(other);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (onPointerEnter != null) onPointerEnter(gameObject, eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (onPointerExit != null) onPointerExit(gameObject, eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onPointerDown != null) onPointerDown(gameObject, eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onPointerUp != null) onPointerUp(gameObject, eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onPointerClick != null) onPointerClick(gameObject, eventData);
    }

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (onInitializePotentialDrag != null) onInitializePotentialDrag(gameObject, eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (onBeginDrag != null) onBeginDrag(gameObject, eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (onDrag != null) onDrag(gameObject, eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (onEndDrag != null) onEndDrag(gameObject, eventData);
    }

    public override void OnDrop(PointerEventData eventData)
    {
        if (onDrop != null) onDrop(gameObject, eventData);
    }

    public override void OnScroll(PointerEventData eventData)
    {
        if (onScroll != null) onScroll(gameObject, eventData);
    }

    public override void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelected != null) onUpdateSelected(gameObject, eventData);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(gameObject, eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        if (onDeselect != null) onDeselect(gameObject, eventData);
    }

    public override void OnMove(AxisEventData eventData)
    {
        if (onMove != null) onMove(gameObject, eventData);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        if (onSubmit != null) onSubmit(gameObject, eventData);
    }

    public override void OnCancel(BaseEventData eventData)
    {
        if (onCancel != null) onCancel(gameObject, eventData);
    }
}

