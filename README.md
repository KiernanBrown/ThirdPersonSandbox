# Third Person Sandbox
A third person sandbox in Unity to develop action game projects or experiment with action game elements.

## Table of Contents
* [Features](#features)
* [Credits](#credits)

## Features <a name="features"></a>
* A sample scene where you can move a character around in third person and the camera follows you.
* The foundation of a combo system. Left clicking allows you to attack, and doing attacks in succession allows you to do up to a four hit combo. You can stop for a brief period after an attack to restart the combo.
![Imgur Gif of 4 Hit Combo](https://i.imgur.com/jfEbgGu.gif)
* The foundation of combo modifiers, which are moves that can be used to modify a combo, but can only be used mid combo. Right clicking after doing one of the first three hits in the combo allows you to perform a stomp, which restarts your combo.
![Imgur Gif of Stomp Combo Modifier](https://i.imgur.com/rtAYrbf.gif)
* The foundation of guarding. Pressing Left Shift while you are not doing a directional input allows you to guard. You cannot cancel an attack that you have already started by guarding. Guarding during a combo will restart your combo.
* The foundation of evading. Pressing Left Shift while you are doing a directional input allows you to cartwheel in the direction you are moving. You cannot cancel an attack that you have already started by evading. Evading during a combo will restart your combo.

## Credits <a name="credits"></a>
* Filmstorm - Third Person Camera Tutorial and Open World Movement System Tutorial. This helped to create the basic movement script for the character controller and helped with setup for the Cinemachine Third Person Camera.
* Adobe Mixamo - The character XBot and all animations are from Mixamo.