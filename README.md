﻿![GrowOne](./media/banner.svg)

A DIY IoT project that helps keeping your plant happy - powered by nanoFramework, Preact and Pico.css

## About

Keeping indoor plants can have 
[lots of benefits](https://www.healthline.com/health/healthy-home-guide/benefits-of-indoor-plants) - 
they can boost your productivity, improve your mental health or reduce your stress levels.
Some even like taking it a step further - and grow their own citrus plants or 
[chillies](https://chili-plant.com/chili-care/chili-plants-in-the-house/)! But as rewarding as 
making your very own chilli sauce might be, checking and watering your plant multiple times a day 
for months can be a challenge.

_GrowOne_ can make this process a whole lot easier: Fill its reservoir with water, put all sensors in 
place - and do the rest over the included web app. Set up the automatic watering system to keep the
soil moisture in range, supervise the temperature, air humidity or reservoir fill level, even 
configure additional acoustic warnings in case any of the parameters exceed a specific range - 
allowing you to safely leave your plant for a few days without having to worry.

While the setup of the actual device can vary depending on the specific use-case, it will most 
likely consist of a water reservoir with a magnetic valve or pump, that is - among the other 
sensors for determining the soil moisture, air temperature/humidity and reservoir fill level - 
connected to a microcontroller with WiFi (like the 
[ESP32-WROOM-32](https://en.wikipedia.org/wiki/ESP32#Printed_circuit_boards)).

The software for the microcontroller is written in C# (using 
[nanoFramework](https://www.nanoframework.net/)). It provides a HTTP REST API and a web app, 
written in JavaScript (ES11), HTML5 and CSS (using [Preact](https://preactjs.com/) and 
[Pico.css](https://picocss.com/)).

### User interface demo

https://user-images.githubusercontent.com/21185621/186000170-53a2dfa1-3c39-4168-be37-728cdfa24bcd.mp4

## Setup overview

_GrowOne_ can either be deployed directly using the most recent 
[deployment image](https://github.com/bauermaximilian/GrowOne/releases) and 
[nanoff](https://github.com/nanoframework/nanoFirmwareFlasher),
or compiled and deployed manually from source using the Visual Studio 2022 solution, which 
requires the nanoFramework extension for Visual Studio. With this extension being installed, 
the solution can then be opened and either directly executed and debugged (which gives access to 
useful debug output), or built and deployed to the board. 

Please note that, in any case, the WiFi network must be configured on the ESP32. This can be easily 
done using the nanoFramework extension for Visual Studio - after filling out all the required fields,
the WiFi options should be set to "Enable".

Depending on the requirements (and the available parts and budget), the setup of the hardware can 
vary: Not all supported sensors have to be connected, the irrigation can be done using a pump or 
a magnetic valve and so on.

[The wiki](https://github.com/bauermaximilian/GrowOne/wiki/) contains a more in-depth description 
on how to get the software running on your microcontroller and how to connect the supported 
hardware components with each other.

## License

GrowOne - Copyright (C) 2022 Maximilian Bauer

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.

The _GrowOne_ banner and the _GrowOne_ logo (the files "banner.svg", "logo.svg" and "logo.ai")
are licensed under [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/).

GrowOne uses several third-party libraries under different licenses. See the file "LICENSE" for 
more information.
