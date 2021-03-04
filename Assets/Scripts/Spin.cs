using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float speed = 5;
    public Vector3 axis = Vector3.up;
    public float maxAngle = 0;

    [Range(0, 1)]
    public float lerp = 1;

    private Quaternion _startingRotation;
    private bool _goingUp = true;

    private void Start() {
        _startingRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update() {
        float goingUpF = (_goingUp ? 1 : -1);
        Quaternion toRotate = transform.localRotation * Quaternion.Euler(goingUpF * axis * speed * Time.deltaTime);

        if (maxAngle != 0) {
            Quaternion edgeBottom = Quaternion.Euler(axis * -maxAngle);
            Quaternion edgeTop = Quaternion.Euler(axis * maxAngle);
            float angleTop = Quaternion.Angle(transform.localRotation, edgeTop) / maxAngle;
            float angleBottom = Quaternion.Angle(transform.localRotation, edgeBottom) / maxAngle;

            transform.localRotation = Quaternion.Lerp(transform.localRotation, toRotate, Mathf.Clamp((1 - lerp) + angleTop * angleBottom, 0.05f, 1));

            if (angleTop > 2) {
                _goingUp = true;
            }

            if (angleBottom > 2) {
                _goingUp = false;
            }

            Quaternion edgeSmall = Quaternion.Euler(axis * maxAngle);
            Quaternion edgeLarge = Quaternion.Euler(axis * -maxAngle);


        } else {
            float angle = Quaternion.Angle(transform.localRotation, _startingRotation) / 180;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, toRotate, Mathf.Clamp((1 - lerp) + angle, 0.05f, 1));
        }
    }
}
