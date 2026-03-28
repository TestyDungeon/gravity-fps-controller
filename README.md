<div align="center">
<h2> Non-Linear Gravity Character Controller for Unity </h2>

</div>

## 💡 Overview


A custom character controller with custom collision detection and handling, that allows for different gravity directions in specific areas 


![Demo](Gifs/demo1.gif)


and around spheres.


![Demo](Gifs/demo2.gif)


## ✨ Features
- Linear and spherical gravity fields
- Custom player collision using "collide and slide" algorithm
- Input handling
- Smooth movement, rotation and jumps
- Camera rotation system
- Custom ground detection

! Scripts in FixedInterpolation folder are not mine, but from another open source project !

## 🎮 Installation

### Option 1: Unity Package (Recommended)
Download the latest .unitypackage which includes scripts, player prefab and a demo scene.

### Option 2: Manual Installation
1. Copy all files from the `Scripts/` and 'Prefabs/' folders.
2. Paste into your Unity project.
3. Create an object in your scene and add 'Interpolation Controller' script to it.
4. In Project Settings / Script Execution Order, make sure that 'InterpolationController' is -100, 'InterpolatedTransform' is -50, 'InterpolatedTransformUpdater' is 100.
5. Add the Player prefab to the scene.
6. Add the GravityField prefabs to whatever object you want.

## Requirements
- Legacy Input Manager (or adapt for New Input System yourself)

## License 
MIT License - see LICENSE file
