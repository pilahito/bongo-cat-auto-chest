> **Community fork by [pilahito](https://github.com/pilahito).** Derived from [NH4OH-35/bongo-cat-auto-chest](https://github.com/NH4OH-35/bongo-cat-auto-chest); the original author and MIT license remain credited in [`LICENSE`](LICENSE) and [`THIRD-PARTY-NOTICES.md`](THIRD-PARTY-NOTICES.md). This fork adds an optional automatic duplicate-emoji exchange button beside the collection's fill arrow. It starts off and only trades the game's emote group. See [`CONTRIBUTING.md`](CONTRIBUTING.md) for contribution rules.

> **Fork build note:** upstream downloads do not contain this fork's new feature. Build this repository on Windows with [`build.ps1`](build.ps1); no proprietary Steam or game files are included.

---
# Bongo Cat Auto Chest ฅ

**Auto-claim normal and emote chests, open stored tokens, and control your pet score in Bongo Cat.** Set up the Steam launch command once, then use Steam's **Play** button or your usual Steam shortcut. The launch helper checks the patch after Steam updates and before the game starts, and reapplies it when the new build is supported.

[Upstream v1.4.0 Windows download (does not include this fork's feature)](https://github.com/NH4OH-35/bongo-cat-auto-chest/releases/download/v1.4.0/BongoCatAutoChest-v1.4.0-windows.zip) · [Upstream releases](https://github.com/NH4OH-35/bongo-cat-auto-chest/releases) · [Fork issues](https://github.com/pilahito/bongo-cat-auto-chest/issues)

![Bongo Cat Auto Chest v1.2 in Bongo Cat General settings: normal and emote auto-claim, Keep pets, token claim buttons, and pet score editor](docs/game-settings.png)

The screenshot shows the in-game controls introduced in **v1.2**, which are unchanged in **v1.4**. Your token counts and pet score will differ. **Keep pets when claiming** is checked in this example; it starts **off** on a fresh installation.

## 1. Install or upgrade

**Already using v1.2 or v1.3? Download v1.4 and set up the Steam launch command below.** The v1.3 startup check ran at Windows sign-in; a later Steam update and launch could replace the patch after that check. The new Steam launch helper checks immediately before each game launch.

1. Download **BongoCatAutoChest-v1.4.0-windows.zip** using the link above. Do not download GitHub's **Source code** ZIP for installation.
2. Right-click the downloaded ZIP → **Extract All…** → **Extract**. Open the extracted folder; do not run the app from inside the ZIP.
3. Exit **Bongo Cat** normally. Leave Steam available for launching the game later.
4. Double-click **BongoCatAutoChest.exe**. Keep **Mono.Cecil.dll** in the same folder. **BongoCatAutoChest.Cli.exe** is optional and is not needed for normal use.
5. Check the game folder shown in Bongo Cat Auto Chest. If it is empty or wrong, click **Browse…** and select the folder containing **BongoCat.exe**. Select the **folder**, not the EXE file, **BongoCat_Data**, or **Managed**. You can find it through Steam → Bongo Cat → **Properties → Installed Files → Browse**.
6. The helper checks the build automatically. You can click **Check build** to check again. If the build is supported, click **Install auto claim →**. Existing v1.0/v1.1 installations show **Upgrade controls →** instead.
7. Wait for the success message. The button changes to **Controls installed**. Keep the game closed and continue with **Set up the Steam launch command** below.

The installer backs up your exact original game DLL before patching it. Upgrades use that verified original backup and preserve your saved normal/emote switch settings.

## Set up the Steam launch command

Do this **once** after installing the patch. Steam will then run the check after its updates finish and before starting Bongo Cat.

1. Keep Bongo Cat closed. In the v1.4 helper, confirm the correct game folder and **Controls installed** status.
2. Click **Copy Steam launch command**. The helper prepares a copy inside the selected game folder and copies the complete command to your clipboard.
3. In **Steam → Library**, right-click **Bongo Cat** → **Properties** → **General**.
4. Find **Launch Options** and paste the copied command. Keep its quoted executable path and the final **`%command%`** exactly as copied. Leave `%command%` literal; Steam replaces it when launching the game. If the field already contains custom options, save a copy of them before changing it.
5. Close Properties, close the installer window, and click **Play** in Steam. Your normal Steam shortcut also uses the launch option.
6. In the game, open **gear → General**, scroll down, and confirm the added controls are present.

Steam's [launch options guide](https://help.steampowered.com/en/faqs/view/7D01-D2DD-D75E-2955) shows where to find this setting. Use the command copied by your helper: it contains the actual full path inside your selected game folder.

**Keep `.bongo-claim\launcher` inside your selected game folder and retain its files while the Steam launch option uses it.** It contains the Steam helper, Mono.Cecil dependency, local repair setting and launch log. You can move or delete the extracted download folder after setup. The helper preserves Steam's original game arguments and environment and waits for the game to exit so Steam can track the session.

If you move the game to another folder or Steam library, select its new folder in the helper, click **Copy Steam launch command** again, and paste the updated command into Steam Launch Options. Keep `%command%` unchanged.

The Steam launch check is separate from the optional [Windows startup repair](#set-up-startup-repair-optional). Set up this launch command even if you already enabled startup repair in v1.3.

## 2. Open the controls

1. Start Bongo Cat normally through Steam.
2. Focus the game and press **Escape** or **M** to open its menu.
3. Click the **gear** tab, then **General**.
4. Scroll down inside the panel until you see the rows shown in the screenshot.

Leave **Steam and Bongo Cat running** for auto-claim to work. You can close the installer window. With the Steam launch command set, use Steam normally; its launch helper runs hidden for the game session. Optional startup repair also starts the game at Windows sign-in.

## 3. Turn automatic claims on or off

1. Check **Auto claim normal chests** to enable automatic normal-chest claims; uncheck it to pause them.
2. Check or uncheck **Auto claim emote chests** independently.
3. Close the menu and continue playing. Your choices take effect immediately and save for the next launch.

| Setting | Fresh-install default | Effect |
| --- | --- | --- |
| Auto claim normal chests | On | Claims an eligible normal chest when the game's timer is ready. |
| Auto claim emote chests | On | Claims an eligible emote chest when the game's timer is ready. |
| Keep pets when claiming | Off | When enabled, normal and emote chest claims do not deduct available pets. |

Saved preferences take precedence over these defaults. A request already sent can finish after you disable its switch. Automatic claims use the game's existing timer and exchange requirements; claims cost pets while **Keep pets when claiming** is off.
## Intercambio automático de emojis duplicados

Pulsa el botón **♥** que aparece junto a la flecha de rellenado de duplicados en el panel de colección para activar o detener el intercambio automático. El botón usa el tema visual del juego y cambia de aspecto cuando está activo. También puedes usar **Ajustes → General → 🔄 Intercambio automático de emojis duplicados**. Empieza desactivado cada vez que abres el juego.

Cuando está activado, el mod revisa el grupo de intercambio de **emojis** e inicia los intercambios elegibles con un intervalo mínimo de tres segundos. Espera a que termine cualquier intercambio en curso y respeta los espacios que hayas rellenado manualmente. No rellena ni intercambia el grupo de cosméticos. El juego consume los duplicados al intercambiarlos; apagar el botón no cancela una petición ya enviada, pero evita que empiece otra. Si ocurre un error, el botón se apaga y muestra el estado.

La reclamación automática de cofres normales y de emoji se configura por separado en **Ajustes → General**.

## Build this fork

On Windows with .NET Framework 4.8 available, run `./build.ps1` from the repository folder. Run `./package.ps1` to create the portable ZIP under `artifacts/`. The scripts compile the helper and package only project files and the pinned Mono.Cecil dependency; they do not need or include Steam credentials or the game's assemblies.

## 4. Claim stored chest tokens

1. Open **gear → General** and find **Normal tokens: N | claim** or **Emote tokens: N | claim**.
2. If the number is greater than **0**, click the row for the chest type you want.
3. Wait for the game and Steam to finish that claim. Each successful claim consumes **one owned token**.
4. Click again to open another token, if you still have one.

These buttons can open stored tokens while the local 30-minute countdown is still running. They work independently of the auto-claim switches. A successful manual claim starts the game's normal cooldown again.

**“0 | claim” means you currently have no owned tokens of that type to open.** The row is disabled. “Loading” means Steam inventory is not ready yet. The screenshot's zero counts are an example, not a target value.

Claims also require the game's other exchange inputs, enough pets unless **Keep pets** is enabled, and no pending request. Steam decides whether an exchange succeeds. These controls do not create tokens or change Steam's drop schedule; editing your pet score does not create tokens either.

## 5. Stop chest claims deducting pets

1. In **gear → General**, check **Keep pets when claiming**.
2. Claim a normal or emote chest automatically, through a stored-token row, or through the game's original chest button.

Available pets are preserved while this switch is on. Uncheck it to restore the normal pet cost. The switch saves for the next launch. Owned tokens are still consumed, and gifts retain their original spending rules. An in-progress request uses the price choice from when it started.

## 6. Edit your available pet score

1. Click **Pets: N | edit** in General, or press **F10** with the game menu open.
2. Type the **full new score** using the number row or numpad. The editor starts empty; you do not need to erase the old score first. For example, type `100000` to set available pets to 100,000.
3. Use **Backspace** to correct a digit.
4. Press **Enter** to save, or **Escape** to cancel. Clicking the editor again or closing the menu also cancels.

Use a nonnegative whole number. If the row says **Invalid score; type again**, enter a smaller valid number. The maximum is `2,147,483,646 − total pets previously spent`.

Saving changes your available pets through the game's Steam stats API. Lifetime achievement taps and total pets previously spent are preserved. Restoring the original game DLL removes the controls but does not undo a score you saved.

## Keyboard shortcuts

Focus Bongo Cat and keep its menu open. Shortcuts do not run while another text field is active. Toggle and token shortcuts pause during score editing.

| Key | Action |
| --- | --- |
| **F5** | Toggle Keep pets when claiming |
| **F7** | Toggle normal auto-claim |
| **F9** | Toggle emote auto-claim |
| **F10** | Start editing pet score |
| **F11** | Claim one owned normal token |
| **Insert** | Claim one owned emote token |

During score editing, **Enter** saves, **Escape** cancels, and **Backspace** deletes a digit. If plain F9 is missed on your PC, **Shift + F9** reaches the same emote toggle. The shortcuts update the same saved controls as mouse clicks.

## Set up startup repair (optional)

Use this if you also want Bongo Cat to start when you sign in to Windows. **Complete the Steam launch command setup above first**, so launches after Steam updates also get checked.

1. In Bongo Cat, open **gear → General** and enable the game's **Autostart** setting.
2. Let Steam finish any game update, then exit Bongo Cat normally.
3. Open **BongoCatAutoChest.exe** from the extracted v1.4 folder and check the selected game folder.
4. Click **Enable startup repair** and wait for its success message. The button changes to **Disable startup repair**.
5. Close the helper. At your next Windows sign-in, startup repair checks the game while it is closed, repairs a compatible missing patch if needed, then launches through Steam. The Steam launch command checks again just before the game starts.

Windows startup repair keeps its separate helper copy, settings and log in `%LOCALAPPDATA%\BongoCatAutoChest`. Keep that folder while startup repair is enabled. The Steam launch helper uses the game-local `.bongo-claim\launcher` folder.

Keep the game's **Autostart** setting on if you want the Windows sign-in launch. Turning it off removes the game's startup entry. Turning it back on points that entry directly at the game again; open the v1.4 helper and click **Enable startup repair** again to restore the sign-in check. The Steam launch option is a separate setting.

To stop the sign-in repair, click **Disable startup repair**. This restores the startup command that existed before you enabled repair, when Bongo Cat Auto Chest still owns the entry. The Steam launch command and installed chest controls remain active.

The helper refuses to patch a running game, a pending Steam update, or an unsupported assembly layout. Let updates finish and close the game before retrying. Unsupported builds need a compatible helper update.

## Troubleshooting

| What you see | What to do |
| --- | --- |
| **Controls installed** in the helper | The patch is already installed. Start Bongo Cat from Steam; no second installation is needed. |
| Missing rows in General | Scroll down. If only the two auto-claim switches remain, exit the game and use **Upgrade controls**. If all added rows disappeared after an update, use the v1.4 helper and complete the Steam launch command setup. |
| **0** tokens or **loading** | Zero means no owned token is available. For loading, allow Steam inventory to finish loading with Steam running. |
| Tokens exist but a claim does not start | Wait for any pending request. Check the game's exchange requirements and your pet balance, or enable **Keep pets**. |
| Pets decreased during a claim | Check **Keep pets when claiming** before starting another claim. It defaults off; changing it does not alter a request already sent. |
| Missing **Mono.Cecil.dll** or dependency error | Extract the whole release ZIP and keep its files together. |
| Game is running during install, repair or restore | Exit Bongo Cat normally, then try again. The helper will not force-close it. |
| Auto-claim disappeared after a Steam update | Finish the update, exit the game, check/install with the v1.4 helper, then click **Copy Steam launch command** and set Steam Launch Options. Startup repair alone only checks at sign-in. |
| Startup repair reports a pending Steam update | Allow Steam to finish the update before retrying. The helper will not patch during the update. |
| Game starts at sign-in but the controls are missing | Confirm the copied v1.4 command is still in Steam Launch Options. If you toggled the game's Autostart off and on, also enable startup repair again. |
| DLL changed during launch | Exit the game, let Steam finish, confirm the v1.4 Steam launch command, then click **Play** again. |
| Steam cannot find the launch helper | Keep `.bongo-claim\launcher` inside the selected game folder. If the game moved, select its new folder in the v1.4 helper. Click **Copy Steam launch command**, then paste the refreshed command into Steam Launch Options. |
| Unsupported build or a blocked file write | Read the reported error. The helper refuses incompatible layouts and does not change Windows security settings. Report the build ID and error text. |

Requires **Windows 10 or 11**, **.NET Framework 4.8** (normally already installed), the Steam Windows version of Bongo Cat, and write access to its game folder. No administrator privileges are requested. Release binaries are unsigned. Keep antivirus protection enabled.

## Restore the original game

1. Exit Bongo Cat normally.
2. Open **BongoCatAutoChest.exe** and check the selected game folder.
3. Click **Restore original** and wait for success.
4. Close the helper and start Bongo Cat through Steam.

Restoration disables the local Steam launch repair setting and startup repair, restoring the previous startup command when the helper still owns it. It then returns the matching original DLL byte-for-byte. The Steam Launch Options command can stay in place: the disabled helper passes the original launch through without patching. To remove the Steam helper entirely, clear that command from Steam Launch Options before deleting the game-local `.bongo-claim\launcher` folder.

Game saves and inventory files are not touched. A pet score you explicitly saved remains saved.

Backups and the Steam launch helper live inside the selected game folder:

```text
.bongo-claim/
  install.json
  backups/<original-sha256>.dll
  launcher/
    BongoCatAutoChest.exe
    Mono.Cecil.dll
    steam-guard.json
    steam-launch.log
```

The `launcher` folder is prepared when you copy the Steam launch command; its log is written during launches.

The helper verifies backup hashes before using them. If Steam changes the installed DLL, the helper refuses to restore an older backup over it.

## After a Steam update or an older replacement DLL

1. Exit Bongo Cat after Steam finishes the update.
2. Open Bongo Cat Auto Chest and click **Check build**.
3. If the current build is supported and the patch is absent, install it again. Complete the v1.4 Steam launch command setup if you have not done so, then start the game through Steam. Previous backups are retained.
4. If the layout is unsupported, wait for a compatible Bongo Cat Auto Chest update and report your build ID.

If you previously installed the archived replacement DLL, or your original backup is missing or damaged, use Steam → Bongo Cat → **Properties → Installed Files → Verify integrity of game files** to recover the current official files before applying Bongo Cat Auto Chest.

## Compatibility and validation

On **3 November 2026**, Steam updated Bongo Cat and launched it after the Windows sign-in check had already run. The v1.3 startup entry and stable helper files were still intact. That check covered the sign-in launch; later Steam launches needed their own check. v1.4 adds the Steam launch command so repair runs after Steam's updates and immediately before the actual game executable.

Windows build **26402940** was inspected and its official DLL layout was compatible. A controlled live test restored its verified official DLL to simulate a missing patch, then launched through the normal Steam command. Steam started the game-local guard; the guard reapplied and verified the patch before starting Bongo Cat. The General controls were manually confirmed visible, all three saved switches remained enabled, and the game log contained no runtime errors. Steam tracked the guard and its child game process. A full Windows reboot was not performed during this check.

```text
Build 26402940 original Assembly-CSharp.dll SHA-256
3ae33d93def6346f715256d2b65192097632059252eed969c2641f63b946d4dd
```

The **14 October 2026** Steam update installed Windows build **25936063** and replaced the patched DLL. On **15 October 2026**, the helper inspected that build, confirmed its layout was compatible, and reapplied the patch. Saved switch preferences were retained. Startup enable/disable/re-enable restored the original command exactly, and the permanent helper/dependency copies matched their source hashes. A controlled live replacement with the verified official DLL was automatically repaired by the saved launcher before starting through Steam; the patched file and startup command remained intact after launch, and the game log contained no runtime errors. A full Windows reboot with startup repair has not been recorded.

```text
Build 25936063 original Assembly-CSharp.dll SHA-256
96bbcbb2733988fd31ce261891eb3027888389e03b6a23dbd63e9e7f518adfb8
```

Earlier compatibility and live checks were performed on **12 October 2026**, on Steam Windows build **25723683**:

```text
Original Assembly-CSharp.dll SHA-256
7566e8d9846cdb38eadcdb729cfcf676dc338cd738e99f664dc6ef3b79e87b83
```

**70 executable tests passed** using a source-owned simulated game assembly. They cover automatic and manual claims, independent switches, token quantities and consumption, claims during cooldown, missing inputs and pending requests, captured price choices, gift spending, score save/cancel/backspace/overflow, focus gates, upgrades, corrupted backups, Steam updates, rollback and exact restoration. Eight launcher tests additionally cover automatic preparation, repeated no-op checks, supported Steam replacements, retained original backups, unsupported builds, pending updates and malformed manifest flags. Twelve Steam guard tests cover actual Windows argument parsing, literal shell characters, valid executable paths, and enabled, explicitly disabled, missing or mismatched guard state.

Live checks verified normal and emote automatic claims in earlier versions. In v1.2, all six added rows rendered; a stored normal-token claim succeeded during countdown without reducing pets; an emote-token claim through Insert reached its successful cooldown during countdown; and score input followed by cancellation left the score unchanged. The final game log contained no runtime errors. **Live score saving and cosmetic identities were not recorded**; score saving and persistence are covered by the simulated-game tests. The supplied screenshot documents the current controls.

The installer inspects the current game assembly and runtime dependencies, then reads back the patched output. It refuses unsupported layouts and conflicting auto-claim mods. Future game builds and Steam server behavior remain outside these checks.

## Build from source

Run in PowerShell on Windows:

```powershell
.\build.ps1 -Test
.\package.ps1
```

No .NET SDK or Visual Studio is required. The build uses the Windows .NET Framework compiler and a pinned, hash-verified Mono.Cecil NuGet package. Outputs go in `artifacts/`; dependencies go in `tools/`. Neither directory is tracked.

The release ZIP contains **BongoCatAutoChest.exe**, **BongoCatAutoChest.Cli.exe**, **Mono.Cecil.dll**, this README, **docs/game-settings.png**, and the licenses. The screenshot is included so it displays in the extracted instructions. No game DLL, test fixture, backup, save, or account data is packaged. The adjacent `.zip.sha256` file contains the expected SHA-256 hash for checking the ZIP.

## Command line

For diagnostics and scripted use:

```powershell
.\BongoCatAutoChest.Cli.exe --inspect 'G:\SteamLibrary\steamapps\common\BongoCat'
.\BongoCatAutoChest.Cli.exe --install 'G:\SteamLibrary\steamapps\common\BongoCat'
.\BongoCatAutoChest.Cli.exe --steam-command 'G:\SteamLibrary\steamapps\common\BongoCat'
.\BongoCatAutoChest.Cli.exe --restore 'G:\SteamLibrary\steamapps\common\BongoCat'
.\BongoCatAutoChest.Cli.exe --enable-startup 'G:\SteamLibrary\steamapps\common\BongoCat'
.\BongoCatAutoChest.Cli.exe --disable-startup 'G:\SteamLibrary\steamapps\common\BongoCat'
.\BongoCatAutoChest.Cli.exe --launch 'G:\SteamLibrary\steamapps\common\BongoCat'
.\BongoCatAutoChest.Cli.exe --patch-copy 'path\Assembly-CSharp.dll' "$env:TEMP\patched-copy.dll"
```

`--steam-command` prepares the helper and local Steam repair setting in the selected game folder's `.bongo-claim\launcher` directory, then prints the command to paste into Steam Launch Options. Keep its `%command%` literal. The GUI's **Copy Steam launch command** button also copies it to the clipboard.

`--enable-startup` installs the Windows startup copy under `%LOCALAPPDATA%\BongoCatAutoChest` and wraps the existing Bongo Cat startup entry. `--disable-startup` restores the previous command when this helper still owns the entry. `--launch` prepares a closed game and asks Steam to launch it; set the Steam launch command for the final check after Steam updates.

`--steam-launch <game-exe> [args]` is invoked by Steam through the configured launch option. It checks the patch and starts the original executable with its supplied arguments and environment, without invoking Steam again. Run the game through Steam normally; this internal command is not a separate setup step.

Both chest types default on. `--normal-only` makes the initial emote default off; a saved in-game preference takes precedence. `--emotes` remains accepted for compatibility with v1.0. `--verbose` includes a diagnostic stack trace. `--patch-copy` refuses to overwrite its source or an existing output.

## How the patch works

The archived [Bongo-Cat-Auto-Claim](https://github.com/zelda0079/Bongo-Cat-Auto-Claim) distributes an older complete game DLL. Bongo Cat Auto Chest patches the assembly actually installed on your PC, checks the expected structure, and retains your exact original for restoration.

`PatchEngine.cs` inserts a guarded claim helper into the game's chest-ready callback and timer. It reads saved normal/emote switches and checks readiness and purchase locks before calling the original `ShopItem.Buy()`. The game's timer interval, purchase callbacks and failed-request cooldown are retained.

`NativeControlsPatch.cs` adds three saved switches and three native action rows. `OwnedTokensPatch.cs` binds source-owned feature methods from an embedded compile-time template to the installed game's Unity and Steam APIs. Only new feature members are copied; no stub types or template dependency are installed.

`RuntimeFeatures.cs` reads the real normal and emote inventory quantities, checks the full exchange requirements before manual purchases, and implements the inline score editor. Keep pets changes the captured local price flag for chest purchases. No game or Steam assembly is shipped.

The Steam launch helper uses the same compatibility, backup and atomic-write checks as manual installation. Steam supplies the actual executable and arguments through `%command%`; the helper checks or repairs before starting that executable, preserves its environment, then waits for it to exit. It runs hidden without polling for game processes or launching Steam recursively.

Optional startup repair manages the current user's Bongo Cat startup entry and preserves its previous command for disabling or restoration. Both repair paths retain older exact backups after Steam updates.

Development inspection used [REA](https://github.com/morluto/rea) and [Mono.Cecil](https://github.com/jbevain/cecil). See [third-party notices](THIRD-PARTY-NOTICES.md).

## License

MIT for Bongo Cat Auto Chest. This is an independent, unofficial project; it is not affiliated with Bongo Cat or Steam.
