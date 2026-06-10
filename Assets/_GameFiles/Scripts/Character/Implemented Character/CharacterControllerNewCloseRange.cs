using UnityEngine;

public class CharacterControllerNewCloseRange : CharacterControllerNew, ICloseRangeAttacker
{
    private void Update() //возможно здесь будем корректировать то, куда смотрит ГГ (но возможно это стоит делать не здесь)
    {
        _model.MechanicStateMachine.State.DoWithinFrame(_model);

        _gameObjectPivot.LookAt(_playerPoint);

        if (Vector3.Distance(transform.position, _playerPoint.position) > 1.1f)
        {
            //_gameObjectPivot.LookAt(new Vector3(_playerPoint.position.x, 0f, _playerPoint.position.y));

            _isCloseToPlayer = false;
            Locomote(new Vector2(transform.forward.x, transform.forward.z));
            counter = 0;
            return;
        }
        _isCloseToPlayer = true;
        Idle();

        if (counter == 0)
        {
            AttackCloseRange(_gameObjectPivot.position, new Vector2(_gameObjectPivot.forward.x, _gameObjectPivot.forward.z));
            counter += 1;
        }
    }

    public void AttackCloseRange(Vector3 gameObjectPosition, Vector2 gameObjectRotation)
    {
        _model.AttackCloseRange(gameObjectPosition, gameObjectRotation);
        WindEffects.Play();
        WindSound.Play();
    }
}
