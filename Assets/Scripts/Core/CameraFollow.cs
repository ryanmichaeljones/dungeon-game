using UnityEngine;

namespace Assets.Scripts
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private float _smoothTime = 0.15f;
        [SerializeField] private float _cameraHeight = 15f;

        private Transform _target;
        private Vector3 _velocity = Vector3.zero;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 targetPos = new Vector3(_target.position.x, _cameraHeight, _target.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, _smoothTime);
        }
    }
}
