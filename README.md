# MoistureBot

Note: This is still a work in progress...

MoistureBot is an extensible chat bot for steam based on [SteamKit2](https://github.com/SteamRE/SteamKit). 
It uses [Mono.Addins](http://monoaddins.codeplex.com/) to provide extensibility.

![Example usage](/assets/example_usage.gif?raw=true "Example usage")

## Dependencies

The project has so far been built only under Windows using Mono 3.2.3.

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

The project includes two sample addins: 

  - "Moikkaaja", responds to basic Finnish greetings in messages received either in group or friend chat.
  - "AutoJoiner", automatically joins group chats when invited.

All the interfaces needed for creating an addin are included in the MoistureBotLib library.

### Addin dependencies

In order to compile an addin, the following dependencies are required:

  - [Mono.Addins](http://monoaddins.codeplex.com/) (published to NuGet)
  - MoistureBotLib.dll (included in this project)

## License

This project is licensed under the [MIT](http://opensource.org/licenses/MIT) license.