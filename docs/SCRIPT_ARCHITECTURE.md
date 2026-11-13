# Архитектура скриптов SPIKE

Документ описывает код в [`Assets/_GameFiles/Scripts`](../Assets/_GameFiles/Scripts) и его подключения в сценах и префабах SPIKE. Выводы основаны на статическом чтении C# и сериализованных ассетов Unity. Поведение в запущенной игре и сборка на Android требуют отдельной проверки. `ECS` находится вне области анализа. Игровые требования приведены в [ТЗ](GAME_DESIGN.md).

Целевые границы слоёв и решения по рефакторингу зафиксированы в [описании луковичной архитектуры](ONION_ARCHITECTURE.md).

## Точки входа

В [настройках сборки](../ProjectSettings/EditorBuildSettings.asset) включены две сцены. Порядок важен, поскольку `SceneLoading` загружает сцены по индексу.

| Сцена | Подключение и назначение |
| --- | --- |
| [MainMenu](../Assets/_GameFiles/Scenes/MainMenu/MainMenu.unity), индекс `0` | `BootstrapMainMenuScene` находит `MainMenuUI` и `AudioSource`, подключает кнопку запуска, окно настроек и слайдер громкости. Bootstrap создаёт `SceneLoading` со ссылкой на `SoundDesign`. Кнопка запуска через этот экземпляр открывает `FirstLevel`. `SoundPlayer` сохраняет свой объект при смене сцены через `DontDestroyOnLoad`. |
| [FirstLevel](../Assets/_GameFiles/Scenes/FirstLevel/FirstLevel.unity), индекс `1` | [Префаб Bootstrap](../Assets/_GameFiles/Prefabs/Bootstrap.prefab) содержит `BootstrapTestScene`, `InputController` и визуализатор зон overlap. Сцена подставляет ссылки на игрока и другие объекты через переопределения префабов. `BootstrapTestScene` связывает управление, игрока, паузу и кнопки сохранения и загрузки. |
| [Test](../Assets/_GameFiles/Scenes/Test/Test.unity), вне списка сборки | Отдельная сцена с собственными подключениями, включая компонент старой системы сохранений. Для проверки штатного маршрута от меню к миссии ориентируйся на `MainMenu` и `FirstLevel`. |

Bootstrap уровня находит переживший переход `SoundPlayer` и передаёт его объект своему экземпляру `SceneLoading`. Тот же экземпляр получает `UIController`, а интерактор сохранения получает его метод `LoadLevelScene`. При выходе из паузы в главное меню или возврате с экрана победы или поражения `SceneLoading.LoadMainMenuScene()` отключает и удаляет текущий `SoundDesign` перед загрузкой `MainMenu`. Музыку запускает новый `MusicLayer` из сцены меню. `LoadLevelScene()` сохраняет текущий источник музыки, в том числе при перезагрузке уровня.

В [ProjectVersion.txt](../ProjectSettings/ProjectVersion.txt) записана Unity `6000.3.9f1`. В [manifest.json](../Packages/manifest.json) указаны URP `17.3.0`, Input System `1.18.0` и Cinemachine `3.1.6`. В [ProjectSettings.asset](../ProjectSettings/ProjectSettings.asset) записаны `AndroidMinSdkVersion: 25` и `AndroidTargetSdkVersion: 0`. Android 14 указан в ТЗ как целевая среда.

Для прямого управления открытым редактором подключён официальный пакет `com.unity.pipeline` версии `1.0.0-pre.1`. Он предоставляет локальное соединение для Unity CLI. Подключение проверено чтением списка открытых сцен, редактор вернул активную сцену `FirstLevel`. Команды подключения приведены в [AGENTS.md](../AGENTS.md). Проверка соединения выполнялась в режиме редактирования.

## Управление и главный герой

Цепочка команд игрока проходит через `InputReader` → `InputController` → обработчики в `BootstrapTestScene` → `PlayerController` → `Player`.

