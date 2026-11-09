<p align="center">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="docs/img/brand/lockup-1280.png">
    <img alt="NitroOS" src="docs/img/brand/lockup-light-1280.png" width="520">
  </picture>
</p>

<p align="center"><b>Our aim: an open-source, Windows-compatible operating system.</b><br>
NitroOS is built on ReactOS, with major enhancements: current 64-bit and 32-bit apps side by side, .NET Framework 4.8, PowerShell 7, NTFS by default and a firewall that is on.</p>

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

<p align="center"><a href="docs/img/hero.webp"><img alt="NitroOS 2026.11-1 running Chrome, Visual Studio Code and the Features dialog" src="docs/img/hero.webp"></a></p>

> **NitroOS is an unofficial build based on [ReactOS](https://reactos.org). It is not affiliated with or endorsed by the ReactOS project.** ReactOS is a trademark of the ReactOS project. **Do not report problems from this build to the ReactOS project; report them here.**
>
> Like ReactOS itself, this is alpha-quality software. Use it in a virtual machine, not on a disk holding data you care about. It was tested under QEMU/KVM only.

This repository (`nitroos-org/releases`, formerly "reactos-nitro") publishes the NitroOS releases: the installer ISOs, the source of each release, the changelog and the benchmarks. The development fork is [nitroos-org/reactos](https://github.com/nitroos-org/reactos).

**Latest release: `2026.11-1` (2026-11-09)**, on upstream ReactOS base [`e46f4bb1a1e`](https://github.com/reactos/reactos/commit/e46f4bb1a1ea7c722f941231565144fe0c286d3e). 592 changes since 2026.10-5. Everything on this page was produced with that build.

## Major enhancements

What NitroOS adds on top of ReactOS, in release 2026.11-1.

| Enhancement | What you get |
|---|---|
| **Apps** | |
| Current apps run | Chrome 157, Firefox 159, Edge (with its own sandbox on), VS Code 1.145 and PowerShell 7.6 start and work. ([screenshots](#screenshots)) |
| 32-bit and 64-bit together | 32-bit programs run on the 64-bit system (WoW64). 64-bit and 32-bit Firefox run at the same time; Task Manager marks 32-bit processes with `*32`. ([screenshots](#screenshots)) |
| .NET Framework 4.8 and PowerShell 7 | .NET 4.8 WinForms programs run. PowerShell 7 is preinstalled, with NetAdapter, NetTCPIP, DnsClient, Storage, ScheduledTasks and LocalAccounts modules. ([screenshots](#screenshots)) |
| Windows 10 identity | Reports NT 10.0, build 19045, so installers and apps that check the version proceed. ([screenshots](#screenshots)) |
| Features on demand | A Features dialog (and `featmgr` on the command line) installs PowerShell 7, Scoop, .NET 4.8, Firefox, Gecko, the AAC decoder, the GPU pack and WinFsp, with their dependencies. ([screenshots](#screenshots)) |
| Terminals and scheduled tasks | ConPTY pseudoconsoles for the VS Code terminal, Windows Terminal and PowerShell 7. Task Scheduler 2.0: app update tasks fire. |
| **System** | |
| All CPUs, all memory | Multi-processor kernel by default on x86 and amd64; 3.99 GB of 4 GB usable. ([screenshots](#screenshots)) |
| A memory pager | On by default on amd64: unused memory is paged out, and programs can commit up to RAM plus the paging file. A runaway program gets an allocation failure instead of taking the system down. |
| Measured speed-ups | 134x faster demand-zero page faults, 66x faster copy-on-write faults, 10.6x faster heap than the ReactOS code it builds on. ([numbers](#speed)) |
| An accurate clock in VMs | Time is taken from the hypervisor's TSC frequency: drift of 0.004% or less under heavy load. |
| **Files and storage** | |
| Disk Management | A Windows 7-style Disk Management (`diskmgmt.msc`) with a graphical disk layout: Initialize Disk (MBR or GPT), New Simple Volume, Extend and Shrink Volume, Format, and Change Drive Letter and Paths, from a right-click menu on each block. A greyed-out item says why in the status bar. Before writing, it checks it is changing the disk and partition table you saw, and reads the result back. |
| NTFS by default | A read/write NTFS driver built on the Linux kernel's `fs/ntfs`, with a journal, replay after a power cut and chkdsk. Setup preselects it. ([screenshots](#screenshots)) |
| Btrfs | WinBtrfs 1.10 with our fixes: volumes made by current btrfs-progs mount, `format /FS:BTRFS` works and drive letters stick. Block group tree, free-space bitmaps and compressed inline files. |
| WinFsp file systems | WinFsp 2.x installs and mounts user-mode file systems on drive letters and UNC paths. |
| **Devices and graphics** | |
| Device Manager that does what it says | Disabling a device lasts across restarts (the boot device is protected), a failed enable or disable is reported, and installing a driver over a running device restarts it. Driver installs run in a service from a protected copy. |
| Graphics | A virtio-gpu display driver; Mesa OpenGL (llvmpipe) and Vulkan (lavapipe) in the GPU pack on amd64; first stages of Windows.UI.Composition and DirectComposition, so WebView2 and Edge render. |
| **Networking and security** | |
| A firewall that is on | Inbound and outbound filtering on after a fresh install, Windows-format rules, firewall.cpl, wf.msc and a prompt when a blocked program starts listening. ([screenshots](#screenshots)) |
| Networking for everyone | Standard users can use TCP and UDP sockets; ipconfig and net show what Windows shows; HTTP Server API, `netsh portproxy`, schannel TLS. |
| Hardening | Registry security descriptors, grouped security fixes across the kernel, win32k and networking, and sandboxed browsers. |
| **Desktop** | |
| Comfortable desktop | LCD text rendering, bundled open fonts, crash reports in a Windows-style dialog, console copy and paste with Ctrl+C/Ctrl+V and mouse selection, installs to `C:\Windows`. |

### Speed

NitroOS compared with the upstream ReactOS code it is built on: same test program, same virtual machine, 32-bit builds of both. Each result says how much faster or slower NitroOS is.

| Test | ReactOS | NitroOS | NitroOS is |
|---|--:|--:|---|
| **Memory** | | | |
| New memory pages (demand-zero page faults), pages per second | 8,500 | 1,142,000 | 🟢 **134× faster** |
| Copy-on-write page faults, pages per second | 6,700 | 447,000 | 🟢 **66× faster** |
| File-mapped page faults, pages per second | 42,900 | 1,115,000 | 🟢 **26× faster** |
| Heap: 1 million allocate + free | 1.60 s | 0.15 s | 🟢 **10.6× faster** |
| VirtualAlloc: 32,768 commit + decommit | 69 ms | 13 ms | 🟢 **5.2× faster** |
| Heap: 4 threads at once | 0.45 s | 12.2 s | 🟠 **27× slower** |
| **Programs** | | | |
| Boot to logon screen | 10 s | 6 s | 🟢 **1.7× faster** |
| Start `cmd.exe` 50 times † | 0.60 s | 0.29 s | 🟢 **2.1× faster** |
| Create and wait for 200 processes † | 1.69 s | 0.58 s | 🟢 **2.9× faster** |
| 7-Zip built-in benchmark (MIPS, higher is better) † | 3,444 | 22,640 | 🟢 **6.6× faster** |
| Two threads signalling each other (event ping-pong) | 51 ms | 583 ms | 🟠 **11× slower** |
| **Files** | | | |
| Read 10,000 small files (FAT) | 95 ms | 183 ms | 🟠 **1.9× slower** |

† Uses several CPUs: the 32-bit ReactOS build runs a single-CPU kernel by default, NitroOS uses all 4, so part of that gain comes from the extra CPUs. The slower rows are lock-contention and small-file tests; they are being worked on for 2026.11-2.

Method: lane `bench4`, 2026-11-08, one host (AMD EPYC 9254, QEMU 8.2 with KVM), 4 vCPU, 4 GB, medians of 5 runs, both systems on FAT. NitroOS i386 from main `a690f0ba892`, ReactOS from upstream master `4d1f0148bed`, built the same way. Per-run spread, the 1 GB memory-pressure set and the method: [`BENCHMARKS.md`](BENCHMARKS.md).

## Screenshots

NitroOS 2026.11-1 (amd64, fork main `049578fe905` with the NitroOS brand layer applied, per `BRAND-APPLY.md`) on a fresh VM: 4 vCPU, 4 GB, 1024x768. Captured with QEMU's screen dump, **not edited**. Click a picture for full size.

| | |
|---|---|
| <a href="docs/img/nitro-desktop.webp"><img src="docs/img/nitro-desktop.webp" alt="The desktop, 1024x768"></a><br>The desktop, 1024x768 | <a href="docs/img/nitro-winver.webp"><img src="docs/img/nitro-winver.webp" alt="About: NT 10.0, build 19045; 3.99 GB of 4 GB usable"></a><br>About: NT 10.0, build 19045; 3.99 GB of 4 GB usable |
| <a href="docs/img/nitro-browser.webp"><img src="docs/img/nitro-browser.webp" alt="Chrome 157 on a real https page"></a><br>Chrome 157 on a real https page | <a href="docs/img/nitro-firefox-64-32.webp"><img src="docs/img/nitro-firefox-64-32.webp" alt="64-bit and 32-bit Firefox side by side; Task Manager marks 32-bit processes with *32"></a><br>64-bit and 32-bit Firefox side by side; Task Manager marks 32-bit processes with `*32` |
| <a href="docs/img/nitro-vscode.webp"><img src="docs/img/nitro-vscode.webp" alt="Visual Studio Code 1.145"></a><br>Visual Studio Code 1.145 | <a href="docs/img/nitro-powershell7.webp"><img src="docs/img/nitro-powershell7.webp" alt="PowerShell 7.6"></a><br>PowerShell 7.6 |
| <a href="docs/img/nitro-dotnet.webp"><img src="docs/img/nitro-dotnet.webp" alt=".NET Framework 4.8 WinForms program, built inside the VM"></a><br>.NET Framework 4.8 WinForms program, built inside the VM | <a href="docs/img/nitro-features.webp"><img src="docs/img/nitro-features.webp" alt="Features dialog: optional components on demand"></a><br>Features dialog: optional components on demand |
| <a href="docs/img/nitro-features-installing.webp"><img src="docs/img/nitro-features-installing.webp" alt="Features installing the selected components"></a><br>Features installing the selected components | <a href="docs/img/nitro-format.webp"><img src="docs/img/nitro-format.webp" alt="Setup: NTFS preselected"></a><br>Setup: NTFS preselected |
| <a href="docs/img/nitro-install.webp"><img src="docs/img/nitro-install.webp" alt="Setup summary: multiprocessor kernel, installs to \Windows"></a><br>Setup summary: multiprocessor kernel, installs to `\Windows` | <a href="docs/img/nitro-firewall.webp"><img src="docs/img/nitro-firewall.webp" alt="NitroOS Firewall, on after a fresh install"></a><br>NitroOS Firewall, on after a fresh install |

## Technical details

The major changes and notable bug fixes in 2026.11-1, by area. The release has 592 changes in total; [`CHANGELOG.md`](CHANGELOG.md) lists every one, with a link to its commit.

### Memory manager and cache

NitroPager, on by default on amd64: working-set trimming and paging of private memory, a low-water balancer and a pager test suite (i386 keeps it off; `NitroPager=0` turns it off). Commit limit is RAM plus paging files, placeholder memory (`NtAllocateVirtualMemoryEx`).

- A section view fault builds its page tables before taking the segment lock (lock-order deadlock with the lazy writer at low memory, bugcheck 0x7A).
- `CcCopyRead`/`CcCopyWrite` keep the shared cache map referenced for the call (the lazy writer could delete it underneath: bugcheck).
- A driver image load waits for the balancer, bounded, instead of failing or taking an unusable page.

<sub>Bug fixes in this area: 36 + 8 I/O</sub>

### Heap and locks

`RtlEnterCriticalSection` spins up to the section's spin count before waiting, without barging past queued waiters; user-mode heaps get a small-block front end. The heap is about 30x faster under contention.

### Kernel, scheduler and exceptions

Foreground boost and quantum work, amd64 user hardware breakpoints and debug registers, XState contexts as on Windows, QPC in user mode, `NtSetInformationKey`, `NtAlertThreadByThreadId`/`NtWaitForAlertByThreadId`.

- `NtCreateThread` captures the CONTEXT and INITIAL_TEB once and checks them before the process handle.
- A ready foreground thread behind a busy background thread of the same priority gets a decaying boost.
- A terminating thread runs at low real-time priority, so `TerminateProcess` of an idle-class process under load completes promptly.

<sub>Bug fixes in this area: 25 + 31</sub>

### WoW64 (32-bit programs on 64-bit)

System call overhead drops from about 245 ns to about 92 ns: plain services skip FXSAVE, and APC dispatch and `Get/SetThreadContext` take the live x87/SSE state. Also: the 32-bit debug API, WOW64 TEB offset, thread context in 64-bit code, extended handle information, SCSI/ATA pass-through and a 336-entry pointer-free ioctl allow-list.

- The remaining SysWOW64 COM servers (DirectDraw, DirectSound, DirectInput, UI Automation, MSHTML, HTML Help, ADSI) are registered in the 32-bit view.
- Crash reporter in SysWOW64, `ExitWindowsEx` and console control forwarding.

<sub>Bug fixes in this area: 52</sub>

### NTFS and FAT

NTFS-NG: journal commit without the core lock, POSIX delete, MFT record reuse, chkdsk/autochk, and non-cached writes sent to the device directly, up to 1 MB per request (AHCI sequential write 205 to 358 MB/s in our test). FAT: a checker synced to dosfstools 4.2.

- NTFS-NG only re-zeroes the page the old end of file sits in (growing a file by appends re-zeroed every page: 60-77 s).
- Byte-range locks belong to their file object: unlocking through one handle no longer releases locks taken through another.
- fastfat: interlocked volume-state updates and a cleared create-in-progress flag after an exception (the next create bugchecked 0x23).

<sub>Bug fixes in this area: 26 + 11</sub>

### Display, compositor and win32k

Windows.UI.Composition stages 0 to 3 (visual tree) and a minimal DirectComposition: WebView2 controllers and Edge render, with Edge's default sandbox. DirectWrite font fallback, per-desktop win32k work, `UpdateLayeredWindow`.

- `SetParent` works on another process' window as on Windows.
- Window message behaviour closer to Windows 10, measured on a reference Windows install.
- Pattern fills honour the DC brush origin in the 8/16/24/32 bpp blitters.

<sub>Bug fixes in this area: 41</sub>

### Networking and firewall

NitroOS Firewall on by default: inbound/outbound filter, rule store in the Windows registry format, firewall.cpl, wf.msc and a block prompt. TCP receive window and loopback, AFD send window and shutdown, `WSASendMsg`, `netsh portproxy`, HTTP Server API, schannel; ipconfig and net.exe output as on Windows.

- `SO_RCVTIMEO` applies to datagram sockets (a UDP receive with a timeout waited forever).
- Firewall: per-port address-file index (inbound owner lookup about 7x cheaper), address files pin their creator process.

<sub>Bug fixes in this area: 48</sub>

### Console and terminal

ConPTY and VT input: the pseudoconsole that VS Code, Windows Terminal and PowerShell 7 use.

- Ctrl+C copies when text is selected, Ctrl+V pastes, Ctrl+A selects all; QuickEdit is on by default, as on Windows 10.
- Console programs that own windows get `WM_QUERYENDSESSION`/`WM_ENDSESSION` at shutdown instead of being killed first.

<sub>Bug fixes in this area: 24</sub>

### Applications, COM and Wine 11.21

Sync to Wine 11.21: 55 modules plus comctl32, mshtml, jscript, msxml3/4, urlmon, ieframe and msvcrt. PowerShell 7 preinstalled with clean-room NetAdapter, NetTCPIP, DnsClient, Storage, ScheduledTasks and LocalAccounts modules. Office Click-to-Run installs (Word itself still stops at its splash screen).

- msxml3 XPath parses prefixed function calls such as `ms:string-compare`.
- DirectPlay8Create returns real objects (dplayx, dpnet synced).

<sub>Bug fixes in this area: 66 + 91</sub>

### Locale data

The 146 locale tables and the calendar data are generated from Unicode CLDR 49 by an in-tree, deterministic generator (`sdk/tools/nlsgen`). Nothing is taken from Windows.

### Setup, storage and drivers

New installs go to `C:\Windows`, with a junction. The i386 live setup runs with DMA (about 1 minute to install in our test VM). NVMe storport miniport; Gecko, AAC and the GPU pack install on demand from Features.

- usbport: two CPUs flushing transfers at once no longer map one transfer twice (endless list walk).
- i386 Debug installs work again (driver-install lock is a mutex; aligned `TOKEN_USER` buffers).

<sub>Bug fixes in this area: 9 + 20</sub>

### Security and build

Registry security descriptors and eight grouped hardening sets (file-system links, loader and path handling, LSA, win32k USER, networking and others; details are held back until users have had time to update). The amd64 Release binaries are free of the SEH `__finally` code-generation defect (38 functions were affected, verified by a scan).

<sub>Bug fixes in this area: 18 + 21</sub>

## Download

Release `2026.11-1` (amd64 with 32-bit support, and i386). The ISOs are hosted on repo.nitroos.org; check them against `SHA256SUMS`.

| File | Size | SHA-256 |
|---|---|---|
| [reactos-nitro-2026.11-1-amd64-wow64-Release.iso](https://repo.nitroos.org/releases/nitro-2026.11-1/reactos-nitro-2026.11-1-amd64-wow64-Release.iso) | 800,063,488 bytes | `1f12edc4e134299f7f4341f64f7d171872b88f00bfaed03399b2103524d7f14f` |
| [reactos-nitro-2026.11-1-i386-Release.iso](https://repo.nitroos.org/releases/nitro-2026.11-1/reactos-nitro-2026.11-1-i386-Release.iso) | 472,907,776 bytes | `ddfb5a43768d44b5c45ae7c588252afc7e587db0ba9cce01b452d0defef4011e` |
| [SHA256SUMS](https://repo.nitroos.org/releases/nitro-2026.11-1/SHA256SUMS) | | |

The source tarball and one cumulative patch over the upstream base are attached to the [GitHub release](https://github.com/nitroos-org/releases/releases/tag/nitro-2026.11-1) (`reactos-nitro-2026.11-1-src.tar.gz`, `nitro-2026.11-1-over-e46f4bb1a1e.cumulative.patch`). Older releases: [`repo.nitroos.org/releases/`](https://repo.nitroos.org/releases/).

Use the **amd64** ISO unless you need a 32-bit system: every NitroOS screenshot on this page is from an amd64 build.

## Quick start

1. Create a VM. Tested configuration: QEMU/KVM, machine `pc` (i440fx), SeaBIOS, 2 to 4 vCPUs, 2 to 4 GB RAM, an IDE disk of 8 GB or more, an `e1000` network card, standard VGA. Other hypervisors and real hardware are untested.

   ```sh
   qemu-img create -f raw nitroos.img 20G
   qemu-system-x86_64 -machine pc,accel=kvm -cpu host -smp 4 -m 4096 \
     -drive file=nitroos.img,format=raw,if=ide \
     -drive file=reactos-nitro-2026.11-1-amd64-wow64-Release.iso,media=cdrom,if=ide \
     -boot order=d -netdev user,id=n0 -device e1000,netdev=n0 \
     -device VGA -usb -device usb-tablet
   ```

2. Boot the ISO, click **Install**, accept the defaults (NTFS is preselected). Setup restarts into its second stage, then to the desktop. The installer text may still say "ReactOS" in places (see limitations).
3. First boot: cancel the "Other ACPI device" wizard. A *Choose Your Web Browser* window offers to install Firefox (it downloads it through the Features manager).
4. For more resolution, use right-click on the desktop, Properties, Settings. The list offered 800x600, 1024x600, 1024x768, 1152x864 and more in our VM.
5. **Features** (Run: `optfeatures`) installs PowerShell 7, Scoop (then `scoop install git` and `scoop bucket add extras`, then e.g. `scoop install vscode googlechrome`), .NET Framework 4.8, Wine Gecko, the AAC decoder and more.

## Known limitations

From the 2026.11-1 release notes, and from what we saw while taking the screenshots above:

- **Alpha quality.** Tested under QEMU/KVM only; real-hardware paths, notably the APIC HAL, are not covered.
- **The ReactOS name still appears in places.** We did not audit every dialog and installer page.
- **Memory pager:** on by default on amd64, off on i386 (`NitroPager=0` turns it off on amd64). In a 1 GB VM a process can commit about 768 MB: a 1400 MB allocation fails cleanly with `ERROR_COMMITMENT_LIMIT`, and `7z b` stops at dictionary 25. A 1400 MB hog plus a `git clone` at the same moment still fails in 4 of 9 runs on a 1 GB VM.
- **File system speed:** NTFS directory enumeration is slow (20 passes over 10,000 files: 2.19 s on amd64; the same kernel on FAT: 44 ms), and so is reading many small files (2.38 s per 10,000 on NTFS). Cached sequential write: 45 MB/s on amd64.
- **Multi-threaded heap contention is slow:** 4 threads x 250,000 alloc/free take 6.8 s on amd64 and 12.2 s on i386.
- **Word** (Office Click-to-Run) installs but stops at its splash screen.
- **VS Code's integrated terminal** renders corrupted with its default WebGL renderer in about 16 of 18 launches. Workaround: set `terminal.integrated.gpuAcceleration` to `off`, or start VS Code with `--use-angle=swiftshader --enable-unsafe-swiftshader`.
- **UIPI is missing:** no window-message isolation between integrity levels. Do not expose a NitroOS machine to untrusted local users.
- **FAT:** a bugcheck 0x23 in about 2% of runs under heavy load.
- **USB hot-plug:** a rare guest hang (fixed on main after this release).
- **i386 Setup is slow in QEMU:** about 26 minutes in our test VM; the amd64 ISO installs in about 70 seconds.
- WoW64 (running 32-bit programs) is amd64 only.
- Everything in upstream ReactOS's own list of known issues also applies.

## Build from source

Every release is tagged in the development fork: [`nitro-2026.11-1`](https://github.com/nitroos-org/reactos/releases/tag/nitro-2026.11-1) in [nitroos-org/reactos](https://github.com/nitroos-org/reactos). The source tarball and the cumulative patch (apply to upstream `e46f4bb1a1e` with `git apply --binary`; the result is byte-identical to the tag) are attached to the release. Build as you would upstream ReactOS, with the ReactOS Build Environment (RosBE). The amd64 build with 32-bit support needs `-DBUILD_WOW64_ENABLED=ON -DSYSWOW64=ON`; an amd64 Release build without `SYSWOW64` hangs the unattended install.

## Relationship to ReactOS

NitroOS is built on [ReactOS](https://reactos.org): the kernel, the Win32 subsystem and most of the user mode come from the ReactOS project and its contributors, and we merge upstream ReactOS master regularly (this release has no upstream sync; its base is `e46f4bb1a1e` from 2026-10-25). See [UPSTREAM.md](UPSTREAM.md) for what we change and why our NTFS is a separate driver. NitroOS is **not** the ReactOS project, is not supported by it, and its bugs belong in this repository's issues. The name "ReactOS" is a trademark of the ReactOS project; "Windows" and the names of other products in screenshots belong to their owners and are used only to say what a program is compatible with.

## Licence

NitroOS is distributed under the same licences as ReactOS: mainly GPL-2.0-or-later, with per-file licences kept as they are in the source; upstream copyright notices are kept intact. The test program `docs/demo/app.cs` is original and dedicated to the public domain (CC0). The logo files under `docs/img/brand/` are NitroOS artwork.

## Credits

NitroOS stands on the work of many open-source projects. Thank you to:

- **[ReactOS](https://reactos.org)**: the ReactOS project and its contributors. NitroOS is built on their kernel, Win32 subsystem and most of the user mode, and we merge upstream ReactOS regularly.
- **[Wine](https://www.winehq.org)**: the Wine project, whose DLLs ReactOS and NitroOS reuse (synced to Wine 11.21 in this release).
- **[Linux fs/ntfs](https://www.kernel.org)**: the authors of the Linux kernel's NTFS driver, the core of NitroOS's NTFS driver.
- **[WinBtrfs](https://github.com/maharmstone/btrfs)**: Mark Harmstone's Btrfs driver and tools.
- **[Unicode CLDR and ICU](https://cldr.unicode.org)**: the Unicode Consortium's locale data, which the locale tables are generated from, and ICU 64.2.
- **[Mesa, LLVM and the Vulkan loader](https://www.mesa3d.org)**: Mesa3D (llvmpipe, lavapipe), LLVM and the Khronos Vulkan loader: software OpenGL and Vulkan in the GPU pack.
- **[FFmpeg](https://ffmpeg.org)**: the AAC audio decoder add-ons.
- **[WinFsp](https://winfsp.dev)**: Bill Zissimopoulos's framework for user-mode file systems, available as an optional feature.
- **[dosfstools](https://github.com/dosfstools/dosfstools)**: the FAT checker.
- **[PowerShell and Scoop](https://github.com/PowerShell/PowerShell)**: PowerShell 7 (preinstalled from the official package) and Scoop (an optional feature).
- **[mingw-w64](https://www.mingw-w64.org)**: public headers, one of our clean sources for interface definitions.
- **[Fonts](https://fonts.google.com/noto)**: Selawik, Carlito, Caladea, Noto Sans CJK, Noto Sans Symbols 2 and Noto Emoji, under the SIL Open Font License.

The full list with versions and licences is in [`media/doc/3rd Party Files.txt`](https://github.com/nitroos-org/reactos/blob/main/media/doc/3rd%20Party%20Files.txt).