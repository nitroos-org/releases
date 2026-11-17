<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="docs/img/brand/lockup-1280.png">
    <img alt="NitroOS" src="docs/img/brand/lockup-light-1280.png" width="520">
  </picture>
</p>

<p align="center"><b>An open-source, Windows-compatible operating system, built on ReactOS.</b><br>
NitroOS builds on ReactOS and adds what you need to use it every day: current 64-bit and 32-bit programs side by side, .NET Framework 4.8, PowerShell 7, NTFS as the default file system, a firewall that is switched on and IPv6 networking. Latest release: 2026.11-2.</p>

<p align="center">
  <a href="#download">Download</a> ·
  <a href="#screenshots">Screenshots</a> ·
  <a href="#quick-start">Quick start</a> ·
  <a href="#technical-details">Technical details</a> ·
  <a href="#known-limitations">Known limitations</a> ·
  <a href="#credits">Credits</a> ·
  <a href="CHANGELOG.md">Changelog</a> ·
  <a href="BENCHMARKS.md">Benchmarks</a>
</p>

<p align="center"><a href="docs/img/hero.webp"><img alt="NitroOS running Chrome, the About dialog and Visual Studio Code" src="docs/img/hero.webp"></a></p>

> **NitroOS is based on [ReactOS](https://reactos.org) and is not affiliated with or endorsed by the ReactOS project.** ReactOS is a trademark of the ReactOS project. **Please report NitroOS problems here, not to ReactOS.**
>
> NitroOS is alpha software. Use it in a virtual machine, not on a disk with data you care about. We have tested it only under QEMU/KVM.

This repository (`nitroos-org/releases`, formerly "reactos-nitro") publishes the NitroOS releases: the installation ISOs, the source code of each release, the changelog and the benchmarks. Development happens in [nitroos-org/reactos](https://github.com/nitroos-org/reactos).

**Latest release: `2026.11-2` (17 November 2026)**, based on ReactOS commit [`8fe3cee0560`](https://github.com/reactos/reactos/commit/8fe3cee05606aecd9442674e40038649f048d10d), with 453 changes since 2026.11-1. Unless a section says otherwise, everything on this page refers to that release.

## Why NitroOS

We want an open-source operating system that runs Windows programs well enough to use every day. Building that by hand takes decades: ReactOS has been working on it since 1998, and NitroOS is built on that work. With AI coding agents doing the engineering under a human maintainer, we believe it can take months instead. Our first release came out on 20 October 2026. Three weeks and six releases later, Chrome, Firefox, Edge, VS Code, PowerShell 7 and .NET Framework 4.8 programs run on it.

- **Made for everyday use.** We work first on the programs people actually use, the system is ready to use straight after installation, and we release every week or two.
- **Proven, not promised.** We publish our benchmarks, including the tests where we are slower, and we show what works with screenshots.
- **Built with care.** NitroOS behaves the way Windows does and uses only clean, public sources. Every change is reviewed by a second AI model and tested before it is accepted.

**Our end goal is to merge all relevant work back into ReactOS.** ReactOS does not accept AI-generated code today, and we respect that. If its policy changes, we want this work upstream. Until then, NitroOS is a separate open-source (GPL) project; our [AI policy](https://github.com/nitroos-org/.github/blob/main/AI-POLICY.md) explains how we work.

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

### Speed

How NitroOS compares with the ReactOS code it is built on: the same test program on the same virtual machine, using the 32-bit version of both. Each row shows how much faster or slower NitroOS is.

| Test | ReactOS | NitroOS | NitroOS is |
|---|--:|--:|---|
| **Memory** | | | |
| Getting new memory (page faults per second) | 8,500 | 1,142,000 | 🟢 **134× faster** |
| Copy-on-write memory (page faults per second) | 6,700 | 447,000 | 🟢 **66× faster** |
| Memory-mapped files (page faults per second) | 42,900 | 1,115,000 | 🟢 **26× faster** |
| Heap: 1 million allocations and frees | 1.60 s | 0.15 s | 🟢 **10.6× faster** |
| VirtualAlloc: 32,768 commits and decommits | 69 ms | 13 ms | 🟢 **5.2× faster** |
| Heap: 4 threads allocating at once ‡ | 0.45 s | 12.2 s | 🟠 **27× slower** |
| **Programs** | | | |
| Boot to the logon screen | 10 s | 6 s | 🟢 **1.7× faster** |
| Start `cmd.exe` 50 times † | 0.60 s | 0.29 s | 🟢 **2.1× faster** |
| Start 200 processes and wait for each † | 1.69 s | 0.58 s | 🟢 **2.9× faster** |
| 7-Zip built-in benchmark (MIPS, higher is better) † | 3,444 | 22,640 | 🟢 **6.6× faster** |
| Two threads signalling each other ‡ | 51 ms | 583 ms | 🟠 **11× slower** |
| **Files** | | | |
| Read 10,000 small files (FAT) | 95 ms | 183 ms | 🟠 **1.9× slower** |

† These tests use several CPUs. The 32-bit ReactOS build uses only one CPU by default, while NitroOS uses all four, so part of the gain comes from the extra CPUs.

‡ Measured on a development build from before release 2026.11-1. Releases 2026.11-1 and 2026.11-2 make the 4-thread heap test about 30 times faster and small-file reads about 2 times faster in our own tests; the comparison with ReactOS has not been re-run yet.

How we measured: on 8 November 2026, on one host (AMD EPYC 9254, QEMU/KVM), with 4 virtual CPUs and 4 GB of memory, taking the median of 5 runs, with both systems on FAT. ReactOS: upstream master `4d1f0148bed`. NitroOS: development build `a690f0ba892`. Per-run spread and the full method: [`BENCHMARKS.md`](BENCHMARKS.md).

## Screenshots

NitroOS (64-bit, with NitroOS branding) on freshly installed virtual machines with 4 CPUs, 4 GB of memory and a 1024x768 screen. Shots marked * are from a development build of 9 November, after release 2026.11-1; shots marked † are from release 2026.11-2 with the NitroOS artwork that the next release ships by default, on our isolated IPv6 test network (its addresses are from the documentation range `2001:db8::/32`); the others are from release 2026.11-1. These are unedited screen captures. Click a picture to see it full size.

### The system

| | |
|---|---|
| <a href="docs/img/nitro-desktop.webp"><img src="docs/img/nitro-desktop.webp" alt="The desktop"></a><br>The desktop | <a href="docs/img/nitro-winver.webp"><img src="docs/img/nitro-winver.webp" alt="About NitroOS: Windows version 10.0, build 19045, with 3.99 GB of 4 GB of memory usable *"></a><br>About NitroOS: Windows version 10.0, build 19045, with 3.99 GB of 4 GB of memory usable * |
| <a href="docs/img/nitro-features.webp"><img src="docs/img/nitro-features.webp" alt="The Features dialog, for installing optional components"></a><br>The Features dialog, for installing optional components | <a href="docs/img/nitro-features-installing.webp"><img src="docs/img/nitro-features-installing.webp" alt="Features installing the selected components"></a><br>Features installing the selected components |
| <a href="docs/img/nitro-diskmgmt.webp"><img src="docs/img/nitro-diskmgmt.webp" alt="Disk Management: NTFS, FAT32, a logical drive, free space and unallocated space on one disk *"></a><br>Disk Management: NTFS, FAT32, a logical drive, free space and unallocated space on one disk * | <a href="docs/img/nitro-diskmgmt-wizard.webp"><img src="docs/img/nitro-diskmgmt-wizard.webp" alt="The New Simple Volume wizard creating an NTFS logical drive *"></a><br>The New Simple Volume wizard creating an NTFS logical drive * |

### Apps

| | |
|---|---|
| <a href="docs/img/nitro-browser.webp"><img src="docs/img/nitro-browser.webp" alt="Chrome showing nitroos.org *"></a><br>Chrome showing nitroos.org * | <a href="docs/img/nitro-firefox-64-32.webp"><img src="docs/img/nitro-firefox-64-32.webp" alt="64-bit and 32-bit Firefox running together; Task Manager marks the 32-bit processes with *32"></a><br>64-bit and 32-bit Firefox running together; Task Manager marks the 32-bit processes with `*32` |
| <a href="docs/img/nitro-vscode.webp"><img src="docs/img/nitro-vscode.webp" alt="Visual Studio Code with a file open *"></a><br>Visual Studio Code with a file open * | <a href="docs/img/nitro-powershell7.webp"><img src="docs/img/nitro-powershell7.webp" alt="PowerShell 7.6"></a><br>PowerShell 7.6 |
| <a href="docs/img/nitro-dotnet.webp"><img src="docs/img/nitro-dotnet.webp" alt="A .NET Framework 4.8 Windows Forms program, built inside the virtual machine"></a><br>A .NET Framework 4.8 Windows Forms program, built inside the virtual machine |   |

### Networking and security

| | |
|---|---|
| <a href="docs/img/nitro-ipv6-properties.webp"><img src="docs/img/nitro-ipv6-properties.webp" alt="IPv6 is built in: TCP/IPv6 sits next to TCP/IPv4 in the connection properties, set to automatic by default †"></a><br>IPv6 is built in: TCP/IPv6 sits next to TCP/IPv4 in the connection properties, set to automatic by default † | <a href="docs/img/nitro-ipv6-ipconfig.webp"><img src="docs/img/nitro-ipv6-ipconfig.webp" alt="ipconfig /all: an address from the router, a temporary address for privacy, and the DNS server announced by the network, all picked up automatically †"></a><br>`ipconfig /all`: an address from the router, a temporary address for privacy, and the DNS server announced by the network, all picked up automatically † |
| <a href="docs/img/nitro-ipv6-only-page.webp"><img src="docs/img/nitro-ipv6-only-page.webp" alt="ping -6 and PowerShell 7 fetching a web page from a name that has only an IPv6 address †"></a><br>`ping -6` and PowerShell 7 fetching a web page from a name that has only an IPv6 address † | <a href="docs/img/nitro-ipv6-tracert.webp"><img src="docs/img/nitro-ipv6-tracert.webp" alt="tracert -6 following an IPv6 route across two hops †"></a><br>`tracert -6` following an IPv6 route across two hops † |
| <a href="docs/img/nitro-firewall.webp"><img src="docs/img/nitro-firewall.webp" alt="NitroOS Firewall, switched on after a fresh install"></a><br>NitroOS Firewall, switched on after a fresh install |   |

### Setup

| | |
|---|---|
| <a href="docs/img/nitro-format.webp"><img src="docs/img/nitro-format.webp" alt="Setup, with NTFS selected"></a><br>Setup, with NTFS selected | <a href="docs/img/nitro-install.webp"><img src="docs/img/nitro-install.webp" alt="Setup summary: multi-processor kernel, installing to \Windows"></a><br>Setup summary: multi-processor kernel, installing to `\Windows` |

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

## Next release: 2026.12-1

What we are working on now. These are goals, not promises: the release notes will say what made it in.

| Area | Status | Goal |
|---|---|---|
| Faster | in progress | Our goal for 2026.12-1 is to match or beat Windows 10 across our benchmarks. |
| File sharing | planned | Windows file sharing (SMB), as a client and as a server. |
| Office | in progress | Word installs and reaches its Start screen; next is getting it past its licence check. |
| IPv6 | in progress | The remaining differences from Windows listed under known limitations. |
| Security | daily | Security fixes across the kernel, the graphics subsystem and networking, every day. |

## Download

Release `2026.11-2` comes in two versions: 64-bit (which also runs 32-bit programs) and 32-bit. The ISO files are hosted on repo.nitroos.org; check them against `SHA256SUMS`.

| File | Size | SHA-256 |
|---|---|---|
| [reactos-nitro-2026.11-2-amd64-wow64-Release.iso](https://repo.nitroos.org/releases/nitro-2026.11-2/reactos-nitro-2026.11-2-amd64-wow64-Release.iso) | 807,403,520 bytes | `183881017e4f2f9e01409f2b0c0fba1b4ba8900012d265a35beed1edcf228ff8` |
| [reactos-nitro-2026.11-2-i386-Release.iso](https://repo.nitroos.org/releases/nitro-2026.11-2/reactos-nitro-2026.11-2-i386-Release.iso) | 480,247,808 bytes | `103fdda80eedc6837508ddf3365c14097c0bf60e1bd9a8df3704ac04a39e63cf` |
| [SHA256SUMS](https://repo.nitroos.org/releases/nitro-2026.11-2/SHA256SUMS) | | |

The source code and a single patch against the ReactOS base are attached to the [GitHub release](https://github.com/nitroos-org/releases/releases/tag/nitro-2026.11-2) (`reactos-nitro-2026.11-2-src.tar.gz`, `nitro-2026.11-2-over-8fe3cee0560.cumulative.patch`). Older releases: [`repo.nitroos.org/releases/`](https://repo.nitroos.org/releases/).

Choose the 64-bit ISO unless you need a 32-bit system; all screenshots on this page are from the 64-bit version.

## Quick start

1. Create a virtual machine. We tested with QEMU/KVM: machine `pc` (i440fx), SeaBIOS, 2 to 4 virtual CPUs, 2 to 4 GB of memory, an IDE disk of 8 GB or more, an `e1000` network card and standard VGA. Other hypervisors and real hardware are untested.

   ```sh
   qemu-img create -f raw nitroos.img 20G
   qemu-system-x86_64 -machine pc,accel=kvm -cpu host -smp 4 -m 4096 \
     -drive file=nitroos.img,format=raw,if=ide \
     -drive file=reactos-nitro-2026.11-2-amd64-wow64-Release.iso,media=cdrom,if=ide \
     -boot order=d -netdev user,id=n0 -device e1000,netdev=n0 \
     -device VGA -usb -device usb-tablet
   ```

2. Boot from the ISO, click **Install** and accept the defaults (NTFS is already selected). Setup restarts once to finish, then starts the desktop. Some installer text may still say "ReactOS" (see the known limitations).
3. On first boot, cancel the "Other ACPI device" wizard. A *Choose Your Web Browser* window offers to install Firefox, which it downloads through Features.
4. To change the screen resolution, right-click the desktop and choose **Properties**, then **Settings**. In our VM the list offered 800x600, 1024x600, 1024x768, 1152x864 and more.
5. **Features** (Run: `optfeatures`) installs PowerShell 7, Scoop, .NET Framework 4.8, Wine Gecko, the AAC decoder and more. With Scoop you can then install other software, for example `scoop install git`, `scoop bucket add extras` and `scoop install vscode googlechrome`.

## Known limitations

From the 2026.11-2 release notes and our own testing:

- NitroOS is alpha software and has been tested only in QEMU/KVM virtual machines. Real hardware is untested; in particular, the interrupt handling for real multi-processor machines (the APIC HAL) is not covered.
- The ReactOS name still appears in some places; we have not checked every dialog and installer page.
- Word installs and reaches its Start screen, but does not run yet: it stops at its licence check.
- Oracle Database XE 21c: EM Express (https://localhost:5500/em) does not load yet; the database itself (localhost:1521) works.
- The VS Code terminal draws garbled text with its default GPU renderer in most launches. Workaround: set `terminal.integrated.gpuAcceleration` to `off`.
- In a long stress test, a Firefox helper process stopped once because of an error in the text-drawing component (`dwrite.dll`); Firefox itself carried on. A fix is in progress.
- Firewall rules that are limited to a service (for example `netsh ... service=any`) do not let traffic in yet, where Windows would. They never let in more than Windows would.
- Some IPv6 details still differ from Windows (for example IPv6 change notifications and the IPv6 neighbour table); the changelog has the list. IPv4 multicast works, but NitroOS does not announce its IPv4 groups to the network, so a switch that filters multicast may not deliver them.
- The NTFS driver keeps recently used file information in memory, up to about 2.6 % of RAM. This is by design, and the memory is given back when it runs low.
- User Interface Privilege Isolation (UIPI) is missing, so windows at different integrity levels can send each other messages. Do not let untrusted people log on to a NitroOS machine.
- Installing to a FAT system volume: the system can fail to start after the first restart ('SYSTEM hive could not be used'), because a grown registry hive file's size is not written back. Use NTFS, the default.
- Running 32-bit programs is supported on the 64-bit version only.
- Everything in ReactOS's own list of known issues also applies.

## Build from source

Every release is tagged in the development repository: [`nitro-2026.11-2`](https://github.com/nitroos-org/reactos/releases/tag/nitro-2026.11-2) in [nitroos-org/reactos](https://github.com/nitroos-org/reactos). The source tarball and the cumulative patch are attached to the release; applying the patch to upstream `8fe3cee0560` with `git apply --binary` gives exactly the tagged tree. Build it as you would build ReactOS, with the ReactOS Build Environment (RosBE). For the 64-bit version that runs 32-bit programs, add `-DBUILD_WOW64_ENABLED=ON -DSYSWOW64=ON`; a 64-bit Release build without `SYSWOW64` hangs during unattended installation.

## Relationship to ReactOS

NitroOS is built on [ReactOS](https://reactos.org): the kernel, the Win32 subsystem and most of the user mode come from the ReactOS project and its contributors, and we merge the latest ReactOS code regularly (this release did not include a new merge; it is based on `e46f4bb1a1e` from 25 October 2026). [UPSTREAM.md](UPSTREAM.md) explains what we change and why NitroOS has its own NTFS driver. NitroOS is not the ReactOS project and is not supported by it; please report NitroOS bugs in this repository. "ReactOS" is a trademark of the ReactOS project. "Windows" and the other product names in screenshots belong to their owners and are used only to show what NitroOS is compatible with.

## Licence

NitroOS uses the same licences as ReactOS, mainly GPL-2.0-or-later. Each file keeps its own licence and copyright notices. The test program `docs/demo/app.cs` is our own and is released into the public domain (CC0). The logo files in `docs/img/brand/` are NitroOS artwork.

## Credits

NitroOS stands on the work of many open-source projects. Thank you.

- **[ReactOS](https://reactos.org)**: the ReactOS project and its contributors. NitroOS is built on their kernel, Win32 subsystem and most of the user mode, and we merge their latest changes regularly.
- **[Wine](https://www.winehq.org)**: whose libraries ReactOS and NitroOS reuse (Wine 11.21).
- **[Linux NTFS driver](https://www.kernel.org)**: its authors; it is the core of NitroOS's NTFS support.
- **[WinBtrfs](https://github.com/maharmstone/btrfs)**: Mark Harmstone's Btrfs driver and tools.
- **[Unicode CLDR and ICU](https://cldr.unicode.org)**: the Unicode Consortium's locale data, from which our locale tables are generated, and ICU 64.2.
- **[Mesa, LLVM and the Vulkan loader](https://www.mesa3d.org)**: software OpenGL and Vulkan in the optional graphics pack.
- **[Little CMS](https://www.littlecms.com)**: Marti Maria's colour management library, in the colour system (as Wine bundles it).
- **[IANA time zone database](https://www.iana.org/time-zones)**: the time zone rules, together with the Windows zone names from Unicode CLDR.
- **[FFmpeg](https://ffmpeg.org)**: the optional AAC audio decoders.
- **[WinFsp](https://winfsp.dev)**: Bill Zissimopoulos's framework for file systems that run as ordinary programs (an optional feature).
- **[dosfstools](https://github.com/dosfstools/dosfstools)**: the FAT disk checker.
- **[PowerShell and Scoop](https://github.com/PowerShell/PowerShell)**: PowerShell 7, preinstalled from Microsoft's official package, and the Scoop package manager (an optional feature).
- **[mingw-w64](https://www.mingw-w64.org)**: public headers, one of our clean sources for interface definitions.
- **[Fonts](https://fonts.google.com/noto)**: Selawik, Carlito, Caladea, Noto Sans CJK, Noto Sans Symbols 2 and Noto Emoji, under the SIL Open Font License.

NitroOS uses the same licences as ReactOS, mainly GPL-2.0-or-later. Every file keeps its own licence and copyright notices, and the third-party licence texts are on the installation CD. The full list, with versions and licences, is in [`media/doc/3rd Party Files.txt`](https://github.com/nitroos-org/reactos/blob/main/media/doc/3rd%20Party%20Files.txt).
