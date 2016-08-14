# CSharp-Navigation-Service

[![Build status](https://ci.appveyor.com/api/projects/status/k3aeppeyvjf4db89/branch/master?svg=true)](https://ci.appveyor.com/project/ColinWilliams/csharp-navigation-service/branch/master) [![NuGet](https://img.shields.io/nuget/v/CSharp-Navigation-Service.svg?maxAge=2592000)](https://www.nuget.org/packages/CSharp-Navigation-Service/)

## What is it?
**NOTE: This is still in early development. Use at your own risk. Breaking changes will most likely be introduced.**

This is a C# library for handling page navigation for Universal Windows Platform (UWP) applications. It takes the SuspensionManager and NavigationHelper files from the Visual Studio App Templates, adds the ability to pass complex objects during page navigation, and makes it a library.

Key features:
* Register and unregister ALL your frames, with multiple frame support.
* Pass complex, custom objects as the parameter between pages during navigation
* Full support for app suspension and resuming
* Sets the default page cache mode to be "Enabled"
* Can be used in an MVVM model application structure

## How to use it?
The NavigationService is the entry-point to the library and is where all of you interactions will take place. It mimicks the navigation methods of the Frame class.

### Sample Apps
* See the Sample App projects for an example of how to set things up with both a single Frame and multiple frames
* They are heavily commented with explanations
* They can be used as a starting point for a new project

### Setup
* Modify your App.xaml.cs to:
  * Change the default page cache mode, if desired
  * Register your root frame with the NavigationService
  * If your app was "Terminated" last, restore the NavigationService's state
  * In your app's OnSuspending event handler, call NavigationService.SaveState()
* Modify your pages to inherit from PageBase
* Modify your ViewModels to implement INavigatableViewModel

### Navigation
* When your ViewModel is activated, save the NavigationSerice provided to it so that it can be used for navigation.
* Call NavigationService.Navigate(Type pageType, NavigationContextBase context) to navigate, passing either null or your custom navigation context
* Call any of the other navigation methods as needed
* If you want to navigate with a NavigationService for a different frame, provide access using an IOC or other global method.

## Development

### Building
* Requirements: Visual Studio 2015 Community (other editions, such as Enterprise, may work as well but are untested)
* Clone the repository
* Open the solution in Visual Studio
* Choose Build -> Build Solution to build

### Sample Apps
* In addition to providing an example for users of the library, the Sample Apps should be used for smoke testing before check-in, particularly while there aren't unit tests.
* "Sample" shows usage with a single frame.
* "SampleMultipleFrames" shows usage with multiple frames.
* Both Sample Apps should be smoke tested before check-in as sometimes changes can crop up in one or the other.
* The Sample App should already be set as the Startup Project. Hit F5 (Play) to run it.

### Tests
No official testing supported yet. Unit tests planned for future.

## Known Issues
* No unit tests
