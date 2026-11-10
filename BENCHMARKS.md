# Benchmarks

Two comparisons. The first, NitroOS against the ReactOS code it is built on, is the one shown on [nitroos.org](https://nitroos.org/#numbers). The second compares NitroOS with LiberNT, another ReactOS-based operating system, on the same virtual hardware. We measure against both to know where NitroOS stands and what to work on next.

All tests are our own small programs (plus 7-Zip's built-in benchmark), run in virtual machines on one host. They are not a substitute for real-hardware testing.

## 1. NitroOS and the ReactOS code it builds on

Measured 2026-11-08 on one host (AMD EPYC 9254, QEMU/KVM), 4 virtual CPUs, 4 GB of memory, median of 5 runs, 32-bit builds of both, both systems on FAT. ReactOS: upstream master `4d1f0148bed`. NitroOS: development build `a690f0ba892`. The table and notes are on [nitroos.org](https://nitroos.org/#numbers) and in the [README](README.md#speed).

## 2. NitroOS and LiberNT on the same virtual hardware

Measured 2026-11-09 on one host (AMD EPYC 9254, QEMU 8.2.2 with KVM). Every system ran in a fresh virtual machine with the same machine model (q35), an AHCI disk, 4 virtual CPUs (`-cpu host`), 4 GB of memory and user-mode networking.

- **NitroOS:** development build `7e36c6805c6` (64-bit, with 32-bit support, memory paging on). It booted with legacy BIOS (SeaBIOS), because booting NitroOS with UEFI is not verified yet; LiberNT booted with UEFI (OVMF).
- **LiberNT:** build `1c538150eaa` of 9 November 2026. LiberNT publishes Debug builds only, and Debug builds are usually slower than release builds, so read its column with that in mind.

Each figure is the median over 4 rounds of 5 runs, with the systems interleaved and a fresh virtual machine for each round. In the last column, a value above 1 means NitroOS is better.

| Test | Unit | NitroOS | LiberNT | NitroOS vs LiberNT |
|---|---|--:|--:|--:|
| VirtualAlloc commit/decommit (32768 pages) | ms | 15.06 | 16.67 | 1.11 |
| Demand-zero page faults (steady) | thousand pages/s | 1,195 | 1,211 | 0.99 |
| File-mapped page faults | thousand pages/s | 933 | 97.58 | 9.56 |
| Copy-on-write faults | thousand pages/s | 378 | 82.74 | 4.57 |
| HeapAlloc/free (1M) | ms | 113 | 141 | 1.25 |
| Heap, 4 threads | ms | 219 | 487 | 2.22 |
| Event create/close (100k) | ms | 39.59 | 52.15 | 1.32 |
| Mutex+semaphore (50k) | ms | 42.05 | 52.54 | 1.25 |
| File open/close (30k) | ms | 350 | 343 | 0.98 |
| WaitForSingleObject (1M) | ms | 115 | 203 | 1.77 |
| Registry open/close (100k) | ms | 314 | 553 | 1.76 |
| GetFileAttributes (200k) | ms | 1,720 | 4,553 | 2.65 |
| Thread create+join (20k) | ms | 968 | 2,367 | 2.45 |
| Event ping-pong (50k) | ms | 249 | 1,306 | 5.25 |
| CreateProcess+wait (200) | ms | 363 | 458 | 1.26 |
| cmd.exe /c exit x50 | ms | 117 | 330 | 2.81 |
| Create 10k small files | ms | 3,681 | 2,700 | 0.73 |
| Read 10k small files | ms | 347 | 344 | 0.99 |
| Delete 10k small files | ms | 726 | 3,716 | 5.12 |
| Enumerate a 10k-file directory x20 | ms | 144 | 169 | 1.17 |
| Sequential read 1 GB (cached) | MB/s | 1,612 | 383 | 4.21 |
| Sequential write 1 GB (cached) | MB/s | 699 | 1,121 | 0.62 |
| Sequential read 1 GB (no buffering) | MB/s | 1,392 | 1,394 | 1.00 |
| Sequential write 1 GB (no buffering) | MB/s | 426 | 1,127 | 0.38 |
| 7-Zip benchmark total (x64) | MIPS (higher is better) | 29,790 | 29,700 | 1.00 |
| 7-Zip benchmark total (x86) | MIPS (higher is better) | 22,060 | 22,130 | 1.00 |

**Summary.** Across the 32 measured values (geometric mean of the ratios), NitroOS is 1.69 times better than LiberNT (better on 23, level on 6, behind on 3: both sequential-write tests and small-file creation).

**With 1 GB of memory** and a 1.4 GB working set, both systems complete. The second pass over the working set takes about 37 s on LiberNT and 56 to 72 s on NitroOS. At 1 GB, NitroOS is also behind LiberNT on NTFS directory listing and small-file reads, because its file-system caches shrink with the amount of memory.

**What we are working on** for the next releases: sequential writes, small-file creation, file-system caching on small machines, paging speed, and the multi-threaded heap.

## History: 2026.10 measurements

The numbers below are from a development build of October 2026 and are kept for reference only.

> **Not re-measured for 2026.10-5.** These numbers come from a development build that predates `nitro-2026.10-1` (see below) and have not been re-measured for this release; they do not reflect the Windows 10 identity default, the memory commit cap or other changes since.
>
> **DRAFT / preliminary.** These numbers were measured on a development build (batch 87, `23dd8f7e336`), **not** the tagged `2026.10-1` release, and will be re-run on the release build before launch. Two outliers (marked †) are large enough that they need an independent re-check. Read this as a direction, not final figures.

Measured 2026-10-19 on one AMD EPYC 9254 host under QEMU/KVM, 4 vCPU / 2 GB, both sides built Release from the same sources.

- **A — ReactOS master:** upstream [`ac1ab9e3043`](https://github.com/reactos/reactos/commit/ac1ab9e3043) (the reactos-nitro base).
- **B — reactos-nitro:** fork `main` [`23dd8f7e336`](https://github.com/nitroos-org/reactos/commit/23dd8f7e336).

### The two things that don't compare

- **amd64: upstream could not be measured.** The upstream amd64 build does not get through setup — it bugchecks with `STOP 0x71 SESSION5_INITIALIZATION_FAILED`, 2 of 2 attempts. reactos-nitro installs and runs on amd64. So there is no amd64 A-vs-B row; the amd64 column below is nitro only.
- **SMP: upstream runs on one CPU.** Upstream's setup installs the uniprocessor kernel, and its multiprocessor kernel, swapped in by hand, still sees one CPU and then stops. So the real comparison below is upstream uniprocessor vs nitro multiprocessor — which is the point.

**Stability:** reactos-nitro finished all 26 counted runs across i386/amd64, FAT/NTFS and 1/4 vCPU with no crash or hang.

### i386, quiet host (the real A-vs-B comparison)

Medians of 5 runs. "B/A" above 1 means nitro is faster; for latency, lower is better. Cases where nitro is **slower** are marked.

| Area | Test | ReactOS master | reactos-nitro | B/A |
|---|---|---|---|---|
| SMP | CPU throughput, 4 threads (Mops/s) | 687 (1 CPU) | 2744 | **4.0×** |
| SMP | CPU throughput, 2 threads | 687 (1 CPU) | 1376 | 2.0× |
| Memory | memcpy (MB/s) | 27,688 | 28,112 | 1.0× (same) |
| Memory | memset (MB/s) | 51,895 | 52,493 | 1.0× (same) |
| Memory | demand-zero page faults (faults/s) | 1,961,140 | 1,933,550 | 1.0× (same) |
| Memory | VirtualAlloc/Free 64 KiB (ops/s) | 227,125 | 1,640,700 | 7.2× |
| Memory | VirtualAlloc + touch + Free (ops/s) | 533 | 56,340 | **106× †** |
| Disk (FAT) | sequential write + flush (MB/s) | 22.3 | 56.4 | 2.5× |
| Disk (FAT) | buffered read (MB/s) | 26.3 | 291 | **11× †** |
| Disk (FAT) | small-file create (files/s) | 423 | 3,299 | 7.8× |
| Disk (FAT) | small-file delete (files/s) | 36,958 | 35,318 | 1.0× (same) |
| Disk (FAT) | uncached read (MB/s) | 708 | 665 | 0.94× (**slower**) |
| Network | HTTP download (MB/s) | 23.5 | 23.4 | 1.0× (same) |
| Network | requests/s, 1 connection | 473 | 410 | 0.87× (**slower**) |
| Network | requests/s, 4 parallel | 473 | 701 | 1.5× |
| SMP | cross-CPU wake latency, unpinned (µs) | n/a (1 CPU) | 9.6 | slower than A's single-CPU 5.0 |

† The 106× and 11× figures reflect a slow path in the upstream build and are pending an independent re-check before they're treated as final.

### NTFS

_Note: upstream ReactOS has experimental NTFS write (off by default, partial, unjournaled). The upstream-vs-nitro NTFS comparison was not run and is pending re-measurement. The table below is reactos-nitro FAT vs NTFS only._

| Test (reactos-nitro) | FAT | NTFS |
|---|---|---|
| buffered read (MB/s) | 267 | 2,477 |
| sequential write (MB/s) | 55.1 | 53.1 |
| small-file create (files/s) | 3,008 | 2,394 |
| small-file delete (files/s) | 33,300 | 9,657 |

### Honest caveats

- Preliminary pre-2026.10-1 dev build, not any release tag; not re-measured for 2026.10-5; re-run pending.
- No amd64 A-vs-B (upstream amd64 doesn't install).
- Upstream NTFS write (experimental, off by default) was not benchmarked against nitro's ntfsng; that comparison is pending.
- The SMP comparison is upstream-uniprocessor vs nitro-multiprocessor.
- Under heavy host load, nitro degraded more than upstream on the network tests (down to 0.54× on single-connection requests/s). At 1 vCPU, nitro was slower on download, requests/s and uncached read.
- Disk tests measure the guest file system and cache manager over the host page cache, not raw device speed.
