_Add mouse & keyboard support to xcloud gaming platform using virtual gamepad emulation layer._

# About
The main objective of this software is to provide mouse & keyboard support for xcloud using an emulation layer directly through a virtual gamepad.

It will not be necessary to install any extensions in chrome. I personally did this because there were other plugins/extensions that did the same but in a poorly optimized way or didn't work correctly with what I wanted. So I decided to implement this from scratch.

# Requeriments
- .NET 7.0 Desktop Runtime - [https://dotnet.microsoft.com/pt-br/download/dotnet/7.0](https://dotnet.microsoft.com/pt-br/download/dotnet/7.0) (If you wanna make changes, clone or even make PRs you must install .NET 7.0 SDK, otherwise .NET 7.0 Desktop Runtime is enough)

- ViGEm Bus (Add capability to emulate virtual gamepad using a kernel mode driver) - [https://github.com/ViGEm/ViGEmBus](https://github.com/ViGEm/ViGEmBus)

- Windows Machine


# Notes

- Sice **ViGEm Bus Driver** is a windows kernel mode driver, we can only run on windows. If you find some multiplatform way to add a kernel mode driver you can notify me.

- Tested also transport input data using websockets, but the huge delay between pasing/(de)serializing data tooks a huge time and the result was a very bad experience.

# Special Thanks

- [ViGEm Team](https://github.com/ViGEm) to provide this amazing library for managing usb virtual gamepad devices

- [ChatGPT](https://chat.openai.com/) to help me understand some calculations in mouse delta movement.

# Remarks (Current Version) 

_Aka: Things that i'm working on in a future version_

- I've uploaded an early version of emulator while i was testing, so i've used an predefined and hard-coded preset for [Atomic Hearts](https://www.xbox.com/pt-BR/play/games/atomic-heart/9P731Z4BBCT3). You can change this preset by editing the source code but now i'm working in a revamped version to implement an GUI (this reason .NET Desktop Runtime is required in future update) with customizable presets and keybindings configurator.

- I've also ajusted by default update rate of input to 16ms. This means the thread will update controller buttons, values, triggers and D-Pad at 60hz. This was enough for me in my internal tests with atomic hearts at max sensitivity in game settings (aim looks nice, moving too. Major problem still in aim, i will test some other win32 apis later).

- I (in next version) will implement each input in separated threads such as LEFT/RIGHT Axis both in different threads this also include buttons. DPad keys, etc. For better performance and less latency.

- Be patient, i'm working on this in my free time, but i'm trying to do my best!