# Архитектура скриптов SPIKE

Документ описывает код в [`Assets/_GameFiles/Scripts`](../Assets/_GameFiles/Scripts) и его подключения в сценах и префабах SPIKE. Выводы основаны на статическом чтении C# и сериализованных ассетов Unity. Поведение в запущенной игре и сборка на Android требуют отдельной проверки. `ECS` находится вне области анализа. Игровые требования приведены в [ТЗ](GAME_DESIGN.md).

## Точки входа

В [настройках сборки](../ProjectSettings/EditorBuildSettings.asset) включены две сцены. Порядок важен, поскольку `SceneLoading` загружает сцены по индексу.

| Сцена | Подключение и назначение |
| --- | --- |
| [MainMenu](../Assets/_GameFiles/Scenes/MainMenu/MainMenu.unity), индекс `0` | `BootstrapMainMenuScene` находит `MainMenuUI` и `AudioSource`, подключает кнопку запуска, окно настроек и слайдер громкости. Метод `LoadTestScene` вызывает `SceneLoading.LoadLevelScene()`, который открывает `FirstLevel`. `SoundPlayer` сохраняет свой объект при смене сцены через `DontDestroyOnLoad`. |
| [FirstLevel](../Assets/_GameFiles/Scenes/FirstLevel/FirstLevel.unity), индекс `1` | [Префаб Bootstrap](../Assets/_GameFiles/Prefabs/Bootstrap.prefab) содержит `BootstrapTestScene`, `InputController`, `GameSaveLoadInteractor`, `ScoreController` и визуализатор зон overlap. Сцена подставляет ссылки на игрока и другие объекты через переопределения префабов. `BootstrapTestScene` связывает управление, игрока, паузу и кнопки сохранения и загрузки. |
| [Test](../Assets/_GameFiles/Scenes/Test/Test.unity), вне списка сборки | Отдельная сцена с собственными подключениями, включая компонент старой системы сохранений. Для проверки штатного маршрута от меню к миссии ориентируйся на `MainMenu` и `FirstLevel`. |

В [ProjectVersion.txt](../ProjectSettings/ProjectVersion.txt) записана Unity `6000.3.9f1`. В [manifest.json](../Packages/manifest.json) указаны URP `17.3.0`, Input System `1.18.0` и Cinemachine `3.1.6`. В [ProjectSettings.asset](../ProjectSettings/ProjectSettings.asset) записаны `AndroidMinSdkVersion: 25` и `AndroidTargetSdkVersion: 0`. Android 14 указан в ТЗ как целевая среда.

Для прямого управления открытым редактором подключён официальный пакет `com.unity.pipeline` версии `1.0.0-pre.1`. Он предоставляет локальное соединение для Unity CLI. Подключение проверено чтением списка открытых сцен, редактор вернул активную сцену `FirstLevel`. Команды подключения приведены в [AGENTS.md](../AGENTS.md). Проверка соединения выполнялась в режиме редактирования.

## Управление и главный герой

Цепочка команд игрока проходит через `InputReader` → `InputController` → обработчики в `BootstrapTestScene` → `PlayerController` → `Player`.

- [`InputController`](../Assets/_GameFiles/Scripts/ServicesGeneral/Input/InputController.cs) создаёт `InputReader`, читает направление в `Update` и публикует события движения, атаки, блока, трансформации и паузы. Состояние кнопки бега читается при обработке движения. `InputReader` генерируется из [InputActionMapSystemDefault.inputactions](../Assets/_GameFiles/InputActionMapSystems/InputActionMapSystemDefault.inputactions). В карте `MainCharacter` есть привязки клавиатуры, мыши и геймпада. Глобальный ассет действий в `EditorBuildSettings` — [InputActionMapSystemInEngine.inputactions](../Assets/_GameFiles/InputActionMapSystems/InputActionMapSystemInEngine.inputactions).
- [`PlayerController`](../Assets/_GameFiles/Scripts/Player/PlayerController.cs) — компонент на [префабе игрока](../Assets/_GameFiles/Prefabs/MainCharacters/Player%20%28MainCharacter%29.prefab). В `Awake` он создаёт модель `Player`, контроллеры подсистем и `PlayerView` из сериализованных ссылок. Он передаёт команды модели и подключает события к представлению. `PlayerView` обновляет трансформы, `PlayerAnimator`, `PlayerUI`, звук и частицы.
- [`Player`](../Assets/_GameFiles/Scripts/Player/Player.cs) передаёт команды в `PlayerMechanicStateMachine`. Допустимость перехода задаёт `TryExit` текущего состояния. Состояния лежат в `Player/PlayerMechanicStateMachine`. Контроллер вызывает `MechanicStateUpdate()` каждый кадр, а модель вызывает `DoWithinFrame` активного состояния.
- Ходьба и бег используют `PlayerMovementController` и `PlayerLocomotion`. Направление движения и поворот учитывают поворот камеры. `PlayerLocomotion` хранит последнюю позицию, которую при загрузке обновляет `PlayerController.SetLastPosition`. Блок проходит через состояние `PlayerMechanicBlockState` в `PlayerHealth`. `PlayerDefenseController` пока пуст.

