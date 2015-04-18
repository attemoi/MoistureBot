# MoistureBot

MoistureBot is an extensible chat bot for steam based on [SteamKit2](https://github.com/SteamRE/SteamKit). 
It uses [Mono.Addins](http://monoaddins.codeplex.com/) to provide extensibility.

![Example usage](/assets/example_usage.gif?raw=true "Example usage")

## Dependencies

In order to run or build MoistureBot, you will need .NET 4.0 or [Mono ≥ 2.8](http://mono-project.com). You can check your current Mono version by typing `mono --version` into your command line.

### Building on Linux

Tested on: Debian GNU/Linux 7 (wheezy), Mono JIT compiler version 2.10.8.1 (Debian 2.10.8.1-8)

```
cd ~
git clone https://github.com/attemoi/MoistureBot.git
cd MoistureBot
xbuild
```

After a successful build, the binaries will be in _MoistureBot/bin/Debug_ and can be run with `mono MoistureBot.exe`

#### Possible issues
  - Nuget can't find referenced libraries:
    * Try to import required certificates with `mozroots --import --sync`

### Building on Windows

Building on Windows is the same as for linux, but you may also use msbuild instead of xbuild if you don't have Mono installed.

## Creating addins

You can start by taking a look at some of the sample plugins listed below. In short, an addin only needs three things to work:   
  1. A reference to _MoistureBotLib.dll_ (included in the project)
  2. A file named _PluginName.addin.xml_ for configuring what the addin extends.
  3. Your own fantastic addin code, that implements the wanted extension point interfaces defined in MoistureBotLib.dll.

If you want to know more about how the addin system works, see [http://monoaddins.codeplex.com/](http://monoaddins.codeplex.com/).

The project includes the following sample addins: 

  - **Moikkaaja:** reply to greetings defined in an xml file.
  - **AutoInviteJoin:** automatically join group chats when invited.
  - **AutoReconnect:** automatically check connection every 10 minutes and reconnect and join favorite rooms if needed.
  - **GameInviteReply:** automatically reply to game lobby invites with random message sequences selected from an xml file.
  - **EmbedlyUrlInfo:** reply to video urls with title using [embed.ly](http://embed.ly/) API.
  - **SQLiteChatLogger:** log chat messages and urls to SQLite database.
  - **UrlHistoryCommand:** Displays last 20 urls from chat log when user types !urlhistory.
  - **HistoryCommand:** Displays last 20 messages from chat log when user types !history.

## License

This project is licensed under the [MIT](http://opensource.org/licenses/MIT) license.
