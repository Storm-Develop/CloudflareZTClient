# CloudflareZTClient
Cloudflare Zero Trust Mock Client is designed to communicate with the daemon server.
## Platform: MacOS 13

## Description
Cross Platform app created using Xamarin Forms.

Targeted MacOS client in order to communicate with daemon server. However, it can still be run on Windows there's UWP project included just not going to be able to interact with the server itself.

> I've created a couple of videos I recommend to check out describing the project structure, unit tests and GUI interaction. 
## App Download
You can download the app package [here](https://github.com/Storm-Develop/CloudflareZTClient/releases/download/v1.0/CloudFareZTClient.zip)

### Installation Instructions:
Once downloaded and unpacked zip file. Navigate to Privacy&Security and in the Security section select AppStore and identified developers. Below there should be an option to open the CloudFareZTclient app.
![Image](https://github.com/Storm-Develop/CloudflareZTClient/blob/dev/ImagesUsedInReadme/photo_2023-09-14_20-30-13.jpg)

## Compilation
In order to compile the project you can use Visual Studio Community 2022 for Mac.

Recommend to use the latest: **Version 17.6.4**

> Note: *Visual Studio should highlight & suggest installing additional dependencies if you are missing some components.*
<details>
  <summary>Here's a list of all components installed on my PC:
</summary>
Visual Studio Community 2022 for Mac
Version 17.6.4 (build 472)
Installation UUID: ac4871a0-982f-4001-9e37-b98c6fd3c13c

Runtime
.NET 7.0.3 (64-bit)
Architecture: Arm64
Microsoft.macOS.Sdk 13.1.1007; git-rev-head:8afca776a0a96613dfb7200e0917bb57f9ed5583; git-branch:release/7.0.1xx-xcode14.2

Roslyn (Language Service)
4.6.0-3.23180.6+99e956e42697a6dd886d1e12478ea2b27cceacfa

NuGet
Version: 6.4.0.117

.NET SDK (Arm64)
SDK: /usr/local/share/dotnet/sdk/7.0.308/Sdks
SDK Versions:
  7.0.308
  7.0.306
  7.0.202
  6.0.414
  6.0.412
  6.0.407
MSBuild SDKs: /Applications/Visual Studio.app/Contents/MonoBundle/MSBuild/Current/bin/Sdks

.NET SDK (x64)
SDK Versions:
  6.0.414
  6.0.412
  6.0.106
  6.0.105
  6.0.104
  6.0.103
  6.0.102
  6.0.101
  5.0.408
  5.0.407
  5.0.406
  5.0.405
  5.0.404
  3.1.426
  3.1.420
  3.1.419
  3.1.418
  3.1.417
  3.1.416

.NET Runtime (Arm64)
Runtime: /usr/local/share/dotnet/dotnet
Runtime Versions:
  7.0.11
  7.0.9
  7.0.4
  6.0.22
  6.0.20
  6.0.15

.NET Runtime (x64)
Runtime: /usr/local/share/dotnet/x64/dotnet
Runtime Versions:
  6.0.22
  6.0.20
  6.0.6
  6.0.5
  6.0.4
  6.0.3
  6.0.2
  6.0.1
  5.0.17
  5.0.16
  5.0.15
  5.0.14
  5.0.13
  3.1.32
  3.1.26
  3.1.25
  3.1.24
  3.1.23
  3.1.22

Xamarin.Profiler
Version: 1.8.0.49
Location: /Applications/Xamarin Profiler.app/Contents/MacOS/Xamarin Profiler

Updater
Version: 11

Apple Developer Tools
Xcode: 14.3.1 21815
Build: 14E300c

Xamarin.Mac
Version: 9.3.0.6 Visual Studio Community
Hash: 97731c92c
Branch: xcode14.3
Build date: 2023-04-11 22:38:35-0400

Xamarin.iOS
Version: 16.4.0.6 Visual Studio Community
Hash: 97731c92c
Branch: xcode14.3
Build date: 2023-04-11 22:38:36-0400

Xamarin Designer
Version: 17.6.3.9
Hash: 2648399ae8
Branch: remotes/origin/d17-6
Build date: 2023-09-07 02:05:20 UTC

Xamarin.Android
Version: 13.2.1.2 (Visual Studio Community)
Commit: xamarin-android/d17-5/a8a26c7
Android SDK: /Users/home/Library/Developer/Xamarin/android-sdk-macosx
  Supported Android versions:
    11.0 (API level 30)
    10.0 (API level 29)
    9.0  (API level 28)
    13.0 (API level 33)

SDK Command-line Tools Version: 7.0
SDK Platform Tools Version: 33.0.3
SDK Build Tools Version: 32.0.0

Build Information: 
Mono: d9a6e87
Java.Interop: xamarin/java.interop/d17-5@149d70fe
SQLite: xamarin/sqlite/3.40.1@68c69d8
Xamarin.Android Tools: xamarin/xamarin-android-tools/d17-5@ca1552d

Microsoft Build of OpenJDK
Java SDK: /Library/Java/JavaVirtualMachines/microsoft-11.jdk
11.0.16.1
Android Designer EPL code available here:
https://github.com/xamarin/AndroidDesigner.EPL

Eclipse Temurin JDK
Java SDK: /Library/Java/JavaVirtualMachines/temurin-8.jdk
1.8.0.302
Android Designer EPL code available here:
https://github.com/xamarin/AndroidDesigner.EPL

Android SDK Manager
Version: 17.6.0.50
Hash: a715dca
Branch: HEAD
Build date: 2023-09-07 02:05:26 UTC

Android Device Manager
Version: 0.0.0.1309
Hash: 06e3e77
Branch: HEAD
Build date: 2023-09-07 02:05:26 UTC

Build Information
Release ID: 1706040472
Git revision: 0b8c2cb9f01ef14a2b07ff4ea047268c8756fee6
Build date: 2023-09-07 02:03:50+00
Build branch: release-17.6
Build lane: release-17.6

Operating System
Mac OS X 13.2.1
Darwin 22.3.0 Darwin Kernel Version 22.3.0
    Mon Jan 30 20:39:35 PST 2023
    root:xnu-8792.81.3~2/RELEASE_ARM64_T8103 arm64
</details>

## [GUI Overview](https://youtu.be/7GnAxm0GyOI)

[![GUI Overview](https://github.com/Storm-Develop/CloudflareZTClient/blob/dev/ImagesUsedInReadme/Screenshot%202023-09-14%20221133.png)](https://youtu.be/7GnAxm0GyOI)

## [Project Overview](https://youtu.be/LxsakqWPrT4?si=IFrQG-AeT6gQ6tvb)
[![Project Overview](https://github.com/Storm-Develop/CloudflareZTClient/blob/dev/ImagesUsedInReadme/Screenshot%202023-09-14%20221414.png)](https://youtu.be/LxsakqWPrT4?si=IFrQG-AeT6gQ6tvb)

## [Unit Tests Overview](https://youtu.be/anQMeQBaoao)
[![Unit Test Overview](https://github.com/Storm-Develop/CloudflareZTClient/blob/dev/ImagesUsedInReadme/Screenshot%202023-09-14%20221259.png)](https://youtu.be/anQMeQBaoao)

