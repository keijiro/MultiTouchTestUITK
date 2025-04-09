using UnityEngine;
using UnityEngine.UIElements;
using Unity.Mathematics;

namespace MultiTouchTest {

public sealed class CameraController : MonoBehaviour
{
    [SerializeField] UIDocument _ui = null;
    [SerializeField] Camera _camera = null;
    [SerializeField] Transform _pivotNode = null;
    [SerializeField] Transform _distanceNode = null;

    void Start()
    {
        var drag = new TouchDragManipulator();
        drag.OnDragging += OnDragging;
        drag.OnScrolling += OnScrolling;

        var area = _ui.rootVisualElement;
        area.AddManipulator(drag);
    }

    void OnDragging(float2 delta)
    {
        var rot = (float3)_pivotNode.localEulerAngles;
        var limit = math.float2(40, 60);
        rot.xy = (rot.xy + 180) % 360 - 180;
        rot.xy = math.clamp(rot.xy + delta.yx * 90, -limit, limit);
        _pivotNode.localEulerAngles = rot;
    }

    void OnScrolling(float delta)
    {
        var dist = _distanceNode.localPosition.z;
        dist = math.clamp(dist + 4 * delta, -5, -2);
        _distanceNode.localPosition = new float3(0, 0, dist);
    }

    void Update()
    {
        var dist = _distanceNode.localPosition.z;
        _camera.fieldOfView = 20 + (-dist - 2);
    }
}

} // namespace MultiTouchTest
