using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AnimationState {
    Max, 
    Min,
};

public class CameraDemoSway : MonoBehaviour
{
    public Transform target;
    public Vector3 swayMin;
    public Vector3 swayMax;

    [Range(0.01f, 10)]
    public float swayDuration = 1f;

    private Vector3 _startingOffset;

    private AnimationState _state = AnimationState.Max;
    private Vector3 _stateStartingPosition;
    private float _stateStartingTime;

    // Start is called before the first frame update
    void Start() {
        _startingOffset = this.transform.position - target.position;
        _stateStartingPosition = this.transform.position;
        _stateStartingTime = Time.time;
    }

    // Update is called once per frame
    void Update() {
        Vector3 targetPosition = TargetPositionForState(_state);
        float progress = Mathf.Clamp01((Time.time - _stateStartingTime) / swayDuration);
        this.transform.position = Vector3.Lerp(_stateStartingPosition, targetPosition, EaseInOutQuad(progress));
        this.transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);

        if (progress > 0.99f) {
            _state = NextState(_state);
            _stateStartingPosition = this.transform.position;
            _stateStartingTime = Time.time;
        }
    }

    private float EaseInOutQuad(float x) {
        return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
    }

    private AnimationState NextState(AnimationState state) {
        switch(state) {
            case AnimationState.Max:
                return AnimationState.Min;
            case AnimationState.Min:
                return AnimationState.Max;
        }
        return AnimationState.Max;
    }

    private Vector3 TargetPositionForState(AnimationState state) {
        switch (state) {
            case AnimationState.Max:
                return _startingOffset + target.position + swayMax;
            case AnimationState.Min:
                return _startingOffset + target.position + swayMin;
        }
        return _startingOffset + target.position;
    }
}
