// Makes an object spin and glow

using UnityEngine;

[RequireComponent(typeof(Material))]
public class PickableObjectBehaviour : MonoBehaviour {
    [SerializeField] [Range(0, 5)] float minLightIntensity = 1.5f;
    [SerializeField] [Range(0, 5)] float maxLightIntensity = 2.5f;

    [Header("Animation Settings")]
    [SerializeField] [Range(0, 100)] float rotationSpeed = 25f;
    [SerializeField] [Range(0, 1)] float animationSpeed = 0.5f;
    const float _verticalDistanceModifier = 0.1f;
    const string _matPropretyName = "_RingAmp";
    const float _matPropretyMin = 0f, _matPropretyMax = 5f;
    float _curveValueRemaped, _animationTime;
    Vector3 _initialPos;
    Light _light;
    AnimationCurve _curve;
    Material _material;
    void Start() {
        // Setup Animation Curve
        _curve = new AnimationCurve();
        _curve.AddKey(new Keyframe(0, 1, 0, 0));
        _curve.AddKey(new Keyframe(1, 1, 0, 0));
        _curve.AddKey(new Keyframe(0.5f, -1, 0, 0));

        // Get components for object's light and material
        _light = GetComponentInChildren<Light>();
        _material = GetComponent<Renderer>().material;

        // Initialize the variables
        _initialPos = transform.position;
        _light.intensity = minLightIntensity;
        _animationTime = 0;
    }
    void Update() {
        // Value on the animation curve remaped to 0 and 1
        _curveValueRemaped = _curve.Evaluate(_animationTime).Remap(-1f, 1, 0f, 1f);

        // Update the object's rotation, position, light intensity, and emission color
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        transform.position = _initialPos + Vector3.up * _curve.Evaluate(_animationTime) * _verticalDistanceModifier;
        _light.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, _curveValueRemaped);
        _material.SetFloat(_matPropretyName, _light.intensity.Remap(minLightIntensity, maxLightIntensity, _matPropretyMin, _matPropretyMax));

        // Update the animation time variable
        _animationTime += animationSpeed * Time.deltaTime;
        if (_animationTime > 1)
            _animationTime = 0;
    }
}
