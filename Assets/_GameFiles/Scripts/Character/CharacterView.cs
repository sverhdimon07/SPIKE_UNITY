using System.Threading.Tasks;
using UnityEngine;

public class CharacterView
{
    private readonly CharacterUI _ui;
    private readonly CharacterAnimator _animator;
    private readonly Transform _gameObjectPivot;
    private readonly Transform _renderAndSkeletonPivot;

    // Массивы для хранения всех эффектов и звуков
    private readonly ParticleSystem[] _effects;
    private readonly AudioSource[] _sounds;

    public CharacterView(
        CharacterUI ui,
        CharacterAnimator animator,
        Transform gameObjectPivot,
        Transform renderAndSkeletonPivot,
        ParticleSystem firstEffect,
        ParticleSystem secondEffect,
        ParticleSystem thirdEffect,
        ParticleSystem fourthEffect,
        ParticleSystem fifthEffect,
        ParticleSystem sixthEffect,
        ParticleSystem seventhEffect,
        ParticleSystem eighthEffect,
        ParticleSystem ninthEffect,
        ParticleSystem tenthEffect,
        AudioSource firstSound,
        AudioSource secondSound,
        AudioSource thirdSound,
        AudioSource fourthSound,
        AudioSource fifthSound,
        AudioSource sixthSound,
        AudioSource seventhSound,
        AudioSource eighthSound,
        AudioSource ninthSound,
        AudioSource tenthSound)
    {
        _ui = ui;
        _animator = animator;
        _gameObjectPivot = gameObjectPivot;
        _renderAndSkeletonPivot = renderAndSkeletonPivot;

        // Заполняем массивы
        _effects = new ParticleSystem[]
        {
            firstEffect, secondEffect, thirdEffect, fourthEffect,
            fifthEffect, sixthEffect, seventhEffect, eighthEffect,
            ninthEffect, tenthEffect
        };

        _sounds = new AudioSource[]
        {
            firstSound, secondSound, thirdSound, fourthSound,
            fifthSound, sixthSound, seventhSound, eighthSound,
            ninthSound, tenthSound
        };
    }

    public void PresentIdle() => _animator.PlayIdle();

    public void PresentDamageTake(float valueLevel)
    {
        _ui.RefreshHealthBar(valueLevel);
        _animator.PlayStun();
    }

    public void PresentDeath() { }

    public void MoveCharacterModelInLocomotionForm(Vector3 requiredWorldPosition)
    {
        _gameObjectPivot.position = requiredWorldPosition;
        _animator.PlayLocomotion();
    }

    public void MoveCharacterModelInRunForm(Vector3 requiredWorldPosition)
    {
        _gameObjectPivot.position = requiredWorldPosition;
        _animator.PlayRun();
    }

    public void TurnCharacterModel(Quaternion requiredWorldRotation)
    {
        _renderAndSkeletonPivot.rotation = requiredWorldRotation;
    }

    public async Task PresentCloseRangeAttack()
    {
        _animator.PlayCloseRangeAttack();
        await Task.Delay(100);

        // Случайный эффект и звук
        PlayRandomEffectAndSound();
    }

    public async Task PresentLongRangeAttack()
    {
        _animator.PlayLongRangeAttack();
        await Task.Delay(900);

        // Случайный эффект и звук
        PlayRandomEffectAndSound();
    }

    // Вспомогательный метод для выбора случайного эффекта и звука
    private void PlayRandomEffectAndSound()
    {
        if (_effects.Length > 0)
        {
            int effectIndex = Random.Range(0, _effects.Length);
            _effects[effectIndex]?.Play();
        }

        if (_sounds.Length > 0)
        {
            int soundIndex = Random.Range(0, _sounds.Length);
            _sounds[soundIndex]?.Play();
        }
    }
}