- [`InputController`](../Assets/_GameFiles/Scripts/ServicesGeneral/Input/InputController.cs) создаёт `InputReader`, читает направление в `Update` и публикует события движения, атаки, блока, трансформации и паузы. Состояние кнопки бега читается при обработке движения. `InputReader` генерируется из [InputActionMapSystemDefault.inputactions](../Assets/_GameFiles/InputActionMapSystems/InputActionMapSystemDefault.inputactions). В карте `MainCharacter` есть привязки клавиатуры, мыши и геймпада. Глобальный ассет действий в `EditorBuildSettings` — [InputActionMapSystemInEngine.inputactions](../Assets/_GameFiles/InputActionMapSystems/InputActionMapSystemInEngine.inputactions).
- [`PlayerController`](../Assets/_GameFiles/Scripts/Player/PlayerController.cs) — компонент на [префабе игрока](../Assets/_GameFiles/Prefabs/MainCharacters/Player%20%28MainCharacter%29.prefab). В `Awake` он создаёт модель `Player`, контроллеры подсистем и `PlayerView` из сериализованных ссылок. Он передаёт команды модели и подключает события к представлению. `PlayerView` обновляет трансформы, `PlayerAnimator`, `PlayerUI`, звук и частицы. `PlayerController` подписывает своё представление на уведомления экземпляров `Idled`, `Locomoted`, `Runned` и `Rotated` в `OnEnable` и снимает подписки в `OnDisable`. `PlayerMechanicIdleState` напрямую вызывает публичный делегат `player.Idled`.
- [`Player`](../Assets/_GameFiles/Scripts/Player/Player.cs) передаёт команды в `PlayerMechanicStateMachine`. Допустимость перехода задаёт `TryExit` текущего состояния. Состояния лежат в `Player/PlayerMechanicStateMachine`. Контроллер вызывает `MechanicStateUpdate()` каждый кадр, а модель вызывает `DoWithinFrame` активного состояния.
- Ходьба и бег используют `PlayerMovementController` и `PlayerLocomotion`. Направление движения и поворот учитывают поворот камеры. `PlayerLocomotion` хранит последнюю позицию, которую при загрузке обновляет `PlayerController.SetLastPosition`. Блок проходит через состояние `PlayerMechanicBlockState` в `PlayerHealth`. `PlayerDefenseController` пока пуст.
- Экранная кнопка блока в `GameplayUI` отправляет `<Gamepad>/leftTrigger` в действие `MainCharacter/Block`. Для того же действия доступны пробел и `<Gamepad>/buttonSouth`. `InputController` передаёт нажатие и снимает блок при отпускании, открытии меню, трансформации или отключении боевого ввода.

### Трансформация и полёт

В `FirstLevel` компонент `MainDoors` получает события от триггера с кинематическим `Rigidbody` и распознаёт капсулу игрока по `PlayerController`. Корневой объект корабля сохранён неактивным и выключается в `MainDoors.Awake()`. При выходе игрока из триггера компонент включает корабль целиком и запускает закрытие дверей.

`PlayerView` воспроизводит два звука трансформации и управляет зацикленным звуком двигателя на отдельных `AudioSource` префаба игрока. Микшер `FighterEngine` усиливает только двигатель на 3,52 дБ. При загрузке сохранения восстанавливается звук двигателя для текущей формы. Дальняя плоскость отсечения установлена на 10 000 единиц в линзе Cinemachine, в камере префаба и в переопределении камеры в `FirstLevel`.

`InputController` принимает действие `Transform` с клавиатуры, геймпада и экранного `OnScreenButton` из префаба `GameplayUI`. `BootstrapTestScene` вызывает `PlayerController.ToggleFighterForm` и по `FighterFormChanged` обновляет боевой ввод и экранные элементы. `PlayerView` переключает HUD и две модели. Контроллеры противников по тому же событию скрывают шкалы здоровья и жизней. Истребитель использует вложенный префаб `Space Ship 2` без компонентов движения и звука фонового корабля. Поворот модели задан внутри `FighterPivot`. В полёте `PlayerView` настраивает позицию и дистанцию Cinemachine относительно этого pivot и восстанавливает параметры человека при выходе из формы.

