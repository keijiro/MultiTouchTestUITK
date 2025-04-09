using UnityEngine;
using UnityEngine.UIElements;
using Unity.Mathematics;
using Klak.Math;

namespace MultiTouchTest {

public sealed class CameraController : MonoBehaviour
{
    #region Scene object references

    [SerializeField] UIDocument _ui = null;
    [SerializeField] Camera _camera = null;
    [SerializeField] Transform _pivotNode = null;
    [SerializeField] Transform _slideNode = null;

    #endregion

    #region Camera controlling parameters

    [Space]
    [field:SerializeField] public float RotationSpeed = 2;
    [field:SerializeField] public float PitchLimit = 1;
    [Space]
    [field:SerializeField] public float ZoomSpeed = 0.5f;
    [field:SerializeField] public float2 DistanceRange = math.float2(3, 6);
    [Space]
    [field:SerializeField] public float2 FovRange = math.float2(20, 30);
    [Space]
    [field:SerializeField] public float TweenSpeed = 5;

    #endregion

    #region Transform parameters

    float2 _rotation;
    (float target, float current) _zoom;

    #endregion

    #region TouchDragManipulator callbacks

    void OnDragging(float2 delta)
    {
        _rotation += delta.yx * RotationSpeed;
        _rotation.x = math.clamp(_rotation.x, -PitchLimit, PitchLimit);
        _zoom.target = math.max(_zoom.target, 0.3334f);
    }

    void OnScrolling(float delta)
      => _zoom.target = math.saturate(_zoom.target - ZoomSpeed * delta);

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
        _zoom.current = ExpTween.Step(_zoom.current, _zoom.target, TweenSpeed);

        var amp = math.saturate(_zoom.current * 3);

        if (_zoom.current < 0.05f) _rotation = 0;

        var rot = quaternion.Euler(math.float3(_rotation * amp, 0));
        _pivotNode.localRotation =
          ExpTween.Step(_pivotNode.localRotation, rot, TweenSpeed);

        _slideNode.localPosition =
          math.float3(0, 0, -math.lerp(DistanceRange.x, DistanceRange.y, _zoom.current));

        _camera.fieldOfView = math.lerp(FovRange.x, FovRange.y, _zoom.current);
    }

    #endregion
}

} // namespace MultiTouchTest
