# What NitroOS adds to ReactOS

[← Back to the front page](README.md)

Everything on this page refers to release `2026.11-2` (17 November 2026) unless it says otherwise.

- [Major enhancements](#major-enhancements)
- [Technical details](#technical-details)

## Major enhancements

What NitroOS adds to ReactOS, as of release 2026.11-2.

| | |
|---|---|
| **Apps** | |
| Current apps | Chrome 157, Firefox 159, Edge, VS Code 1.145 and PowerShell 7.6 install and work. Edge runs with its security sandbox switched on. |
| 32-bit and 64-bit programs together | The 64-bit version also runs 32-bit programs, as Windows does. You can run 64-bit and 32-bit Firefox at the same time; Task Manager marks the 32-bit ones with `*32`. |
| .NET Framework 4.8 and PowerShell 7 | Programs written for .NET Framework 4.8 run. PowerShell 7 comes preinstalled, including its network, storage, scheduled-task and local-account modules. |
| Recognised as Windows 10 | NitroOS reports itself as Windows 10 (version 10.0, build 19045), so installers that check the Windows version carry on instead of refusing. |
| Optional features | The Features dialog (or `featmgr` on the command line) installs extras with one click: PowerShell 7, the Scoop package manager, .NET Framework 4.8, Firefox, an AAC audio decoder, software 3D graphics and WinFsp. |
| Terminals and scheduled tasks | Modern terminals work: the VS Code terminal, Windows Terminal and PowerShell 7. Scheduled tasks run, so programs can keep themselves up to date. |
| **System** | |
| All your CPUs and memory | Multi-processor support is on by default in both the 32-bit and 64-bit versions, and a 4 GB machine shows 3.99 GB of usable memory. |
| Memory paging | On 64-bit, memory that programs are not using is moved to the paging file, so more programs fit. A program that asks for too much memory gets an error instead of bringing down the whole system. |
| Much faster memory handling | Up to 134 times faster than the ReactOS code it builds on, in our benchmarks. ([benchmarks](BENCHMARKS.md)) |
| Accurate time in virtual machines | The clock stays accurate even under heavy load (drift of 0.004% or less). |
| **Files and storage** | |
| Disk Management | A Windows 7-style Disk Management tool that draws each disk and its partitions. Right-click a partition to create, format, extend or shrink a volume or change its drive letter, or set up a new disk as MBR or GPT. If an option is greyed out, the status bar tells you why. Before it changes anything, it checks that the disk is still the way you saw it, and afterwards it checks the result. |
| NTFS by default | A full read/write NTFS driver, based on the Linux kernel's NTFS code, with a journal that recovers the disk after a power cut, and a disk checker. Setup selects NTFS for you. |
| Btrfs | Updated to WinBtrfs 1.10, with our own fixes: disks formatted on current Linux systems open, `format /FS:BTRFS` works, and drive letters stay the same. |
| Faster file writes | Programs that write large files straight to disk, such as databases and installers, write about 1.5 to 2 times faster than in the previous release in our tests, because NTFS now records several such writes in its journal at once. A file's data still reaches the disk before its new size is recorded. |
| WinFsp | WinFsp installs, so file systems written as ordinary programs can be mounted as drive letters or network paths. |
| **Devices and graphics** | |
| A dependable Device Manager | A device you disable stays disabled after a restart (the boot disk is protected from this). If enabling or disabling a device fails, you are told. Installing a new driver for a running device restarts the device so the new driver takes effect. Driver installation runs in a protected system service. |
| Graphics | A display driver for virtio-gpu virtual machines, optional software OpenGL and Vulkan on 64-bit, and the first parts of Windows' modern composition interfaces, enough for WebView2 and Edge to draw their windows. |
| **Networking and security** | |
| IPv6 | IPv6 is on by default, as on Windows. NitroOS gets its addresses from your router, uses temporary private addresses for outgoing connections, supports DHCPv6, and chooses between IPv6 and IPv4 addresses the way Windows does. We tested it with a real home router: web pages and name lookups over IPv6 work. Its settings appear in the network connection properties, and you can turn IPv6 off with the same registry setting as on Windows. |
| A firewall that is on | The firewall is on from the first boot. It filters incoming and outgoing traffic, stores its rules the same way Windows does, has the usual Control Panel and Advanced Security tools, and asks you when a blocked program tries to accept connections. |
| Networking without administrator rights | Programs running as a standard user can use the network normally. `ipconfig` and `net` show the same output as on Windows, and the HTTP Server API, `netsh portproxy` and TLS through schannel work. |
| Security hardening | Registry keys carry real permissions that are inherited as on Windows (checked and logged, not yet enforced), a secure random number generator, many security fixes across the kernel, the graphics subsystem and networking, and browsers that run inside their security sandbox. |
| **Desktop** | |
| A nicer desktop | Sharper LCD text, extra open fonts, a Windows-style crash report dialog, copy and paste in the console with Ctrl+C and Ctrl+V or the mouse, and Windows installed to `C:\Windows`. |

How fast it is: see the [speed table](README.md#speed) and [BENCHMARKS.md](BENCHMARKS.md).

## Technical details

For developers: the main changes and notable bug fixes in 2026.11-2, grouped by area. The release contains 453 changes in total; the [changelog](CHANGELOG.md) lists every one, with a link to its commit.

### Networking and IPv6

IPv6 is on by default: SLAAC with RDNSS/DNSSL, RFC 8981 temporary addresses (on by default as in Windows 10), a stateless and stateful DHCPv6 client (`ipconfig /renew6` and `/release6`), DAD, MLD listener reports, RFC 6724 destination ordering and source selection with the Windows prefix policy table, raw IPv6 sockets with Windows' access rule, IPv6 firewall rules, `netsh interface ipv6`, the TCP/IPv6 entry and properties page in the network connection properties (check boxes read-only in this release), and IPv6 in iphlpapi, WinHTTP, WinINet and the PowerShell network modules. `Tcpip\Parameters\Ipv6Enable = 0` and `Tcpip6\Parameters\DisabledComponents` leave IPv6 on the loopback interface only, as on Windows.

- IPv4 groups joined with `IP_ADD_MEMBERSHIP` reach the socket while IPv6 is on (they were never put in the adapter's multicast list).
- A UDP datagram answered by ICMP or ICMPv6 port unreachable fails the next receive with `WSAECONNRESET` unless `SIO_UDP_CONNRESET` turned that off.
- The DHCP client service no longer crashes with two adapters.

<sub>Bug fixes in this area: 115</sub>

### NTFS and storage

NTFS-NG commits extending write-through writes in groups (32 MB, 1 s, flush or close; data before size kept; `NoGroupCommit=1` restores per-write commits). Directory listings and small-file reads and creates are faster, and the caches scale with RAM and shrink under memory pressure. AHCI issues 512 KB commands to 48-bit LBA disks and uses queued FUA only on drives that advertise it.

- A file whose MFT record was reused while an open raced with the delete of the earlier file is no longer stuck until reboot.
- Clusters freed by a transaction are reused only after the commit that frees them is durable.

<sub>Bug fixes in this area: 29</sub>

### Memory manager and cache

A 128 TiB user address range on amd64 (as Windows 8.1 and later), page-file-backed `SEC_RESERVE` sections up to 8 TB, a real `FlushViewOfFile`, and NitroPager reads and writes the page file in clusters of up to 256 KB with read-ahead.

- Contiguous and physical-range page searches never take a standby page: this fixes the 64-bit data corruption issue found after 2026.11-1.

<sub>Bug fixes in this area: 41</sub>

### Kernel, registry and security

Registry keys store real security descriptors, inherited from the parent as on Windows (auto-inheritance, protected keys, owner and SACL handling); access checks run in audit mode and refuse nothing yet. A ChaCha20 system random number generator backs `RtlGenRandom` and `BCryptGenRandom`. A minifilter-grade Filter Manager, wait completion packets and reserve objects, NTLMv2 without an external helper, service virtual accounts.

- A process or thread handle can no longer be opened before its creator inserted the object (bugcheck 0x1E).
- 8 security fixes in the kernel, win32k, RPC and networking; details are withheld until users have had time to update.

<sub>Bug fixes in this area: 76</sub>

### Display, compositor and win32k

More of Windows.UI.Composition and DirectComposition (DesktopWindowTarget, CompositorController, nine-grid brushes, interaction trackers): Word reaches its Start screen. Redirected window surfaces present only the dirty rectangles (off by default).

<sub>Bug fixes in this area: 69</sub>

### System DLLs, WoW64 and console

The heap front end follows the Windows 10 thresholds; vectored exception handling is lock-free; `QueueUserWorkItem` is 3.1 times faster; 231 locales from Unicode CLDR 49 and 143 time zones in `tzres.dll`. 32-bit programs can save and load registry hives and get more window messages translated.

<sub>Bug fixes in this area: 72</sub>
