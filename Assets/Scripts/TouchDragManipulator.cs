using UnityEngine;
using UnityEngine.UIElements;
using Unity.Mathematics;
using System;

namespace MultiTouchTest {

public class TouchDragManipulator : PointerManipulator
{
    public event Action<float2> OnDragging;
    public event Action<float> OnScrolling;
    (int id, float2 pos) _p1;
    (int id, float2 pos) _p2;

    public TouchDragManipulator()
    {
        _p1.id = _p2.id = -1;
        activators.Add(new ManipulatorActivationFilter{button = MouseButton.LeftMouse});
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        target.RegisterCallback<WheelEvent>(OnWheelScrolled);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        target.UnregisterCallback<WheelEvent>(OnWheelScrolled);
    }

    void OnPointerDown(PointerDownEvent e)
    {
        if (!CanStartManipulation(e)) return;

        var id = e.pointerId;
        if (_p1.id == id || _p2.id == id) return;

        var pos = math.float3(e.localPosition).xy;

        if (_p1.id < 0)
        {
            _p1 = (id, pos);
            target.CapturePointer(id);
            e.StopPropagation();
        }
        else if (_p2.id < 0)
        {
            _p2 = (id, pos);
            target.CapturePointer(id);
            e.StopPropagation();
        }
    }

    void OnPointerMove(PointerMoveEvent e)
    {
        var id = e.pointerId;
        var pos = math.float3(e.localPosition).xy;
        float2 delta;

        if (_p1.id == id)
            (_p1.pos, delta) = (pos, pos - _p1.pos);
        else if (_p2.id == id)
            (_p2.pos, delta) = (pos, pos - _p2.pos);
        else
            return;

        var style = target.resolvedStyle;
        delta /= math.min(style.width, style.height);

        if (_p1.id < 0 || _p2.id < 0)
            OnDragging?.Invoke(delta);
        else
            OnScrolling?.Invoke(delta.y / 2);

        e.StopPropagation();
    }

    void OnPointerUp(PointerUpEvent e)
    {
        var id = e.pointerId;

        if (_p1.id == id)
            _p1.id = -1;
        else if (_p2.id == id)
            _p2.id = -1;
        else
            return;

        target.ReleasePointer(id);
        e.StopPropagation();
    }

    void OnWheelScrolled(WheelEvent e)
    {
        OnScrolling(e.delta.y / -2000);
        e.StopPropagation();
    }
}

} // namespace MultiTouchTest
