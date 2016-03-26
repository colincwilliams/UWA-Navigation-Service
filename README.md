# CSharp-Navigation-Service

## What is it?
**NOTE: This is still in early development. Use at your own risk. Breaking changes will most likely be introduced.**

This is a C# library for handling page navigation in modern Windows applications (Windows 8 and greater). It takes the SuspensionManager and NavigationHelper files from the Visual Studio App Templates, adds the ability to pass complex objects during page navigation, and makes it a library.

Key features:
* Register and unregister your root frame. * See "Known Issues" below for multiple frame support
* Pass complex, custom objects as the parameter between pages during navigation
* Full support for app suspension and resuming
* Sets the default page cache mode to be "Enabled"

## How to use it?
The NavigationService is the entry-point to the library and is where all of you interactions will take place. It mimicks the navigation methods of the Frame class.

### Setup
* Modify your App.xaml.cs to:
  * Change the default page cache mode, if desired
  * Register your root frame with the NavigationService
  * Save the returned NavigationService for future use with an IOC or other method of your choosing
  * If your app was "Terminated" last, restore the NavigationService's state
  * In your app's OnSuspending event handler, call NavigationService.SaveState()

### Navigation
* Get the NavigationService for the Frame you want to navigate
* Call NavigationService.Navigate(Type pageType, NavigationContextBase context) to navigate, passing either null or your custom navigation context
* Call any of the other navigation methods as needed

## Development

### Building
* Requirements: Visual Studio 2015 Community (other editions, such as Enterprise, may work as well but are untested)
* Clone the repository
* Open the solution in Visual Studio
* Choose Build -> Build Solution to build

### Testing
No official testing supported yet. Unit tests and sample app planned for future which will support this.

## Known Issues
* No test/sample app
* Multiple frame support is untested and unverified. In theory it works?
* No unit tests
