using UnityEngine;

[RequireComponent(typeof(InputController))]
public abstract class Bootstrap : MonoBehaviour
{
    public abstract void Initialize(); //
}

//инкапсуляция ли это, если я пишу методы загрузки конкретных сцен, а не позволяю это делать в "свободном" режиме через локальную переменную?