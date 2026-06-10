using UnityEngine;

public class CharacterControllerNewLongRange : CharacterControllerNew, ILongRangeAttacker
{
    private void Update() //возможно здесь будем корректировать то, куда смотрит ГГ (но возможно это стоит делать не здесь)
    {
        _model.MechanicStateMachine.State.DoWithinFrame(_model);

        if (Vector3.Distance(transform.position, _playerPoint.position) < 3f) //МГ
        {
            _renderAndSkeletonPivot.LookAt(_lookAndLocomotionPoint);

            _isCloseToPlayer = false;

            Locomote(/*ThirdPersonCameraControllerPivot, */new Vector2(_lookAndLocomotionPoint.forward.x, _lookAndLocomotionPoint.forward.z));

            counter = 0;

            return;
        }
        _renderAndSkeletonPivot.LookAt(_playerPoint.position);

        _isCloseToPlayer = true;

        Idle();

        if (counter == 0)
        {
            AttackLongRange(_gameObjectPivot.position, new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));

            counter += 1;
        }
    }
    public void AttackLongRange(Vector3 gameObjectPosition, Vector2 gameObjectRotation)
    {
        _model.AttackLongRange(gameObjectPosition, gameObjectRotation);
        FireEffects.Play();
        FireSound.Play();
    }
}
