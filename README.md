# PowerShell Host Management CmdLets

## Overview

This project provides a set of PowerShell CmdLets that can be used to add, remove and modify
entries in your system's host file. This is normally at \windows\system32\drivers\etc\hosts.

## Build

The solution provided is a VS 2015 solution and builds the primary project and associated test project. 
I will attempt to add more build options as I have time.

## Installation

Currently there is no installation script or application to install the PS assembly. There are two recommended
ways to install the module:
 * **Manually Load as Needed** - Build the project and take the resulting ManageHosts.dll and place it somewhere of your choosing. In a PowerShell command prompt you would then run, `Import-Module {Path-To-Your-DLL}`.
 * **Use PS Module Directories** - Install the module in your local PS Module path. This will cause it to load for each PowerShell instance. To do this:
  1. Build the solution. 
  2. Take the Release directory, from `pshostmgr/pshostmgr/bin/Release` and copy it to your {PROFILE}\Documents\WindowsPowerShell directory. 
  3. Rename the directory to ManageHosts for clarity. 
  
## CmdLets

This section details which CmdLets are provided by this module and shows, briefly, how to use them.

### Get-HfHost 

This will return a listing of the current host file entries. 

```
PS> Get-HfHost

| Hostname        | Address      |
| ------------- |:-------------:|
| api.domain.com | 127.0.0.1 |
| machine.test.net | 10.0.1.25      |
| other.test.net | 10.0.1.26      |
```

This list may be filtered, optionally, by hostname.

```
Get-HfHost -Hostname "api.domain.com","other.test.net"

| Hostname        | Address      |
| ------------- |:-------------:|
| api.domain.com | 127.0.0.1 |
| other.test.net | 10.0.1.26      |
```

### Add-HfHost 

This cmdlet allows a single entry to be added to the host file. This hostname must not already be in use or an exception is thrown. Both a valid hostname and address must be provided.

```
PS> Add-HfHost -Hostname "test.myhost.com" -Address "10.0.2.22"

| Hostname        | Address      |Type     |
| ------------- |:-------------:|
| test.myhost.com | 10.0.2.22 | HostForIPv4 |
```

### Remove-HfHost

The opposite of the add, this will remove a single entry from the host file by it's hostname. The hostname must be a valid entry or an error will be thrown.

```
PS> Remove-HfHost -Hostname "test.myhost.com" 
```

### Set-HfHostDestination

This is a shortcut method that can basically be accomplished with an Add-/Remove-. This provides the ability to just update a single destination host to a new address. The hostname must already exist and the address must be valid else an error will be thrown.

```
PS> Get-HfHost

| Hostname        | Address      |
| ------------- |:-------------:|
| test.myhost.com | 127.0.0.1 |

PS> Set-HfHostDestination -Hostname "test.myhost.com" -Address "10.2.22.33"

PS> Get-HfHost

| Hostname        | Address      |
| ------------- |:-------------:|
| test.myhost.com | 10.2.22.33 |
```
