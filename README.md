# WardIsLove GUI Translator

Created by **Dorn**, WardIsLove GUI Translator translates the WardIsLove interface into the language selected in Valheim. It is a client-side add-on developed for WardIsLove 4.0.9.

Brazilian Portuguese is included. Other languages require a translation file; untranslated text remains in English.

## Installation

1. Install BepInEx and WardIsLove in your Valheim profile.
2. Place `WardIsLove.GUITranslator.dll` in the profile's `BepInEx/plugins` folder.
3. Start the game and select your preferred language in the settings.

Each player who wants to use the add-on must install it on their own computer. It does not need to be installed on the server.

## Adding or customizing translations

1. Copy `Translations/Template.json`.
2. Rename the copy to match Valheim's internal language name, for example `Spanish.json`.
3. Open the file in a text editor and translate the values to the right of each colon. Keep the English text on the left unchanged.
4. Save the file as UTF-8 in `BepInEx/config/WardIsLoveInterfaceTranslations`.
5. Restart the game to load the file.

Example:

```json
{
  "General": "General",
  "Access": "Acceso",
  "Protection": "Protección"
}
```

Preserve placeholders such as `{0}` and formatting tags such as `<color=...>`. Do not add comments or a comma after the last entry. Translation files may contain only the entries you want to translate.

To customize the included Brazilian Portuguese translation, create a file named `Portuguese_Brazilian.json` in the same folder. `Translations/Portuguese_Brazilian.json` is a complete example.

This add-on translates the ward interface. Other WardIsLove text uses the original mod's translation system.