### Трансформация и полёт

`InputController` принимает действие `Transform` с клавиатуры, геймпада и экранного `OnScreenButton` из префаба `GameplayUI`. `BootstrapTestScene` вызывает `PlayerController.ToggleFighterForm` и по `FighterFormChanged` обновляет боевой ввод и экранные элементы. `PlayerView` переключает HUD и две модели. Контроллеры противников по тому же событию скрывают шкалы здоровья и жизней. Истребитель использует вложенный префаб `Space Ship 2` без компонентов движения и звука фонового корабля. Поворот модели задан внутри `FighterPivot`. В полёте `PlayerView` настраивает позицию и дистанцию Cinemachine относительно этого pivot и восстанавливает параметры человека при выходе из формы.

Вход в `PlayerMechanicFlightState` проходит через `TryExit` текущего состояния игрока. Состояние полёта снимает блок при входе и допускает переход в `Idle`. В полёте `Player` отклоняет урон, а `InputController` подавляет атаки и блок. `PlayerMovementController` вычисляет направление полёта по полным векторам камеры. `PlayerLocomotion` использует капсулу игрока для проверки пути, останавливает полёт при препятствии и сообщает представлению фактическую позицию.

При обратной трансформации `PlayerController` сразу возвращает человека в `Idle`. `Player.MechanicStateUpdate` обновляет гравитацию в `PlayerLocomotion` каждый кадр в человеческой форме. Проверка капсулой определяет опору и ограничивает смещение вниз при приземлении. Пока человек падает, обработчики ходьбы и бега пропускают смещение по горизонтали. При старте, загрузке и выходе из формы истребителя `PlayerController` проверяет опору сразу. При загрузке сохранённая скорость падения применяется, если человек в воздухе. Новая трансформация прерывает падение.

На объекте `FlightLimiter` в `FirstLevel` стоят триггер и кинематический `Rigidbody` для обработки касания игроком. `PlayerLocomotion` также проверяет пересечение триггера по всей траектории шага полёта. Оба пути вызывают `PlayerController.LockFighterForm`, который сохраняет запрет и принудительно возвращает истребитель в форму человека.

## Противники и появление босса

[`CharacterControllerNew`](../Assets/_GameFiles/Scripts/Character/CharacterControllerNew.cs) — базовый `MonoBehaviour` для противников. Он находит игрока и `ScoreController`, создаёт `Character`, `CharacterView` и контроллеры здоровья, движения и атак. Механические состояния задают допустимые действия, а конкретные контроллеры противников выбирают команды в `Update`.

| Контроллер | Логика поведения |
| --- | --- |
| [`CharacterControllerNewCloseRange`](../Assets/_GameFiles/Scripts/Character/Implemented%20Character/CharacterControllerNewCloseRange.cs) | Сближается с игроком. В радиусе ближнего боя чередует блок и удары с задержкой между атаками. |
| [`CharacterControllerNewLongRange`](../Assets/_GameFiles/Scripts/Character/Implemented%20Character/CharacterControllerNewLongRange.cs) | При дистанции меньше трёх единиц отходит в заданном направлении. Снаружи этой зоны поворачивается к игроку и запрашивает выстрел один раз до сброса счётчика при следующем сближении. |
| [`CharacterControllerNewBoss`](../Assets/_GameFiles/Scripts/Character/Implemented%20Character/CharacterControllerNewBoss.cs) | Выбирает преследование, дистанционную атаку, удар и блок в открытой фазе. Для ближней фазы включает отдельный режим поведения и ограничители арены. |

В [`FirstLevel`](../Assets/_GameFiles/Scenes/FirstLevel/FirstLevel.unity) уже стоят экземпляры ближнего противника и босса. Три объекта `SpawnerCharacterCloseRange`, `SpawnerCharacterLongRange` и `SpawnerCharacterBoss` сохранены неактивными. `CharacterFactory` создаёт экземпляры в `Awake`, `CharacterFactotyForBoss` (имя класса в коде) — по вызову `Spawn()`.

