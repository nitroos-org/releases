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
  <a href="#known-limitations">Known limitations</a> ·
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

What NitroOS adds on top of ReactOS. Each point links to a screenshot or a measured number.

| Enhancement | Evidence |
|---|---|
| **Current apps run.** Chrome 157, Firefox 159, Visual Studio Code 1.145 and PowerShell 7.6 start and work (versions seen on 2026-11-08). | [browser](#screenshots), [VS Code](#screenshots), [PowerShell 7](#screenshots) |
| **32-bit and 64-bit programs side by side.** 64-bit Firefox and 32-bit Firefox run at the same time; Task Manager marks the 32-bit processes with `*32`. | [Task Manager](#screenshots) |
| **.NET Framework 4.8 programs run.** A WinForms program built inside the VM with the .NET 4.8 compiler starts and its controls work (CLR 4.0.30319.42000). | [.NET](#screenshots) |
| **A Windows 10-compatible version number.** The system reports NT 10.0, build 19045, so installers and apps that check the version proceed. | [About dialog](#screenshots) |
| **The whole 4 GB is usable.** 3.99 GB of 4 GB shows as installed memory, and Setup picks the multiprocessor kernel on a multi-CPU machine. | [About dialog](#screenshots), [Setup summary](#screenshots) |
| **NTFS by default.** Setup offers NTFS and preselects it. | [Setup, format step](#screenshots) |
| **A firewall that is on.** NitroOS Firewall reports state "On" for the private network after a fresh install. | [Firewall](#screenshots) |
| **Features on demand.** A Features dialog turns on PowerShell 7, Scoop, .NET Framework 4.8, Firefox, the AAC audio decoder and more, and downloads them when ticked. | [Features](#screenshots) |
| **Faster memory paths** than the ReactOS code it builds on (measured on 32-bit builds of both): see [the numbers](#measured-nitroos-vs-stock-reactos). | table below |

### Measured: NitroOS vs stock ReactOS

Lane `bench4`, 2026-11-08, one host (AMD EPYC 9254, QEMU 8.2 with KVM), 4 vCPU, 4 GB, medians of 5 runs, same benchmark program (written for the test, in [`BENCHMARKS.md`](BENCHMARKS.md)) built as 32-bit. Both systems on FAT (stock ReactOS Setup offers FAT), so the file system does not distort the comparison. **Stock ReactOS i386 runs a uniprocessor kernel by default while NitroOS uses all 4 vCPUs:** rows marked † measure multi-core work and partly reflect that difference; the other rows are single-threaded. NitroOS i386 from main `a690f0ba892`; stock ReactOS from upstream master `4d1f0148bed`, built the same way. Higher factor = NitroOS better.

| Test | Stock ReactOS | NitroOS | Factor |
|---|---|---|---|
| Demand-zero page faults (kpages/s) | 8.5 | 1142 | **134x** |
| Copy-on-write page faults (kpages/s) | 6.7 | 447 | **66x** |
| File-mapped page faults (kpages/s) | 42.9 | 1115 | **26x** |
| `HeapAlloc`/`HeapFree`, 1M operations (ms) | 1600 | 151 | **10.6x** |
| `VirtualAlloc` commit/decommit, 32768 (ms) | 69.4 | 13.4 | **5.2x** |
| 7-Zip benchmark total (MIPS) † | 3444 | 22640 | 6.6x † |
| `cmd.exe /c exit`, 50 starts (ms) † | 596.6 | 285.9 | 2.1x † |
| Process create + wait, 200 (ms) † | 1686 | 584.3 | 2.9x † |
| Boot to logon, 3 boots (s) | 10 | 6 | 1.7x |

Where stock ReactOS is ahead, from the same run: stock ReactOS is a **single-CPU** kernel, which helps it on lock-contention tests. A 4-thread heap test takes 450 ms on stock and 12,180 ms on NitroOS i386 (stock is 27x better); event ping-pong between two threads takes 51 ms on stock and 583 ms on NitroOS (11x better); reading 10,000 small files on FAT takes 95 ms on stock and 183 ms on NitroOS (1.9x better). Details, per-run spread, the 1 GB memory-pressure set and the method are in `BENCHMARKS.md` and the full study.

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

## Licence and credits

NitroOS is distributed under the same licences as ReactOS: mainly GPL-2.0-or-later, with per-file licences kept as they are in the source; upstream copyright notices are kept intact. The test program `docs/demo/app.cs` is original and dedicated to the public domain (CC0). The logo files under `docs/img/brand/` are NitroOS artwork.

Credit goes to the [ReactOS project](https://reactos.org) and its contributors; the authors of the Linux kernel's `fs/ntfs` driver; [WinBtrfs](https://github.com/maharmstone/btrfs) by Mark Harmstone; the [Wine project](https://www.winehq.org), whose code ReactOS and this patch set reuse; and FFmpeg for the AAC decoder add-on.
