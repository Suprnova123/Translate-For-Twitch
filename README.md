# Translate For Twitch
 A fun viewer interaction tool to be used by your mods or VIPs, which will translate their messages for one language to another in exchange for Bits and Subscriptions.

## Note
Because of the configurative nature of this program, you are required to build it yourself in an IDE or from a command line. Depending on how many people are going to run this program for a single stream, you might consider leaving certain parameters blank so that they can be input directly by the user, or instead custom building each .exe for each person.


## Building
Ensure you have the following installed:
* The latest version of [the .NET Core SDK](https://dotnet.microsoft.com/download).
* The latest version of [Git](https://git-scm.com/downloads).

Clone the repository:

```shell
git clone https://github.com/suprnova123/Translate-For-Twitch
cd Translate-For-Twitch
```

Open Program.cs in your preferred IDE, or alternatively in any text editor, and replace the following:
* Line 31-32: Username for the user's Twitch account and their [OAuth password](https://twitchapps.com/tmi/).
  * Either option can be left blank, as it will just ask the user for these credentials at runtime.
* Line 33: The Twitch channel where their messages will appear in.
* Line 34-35: The input and output languages for the messages being translated, written in their appropriate [language code](https://sites.google.com/site/opti365/translate_codes).
* Line 36-37: The values for each Bit and Sub in seconds.
* Line 39: The X and Y coordinates of where the window will appear, and the X and Y size of the window.
  * The default values are assuming that you are on a 1080p monitor, on a web browser at 100% zoom, on the default Twitch layout, in a maximized window.

Build the application:

```shell
dotnet publish -p:PublishProfile=FolderProfile
```

If the publish fails, attempt the following:

```shell
dotnet restore
dotnet publish -p:PublishProfile=FolderProfile
```
**OR**
```shell
dotnet add package Google.Cloud.Translation.V2 --version 2.0.0
dotnet add package TwitchLib --version 3.1.1
dotnet publish -p:PublishProfile=FolderProfile
```

The resulting .exe file will be in **\<working directory>\bin\Release\netcoreapp3.1\publish**.