Вход в `PlayerMechanicFlightState` проходит через `TryExit` текущего состояния игрока. Состояние полёта снимает блок при входе и допускает переход в `Idle`. В полёте `Player` отклоняет урон, а `InputController` подавляет атаки и блок. `PlayerMovementController` вычисляет направление полёта по полным векторам камеры. `PlayerLocomotion` проверяет путь укороченной капсулой с нижней точкой на уровне центра коллайдера, чтобы зона у ног человека не мешала взлёту с площадки. При падении и проверке опоры используется полная капсула.

При обратной трансформации `PlayerController` сразу возвращает человека в `Idle`. `Player.MechanicStateUpdate` обновляет гравитацию в `PlayerLocomotion` каждый кадр в человеческой форме. Проверка капсулой определяет опору и ограничивает смещение вниз при приземлении. Пока человек падает, обработчики ходьбы и бега пропускают смещение по горизонтали. При старте, загрузке и выходе из формы истребителя `PlayerController` проверяет опору сразу. При загрузке сохранённая скорость падения применяется, если человек в воздухе. Новая трансформация прерывает падение.

На объекте `FlightLimiter` в `FirstLevel` стоят триггер и кинематический `Rigidbody` для обработки касания игроком. `PlayerLocomotion` также проверяет пересечение триггера по всей траектории шага полёта и передаёт найденный ограничитель в `PlayerController`. Оба пути вызывают `FlightLimiter.Reach`. При первом касании он запрещает трансформацию через `PlayerController.LockFighterForm`, возвращает истребитель в форму человека и запускает появление рядовых противников.

`Road Event Wall 1` использует кинематический `Rigidbody` и распознаёт игрока по `PlayerController`. Он запускает три компонента `ObjectSpawner1`, `ObjectsSpawner2` и `ObjectSpawner3` по ссылкам из сцены. Интервал между началами партий задаётся отдельно на каждом спавнере полем `Spawn Cooldown` и по умолчанию равен пяти секундам игрового времени. `FlightLimiter.Reach` отменяет следующие партии и запланированные корабли текущей партии. Восстановленный запрет полёта также блокирует запуск спавна за ограничителем.

При загрузке `PlayerController.SetHealthValue()` обновляет здоровье и HUD без анимации попадания. В форме истребителя `PlayerController.Idle()` игнорирует события завершения человеческих анимаций. Явный выход из полёта переводит модель в `Idle` напрямую и снова включает проверку опоры.

## Противники и появление босса

[`CharacterControllerNew`](../Assets/_GameFiles/Scripts/Character/CharacterControllerNew.cs) — базовый `MonoBehaviour` для противников. Он находит игрока, создаёт `Character`, `CharacterView` и контроллеры здоровья, движения и атак. Механические состояния задают допустимые действия, а конкретные контроллеры противников выбирают команды в `Update`.

| Контроллер | Логика поведения |
| --- | --- |
| [`CharacterControllerNewCloseRange`](../Assets/_GameFiles/Scripts/Character/Implemented%20Character/CharacterControllerNewCloseRange.cs) | Сближается с игроком. В радиусе ближнего боя чередует блок и удары с задержкой между атаками. |
| [`CharacterControllerNewLongRange`](../Assets/_GameFiles/Scripts/Character/Implemented%20Character/CharacterControllerNewLongRange.cs) | При дистанции меньше трёх единиц отходит в заданном направлении. Снаружи этой зоны поворачивается к игроку и запрашивает выстрел один раз до сброса счётчика при следующем сближении. |
| [`CharacterControllerNewBoss`](../Assets/_GameFiles/Scripts/Character/Implemented%20Character/CharacterControllerNewBoss.cs) | Выбирает преследование, дистанционную атаку, удар и блок в открытой фазе. Для ближней фазы включает отдельный режим поведения и ограничители арены. |

