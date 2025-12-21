# Multitool

**Multitool** is a C# console-based utility designed for network diagnostics, system information gathering, and Discord integration. It brings together several administrative commands into a single, easy-to-use menu interface.

> **Author:** M3galodn81
>
> **Platform:** Windows (Requires `netsh` and `ipconfig`)

## Features

1. **Discord Webhook Sender**

    * Send custom messages to a private Discord server via a webhook URL.

2. **MAC Address Lookup**

    * Retrieves the Physical (MAC) Address of all active Network Interface Cards (NICs) on the machine.

3. **Local Network IP Scanner**

    * Performs a ping sweep on the local subnet (`192.168.1.x`) to identify active devices.

4. **DNS History Viewer**

    * Parses the local DNS cache (`ipconfig /displaydns`) to show a list of recently visited domains.

5. **Wi-Fi Profile Extractor**

    * Scans the system for saved Wi-Fi profiles and retrieves the cleartext passwords (security keys).

## Prerequisites

* **Operating System:** Windows 10/11 (The tool relies on Windows specific commands like `netsh` and `ipconfig`).
* **Framework:** .NET 6.0, 7.0, or 8.0 (depending on your project settings).
* **Permissions:** Must be run as **Administrator** to access Wi-Fi passwords and raw socket operations.

## Configuration & Setup

### 1. Fix Missing Dependencies

The code references a class named `URLKeys` which contains the webhook URL. You must create this class for the project to compile.

Create a new file named `URLKeys.cs` in your project and add the following:

```csharp
namespace Multitool
{
    internal class URLKeys
    {
        // Replace this string with your actual Discord Webhook URL
        public static string webhook = "https://discord.com/api/webhooks/YOUR_WEBHOOK_ID/YOUR_WEBHOOK_TOKEN";
    }
}

```

### 2. Configure IP Scanner

By default, the IP scanner checks the `192.168.1.x` range. If your network uses a different subnet (e.g., `192.168.0.x` or `10.0.0.x`), modify the `IPScan` method in `Program.cs`:

```csharp
// In Program.cs
static async Task IPScan()
{
    // Change this to match your router's gateway
    string subnet = "192.168.0."; 
    // ...
}

```

## How to Run

1. Clone or download the repository.
2. Open the solution in Visual Studio or VS Code.
3. Ensure `URLKeys.cs` is created (see Configuration above).
4. Build the solution (`Ctrl + Shift + B`).
5. **Important:** Run the generated `.exe` file as **Administrator**.
    * *Right-click the executable -> Run as Administrator.*

## Disclaimer & Legal Warning

This tool includes features that extract saved passwords and scan local networks.

* **Educational Use Only:** This tool is intended for personal use on your own hardware or networks you have permission to audit.
* **Privacy:** Do not use the Wi-Fi extraction tool on public computers or computers that do not belong to you.
* **Liability:** The author is not responsible for any misuse of this software or damage caused by it.

## Contributing

Feel free to fork this project and submit pull requests for:

* Dynamic subnet detection (removing hardcoded IP ranges).
* Improved error handling for the Webhook client.
* Cross-platform support.