Активный маршрут появления босса проходит через [`ScoreController`](../Assets/_GameFiles/Scripts/ServicesGeneral/Score/ScoreController.cs). В `Awake` он находит босса и деактивирует его. Смерть рядового противника вызывает `CharacterControllerNew.CheckScore()` и добавляет очко. При начислении очка `ScoreController` активирует отключённого босса, если итоговый счёт достиг или превысил `1`. Это учитывает очки, восстановленные из сохранения. Для условия из ТЗ нужны две победы над конкретными типами врагов.

## Попадания, здоровье и фазы

1. Состояние атаки вызывает `PlayerOffenseController` или `CharacterOffenseController`. Те передают выбранный экземпляр `Weapon` в реализацию `PlayerAttack` или `CharacterAttack`.
2. Реализация атаки отправляет `Attacked` для анимации и эффектов, затем ищет получателя урона через [`EnvironmentAreaOverlapAnalyzer`](../Assets/_GameFiles/Scripts/ServicesGeneral/EnvironmentAreaAnalyzers/EnvironmentAreaOverlapAnalyzer.cs). Для обеих дистанций используется `Physics.OverlapSphere`. Анализатор берёт первый подходящий коллайдер с `IDamageable`, пропуская компоненты исключаемого типа. `EnvironmentAreaOverlapAnalyzerRender` подписывается на события анализатора.
3. `DamageCalculatorBasic` возвращает `Weapon.Damage`, затем вызывается `IDamageable.TakeDamage()`. Урон и дальность задаются в компонентах `Weapon`. В префабе игрока у меча урон переопределён на `4`, у дальнего оружия `RocketLauncher` — на `1`. Эффекты, звук и индикатор дальнего оружия управляются `PlayerView` и `PlayerUI`. Индикатор обновляется с задержками `Task.Delay`. Допуск к атаке в просмотренном пути определяется переходами состояний.
4. `PlayerHealth` и `CharacterHealth` хранят текущее здоровье и флаг блока. В обеих реализациях `TakeDamage` вычитает урон в нескольких ветках одного вызова. Если меняется баланс, сначала стоит проверить фактическое снятие здоровья от одиночного попадания.

[`PlayerHealth.Die()`](../Assets/_GameFiles/Scripts/Player/PlayerHealth/PlayerHealth.cs) начинает с `Lives = 3`. Пока счётчик больше нуля, метод восстанавливает здоровье, уменьшает счётчик и уведомляет HUD через `LivesChanged`. При вызове с уже нулевым счётчиком он отправляет `Died`. По этому коду экран поражения открывается при следующем исчерпании здоровья после перехода `1 → 0`.

У босса `_currentGlobalLife` считает исчерпанные полоски здоровья. Порог `globalLives` по умолчанию равен `6`. `OnBossDeath` при промежуточной потере полоски переключает фазу и восстанавливает здоровье. На отметках `2`, `4` и `6` начисляется очко и обновляется шкала глобальных жизней босса. При достижении порога вызывается `WinUI`. В просмотренных обработчиках жизней игрока и фаз босса отсутствует взаимное восстановление жизней из ТЗ.

`ActivatePhaseTwo()` снимает блок, ищет свободную позицию рядом с игроком и после телепорта обновляет исходную позицию в `CharacterLocomotion`. Компонент [`Limiters`](../Assets/_GameFiles/Scripts/ServicesGeneral/Limiters.cs) центрирует границы по их `BoxCollider` между персонажами и при необходимости расширяет зону под обе капсулы. Босс отсоединяет ограничители от себя перед активацией. При возврате в открытую фазу он выключает их и снова прикрепляет к себе.

`HandlePhaseTwoBehaviour()` отсчитывает 10 секунд боя и 3 секунды восстановления по игровому времени. В бою босс проверяет траекторию своего шага капсулой и использует `meleeRange` и для сближения, и для начала атаки. В начале восстановления контроллер снимает блок и переводит босса в `Idle`. Удары игрока проходят через обычный обработчик урона и во время боя, и во время восстановления.

## Пауза, экраны и очки

