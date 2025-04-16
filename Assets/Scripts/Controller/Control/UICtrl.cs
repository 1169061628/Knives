using UnityEngine;
using UnityEngine.EventSystems;

public class UICtrl
{
    UIMgr uiMgr;
    EventTrigger ctrlEvent;
    RectTransform joystickBg;
    RectTransform joystickHead;
    public EventHandler<Vector2> joystickBindable;
    RectTransform joystickOriPos;
    Vector2 joySize;
    Vector2 headCenterPos;
    Vector2 vec2Zero = Vector2.zero;
    Vector2 Vec2One = Vector2.one;
    Vector2 vec2Half = Vector2.one * 0.5f;
    float joyBoundOff;
    public UICtrl(UIMgr uiMgr, EventTrigger ce, RectTransform jbg, RectTransform jh, RectTransform jop)
    {
        this.uiMgr = uiMgr;
        ctrlEvent = ce;
        joystickBg = jbg;
        joystickHead = jh;
        joystickOriPos = jop;
        joySize = joystickBg.sizeDelta * 0.5f;
        joyBoundOff = joySize.x + 10;
        headCenterPos = vec2Zero;

        var trigger = new EventTrigger.Entry();
        trigger.eventID = EventTriggerType.PointerDown;
        trigger.callback.AddListener(eventData => OnPointerDown(eventData as PointerEventData));

        var trigger2 = new EventTrigger.Entry();
        trigger2.eventID = EventTriggerType.PointerUp;
        trigger2.callback.AddListener(eventData => OnPointerUp(eventData as PointerEventData));

        var trigger3 = new EventTrigger.Entry();
        trigger3.eventID = EventTriggerType.Drag;
        trigger3.callback.AddListener(eventData => OnDrag(eventData as PointerEventData));

        ctrlEvent.triggers.Add(trigger);
        ctrlEvent.triggers.Add(trigger2);
        ctrlEvent.triggers.Add(trigger3);
    }

    public void Ready() => joystickBg.gameObject.SetActive(true);
    public void Hide() => joystickBg.gameObject.SetActive(false);

    void OnPointerDown(PointerEventData eventData)
    {
        joystickBg.localPosition = uiMgr.realCanvasSize * (eventData.position / new Vector2(Screen.width, Screen.height));
    }

    void OnPointerUp(PointerEventData eventData)
    {
        joystickBg.anchoredPosition = joystickOriPos.localPosition;
        joystickHead.anchoredPosition = headCenterPos;
        joystickBindable.Send(vec2Zero);
    }

    void OnDrag(PointerEventData eventData)
    {
        var curPos = uiMgr.realCanvasSize * (eventData.position / new Vector2(Screen.width, Screen.height));
        var off = curPos - joystickBg.anchoredPosition;
        // 摇杆偏移量限制
        if (off.magnitude > joySize.x) off = off.normalized * joySize.x;
        joystickHead.anchoredPosition = off;
        joystickBindable.Send(off.normalized);
        var bgPos = joystickBg.anchoredPosition;
        if (curPos.x > bgPos.x + joyBoundOff) bgPos.x -= joyBoundOff;
        else if(curPos.x < bgPos.x - joyBoundOff) bgPos.x += joyBoundOff;
        if (curPos.y > bgPos.y + joyBoundOff) bgPos.y -= joyBoundOff;
        else if (curPos.y < bgPos.y - joyBoundOff) bgPos.y += joyBoundOff;
        joystickBg.anchoredPosition = bgPos;
    }
    public float Evaluate(float from, float to, float time)
    {
        return from + (to - from) * ((0 - time) * time - 2);
    }

    public void Dispose()
    {
        joystickBindable.Clear();
        joystickBindable = null;
    }
}