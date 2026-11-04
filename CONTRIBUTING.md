# Contributing

Thanks for considering a contribution to this community fork of [NH4OH-35/bongo-cat-auto-chest](https://github.com/NH4OH-35/bongo-cat-auto-chest).

## Project status and attribution

The existing code is by NH4OH-35 and is distributed under the MIT License. This fork preserves the upstream `LICENSE` and third-party notices. Changes made in this fork should be described as fork contributions; do not imply that the original project or its code was created by this account.

This fork adds an optional control for exchanging duplicate emojis from the game's emote group. It starts off, leaves manual exchange slots untouched, and waits at least three seconds between attempts. The game's `Assembly-CSharp.dll` is proprietary and must never be committed or bundled. Do not include API keys, Steam credentials, game assemblies, or other proprietary game files.

## Proposing a change

1. Open an issue first for substantial changes, compatibility changes, or new integrations.
2. Keep changes focused and explain the user-visible behavior and compatibility impact.
3. Preserve upstream copyright and license notices in copied or modified files.
4. Add or update tests for behavior changes where practical, and document any manual verification.
5. Never commit secrets, generated game binaries, Steam files, or copies of proprietary game assets.
6. Submit a pull request with a concise summary, testing details, and any limitations.

## Compatibility and updates

Bongo Cat updates may change the game assembly layout and break a patch. Keep compatibility checks explicit, fail safely on unsupported builds, and document which game versions were verified. Do not claim automatic update compatibility unless it has been demonstrated.

## License

By submitting a contribution, you agree that your contribution is made available under the repository's MIT License, while preserving existing third-party notices and copyrights.
