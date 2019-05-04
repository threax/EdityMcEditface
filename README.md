# Edity McEditface
Some tools to play around with static websites using .net.

This guide has 2 parts. The first tells you how to build and run and assumes you have the dependencies installed. If you don't skip down to Setting Up Depenencies.

## How to Build
After cloning the repository go to the root folder of the repo on your command line. Then run the following commands:
```
cd EdityMcEditface
dotnet restore
npm install
npm run build
dotnet build
```

To run the app use the following command:
```
dotnet run --workingDir="your/target/dir"
```
Replace your/target/dir with the path to the git repo for your website. This folder should be a git repository with html files in it.

This should run the app. It will print out the url that you should access it with using your browser. This should show you the site with the default menu. Visiting the url for any of the files in your git repo should show that page.

## Basic Operation
Edity McEditface will glue together all the directories it is presented and will then treat them as 1 giant directory with files in your project taking precidence over files in the Edity McEditface site itself. By default you will get a layout that has a menu on the side and the name of your site at the top. This theme will need Bootstrap 3.3.7 or greater, but still on version 3.

If you want to override this or any file create it in your project and that version will be used instead.

## Setting Up Dependencies
There are a couple of things you will need to install to make everything work. Even though this section is last you will need to do it before you can build.

### Install Dotnet Core
First install Dotnet Core. The instructions for setting up the sdk on the [hello world tutorial](https://dotnet.microsoft.com/learn/dotnet/hello-world-tutorial) work well. You don't need to do the Create your app steps. Just install the sdk.

### Install NodeJs
You need to install a current version of [NodeJs](https://nodejs.org/).

### Install Yarn and Build Tools
Run the following commands to install everything:
```
 npm install -g typescript
 npm install -g threax-npm-tk
 ```