В [`FirstLevel`](../Assets/_GameFiles/Scenes/FirstLevel/FirstLevel.unity) `FlightLimiter` ссылается на `SpawnerCharacterCloseRange` и `SpawnerCharacterLongRange`. Компонент `CharacterFactory` создаёт врага по явному вызову. Ограничитель создаёт по одному противнику на заданных в сцене точках. Защита от повторного спавна использует сохранённый в игроке запрет трансформации. В сцене `Test` для автоматического спавна используется `CharacterAutoSpawnFactory`.

[`ScoreController`](../Assets/_GameFiles/Scripts/ServicesGeneral/Score/ScoreController.cs) — обычный C#-класс, который Bootstrap создаёт на время попытки. Фабрика публикует `CharacterCreated`, а Bootstrap связывает `Died` рядового и `PhasePairCompleted` босса со счётчиком. Счётчик определяет, доступен ли босс. Bootstrap применяет это состояние к объекту сцены, в том числе при восстановлении сохранения.

## Попадания, здоровье и фазы

В `PlayerController.Awake` при сборке `Player` создаются контроллер оружия, обёртки ближнего и дальнего оружия, обе атаки, их анализаторы попаданий и калькуляторы урона. `PlayerOffenseController` получает готовый контроллер оружия и обе атаки через конструктор. `PlayerWeaponController.Initialize` принимает обёртки и передаёт им компоненты `Weapon` из сериализованных ссылок игрока.

1. Состояние атаки вызывает `PlayerOffenseController` или `CharacterOffenseController`. Те передают выбранный экземпляр `Weapon` в реализацию `PlayerAttack` или `CharacterAttack`.
2. Реализация атаки отправляет `Attacked` для анимации и эффектов, затем ищет получателя урона через [`EnvironmentAreaOverlapAnalyzer`](../Assets/_GameFiles/Scripts/ServicesGeneral/EnvironmentAreaAnalyzers/EnvironmentAreaOverlapAnalyzer.cs). Для обеих дистанций используется `Physics.OverlapSphere`. Анализатор берёт первый подходящий коллайдер с `IDamageable`, пропуская компоненты исключаемого типа. `EnvironmentAreaOverlapAnalyzerRender` подписывается на события анализатора.
3. `DamageCalculatorBasic` возвращает `Weapon.Damage`, затем вызывается `IDamageable.TakeDamage()`. Урон и дальность задаются в компонентах `Weapon`. В префабе игрока у меча урон переопределён на `4`, у дальнего оружия `RocketLauncher` — на `1`. Эффекты, звук и индикатор дальнего оружия управляются `PlayerView` и `PlayerUI`. Индикатор обновляется с задержками `Task.Delay`. Допуск к атаке в просмотренном пути определяется переходами состояний.

В `FirstLevel` группа `Weapon Pickups` возле спавна содержит четыре экземпляра префабов оружия. Каждый `PlayerWeaponPickup` использует сферический триггер и кинематический `Rigidbody`. При входе коллайдера игрока компонент выключает одну модель в его скелете и включает другую. Если игрок загружен внутри зоны подбора, эта зона снова переключит оружие при следующем входе после выхода. Каждая точка ссылается на свою пару моделей. `PlayerController` инициализирует атаки компонентами `SwordBasic` и `RocketLauncherBasic`. Атаки продолжают читать их характеристики при выключенных объектах `Sword` и `RocketLauncher`.
4. `PlayerHealth` и `CharacterHealth` хранят текущее здоровье и флаг блока. В обеих реализациях `TakeDamage` вычитает урон в нескольких ветках одного вызова. Если меняется баланс, сначала стоит проверить фактическое снятие здоровья от одиночного попадания.

При обычном попадании экземпляр `PlayerHealth` публикует `DamageTaken` с текущим здоровьем. `PlayerController` подключает `PlayerView.PresentDamageTake` в `OnEnable` и снимает подписку в `OnDisable`.

