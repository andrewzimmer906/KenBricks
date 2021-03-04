using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PanAnimationState {
    Max,
    Min,
};

public class CameraDemoPan : MonoBehaviour {
    public Vector3 panMax;
    public Vector3 panMin;

    [Range(0.01f, 30)]
    public float panDuration = 1f;

    private Vector3 _startingPosition;

    private PanAnimationState _state = PanAnimationState.Max;
    private Vector3 _stateStartingPosition;
    private float _stateStartingTime;

    // Start is called before the first frame update
    void Start() {
        _startingPosition = this.transform.position;
        _stateStartingPosition = this.transform.position;
        _stateStartingTime = Time.time;
    }

    // Update is called once per frame
    void Update() {
        Vector3 targetPosition = TargetPositionForState(_state);
        float progress = Mathf.Clamp01((Time.time - _stateStartingTime) / panDuration);
        this.transform.position = Vector3.Lerp(_stateStartingPosition, targetPosition, EaseInOutQuad(progress));

        if (progress > 0.99f) {
            _state = NextState(_state);
            _stateStartingPosition = this.transform.position;
            _stateStartingTime = Time.time;
        }
    }

    private float EaseInOutQuad(float x) {
        return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
    }

    private PanAnimationState NextState(PanAnimationState state) {
        switch (state) {
            case PanAnimationState.Max:
                return PanAnimationState.Min;
            case PanAnimationState.Min:
                return PanAnimationState.Max;
        }

        return PanAnimationState.Max;
    }

    private Vector3 TargetPositionForState(PanAnimationState state) {
        switch (state) {
            case PanAnimationState.Max:
                return _startingPosition + panMax;
            case PanAnimationState.Min:
                return _startingPosition + panMin;
        }
        return _startingPosition;
    }
}
