using UnityEngine;

public class PlayerCameraRotation : MonoBehaviour //убрать и переделать архитектуру под это
{
    [SerializeField] private Transform _playerPoint;

    private Vector3 _reqiuredRotation;

    public Vector3 ReqiuredPosition => new Vector3(_playerPoint.position.x + 0.753f, _playerPoint.position.y + 1.787f, _playerPoint.position.z - 1.084f);

    private void Awake()
    {
        _reqiuredRotation = transform.rotation.eulerAngles;

        transform.SetParent(null);

        transform.position = ReqiuredPosition;
    }

    private void LateUpdate()
    {
        CalibrateInUpdate();
    }

    private void CalibrateInUpdate()
    {
        transform.rotation = Quaternion.Euler(_reqiuredRotation);
        transform.position = ReqiuredPosition;
    }
}
