# Pegginglin - Buttplug.io mod for Peglin

This is a simple BepInEx mod for Peglin with the following features:

- Runs vibrator for 100ms whenever a peg is hit

## How to install

- Download from [Thunderstore](https://thunderstore.io/c/peglin/p/nonpolynomial/Pegginglin/) using r2modman
- Manual install if that doesn't work
  - Download [BepInEx v5.4.23.2](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2) for your
    specific architecture (most likely win_x64)
  - Unzip the BepInEx directory to your Peglin install directory
  - Download latest Pegginglin release from [github](https://github.com/qdot/pegginglin/releases)
  - Unzip into `plugins` directory in the `BepInEx` directory of your Peglin install

## How to use

- Start [Intiface Central](https://intiface.com/central)
- Start Peglin
- Soon after install, Intiface Central should show "Pegginglin" as connected
- Mod will currently vibrate any vibration capable hardware currently connected to Intiface Central

## Source

[Pegginglin source is available on github](https://github.com/qdot/pegginglin).

Note that the project is currently set up against the developer's local configuration, as nuget
Peglin packages are currently out of date.

Due to incompatibilities in the Nuget Buttplug packages and the version of .Net Peglin uses, building against the Buttplug C# library will require a local clone of the [buttplug-csharp repo](https://github.com/buttplugio/buttplug-csharp), with the `no-tuple` branch checked out.

## ChangeLog

- v0.0.3
  - README updates
  - Move to System.Net.Websockets to fix Thunderstore rejection and reduce dependencies
- v0.0.2
  - Update release to try to fix Thunderstore rejection (didn't work)
- v0.0.1
  - First release
  - Minimal vibration functionality
  - Just seeing if this will even work for anyone else