[`PlayerHealth.Die()`](../Assets/_GameFiles/Scripts/Player/PlayerHealth/PlayerHealth.cs) начинает с `Lives = 3`. Пока счётчик больше нуля, метод восстанавливает здоровье, уменьшает счётчик и уведомляет HUD через `LivesChanged`. При вызове с уже нулевым счётчиком он отправляет `Died`. По этому коду экран поражения открывается при следующем исчерпании здоровья после перехода `1 → 0`.

`PlayerMechanicStunState` отклоняет движение и боевые действия во время оглушения. Обычно событие `Idle` в клипе `Stun` возвращает управление. Если событие не сработало, состояние возвращает игрока в `Idle` через 1,35 секунды игрового времени с момента входа. Пауза останавливает отсчёт.

У босса `_currentGlobalLife` считает исчерпанные полоски здоровья. Порог `globalLives` по умолчанию равен `6`. `OnBossDeath` при промежуточной потере полоски переключает фазу и восстанавливает здоровье. На отметках `2`, `4` и `6` начисляется очко и обновляется шкала глобальных жизней босса. При достижении порога босс публикует `Defeated`. В просмотренных обработчиках жизней игрока и фаз босса отсутствует взаимное восстановление жизней из ТЗ.

`ActivatePhaseTwo()` снимает блок, ищет свободную позицию рядом с игроком и после телепорта обновляет исходную позицию в `CharacterLocomotion`. Компонент [`Limiters`](../Assets/_GameFiles/Scripts/ServicesGeneral/Limiters.cs) центрирует границы по их `BoxCollider` между персонажами и при необходимости расширяет зону под обе капсулы. Босс отсоединяет ограничители от себя перед активацией. При возврате в открытую фазу он выключает их и снова прикрепляет к себе.

`HandlePhaseTwoBehaviour()` отсчитывает 10 секунд боя и 3 секунды восстановления по игровому времени. В бою босс проверяет траекторию своего шага капсулой и использует `meleeRange` и для сближения, и для начала атаки. В начале восстановления контроллер снимает блок и переводит босса в `Idle`. Удары игрока проходят через обычный обработчик урона и во время боя, и во время восстановления.

## Пауза, экраны и очки

- [`UIController`](../Assets/_GameFiles/Scripts/UIGeneral/UIController.cs) связывает меню паузы, итоговые экраны, ввод и сохранение. Bootstrap подписывает его на `Defeated` игрока и босса. При победе событие приходит после начисления бонуса за последнюю пару фаз. Контроллер останавливает время, блокирует игровой ввод и передаёт итоговый счёт панели. Кнопки итоговых экранов используют `SceneLoading`.
- `ScoreController` хранит очки за рядовых и бонусы за пары фаз босса. Босс публикует `PhasePairCompleted`, а счётчик начисляет бонус. Bootstrap подключает HUD к `ScoreChanged` и передаёт начальное значение. `WinUI` и `LoseUI` получают итоговое число при открытии. `GameSaveLoadInteractor` получает `ScoreController` от Bootstrap и сохраняет только очки за рядовых.
- В основном меню подключён слайдер громкости музыки. В проверенных обработчиках и подключениях нет действия выхода из приложения, окна настроек в паузе и сохранения громкости между запусками.

## Сохранения

Штатный маршрут `FirstLevel` проходит через [`GameSaveLoadInteractor`](../Assets/_GameFiles/Scripts/ServicesGeneral/SaveAndLoad/NewSaveLoadSystem/GameSaveLoadInteractor.cs). Bootstrap в `Awake` создаёт `JsonPlayerRepository` для `player_save.json` и `JsonCharacterRepository` для `characters_save.json` и передаёт их интерактору вместе со счётчиком. Bootstrap создаёт `SceneSaveState` и интерактор в `Awake`, а в `Start` фиксирует начальную позицию игрока и явно запускает загрузку. Интерактор задаёт порядок восстановления, а адаптер собирает и применяет данные объектов Unity. Кнопка «Сохранить» в паузе записывает два файла через `JsonUtility` в `Application.persistentDataPath`. Кнопка загрузки в паузе перезапускает уровень при наличии файла игрока. При победе `UIController` вызывает очистку через интерактор. Интерактор удаляет оба файла и блокирует повторную запись в завершённой попытке.

