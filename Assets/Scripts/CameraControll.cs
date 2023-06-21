using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CamControl
{
    public class CameraControll : MonoBehaviour
    {
        [Header("Framing")]
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _followtransform;
        [SerializeField] private Vector3 _framing = new Vector3(0, 0, 0);

        [Header("Distance")]
        [SerializeField] private float _zoomSpeed = 500f;
        [SerializeField] private float _defaultDistance = 1f;
        [SerializeField] private float _minDistance = 0f;
        [SerializeField] private float _maxDistance = 5000f;

        [Header("Rotation")]
        [SerializeField] private float _defaultVerticalAngle = 20f;
        [SerializeField] private float _rotationSharpness = 25f;
        [SerializeField][Range(-90, 90)] private float _minVerticalAngle = -90;
        [SerializeField][Range(-90, 90)] private float _maxVerticalAngle = 90;


        //Pri
        private Vector3 _planarDirection;
        private float _targetDistance;
        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private float _targetVerticalAngle;

        private Vector3 _newPostion;
        private Quaternion _newRotation;

        private void OnValidate()
        {
            _defaultDistance = Mathf.Clamp(_defaultDistance, _minDistance, _maxDistance);
            _defaultVerticalAngle = Mathf.Clamp(_defaultVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
        }

        public void Active()
        {
            //important
            _planarDirection = _followtransform.forward;

            //Calculate
            _targetDistance = _defaultDistance;
            _targetVerticalAngle = _defaultVerticalAngle;
            _targetRotation = Quaternion.LookRotation(_planarDirection) * Quaternion.Euler(_targetVerticalAngle, 0, 0);
            _targetPosition = _followtransform.position - (_targetRotation * Vector3.forward) * _targetDistance;

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            bool _isEsc = CameraInput.EscInput;
            if (_isEsc)
                Cursor.lockState = CursorLockMode.None;
                
            if (Cursor.lockState != CursorLockMode.Locked)
                return;

            //Input
            float _zoom = -CameraInput.MouseScrollInput * _zoomSpeed;
            float _mouseX = CameraInput.MouseXInput;
            float _mouseY = -CameraInput.MouseYInput;

            Vector3 _focusPosition = _followtransform.position + _camera.transform.TransformDirection(_framing);

            _planarDirection = Quaternion.Euler(0, _mouseX, 0) * _planarDirection;
            _targetDistance = Mathf.Clamp(_targetDistance + _zoom, _minDistance, _maxDistance);
            _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle + _mouseY, _minVerticalAngle, _maxVerticalAngle);
            Debug.DrawLine(_camera.transform.position, _camera.transform.position + _planarDirection, Color.red);

            //Final target
            _targetRotation = Quaternion.LookRotation(_planarDirection) * Quaternion.Euler(_targetVerticalAngle, 0, 0);
            _targetPosition = _focusPosition - (_targetRotation * Vector3.forward) * _targetDistance;
            //Smoothing
            _newRotation = Quaternion.Slerp(_camera.transform.rotation, _targetRotation, Time.deltaTime * _rotationSharpness);
            _newPostion = Vector3.Lerp(_camera.transform.position, _targetPosition, Time.deltaTime * _rotationSharpness);

            //Apply
            _camera.transform.rotation = _newRotation;
            _camera.transform.position = _newPostion;

        }
    }
}

