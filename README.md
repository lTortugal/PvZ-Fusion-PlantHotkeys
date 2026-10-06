# PvZ Fusion PlantHotkeys

**RU** | [EN](#english)

Хоткеи для выбора растений в PvZ Fusion (MelonLoader). Нажми 1-0 или Q W E R, чтобы взять растение из нужного слота, без мыши.

## Требования
- PvZ Fusion 3.9 или 4.0.5.2
- MelonLoader (Il2Cpp), проверено на 0.7.3

## Установка
1. Скачай `PlantHotkeys-1.0.zip` из раздела [Releases](../../releases).
2. Положи `Mods/HotkeyMod.dll` в папку `Mods` игры (рядом с другими модами).
3. Запусти игру.

## Раскладка
| Клавиша | Действие |
|---|---|
| 1 2 3 4 5 6 7 8 9 0 | слоты 1-10 |
| Q W E R | слоты 11-14 |
| A | лопата |
| S | перчатка |
| D | замедление |
| M | молоток |
| K | колесо |
| T | показать HP растений |
| Y | показать HP зомби |
| F | золотой боб |
| U | показать урон пуль |
| O | полный экран (перенесён с F) |

Подписи у лопаты, перчатки, замедления и золотого боба обновляются автоматически. Клавиша R скрыта от игры, чтобы не срабатывал встроенный сброс растения. F10 переключает способ клика по карточке (обычно не нужен).

## Ограничения
- Проверено на обычных уровнях. Конвейер и режимы с нестандартными слотами не тестировались.
- После обновления игры мод может перестать работать и потребовать пересборки.
- Антивирус может ругаться на неподписанный DLL; исходники лежат в репозитории.

## Изменение клавиш и сборка
Клавиши лежат в начале `Mod.cs`. Нужен .NET SDK; в `HotkeyMod.csproj` поправь `GamePath` на путь к своей игре и выполни `dotnet build -c Release`.

---

## English

Plant selection hotkeys for PvZ Fusion (MelonLoader). Press 1-0 or Q W E R to pick the plant from the matching card slot, no mouse needed.

### Requirements
- PvZ Fusion 3.9 or 4.0.5.2
- MelonLoader (Il2Cpp), tested on 0.7.3

### Installation
1. Download `PlantHotkeys-1.0.zip` from [Releases](../../releases).
2. Put `Mods/HotkeyMod.dll` into the game's `Mods` folder.
3. Launch the game.

### Keys
- **1-0**: card slots 1-10; **Q W E R**: card slots 11-14
- **A** shovel, **S** glove, **D** slow time, **M** hammer, **K** wheel
- **T** show plant HP, **Y** show zombie HP, **F** gold bean, **U** show bullet damage
- **O** fullscreen (moved from F)

Tool labels update automatically. R is hidden from the game so its built-in plant reset does not trigger. F10 switches the card click method (normally not needed).

### Limitations
- Tested on regular levels only; conveyor and non-standard slot modes are untested.
- The mod may break after a game update and need a rebuild.
- Antivirus software may flag the unsigned DLL; the source code is in this repository.

### Changing keys / building
Keys are at the top of `Mod.cs`. Requires the .NET SDK; set `GamePath` in `HotkeyMod.csproj` to your game folder and run `dotnet build -c Release`.

## License
MIT. Author: Tortuga.
