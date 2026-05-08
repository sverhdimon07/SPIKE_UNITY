using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public sealed class PlayerController : MonoBehaviour
{
    [SerializeField] private Image _healthBar; //хотел переносить эти поля в бутстрап, НО почему-бы все поля, не отвечающие за геймплейную логику, не хранить именно здесь (ибо бутстрап должен инитить ГЕЙМДИЗАЙНЕРСКИЕ ДАННЫЕ, зачем их мешать с ссылками на вспомогательные классы?)?
    [SerializeField] private Image _weaponLongRangeCooldownBar;
    [SerializeField] private TMP_Text _dealthMessageText;
    [SerializeField] private Transform _renderAndSkeletonPivot;
    [SerializeField] private Transform _thirdPersonCameraControllerPivot;

    private Player _model;

    private PlayerView _view;

    public Transform RenderAndSkeletonPivot => _renderAndSkeletonPivot; //ВРЕМЕННАЯ МЕРА; хз порядок свойств (до или после событий);

    private void Awake() //чекнуть конструкторы и деструкторы в монобехах
    {
        _model = new Player(new PlayerMechanicStateMachine(_model), ); //тут такой прикол, что любой человек сможет создавать объект этого класса в любой части программы, но как бы и работать он с ним не сможет без верхнеуровнего монобеховского слоя. Тут все норм, я бы только засинглтонил PlayerController и Player (про PlayerView - хз)
        _view = new PlayerView();
    }

    public void SetRenderAndSkeletonPivot(Quaternion rotation) //?
    {
        _renderAndSkeletonPivot.rotation = rotation;
    }
}