| Файл | Поля |
| --- | --- |
| `player_save.json` | `PlayerSaveData` хранит позицию и поворот корня, поворот модели, текущее здоровье, очки за рядовых врагов, форму, запрет трансформации, факт пересечения `FlightLimiter`, флаг и скорость падения, выбор ножа и пистолета. |
| `characters_save.json` | `SavedCharactersData` хранит тип контроллера, позицию, поворот и здоровье рядовых врагов. Запись `Boss` хранит тип, позицию и поворот активного босса. |

`GameSaveLoadInteractor` восстанавливает бой и очки за рядовых врагов по сохранённому факту пересечения `FlightLimiter`. Для старого файла без этого признака проверяет запрет трансформации и положение игрока со стороны арены. До пересечения счёт начинается с нуля. После касания при нулевом счёте восстанавливаются оба противника, при одном очке — оставшийся по типу из сохранения. Старые записи без типа сопоставляются по порядку. При одном очке и неизвестном типе оставшегося противника загружается ближний. При загрузке старых файлов счёт ограничивается двумя очками, а из старой записи босса используются позиция и поворот. Босс начинает с полной полоской здоровья, первой фазы и начального счётчика глобальных жизней. Для старого файла без выбора оружия используются катана и бластер. `FirstLevel` подставляет в `PlayerController` ссылки на те же модели ножа и пистолета, которые переключают триггеры подбора.

В [`OldSaveLoadSystem 1`](../Assets/_GameFiles/Scripts/ServicesGeneral/SaveAndLoad/OldSaveLoadSystem%201) есть другой формат для позиции и поворота игрока. В [`GameplayUI.prefab`](../Assets/_GameFiles/Prefabs/UI/UILayouts/GameplayUI.prefab) остались сериализованные вызовы его `SaveData` и `LoadData` с пустыми целями. Рабочие обработчики кнопок в `FirstLevel` назначает `BootstrapTestScene`.

## Где начинать изменения

| Задача | Первые места для проверки |
| --- | --- |
| Управление на Android, кнопки, бег, блок | Карта `MainCharacter`, `InputController`, подписки в `BootstrapTestScene`, переходы в состояниях `Player`. В паузе `InputController` приостанавливает игровой ввод. |
| Дальность, урон, кулдаун, варианты оружия | `PlayerOffenseController`, `CharacterOffenseController`, реализации атак, `EnvironmentAreaOverlapAnalyzer`, префабы оружия и переопределения в префабе игрока. `WeaponType` у противников переключает видимые варианты в `Awake`. `PlayerWeaponPickup` переключает модели игрока при входе в триггеры в `FirstLevel`. |
| Появление босса, фазы, глобальные жизни | `FirstLevel`, `ScoreController`, `CharacterControllerNew.Died`, `CharacterControllerNewBoss.OnBossDeath`, `PlayerHealth.Die` и обе шкалы жизней в сцене. |
| Трансформация и полёт | Действие `Transform` в `MainCharacter`, `BootstrapTestScene`, `PlayerMechanicFlightState` и воздушное движение в `PlayerLocomotion`. |
| Пауза, итоговые экраны и сохранения | `BootstrapTestScene`, `GameplayUI.prefab`, `WinUI`, `LoseUI`, `GameSaveLoadInteractor`, DTO и репозитории из `NewSaveLoadSystem`. Проверять восстановление в новой сессии, поскольку часть состояния статическая. |

`GameControllerScenes/GameController.cs` и `EventBusGeneral/EventBusGeneral.cs` пока содержат пустые классы. Координация сцены и событий проходит через bootstrap, ссылки Unity, обработчики в компонентах и события моделей. При изменении подписок проверяй пары `OnEnable` и `OnDisable`. В нескольких компонентах для отписки создаются новые анонимные делегаты. Повторная активация объектов требует проверки этих связей.
