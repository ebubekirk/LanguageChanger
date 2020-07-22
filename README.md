# LanguageChanger
C# Forms Change Language Dynamically

To use directly in your project edit your App.config file responsively.
English is default language in my case!

  < appSettings >
    < add key="language" value="en"/ >
    < ... >
  < /appSettings >
    < /code >

Add LanguageChanger.cs class to your project and you are ready to go!

To change your language just call ChangeLanguage function.

For French:
LanguageChanger.ChangeLanguage("fr");