- [`GameplayMenuUI`](../Assets/_GameFiles/Scripts/UIGeneral/GameplayMenuUI/GameplayMenuUI.cs) отвечает за открытие панели и переключение обработчиков ввода. [`ScenePausing`](../Assets/_GameFiles/Scripts/ServicesGeneral/ScenePausing/ScenePausing.cs) переключает `Time.timeScale` между `0` и `1`. `BootstrapTestScene` подключает к меню продолжение, сохранение, загрузку и выход в главное меню.
- [`PlayerController`](../Assets/_GameFiles/Scripts/Player/PlayerController.cs) подключает `LoseUI` к окончательной смерти игрока. Кнопка загрузки на экране поражения вызывает `SceneLoading.LoadLevelScene()`. После открытия `FirstLevel` сохранение читается при инициализации `GameSaveLoadInteractor`.
- [`WinUI`](../Assets/_GameFiles/Scripts/UIGeneral/WinUI.cs) и [`LoseUI`](../Assets/_GameFiles/Scripts/UIGeneral/LoseUI.cs) выводят `ScoreController.Score`. Очки за рядовых врагов и за пары фаз босса идут в один счётчик. Экранный счётчик HUD увеличивается по `ScoreIncreased`, тогда как `SetScore()` при загрузке обновляет значение без этого события.
- В основном меню подключён слайдер громкости музыки. В проверенных обработчиках и подключениях нет действия выхода из приложения, окна настроек в паузе и сохранения громкости между запусками.

## Сохранения

Штатный маршрут `FirstLevel` проходит через [`GameSaveLoadInteractor`](../Assets/_GameFiles/Scripts/ServicesGeneral/SaveAndLoad/NewSaveLoadSystem/GameSaveLoadInteractor.cs). Он читает сохранение в `Start` после инициализации игрока и подписок, сохраняет по кнопке паузы и подписывается на `Application.quitting`. Репозитории используют `JsonUtility` и записывают два отдельных файла в `Application.persistentDataPath`.

| Файл | Поля |
| --- | --- |
| `player_save.json` | `PlayerSaveData`: позиция и поворот корня, поворот модели, текущее здоровье, очки, форма, запрет трансформации, флаг и скорость падения. |
| `characters_save.json` | `SavedCharactersData`: массив записей `CharacterSaveData` с позицией, поворотом и текущим здоровьем. |

Записи противников применяются по индексам массива из `FindObjectsByType(..., FindObjectsSortMode.None)`. В JSON отсутствуют идентификаторы и типы противников, глобальные жизни, фаза босса и выбранное оружие. Старые файлы загружаются в человеческой форме с прежними координатами и поворотом корня. Если поля запрета в сохранении ещё нет, трансформация доступна до касания триггера. `IsBoss` существует как статический флаг процесса, обновляется при обходе противников во время сохранения и используется при загрузке для переключения сценовых объектов. Пустой список активных противников не перезаписывает старый файл. Эти детали важны при доработке точного восстановления боя.

В [`OldSaveLoadSystem 1`](../Assets/_GameFiles/Scripts/ServicesGeneral/SaveAndLoad/OldSaveLoadSystem%201) есть другой формат для позиции и поворота игрока. В [`GameplayUI.prefab`](../Assets/_GameFiles/Prefabs/UI/UILayouts/GameplayUI.prefab) остались сериализованные вызовы его `SaveData` и `LoadData` с пустыми целями. Рабочие обработчики кнопок в `FirstLevel` назначает `BootstrapTestScene`.

## Где начинать изменения

| Задача | Первые места для проверки |
| --- | --- |
| Управление на Android, кнопки, бег, блок | Карта `MainCharacter`, `InputController`, подписки в `BootstrapTestScene`, переходы в состояниях `Player`. В паузе `InputController` приостанавливает игровой ввод. |
| Дальность, урон, кулдаун, варианты оружия | `PlayerOffenseController`, `CharacterOffenseController`, реализации атак, `EnvironmentAreaOverlapAnalyzer`, префабы оружия и переопределения в префабе игрока. `WeaponType` у противников переключает видимые варианты в `Awake`. Подключённый выбор оружия игроком в рассмотренном коде не обнаружен. |
| Появление босса, фазы, глобальные жизни | `FirstLevel`, `ScoreController`, `CharacterControllerNew.CheckScore`, `CharacterControllerNewBoss.OnBossDeath`, `PlayerHealth.Die` и обе шкалы жизней в сцене. |
| Трансформация и полёт | Действие `Transform` в `MainCharacter`, `BootstrapTestScene`, `PlayerMechanicFlightState` и воздушное движение в `PlayerLocomotion`. |
| Пауза, итоговые экраны и сохранения | `BootstrapTestScene`, `GameplayUI.prefab`, `WinUI`, `LoseUI`, `GameSaveLoadInteractor`, DTO и репозитории из `NewSaveLoadSystem`. Проверять восстановление в новой сессии, поскольку часть состояния статическая. |

`GameControllerScenes/GameController.cs` и `EventBusGeneral/EventBusGeneral.cs` пока содержат пустые классы. Координация сцены и событий проходит через bootstrap, ссылки Unity, обработчики в компонентах и события моделей. При изменении подписок проверяй пары `OnEnable` и `OnDisable`. В нескольких компонентах для отписки создаются новые анонимные делегаты. Повторная активация объектов требует проверки этих связей.
