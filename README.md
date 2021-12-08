# Visualizing File Structures in Virtual Reality

## About

Hello! Welcome to the repository used to store our CMSC 436 visualization project code. Our group explored the depiction of file structures within virtual reality. To use this project it is recommended that you have installed Unity 2020.3.19f1. At the moment, this project supports Windows only, and has been tested with the Meta Quest 1 and 2.

## How to Use

The Unity project is contained within the `HFS-group-proj` directory located in the root of this repository. Open the project directly through the editor or through the Unity Hub. Open the Unity scene `FileSystemScene` by selecting `Assets/Scenes/FileSystemScene.unity` through the Project panel. Select the `Reader` GameObject and paste the full path of your target directory into the "Root Path" property of the Reader script. For your convenience, a default directory path has been chosen that will work out of the box with the project. You may need to install the project's required packages through the Unity Package Manager. The console may also provide any additional configuration instructions, should you need any.

### XR Device Simulator

To quickly view the visualization without the usage of a head mounted display (HMD), select the `XR Device Simulator` GameObject located in the Hierarchy panel and verify that the object is enabled within the Inspector. Ensure that you do not have any HMDs tethered to your host machine. Press play.

### HMD

To view the visualiztion through an HMD, begin by tethering your device to your host machine. If you are using a Meta HMD, your device will need to have developer mode enabled. Disable the `XR Device Simulator` GameObject from within the Inspector panel. Press play.

### Controls

The project is fully interactable through both the XR Device Simulator (XDS) and an HMD. Controllers can be manipulated through the XDS by using the Shift buttons located on either side of the keyboard for the left and right controllers respectively.
- To rotate the current platform's file contents use either controller's thumbstick x axis. 
- To teleport between platforms, hover over a directory with a controller, and press the "grip" button (Shift + G on the XDS). 
- To visit the platform's parent, turn around and "grip" the directory on your current platform. 
- Cycle through the file ring sorting methods with the primary buttons on either controller (Shift + B / N on the XDS).