# PvZ Fusion PlantHotkeys

**RU** | [EN](#english)

Хоткеи для выбора растений в PvZ Fusion (MelonLoader). Нажми 1-0 или Q W E R, чтобы взять растение из нужного слота, без мыши. Под каждой карточкой видна её клавиша, а клавиши можно переназначить прямо в игре.

## Возможности
- 14 слотов на клавишах 1-0 и Q W E R
- подпись с клавишей под каждой карточкой
- окно настройки клавиш (F11) и файл настроек `UserData/PlantHotkeys.cfg`
- лопата, перчатка, замедление, золотой боб и другое перенесены на свободные клавиши, подписи у инструментов обновляются

## Требования
- PvZ Fusion 3.9 или 4.0.5.2
- MelonLoader (Il2Cpp), проверено на 0.7.3

## Установка
1. Скачай `PlantHotkeys-1.1.zip` из раздела [Releases](../../releases).
2. Положи `Mods/HotkeyMod.dll` в папку `Mods` игры (рядом с другими модами).
3. Запусти игру.

## Раскладка по умолчанию
| Клавиша | Действие |
|---|---|
| 1 2 3 4 5 6 7 8 9 0 | слоты 1-10 |
| Q W E R | слоты 11-14 |
| A | лопата |
| S | перчатка |
| D | замедление |
| Tab | молоток |
| LeftShift | колесо |
| T | показать HP растений |
| Y | показать HP зомби |
| F | золотой боб |
| U | показать урон пуль |
| CapsLock | полный экран |

## Окно настройки (F11)
Нажми F11, выбери кнопку нужного действия и нажми новую клавишу. Если клавиша занята другим действием мода, они поменяются местами. Esc отменяет выбор, кнопка «Сбросить всё» возвращает стандартную раскладку. F10 и F11 назначить нельзя; если выбрать клавишу, которую использует сама игра (C, H, G, P, X, V, B, F1), мод предупредит. Открывай окно лучше в паузе или в меню: клики по нему доходят и до игры.

Настройки хранятся в `UserData/PlantHotkeys.cfg`. Это обычный текстовый файл, его можно править руками, пока игра закрыта; удали файл, чтобы вернуть стандартные клавиши.

## Прочее
- Клавиша R скрыта от игры, чтобы не срабатывал встроенный сброс растения.
- F10 переключает способ клика по карточке (запасной вариант, обычно не нужен).

## Ограничения
- Проверено на обычных уровнях. Конвейер и режимы с нестандартными слотами не тестировались.
- После обновления игры мод может перестать работать и потребовать пересборки.
- Антивирус может ругаться на неподписанный DLL; исходники лежат в репозитории.

## Сборка из исходников
Нужен .NET SDK; в `HotkeyMod.csproj` поправь `GamePath` на путь к своей игре и выполни `dotnet build -c Release`.

---

## English

Plant selection hotkeys for PvZ Fusion (MelonLoader). Press 1-0 or Q W E R to pick the plant from the matching card slot, no mouse needed. Every card shows its key, and keys can be rebound in game.

### Features
- 14 card slots on 1-0 and Q W E R
- key label under every card
- in-game key settings window (F11) and a config file `UserData/PlantHotkeys.cfg`
- shovel, glove, slow time, gold bean and others moved to free keys, tool labels update automatically

### Requirements
- PvZ Fusion 3.9 or 4.0.5.2
- MelonLoader (Il2Cpp), tested on 0.7.3

### Installation
1. Download `PlantHotkeys-1.1.zip` from [Releases](../../releases).
2. Put `Mods/HotkeyMod.dll` into the game's `Mods` folder.
3. Launch the game.

### Default keys
- **1-0**: card slots 1-10; **Q W E R**: card slots 11-14
- **A** shovel, **S** glove, **D** slow time, **Tab** hammer, **LeftShift** wheel
- **T** show plant HP, **Y** show zombie HP, **F** gold bean, **U** show bullet damage
- **CapsLock** fullscreen

### Settings window (F11)
Press F11, click the button of an action and press the new key. If the key is already used by another action of the mod, the two are swapped. Esc cancels, "Сбросить всё" restores defaults. F10 and F11 cannot be assigned; keys used by the game itself (C, H, G, P, X, V, B, F1) trigger a warning. Better open the window in the pause menu: clicks also reach the game.

Settings are stored in `UserData/PlantHotkeys.cfg`, a plain text file you can edit while the game is closed; delete it to restore defaults.

### Notes
- R is hidden from the game so its built-in plant reset does not trigger.
- F10 switches the card click method (fallback, normally not needed).

### Limitations
- Tested on regular levels only; conveyor and non-standard slot modes are untested.
- The mod may break after a game update and need a rebuild.
- Antivirus software may flag the unsigned DLL; the source code is in this repository.

### Building
Requires the .NET SDK; set `GamePath` in `HotkeyMod.csproj` to your game folder and run `dotnet build -c Release`.

## License
MIT. Author: Tortuga.
