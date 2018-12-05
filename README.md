# Edity McEditface
Some tools to play around with static websites using .net.

## How to Build
After cloning the repository go to the root folder of the repo on your command line. Then run the following commands:
```
cd EdityMcEditface
dotnet restore
yarn run yarn-install
yarn run build
dotnet build
```

To run the app use the following command:
```
dotnet run --workingDir="your/target/dir"
```
Replace your/target/dir with the path to the git repo for your website.

This should run the app. It will print out the url that you should access it with using your browser. This should show you the site with the default menu. Visiting the url for any of the files in your git repo should show that page.

## Setting up Yarn
 setting up yarn
 1. npm install -g yarn
 1. yarn global add typescript
 1. yarn global add threax-npm-tk
 1. %localappdata%\Yarn\bin on path (expand it with explorer first and paste in the full path)
 1. cd %localappdata%\Yarn\config\global\node_modules\threax-npm-tk
 1. yarn link
 1. then you can yarn link threax-npm-tk from your projects
 1. In visual studio type package restore in quick launch and turn off the automatic package restore for everything so it won't wipe out our yarn code
