# MoistureBot

MoistureBot is an extensible chat bot for steam based on [SteamKit2](https://github.com/SteamRE/SteamKit). 
It uses [Mono.Addins](http://monoaddins.codeplex.com/) to provide extensibility.

![Example usage](/assets/example_usage.gif?raw=true "Example usage")

## Dependencies

In order to compile and use MoistureBot, the following dependencies are required:

  - .NET 4.0 or [Mono ≥ 2.8](http://mono-project.com)
  - [Steamkit2](https://github.com/SteamRE/SteamKit) (published to NuGet)
  - [Mono.Addins](http://monoaddins.codeplex.com/) (published to NuGet)
  - [INI File Parser](https://github.com/rickyah/ini-parser) by [rickyah](https://github.com/rickyah) (published to NuGet)
  - [Log4Net](http://logging.apache.org/log4net/) (published to NuGet)
  
## Creating addins

I've tried to make creating an addin as simple as possible. However, you should
probably familiarize yourself with the basic concepts of
[Mono.Addins](http://monoaddins.codeplex.com/) before trying to create one.

The project includes the following sample addins: 

  - **AutoInviteJoin:** automatically join group chats when invited.
  - **AutoReconnect:** automatically check connection every 10 minutes and reconnect and join favorite rooms if needed.
  - **GameInviteReply:** automatically reply to game lobby invites with random message sequences selected from an xml file.
  - **SQLiteChatLogger:** log chat messages and urls to SQLite database.
  - **EmbedlyUrlInfo:** reply to video urls with title using [embed.ly](http://embed.ly/) API.
  - **Moikkaaja:** reply to a number of finnish greetings.

All the interfaces needed for creating an addin are included in the MoistureBotLib library.

### Addin dependencies

In order to compile an addin, the following dependencies are required:

  - [Mono.Addins](http://monoaddins.codeplex.com/) (published to NuGet)
  - MoistureBotLib.dll (included in this project)

## License

This project is licensed under the [MIT](http://opensource.org/licenses/MIT) license.
