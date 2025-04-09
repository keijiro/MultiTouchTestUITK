using UnityEngine;
using UnityEngine.UIElements;
using Unity.Mathematics;

namespace MultiTouchTest {

public sealed class CameraController : MonoBehaviour
{
    #region Scene object references

    [SerializeField] UIDocument _ui = null;
    [SerializeField] Camera _camera = null;
    [SerializeField] Transform _pivotNode = null;
    [SerializeField] Transform _distanceNode = null;

    #endregion

    #region Camera controlling parameters

    [SerializeField] float2 _angleLimit = math.float2(60, 180);
    [SerializeField] float _angleSpeed = 90;
    [SerializeField] float2 _distanceLimit = math.float2(3, 6);
    [SerializeField] float _distanceSpeed = 4;
    [SerializeField] float2 _fovRange = math.float2(20, 45);

    #endregion

    #region TouchDragManipulator callbacks

    void OnDragging(float2 delta)
    {
        var r = (float3)_pivotNode.localEulerAngles;
        r.xy = (r.xy + 180) % 360 - 180; // (0, 360) => (-180, 180)
        r.xy += delta.yx * _angleSpeed;
        r.xy = math.clamp(r.xy, -_angleLimit, _angleLimit);
        _pivotNode.localEulerAngles = r;
    }

    void OnScrolling(float delta)
    {
        var dist = _distanceNode.localPosition.z;
        dist += _distanceSpeed * delta;
        dist = math.clamp(dist, -_distanceLimit.y, -_distanceLimit.x);
        _distanceNode.localPosition = new float3(0, 0, dist);
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        var drag = new TouchDragManipulator();
        drag.OnDragging += OnDragging;
        drag.OnScrolling += OnScrolling;
        _ui.rootVisualElement.AddManipulator(drag);
    }

    void Update()
    {
        var dist = -_distanceNode.localPosition.z;
        var ndist = (dist - _distanceLimit.x) / (_distanceLimit.y - _distanceLimit.x);
        _camera.fieldOfView = math.lerp(_fovRange.x, _fovRange.y, ndist);
    }

    #endregion
}

} // namespace MultiTouchTest
