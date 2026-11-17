# reactos-nitro changelog

NitroOS (reactos-nitro) is an open-source, Windows-compatible operating system based on ReactOS, with our own enhancements. It is not affiliated with the ReactOS project. Newest entry first. All times UTC.

## reactos-nitro 2026.11-2

Tagged `nitro-2026.11-2` (2026-11-17) at fork main [`ae33cd4badf`](https://github.com/nitroos-org/reactos/commit/ae33cd4badf81a7e0ef2cb9c91b4c8f133d03e7d). 453 changes since 2026.11-1. The headline is IPv6, which is now on by default and a first-class network protocol: router-advertised and temporary (privacy) addresses, a DHCPv6 client, Windows' rules for choosing addresses, and raw IPv6 sockets, tested against a real home router. Writes to NTFS are faster thanks to group commit. The release also brings registry security descriptors that behave as on Windows, a cryptographic system random number generator, a minifilter-grade Filter Manager, the fix for the 64-bit data corruption issue found after 2026.11-1, and a weekly upstream sync.

| Item | Value |
|---|---|
| Release tag | `nitro-2026.11-2` → [`ae33cd4badf`](https://github.com/nitroos-org/reactos/commit/ae33cd4badf81a7e0ef2cb9c91b4c8f133d03e7d) |
| ReactOS base | upstream [`8fe3cee0560`](https://github.com/reactos/reactos/commit/8fe3cee05606aecd9442674e40038649f048d10d) "[NDK] Declare HalGetInterruptSource on ARM64" (2026-11-10, weekly sync #5; 96 upstream commits since `e46f4bb1a1e`) |
| Patch | one cumulative patch over `8fe3cee0560` (`git diff --binary`, 115 binary files; apply with `git apply --binary`); the applied tree `78cfcdd45450` is byte-identical to the tag |
| Artifacts | amd64-wow64 ISO, i386 ISO (both on repo.nitroos.org), source tarball, cumulative patch, SHA256SUMS |

### Highlights
453 changes (451 merges, 2 direct commits). The main tracks:

- **IPv6 on by default, as a first-class protocol:** stateless autoconfiguration from router advertisements (including DNS servers and the search list), temporary privacy addresses (RFC 8981, on by default as in Windows 10), a DHCPv6 client (stateless and stateful; `ipconfig /renew6` and `/release6`), duplicate address detection, MLD listener reports, destination and source address selection by RFC 6724 with the Windows prefix policy table, raw IPv6 sockets with Windows' access rule, IPv6 firewall rules, `netsh interface ipv6`, IPv6 in the PowerShell network modules, and WinHTTP and WinINet over IPv6. Tested against a real home router: autoconfiguration, a temporary address, DHCPv6, DNS and HTTPS over IPv6. `Ipv6Enable = 0`, or the Windows value `DisabledComponents`, turns IPv6 off on the network adapters as on Windows (the loopback interface keeps it).
- **IPv6 settings in the network connection properties:** an "Internet Protocol Version 6 (TCP/IPv6)" entry with its properties page, as on Windows. In this release the IPv4 and IPv6 check boxes there are read-only.
- **IPv4 multicast with IPv6 on:** groups that a program joins now reach it (before, they never reached the network driver while IPv6 was on).
- **Faster NTFS writes (group commit):** write-through writes that extend a file are committed to the journal in groups (at a flush, at close, every 32 MB or every second) instead of one journal commit per write; file data is still written before the new file size. In our matched test, sequential unbuffered writes are about 1.5 to 1.85 times faster than before the change. `NoGroupCommit=1` in the ntfsng service key restores the old behaviour.
- **Fix for the 64-bit data corruption issue found after 2026.11-1:** a driver that asks for a block of physical memory is never given a page that still belongs to a program. Paging stays on by default on 64-bit.
- **Registry security as on Windows:** keys store and inherit real security descriptors and report and change them as Windows 10 does (owner checks, auto-inheritance, protected keys); hive roots, SAM and SECURITY carry the Windows 10 descriptors. Access checks run in audit mode: they log what Windows would deny, and nothing is refused yet.
- **Security:** a cryptographic system random number generator (ChaCha20, seeded from the CPU's hardware generator and timing jitter), service accounts as on Windows (NT SERVICE SIDs, password-less service logons for trusted callers only), and 8 security fixes in the kernel, win32k, RPC and networking, described generically below.
- **NTLM without an external helper** (NTLMv2 and session security), RPC with NTLM authentication, and same-machine NTLM handled as a local call.
- **Filter Manager:** minifilter-grade (altitudes, instances, contexts, communication ports, fast I/O).
- **Memory manager:** a 128 TiB user address space on amd64 (as on Windows 8.1 and later), page-file-backed reserved sections up to 8 TB, `FlushViewOfFile` writes the view's pages, the pager reads and writes the page file in clusters, faster cached sequential reads.
- **Composition for Office:** more of Windows.UI.Composition and DirectComposition; Word now gets as far as its Start screen (it does not run yet, see Limitations).
- **Locales and time zones:** 231 locales generated from Unicode CLDR 49 (85 more than 2026.11-1) with date, number and currency formats closer to Windows, and 143 time zones with display names in a new `tzres.dll`.
- **Apps:** Oracle Database XE 21c installs with its setup.exe on the first run (EM Express does not load yet, see Limitations), Java reports Windows 10 and can use the large address space, Go 1.26 programs run, MariaDB, the Windows App SDK runtime of PowerToys loads, and `rustup` works.
- **Upstream sync #5:** ReactOS master `8fe3cee0560` (96 upstream commits, including the new USB3 stack, which NitroOS does not enable by default yet).

### What reactos-nitro adds on top of ReactOS
Everything from the previous releases, plus the following in this release.

#### Networking
IPv6 on by default (see Highlights and the IPv6 notes under Limitations). Also: IPv4 multicast with IPv6 on; a UDP "port unreachable" reported as `WSAECONNRESET` for both families, as on Windows; IPv4 answers datagrams to closed ports; `localhost` resolves to `::1` first, as on Windows; firewall Allow rules with a condition the filter cannot match are refused (narrower than Windows, never broader); a DHCP client crash with two adapters fixed; TCP, AFD and DNS fixes.

#### NTFS and storage
NTFS-NG group commit, faster directory listing and small-file reads and creates, caches that scale with memory and shrink under pressure, extended directory change notifications, 512 KB ATA commands on AHCI, and write-through on AHCI made durable (queued FUA only on drives that advertise it).

#### Kernel and memory
The 64-bit data corruption fix, the 128 TiB user address space, 8 TB reserved sections, a real `FlushViewOfFile`, page-file clustering and read-ahead, the Filter Manager, wait completion packets and reserve objects (Go 1.26), more registry calls (NtQueryMultipleValueKey, NtCompactKeys, the transacted key calls), and sessions P3 and P4 (a second session with its own win32k, off by default behind a flag).

#### Security
The cryptographic random number generator, service virtual accounts, registry security descriptors (audit mode), NTLM, and 8 security fixes described generically below.

#### Display and compositor
More of Windows.UI.Composition and DirectComposition (DesktopWindowTarget, CompositorController, nine-grid brushes, interaction trackers), redirected window surfaces with dirty-rectangle presentation (off by default), DirectWrite fallback fonts, robust GDI arcs and ellipses, Direct3D 11 WARP devices created as on Windows.

#### WoW64
32-bit processes can save, load and replace registry hives and start 32-bit children under a 32-bit debugger, and more window messages are translated (WM_HELP, owner-draw, MDI create, drag and IME messages).

#### Locales and setup
231 locales from Unicode CLDR 49, 143 time zones, event logs sized as on Windows 10, help and support links that point to NitroOS, and "based on ReactOS" wording in the watermark, the About box and setup.

#### Other
Heap and exception performance (lock-free vectored handlers, a heap front end at the Windows 10 thresholds), `QueueUserWorkItem` 3.1 times faster, Software Licensing answers that Office needs, the Authenticode signing APIs, Little CMS 2.20 in mscms, and reproducible-build options.

## Bug fixes

The 453 changes on the fork's first-parent history between `nitro-2026.11-1` and this release (451 merges and 2 direct commits), grouped by area. Each line links the commit on the fork. 8 security-relevant fixes are described generically. This section lists only what is new in 2026.11-2; everything from 2026.11-1 is included (see its section below).

<details><summary>Kernel: Ke/HAL/amd64 (12 fixes)</summary>

- An interrupt that joins a shared vector gets its dispatch routine (disconnecting the vector's first interrupt jumped to address 0) ([7349b6c07a7](https://github.com/nitroos-org/reactos/commit/7349b6c07a7))
- The amd64 KDBG backtrace reads only mapped, canonical memory and stops cleanly on a bad frame instead of faulting in the debugger ([8346bb76e77](https://github.com/nitroos-org/reactos/commit/8346bb76e77))
- KDBG resumes correctly after its own breakpoint (cont no longer crashes), and a cleared breakpoint is not planted again ([cea65a0ef0f](https://github.com/nitroos-org/reactos/commit/cea65a0ef0f))
- Amd64 restarts an interlocked S-list pop that faulted on an entry another processor popped and freed (bugcheck 0x50 under fast thread create/exit) ([fc5faf7bc78](https://github.com/nitroos-org/reactos/commit/fc5faf7bc78))
- Wait completion packets without a kernel thread each: a wait block satisfied from the signal path queues the packet (no per-process or system cap needed; behaviour as measured on Windows 10) ([1fb882613b6](https://github.com/nitroos-org/reactos/commit/1fb882613b6))
- Explicit reserves below 64 KB refused as on Windows 10 (legacy VDM reserves kept); kdb prints the saved fault address; PageFaultAddress and MmPager tests made robust ([ee3abfd5565](https://github.com/nitroos-org/reactos/commit/ee3abfd5565))
- Amd64 user exception dispatch raises a stack overflow before retrying without extended state; unwinder skips spare codes instead of asserting; UserModeException expectations for AMD ([22e064a63b8](https://github.com/nitroos-org/reactos/commit/22e064a63b8))
- Amd64 access violations report the faulting address saved on entry, not a late CR2 read (JVM safepoint/implicit null-check crashes) ([a2eae093d2f](https://github.com/nitroos-org/reactos/commit/a2eae093d2f))
- KeQueryBasePriorityThread reads the thread's own process, so a query during a detach no longer faults under the thread lock (soak hang and BAD_POOL_CALLER); amd64 bugchecks IRQL_GT_ZERO_AT_SYSTEM_SERVICE as i386 does ([f3ead6b0713](https://github.com/nitroos-org/reactos/commit/f3ead6b0713))
- Write-protected system image pages clear the bit the CPU checks on the SMP kernel; KDBG writes kernel code through the physical alias ([ed91792ca48](https://github.com/nitroos-org/reactos/commit/ed91792ca48))
- MmAllocatePagesForMdl zeroes the pages it returns unless the caller asks otherwise; physical-range retry after purging standby; #GP report checks the address before reading it ([10311c6383a](https://github.com/nitroos-org/reactos/commit/10311c6383a))
- The x86 APIC HAL reports the IRQL a vector was allocated with when a shared I/O APIC input is translated again (devices sharing a line connected at IRQL 26 and 10 with separate locks: bugcheck 0x0F in KiChainedDispatch with e1000 ... ([0319e2e6bed](https://github.com/nitroos-org/reactos/commit/0319e2e6bed))

</details>

<details><summary>Kernel: Mm/Cc (41 fixes)</summary>

- MM page waits: a fault waits for the I/O on a busy section page instead of a tick (segment-lock-only sites keep the tick wait) ([2275b96de63](https://github.com/nitroos-org/reactos/commit/2275b96de63))
- Cc views unmapped lazily are flushed from the TLB in batches, and no page, segment reference, commit or range of such a view is released before its flush (small_read 1.4-1.5x) ([2c741bf8156](https://github.com/nitroos-org/reactos/commit/2c741bf8156))
- The lazy writer locks the file for the flush when it deletes a cache map (a paging write could set a valid data length above the file size) ([af336ad3e4c](https://github.com/nitroos-org/reactos/commit/af336ad3e4c))
- The CacheAligned pool types return cache-line-aligned blocks (small blocks carved from a larger one) ([0935758dda5](https://github.com/nitroos-org/reactos/commit/0935758dda5))
- The lazy writer keeps a range pinned for writing out of its writes until it is unpinned (at most 10 s), so a file system's half-made changes never reach the disk ([0303611690c](https://github.com/nitroos-org/reactos/commit/0303611690c))
- SystemPerformanceInformation reports FreeSystemPtes; the MmMdl kmtest no longer leaks 512 system PTEs per run ([cc1c9a703a6](https://github.com/nitroos-org/reactos/commit/cc1c9a703a6))
- A contiguous allocation fails cleanly when its pages cannot be mapped (no assert on system PTE exhaustion) ([ab5dd1d138a](https://github.com/nitroos-org/reactos/commit/ab5dd1d138a))
- A cached contiguous block from nonpaged pool is cache-line aligned (i386 e1000 had no network since the cached descriptor rings) ([74bac482445](https://github.com/nitroos-org/reactos/commit/74bac482445))
- A data view cannot be reprotected beyond the protection it was mapped with ([9804d2b14f0](https://github.com/nitroos-org/reactos/commit/9804d2b14f0))
- A section page can be mapped by more than 511 address spaces (the 512th process mapping a shared page such as ntdll's bugchecked MEMORY_MANAGEMENT); 64-bit entries get a 24-bit share count, 32-bit counts saturate ([fffbcad31be](https://github.com/nitroos-org/reactos/commit/fffbcad31be))
- Amd64 sparse prototype PTE store: a leaked chunk stays counted to its owner, Debug fault-injection knobs, documented commit charge, and tests for the per-process and system share of the range ([954cb6b70f6](https://github.com/nitroos-org/reactos/commit/954cb6b70f6))
- The NT default critical section timeout (30 days) instead of 2.5 minutes (a lock held that long by a software renderer raised STATUS_POSSIBLE_DEADLOCK) ([93f3e0883a6](https://github.com/nitroos-org/reactos/commit/93f3e0883a6))
- A 64-bit process without LARGEADDRESSAWARE gets system-chosen addresses below 2 GB (relocated DLLs, allocations and views without a requested address) ([256f22cf5a4](https://github.com/nitroos-org/reactos/commit/256f22cf5a4))
- Reapply "Merge: amd64 SEC_RESERVE page-file sections up to 8 TB (sparse Prototype PTE store: charged as commit, resident store capped at 1/32 of RAM, bounded page wait) and a section view keeps the protection limits of Windows 10 ... ([0dbac997704](https://github.com/nitroos-org/reactos/commit/0dbac997704))
- Revert "Merge: amd64 SEC_RESERVE page-file sections up to 8 TB (sparse Prototype PTE store: charged as commit, resident store capped at 1/32 of RAM, bounded page wait) and a section view keeps the protection limits of Windows 10 ... ([54bd9ff7242](https://github.com/nitroos-org/reactos/commit/54bd9ff7242))
- A relocated 64-bit DYNAMICBASE image is placed high, as on Windows (the .NET JIT no longer recompiles methods without optimization: PowerShell 7 arithmetic loop 3x faster) ([069e20aaee2](https://github.com/nitroos-org/reactos/commit/069e20aaee2))
- The VDL-limited image page-in read is clamped to the length asked for (no MDL longer than the read for a truncated image section) ([c1bf3f52732](https://github.com/nitroos-org/reactos/commit/c1bf3f52732))
- Amd64 SEC_RESERVE page-file sections up to 8 TB (sparse Prototype PTE store: charged as commit, resident store capped at 1/32 of RAM, bounded page wait) and a section view keeps the protection limits of Windows 10 (NOACCESS views) ([74e7a84611f](https://github.com/nitroos-org/reactos/commit/74e7a84611f))
- The PhysicalMemory user-map bound is computed without 64-bit wrap ([7851c23239d](https://github.com/nitroos-org/reactos/commit/7851c23239d))
- User-mode map of \Device\PhysicalMemory past the highest RAM page at NT 10 (bounded by the CPU's physical address width); views starting past a section with no size are refused before any size arithmetic ([63dd3eebc0a](https://github.com/nitroos-org/reactos/commit/63dd3eebc0a))
- A section view keeps the protection it was mapped with: no protection change or commit beyond it (STATUS_SECTION_PROTECTION, as Windows 10) ([653404cbd84](https://github.com/nitroos-org/reactos/commit/653404cbd84))
- Process start-up stack 3 pages as on Windows (manifest code no longer copies its XML buffer per element); stack overflow from the third page and top-guard re-arm as on Windows 10 ([3d78a53bcb1](https://github.com/nitroos-org/reactos/commit/3d78a53bcb1))
- A guard page in the last pages of a stack raises the stack overflow again after an earlier one (JVM StackOverflowError on worker threads) ([bd09f92a4d8](https://github.com/nitroos-org/reactos/commit/bd09f92a4d8))
- A file flush harvests the dirty bits of every user mapping (non-cached reads see mapped writes, as measured on Windows 10), flush write failures are reported, writes in flight are waited for ([a8720ca4f40](https://github.com/nitroos-org/reactos/commit/a8720ca4f40))
- NitroPager follow-ups: balancer pass count (no lost wakeup), read-ahead wait flag cleared per fault in a private ETHREAD byte, scarce-slot hysteresis, copies-dropped/read-ahead-wait counters ([479fc0c9dd3](https://github.com/nitroos-org/reactos/commit/479fc0c9dd3))
- Amd64 requires CMPXCHG16B as Windows 8.1+ (16-byte SLISTs for the 128 TiB range); user page-table bounds through PML4 slot 255; 64-bit commitment counts; VirtualAllocLarge checks commit accounting ([35b8e737144](https://github.com/nitroos-org/reactos/commit/35b8e737144))
- Debug builds bugcheck 0xA / 0xD1 on a kernel page fault at raised IRQL, as Windows (Release unchanged) ([795528a4dcc](https://github.com/nitroos-org/reactos/commit/795528a4dcc))
- Amd64 user address range 128 TiB as on Windows 8.1+ (MariaDB InnoDB 8 TiB reserve, Java ZGC) ([d740c21159e](https://github.com/nitroos-org/reactos/commit/d740c21159e))
- A process created into another session joins it only while the session is live (else ACCESS_DENIED); CreateProcessAsUser cleans up every failure path; a session's last process leaves it once ([6ae175c43a3](https://github.com/nitroos-org/reactos/commit/6ae175c43a3))
- FlushViewOfFile takes dirty bits in batches with one TLB flush per batch and walks large views in 4 MB chunks, dropping the locks between chunks ([c90e3d526bf](https://github.com/nitroos-org/reactos/commit/c90e3d526bf))
- NitroPager reads and writes the page file in clusters of up to 256 KB, keeps free slots in runs, waits once for the balancer instead of reading a single page, and wakes every waiter of a balancer pass (1 GB second pass 21 s -> ... ([cafce2fe61b](https://github.com/nitroos-org/reactos/commit/cafce2fe61b))
- FlushViewOfFile writes the view's dirty pages to the file (MmFlushVirtualMemory was a stub); data written through a mapped view survives a power cut after FlushViewOfFile + FlushFileBuffers ([bfa611aea25](https://github.com/nitroos-org/reactos/commit/bfa611aea25))
- After boot a freed page goes to the head of the free lists, so never-touched memory is not handed out first ([3892aa1299a](https://github.com/nitroos-org/reactos/commit/3892aa1299a))
- Session images (one address, per-session pages, cookie and protections; session imports per importer), gated by NitroSessions (off by default) ([ccdf3f2e643](https://github.com/nitroos-org/reactos/commit/ccdf3f2e643))
- No read-ahead retry from a retired paging file (STATUS_FILE_INVALID instead of a NULL dereference), prefetch statistics only without a retry ([b6d91250222](https://github.com/nitroos-org/reactos/commit/b6d91250222))
- Contiguous and physical-range page searches never take a standby page (data corruption / kernel #GP under memory pressure with NitroPager); amd64 reports an unexplained kernel #GP on the debug port ([7350df4d988](https://github.com/nitroos-org/reactos/commit/7350df4d988))
- A failed read-ahead page no longer fails the faulting page (re-read alone); read-ahead pages signal their in-page event ([5a6daf7ed33](https://github.com/nitroos-org/reactos/commit/5a6daf7ed33))
- NitroPager reads up to 15 following page-file pages in one I/O on a page-file fault (1 GB second pass 39-86 s -> 7-16 s) ([1a9647bc53a](https://github.com/nitroos-org/reactos/commit/1a9647bc53a))
- Cached sequential read at Windows 10 parity (kernel gap-search hint, view lookup from the last hit, resident pages of a view mapped in one pass, paging reads up to 256 KB) ([31128097956](https://github.com/nitroos-org/reactos/commit/31128097956))
- With NitroPager on and a paging file present, private pageable user commitment is limited by RAM plus paging files (MmTotalCommitLimit minus headroom and the critical reserve) and waits for the paging file to grow; each VAD ... ([a572caf5b65](https://github.com/nitroos-org/reactos/commit/a572caf5b65))
- Callers that used MiRemoveAnyPage/MiRemoveZeroPage's 0 as a page now wait for the balancer (bounded) or fail cleanly; MiFillSystemPageDirectory re-tests the PDE after the wait (review rvpagerfu) ([e8337bc0613](https://github.com/nitroos-org/reactos/commit/e8337bc0613))

</details>

<details><summary>Kernel: Ob/Se/Ps/Ex/registry (46 fixes)</summary>

- Security fix (described generically): hardening in this area; details withheld until users have had time to update ([1b87351e57e](https://github.com/nitroos-org/reactos/commit/1b87351e57e))
- Registry DACL propagation: a hive root inherits from the root of its class, a parent the caller cannot read passes on nothing ([7cc15233d0e](https://github.com/nitroos-org/reactos/commit/7cc15233d0e))
- No kernel handle and pool leak on each device start: resource-map keys, interface GUID string, device security descriptor, NDIS bind registry path ([a50eb967e37](https://github.com/nitroos-org/reactos/commit/a50eb967e37))
- Hive key flags follow-ups: one-shot upgrade, ControlSet keys and the WoW64 view like Windows, upgrade never rewrites an admin-changed key ([277652ed299](https://github.com/nitroos-org/reactos/commit/277652ed299))
- Registry DACL propagation leaves keys that are not auto-inherited alone and flags them protected, as Windows does ([fb5ddc3fc1a](https://github.com/nitroos-org/reactos/commit/fb5ddc3fc1a))
- No handle to a process or thread before its creator inserted it (bugcheck 0x1E in ObInsertObject from PspCreateProcess) ([04d7f27f30e](https://github.com/nitroos-org/reactos/commit/04d7f27f30e))
- System hive keys carry the Windows DACL control flags (auto-inherited, protected), existing installs upgraded at logon ([b0192d78f76](https://github.com/nitroos-org/reactos/commit/b0192d78f76))
- Registry DACL merge drops redundant inherited ACEs and marks covered copies inherit-only, as Windows does ([8ceb8813825](https://github.com/nitroos-org/reactos/commit/8ceb8813825))
- A DACL-only query of a registry key judges inherited ACEs with the key's owner ([2cef8ad66c5](https://github.com/nitroos-org/reactos/commit/2cef8ad66c5))
- SetSecurityInfo on a registry key does DACL auto-inheritance (children whose DACL is the parent's inherited tail follow it; any other key is left alone) ([26823e845de](https://github.com/nitroos-org/reactos/commit/26823e845de))
- NtUnloadKey no longer fails with STATUS_CANNOT_DELETE for keys waiting only on their delayed release ([1d3537408c8](https://github.com/nitroos-org/reactos/commit/1d3537408c8))
- Security fix (described generically): hardening in this area; details withheld until users have had time to update ([ee2a965c434](https://github.com/nitroos-org/reactos/commit/ee2a965c434))
- Security fix (described generically): hardening in this area; details withheld until users have had time to update ([33865c274e6](https://github.com/nitroos-org/reactos/commit/33865c274e6))
- NtQueryOpenSubKeys(Ex) keep their temporary key handle in the kernel handle table ([399df85065a](https://github.com/nitroos-org/reactos/commit/399df85065a))
- Tokens and objects created with an explicit SACL report SE_SACL_AUTO_INHERITED, as on Windows ([bbeee7e45c4](https://github.com/nitroos-org/reactos/commit/bbeee7e45c4))
- Implement NtQueryMultipleValueKey, NtQueryOpenSubKeysEx and NtCompactKeys (validated no-op) ([ba4a1250162](https://github.com/nitroos-org/reactos/commit/ba4a1250162))
- KUSER_SHARED_DATA.NtBuildNumber holds the build number (19045) as on Windows 10 (it read 0) ([69e720a9017](https://github.com/nitroos-org/reactos/commit/69e720a9017))
- Registry ACL R4: keys report and change their own security descriptor (owner checks, SACL with ACCESS_SYSTEM_SECURITY, notifications, last write time) as measured on Windows 10; opens are not enforced yet ([798ee48af4e](https://github.com/nitroos-org/reactos/commit/798ee48af4e))
- 32-bit processes under WoW64 can save, load, unload and replace registry hives (the NtSaveKey/NtLoadKey family had no WoW64 thunks) ([de119b52fcc](https://github.com/nitroos-org/reactos/commit/de119b52fcc))
- Registry KCB fixes: the delayed-close size is clamped to its 12-bit field (a large DelayCloseSize no longer bugchecks), MaxValueDataLen above 64 KB is not truncated (i386 Debug assertion), and a root key's NCB failure does not ... ([d8b054455ea](https://github.com/nitroos-org/reactos/commit/d8b054455ea))
- A registry name control block reference count no longer wraps after 65535 open keys of one name (the block was freed while still in use: bugcheck 0x7E / pool corruption) ([13b4c7d7e9b](https://github.com/nitroos-org/reactos/commit/13b4c7d7e9b))
- Existing event logs take the MaxSize of the registry (grown in place or by moving the wrapped records, never truncated); the old 512 KB/7-day defaults migrate to the Windows 10 values ([745cb56d97c](https://github.com/nitroos-org/reactos/commit/745cb56d97c))
- Registry ACL R3 follow-ups (storage switches only from admin-protected keys, creator descriptor checked as on Windows, bounded security-cell cache for linear hive loads), gated with the setup-time NULL-SACL templates ([58ffde8e4d4](https://github.com/nitroos-org/reactos/commit/58ffde8e4d4))
- GCC __writeeflags is a compiler memory barrier (memory clobber), so no access moves across the popf that restores interrupts; the per-site barriers it makes redundant are removed ([e4c5eb694d5](https://github.com/nitroos-org/reactos/commit/e4c5eb694d5))
- Amd64 ERESOURCE lock and per-CPU handle cache held with interrupts disabled instead of an IRQL raise/lower pair, NTFS-NG call-site accounting lock-free (registry open/close -22%, events -15%, file handles -16%); IRQL benchmark ... ([b67b107c313](https://github.com/nitroos-org/reactos/commit/b67b107c313))
- A present NULL DACL or SACL (offset 0) is a valid relative security descriptor, as on Windows; the registry root descriptors keep the NULL SACL present like Windows 10 ([824a4cfaacd](https://github.com/nitroos-org/reactos/commit/824a4cfaacd))
- Registry hive roots, SAM, SECURITY and the override keys get the Windows 10 security descriptors at build and setup time (mkhive, usetup, samsrv, userenv; nothing enforced yet) ([6d9adbf16b0](https://github.com/nitroos-org/reactos/commit/6d9adbf16b0))
- Registry ACL R3: keys store a real security descriptor on create (inherited from the parent as Windows does, shared cells, kept across save/reload/rename); queries and opens unchanged ([edf6b377992](https://github.com/nitroos-org/reactos/commit/edf6b377992))
- RegLoadMUIStringW follows the documented contract (non-@ values, pcbData, base directory, truncation); EnumDynamicTimeZoneInformation sets ERROR_SUCCESS (advapi32 registry winetest 530 -> 68 failures) ([0383ed8265f](https://github.com/nitroos-org/reactos/commit/0383ed8265f))
- SetEntriesInAcl REVOKE_ACCESS keeps the trustee's deny entries and old entries keep their order; registry functions on the HKEY_PERFORMANCE_* keys answer as Windows ([9b8068114a9](https://github.com/nitroos-org/reactos/commit/9b8068114a9))
- Registry ACL audit mode, default on, no change to any open or create (logs what Windows would deny; /REGACL boot switch; the log writer stops before the file systems at shutdown) ([e278fb01803](https://github.com/nitroos-org/reactos/commit/e278fb01803))
- SeFilterToken treats NULL optional lists as none (no empty restricted token); HKEY_PERFORMANCE_DATA serves Counter/Help/Explain names by prefix with the End Marker entry; advapi32 registry and security winetests run to the end ([fd3ce79d791](https://github.com/nitroos-org/reactos/commit/fd3ce79d791))
- Windows.Foundation.Metadata.ApiInformation in wintypes (answers what NitroOS provides: contracts as RoIsApiContractPresent, registered classes, own members), registered in both registry views ([78c08d0cc6c](https://github.com/nitroos-org/reactos/commit/78c08d0cc6c))
- NtSetIoCompletionEx uses its IoCompletionReserve as packet storage (one packet per reserve, as on Windows), NtQueryObject handle flags (Inherit as BOOLEAN, protect-from-close) and non-paged pool type as on Windows 8+ ([cae04d097f4](https://github.com/nitroos-org/reactos/commit/cae04d097f4))
- Kernel routines drivers import (ExAllocatePool2/3, IoCreateSystemThread, IoWMISetNotificationCallback, IoValidateDeviceIoControlAccess, wdmsec IoCreateDeviceSecure, Ob WithTag, processor groups, extended lookasides), first-call ... ([310806ab075](https://github.com/nitroos-org/reactos/commit/310806ab075))
- The 32-bit registry view holds the same Windows NT\CurrentVersion version values as the 64-bit view, as on Windows ([51190baa432](https://github.com/nitroos-org/reactos/commit/51190baa432))
- The registry value CurrentVersion is "6.3" on Windows 10, as on Windows (installers compare it as a string) ([661780db960](https://github.com/nitroos-org/reactos/commit/661780db960))
- The configuration manager finds a value by name through a per-key table of value names and reads value lists under the shared KCB lock; RegEnumValueA asks for each value once ([811f96f3679](https://github.com/nitroos-org/reactos/commit/811f96f3679))
- A failed open procedure takes back the handle counts instead of asserting; ConsoleCtrlEventRace no longer floods processes with handler threads ([e7401c36fb6](https://github.com/nitroos-org/reactos/commit/e7401c36fb6))
- Process creation into another session (SeTcbPrivilege, session must exist), CreateProcessAsUser through CreateProcessInternalW; Windows parity measured for a token naming a missing session (access denied) and a foreign-session ... ([e64860ec78e](https://github.com/nitroos-org/reactos/commit/e64860ec78e))
- Wait completion packets (NtCreate/NtAssociate/NtCancelWaitCompletionPacket) with Windows statuses; Go 1.26 programs start and run ([7822b8ede1a](https://github.com/nitroos-org/reactos/commit/7822b8ede1a))
- Win32k and its GDI drivers load as session images with NitroSessions; session-space lookaside lists and resources stay off the global lists (off by default) ([98fd21627ee](https://github.com/nitroos-org/reactos/commit/98fd21627ee))
- NtCreateKeyTransacted, NtOpenKeyTransacted(Ex) as system calls (no transaction manager: the handle is validated as on Windows); Office C2R's per-process virtual registry now installs ([9ecdda1f721](https://github.com/nitroos-org/reactos/commit/9ecdda1f721))
- NtAllocateReserveObject, NtQueueApcThreadEx with a user APC reserve, NtSetIoCompletionEx; wait completion packet entry points (not implemented) ([a009a857640](https://github.com/nitroos-org/reactos/commit/a009a857640))
- Security fix (described generically): hardening in this area; details withheld until users have had time to update ([e546eb21b61](https://github.com/nitroos-org/reactos/commit/e546eb21b61))
- (part 2): object type locks are exclusive push locks that their owner may take again (no shared mode: no nested-shared self-deadlock), apitest ObjectLockStress; amd64 PSEH scope records stored member by member (SEH split scan: 0 ... ([590d4455811](https://github.com/nitroos-org/reactos/commit/590d4455811))

</details>

<details><summary>I/O + fs libs (4 fixes)</summary>

- Filter manager follow-ups: FsFilter callbacks for every operation and driver, deleted volumes live until their last reference (waited for on a thread of their own), device deletion notifies once ([8ae174c3b89](https://github.com/nitroos-org/reactos/commit/8ae174c3b89))
- Filter Manager review findings: post-create at PASSIVE, safe completion processing, section synchronization for minifilters, volume removal without dismount, message buffers as the WDK says, port lock not held across a send, fast ... ([822badfd265](https://github.com/nitroos-org/reactos/commit/822badfd265))
- Minifilter-grade filter manager (altitudes, instances, contexts, communication ports, fast I/O) with review fixes ([0f963c89734](https://github.com/nitroos-org/reactos/commit/0f963c89734))
- IoConnectInterruptEx no longer reports success for interrupts it did not connect: line-based connects the device's single translated line interrupt, message-based falls back to it (Version = CONNECT_LINE_BASED) when no message ... ([1854de0d618](https://github.com/nitroos-org/reactos/commit/1854de0d618))

</details>

<details><summary>NTFS (ntfsng) (29 fixes)</summary>

- Group commit for extending non-cached write-through writes (data before size kept; commit at flush, close, 32 MB or 1 s) ([631a88b46c4](https://github.com/nitroos-org/reactos/commit/631a88b46c4))
- NTFS-NG files no longer stuck until reboot after an open raced with the delete of an earlier file of the same MFT record ([c951061f764](https://github.com/nitroos-org/reactos/commit/c951061f764))
- NTFS-NG directory listing faster (no 8.3 test for long-only names, snapshot chunk reuse, larger inode hash, inode prefetch) ([404a6267585](https://github.com/nitroos-org/reactos/commit/404a6267585))
- NTFS-NG allocates after a free without waiting for the journal commit, skipping clusters whose free is not committed yet (and never refusing a write a full volume has room for); per-file tcc compiles -24% ([d330a85e7c8](https://github.com/nitroos-org/reactos/commit/d330a85e7c8))
- NTFS-NG FcbListLock is a guarded mutex, no IRQL raise/lower per FCB list operation (file open/close -3.5%, GetFileAttributes -6.3%) ([9c8861454ab](https://github.com/nitroos-org/reactos/commit/9c8861454ab))
- NTFS-NG frees clusters after waiting only for the in-flight transfers on those clusters (not every unlocked transfer on the volume) ([7bf3fa0567a](https://github.com/nitroos-org/reactos/commit/7bf3fa0567a))
- NTFS-NG page cache limit per mapping scales with memory (long-name directory listing 238 -> 6 ms, small reads 4-9x faster) ([750c86b0f09](https://github.com/nitroos-org/reactos/commit/750c86b0f09))
- NTFS-NG verbose dump shows the inode cache against its limits ([004d31b2a29](https://github.com/nitroos-org/reactos/commit/004d31b2a29))
- ReadDirectoryChangesExW and NtNotifyChangeDirectoryFileEx with FILE_NOTIFY_EXTENDED_INFORMATION records (NTFS-NG); Gradle 9 starts ([3a80a5bdb4e](https://github.com/nitroos-org/reactos/commit/3a80a5bdb4e))
- NTFS-NG builds each paging-write device request as a partial MDL of the caller's MDL, so no request MDL is freed while still mapped (Debug boot assert; leaked system mappings in Release) ([8610cae2ead](https://github.com/nitroos-org/reactos/commit/8610cae2ead))
- Fewer device register accesses per storage request (AHCI/IDE interrupt), flush SRBs completed at dispatch, and NTFS-NG skips its writeback scan when nothing was made dirty (FlushFileBuffers 1.6x, 4 KB requests ~20% faster on AHCI) ([7d1767391d4](https://github.com/nitroos-org/reactos/commit/7d1767391d4))
- NTFS-NG bounds the memory of its unused-inode list in bytes as well as by count ([63ab9828903](https://github.com/nitroos-org/reactos/commit/63ab9828903))
- NTFS-NG takes an existing FCB in the create walk and queries parked FCBs without the volume lock; the I/O manager offers FastIoQueryOpen for by-name attribute queries (git status 153 -> 93 ms) ([5ff04232822](https://github.com/nitroos-org/reactos/commit/5ff04232822))
- NTFS-NG unlocked paging reads with an unaligned tail, one volume lock per open of an existing file, a per-directory name cache; CopyFileEx reads small sources through the cache (tree copy 3x faster) ([452b419126c](https://github.com/nitroos-org/reactos/commit/452b419126c))
- NTFS-NG sizes a decompressed runlist from its mapping pairs instead of a whole page (nonpaged per cached inode ~11.4 KB -> ~3.7 KB) ([f31eaca477b](https://github.com/nitroos-org/reactos/commit/f31eaca477b))
- NTFS-NG gives its nonpaged inode and page cache back while the Mm low-memory condition is signalled ([68ef9520a00](https://github.com/nitroos-org/reactos/commit/68ef9520a00))
- ATAPI sends queued FUA only to drives that advertise it (write-through durable on AHCI); NTFS-NG write-through data and journal slots in flight together, FUA in place of barrier flushes where honoured ([f50e8be35c6](https://github.com/nitroos-org/reactos/commit/f50e8be35c6))
- AHCI channels issue 512 KB ATA commands to 48-bit LBA disks (PATA/ATAPI unchanged); NTFS-NG pure-growth appends take PagingIoResource shared and up to 4 paging write transfers of one stream overlap with the initialized size ... ([2f50dc84a74](https://github.com/nitroos-org/reactos/commit/2f50dc84a74))
- NTFS-NG sets ARCHIVE on rename and on SetFileTime through a handle that wrote (as Windows 10, measured); STATUS_UNEXPECTED_IO_ERROR maps to ERROR_IO_DEVICE; a write-blocked NTFS-NG journal logs one event in Release builds ... ([623f23b85ad](https://github.com/nitroos-org/reactos/commit/623f23b85ad))
- NTFS-NG keeps a compact stat of evicted inodes (direct-mapped by record number, voided when the inode is loaded again) so a directory listing does not re-read every file's record at low memory; 1 GB: dir_enum 1954 -> 88 ms ... ([db05dd62c5d](https://github.com/nitroos-org/reactos/commit/db05dd62c5d))
- NTFS-NG retained journal transactions (a small durable commit stops at its COMMITTED header; its pages go in place at a later checkpoint, two slot areas alternate, freed clusters are not reused before the checkpoint ... ([307ad8bf3ca](https://github.com/nitroos-org/reactos/commit/307ad8bf3ca))
- NTFS-NG keeps recent directory security descriptors (no device read per create), the lazy writer leaves dirty data younger than 3 s alone unless the cache is under pressure, and a cache map uninitialized to zero purges before ... ([74349a6ce7b](https://github.com/nitroos-org/reactos/commit/74349a6ce7b))
- A section-view fault that cannot get a page for its page table returns STATUS_NO_MEMORY (MmHoldPageTable faults the paging levels in itself) so MmAccessFault drops the address-space lock, waits for the balancer and retries ... ([53a004d4e37](https://github.com/nitroos-org/reactos/commit/53a004d4e37))
- Cc writes back in 1 MB requests of up to 4 adjacent views and starts write-behind as soon as a writer leaves much dirty data; NTFS-NG paging writes go to the device from the caller pages with every request of a transfer in ... ([3dec5f68428](https://github.com/nitroos-org/reactos/commit/3dec5f68428))
- (part 1 of 2): NTFS-NG directory queries reject names from the literal ends of the expression (winsxs continuation query 0.54 -> 0.017 ms), the native side of a WoW64 process creates no activation context, the LCID index table is ... ([89cef7dafaa](https://github.com/nitroos-org/reactos/commit/89cef7dafaa))
- Dirty cache data is paced by the lazy writer's progress past the dirty threshold instead of a clock tick from a quarter of it; NTFS-NG asks CcCanIWrite before taking its resources; 64-page flush clusters and a 256 KB write-back ... ([53d949b802f](https://github.com/nitroos-org/reactos/commit/53d949b802f))
- NTFS-NG keeps what a directory listing and a small-file read need cached (unused inodes and idle FCBs scale with RAM, 4096/1024 min, 32768 max; FcbCacheMax still overrides), no view padding in the purge/flush range; 10k-entry ... ([bb0ba444588](https://github.com/nitroos-org/reactos/commit/bb0ba444588))
- The loader records every unresolved driver import (debugger line, System event, HKLM\SYSTEM\...\NitroOS\MissingDriverImports); ObRegisterCallbacks/ObUnRegisterCallbacks/ObGetFilterVersion for process and thread handles (callbacks ... ([3803829c883](https://github.com/nitroos-org/reactos/commit/3803829c883))
- Pager pacing slots by thread id with elapsed-time charging, writer stamp/count ordering (reader snapshots counts first), Want clamped on aging passes, exact /NITROPAGERDEBUG token, NTFS-NG takes the debug decision from the ... ([bab03062114](https://github.com/nitroos-org/reactos/commit/bab03062114))

</details>

<details><summary>Btrfs + other fs (3 fixes)</summary>

- A cancelled listen IRP that a connect already took off the queue is not unlinked twice (0x139 at shutdown) ([7a86e6b758a](https://github.com/nitroos-org/reactos/commit/7a86e6b758a))
- Btrfs allocates the disk space when SetEndOfFile grows a file (a full volume fails the call instead of losing mapped writes) ([036a438ee8d](https://github.com/nitroos-org/reactos/commit/036a438ee8d))
- Ext2 no longer fails fast when two threads miss the same block (racing getblk unlinked an unlinked buffer head) ([d2276da0acc](https://github.com/nitroos-org/reactos/commit/d2276da0acc))

</details>

<details><summary>win32k/GDI/USER (69 fixes)</summary>

- A desktop heap commits what DESKTOPINFO needs up front and never commits without a user mapping (creating many desktops in one process no longer fails; Debug builds bugchecked 0x50) ([52f73135ce7](https://github.com/nitroos-org/reactos/commit/52f73135ce7))
- D3D11/D3D10 CreateDevice without an adapter returns the Windows codes for UNKNOWN, REFERENCE and SOFTWARE ([a7ee0df4745](https://github.com/nitroos-org/reactos/commit/a7ee0df4745))
- D3D11/D3D10 devices of the WARP driver type are created on the WARP adapter like Windows ([50e6ff46f86](https://github.com/nitroos-org/reactos/commit/50e6ff46f86))
- EngLineTo steps only the visible part of a line; a dashed pen whose segments sum to 2^32 no longer divides by zero in the kernel ([15fb9057fd0](https://github.com/nitroos-org/reactos/commit/15fb9057fd0))
- Huge ellipse, arc, pie, chord and round rectangle boxes wrap their device coordinates to 28 bits like Windows instead of clamping ([494815ebcf5](https://github.com/nitroos-org/reactos/commit/494815ebcf5))
- Framebuf copies changed rectangles to the frame buffer once per clock tick instead of after every drawing call (BitBlt to a window 12 -> 7.1 ms, ScrollWindowEx -42%) ([3b3d553ac25](https://github.com/nitroos-org/reactos/commit/3b3d553ac25))
- Arc, Pie, Chord, Ellipse and RoundRect never divide by zero, overflow or stall on huge boxes; the work is proportional to the visible rows ([8f1fd735f36](https://github.com/nitroos-org/reactos/commit/8f1fd735f36))
- Amd64 GDI transform matrices truncate like i386 and Windows; the world-to-device matrix is rebuilt only when the mapping changes; MM_ISOTROPIC with a zero viewport width like Windows ([62a190d98ab](https://github.com/nitroos-org/reactos/commit/62a190d98ab))
- D2d1 compiles its shaders once per device, not per device context (Wine 3bb5938957f, 50c7c25cfa8): Word cold start compiles 168 shaders instead of 768 ([4e220aaf32b](https://github.com/nitroos-org/reactos/commit/4e220aaf32b))
- Window redirection level 1: a burst of scrolls is presented once per frame and its end reaches the screen within two frames (ScrollWindowEx at L1 faster than L0) ([65c06523bba](https://github.com/nitroos-org/reactos/commit/65c06523bba))
- Wide pens: Arc, Pie, Chord, Ellipse and RoundRect are stroked paths rotated with the world transform, as on Windows; path and region code safe at extreme coordinates ([cff3d913564](https://github.com/nitroos-org/reactos/commit/cff3d913564))
- A repeated DwmExtendFrameIntoClientArea keeps the window's redirection surface (black first Firefox x86 frame at window-surfaces level 1) ([cf2c3b2604c](https://github.com/nitroos-org/reactos/commit/cf2c3b2604c))
- Wined3d client threads block instead of spinning while the command-stream thread is busy (Word heavy cold start CPU halved) ([152624de716](https://github.com/nitroos-org/reactos/commit/152624de716))
- Bezier flattening in 64-bit arithmetic (no kernel divide error for curves near the coordinate limits) ([7eea3e86764](https://github.com/nitroos-org/reactos/commit/7eea3e86764))
- Amd64 coordinate transforms round to nearest like i386 and Windows (LPtoDP/DPtoLP no longer truncate) ([943d060a59a](https://github.com/nitroos-org/reactos/commit/943d060a59a))
- Bezier flattening keeps its points when the buffer grows (no uninitialised pool memory in large curves) ([1676c9f5a1f](https://github.com/nitroos-org/reactos/commit/1676c9f5a1f))
- A WIC bitmap render target keeps the bitmap's pixels it does not draw over, and its target bitmap is premultiplied (measured on Windows 10) ([9704ac4f853](https://github.com/nitroos-org/reactos/commit/9704ac4f853))
- Mesa3D LLVM pack missing is loud (banner, renderer file) and an error with NITRO_REQUIRE_LLVM_PACKS ([b97a406903c](https://github.com/nitroos-org/reactos/commit/b97a406903c))
- Wide pen widths under the DC transform for Arc, Pie, Chord, Ellipse, RoundRect, Rectangle, Polygon and stroked paths; arcs reverse under a flipping world transform ([df60e85898c](https://github.com/nitroos-org/reactos/commit/df60e85898c))
- Mesa shader disk cache on Windows (llvmpipe/lavapipe packs 26.3.0-dc2): shaders compiled once per user, protected cache directories ([20cb411e10f](https://github.com/nitroos-org/reactos/commit/20cb411e10f))
- A layered window that has had neither SetLayeredWindowAttributes nor UpdateLayeredWindow is not drawn ([4631d7e4b35](https://github.com/nitroos-org/reactos/commit/4631d7e4b35))
- Direct2D CombineWithGeometry (union, intersect, xor, exclude) ([dfc1fe5a4a2](https://github.com/nitroos-org/reactos/commit/dfc1fe5a4a2))
- Direct2D bitmaps that cannot draw keep a shader resource view when their texture can be sampled (Excel's formula bar, grid and tabs no longer black) ([b2f5f3c2480](https://github.com/nitroos-org/reactos/commit/b2f5f3c2480))
- D3D11 reports pixel-shader stencil reference only at feature level 11_0 and above (fixes the d3d11 winetest crash in test_stencil_export) ([71d1a96b00b](https://github.com/nitroos-org/reactos/commit/71d1a96b00b))
- TranslateCharsetInfo(TCI_SRCLOCALE) fails for an LCID Windows does not know (nl-SR and the other Wine-only locales), as measured on Windows 10 ([99038a9ac5d](https://github.com/nitroos-org/reactos/commit/99038a9ac5d))
- Win32k bounds: MaskBlt mask origin and extent checked (engine and driver blits), RLE decoding never reads past the captured bits, GetDIBits never copies more than the caller's buffer holds (as measured on Windows 10) ([7891ca42f57](https://github.com/nitroos-org/reactos/commit/7891ca42f57))
- TranslateCharsetInfo returns the locale's supported code pages in fsCsb[1] and knows the UTF-8 code page, as measured on Windows 10 ([bbb54efe139](https://github.com/nitroos-org/reactos/commit/bbb54efe139))
- DwmSetWindowAttribute returns E_INVALIDARG for the border, caption and text colour attributes, as Windows 10 19045 does ([1a49de12d7d](https://github.com/nitroos-org/reactos/commit/1a49de12d7d))
- Wide-pen Polyline/PolyPolyline/PolyBezier neither use nor move the current position and PolyBezierTo starts at it, as on Windows; pen width is in logical units under the mapping mode and world transform (elliptical pen for ... ([d0f86e700da](https://github.com/nitroos-org/reactos/commit/d0f86e700da))
- Direct2D batches consecutive ellipses, rounded rectangles, paths and strokes with the same brush and transform into one draw per shape stream (FillEllipse x500 6.3x faster; images unchanged) ([6211cbd2737](https://github.com/nitroos-org/reactos/commit/6211cbd2737))
- GDI arcs, pies, chords and ellipses draw the same pixels into a window and a memory DC (DC origin rounding, pie/chord brush origin, 45-degree dash axis) ([50a185c1dc6](https://github.com/nitroos-org/reactos/commit/50a185c1dc6))
- GDI shapes under transforms: wide-pen Polygon no longer transformed twice, a rotated Rectangle reads a current world transform, flipped axes order the device corners; wide-pen Rectangle outline independent of the current position ([3bcf3621f3c](https://github.com/nitroos-org/reactos/commit/3bcf3621f3c))
- Redirected window surfaces R4: each GDI primitive records the area it drew, up to 8 dirty rectangles per window, EndPaint presents only what was drawn, the frame timer no longer blocks on the USER lock (level 1 LibreOffice redraw ... ([06121702b1e](https://github.com/nitroos-org/reactos/commit/06121702b1e))
- Direct2D draws SVG documents (paths with all commands, basic shapes, fill and opacity, viewBox and preserveAspectRatio) and command lists end their draw and play back on a device context ([e7f15b4ae66](https://github.com/nitroos-org/reactos/commit/e7f15b4ae66))
- Redirected window surfaces level 2 follow-ups (64-bit window edge in the screen copy, budget check shared by both paths, layered history ends with WS_EX_LAYERED, no stale surface under a snapshot blend, desktop paint failure ... ([4001a531b3c](https://github.com/nitroos-org/reactos/commit/4001a531b3c))
- Direct2D SVG documents (ID2D1SvgDocument / ID2D1SvgElement: empty document with a root svg element, element tree and attributes; drawing and parsing still stubs) ([0a73320ef02](https://github.com/nitroos-org/reactos/commit/0a73320ef02))
- Redirected window surfaces stage R2 (NitroRedirection >= 2): layered and glass windows composed from their surfaces; source ranges clipped in 64-bit; levels 0/1 unchanged ([fa14a2dae77](https://github.com/nitroos-org/reactos/commit/fa14a2dae77))
- Uxtheme DrawThemeParentBackgroundEx (WinDirStat's drive list was empty: the stub raised during dialog paint) ([9e58fb77c93](https://github.com/nitroos-org/reactos/commit/9e58fb77c93))
- An ended session's desktop windows go after CSRSS's last thread's own windows; surviving clients fail cleanly instead of using them; window-station delete closes under the USER lock ([64579f0bbfb](https://github.com/nitroos-org/reactos/commit/64579f0bbfb))
- A session's win32k tears down cleanly: the raw input thread ends when CSRSS is terminated, the desktop windows stay until CSRSS has no thread left ([dc15e2a50b1](https://github.com/nitroos-org/reactos/commit/dc15e2a50b1))
- WH_FOREGROUNDIDLE runs only when a thread really waits, not on every empty PeekMessage (SWT/Eclipse apps no longer spin at the splash), and LpkGetCharacterPlacement no longer reads past the caller's arrays ([7710c4da319](https://github.com/nitroos-org/reactos/commit/7710c4da319))
- Window station creation checks where the object landed, creates the switch event only after a successful insert, and never touches the new object when the name already existed (sessions) ([0a6d72171c4](https://github.com/nitroos-org/reactos/commit/0a6d72171c4))
- SetWindowPos/MoveWindow, WM_HELP, EDIT WM_UNDO and WM_MDICREATE leave the thread's last error as Windows does ([25438d769d4](https://github.com/nitroos-org/reactos/commit/25438d769d4))
- Sizing a window whose WM_GETMINMAXINFO says 'no maximum' (Chromium, Electron) no longer overflows into a MAXLONG-sized rectangle ([669b0f6d8ef](https://github.com/nitroos-org/reactos/commit/669b0f6d8ef))
- DirectWrite system fallback ends the symbol, punctuation and currency ranges with Lucida Console (DejaVu Sans Mono based), so Chrome's Tab-key hint (U+21E5) no longer draws as a box ([c436c3da74f](https://github.com/nitroos-org/reactos/commit/c436c3da74f))
- Sessions P3b piece 2: a second session with its own win32k (NitroSessions 5, off by default); window stations and desktops are used only from their own session ([8f5e594c925](https://github.com/nitroos-org/reactos/commit/8f5e594c925))
- Direct2D draws held-back solid fills together, wined3d sends only the state that changed on a context-state swap, and DirectWrite builds a font's character map once under a lock (fixes an occasional crash) ([5e404ec4cf5](https://github.com/nitroos-org/reactos/commit/5e404ec4cf5))
- Frame controls keep their Marlett fonts, font realization caches face names, the edit control looks lines up in an array, measured glyph advances are kept (window create/show/destroy 4.8x, edit scroll 5-10x faster) ... ([8f83d445021](https://github.com/nitroos-org/reactos/commit/8f83d445021))
- An owner deleting its DC or region takes the object's lock and gives the user attribute back in its own context ([d3ef2259b02](https://github.com/nitroos-org/reactos/commit/d3ef2259b02))
- A redirected DC holds a reference on its window's redirection record (no orphan reaper) ([86525d11cf4](https://github.com/nitroos-org/reactos/commit/86525d11cf4))
- DeleteDC of a window DC runs under the USER lock (the DCE list was changed without it); an owner frees a DC's attribute in its own context when another process held it ([232db1f845a](https://github.com/nitroos-org/reactos/commit/232db1f845a))
- StretchBlt SRCCOPY into 32 bpp with precomputed source columns (40-90x faster); text output hashes the glyph cache and reuses one glyph mask surface per string ([4b524fe4497](https://github.com/nitroos-org/reactos/commit/4b524fe4497))
- FreeType mono renderer no longer leaks its worker and pool on early returns (DirectWrite out-of-memory crash; also used by win32k); DirectWrite caches glyphs at sub-pixel positions (DrawText about 23x faster) ([6a803ddc5dc](https://github.com/nitroos-org/reactos/commit/6a803ddc5dc))
- A GDI object deleted while another thread holds it is freed by that thread in its owner's context; a lock re-checks owner and validity after waiting (fixes the destroy-while-drawing bugcheck) ([67f35d967cc](https://github.com/nitroos-org/reactos/commit/67f35d967cc))
- D3D11 shared resources across processes (legacy and NT handles, IDXGIKeyedMutex); opened section headers validated ([7c984af18ee](https://github.com/nitroos-org/reactos/commit/7c984af18ee))
- GDI batch, font buffer, request size and Eng entry point input checks; gdi32 ANSI extended font conversion ([f56490b0f9c](https://github.com/nitroos-org/reactos/commit/f56490b0f9c))
- User32 EDIT, COMBOBOX and MDICLIENT leave the caller's last error alone as on Windows ([6a6e212f2c9](https://github.com/nitroos-org/reactos/commit/6a6e212f2c9))
- User32 STATIC sets its FNID once (WM_NCCREATE) and sends no STN_ENABLE/DISABLE without a parent; EnableWindow/SetWindowLong leave the last error untouched as on Windows ([3bf6ba8b901](https://github.com/nitroos-org/reactos/commit/3bf6ba8b901))
- GdiSwapBuffers/SetPixelFormat no longer take the USER lock under a DC lock (session deadlock); redirection owner check, frame-timer budget, StretchDIBits hazard, PrintWindow rect, ExtFloodFill rect ([a79e6a5fbe4](https://github.com/nitroos-org/reactos/commit/a79e6a5fbe4))
- Mesa 26.3.0 GPU pack (llvmpipe, lavapipe, softpipe; LLVM 19.1.7), with the upstream llvmpipe indirect GS input fix ([5026f1d20c3](https://github.com/nitroos-org/reactos/commit/5026f1d20c3))
- A held CS_OWNDC/CS_CLASSDC DC draws the whole window again after a paint (clip region dropped before the visible region is restored) ([2b5e344c65f](https://github.com/nitroos-org/reactos/commit/2b5e344c65f))
- Redirected window surfaces stage R1 (NitroRedirection >= 1) ([734f0a8a6e9](https://github.com/nitroos-org/reactos/commit/734f0a8a6e9))
- Redirected window surfaces stage R0 (NitroRedirection, default 0) ([9f249a661d1](https://github.com/nitroos-org/reactos/commit/9f249a661d1))
- Direct2D built-in effects rendering ([2e082dbd99c](https://github.com/nitroos-org/reactos/commit/2e082dbd99c))
- Wined3d GLSL selects a geometry shader input vertex chosen at run time with constant indices (llvmpipe's GS JIT reads out of bounds for shader_in[expr]); Chromium/VS Code GPU process crash 7/20 -> 0/20 ([3b1ee2d30b6](https://github.com/nitroos-org/reactos/commit/3b1ee2d30b6))
- User32 fails a cross-process WM_COPYDATA with an unreadable parameter as Windows does (no sender crash); in-context WinEvent hooks load the hook DLL in the target by path (never into system, csrss, protected or other-session ... ([480ddcb9ee3](https://github.com/nitroos-org/reactos/commit/480ddcb9ee3))
- D2d1 registers every built-in effect (as creatable stubs), accepts stock/source/compute transform nodes, and ID2D1Effect::SetInput(NULL) clears the input; Word's NetUI windows now create swap chains and paint (splash, backstage ... ([ab11aa58b56](https://github.com/nitroos-org/reactos/commit/ab11aa58b56))
- Security fix (described generically): hardening in this area; details withheld until users have had time to update ([3105fa30ff6](https://github.com/nitroos-org/reactos/commit/3105fa30ff6))
- Win32k font queries fail cleanly when the DC font handle names no font (no ASSERT/NULL use), the realization stays locked while its font is read, charset info returns DEFAULT_CHARSET with a cleared signature and unlocks the DC on ... ([2587a5813eb](https://github.com/nitroos-org/reactos/commit/2587a5813eb))

</details>

<details><summary>Console/csrss (8 fixes)</summary>

- SetConsoleOutputCP(CP_UTF8) works again (dotnet SDK crashed since TranslateCharsetInfo learned UTF-8) ([d1c5d463067](https://github.com/nitroos-org/reactos/commit/d1c5d463067))
- Csrss serves the requests of a thread it was not told about (thread-pool and RtlCreateUserThread threads calling CreateThread; Word no longer aborts after its splash) ([cb4b6977b00](https://github.com/nitroos-org/reactos/commit/cb4b6977b00))
- Consrv frees a deleted console and its frontend block (csrss grew ~760 B per console child), with a lookup count so a console being validated is never freed under it ([3a73841a116](https://github.com/nitroos-org/reactos/commit/3a73841a116))
- ConsoleCtrlEventRace checks that an unpaced burst of Ctrl+Break is delivered completely and one handler at a time, as on Windows ([bd88ade9e02](https://github.com/nitroos-org/reactos/commit/bd88ade9e02))
- The console input thread leaves its desktop before closing it (csrss no longer keeps a desktop handle per console), and a console is handed an input thread that stays until its window exists ([f5d651feafa](https://github.com/nitroos-org/reactos/commit/f5d651feafa))
- A failed thread insert gives back the process reference it took, so csrss no longer keeps handles of exited processes ([4c2ab8ff447](https://github.com/nitroos-org/reactos/commit/4c2ab8ff447))
- Basesrv registers a new thread only under the ids of the handle it was given, fetched once; csrsrv releases only what it took when a thread insert fails ([653a737898b](https://github.com/nitroos-org/reactos/commit/653a737898b))
- Basesrv takes process and thread ids from the handles and registers only the caller's child; csrss's Sb port serves only the session manager that started it ([b918ee0f992](https://github.com/nitroos-org/reactos/commit/b918ee0f992))

</details>

<details><summary>WoW64 (12 fixes)</summary>

- WoW64 thunks NtCreateProcess(Ex); 32-bit threads get the NT6 all-access mask ([b77759c6904](https://github.com/nitroos-org/reactos/commit/b77759c6904))
- Reported handle access without the internal protect-close bit, WoW64 object types table, invariant font signature and LCIDToLocaleName(0x1000) as on Windows 10 ([f7015e4f8dd](https://github.com/nitroos-org/reactos/commit/f7015e4f8dd))
- WoW64 thunks for NtQueryMultipleValueKey, NtCompactKeys, NtQueryOpenSubKeysEx, NtInitializeRegistry and NtLockProductActivationKeys ([4d4cdee584b](https://github.com/nitroos-org/reactos/commit/4d4cdee584b))
- NTLM without an external helper (NTLMv2 exchange and session security in secur32, response check in msv1_0, WoW64 thunks), RPC_C_AUTHN_WINNT for rpcrt4 clients and servers; network logons to blank-password accounts refused unless ... ([563181241cd](https://github.com/nitroos-org/reactos/commit/563181241cd))
- ICU data lists the locales the NLS gained (335 names, every one resolves); RtlLcidToLocaleName honours AllocateDestinationString (the x64 / WoW64 locale winetest no longer crashes) ([dddc1fa34de](https://github.com/nitroos-org/reactos/commit/dddc1fa34de))
- A 32-bit debugger can launch a 32-bit child under WoW64 (the native layer's debug print exceptions are continued, not left unhandled: the child no longer exits 0x40010006) ([35ba3032b45](https://github.com/nitroos-org/reactos/commit/35ba3032b45))
- Wow64ThreadSegments apitest (segment registers of 64-bit and WoW64 threads created and joined in a loop); verified clean on Intel P/E cores ([ed332ca1742](https://github.com/nitroos-org/reactos/commit/ed332ca1742))
- Ntdll_apitest FaultContext checks the exception record and context of concurrent faults exactly (x64 and WoW64) ([a85cbb86d35](https://github.com/nitroos-org/reactos/commit/a85cbb86d35))
- 32-bit processes get WM_NEXTMENU, the drag family, IME, WM_COPYGLOBALDATA and DDE messages translated under WoW64 ([af9a4988e19](https://github.com/nitroos-org/reactos/commit/af9a4988e19))
- WoW64 translates WM_HELP's HELPINFO both ways and passes owner-draw, MDI create, menu object and combo box info pointers for synchronous same-process sends ([e38c531248c](https://github.com/nitroos-org/reactos/commit/e38c531248c))
- WoW64 passes WM_GETDLGCODE, WM_NEXTDLGCTL, WM_PARENTNOTIFY and pointer messages with a NULL lParam instead of failing them (32-bit dialogs with scroll bars) ([ab002249c02](https://github.com/nitroos-org/reactos/commit/ab002249c02))
- The application compatibility cache is filled (basesrv inserts images the shim database has nothing for, checked by handle identity on local disks; layered or shimmed images never), keyed by NT path with size/time invalidation ... ([42c85b7aab3](https://github.com/nitroos-org/reactos/commit/42c85b7aab3))

</details>

<details><summary>Networking (115 fixes)</summary>

- Setup and the first start of an upgraded install no longer wait for the service manager when the network components are installed (about 2 minutes saved per install) ([ae33cd4badf](https://github.com/nitroos-org/reactos/commit/ae33cd4badf))
- The TCP/IPv6 component and its network connection properties page ([5d8e9bdc395](https://github.com/nitroos-org/reactos/commit/5d8e9bdc395))
- DHCP client: with two adapters the service no longer counts an empty read as an error or reads a freed protocol (service crash) ([d11a708ce1a](https://github.com/nitroos-org/reactos/commit/d11a708ce1a))
- Security fix (described generically): hardening in this area; details withheld until users have had time to update ([04cb829e62f](https://github.com/nitroos-org/reactos/commit/04cb829e62f))
- Getsockname of an IPv6 TCP connection never holds the connection lock and the core lock together (deadlock with a link-local peer) ([56780be7fc1](https://github.com/nitroos-org/reactos/commit/56780be7fc1))
- Raw IPv6: an IPV6_HDRINCL source that is not on this machine is routed by the destination ([00e2f1e88f9](https://github.com/nitroos-org/reactos/commit/00e2f1e88f9))
- IPv4 multicast: IP_ADD_MEMBERSHIP reaches the stack, IPv4 groups are put in the adapter's multicast list ([19db4da0d57](https://github.com/nitroos-org/reactos/commit/19db4da0d57))
- IPv6 loopback-only follow-ups: PS module error text, sortpairs note, WSAENETUNREACH regression test ([2af18a24717](https://github.com/nitroos-org/reactos/commit/2af18a24717))
- DHCP client fixes: DISCOVER from 0.0.0.0 without a lease, earlier runs' automatic private and alternate addresses replaced cleanly, IpReleaseAddress/IpRenewAddress adapter name ([a1efe32e8fd](https://github.com/nitroos-org/reactos/commit/a1efe32e8fd))
- Raw IPv6 follow-ups: a privileged IPV6_HDRINCL sends any source address as Windows does, interlocked ICMP rate window ([dd8987b06d8](https://github.com/nitroos-org/reactos/commit/dd8987b06d8))
- DNS canonical names: A and AAAA answers carry the CNAME chain, cache keyed on the asked name and type, getaddrinfo takes the canonical name from the answer ([1045d916cb6](https://github.com/nitroos-org/reactos/commit/1045d916cb6))
- Security fix (described generically): hardening in this area; details withheld until users have had time to update ([b639e85215e](https://github.com/nitroos-org/reactos/commit/b639e85215e))
- Firewall report follow-ups: unpaired Platform2 stays unpaired, broad Block rules of every protocol counted, event ids by severity and an event when clean again ([5de26e6f145](https://github.com/nitroos-org/reactos/commit/5de26e6f145))
- IPv6 source address selection by RFC 6724 rules 1, 2, 3, 6, 7 and 8 with the prefix policy table, one function for the stack and user mode ([125f857ba85](https://github.com/nitroos-org/reactos/commit/125f857ba85))
- Raw IPv6 sockets with the Windows access rule, UDP port unreachable as WSAECONNRESET for both families, IPv6 UDP up to 65527 bytes, IPv6 option answers ([701124aa7e8](https://github.com/nitroos-org/reactos/commit/701124aa7e8))
- FD_CONNECT is posted before FD_WRITE after a connect ([93a6ae9b779](https://github.com/nitroos-org/reactos/commit/93a6ae9b779))
- DNS client cache follow-ups: flush on network change, full displaydns listing, 9701 for cache-only misses, seeded hash, 64-bit clock, whole owner names ([4f2e33ebf8b](https://github.com/nitroos-org/reactos/commit/4f2e33ebf8b))
- Firewall rule push: honour a rule's Platform list, count and log Block rules that block all traffic of their direction ([003787ef341](https://github.com/nitroos-org/reactos/commit/003787ef341))
- Ipv6Enable = 0 as DisabledComponents 0xFF: IPv6 off on the adapters, loopback and AF_INET6 sockets kept ([e2469473fda](https://github.com/nitroos-org/reactos/commit/e2469473fda))
- Fixes: NetWkstaUserEnum no longer crashes wkssvc or leaks, one buffer per call; widl marshals nested counted strings correctly on 32-bit ([47cb39ce267](https://github.com/nitroos-org/reactos/commit/47cb39ce267))
- The DHCP client automatic private address fix: keep one address across service restarts ([e90a673bef1](https://github.com/nitroos-org/reactos/commit/e90a673bef1))
- LsaQueryInformationPolicy and netapi32 info calls return one freeable block (no per-call leak) ([e0a76afb28c](https://github.com/nitroos-org/reactos/commit/e0a76afb28c))
- IPv4 and IPv6 change notifications kept apart like Windows; ResolveIpNetEntry2 solicits IPv6 neighbours with NS only ([537786dceae](https://github.com/nitroos-org/reactos/commit/537786dceae))
- The DHCP client service starts cleanly after a stop and start; its RPC interface stays registered while other netsvcs services stop ([26a404d4151](https://github.com/nitroos-org/reactos/commit/26a404d4151))
- IPv6 configuration API validates fields like Windows, serialises persistent-store writes, edits full stores ([51fe903b4c1](https://github.com/nitroos-org/reactos/commit/51fe903b4c1))
- No heap left behind per WSAStartup/WSACleanup, per socket or per DNS query ([74831ce5cc1](https://github.com/nitroos-org/reactos/commit/74831ce5cc1))
- Netsh interface ipv6|ipv4 set interface, ipv6 set privacy and add/set/delete prefixpolicy; ifmon in SysWOW64 ([7a7e50d790a](https://github.com/nitroos-org/reactos/commit/7a7e50d790a))
- Dnsapi keeps the IPv6 name servers and search domains between queries until an address change ([5267c2e1535](https://github.com/nitroos-org/reactos/commit/5267c2e1535))
- Stateful DHCPv6 client: IA_NA address, renew/rebind/release/decline/confirm, ipconfig /renew6 and /release6 ([437c28bcdca](https://github.com/nitroos-org/reactos/commit/437c28bcdca))
- The DNS client cache honours record TTLs, keeps negative answers, is bounded and hashed ([b8474470a28](https://github.com/nitroos-org/reactos/commit/b8474470a28))
- 16 IPv6 address slots per interface; temporary addresses keep a reserve and give way to stable, configured and DHCPv6 addresses ([df13571476c](https://github.com/nitroos-org/reactos/commit/df13571476c))
- UDP datagrams that arrive while a receive completes are held instead of dropped; multicast join/leave without an interface index ([1c820cd903b](https://github.com/nitroos-org/reactos/commit/1c820cd903b))
- SetIpInterfaceEntry for IPv4 and IPv6 (MTU, metric, router discovery, DAD transmits; active and persistent) and Set-NetIPInterface ([1063eb9f8d7](https://github.com/nitroos-org/reactos/commit/1063eb9f8d7))
- IPv6 temporary addresses (RFC 8981) with source address rule 7, on by default as in Windows 10 ([4011dfb21eb](https://github.com/nitroos-org/reactos/commit/4011dfb21eb))
- WinINet retries a failed TLS handshake through the proxy with a new CONNECT (and a new NTLM/Negotiate proxy login) instead of the unresolved target ([8f56fda8802](https://github.com/nitroos-org/reactos/commit/8f56fda8802))
- Select() returns when a polled socket is closed meanwhile; select and WSAEventSelect results and errors as Windows ([02ff7e8d510](https://github.com/nitroos-org/reactos/commit/02ff7e8d510))
- Honour Tcpip6 DisabledComponents: 0x10/0xFF leave IPv6 on the loopback only, 0x20 prefers IPv4 ([7b031bf3a98](https://github.com/nitroos-org/reactos/commit/7b031bf3a98))
- IPv6 write side of route and netsh interface ipv6: add, set and delete addresses, routes and DNS servers, active and persistent ([e47f9c93213](https://github.com/nitroos-org/reactos/commit/e47f9c93213))
- IPv6 DNS and getaddrinfo apitests tell a machine without a network adapter or Internet name server from a defect ([267f2b34729](https://github.com/nitroos-org/reactos/commit/267f2b34729))
- Stable IPv6 identifier secret: no zeros on RDRAND failure, strict key DACL, a secret made during setup replaced at the first normal boot ([bf6746eac29](https://github.com/nitroos-org/reactos/commit/bf6746eac29))
- DnsQuery answers CNAME, NS, SOA, MX, TXT and SRV; nslookup prints them; no binding leak per query ([8e99e37e3c4](https://github.com/nitroos-org/reactos/commit/8e99e37e3c4))
- WinHTTP and WinINet IPv6 follow-ups: bracketed non-IPv6 hosts fail 12005, <local> and Host header rules for IPv6 literals, WinINet connect timeout per address ([494c39d41ae](https://github.com/nitroos-org/reactos/commit/494c39d41ae))
- RFC 6724 destination ordering with the Windows prefix policy table: CreateSortedAddressPairs, getaddrinfo, SIO_ADDRESS_LIST_SORT ([477d972e718](https://github.com/nitroos-org/reactos/commit/477d972e718))
- Reverse lookup of IPv6 addresses (ip6.arpa), hosts-file reverse rows; adns ends on compression pointer cycles and on truncated TCP answers ([048bfa5942c](https://github.com/nitroos-org/reactos/commit/048bfa5942c))
- IPv6 duplicate address detection conflicts reported like Windows ([2500c8d1e8e](https://github.com/nitroos-org/reactos/commit/2500c8d1e8e))
- Stateless DHCPv6 client: DNS servers and search list on the O flag, DUID and IAID, dhcpcsvc6.dll ([e2620850972](https://github.com/nitroos-org/reactos/commit/e2620850972))
- IPv6 in route print, netsh interface ipv6 show and a new pathping ([f047b65f496](https://github.com/nitroos-org/reactos/commit/f047b65f496))
- IPv6 router advertisement extras: DNS search lists, route information and router preference, hop limit, M/O flags, deprecation, larger neighbour tables ([f501b829af8](https://github.com/nitroos-org/reactos/commit/f501b829af8))
- A listener admits no more connections than its backlog; a thread in accept() is released (WSAEINTR, as Windows) when its listening socket is closed; a send cancelled before it was queued completes for its caller ([8acd77f76f5](https://github.com/nitroos-org/reactos/commit/8acd77f76f5))
- Names in the hosts file resolve as on Windows: every name and row of a line, edits seen without a reboot, hosts rows shadow DNS, a hosts name is not asked from a DNS server ([3c6df91bfc9](https://github.com/nitroos-org/reactos/commit/3c6df91bfc9))
- The port proxy relays v4tov6, v6tov4 and v6tov6 as well as v4tov4 ([2856b616794](https://github.com/nitroos-org/reactos/commit/2856b616794))
- PowerShell NetTCPIP / DnsClient cmdlets handle IPv6 (addresses, routes, DNS servers, persistence, Test-NetConnection details) ([676a445aca9](https://github.com/nitroos-org/reactos/commit/676a445aca9))
- WinHTTP and WinINet connect to IPv6 literal URLs ([addr], zone ids) and fall back to the next resolved address; a bracketed host is an IPv6 address only ([50999099ef8](https://github.com/nitroos-org/reactos/commit/50999099ef8))
- ICMPv6 echo request rules (File and Printer Sharing, Core Networking Diagnostics) in the default firewall policy, added once to existing installs ([6983396bd92](https://github.com/nitroos-org/reactos/commit/6983396bd92))
- IPv6 configuration: a kernel IPv6 route table, static and persistent IPv6 addresses and routes, and the IP Helper create/set/delete calls for IPv6 ([b49952803ca](https://github.com/nitroos-org/reactos/commit/b49952803ca))
- IPv6 MLD listener reports (solicited-node and socket groups) without an idle wake-up, so multicast-snooping switches keep delivering neighbour discovery and group traffic ([e171b52a579](https://github.com/nitroos-org/reactos/commit/e171b52a579))
- Name lookups return every address of a name with several A records (gethostbyname, getaddrinfo, DnsQuery, WSALookupService), results over 512 bytes are retried with the size needed; nslookup prints every address ([737067c2585](https://github.com/nitroos-org/reactos/commit/737067c2585))
- IPv6 in the IP Helper tables: neighbours, routes, best route, TCP6 table, statistics and change notifications answer for IPv6 (and the IPv4 *Table2 calls work) ([962cb4804fc](https://github.com/nitroos-org/reactos/commit/962cb4804fc))
- The network list reports IPv6 connectivity (an IPv6-only host is connected, with IPv6 Internet behind a router) ([875f950976e](https://github.com/nitroos-org/reactos/commit/875f950976e))
- IPv6 is on by default (Tcpip\Parameters\Ipv6Enable = 0 turns it off), after the full gate and a clean soak ([a168611150c](https://github.com/nitroos-org/reactos/commit/a168611150c))
- IPv6 link-local and SLAAC addresses use stable random interface identifiers (RFC 7217) instead of the MAC, as Windows does; Tcpip6 RandomizeIdentifiers=0 restores EUI-64 ([80c6231bbc3](https://github.com/nitroos-org/reactos/commit/80c6231bbc3))
- DnsQuery_A/UTF8 convert every record type by deep copy and free the wide result (no leak, no wrong-size allocation); 32-bit clients get local data lengths; nslookup reads only inside the received packet ([8bc86d0c743](https://github.com/nitroos-org/reactos/commit/8bc86d0c743))
- Pcnet initialises its adapter lock before the interrupt is registered (bugcheck 0x10 when the first interrupt's DPC ran on another processor) ([86176d29264](https://github.com/nitroos-org/reactos/commit/86176d29264))
- Ws2_32 getaddrinfo test expects ::1 first for localhost when IPv6 sockets are on, as Windows answers ([be7e8974a65](https://github.com/nitroos-org/reactos/commit/be7e8974a65))
- Firewall Allow rules with a condition the kernel cannot match (service, services only, connection security, authorised users or machines, owner, package, interface) are rejected and reported instead of pushed broader ([f77d941f2da](https://github.com/nitroos-org/reactos/commit/f77d941f2da))
- The IPv6 catalog upgrade tells the MSAFD entries apart by family and protocol; ipv6tcp fails fast without IPv6 catalog entries ([4d7c17e690e](https://github.com/nitroos-org/reactos/commit/4d7c17e690e))
- DNS over TCP fallback sends the query, not uninitialised stack bytes; adns retries at the server that truncated and skips failing ones; dnsapi and nslookup bound what they read and write ([250d0d3a35f](https://github.com/nitroos-org/reactos/commit/250d0d3a35f))
- AddIPAddress adds an address beside the primary one instead of replacing it; DeleteIPAddress, the unicast row API and the TCP/IP property sheet handle several addresses per interface ([4ed2061c1d2](https://github.com/nitroos-org/reactos/commit/4ed2061c1d2))
- IPv6 M2 follow-ups: ICMPv6 echo cap and bucketed match, WoW64 async echo alignment, TCP tables list every connection end (fixes a tcpip crash under table queries during connection churn) ([3ccf2a488c2](https://github.com/nitroos-org/reactos/commit/3ccf2a488c2))
- Firewall rule push: long lists split instead of truncated, strict address/prefix/ICMP parsing, per-family scope, Block rules first, unresolvable rules rejected and reported, policy pushed again when the host's addresses change ([50699ddbd82](https://github.com/nitroos-org/reactos/commit/50699ddbd82))
- IPv6 default-ON prerequisites: unicast check for miniports whose multicast list overwrites the station address; existing installs get the IPv6 transport and catalog entries at logon ([a39e5dd3489](https://github.com/nitroos-org/reactos/commit/a39e5dd3489))
- DNS over IPv6 servers (router advertisement RDNSS and static) and AAAA lookups while the IPv6 switch is on ([54ddc0dc6b3](https://github.com/nitroos-org/reactos/commit/54ddc0dc6b3))
- ICMPv6 echo (ping -6), ipconfig and netstat IPv6 output, SIO_ADDRESS_LIST_QUERY for IPv6 sockets (with the IPv6 switch on) ([0ff5adda6c2](https://github.com/nitroos-org/reactos/commit/0ff5adda6c2))
- IPv6 UDP sockets end to end while the IPv6 switch is on (off by default): checksums, multicast, packet info and hop limit, UDP6 tables ([c0d1a8089aa](https://github.com/nitroos-org/reactos/commit/c0d1a8089aa))
- The IPv6 multicast list survives an adapter reset (retried at reset end and on a tick, bounded, then all multicast); e1000 receive address write order; IPv6 MTU follows a link MTU increase ([fd2941a0ddd](https://github.com/nitroos-org/reactos/commit/fd2941a0ddd))
- Network pool leaks: AFD frees unread datagrams on close, tcpip and NDIS free what an adapter unbind left behind ([71c5d11530e](https://github.com/nitroos-org/reactos/commit/71c5d11530e))
- IPv6 Winsock follow-ups (a denied impersonating thread does not pin the process to IPv6 off; catalog refusal order; IPV6_DONTFRAG) ([10c25034321](https://github.com/nitroos-org/reactos/commit/10c25034321))
- IPv6 follow-ups: the firewall prompt sees IPv6 and dual-stack listeners, a Packet Too Big never raises the path MTU, the Winsock helper asks again after a failed IPv6 query ([eed027ac7e6](https://github.com/nitroos-org/reactos/commit/eed027ac7e6))
- IPv6 TCP sockets end to end while the IPv6 switch is on (off by default); fragment reassembly takes the protocol of the first fragment ([b3e813127ac](https://github.com/nitroos-org/reactos/commit/b3e813127ac))
- IPv6 firewall follow-ups (the rule service asks the running tcpip about IPv6; fragmented ND/MLD and nested Fragment headers dropped; echo replies judged) ([e7c0edb668b](https://github.com/nitroos-org/reactos/commit/e7c0edb668b))
- IPv6 reporting follow-ups (canonical name without a ::1 entry, GetAdaptersAddresses size for large answers, table query entity check) ([7a5724fbd62](https://github.com/nitroos-org/reactos/commit/7a5724fbd62))
- Fragmented TCP segments are answered again (IPv4 reassembly hands lwIP a whole datagram; a TCP header split across fragments is joined) ([f87c84dd841](https://github.com/nitroos-org/reactos/commit/f87c84dd841))
- IPv6 inbound firewall subset (unsolicited SYN, UDP and ICMPv6 judged by the rules; ND/MLD checks; ICMPv6 error rate limit) while the IPv6 switch is on (off by default) ([22ea808f760](https://github.com/nitroos-org/reactos/commit/22ea808f760))
- Iphlpapi and getaddrinfo report IPv6 addresses and interfaces while the IPv6 switch is on (off by default) ([d29ce27c106](https://github.com/nitroos-org/reactos/commit/d29ce27c106))
- IPv6 receives its multicast groups (all-nodes, solicited-node) with the switch on; e1000, rtl8139 and pcnet program multicast lists correctly ([ccf8048e64c](https://github.com/nitroos-org/reactos/commit/ccf8048e64c))
- AFD allocates the send window only for a send on a connected socket (a send before the connect no longer trips the connect) ([f45e90a0f1d](https://github.com/nitroos-org/reactos/commit/f45e90a0f1d))
- IPv6 in tcpip behind Tcpip\Parameters\Ipv6Enable (default off): lwIP IPv6, link-local with DAD, ::1, IPv6 receive, no inbound wire connections yet ([95a273a8275](https://github.com/nitroos-org/reactos/commit/95a273a8275))
- Legacy firewall objects (INetFwMgr, authorized applications, open ports) make real rules; advfirewall export, import, dump, set global; netsh fixes ([591d640f458](https://github.com/nitroos-org/reactos/commit/591d640f458))
- Loopback TCP throughput (lwIP block reuse, nonpaged AFD receive windows within a quota and a global budget, I/O completion unlocks only locked MDLs, loopback ACK) ([127e2792f62](https://github.com/nitroos-org/reactos/commit/127e2792f62))
- Getaddrinfo answers ::1 for "localhost" with AF_INET6, as Windows 10 does (AF_UNSPEC unchanged, IPv4 only) ([a6bcd65802f](https://github.com/nitroos-org/reactos/commit/a6bcd65802f))
- An IRP completed by its own thread at PASSIVE_LEVEL runs its completion inline instead of through a self-IPI APC; AFD captures small ioctl inputs without an MDL; MSAFD reads the send window once per process (loopback ... ([068768953fe](https://github.com/nitroos-org/reactos/commit/068768953fe))
- Getaddrinfo answers :: (passive) or ::1 for AF_INET6 without a node name, as Windows 10 does, so servers with an optional IPv6 wildcard bind (Redis default config) start on IPv4 ([f22bf675668](https://github.com/nitroos-org/reactos/commit/f22bf675668))
- NetUserEnum, NetGroupEnum and NetLocalGroupEnum return all entries in one buffer, page by prefmaxlen with resume handles, and NetGroupGetUsers no longer reads past the member list on a resumed call ([6d737a7ef13](https://github.com/nitroos-org/reactos/commit/6d737a7ef13))
- Net start passes the arguments typed after the service name, not the name itself (Oracle's TNS listener service starts) ([ff156ef05e0](https://github.com/nitroos-org/reactos/commit/ff156ef05e0))
- TCP tables list TIME_WAIT connections; ApiInformation uses an expanded DllPath only when it fits; vectored handler walk ordering documented; FaultContext suspender runs above the faulting threads ([7fe059e7042](https://github.com/nitroos-org/reactos/commit/7fe059e7042))
- Loopback TCP connect/accept/close 20% cheaper (lwIP requests run on the calling thread under the core lock; msafd stops asking AFD for a stream socket's receive window) ([2c37786394d](https://github.com/nitroos-org/reactos/commit/2c37786394d))
- LwIP 2.2.1 (official release, 3-way merge keeping every local change) ([953e589c4f4](https://github.com/nitroos-org/reactos/commit/953e589c4f4))
- Tcpip measures a lent receive packet from its buffers (stale cached length dropped TCP segments), and dc21x4 recalculates its packet counts after resizing ([b81187fdbad](https://github.com/nitroos-org/reactos/commit/b81187fdbad))
- WSASend with a completion routine ignores the OVERLAPPED event field, and recv on an unconnected stream socket is WSAENOTCONN, as on Windows (Oracle XE listener answers over TCP) ([2a761851bdb](https://github.com/nitroos-org/reactos/commit/2a761851bdb))
- Rtl8139 restarts sends, keeps interrupt bits and completes sends when the chip is done; dc21x4 copies received frames (both usable again: 18-33 MB/s on QEMU instead of stalls) ([599689d4dab](https://github.com/nitroos-org/reactos/commit/599689d4dab))
- An outgoing connect picks an ephemeral port per remote, so ports in TIME-WAIT towards one remote are reused for others as on Windows; a full range gives WSAEADDRINUSE ([7a9470ac8ac](https://github.com/nitroos-org/reactos/commit/7a9470ac8ac))
- MSI password edits are masked; iphlpapi gains the Vista mask-length, TCP table 2 and unicast-address exports; wsnmp32.dll (Wine 11.21) is added (Oracle XE 21c setup now installs its software) ([fd4a8aeb4f0](https://github.com/nitroos-org/reactos/commit/fd4a8aeb4f0))
- TDI completions run without a thread wake on the lock-free loopback worker and in batches elsewhere; AFD send window 64 KB as on Windows; lowering SO_RCVBUF no longer cuts the window below data in flight (local TCP +24%) ([dd1d8d95050](https://github.com/nitroos-org/reactos/commit/dd1d8d95050))
- A refused non-blocking connect notifies as on Windows (FD_CONNECT carries the error once, WSAPoll reports POLLHUP|POLLWRNORM|POLLERR); no duplicate FD_CONNECT/FD_WRITE ([024a1da5204](https://github.com/nitroos-org/reactos/commit/024a1da5204))
- The MSI custom action server starts in the system directory, so installers that start programs by bare name work (Oracle XE port check); netstat prints Windows' column layout ([a83404c6a01](https://github.com/nitroos-org/reactos/commit/a83404c6a01))
- A listener refuses the connections its backlog has no room for before answering them, as Windows does, instead of completing and then resetting them ([149a1c57246](https://github.com/nitroos-org/reactos/commit/149a1c57246))
- NDIS hands queued sends to a serialized SendPackets miniport in one call, and the e1000 rings its doorbells less often (HTTP download from the host +13%) ([252f5d3e451](https://github.com/nitroos-org/reactos/commit/252f5d3e451))
- TCP segments to the loopback interface go straight back to lwIP with one copy, without NDIS packets or the IP receive path (switch NitroLoopbackFastPath) ([31a5e5d9312](https://github.com/nitroos-org/reactos/commit/31a5e5d9312))
- --datagram: 32-bit bind matrix and UDP loopback exchange ([7def29e5d09](https://github.com/nitroos-org/reactos/commit/7def29e5d09))
- LwIP finds TIME-WAIT connections through a 4-tuple hash and the ephemeral-port search no longer walks them (connect rate stays flat in a connect loop) ([46d37311062](https://github.com/nitroos-org/reactos/commit/46d37311062))
- E1000 receive buffer and descriptor rings in cached memory (HTTP download 55 -> 91 MB/s) ([01984484948](https://github.com/nitroos-org/reactos/commit/01984484948))
- Loopback TCP uses a large MSS (127/8 only; loopback 79 -> 226 MB/s); e1000 reports an IP MTU of 1500 (was 1508); lwIP sanity walks only in debug builds ([b3994575260](https://github.com/nitroos-org/reactos/commit/b3994575260))
- Rpcrt4 interface security callback and RPC_IF_ALLOW_LOCAL_ONLY as measured on Windows 10 (callback gets the interface syntax identifier, TCP is never local, unauthenticated np/TCP callers are re-checked on every call) ([f68817a4883](https://github.com/nitroos-org/reactos/commit/f68817a4883))
- Security fix (described generically): hardening in this area; details withheld until users have had time to update ([cd3a069db4b](https://github.com/nitroos-org/reactos/commit/cd3a069db4b))
- (part 2 of 2): as on Windows, advapi32 imports rpcrt4 and rpcrt4 delay-loads ws2_32, so a console process importing advapi32 maps no socket or window DLL (x64 14 -> 8 modules); x86 guests now load rpcrt4 like Windows (was ... ([2619918a91b](https://github.com/nitroos-org/reactos/commit/2619918a91b))

</details>

<details><summary>Storage/USB/PnP/drivers (3 fixes)</summary>

- SetupDiGetClassBitmapIndex implemented (the stub killed its caller); TVM_GETITEMSTATE returns the state image bits with TVS_CHECKBOXES as Windows ([76bbf769042](https://github.com/nitroos-org/reactos/commit/76bbf769042))
- RegisterDeviceNotification works from 32-bit processes (fixed-width PnP RPC field), interface lists sized in characters, tab insertion without a selection does not select (WinSCP starts) ([55568b3324d](https://github.com/nitroos-org/reactos/commit/55568b3324d))
- Atapi stops using NCQ on a device after an NCQ error recovery that cannot identify the failed command, so a persistently failing request is counted and reported (8 s, 10 attempts) instead of being requeued forever (AHCI guest ... ([8b469758a44](https://github.com/nitroos-org/reactos/commit/8b469758a44))

</details>

<details><summary>Shell/explorer/common-controls (3 fixes)</summary>

- Comctl32 commctrl.c to UTF-8 and without the old DllGetVersion ([db398f7701e](https://github.com/nitroos-org/reactos/commit/db398f7701e))
- The classic Start menu shows no '(Empty)' placeholder or divider when nothing is pinned above Programs; Task Manager counts system handles again ([3c46dcf7b34](https://github.com/nitroos-org/reactos/commit/3c46dcf7b34))
- Help and Support, bug-report and update links point to NitroOS (one place in version.cmake); ReactOS credits unchanged ([2d339130d0a](https://github.com/nitroos-org/reactos/commit/2d339130d0a))

</details>

<details><summary>Setup/boot (4 fixes)</summary>

- Application, System and Security event logs default to 20 MB, overwrite as needed, as on Windows 10 (writers no longer fail with ERROR_EVENTLOG_FILE_FULL after 512 KB) ([ea83cf0982a](https://github.com/nitroos-org/reactos/commit/ea83cf0982a))
- SMSS caps cross-session forwards in flight so a stuck target csrss cannot wedge it, and length-checks the one-way message (sessions flag 5) ([fc535fcc550](https://github.com/nitroos-org/reactos/commit/fc535fcc550))
- A process created into another session is registered with that session's csrss through SMSS (NitroSessions 5, off by default) ([ec0aa1a35cf](https://github.com/nitroos-org/reactos/commit/ec0aa1a35cf))
- "based on ReactOS" instead of "unofficial build based on ReactOS" in the watermark, About and setup notices (operator 2026-11-09) ([0daec3d99c6](https://github.com/nitroos-org/reactos/commit/0daec3d99c6))

</details>

<details><summary>Services/security/profiles (14 fixes)</summary>

- Decide under the database lock which stop ends a shared process's dispatcher; a start after that gets a new process ([4937ac317ee](https://github.com/nitroos-org/reactos/commit/4937ac317ee))
- Service stops: srvsvc, wkssvc, browser and dnsrslvr wait for calls in flight on a worker, not on the control thread ([241df440565](https://github.com/nitroos-org/reactos/commit/241df440565))
- The SAM user query fix: no lsass heap block leaked per full user query ([c5c4e73b600](https://github.com/nitroos-org/reactos/commit/c5c4e73b600))
- RPC server fixes: explicit generic handles get their own memory, shared-process services unregister only their own interfaces, svchost ends with its last service ([f71844a1e73](https://github.com/nitroos-org/reactos/commit/f71844a1e73))
- Rpcrt4 calls the unbind routine of an explicit generic handle with the bound handle (no binding leaked per call); eventlog backup no longer closes a stack value ([b1d99923492](https://github.com/nitroos-org/reactos/commit/b1d99923492))
- Callers of the system random number generator fail cleanly instead of using uninitialised bytes or spinning when it refuses ([36a8a1b5bf8](https://github.com/nitroos-org/reactos/commit/36a8a1b5bf8))
- A cryptographic system random number generator: ksecdd ChaCha20 generator seeded from RDSEED/RDRAND and jitter, RtlGenRandom and BCryptGenRandom on top of it ([6e7a7fe95f6](https://github.com/nitroos-org/reactos/commit/6e7a7fe95f6))
- RPC connections bound at the same time share one association group (concurrent LSA calls and same-machine NTLM exchanges no longer fail with a context mismatch); an expired local NTLM context reports SEC_E_CONTEXT_EXPIRED as on ... ([07643f53484](https://github.com/nitroos-org/reactos/commit/07643f53484))
- Same-machine NTLM with the default credentials is a local call, as on Windows: LSA-issued context bound to the client's own token, accepted only by the issuing process; bounded per user, freed when the issuer exits or the context ... ([d4bbeae905e](https://github.com/nitroos-org/reactos/commit/d4bbeae905e))
- Any caller may connect to the LSA authentication port (LsaConnectUntrusted, as on Windows); the Negotiate package passes QuerySecurityContextToken and impersonation to NTLM (Oracle XE OS authentication works) ([4cbb5e09aa2](https://github.com/nitroos-org/reactos/commit/4cbb5e09aa2))
- Crypt32 and the Cryptography key are opened with the access the operation needs (read-only store and chain opens, write only when writing; no HKLM write opens by a standard user's TLS) ([206bee50fbd](https://github.com/nitroos-org/reactos/commit/206bee50fbd))
- The password-less logon of LocalService and NetworkService is for a trusted caller only (access denied otherwise, as Windows 10) ([9798fcbb6e3](https://github.com/nitroos-org/reactos/commit/9798fcbb6e3))
- Service virtual accounts (NT SERVICE\<service> SIDs, lookups, service logon for trusted callers only), kernel-object GetNamedSecurityInfo, whole-SID local group members, CVssWriter exports; LSA no longer believes a client's own ... ([95c15860b6c](https://github.com/nitroos-org/reactos/commit/95c15860b6c))
- CertCreateCertificateChainEngine accepts the current SDK config size and WSAIoctl answers SIO_BASE_HANDLE, so rustup and other rustls/tokio programs work ([70707e41812](https://github.com/nitroos-org/reactos/commit/70707e41812))

</details>

<details><summary>User-mode system DLLs (52 fixes)</summary>

- CreateProcessAsUser hands the token to kernel32 only for another session, so a same-session call no longer needs SeTcbPrivilege (fails with 1314 before); LsassLeak test measures after the heap warm-up ([06d8f643619](https://github.com/nitroos-org/reactos/commit/06d8f643619))
- The FindFirstFile FILE_SHARE_DELETE fix (Oracle XE setup.exe first run) ([46e5bf885bd](https://github.com/nitroos-org/reactos/commit/46e5bf885bd))
- Sppc answers ProductKeyGroupUniqueness and pkeyConfigLicenseId of a SKU from its end-user license ([15b2e41c5f3](https://github.com/nitroos-org/reactos/commit/15b2e41c5f3))
- OnlineId GetTicketAsync without an online identity answers ServiceConnectionError like Windows offline ([283783116ce](https://github.com/nitroos-org/reactos/commit/283783116ce))
- RtlpExtendHeap no longer returns a free block shorter than the request ([0a594940226](https://github.com/nitroos-org/reactos/commit/0a594940226))
- The loader initialises a module that is mapped but not yet initialised before LoadLibrary, GetProcAddress or a new importer use it, and a failure there fails the outer load, as on Windows (Excel starts) ([c9ed39c0dfa](https://github.com/nitroos-org/reactos/commit/c9ed39c0dfa))
- Heap front end gives back parked blocks nobody takes, before the heap grows and in time ([31c549a2776](https://github.com/nitroos-org/reactos/commit/31c549a2776))
- Heap segments stop doubling at 16192 KB, a churning heap gives back stranded runs instead of reserving new segments, 64-bit heaps still grow past 4.9 GB ([f461562ba56](https://github.com/nitroos-org/reactos/commit/f461562ba56))
- ApiInformation treats a class whose DllPath does not expand within MAX_PATH as not registered ([bd0492e0764](https://github.com/nitroos-org/reactos/commit/bd0492e0764))
- Rpcrt4 client reuses a pooled connection for a new binding handle only for the same token state (logon id + ModifiedId); identities no longer cross between overlapping handles ([5e0796e3585](https://github.com/nitroos-org/reactos/commit/5e0796e3585))
- HeapCompact implemented; a failed grow in place is counted once; the free path finds the commit cap only for a block it can decommit ([c620a3befc5](https://github.com/nitroos-org/reactos/commit/c620a3befc5))
- Heap small-block front end at the Windows 10 thresholds, shared freed blocks, counters outside the block space ([935d44d6140](https://github.com/nitroos-org/reactos/commit/935d44d6140))
- Kk-KZ has no default ANSI code page, as on Windows 10; the FontNullLifetime test yields to the main thread (it looked like a guest hang) ([7c4a3a30021](https://github.com/nitroos-org/reactos/commit/7c4a3a30021))
- Rometadata.dll with MetaDataGetDispenser (E_NOTIMPL), so the Windows App SDK runtime loads (PowerToys no longer stops on a missing import) ([66024876eef](https://github.com/nitroos-org/reactos/commit/66024876eef))
- The heap extends into an uncommitted range together with the free blocks around it, and never returns an entry shorter than the request (csrss no longer strands a page per ~86 consoles; session-1 growth 34 -> 0.68 B per console ... ([213d6e7b17d](https://github.com/nitroos-org/reactos/commit/213d6e7b17d))
- LOCALE_FONTSIGNATURE of all 231 locales as measured on Windows 10 ([55b824d1759](https://github.com/nitroos-org/reactos/commit/55b824d1759))
- Derived code pages, abbreviations and OpenType tags of the 85 round-5 locales as measured on Windows 10 ([03daa065069](https://github.com/nitroos-org/reactos/commit/03daa065069))
- I386 builds for DLL export version 0x502 build, install and boot again (version-gated modules and tests, vista import libs below 0x600) ([2b04779d20c](https://github.com/nitroos-org/reactos/commit/2b04779d20c))
- A 32-bit image that opts in to DEP (/NXCOMPAT) runs with DEP under the default OptIn policy, as on Windows; FaultContext checks the exception record of read, write and execute access violations ([5350833b17e](https://github.com/nitroos-org/reactos/commit/5350833b17e))
- User-mode heaps keep free pages as a bounded cache (a quarter of the committed size) and trim it after a quiet second, instead of decommitting and recommitting on every churn (Word shader draws -19%, heap churn 2.5x faster, idle ... ([2d62d6a4716](https://github.com/nitroos-org/reactos/commit/2d62d6a4716))
- I386 thread start: a routine that returns with EBP, EBX, ESI or EDI replaced (the Go runtime's) ends its thread instead of crashing ([0be54783394](https://github.com/nitroos-org/reactos/commit/0be54783394))
- A vectored-handler walk that never returns no longer pins removed handlers for the process lifetime (per-thread slots claimed and released lock-free; nothing on a walk or at thread exit waits for the lock) ([731eec682af](https://github.com/nitroos-org/reactos/commit/731eec682af))
- Dcomp presentation never makes the app wait (Commit records a frame and returns; a dcomp render thread reads back only changed surfaces and presents; window hooks never block) ([3df74759364](https://github.com/nitroos-org/reactos/commit/3df74759364))
- Time zone display names in the (UTC...) style for all zones; Dynamic DST from IANA tzdata for British Columbia, Alberta, Manitoba, Paraguay and Morocco ([48f0fed4ec8](https://github.com/nitroos-org/reactos/commit/48f0fed4ec8))
- An unloaded DLL's data table entry frees its base name (every LoadLibrary/FreeLibrary cycle leaked it) ([92b36a3a4ca](https://github.com/nitroos-org/reactos/commit/92b36a3a4ca))
- Dcomp draws a DesktopWindowTarget's visual tree into its window on commit (Word shows its Start screen); Compositor.CreatePropertySet; ICompositionGraphicsDeviceInterop ([bad883cee46](https://github.com/nitroos-org/reactos/commit/bad883cee46))
- Dcomp ExpressionAnimation, CompositionPropertySet and Interactions.InteractionTracker (stored state; one of Word's exit paths) ([aa621c6dc49](https://github.com/nitroos-org/reactos/commit/aa621c6dc49))
- Amd64 RtlCreateUserThread starts the thread in RtlUserThreadStart (a routine that returns ends the thread); DebugApi checks the debug event file handles ([e627738e521](https://github.com/nitroos-org/reactos/commit/e627738e521))
- A thread's old TLS vector lives until the thread ends, also for the thread that loads a TLS DLL (Oracle XE database creation no longer crashes in its OLAP callout) ([a028d68a621](https://github.com/nitroos-org/reactos/commit/a028d68a621))
- Heap fixes: whole-heap HeapValidate passes on a consistent heap, a fixed-size heap over 1 MB keeps all its free space (no more hang), the failed-descriptor decommit path re-inserts the right size; kernel32 heap winetest runs on ... ([9949688d6db](https://github.com/nitroos-org/reactos/commit/9949688d6db))
- GetThreadGroupAffinity fails with ERROR_INVALID_PARAMETER for a NULL output instead of writing through it (kernel32 thread winetest no longer crashes) ([28f65049dd7](https://github.com/nitroos-org/reactos/commit/28f65049dd7))
- Dcomp accepts a Direct2D-only rendering device (Office passes one) and answers the real ICompositionDrawingSurfaceInterop ([b54d43f1197](https://github.com/nitroos-org/reactos/commit/b54d43f1197))
- Exceptions scale across threads (lock-free vectored handler walk, fault with no VAD answers before the address-space lock, per-CPU xstate spare, cached pointer cookie) and the .NET NullReferenceException path no longer fails an ... ([2bc858ec618](https://github.com/nitroos-org/reactos/commit/2bc858ec618))
- A removed vectored handler is never called or freed under a running walk (second remove returns 0); the module range table recovers after overflow; KD exception-report decision shared ([19981715866](https://github.com/nitroos-org/reactos/commit/19981715866))
- Windows.UI.Composition CompositionNineGridBrush (interface ids and order from microsoft/windows-rs); Word now reaches its splash window ([84c8fd2a1e1](https://github.com/nitroos-org/reactos/commit/84c8fd2a1e1))
- First-chance exceptions 1.4-2.5x faster (XState buffers from a lookaside, debugger paging only when KD stops, vectored handlers under a shared lock with refcounts, module lookup without the loader lock) ([46379529d7d](https://github.com/nitroos-org/reactos/commit/46379529d7d))
- Windows.UI.Composition.Interactions.VisualInteractionSource (registered to dcomp as on Windows), IVisual2, ICompositor2, ICompositionSurfaceBrush3 (interface ids and order from microsoft/windows-rs) ([710c3bc5aa6](https://github.com/nitroos-org/reactos/commit/710c3bc5aa6))
- 85 more locales from CLDR 49.0.0 with MS-LCID identities (231 locales; .NET sees 233 cultures), GetDateFormat genitive-month hang fixed, i386 base addresses of kernel32 and the DLLs below it moved down ([a47cda4de1a](https://github.com/nitroos-org/reactos/commit/a47cda4de1a))
- Amd64 RtlVirtualUnwind raises STATUS_BAD_FUNCTION_TABLE for undefined unwind ops (as Windows 10) instead of looping, slot table covers all 16 ops ([daf5be7dd05](https://github.com/nitroos-org/reactos/commit/daf5be7dd05))
- Windows.UI.Composition CompositorController, ICompositorDesktopInterop and DesktopWindowTarget (interface ids and member order from microsoft/windows-rs, MIT/Apache-2.0) ([c4d3fec03b1](https://github.com/nitroos-org/reactos/commit/c4d3fec03b1))
- CreateDispatcherQueueController on the current thread accepts every apartment option, as measured on Windows 10 ([a6674cc0275](https://github.com/nitroos-org/reactos/commit/a6674cc0275))
- Cross-session create retries a stale session pick, fails closed on a NULL session, and never returns attached to the child ([e0536a78320](https://github.com/nitroos-org/reactos/commit/e0536a78320))
- Mscms implements the Wcs default-profile queries (as measured on Windows) and ships a CC0 sRGB profile generated from the published standards (GIMP starts) ([6d681e30482](https://github.com/nitroos-org/reactos/commit/6d681e30482))
- Unmanifested programs can be told Windows 8 (6.2) by GetVersion(Ex)/VerifyVersionInfo as on Windows 10, behind a switch that is off by default (UnmanifestedVersionLie) ([0e4f7c59a0a](https://github.com/nitroos-org/reactos/commit/0e4f7c59a0a))
- Locale tables from CLDR round 4: quoted words in long dates and the Swiss group separator closer to Windows' forms (no field worse) ([1f00303a6fc](https://github.com/nitroos-org/reactos/commit/1f00303a6fc))
- Follow-ups: a session is looked up only among complete processes and a session that went away is access denied; kernel32 returns a Safer token only when asked ([79b25140274](https://github.com/nitroos-org/reactos/commit/79b25140274))
- QueueUserWorkItem keeps items in a list and wakes workers only when needed (3.1x faster); x64 NtQuerySystemTime is a short assembly stub (1.9x faster) ([f21748b69aa](https://github.com/nitroos-org/reactos/commit/f21748b69aa))
- Windows.Foundation.PropertyValue activation factory in wintypes (boxed values and arrays as on Windows 10); Word gets past its PropertyValue abort ([2903dbaedc6](https://github.com/nitroos-org/reactos/commit/2903dbaedc6))
- Heap front end serves size-class blocks (a freed block is found by the class requests look in), may take the next class up, 64 slots with 512 KB each, and a growing heap no longer flushes the cache; heapmt 4x250k x64 235 -> 11 ... ([e6d44cff918](https://github.com/nitroos-org/reactos/commit/e6d44cff918))
- (part 1): NtQuerySystemTime reads the shared user data page in ntdll instead of entering the kernel, as on Windows (2M calls: x64 155 -> 7 ms, x86 95 -> 8 ms) ([2ae5c660101](https://github.com/nitroos-org/reactos/commit/2ae5c660101))
- (condvar): condition variables use the default keyed event (NULL handle, as Windows), so a wake or wait from a program's code running inside the loader before ntdll's init no longer asserts in Debug builds; NtKeyedEvent apitest ... ([f8b6c2cef6a](https://github.com/nitroos-org/reactos/commit/f8b6c2cef6a))
- Round 3 CLDR mapping rules (Latin-numbering symbols, CSV list separator, localized AM/PM kept, currency sign position, fullwidth forms, script in multi-script language names, CJK console fallback, Um al-Qura default for SA ... ([91f1273622b](https://github.com/nitroos-org/reactos/commit/91f1273622b))

</details>

<details><summary>Browser/COM/Wine imports (2 fixes)</summary>

- Exe custom action launch failure fails the install (MSI-CA-EXE-LAUNCH-FAILURE) ([ae72a632b6a](https://github.com/nitroos-org/reactos/commit/ae72a632b6a))
- An administrative MSI install copies every feature whatever its Level and drops embedded cabinets from the image, as Windows does (scoop install mongodb) ([164f04178bd](https://github.com/nitroos-org/reactos/commit/164f04178bd))

</details>

<details><summary>Applications/tools (10 fixes)</summary>

- Featmgr does not install the old hosted lavapipe pack over newer in-tree Mesa drivers (already newer, skipped) ([95445921ead](https://github.com/nitroos-org/reactos/commit/95445921ead))
- Featmgr names the existing install an installer updated instead of 'success but not installed' ([564e988991f](https://github.com/nitroos-org/reactos/commit/564e988991f))
- The certificate seeding service skips the store opens when the built-in roots have not changed since the last seed ([b436d112f01](https://github.com/nitroos-org/reactos/commit/b436d112f01))
- The machine Root and AuthRoot certificate stores are populated as SYSTEM at setup and at boot (a standard user never needs to write HKLM SystemCertificates) ([ab20dd4ef32](https://github.com/nitroos-org/reactos/commit/ab20dd4ef32))
- Csrss no longer grows with every drive letter define/remove (the broadcast thread ends through the loader, freeing its TLS vector and activation context stack); churn test DefineDosDevice phase ([9cfa1057867](https://github.com/nitroos-org/reactos/commit/9cfa1057867))
- Featmgr does not follow junctions or symlinks under Scoop persist when it writes app defaults; JSONC brackets must match, BOM-less UTF-16 refused ([47c674dfca8](https://github.com/nitroos-org/reactos/commit/47c674dfca8))
- Answer-file key [Unattend] AcknowledgeAddonNotices: unattended setups accept the first-boot add-on notices (AAC), interactive setups still show them once ([d3d11d92f35](https://github.com/nitroos-org/reactos/commit/d3d11d92f35))
- Optional Features gets an icon, the Claude Desktop entry is removed, and first boot installs the full AAC decoder with its patent notice shown once ([a0575d01fe0](https://github.com/nitroos-org/reactos/commit/a0575d01fe0))
- Featmgr reports a component as installed only when its detect rule holds after the installer exits 0 ([da54a44e4ef](https://github.com/nitroos-org/reactos/commit/da54a44e4ef))
- Diskpart makes a new extended partition the disk's logical container with its free space, so "create partition logical" works in the same session (it needed a rescan) ([ea6ee4ffe03](https://github.com/nitroos-org/reactos/commit/ea6ee4ffe03))

</details>

<details><summary>Build/toolchain/API surface (12 fixes)</summary>

- The comctl32 of the v6 side-by-side assembly reports 6.16.10586 (DllGetVersion and version resource) as on Windows 10; the v5 comctl32 stays 5.82 ([a157ed6f9cc](https://github.com/nitroos-org/reactos/commit/a157ed6f9cc))
- I386 base address tables with growth room per DLL (kernel32 had 44 KB below ntdll) and a build-time check that fails on any i386 image overlap ([bb486159b04](https://github.com/nitroos-org/reactos/commit/bb486159b04))
- 143 time zones (UTC and 58 more, from CLDR windowsZones + IANA tzdata) with MUI_Display/Std/Dlt strings in a new tzres.dll ([007b457a2b8](https://github.com/nitroos-org/reactos/commit/007b457a2b8))
- SignerSign/SignerSignEx/SignerSignEx2 and the PE SIP (create, verify, remove indirect data, page hashes): Authenticode signing roundtrips ([47d66c5288b](https://github.com/nitroos-org/reactos/commit/47d66c5288b))
- Mscms gains Little CMS 2.20 (MIT, as Wine bundles it): profile objects and colour transforms work ([bb8db1fed8e](https://github.com/nitroos-org/reactos/commit/bb8db1fed8e))
- Reproducible builds (NITRO_REPRODUCIBLE_BUILD / NITRO_STABLE_BUILD_ID), OS-update U1 ([4f9dc8aa3a7](https://github.com/nitroos-org/reactos/commit/4f9dc8aa3a7))
- System DLLs carry the Windows 10 product version, so Java reports Windows 10 / 10.0 ([9a28a56c5c7](https://github.com/nitroos-org/reactos/commit/9a28a56c5c7))
- Every user-mode NitroOS executable declares Windows 10 in its compatibility manifest, with a build lint (no behaviour change; prepares the Windows version lie) ([a26219ba130](https://github.com/nitroos-org/reactos/commit/a26219ba130))
- (weekly upstream sync: reactos/master 8fe3cee0560, 96 commits since e46f4bb1a1e; 13 conflicts resolved per the inner merge message: upstream wins except the fork storport timeouts/restart/serial-number/buffer-mapping/transfer-cap ... ([5bb9f93e7e4](https://github.com/nitroos-org/reactos/commit/5bb9f93e7e4))
- Sppc application authentication data and unlicensed answers ([085f31b9053](https://github.com/nitroos-org/reactos/commit/085f31b9053))
- Sppc/slc answer as Windows 10 does for installed, never-activated Office (measured on a disposable Windows copy): SKUs listed in ascending id order and reported Unlicensed (0xC004F014), key queries 0xC004F012, SKU information ... ([5694a5fea6d](https://github.com/nitroos-org/reactos/commit/5694a5fea6d))
- Sppc/slc export the Software Licensing client names Office resolves (documented ones return the documented no-service errors, undocumented ones SL_E_NOT_SUPPORTED on 64-bit only), SLGetLicensingStatusInformation returns the ... ([b94450f51e0](https://github.com/nitroos-org/reactos/commit/b94450f51e0))

</details>

<details><summary>Tests only (14 fixes)</summary>

- NtFreeVirtualMemory interior-base apitest (Windows 10 rows: first page frees the region, past it STATUS_FREE_VM_NOT_AT_BASE) ([5e41df0f772](https://github.com/nitroos-org/reactos/commit/5e41df0f772))
- Access-check differential tests (sampled matrix with a golden hash by default, the full matrix on request) ([e518167a166](https://github.com/nitroos-org/reactos/commit/e518167a166))
- Advapi32_apitest OpenThreadTokenLeak (no handle or token leak from OpenThreadToken on an impersonating thread) ([fd07b532e10](https://github.com/nitroos-org/reactos/commit/fd07b532e10))
- Kernel32 apitest checks that every locale name of the data resolves (and the Wine-only names Windows rejects stay unresolved) ([d02e15942ac](https://github.com/nitroos-org/reactos/commit/d02e15942ac))
- Fsregrow never deletes a still-running thread's file after a join timeout (the reported verify-open failure was the test), plus a reopen stress test and failure diagnostics ([7534af80902](https://github.com/nitroos-org/reactos/commit/7534af80902))
- Ws2_32 apitest acceptlag (a single loopback client's connects are answered at once in the first seconds after boot) ([6e148c6e82f](https://github.com/nitroos-org/reactos/commit/6e148c6e82f))
- Kernel32 apitest CsrAdoptedThreadChurn measures csrss growth after a warm-up (no per-call growth; one-time commit on i386 Debug) ([17643f6e596](https://github.com/nitroos-org/reactos/commit/17643f6e596))
- Build the d3d11 and d3d10core conformance tests (Wine 10.0) ([78f1dc510a6](https://github.com/nitroos-org/reactos/commit/78f1dc510a6))
- Ntdll apitest NtRemoveIoCompletionTerminate (a waiter terminated while a completion is posted loses nothing, for every packet kind) ([d19e24abc0e](https://github.com/nitroos-org/reactos/commit/d19e24abc0e))
- FaultContext test stops cleanly after a timeout, covers every faulting thread with the suspender and resumes before reporting ([b0594145bfd](https://github.com/nitroos-org/reactos/commit/b0594145bfd))
- Kernel32 apitest CsrAdoptedThreadChurn (CSRSS drops the records of threads it adopted when they end) ([73b537d5be5](https://github.com/nitroos-org/reactos/commit/73b537d5be5))
- Rpcrt4 interface_access refusal rows go through a generated stub and check the status a denied client sees (5, measured on Windows 10) ([533e0e32dea](https://github.com/nitroos-org/reactos/commit/533e0e32dea))
- ZwAllocateVirtualMemory kmtest expects the Windows 10 statuses measured on the oracle ([3f783e7863a](https://github.com/nitroos-org/reactos/commit/3f783e7863a))
- PagerBasic samples its paging-file baseline after the process' own pages went out, and checks the test range's slots both ways ([78ceb4c258a](https://github.com/nitroos-org/reactos/commit/78ceb4c258a))

</details>

### Limitations
New in this release:
- **IPv6, known differences from Windows:** IPv6 neighbour entries (`GetIpNetTable2`, `GetIpNetEntry2`, `ResolveIpNetEntry2`), the IPv6 forwarding table and `GetBestRoute2` for IPv6 destinations, IPv6 change notifications (`NotifyIpInterfaceChange` and similar never fire for AF_INET6), `GetTcp6Table2` (empty; `GetExtendedTcpTable(AF_INET6)` does list IPv6 sockets), the AF_INET6 statistics calls (they return IPv4 counters) and `IPV6_V6ONLY` on an AF_INET socket (accepted; Windows refuses it) answer as a system without IPv6. `IPV6_RECVIF` and `IPV6_RECVTCLASS` are accepted but `WSARecvMsg` does not deliver their control messages yet; `IPV6_UNICAST_IF` is not enforced on sends. This machine receives IPv6 UDP datagrams of at most 65487 bytes (Windows: 65527). With IPv6 on, a third-party Ethernet driver that stores the multicast list in its station-address registers can stop receiving unicast frames; tcpip detects this when an IPv4 router answers and falls back to all-multicast, and `Tcpip\Parameters\Ipv6Enable = 0` is the workaround. IPv6 is on by default; to turn it off: `reg add HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters /v Ipv6Enable /t REG_DWORD /d 0 /f` and restart (the loopback interface keeps IPv6, as on Windows).
- **IPv4 multicast without IGMP:** IPv4 groups are not announced with IGMP, so a switch that snoops IGMP does not forward them to NitroOS (Windows sends IGMPv3). An IPv4 multicast datagram reaches every socket bound to its port that has no filter for the group, joined or not (Windows: only sockets that joined). With `Ipv6Enable = 0`, IPv4 multicast does not arrive.
- **Firewall rules with conditions the filter cannot match:** the filter matches protocol, ports, addresses, ICMP type and code, interface type and program. An Allow rule that also names a service (`service=<name>` or `service=any`), connection security, authorised users or computers, a local user, a package or a specific interface is not enforced, so it lets nothing in where Windows lets the matching traffic in (narrower than Windows, never broader). A Block rule with such a condition is applied without it and so blocks more than written. Both are reported in `HKLM\SOFTWARE\ReactOS\Nitro\Firewall` (`LastPushRejectedRules`, `LastPushRejectedNames`, `LastPushBroadBlockRules`) and in the Application event log.
- **Rare crash in DirectWrite (Firefox):** in a 12-hour stress test, one Firefox child process stopped once with an access violation in `dwrite.dll` (the browser carried on). A fix is in progress (ticket SOAK-dwrite-av-null-0x11).
- **NTFS-NG file cache size:** NTFS-NG keeps up to about 2.6 % of RAM (about 158 MB on a 6 GB machine) of recently used file information in memory by design. It gives memory back under memory pressure; it is not a leak.
- **schannel:** `SCH_CREDENTIALS.pDisabledCrypto` is accepted but not enforced (the cipher suite list is mbedTLS's default), and disabling protocols applies to outbound connections only (TLS server-side is not implemented).
- **Oracle Database XE 21c EM Express:** https://localhost:5500/em does not load yet; the database at localhost:1521 works.
- **Image sections and unflushed writes:** Creating an image section from a file that also has a writable mapping sees writes that were not yet flushed (Windows reads those pages from disk). No effect on normal program loading.
- **FAT system volume:** Installing to a FAT system volume: the system can fail to start after the first restart ('SYSTEM hive could not be used'), because a grown registry hive file's size is not written back. Use NTFS, the default.
- **USB3:** the USB3 (xHCI) stack that came with the upstream sync is on the CD but not enabled by default.

Carried over from 2026.11-1:
- **Pager:** on by default on amd64 only. i386 has it off. `NitroPager=0` (HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management) turns it off on amd64. FAT page files were not reviewed under the pager.
- **Locale formats** come from Unicode CLDR 49. They are closer to Windows than in 2026.11-1, but several non-English cultures still format dates, numbers and currency differently from Windows.
- **Live CD desktop:** the live desktop of the boot CD has no 32-bit apps (installed systems are unaffected).
- **Word** installs and now reaches its Start screen, but it does not run yet: it stops at its licence check.
- **UIPI is missing:** there is no cross-process window message isolation between integrity levels.
- **VS Code integrated terminal** renders corrupted with its default WebGL renderer in most launches. Workaround: set `terminal.integrated.gpuAcceleration` to `off`.
- **Installs made before `57c2c276858`** had user-writable system directories; the upgrade boot fixes existing installs.
- Tested under QEMU/KVM only; real-hardware paths (notably the APIC HAL) are not covered.

Fixed since 2026.11-1: the 64-bit data corruption issue (see Highlights); a 1,400 MB program next to a `git clone` on a 1 GB machine now completes.

### Licences
- ReactOS and the fork's own code: GPL-2.0-or-later (0 added files with GPL-2.0-only SPDX identifiers in the release patch).
- Locale data: generated from Unicode CLDR 49.0.0, Unicode License v3 (Unicode-3.0); the notice ships on the CD.
- Time zones: generated from CLDR windowsZones (Unicode License v3) and IANA tzdata (public domain).
- mscms: Little CMS 2.20 (MIT), as Wine bundles it; the default sRGB profile is generated and CC0.
- Mesa (MIT), LLVM (Apache-2.0 with LLVM exception) and the Vulkan loader: licence texts ship on the CD next to the DLLs; the DLLs are standalone aggregates.
- Fonts: their licences are in `3rdParty` on the CD.

### Changes since the previous Nitro
This is the first release after 2026.11-1 (published 2026-11-09, `f2ee4900dab`). The upstream base moves from `e46f4bb1a1e` to `8fe3cee0560` (weekly sync #5); the delta is 453 first-parent changes, all listed under Bug fixes above. Note: on 2026-11-16 the fork's history was rewritten to correct commit metadata; the `nitro-2026.11-1` tag now points to `f2ee4900dab` (same tree as the original `87c24e29c3a`).

## reactos-nitro 2026.11-1

Tagged `nitro-2026.11-1` (2026-11-08) at fork main [`87c24e29c3a`](https://github.com/nitroos-org/reactos/commit/87c24e29c3a0239e73f3bbb4c782c28ea4a21b85). 592 changes since 2026.10-5: the memory pager is on by default on amd64, a first Windows.UI.Composition and DirectComposition layer (WebView2 and Edge render), the NitroOS firewall on by default, Wine 11.21 synchronisation, registry security descriptors, a hardened NTFS-NG and FAT, and the first release built with the SEH plugin fix.

| Item | Value |
|---|---|
| Release tag | `nitro-2026.11-1` → [`87c24e29c3a`](https://github.com/nitroos-org/reactos/commit/87c24e29c3a0239e73f3bbb4c782c28ea4a21b85) |
| ReactOS base | upstream [`e46f4bb1a1e`](https://github.com/reactos/reactos/commit/e46f4bb1a1ea7c722f941231565144fe0c286d3e) "[SAMSRV] Implement SampRemoveUserFromAllAliases" (2026-10-25, sync #4); unchanged since 2026.10-5, so this release has no upstream sync |
| Patch | one cumulative patch over `e46f4bb1a1e` (`git diff --binary`, 257 binary files; apply with `git apply --binary`) instead of a per-topic series; applied tree `dfed06bebed1` is byte-identical to the tag |
| Artifacts | amd64-wow64 ISO, i386 ISO (both on repo.nitroos.org), source tarball, cumulative patch, SHA256SUMS |

### Highlights
592 changes (585 branch merges, 7 direct commits). The main tracks:

- **Memory pager on by default (amd64):** working-set trimming and paging of private memory, with a low-water balancer and a pager test suite. i386 stays off; `NitroPager=0` is the rollback.
- **Windows.UI.Composition and DirectComposition, first stages:** WebView2 controllers and Edge render; Edge runs with its default sandbox.
- **Sessions P0 to P2:** per-session namespace and page tables (off by default behind a flag).
- **NitroOS firewall on by default:** inbound/outbound filter, rule store in the Windows registry format, firewall.cpl, wf.msc, a block prompt.
- **Wine 11.21 synchronisation:** 55 class-A modules, comctl32, mshtml, jscript, msxml3/4, urlmon, ieframe, msvcrt and more.
- **Registry security descriptors** and eight grouped security hardening sets (described generically below).
- **ConPTY and VT input:** pseudoconsole support for VS Code, Windows Terminal and PowerShell 7.
- **PowerShell 7 preinstalled** with clean-room NetAdapter, NetTCPIP, DnsClient, Storage, ScheduledTasks and LocalAccounts modules; Scoop, .NET 4.8, Gecko, AAC and the GPU pack through the Features manager.
- **WoW64:** 50 fixes (debug API, thread context, handle information, SCSI pass-through, ioctl allow-list).
- **NTFS-NG and FAT:** journal and lock-ordering hardening, POSIX delete, chkdsk/autochk for NTFS-NG, a dosfstools 4.2 based FAT checker.
- **Heap and locks:** `RtlEnterCriticalSection` spins before waiting, and user-mode heaps get a small-block front end; the heap is about 30x faster under contention.
- **WoW64 fast path:** syscall overhead drops from about 245 ns to about 92 ns (about 2.6x lower).
- **Locale data from Unicode CLDR 49:** the 146 locale tables and calendar data are generated by an in-tree, deterministic generator (Unicode License v3; nothing taken from Windows). Also: the Windows 10 system ICU, NVMe storport miniport, NtSetInformationKey.
- **Faster NTFS-NG writes:** non-cached writes go to the device directly, up to 1 MB per request (AHCI sequential write 205 to 358 MB/s in our test).
- **Setup fixes:** the i386 live setup runs with DMA (about 1 minute to install in our test VM); FAT 0x23 under heavy load and a USB hot-plug hang are fixed; featmgr gets full AAC (LC, HE-AAC, HE-AAC v2).
- **Clean build:** the amd64 Release binaries are free of the SEH `__finally` defect (0 affected functions, verified by an objdump based scan).
- **Branding:** the product presents as NitroOS 2026.11-1; new installs default to `C:\Windows`.

### What reactos-nitro adds on top of ReactOS
Everything from the previous releases, plus the following in this release.

#### WoW64
Fifty fixes on top of 2026.10-5: the 32-bit debug API, WOW64 TEB offset, thread context in 64-bit code, extended handle information, SCSI/ATA pass-through, a 336-entry pointer-free ioctl allow-list, crash reporter in SysWOW64, ExitWindowsEx and console control forwarding.

#### NTFS and FAT
NTFS-NG journal commit runs without the core lock, POSIX delete for files, MFT record reuse, zero-fill and resident-page handling, dirty-bit handling at mount, and chkdsk/autochk through the NTFS-NG control codes. fastfat gets the stale-tail, orphan-pin, memory-pressure and oplock fixes and a FAT checker synced to dosfstools 4.2.

#### Kernel and memory
The pager (P0, P1, default-on, debug gate), per-session page tables, balancer low-water mark, commit limit as RAM plus paging files, placeholder memory (`NtAllocateVirtualMemoryEx`), QPC in user mode, scheduler and foreground-boost work, amd64 debug registers and exception parity.

#### Display and compositor
Windows.UI.Composition stages 0 to 3 (visual tree), a minimal DirectComposition, DirectWrite font fallback, per-desktop win32k work, UpdateLayeredWindow and GDI fixes, cross-process SetParent as on Windows.

#### Networking
NitroOS firewall and its UI, TCP receive-window and loopback fixes, AFD send-window and shutdown fixes, WSASendMsg, netsh portproxy, iphlpapi parity, ipconfig/net.exe output matching Windows, HTTP Server API, schannel.

#### Setup and user profiles
Default installation directory `C:\Windows` (with a junction), Windows-style computer names, no Wine Gecko at setup (installed on demand through Features), NitroOS product strings.

#### Web stack and apps
Wine 11.21 imports (mshtml, msxml3, urlmon, comctl32, msvcrt, 55 more modules), WebView2 and Edge hosting, Firefox popup/caption/stack fixes, Office C2R install support (Word itself still stops at its splash, see Limitations), Chrome and Firefox default-browser handling.

#### Other
Security hardening in eight groups (file-system links, loader and path handling, LSA, win32k USER, networking, and others), GPL-2.0-or-later headers on 92 fork-authored sources, new apitests.

## Bug fixes

The 592 changes on the fork's first-parent history between `nitro-2026.10-5` and this release (585 branch merges and 7 direct commits), grouped by area. Each line links the merge commit. Security-sensitive fixes are described generically. This section lists only what is new in 2026.11-1; it includes nothing that was already in 2026.10-5.

<details><summary>Kernel: Ke/HAL/amd64 (25 fixes)</summary>

- NtCreateThread captures the CONTEXT and INITIAL_TEB once and checks them before the process handle, as Windows does (statuses measured) ([fix/ntsmall](https://github.com/nitroos-org/reactos/commit/829ca1a1ae5))
- A ready foreground thread queued behind a busy background thread of the same priority gets a decaying boost at the next tick (busy background share on one CPU 25% -> ~12% ([fix/foreground-wallclock-share](https://github.com/nitroos-org/reactos/commit/21a3399556c))
- Nested exceptions marked during amd64 dispatch ([fix/exception-pass5](https://github.com/nitroos-org/reactos/commit/a361821305c))
- Disarm DR7 before loading user DR0-3, DR6 cleared on entry and set to no-event on user return (oracle), kd64 keeps KernelDr0-3 in sync on all processors ([fix/amd64-dr](https://github.com/nitroos-org/reactos/commit/47fbf29dc41))
- Threads start in ntdll!RtlUserThreadStart through kernel32's thread start, native x64 DEP permanent (as Windows), amd64/WoW64 user callback stack layouts, RaiseException keeps non-volatile ... ([fix/exception-pass4](https://github.com/nitroos-org/reactos/commit/fb6a8ed3e2a))
- XState contexts as Windows (RtlInitializeExtendedContext writes only the CONTEXT_EX, GetThreadContext writes only features in use and reports overflow), XSTATE lengths ... ([fix/exception-x64-pass3](https://github.com/nitroos-org/reactos/commit/92c5faf92e0))
- Amd64 user hardware breakpoints (per-thread DRs loaded on return to user mode, kernel DRs in kernel mode, Windows sanitizing), TF cleared before single-step and user dispatch ([fix/ke-amd64-user-debug-registers](https://github.com/nitroos-org/reactos/commit/1bb1249dfc1))
- X87/SSE control state in the contexts of native x64 user frames, unwind context copies drop CONTEXT_XSTATE ([fix/exception-x64-pass2](https://github.com/nitroos-org/reactos/commit/880abc638eb))
- Amd64 user port I/O denied, WoW64 SIMD code, int3/int 2d parameter counts, 64-bit DS/ES ([fix/exception-winetest-x64-clusters](https://github.com/nitroos-org/reactos/commit/4a6dc54f029))
- The process that owns the foreground window keeps its foreground quantum (CSRSS console-focus background requests reset it to 6 → 50/50) ([fix/foreground-quantum-share](https://github.com/nitroos-org/reactos/commit/581114a9ac4))
- MessageWakeBoost waits until the waiter is blocked and tolerates decay ([fix/sched-followups](https://github.com/nitroos-org/reactos/commit/6b4a7517dbb))
- Clock IPI only per completed tick (APs charged ~16x wall time at 1 ms timer resolution), window-message wakes boost +2 and GetMessage/PeekMessage no longer yield to lower-priority CPU hogs ... ([fix/clock-accounting-input-tick](https://github.com/nitroos-org/reactos/commit/33441a2ec1a))
- Foreground scheduling class on foreground changes (KeSetPriorityAndQuantumProcess applied no quantum when the base priority was unchanged), keyboard/mouse input wake boosts ([fix/foreground-responsiveness](https://github.com/nitroos-org/reactos/commit/b8e4c9ada77))
- CONTEXT_EXCEPTION_REQUEST/REPORTING/SERVICE_ACTIVE/EXCEPTION_ACTIVE per oracle (amd64 KTRAP_FRAME.ExceptionActive, i386 ErrCode markers) ([fix/context-exception-request-flags](https://github.com/nitroos-org/reactos/commit/6004e8b0592))
- RtlGetUnloadEventTraceEx contract, amd64 x87 #MF delivery (CR0.NE + KiFloatingErrorFault), KeContextToTrapFrame forces user selectors (bogus selector bugchecked 0x1E) ([fix/x64-exception-winetest](https://github.com/nitroos-org/reactos/commit/5ba2274188d))
- Amd64 int3 ExceptionAddress at the int3 (as Windows/i386), WoW64 debug.c nested record + continue failure, Dbgk FileHeader reads under SEH ([fix/debug-api-followups](https://github.com/nitroos-org/reactos/commit/c99a1a0567f))
- IRTimer with NtCreateTimer2/NtSetTimer2/NtCancelTimer2/NtSetTimerEx, clock raised only while a high-resolution timer is due, CREATE_WAITABLE_TIMER_HIGH_RESOLUTION and tolerable delay in ... ([fix/high-resolution-waitable-timer](https://github.com/nitroos-org/reactos/commit/72df60c6bf9))
- Recheck affinity under the thread lock in the remote ready take ([fix/sched-review-nits](https://github.com/nitroos-org/reactos/commit/cd87506faf0))
- At quantum end a processor with no local ready thread of the current priority takes one from another processor (threads allowed on every processor, preferring this ideal processor), so ... ([fix/ke-ready-queue-rebalance](https://github.com/nitroos-org/reactos/commit/8f3b6d6ad2c))
- A wait that is satisfied at once no longer costs a quantum unit, as on Windows 10 (threads looping on signaled objects kept 0% vs a spinner ([fix/ke-satisfied-wait-quantum](https://github.com/nitroos-org/reactos/commit/2b148e8e41d))
- NtQuerySystemProcessorPerformance apitest guards idle/kernel time accounting (KernelTime >= IdleTime, counters advance) ([fix/ke-up-idle-time](https://github.com/nitroos-org/reactos/commit/f1f8e681c60))
- Kernel-mode delays never end before the interval, as on Windows (KeDelayExecution kmtest) ([fix/ke-delay-kernel-bootanim-ext-r](https://github.com/nitroos-org/reactos/commit/00bb9f058ff))
- [NTOS:KE] correct the reason kernel-mode delays stay tick-granular (measured: floppy head-settle and boot-logo 15 ms delays cost two ticks each when counted from now) ([fix/ke-delay-kernel-bootanim](https://github.com/nitroos-org/reactos/commit/4db13c47690))
- User mode delays (Sleep, NtDelayExecution) never end before the interval has passed ([fix/timer-wait-early-return](https://github.com/nitroos-org/reactos/commit/4e60f0ddcdc))
- Run the STATUS_UNWIND_CONSOLIDATE callback (C++ catch blocks) from a frame that unwinds to the target context, so a rethrow no longer dispatches through already-unwound frames — PowerShell ... ([fix/amd64-unwind-consolidate](https://github.com/nitroos-org/reactos/commit/931b7b27d5e))

</details>

<details><summary>Kernel: Mm/Cc (36 fixes)</summary>

- RtlEnterCriticalSection spins (up to the section's spin count) before waiting, without barging past queued waiters ([fix/heapmt](https://github.com/nitroos-org/reactos/commit/a11d1d96126))
- NitroPagerDebug (fault injection, pager test commands) is ignored by Release builds unless booted with /NITROPAGERDEBUG, in the kernel and in NTFS-NG (a value planted by any user no longer ... ([mm/pager-dbggate](https://github.com/nitroos-org/reactos/commit/c4461be443f))
- NitroPager values above 2 are clamped to 0 right after the control values are read, before ARM3 phase 0 (a planted value no longer switches the pager on in phase 0 and off in phase 1) ([mm/pager-clamp](https://github.com/nitroos-org/reactos/commit/62dd9b82219))
- NitroPager ON by default on amd64 (off on i386 until its soak gate is met ([mm/pager-default](https://github.com/nitroos-org/reactos/commit/5acf71424c1))
- The section-fault pre-fault holds the page table it makes valid (MmHoldPageTable/MmReleasePageTable) so the 2-level UsedPageTableEntries invariant holds and an unused page table is freed ... ([fix/i386pde](https://github.com/nitroos-org/reactos/commit/2374bfda0db))
- NitroPager P1 (flag-gated, off by default): working-set trimmer with per-process cursors and legacy-consumer share, background modified-page writer with slot runs, paging-file ... ([mm/pager-p1](https://github.com/nitroos-org/reactos/commit/0eaf931ccc8))
- A cut of a file whose cache map is being deleted zeroes the resident pages past the new end, so the pending delete-flush can no longer write old data over a regrown range (stale bytes after ... ([fix/ccflush](https://github.com/nitroos-org/reactos/commit/cb25637ab72))
- NitroPager P0 (flag-gated, off by default) - ARM3 private memory paged out on request: owner-tagged page-file slots, private standby repurpose, EmptyWorkingSet + SystemMemoryListInformation ... ([mm/pager-p0](https://github.com/nitroos-org/reactos/commit/c5d6c754bb3))
- Sessions P1 - per-session namespace for sessions other than 0 (session-tagged BNO, AppContainerNamedObjects, Windows-measured DACLs on \Sessions\<n>, \Sessions\<n>\Windows and BNOLINKS ... ([sess/p1](https://github.com/nitroos-org/reactos/commit/f3d301c15c0))
- Pagersoak, the in-guest memory-pressure and integrity test program for the pager phases (private memory with per-page checksums, trim/purge cycles, file intent/ack log for power-cut checks) ([mm/pager-H](https://github.com/nitroos-org/reactos/commit/90109a84c35))
- The balancer keeps a low-water mark of free pages (1/32 of RAM, 4-32 MB) and allocators wake it below it, so cache views are trimmed before the free list empties (1 GB guests no longer ... ([fix/lowmem](https://github.com/nitroos-org/reactos/commit/03a4a264628))
- PAGE_TARGETS_INVALID / PAGE_TARGETS_NO_UPDATE accepted with execute protections (validated and dropped, as Windows reports the plain protection) in allocations, protection changes and views ... ([fix/nodev8](https://github.com/nitroos-org/reactos/commit/24fc9830a1b))
- IopCloseFile waits for the file object lock before the unlock-all only when a thread of the closing process has a request in progress on it (as measured on Windows) ([fix/closehang](https://github.com/nitroos-org/reactos/commit/893c7bf5b56))
- Sessions P2 review follow-ups - a session leader holding the last reference drops its view, data-page reference and session bit ([sess/p2-fu](https://github.com/nitroos-org/reactos/commit/82881a08a0a))
- Sessions P2 - per-session page tables (processes take their session's tables, smss drops its view on detach), session 1 with a basesrv-only csrss at NitroSessions=4 (test flag), Sm API gate ... ([sess/p2](https://github.com/nitroos-org/reactos/commit/865f2ff3b0f))
- Shutdown gives up on a dirty page whose write keeps failing (8 failures over 5 s, or a device with no successful write) and logs a lost delayed write, instead of looping forever ([fix/shutloop](https://github.com/nitroos-org/reactos/commit/62fbf6becb5))
- Sessions P0, the memory manager runs more than one session (behind NitroSessions, default 0 = today's code path): lazy unmap of the leader's session view, private session page tables on ... ([sess/p0](https://github.com/nitroos-org/reactos/commit/6ec651c0aed))
- NtQueryVirtualMemory classes 1 (working set), 5, 7 and 8 answer as Windows 10 does (psapi QueryWorkingSet works), MemorySectionName statuses and handle type check, VirtualQuery reports a ... ([fix/queryvm](https://github.com/nitroos-org/reactos/commit/6f6aa8d2c3a))
- CreateThread/CreateRemoteThread take the stack size as SIZE_T as in the Windows headers (a 1 TB stack on amd64 was cut to the default and never refused ([fix/commitlim](https://github.com/nitroos-org/reactos/commit/db0c9a406b0))
- The lazy writer leaves a file it could not lock without waiting for the next pass, whole, instead of writing its later views out of order (the file system zero-filled ~90 MB below them ... ([fix/ntdelstall](https://github.com/nitroos-org/reactos/commit/ce2d68a789a))
- A page-in takes no file-system lock when the thread already holds the file's main resource, otherwise the paging resource shared (starve-exclusive, recursive), and no section-creation ... ([fix/mm-pagein-locking](https://github.com/nitroos-org/reactos/commit/f21ec784c99))
- Cc defers uninitializing a closed file's shared cache map (ntfsng opt-in ([fix/cc-lazy-ntfs-only](https://github.com/nitroos-org/reactos/commit/0e2a1bd9875))
- NtQueryVirtualMemory(MemoryRegionInformation, class 3) implemented as Windows 10 (oracle RegionType values, whole-allocation sizes), WoW64 thunk ([fix/memory-region-information](https://github.com/nitroos-org/reactos/commit/319d94fc8b5))
- Placeholder memory (NtAllocateVirtualMemoryEx, NtMapViewOfSectionEx, NtUnmapViewOfSectionEx ([feat/mm-placeholders](https://github.com/nitroos-org/reactos/commit/0926213cbfc))
- A relocated image's code goes back to PAGE_EXECUTE_READ after the fix-ups (was execute-only PAGE_EXECUTE), sections without explicit protection get the section mapper's defaults ([fix/reloc-image-protection](https://github.com/nitroos-org/reactos/commit/31b1e759c3f))
- PDH/perf-data Paging File object, wmic pagefile alias with list/get/format (oracle layout), systeminfo page file location from WMI ([fix/pagefile-gaps](https://github.com/nitroos-org/reactos/commit/c9f85f39e46))
- The console framebuffer covers the view, not the 120x9001 screen buffer (540 MB of CSRSS commit per console window) ([fix/commit-baseline-boot](https://github.com/nitroos-org/reactos/commit/e5f88dbd32f))
- Report CommitLimit as RAM plus paging files (as Windows), AvailPageFile = limit - charge ([fix/commit-limit-windows](https://github.com/nitroos-org/reactos/commit/eee078032bb))
- QPC follow-ups — HalpKvmTscStable cleared per read, KeQueryPerformanceCounter fenced like user mode, Hygon as AMD, DisableUserQpc registry knob ... ([fix/qpc-followups](https://github.com/nitroos-org/reactos/commit/5a5b39c6c11))
- QueryPerformanceCounter in user mode from KUSER_SHARED_DATA (RDTSCP, Windows-style bypass flags) at 10 MHz as on Windows 10 — 97 → 18 ns per call x64, 332 → 23 ns WoW64 ([perf/usermode-qpc](https://github.com/nitroos-org/reactos/commit/33bc5d9cb6e))
- Drop a VirtualLock when the last mapping of a section page goes (one page leaked per locked+unmapped cycle), correct the delete-path assert, test fails cleanly ([fix/mm-section-page-leak-nits](https://github.com/nitroos-org/reactos/commit/48b6c7e39b7))
- Drop the prototype PTE page's share count when a pagefile-backed segment frees a page (one page leaked per section cycle) ([fix/mm-section-page-leak](https://github.com/nitroos-org/reactos/commit/eb0935f3ba3))
- Partially overlapping pins in the cache manager serialize as on Windows (gap BCBs plus an OBCB holding every touched BCB lock in ascending order) ([fix/cc-partial-overlap-pins](https://github.com/nitroos-org/reactos/commit/822e31f719e))
- AnyCPU .NET executables (IL-only PE32 without 32-bit flags) start as native 64-bit processes like on Windows ([fix/dotnet-anycpu-native-x64](https://github.com/nitroos-org/reactos/commit/88cb0a81514))
- System-managed paging files sized like Windows 10 (RAM/8 initial, max(3xRAM, 4 GB) maximum, at most 1/8 of the volume) — a large RAM no longer fills a small NTFS volume ([fix/smss-pagefile-cap](https://github.com/nitroos-org/reactos/commit/54bef1972c1))
- Amd64 uses all RAM (GlobalMemoryStatusEx 64-bit, freeldr cap, pool sizing, system-view PDEs, FAT pagefile cap) ([fix/mm-above-4gb](https://github.com/nitroos-org/reactos/commit/45d30452e5d))

</details>

<details><summary>Kernel: Ob/Se/Ps/Ex/registry (31 fixes)</summary>

- NtSetInformationKey implemented (6 classes, oracle-matched class/length/access/buffer checks, WoW64 passthrough ([fix/smalls19](https://github.com/nitroos-org/reactos/commit/5f828c4b1c5))
- NtAlertThreadByThreadId/NtWaitForAlertByThreadId (Office probes them ([fix/smalls18](https://github.com/nitroos-org/reactos/commit/63f538a69ee))
- Security hardening: a group of fixes in this area; details are withheld until users have had time to update ([fix/smalls10](https://github.com/nitroos-org/reactos/commit/200726fd1f2))
- Security hardening: a group of fixes in this area; details are withheld until users have had time to update ([fix/acotl](https://github.com/nitroos-org/reactos/commit/6776aad7ff1))
- Security hardening: a group of fixes in this area; details are withheld until users have had time to update ([fix/lowboxfu](https://github.com/nitroos-org/reactos/commit/e4fd04104c2))
- Security hardening: a group of fixes in this area; details are withheld until users have had time to update ([fix/lowbox](https://github.com/nitroos-org/reactos/commit/3b3a6cf258d))
- The iphlpsvc PortProxy config thread re-arms its registry change notification only after it fired (a new request was queued every 10 s poll: CmNn grew ~360/h) ([fix/cm-notify-leak](https://github.com/nitroos-org/reactos/commit/d5abc72cc2d))
- CmpLookInCache no longer converts a KCB bucket lock out of order under the shared sorted array (registry/SCM freeze under concurrent opens) ([fix/cm-kcb-lock-order](https://github.com/nitroos-org/reactos/commit/4f96b9109c0))
- BuildLabEx as Windows 10 22H2 (per arch), RegGetValueW/A size query with REG_SZ|REG_EXPAND_SZ and expansion ([fix/registry-small](https://github.com/nitroos-org/reactos/commit/24ef7614c94))
- Security hardening: a group of fixes in this area; details are withheld until users have had time to update ([fix/regacl-sk-recount](https://github.com/nitroos-org/reactos/commit/a2bccc614ee))
- Security hardening: a group of fixes in this area; details are withheld until users have had time to update ([fix/regacl-r7c-timezone](https://github.com/nitroos-org/reactos/commit/d74bc7db3ff))
- A loaded hive frees its file image, its name and the root key keep-alive reference on unload (about 1 MB of paged pool per hive load) ([fix/cm-hive-unload-leak](https://github.com/nitroos-org/reactos/commit/7c5f45e1699))
- Security hardening: a group of fixes in this area; details are withheld until users have had time to update ([fix/regacl-r7-usermode](https://github.com/nitroos-org/reactos/commit/a1ab73529c3))
- Security hardening: a group of fixes in this area; details are withheld until users have had time to update ([fix/regacl-r1-sk-primitives](https://github.com/nitroos-org/reactos/commit/f11d3342e0c))
- Release builds list one NITRO-<version> Win32_QuickFixEngineering row (never a KB number), dev builds none ([fix/qfe-nitro-release-row](https://github.com/nitroos-org/reactos/commit/a09efc1b3aa))
- Partial WNF — NtQueryWnfStateData (Windows 10 answers for the quiet-hours state Thunderbird/Firefox query, NOT_FOUND otherwise) and Rtl(Un)SubscribeWnfStateChangeNotification ([fix/wnf-query-minimal](https://github.com/nitroos-org/reactos/commit/c684ad54c1b))
- Security hardening: a group of fixes in this area; details are withheld until users have had time to update ([fix/token-query-parity](https://github.com/nitroos-org/reactos/commit/d13e598a877))
- NtQueryInformationToken and the NtAccessCheck family accept the Windows 8+ token pseudo handles (-4 process, -5 thread, -6 effective) for query access only, as on Windows 10 (Windows ... ([fix/token-pseudo-handles](https://github.com/nitroos-org/reactos/commit/8960cb6f9bd))
- IoWMIOpenBlock closes its kernel handle ([fix/system-handle-leak-ps7](https://github.com/nitroos-org/reactos/commit/f584e76b03d))
- Accept the AppContainer profile names Windows accepts (oracle table), create and delete never leave Packages ([fix/appcontainer-name-parity](https://github.com/nitroos-org/reactos/commit/a121ad827f5))
- Equal compressed sibling names handled as Windows (second dropped), mkhive and the boot loader use the Unicode case table ([fix/cm-case-table-leftovers](https://github.com/nitroos-org/reactos/commit/8cbb0f0a7ed))
- Bring subkey index hashes and order up to the Unicode case table when a hive is checked ([fix/cm-lh-hash-selfheal](https://github.com/nitroos-org/reactos/commit/b67485aed05))
- Stored case in KeyNameInformation, Unicode case tables as Windows, CmpAddSubKey fails cleanly, rename hardening ([fix/cm-rename-followups](https://github.com/nitroos-org/reactos/commit/6f1f2dbb8bf))
- Implement NtRenameKey (new key cell, index swap, KCB re-key with descendant rehash), RegRenameKey, WoW64 thunk, ZwRenameKey ([feat/cm-ntrenamekey](https://github.com/nitroos-org/reactos/commit/7f5812d24d0))
- The NT 6 token classes report what Windows reports with UAC off (TokenElevationType Default, TokenElevation from the Administrators group or the elevating privileges, TokenLinkedToken ... ([fix/token-elevation-classes](https://github.com/nitroos-org/reactos/commit/4265a3f00c2))
- Win32_QuickFixEngineering with the Windows 11-property schema, populated from installed servicing packages (empty when none) — Get-HotFix ([fix/wmi-qfe-properties](https://github.com/nitroos-org/reactos/commit/d4fc238b112))
- Userenv CreateAppContainerProfile/DeleteAppContainerProfile/GetAppContainerFolderPath/GetAppContainerRegistryLocation (Windows registry and folder layout, validated names, paths confined ... ([feat/appcontainer-profile-api](https://github.com/nitroos-org/reactos/commit/f7f7ad5419d))
- SERVICE_CONFIG_REQUIRED_PRIVILEGES_INFO and SERVICE_CONFIG_TRIGGER_INFO in services.exe/advapi32 (registry-backed, Windows layouts incl ([fix/scmr-small-gaps](https://github.com/nitroos-org/reactos/commit/8ebb1969b3a))
- Registry certificate stores write adds, deletes, replacements and property changes through immediately, honour CERT_STORE_CTRL_AUTO_RESYNC, refuse changes on read-only handles ([fix/crypt32-regstore-write-through](https://github.com/nitroos-org/reactos/commit/3a2d4c282b0))
- QueryServiceConfig2/ChangeServiceConfig2 levels 3/5/7, EnumDependentServices access ([fix/scm-query-config](https://github.com/nitroos-org/reactos/commit/46811eb4543))
- Ws2_32 thread detach + shell32 ProfileList key leak ([fix/thread-handle-leak](https://github.com/nitroos-org/reactos/commit/1eb5895e994))

</details>

<details><summary>I/O + fs libs (8 fixes)</summary>

- FsRtl byte-range locks belong to their file object (and process and key): closing or unlocking through one file object no longer releases locks taken through another ([fix/locksep](https://github.com/nitroos-org/reactos/commit/48bf7d2d803))
- CancelSynchronousIo apitest for the libuv reader-stop loop (no kernel change ([fix/cancelio](https://github.com/nitroos-org/reactos/commit/ee153e34d5f))
- NtCancelSynchronousIoFile keeps at most one pending cancel APC per thread and I/O status block (a caller cancelling in a loop while the target thread could not run kernel APCs queued ... ([fix/cancel-sync-io-apc-flood](https://github.com/nitroos-org/reactos/commit/a906972438e))
- A pending replace over a read-only target clears the attribute and retries also on STATUS_ACCESS_DENIED (what NTFS returns), so delayed updates of read-only files apply ([fix/smss-readonly-rename](https://github.com/nitroos-org/reactos/commit/d56bfbf67e7))
- FindNextFileW no longer drops a directory entry whose name ends on the last byte of its query buffer (rmdir /s, del /s left files behind) ([fix/rmdir-s-leftover](https://github.com/nitroos-org/reactos/commit/7fc6f446124))
- NtQueryInformationFile passes FileIdInformation (59) on, ntfsng answers it as NTFS (std::filesystem::equivalent, Office C2R) ([fix/fileidinfo-equivalent](https://github.com/nitroos-org/reactos/commit/0e6f4c7f38f))
- IoIs32bitProcess decides by the issuing thread's process (create/information/flush/unlock requests from WoW64 got FALSE), NULL-thread IRPs no longer dereferenced ([fix/io-is32bitprocess](https://github.com/nitroos-org/reactos/commit/369df8be0a6))
- Port-only I/O requests survive their issuing thread's exit and complete through the completion port as on Windows (detached table, process-context completion, cancel/close/process-exit ... ([fix/iocp-irps-survive-thread-exit](https://github.com/nitroos-org/reactos/commit/ab83bb41fca))

</details>

<details><summary>NTFS (ntfsng) (26 fixes)</summary>

- CcCopyRead/CcCopyWrite through a file object without a private cache map keep the shared map referenced for the call (the lazy writer deleted an unopened map under the copy's VACB: bugcheck ... ([fix/ng139](https://github.com/nitroos-org/reactos/commit/97ed0ee0988))
- MmCanFileBeTruncated no longer counts the cache manager's own section while CcRosDeleteFileCache flushes it (transient ERROR_USER_MAPPED_FILE on CREATE_ALWAYS/SetEndOfFile under load, NTFS ... ([fix/umapped](https://github.com/nitroos-org/reactos/commit/6d266df19e0))
- NTFS-NG tracks where its resident pages are known zero and only re-zeroes the page the old end sits in (an overwrite by appends re-zeroed every page of the old file at each growth: 60-77 s ... ([fix/tcpfu](https://github.com/nitroos-org/reactos/commit/62f3553ca64))
- A section view fault makes its page tables before it takes the segment lock (lock-order deadlock with the lazy writer at low memory -> bugcheck 0x7A) ([fix/ng7a](https://github.com/nitroos-org/reactos/commit/74454485213))
- NTFS-NG zeroes resident cache pages past the old size before a growth and after a failed purge, so a shrink/regrow can never show old data ([fix/ngstale](https://github.com/nitroos-org/reactos/commit/c9b00e7adde))
- NTFS-NG NgZeroGap re-attaches the file's node before each piece (a paging read could park it during the unlocked transfer of the previous piece -> bugcheck 0x7E under memory pressure) ([fix/ng7e](https://github.com/nitroos-org/reactos/commit/98ec451a5f2))
- NTFS-NG named-stream search contexts go through the base inode ([fix/streamaudit](https://github.com/nitroos-org/reactos/commit/908aef9ea0a))
- NTFS-NG sets the initialized size of a named stream through the base inode (through the stream's private record copy the new size was lost at unmount, so non-resident named streams read ... ([fix/attrhang](https://github.com/nitroos-org/reactos/commit/62a674be3b6))
- NTFS-NG gives a new attribute the first instance number no live attribute of the MFT record carries (the 16-bit counter wrapped after ~43,700 renames between short and long names and every ... ([fix/notifyflaky](https://github.com/nitroos-org/reactos/commit/d7d492f0d9c))
- NTFS-NG zero-fills the gap below a write past the initialized size in 1 MB pieces outside the volume lock (under the file's own paging write lock), raising the initialized size after each ... ([fix/zerofill](https://github.com/nitroos-org/reactos/commit/d80f64b0c22))
- The zero page thread maps, zeroes and unmaps on one processor and flushes only that processor's TLB (x86/x64) ([fix/zeroipi](https://github.com/nitroos-org/reactos/commit/00d803694e1))
- An NTFS-NG extend that cannot fit in the free clusters fails before any change (a retried failing extend wrote a journal transaction each time and leaked a cluster) ([fix/ntfsng-full-extend](https://github.com/nitroos-org/reactos/commit/a50ce1bcee1))
- Ntfsng keeps VOLUME_IS_DIRTY on disk while a session has changes (foreign systems still see an unclean volume) but FSCTL_IS_VOLUME_DIRTY/fsutil and autochk report dirty only for a real flag ... ([fix/ntfsng-dirty-bit-b](https://github.com/nitroos-org/reactos/commit/b910adfca4c))
- POSIX delete for files on ntfsng as Windows 10 ($Extend\$Deleted, purged at mount), DeleteFileW uses FileDispositionInformationEx with fallback ([feat/ntfsng-posix-delete-files](https://github.com/nitroos-org/reactos/commit/1f1713f8db5))
- Idle FCBs with cached data are flushed and purged past FcbCacheMax so they get freed (50k files: 64 MB -> 2.5 MB NP) ([fix/ntfsng-fcb-free](https://github.com/nitroos-org/reactos/commit/257aba4c8c9))
- Journal review nits (acquire loads, documented DONE pin, overflow counter, one fault-stage helper) ([fix/ntfsng-unlocked-commit-nits](https://github.com/nitroos-org/reactos/commit/b69fe2e08d5))
- Chkdsk/autochk check NTFS-NG volumes through FSCTL_NG_CHECK (ntfsng consistency check), safe /f repairs (leaks only), system-volume /f schedules the check at the next boot as Windows ([feat/ntfs-chkdsk](https://github.com/nitroos-org/reactos/commit/8d0858adfae))
- Ntfsng FCB lookups by MFT record use a hash instead of scanning every FCB since boot (small-file delete ~1.3k -> ~11k files/s) ([fix/ntfsng-delete-speed](https://github.com/nitroos-org/reactos/commit/270e951e4f2))
- Ntfsng journal commit I/O runs without CoreLock (seal/IO/reap ([fix/ntfsng-unlocked-commit](https://github.com/nitroos-org/reactos/commit/10a63c0ac0b))
- Clusters allocated ahead of a growing stream (no commit and CoreLock hold per extension), Debug CoreLock hold histogram ([fix/ntfsng-commit-alloc-lock](https://github.com/nitroos-org/reactos/commit/8b137c6ed2c))
- A foreign dirty flag is cleared at mount after a clean $LogFile and a passing consistency check, FSCTL_MARK_VOLUME_DIRTY, a boot volume kept read-only boots with a notice instead of ... ([fix/ntfsng-dirty-boot-volume](https://github.com/nitroos-org/reactos/commit/f06c5c083e6))
- Paging and non-cached reads without CoreLock, unlocked write hardened and fault-tested, non-cached reads wait for in-flight stream writes (race since d530677da6f) ([fix/ntfsng-corelock-followups](https://github.com/nitroos-org/reactos/commit/b18b0f13e44))
- Ntfsng paging writes do their disk transfer without holding CoreLock (file opens no longer wait behind background writers) ([fix/ntfsng-corelock-io](https://github.com/nitroos-org/reactos/commit/d530677da6f))
- Mft_data_pos updated under mftbmp_lock (no lost lowering), user records never get sequence number 0, reuse tests strict + threaded ([fix/ntfsng-mft-reuse-followups](https://github.com/nitroos-org/reactos/commit/de12ba0326a))
- Ntfsng reuses a freed MFT record at once (create/delete churn no longer fills the $MFT page cache in nonpaged pool) ([fix/nonpaged-pool-leak-msi](https://github.com/nitroos-org/reactos/commit/89eaab17466))
- Ntfsng's shim mutex waits only when contended (FAST_MUTEX protocol), the flusher runs at the critical work queue priority, disk completions boost their waiters — NTFS operations stay in ... ([fix/ntfsng-flusher-convoy](https://github.com/nitroos-org/reactos/commit/d1c4bdf765a))

</details>

<details><summary>FAT (11 fixes)</summary>

- Fastfat writes Vcb->VcbState with interlocked OR/AND (55 writers ([fix/fat23b](https://github.com/nitroos-org/reactos/commit/f608f045854))
- Fastfat clears VCB_STATE_FLAG_CREATE_IN_PROGRESS in a finally around FatOpenExistingFcb's flush/purge region (a raise there left the volume flagged and the next create bugchecked 0x23) ([fix/fat23](https://github.com/nitroos-org/reactos/commit/a690f0ba892))
- Below the reserve, a user-mode fault on a section page waits for the balancer like a demand-zero fault, and MmCreateKernelStack waits (bounded) instead of failing: mapped-file churn no ... ([fix/fathang](https://github.com/nitroos-org/reactos/commit/a03b2179aa4))
- Fsregrow reports failures per variant ([fix/mapbeyond](https://github.com/nitroos-org/reactos/commit/96d7415f8a9))
- Fastfat dirties repinned ranges again before unpinning them, so a lazy write between the early dirty and the later change can no longer drop a directory entry's first cluster (lost cluster ... ([fix/fatorph](https://github.com/nitroos-org/reactos/commit/9e216984d12))
- Fastfat clears the tail of the last sector and purges cached pages past the new end when a file is cut (old bytes past the cut were read back after a regrow and reopen) ([fix/fatstale](https://github.com/nitroos-org/reactos/commit/8a2c43f1033))
- R2: fastfat's volume file gets ValidDataLength = FatMaxLarge like its directory and EA files (a dropped cache page of the FAT was re-read as zeros under memory pressure: free-count drift ... ([fix/ccdirty](https://github.com/nitroos-org/reactos/commit/385335d27fb))
- Fastfat metadata pins wait for memory (50 ms retries, up to 60 s) instead of failing half way through an operation with STATUS_NO_MEMORY, as Windows blocks ([fix/fatpress](https://github.com/nitroos-org/reactos/commit/e1b758e3157))
- FsRtl oplocks return STATUS_PENDING for queued requests (Windows contract), no double IntLock release, no walking freed list entries ([fix/fatoplock](https://github.com/nitroos-org/reactos/commit/0fc00ed3f21))
- MmPurgeSegment keeps a dirty page that is only partly in the purge range (a file cut mid-page with its cache views unmapped lost the kept bytes as zeros) ([fix/cc-lazy-fat-r2](https://github.com/nitroos-org/reactos/commit/abf737f9835))
- FAT checker synced to dosfstools 4.2 + later fsck.fat fixes (GPL-3.0-or-later, recorded), autochk repairs dirty FAT volumes, BootExecute 'autocheck autochk *' as Windows ([feat/fat-checker-sync-autochk](https://github.com/nitroos-org/reactos/commit/89ab050ded6))

</details>

<details><summary>win32k/GDI/USER (41 fixes)</summary>

- SetParent works on another process' window as on Windows ([fix/cross-process-parent](https://github.com/nitroos-org/reactos/commit/692d5cd1c62))
- MaskBlt/StretchBlt pattern fills honour the DC brush origin, negative pattern offsets wrap correctly in the 8/16/24/32 bpp blitters ([fix/smalls12](https://github.com/nitroos-org/reactos/commit/b1b6d0b6f65))
- Win32k message behaviour closer to Windows 10 (measured on the oracle): initial power-setting notification, WM_DWMNCRENDERINGCHANGED, owner-group WM_WINDOWPOSCHANGING, no WM_NCPAINT for a ... ([fix/msggaps](https://github.com/nitroos-org/reactos/commit/c1d756e3459))
- D3DKMTCheckOcclusion answers as Windows 10 does without a display miniport (live window: success ([fix/smalls2](https://github.com/nitroos-org/reactos/commit/190c1fb37ca))
- GetGlyphOutline holds the FreeType lock through every read of the glyph slot for all formats (bitmap, gray, native, bezier read it after unlocking: threads drawing glyph outlines of one ... ([fix/ggorace](https://github.com/nitroos-org/reactos/commit/0652c44dbbb))
- Starting 50 windows at once no longer overflows the kernel stack (the z-order call inside a foreground switch activated again and threads took the foreground from each other, one nesting ... ([fix/np50](https://github.com/nitroos-org/reactos/commit/f3e34d10976))
- Font size is set and read under one hold of the FreeType lock in GetOutlineTextMetrics, GetGlyphOutline, GetKerningPairs and ExtTextOut's space measuring (8 threads at different sizes of ... ([fix/fontrace](https://github.com/nitroos-org/reactos/commit/0ef0765d2eb))
- DwmGetWindowAttribute answers like Windows 10 (caption button bounds, extended frame bounds, NC rendering, cloaked and the other DWORD attributes ([fix/office-paint](https://github.com/nitroos-org/reactos/commit/fb55bca6f81))
- UpdateLayeredWindow(ULW_ALPHA) blends each update once over a snapshot of the screen below the window, not over the window's previous frame (repeated updates accumulated ([fix/ulw-alpha](https://github.com/nitroos-org/reactos/commit/253b0bd36aa))
- Riched20 draws a paragraph whose characters the font lacks as plain text through font linking (WordPad and RichTextBox drew CJK as nothing) ([fix/richedit-cjk](https://github.com/nitroos-org/reactos/commit/754bc61c150))
- A failed window creation drops its window station reference (one sandbox window station leaked per failed CreateWindow) ([fix/winsta-leaks](https://github.com/nitroos-org/reactos/commit/80b9b74c999))
- GDI font linking picks linked fonts by face name, not the base font's pitch (fixed-pitch bases such as Notepad's dropped every CJK link: Hangul showed as boxes) ([fix/gdi-hangul](https://github.com/nitroos-org/reactos/commit/edc4a797873))
- A minimal DirectComposition (dcomp.dll device, window targets, visuals, swap chain content) and DXGI CreateSwapChainForComposition (GDI present into the bound window) ([fix/webview2-gpu](https://github.com/nitroos-org/reactos/commit/3a2a1d89e3c))
- Logical-to-device conversion is exact integer arithmetic without scaling and saturates instead of overflowing (an INT_MAX clip rect became INT_MIN..0 and clipped all text: WinForms ... ([fix/clip-intmax](https://github.com/nitroos-org/reactos/commit/a4cedb254cd))
- A realized font reports its own requested style instead of the last request stored in the shared face (font matching no longer drifts ([fix/font-request-state](https://github.com/nitroos-org/reactos/commit/14ab663e3c7))
- Font enumeration reports each face's own weight and style instead of the last request's (GDI+ found 'Arial Italic' first and reported Arial missing to GraphPad Prism) ([fix/arial-enumeration](https://github.com/nitroos-org/reactos/commit/2727b53faec))
- The Display applet (and timedate, sysdm environment, intl) broadcast setting changes with SendMessageTimeout(SMTO_ABORTIFHUNG) so a hung window no longer blocks applying a new resolution ([fix/desk-resolution-hang](https://github.com/nitroos-org/reactos/commit/b6531b40780))
- GraphPad Prism 11 starts (SetWindowRgn no longer raises the window: Prism's sibling walk looped) ([fix/prism](https://github.com/nitroos-org/reactos/commit/a2abdaeac46))
- Firefox popups (downloads panel, app menu, doorhangers) no longer show a black box ([fix/firefox-popup-black](https://github.com/nitroos-org/reactos/commit/f61064e6adf))
- DirectWrite system font fallback maps CJK, Hangul, emoji, symbols, Hebrew/Arabic/Greek/Cyrillic/Devanagari to the fonts NitroOS ships ([fix/dwrite-fallback](https://github.com/nitroos-org/reactos/commit/ce5e7e4d8ea))
- Cross-desktop SetParent fails with ERROR_INVALID_PARAMETER as Windows 10 (oracle) ([fix/setparent-cross-desktop](https://github.com/nitroos-org/reactos/commit/f5429657053))
- Per-desktop lookups instead of the input desktop, class clone chain and callprocs kept valid across desktops, deleted desktop's window freed in CSRSS's context ([fix/win32k-desktop-followups](https://github.com/nitroos-org/reactos/commit/aa7abb7f829))
- Desktop views released at process destroy (desktop+heap+winsta per sandboxed process), RemoveMenu submenu lock, exiting-thread TEB pointer cleared, input desktop handed back on delete, NULL ... ([fix/rotation-object-leaks](https://github.com/nitroos-org/reactos/commit/5d21d568e6f))
- IME/owned-popup windows linked only into their own desktop ([fix/win32k-desktop-lock-underflow](https://github.com/nitroos-org/reactos/commit/245fad78ab1))
- Win32k finds a thread's dirty windows on its own desktop, not the input desktop (windows stopped repainting after a desktop switch) ([fix/ghost-pixels-after-kill](https://github.com/nitroos-org/reactos/commit/806550a1824))
- User32 calls a hook proc in an already-mapped module without resolving it by path (kbswitch's global keyboard hook probed the file system on every key and queued behind disk writers ... ([fix/input-lag-disk-write](https://github.com/nitroos-org/reactos/commit/5f222f739a2))
- Win+Left/Right/Up/Down and Win+Shift+Up tile every top-level window (the hotkey is consumed and the snap runs in the window's thread) ([feat/win-arrow-snap-global](https://github.com/nitroos-org/reactos/commit/6dc9086aef0))
- Original CC0 caption-button glyph font substituted for Segoe MDL2 Assets / Segoe Fluent Icons (Firefox titlebar buttons) ([fix/firefox-caption-buttons](https://github.com/nitroos-org/reactos/commit/a225c02f271))
- EndTask no longer blocks on a hung window (not-responding dialog after 5 s), taskmgr End Task off the UI thread, SendMessageTimeout deadline covers the whole call, featmgr keeps its busy ... ([fix/hung-window-senders](https://github.com/nitroos-org/reactos/commit/df1eb405d5e))
- IntHideDesktop takes no reference to the desktop window (switched desktops leaked their window and USER handle ([fix/desktop-close-window-leak](https://github.com/nitroos-org/reactos/commit/acb8bc63ad4))
- EndTask as Windows 10 (invalid/other-session/non-GUI caller → FALSE/6, unforced waits for the window, forced ends the process under the caller's PROCESS_TERMINATE access) ([fix/endtask-parity](https://github.com/nitroos-org/reactos/commit/b04208876cb))
- GdiPoolAllocate gives the slot back when committing its pages fails (slots leaked under commit pressure, later DCs/brushes/regions got no user attribute ([fix/gdi-pool-dcattr](https://github.com/nitroos-org/reactos/commit/fb14b7662b5))
- Gdi32 tolerates a DC without a DC_ATTR in CreateCompatibleBitmap, CancelDC, Get/SetBrushOrgEx, GetColorSpace (kbswitch tray icon crash) ([fix/kbswitch-gdi-crash](https://github.com/nitroos-org/reactos/commit/d0344042ce1))
- Reuse the EDIT class CallProc instead of adding one per EDIT/COMBOBOX window (one USER object leaked per window) ([fix/edit-child-user-object-leak](https://github.com/nitroos-org/reactos/commit/a2358e5ae0b))
- CALLWNDPROC(RET) CallNextHookEx calls the next hook (it re-ran the current one), WH_DEBUG along CallNextHookEx chains, HCBT_MOVESIZE message order, keyboard size edges/step, injected mouse ... ([fix/win32k-hook-parity-leftovers](https://github.com/nitroos-org/reactos/commit/e9be035d03d))
- HCBT_MOVESIZE sets/cancels the final rect, keyboard move/size, HCBT_ACTIVATE hWndActive, WH_FOREGROUNDIDLE before waits, MSLL supplied time, WH_DEBUG ([fix/win32k-hook-parity](https://github.com/nitroos-org/reactos/commit/b4a049e2232))
- Free the desktop heap and desktop windows on delete, drop leaked window references, no desktop handle leak per SwitchDesktop ([fix/win32k-desktop-heap-leak](https://github.com/nitroos-org/reactos/commit/73a28daeca5))
- 19 user32 exports Windows 10 has (physical-coordinate and DPI helpers, dialog DPI behaviour, GetIconInfoEx, CreateDesktopEx, ShutdownBlockReason*, pointer device queries) ... ([fix/user32-missing-exports](https://github.com/nitroos-org/reactos/commit/0c4e80abd31))
- Gdi32 exports the 40 Uniscribe Script* functions as forwarders to usp10, as Windows 10 does — mingw-w64 binaries that link Script* from GDI32 load ([fix/gdi32-uniscribe-forwarders](https://github.com/nitroos-org/reactos/commit/652bf53db71))
- Wined3d reports the Microsoft Basic Render Driver (1414:008c) for a software GL renderer (llvmpipe/softpipe/swrast), as Windows without a display driver — Chromium/Electron apps choose ... ([fix/electron-generic-swcomp](https://github.com/nitroos-org/reactos/commit/253f20c2b1a))
- GetSystemMetrics SM_MOUSEPRESENT/SM_CMOUSEBUTTONS/SM_MOUSEWHEELPRESENT keep the attached mouse across metrics re-initialisation ([fix/win32k-sm-mousepresent](https://github.com/nitroos-org/reactos/commit/9a686799302))

</details>

<details><summary>Console/csrss (24 fixes)</summary>

- Console-linked programs that own windows get WM_QUERYENDSESSION/WM_ENDSESSION from usersrv at shutdown (consrv no longer kills them first), logoff sends ENDSESSION_LOGOFF ([fix/smalls14](https://github.com/nitroos-org/reactos/commit/83c830669f6))
- The console filters pasted text as Windows 10 does with FilterOnPaste (tabs removed, curly quotes, dashes and non-breaking spaces made plain) ([fix/pastefilter](https://github.com/nitroos-org/reactos/commit/063b98c8aeb))
- The console copies with Ctrl+C when text is selected (otherwise Ctrl+C still breaks), pastes with Ctrl+V, selects all with Ctrl+A and selects with Shift+arrows, as Windows 10 does while the ... ([fix/conkeys](https://github.com/nitroos-org/reactos/commit/fdbde95bd53))
- The console selects with the mouse out of the box (QuickEdit on by default, HKCU\Console defaults as Windows 10) ([fix/quickedit](https://github.com/nitroos-org/reactos/commit/fd9b9f835f0))
- The pseudoconsole host hands the terminal the rows a client printed before the first paint after a start or resize (they were in the server's ring but dropped) ([fix/conpty-early-scrollback](https://github.com/nitroos-org/reactos/commit/a00d3ee13c0))
- Wide characters take two cells in every code page (as Windows 10) ([fix/conpty-wide-char](https://github.com/nitroos-org/reactos/commit/2615e0834f3))
- 256-color and 24-bit colors through the pseudoconsole (CONPTY-colors-quantized-to-16) ([fix/conpty-extended-colors](https://github.com/nitroos-org/reactos/commit/05fe5bf57a5))
- The console server keeps rows that scroll off a buffer for the ConPTY host, which feeds them to the terminal's own scrollback in order at any output speed ([fix/conpty-scrollback](https://github.com/nitroos-org/reactos/commit/f78bf8897f6))
- ConPTY host paints the active screen buffer (nano), sends surrogate pairs as one UTF-8 character, Ctrl+letter keys carry the control character, non-BMP input as two key events ([fix/conpty-surrogate-active-buffer](https://github.com/nitroos-org/reactos/commit/b2776180fd1))
- Featmgr sets VS Code windowsUseConptyDll=false for Scoop vscode installs (TODO CONSOLE-condrv-device) ([fix/scoop-vscode-conpty-default](https://github.com/nitroos-org/reactos/commit/8dc14f3a16a))
- CreatePseudoConsole ignores flags it does not use (as Windows), conhost waits for a terminal that connects its pipe late (node-pty / VS Code terminal) ([fix/vscode-conpty-launch](https://github.com/nitroos-org/reactos/commit/ade562701a0))
- Default console 120x30 on a 120x9001 buffer as on Windows 10 (initial window cut to the largest the display allows), perfdata reads ready threads through the NDK structs ([fix/pagefile-info-console-defaults](https://github.com/nitroos-org/reactos/commit/0553e38946f))
- ENABLE_VIRTUAL_TERMINAL_INPUT — keys enter the input buffer as Windows 10 VT sequences (DECCKM, modifiers, F-keys, Alt prefix), ConPTY passes terminal input through unchanged ([feat/console-vt-input](https://github.com/nitroos-org/reactos/commit/5a5bc97669c))
- A console without a visible window is not limited to the monitor-derived window size, and the headless conhost reads the screen in bands (ConPTY terminals beyond ~100x34 were blank) ([fix/conpty-large-window](https://github.com/nitroos-org/reactos/commit/b9bd829399f))
- Consrv binds a pseudoconsole host to the process that created it with a never-recycled creation serial instead of the recyclable parent process ID ([fix/conpty-attach-binding](https://github.com/nitroos-org/reactos/commit/795a5caded6))
- Pseudoconsole parent-side pipe ends not inherited (close 3 s → ~16 ms), Windows WORD signal frames (sshd resize works), Ctrl+C raises CTRL_C_EVENT, forged-host apitest ([fix/conpty-followups](https://github.com/nitroos-org/reactos/commit/da725243740))
- Pseudoconsole support (CreatePseudoConsole/ResizePseudoConsole/ClosePseudoConsole) with a headless conhost ([fix/conpty](https://github.com/nitroos-org/reactos/commit/bb7d46960c3))
- Perfdata System\Processor Queue Length counter (btop4win died on PDH_CSTATUS_NO_COUNTER via an uncaught C++ exception) ([fix/btop-console](https://github.com/nitroos-org/reactos/commit/e323c6a2ed3))
- End console processes after CTRL_CLOSE (handled or 5 s timeout, STATUS_CONTROL_C_EXIT), Alt+F4 closes a console window ([fix/console-close-and-altf4](https://github.com/nitroos-org/reactos/commit/86b12841a83))
- A new console process keeps its parent's console alive until it connects — 'start /b program' at the end of a batch file no longer kills the program ([fix/console-start-b-orphan](https://github.com/nitroos-org/reactos/commit/c134148ecd5))
- A "cmd /C" child no longer repaints the console (colours already on screen survive) ([fix/cmd-keep-console-colors](https://github.com/nitroos-org/reactos/commit/0902724adff))
- HKEY_PERFORMANCE_DATA reports no subkeys or class, so Registry:: listing and registry tab completion work in PowerShell 7 ([fix/ps7-pester-console-host](https://github.com/nitroos-org/reactos/commit/be307e81e08))
- Console screen buffers interpret virtual terminal sequences (SGR colours, cursor movement, erase, scroll region, alternate screen, title) ([fix/consrv-vt-processing](https://github.com/nitroos-org/reactos/commit/8f4f0c77e2d))
- FindNLSStringEx zero-length source/value semantics as Windows 10 (ConsoleCtrlEventRace: no hang on main) ([fix/consolectrl-race-findnls](https://github.com/nitroos-org/reactos/commit/c86a7fb07f8))

</details>

<details><summary>WoW64 (52 fixes)</summary>

- Legide leaves the legacy IDE channels a compatibility-mode PCI IDE controller serves (since 14ce32b653f it claimed IRQ 14/15 first, pciide could not connect and the bootcd live setup ran on ... ([fix/relfix](https://github.com/nitroos-org/reactos/commit/de36d8488b3))
- WoW64 syscall overhead ~245 -> ~92 ns: plain services skip FXSAVE (APC dispatch and Get/SetThreadContext take the live x87/SSE state), plain stores instead of locked exchanges, far-jump ... ([fix/wowsc](https://github.com/nitroos-org/reactos/commit/c49d25b2402))
- Register the remaining SysWOW64 COM servers (DirectDraw/DirectSound/DirectInput/UI Automation/MSHTML/HTML Help/ADSI) in the 32-bit view ([fix/wow64-com-registration](https://github.com/nitroos-org/reactos/commit/efe69c3e44b))
- IsWow64GuestMachineSupported ([fix/kernel32-wow64-parity](https://github.com/nitroos-org/reactos/commit/8a7536584d6))
- Windows.UI.ViewManagement.UISettings for 32-bit processes (SysWOW64 windows.ui.dll + Wow6432Node activation) ([fix/winrt-uisettings-wow64](https://github.com/nitroos-org/reactos/commit/1bd46aab8c2))
- WoW64 SystemExtendedHandleInformation in the 32-bit layout (32-bit Restart Manager sees file-holding processes) ([fix/wow64-handleinfo](https://github.com/nitroos-org/reactos/commit/236fa8e1761))
- A WoW64 process keeps the logical System32 DllPath (modules report System32 as on Windows), winscard answers like Windows with the Smart Card service stopped ([fix/eid-cardapi-crash](https://github.com/nitroos-org/reactos/commit/7ee789bc5e0))
- A new WoW64 thread starts with reset debug and FP state, 64-bit user EFLAGS sanitized as Windows reads them back ([fix/thread-initial-context](https://github.com/nitroos-org/reactos/commit/f712ac09480))
- WoW64 start-context entry computed from kernel-owned section data only (relocation-safe), discriminating edited-PEB and relocated-image tests, guest stack commit clamp ([fix/wow64-context-followups](https://github.com/nitroos-org/reactos/commit/42edb91b791))
- WoW64 GetThreadContext in 64-bit code and before the guest runs, as on Windows (no retry outside the transition code, kernel-staged main-thread start context at ntdll32!RtlUserThreadStart ... ([fix/wow64-context-x64-mode](https://github.com/nitroos-org/reactos/commit/38714078968))
- WoW64 CreateProcess carries a 32-bit process's pseudoconsole (native-width copy of the HPCON) instead of failing with ERROR_NOT_SUPPORTED ([fix/conpty-wow64](https://github.com/nitroos-org/reactos/commit/a6ea5b63d38))
- WoW64 guest EFlags drop NT/IOPL (IRETQ with NT faulted forever), running guest flags as Windows ([fix/wow64-ntcreatethread-hang](https://github.com/nitroos-org/reactos/commit/4b8105e6ee7))
- User-mode debug API for 32-bit debuggers under WoW64 (wait-event conversion, shared debug object, 32-bit module events, Dbgk PE32 start address) ([fix/wow64-debug-api](https://github.com/nitroos-org/reactos/commit/3d14d4ec07a))
- TEB.WowTebOffset (+0x2000 / -0x2000) for WOW64 threads ([fix/wow64-teb-offset](https://github.com/nitroos-org/reactos/commit/cebaa726713))
- NtSetSystemInformation for the time zone and time adjustment classes under WoW64 ([fix/wow64-set-timezone](https://github.com/nitroos-org/reactos/commit/faf95a0a396))
- Gditools reads the GDI table via the 64-bit PEB under WoW64 ([fix/wow64-gdi32-dcattr](https://github.com/nitroos-org/reactos/commit/6fb304bae58))
- SCSI/ATA pass-through for 32-bit callers (private event/IOSB so the storage drivers see a 32-bit request and parse the 32-bit layout, copied-and-validated header) ([fix/wow64-scsi-ata-passthrough](https://github.com/nitroos-org/reactos/commit/99fa2ac0a4a))
- 336 pointer-free I/O and file system control codes pass unchanged for 32-bit callers, 3 FSCTLs translated (handle widening) ([fix/wow64-ioctl-allowlist](https://github.com/nitroos-org/reactos/commit/d9a9e3ca0fc))
- Translate EndTask, SoundSentry and seven console window CSR requests for 32-bit processes ([fix/wow64-csr-unsupported-apis](https://github.com/nitroos-org/reactos/commit/c0f55a27a9d))
- Ship drwtsn32 in SysWOW64 (Windows reports 32-bit crashes with SysWOW64\WerFault), classic fallback box honours DontShowUI and invisible window stations ([fix/wow64-crash-reporter](https://github.com/nitroos-org/reactos/commit/fb099bab9dd))
- Forward the UserpExitWindowsEx CSR request for 32-bit processes (ExitWindowsEx failed with error 50), reboot/power-off count as shutdown for the console event ([fix/wow64-exitwindowsex](https://github.com/nitroos-org/reactos/commit/a85e8779545))
- CTRL_SHUTDOWN_EVENT at shutdown and services kept when they handle it ([fix/console-logoff-and-wow64-ctrl](https://github.com/nitroos-org/reactos/commit/bf4e0a017d8))
- A failing extra SysWOW64 COM registration is logged, not a setup failure ([fix/syssetup-wow64-extras-nonfatal](https://github.com/nitroos-org/reactos/commit/aceacc74ca4))
- Ship 146 more i386 DLLs and tools in SysWOW64 (all in Windows 10 SysWOW64), register 36 more COM servers in the 32-bit view ([fix/syswow64-missing-dlls](https://github.com/nitroos-org/reactos/commit/cb79ad8808b))
- Harden Wow64WinCallNextHook (mouseData read guarded, zeroed record, cached hook id), HookFamilies chained-hook coverage ([fix/wow64-thread-hooks-followups](https://github.com/nitroos-org/reactos/commit/5f88c11be4a))
- 32-bit processes can set every thread hook family (CBT, MSGFILTER, KEYBOARD, MOUSE, LL, SHELL, FOREGROUNDIDLE) ([fix/wow64-thread-hooks](https://github.com/nitroos-org/reactos/commit/dd765e9f21a))
- Thunk NtGetCurrentProcessorNumber(Ex) for 32-bit processes ([fix/wow64-processor-number](https://github.com/nitroos-org/reactos/commit/0c764d706d6))
- Msi loads apphelp from the process's own system directory (no delay-load through a redirection-disabled System32) and keeps redirection off for every package in a WoW64 client, as Windows ... ([fix/wow64-apphelp-checkpackage](https://github.com/nitroos-org/reactos/commit/f117b9b070e))
- System16Folder is unset on 64-bit systems as on Windows ([fix/msi-wow64-folders](https://github.com/nitroos-org/reactos/commit/6920a4e8b88))
- IsWow64Process2, CalculatePopupWindowPosition, GetAppContainerNamedObjectPath, CompareObjectHandles (NtCompareObjects), CM_Get_DevNode_PropertyW, CancelSynchronousIo ... ([fix/app-import-exports-p1](https://github.com/nitroos-org/reactos/commit/f2586a2343f))
- WoW64 thunks for async NtFlushBuffersFile, pipe impersonation/connection-attribute FSCTLs, DefineDosDevice CSR message, NtFlushVirtualMemory, NtQuery/SetEaFile and mount-manager/storage ... ([fix/wow64-pipe-file-gaps](https://github.com/nitroos-org/reactos/commit/2fca6742e04))
- Wined3d moves contexts off a swapchain being destroyed (Direct2D DC render targets drew nothing after an HWND target was released) ([fix/d2d1-dcrt-wow64](https://github.com/nitroos-org/reactos/commit/2189df99b1f))
- WoW64 converts the file information classes whose 32-bit layout differs (rename/link/completion/tracking and friends, guest fields read once) and passes the rest through ([fix/wow64-file-info-classes](https://github.com/nitroos-org/reactos/commit/985cad4cc27))
- Dxgi_apitest EnumWarpAdapter no longer skips DXGI_ERROR_UNSUPPORTED under WoW64 and checks D3D11 device creation on adapter 0 ([fix/wow64-dxgi-32bit](https://github.com/nitroos-org/reactos/commit/3f7074a0ad0))
- WoW64 passes NtQuerySystemInformation(SystemProcessorPerformanceInformation) through ([fix/perfdata-wow64-cpu](https://github.com/nitroos-org/reactos/commit/83240c4a358))
- 32-bit OpenGL under WoW64 — Mesa3D registered in the Wow6432Node view (as Windows registers 32-bit ICDs) and i386 llvmpipe in SysWOW64 (staged from the pinned artifact, softpipe fallback) ... ([fix/wow64-32bit-mesa-gl](https://github.com/nitroos-org/reactos/commit/220b15cdd06))
- WoW64 thunks for NtCancelIoFileEx, NtNotifyChangeDirectoryFile and FileIoCompletionNotificationInformation — 32-bit CancelIoEx, ReadDirectoryChangesW, SetFileCompletionNotificationModes ([fix/wow64-io-thunks](https://github.com/nitroos-org/reactos/commit/2ac8531f317))
- WoW64 thunks the 32-bit MSV1_0_S4U_LOGON submit buffer (pointers or offsets, captured once) and the LM20 logon profile reply — 32-bit S4U logons ([fix/wow64-lsa-s4u-thunk](https://github.com/nitroos-org/reactos/commit/2fd1c627cfd))
- NtContinue and RtlRestoreContext restore only the register groups a partial CONTEXT names, return to the caller without CONTEXT_CONTROL and return STATUS_SUCCESS without CONTEXT_INTEGER, as ... ([fix/ntcontinue-partial-context](https://github.com/nitroos-org/reactos/commit/3ec74532c43))
- WoW64 NT service thunks capture the guest argument block once (per-service counts), Wow64RaiseException reads NumberParameters once, CSR/BASESRV/LSA port-name capture-once ([fix/wow64-thunk-single-fetch](https://github.com/nitroos-org/reactos/commit/433c9248035))
- Amd64 CONTEXT_XSTATE (AVX/YMM) through Get/SetThreadContext, NtContinue, NtRaiseException, user exception dispatch, RtlRestoreContext and WoW64 guest contexts ([fix/kernel-context-xstate](https://github.com/nitroos-org/reactos/commit/4b4a5b5b323))
- WoW64 converts AFD_GET_CONTEXT by ioctl code only, so other AFD requests with a 140-byte output buffer are no longer mistaken for it ([fix/wow64-afd-getcontext](https://github.com/nitroos-org/reactos/commit/f67afb6396c))
- Ship and register the 32-bit WMI client (wbemprox, wbemdisp, wmiutils) and Windows Script Host (cscript, wscript, vbscript, jscript, scrrun, wshom) in SysWOW64 — 32-bit WMI clients and ... ([fix/wow64-wbemlocator](https://github.com/nitroos-org/reactos/commit/754ae60adaf))
- A relative registry open without a view flag keeps the view of its parent handle (kernel remembers it per key handle ([fix/wow64-registry-relative-view](https://github.com/nitroos-org/reactos/commit/ff86918c3aa))
- 32-bit callers under WoW64 get service enumerations, QueryServiceConfig2 and QueryServiceLockStatus in the 32-bit layout ([fix/advapi32-wow64-scm-structs](https://github.com/nitroos-org/reactos/commit/4f003d377f1))
- 32-bit console programs run their own Ctrl+C/Break/close handlers under WoW64 instead of being terminated ([fix/wow64-console-ctrl-handler](https://github.com/nitroos-org/reactos/commit/7f63ba8ad6a))
- WoW64 translates every console CSR request (input, cursor, attributes, fill/read/write output, scroll, title, aliases, history, AllocConsole), so 32-bit console and TUI programs work ([fix/wow64-console-csr](https://github.com/nitroos-org/reactos/commit/4121d72a9dc))
- WTHelperGetFileHash (wintrust) and ResolveDelayLoadedAPI (ntdll/kernel32) — 32-bit PowerShell no longer hangs on an Entry Point Not Found box when loading the Security module ([fix/wow64-get-acl-hang](https://github.com/nitroos-org/reactos/commit/7585c3c383f))
- WoW64 stores NULL in the 32-bit handle output of a failed open/create/duplicate (as Windows' WoW64 does), so 32-bit code can't close a stale live handle ([fix/wow64-null-handle-on-fail](https://github.com/nitroos-org/reactos/commit/0b87823cba6))
- 16 more 32-bit DLLs in SysWOW64 that .NET assemblies P/Invoke (httpapi, authz, netutils, activeds, odbc32, ...) ([fix/syswow64-dotnet-pinvoke](https://github.com/nitroos-org/reactos/commit/b254e94da23))
- 32-bit PowerShell 7 under WoW64 (SysWOW64 mscoree, sspicli, wevtapi ([fix/wow64-pwsh-x86](https://github.com/nitroos-org/reactos/commit/8f05466db03))
- SysWOW64 uiautomationcore, d2d1 + deps ([fix/syswow64-uia-d2d1](https://github.com/nitroos-org/reactos/commit/8150208fe9f))

</details>

<details><summary>Networking (48 fixes)</summary>

- Security hardening (group G7): fixes in this area, described generically ([sec/g7-net](https://github.com/nitroos-org/reactos/commit/d7fedf4cfbe))
- Firewall filter follow-ups - per-port address file index (inbound owner lookup ~7x cheaper on closed ports), address files pin their creator process (no PID-reuse attribution), interface ... ([fix/fwfu](https://github.com/nitroos-org/reactos/commit/a7e564482ec))
- WSASendMsg extension pointer with UDP_SEND_MSG_SIZE segmentation and IP_ECN, IP/UDP socket options as Windows answers them ([fix/wsamsg](https://github.com/nitroos-org/reactos/commit/69ed5bed245))
- Firewall block prompt (fwalert) when a blocked user program starts listening ([fix/fwprompt](https://github.com/nitroos-org/reactos/commit/8bded0ab56a))
- Wf.msc opens NitroOS Firewall with Advanced Security (wfadv.exe, standalone) ([fix/wfmsc](https://github.com/nitroos-org/reactos/commit/9e5163a8926))
- NitroOS firewall enforces by default ([fix/fwdefault](https://github.com/nitroos-org/reactos/commit/af7e08af230))
- Firewall.cpl, the NitroOS Firewall control panel (status per profile, turn on/off and block all incoming, notification setting, allowed apps with Private/Public, restore defaults, Advanced ... ([fix/fwcpl](https://github.com/nitroos-org/reactos/commit/b6e65aa4474))
- AFD keeps the send-window bytes of a cancelled or immediate send as ghosts in window order, so each queued send completes when its own bytes are sent (they were counted against the next ... ([fix/afdsend](https://github.com/nitroos-org/reactos/commit/ebbb565b481))
- Set-NetIPInterface sets the interface metric (persistent ([fix/ps7net](https://github.com/nitroos-org/reactos/commit/be37d235fe8))
- NitroOS firewall (enforcement still off by default): local traffic is recognised by the frames our own send path loops back, not by the source MAC alone (a spoofed frame with our MAC and IP ... ([fix/fwnext](https://github.com/nitroos-org/reactos/commit/6b9488cd275))
- WM_WINDOWPOSCHANGED gives a child window its position relative to its parent, as Windows does (it was screen coordinates) ([fix/netui-msg](https://github.com/nitroos-org/reactos/commit/ba707483b33))
- NitroOS firewall filter fixes before default-on (enforcement stays off by default): traffic from our LAN address to itself passes, a registry read failure keeps the previous state instead ... ([fix/fwprep](https://github.com/nitroos-org/reactos/commit/92ed4cac14b))
- The TCP receive window closes when the owner does not read, as on Windows (unread data stays bounded instead of growing nonpaged pool without limit), autotunes from 2 MB to 32 MB for an ... ([fix/tcp-receive-window](https://github.com/nitroos-org/reactos/commit/0cd164c5788))
- Revert "Merge fix/tcp-receive-window" (4de549f4a4c): i386 Debug double fault (unbounded DispTdiReceive/RefillSocketBufferEx recursion) and shutdown(SD_SEND) blocking ([cd06bbc45bd](https://github.com/nitroos-org/reactos/commit/cd06bbc45bd))
- The TCP receive window closes while the application does not read and reopens as it reads (a non-reading receiver buffered without limit) ([fix/tcp-receive-window](https://github.com/nitroos-org/reactos/commit/4de549f4a4c))
- Bind conflicts decided per socket pair as Windows 10 (SO_REUSEADDR/SO_EXCLUSIVEADDRUSE, wildcard vs specific ([fix/winsock-quirks](https://github.com/nitroos-org/reactos/commit/e8699cdefb9))
- Msafd no longer keeps or closes AFD's kernel TDI handle values (every connected socket made failed NtCloses at closesocket) ([fix/msafd-context-handles](https://github.com/nitroos-org/reactos/commit/5616fc4a67b))
- Shutdown(SD_RECEIVE) with unread or late data aborts the connection with an RST and later operations fail with WSAECONNABORTED (oracle) ([fix/afd-sdreceive-reset](https://github.com/nitroos-org/reactos/commit/43716d47dcc))
- Loopback answers for all of 127.0.0.0/8, accepted socket reports the SYN's destination address, getsockname before SO_UPDATE_ACCEPT_CONTEXT is WSAEINVAL (oracle) ([fix/tcp-loopback-accept](https://github.com/nitroos-org/reactos/commit/abdd0c9d25c))
- Concurrent first lookups no longer fail with WSAHOST_NOT_FOUND (shared protocol database open), getaddrinfo answers IPv6 literals without an IPv6 stack, no FD_CONNECT for a connect already ... ([fix/ws2-eid-followups](https://github.com/nitroos-org/reactos/commit/31de1890a1d))
- Connect on a connected/connecting socket returns WSAEISCONN/WSAEALREADY as Windows, WSAAsyncSelect no longer loses FD_READ (FD_CONNECT before FD_WRITE), netprofm NetworkListManager class ... ([fix/eid-socket-timeout](https://github.com/nitroos-org/reactos/commit/019cc8089c7))
- Tcpip inbound firewall filter (\Device\NitroFw, policy from mpssvc), enforcement off by default (NitroFwEnforce=1 opt-in ([feat/firewall-inbound-filter](https://github.com/nitroos-org/reactos/commit/62cff6bc15a))
- An accepted TCP socket reports its concrete local address (getsockname, AcceptEx sockaddrs) instead of the listener's 0.0.0.0 ([fix/afd-accepted-getsockname](https://github.com/nitroos-org/reactos/commit/801dd2735b5))
- Ipconfig output, usage, switches and exit codes as Windows 10 22H2 (header 'NitroOS IP Configuration') ([fix/ipconfig-1to1](https://github.com/nitroos-org/reactos/commit/7cf827f57dc))
- Firewall rules in the Windows registry format, mpssvc service, INetFwPolicy2 backed by the store, netsh advfirewall (Windows output) ([feat/firewall-rule-store](https://github.com/nitroos-org/reactos/commit/e20cab90587))
- OnLinkPrefixLength, DDNS flag, loopback Mtu, link speeds, GetPerAdapterInfo DNS mask ([fix/iphlpapi-parity](https://github.com/nitroos-org/reactos/commit/b97e367391a))
- Netsh interface portproxy (byte-identical to Windows 10 22H2) and the iphlpsvc TCP relay service ([feat/netsh-portproxy](https://github.com/nitroos-org/reactos/commit/a71df70ac68))
- GetPerAdapterInfo answers the loopback and sizes/fields as Windows (.NET GetAllNetworkInterfaces regression from d7c004388c8) ([fix/iphlpapi-peradapterinfo-loopback](https://github.com/nitroos-org/reactos/commit/d1c3393950a))
- GetAdaptersAddresses sets IP_ADAPTER_IPV4_ENABLED as Windows (.NET GetIPv4Properties no longer throws) ([fix/dotnet-getipv4properties](https://github.com/nitroos-org/reactos/commit/d7c004388c8))
- TransmitFile/TransmitPackets extension functions (Python asyncio), WSAStringToAddress returns IPv4 address and port in network order ([fix/msafd-transmitfile](https://github.com/nitroos-org/reactos/commit/058db7f43f9))
- Msafd SIO_ADDRESS_LIST_CHANGE (blocking, non-blocking with FD_ADDRESS_LIST_CHANGE, overlapped) and SIO_ADDRESS_LIST_QUERY as Windows (.NET NetworkChange, KROS) ([fix/sio-address-list-change](https://github.com/nitroos-org/reactos/commit/3d77232b5d9))
- The HTTP Server API (httpapi.dll v1+v2) served by an in-process HTTP/1.1 server with IOCP-compatible completions — System.Net.HttpListener works for .NET Framework 4.8 (32/64-bit) and .NET ... ([feat/httpapi-inprocess-server](https://github.com/nitroos-org/reactos/commit/cd6859f2ab1))
- Schannel SECPKG_ATTR_ENDPOINT_BINDINGS/UNIQUE_BINDINGS (RFC 5929 channel bindings as Windows 10 returns them) — .NET Framework channel-binding callers (KROS ALFA plus login) no longer fail ... ([fix/schannel-dotnet-sslstream](https://github.com/nitroos-org/reactos/commit/870aa5d1caa))
- A standard user can ping (Device\Ip grants Everyone execute, as on Windows) and WMI Win32_Process reports processes the caller cannot open (btop as non-admin) ([fix/nonadmin-btop-ping](https://github.com/nitroos-org/reactos/commit/b4c12091de0))
- Net.exe exit codes and error text per Windows (2 on failure, 1 on syntax), net localgroup lists all groups ([fix/net-exe-exit-codes](https://github.com/nitroos-org/reactos/commit/772a8475451))
- WsProcDelete closes the catalog events (2 Event handles leaked per WSAStartup/WSACleanup cycle ([fix/dnscache-handle-leak](https://github.com/nitroos-org/reactos/commit/a5509fa54fc))
- Clean-room NetAdapter, NetTCPIP and DnsClient modules for PowerShell 7 (Windows type names, property sets, views and errors measured on the oracle) ([fix/ps7-net-modules](https://github.com/nitroos-org/reactos/commit/876a191296d))
- Shutdown(SD_RECEIVE/SD_BOTH) keeps pending receives and later data completes them, new receives fail WSAESHUTDOWN, sends keep working after SD_RECEIVE, repeated shutdown succeeds — as on ... ([fix/afd-shutdown-receive-pending](https://github.com/nitroos-org/reactos/commit/ba699c02ad3))
- Msafd posts the completion-port packet when an event is given (unless its low bit is set), closing a stream socket aborts pending receives with STATUS_CONNECTION_ABORTED ... ([fix/io-native-iocp-gaps](https://github.com/nitroos-org/reactos/commit/b398724c73c))
- Interfaces are numbered from 1 as on Windows (loopback = 1), route table and iphlpapi route/neighbor lookups use the real interface index, route add works ([fix/loopback-ifindex](https://github.com/nitroos-org/reactos/commit/4cc63ec2704))
- [IPHLPAPI] GetAnycastIpAddressTable (empty table, as Windows on a host without anycast addresses) — JDK 21 NetworkInterface/SecureRandom, NetBeans ([fix/iphlpapi-anycast-table](https://github.com/nitroos-org/reactos/commit/36810282be8))
- Overlapped stream sends complete whole also on non-blocking sockets, as on Windows ([fix/afd-whole-send-followups](https://github.com/nitroos-org/reactos/commit/17c6147de02))
- Event and completion-port overlapped WSASend on a stream socket completes with the whole length as on Windows (AFD keeps the request queued and refills the send window) ([fix/afd-whole-overlapped-send](https://github.com/nitroos-org/reactos/commit/2cbe3864c10))
- GetAdaptersAddresses fills FirstGatewayAddress for GAA_FLAG_INCLUDE_GATEWAYS — .NET NetworkInterface gateway addresses work ([fix/iphlpapi-gateway-address](https://github.com/nitroos-org/reactos/commit/b227dd2b6a3))
- Reverse DNS lookups work — dnsapi PTR queries, full PTR/CNAME/NS names over the resolver RPC, type-aware resolver cache, mswsock NSP answers getnameinfo and gethostbyaddr, NI_NOFQDN ([fix/ws2-reverse-lookup](https://github.com/nitroos-org/reactos/commit/349dccc0df8))
- Win32-OpenSSH sshd works with password logons (cmd, scp, sftp, PowerShell remoting over SSH) — LogonUserExExW and network-cleartext logon, services log on with the real password read from ... ([fix/openssh-m1](https://github.com/nitroos-org/reactos/commit/1cf18d72f67))
- ApplyControlToken retry flushes instead of duplicating the alert ([fix/schannel-reland-nits](https://github.com/nitroos-org/reactos/commit/fc16c5440ce))
- Re-land SCH_CREDENTIALS pieces (TLS_PARAMETERS mapping, protocol-mask enforcement, ApplyControlToken, IP-literal SNI, version-refused codes, tests) with the root-cause fix (read_config ... ([fix/schannel-v5-reland](https://github.com/nitroos-org/reactos/commit/7932c258e2f))

</details>

<details><summary>Audio/multimedia (3 fixes)</summary>

- Featmgr add-on "aac" (full AAC: LC, HE-AAC, HE-AAC v2 from unpatched FFmpeg 9.0.2's native decoder, as Fedora's ffmpeg-free ships it) next to the LC-only "aac-lc" ([fix/aacfull](https://github.com/nitroos-org/reactos/commit/91cc3e45f7f))
- Opt-in featmgr add-on "AAC audio decoder (AAC-LC)": a Media Foundation AAC decoder transform built from FFmpeg's aac decoder with SBR/PS/USAC removed (Fedora fdk-aac-free scope), hosted ... ([fix/aacmft](https://github.com/nitroos-org/reactos/commit/a7b869e3ed9))
- Featmgr ships the Firefox policy media.wmf.media-engine.enabled=0 (user-changeable) so mp4/H.264 plays out of the box ([fix/firefox-mp4-defaults](https://github.com/nitroos-org/reactos/commit/09f2ad6ed0c))

</details>

<details><summary>Storage/USB/PnP/drivers (20 fixes)</summary>

- Usbport allocates the adapter channel for one transfer at a time (IoAllocateAdapterChannel keeps its WCB in the FDO: two CPUs flushing at once overwrote each other's context, mapped one ... ([fix/wizhang](https://github.com/nitroos-org/reactos/commit/e0ae2c601d5))
- I386 Debug installs again: DrvInstLock is a mutex (a critical section held across a drvinst run hit Debug's CS timeout), and TOKEN_USER buffers are ULONG-aligned (a misaligned stack buffer ... ([fix/drvlock](https://github.com/nitroos-org/reactos/commit/c6cb2941765))
- A driver image load takes its pages only when enough are usable (waits for the balancer, bounded, then fails with STATUS_INSUFFICIENT_RESOURCES): MiRemoveAnyPage's page 0 was mapped as a ... ([fix/mm7e](https://github.com/nitroos-org/reactos/commit/0511536af8a))
- Usbport pops MapTransferList under MapTransferSpinLock (two CPUs flushing at once mapped one transfer twice and linked it into the endpoint's TransferList twice: endless list walk or ... ([fix/usbspin](https://github.com/nitroos-org/reactos/commit/75d05db6be3))
- Security hardening: a group of fixes in this area; details are withheld until users have had time to update ([fix/regacl-r7d2](https://github.com/nitroos-org/reactos/commit/bc016520bff))
- Msxml3 schema cache parses the serialized schema as UTF-8 whatever its declaration says (regression from the Wine-11.21 sync 87360404d1f: IXMLDOMSchemaCollection::add rejected App-V's ... ([fix/offbis](https://github.com/nitroos-org/reactos/commit/4115bfceb3c))
- Setupapi no longer asks for a reboot after installing the same driver the kernel already started (tablet/hub hot-plug showed 'You need to reboot'), and restarts the device stack after any ... ([fix/pnpreenum](https://github.com/nitroos-org/reactos/commit/09660f6865a))
- The HAL's map buffers for 32-bit scatter/gather PCI bus masters above 4 GB are cached (PCI bus-master DMA is coherent on x86/x64 ([fix/cc-write-stalls](https://github.com/nitroos-org/reactos/commit/fbc0c82b639))
- Atapi recovery initializes its status on the timeout/reset path (a stale value could skip completing the failed request) ([fix/ide-stall-recovery](https://github.com/nitroos-org/reactos/commit/c1bc7fa4d7e))
- A request waiting for map registers when a map buffer grow failed is served by a deferred grow retry (10 ms - 1 s backoff) instead of waiting forever ([fix/storport-sg-timeout-r2](https://github.com/nitroos-org/reactos/commit/dc7015f6c91))
- A thread that faults on a cache view of a file whose FCB resource it already holds (NgRead -> CcCopyRead) no longer deadlocks on itself in MiNotPresentFaultSectionView (the 'I/O stall under ... ([fix/storage-io-stall](https://github.com/nitroos-org/reactos/commit/5a0dc7b20e9))
- Partmgr MOUNTDEV queries reject a buffer smaller than the structure (INVALID_PARAMETER), report the structure size on overflow, no suggested link name (as Windows) ([fix/deviceiocontrol](https://github.com/nitroos-org/reactos/commit/58bc0955c40))
- Security hardening (group G1): fixes in this area, described generically ([sec/g1-pnp-access](https://github.com/nitroos-org/reactos/commit/d187834ddb3))
- NVM Express storport miniport (stornvme), storport timeouts with reset/restart escalation, pass-through and timer fixes, HAL processor vectors skipped in the PnP IRQ conflict check ([halsg/nvme-stack](https://github.com/nitroos-org/reactos/commit/14ce32b653f))
- Main-fixes: HAL scatter/gather list of any length + Mm page-in VDL clamp ([halsg](https://github.com/nitroos-org/reactos/commit/34e9f425108))
- Atapi SATL mode data length per SPC (disk.sys rejected the caching page since T74, so no FLUSH CACHE ever reached the drive) ([fix/storage-flush-cache](https://github.com/nitroos-org/reactos/commit/7c851574758))
- Serial.sys takes the COMx name from the devnode's PortName ([fix/serial-com2-stable-names](https://github.com/nitroos-org/reactos/commit/a1372327f74))
- Clean-room Storage module for PowerShell 7 (Get-Volume, Get-Disk, Get-Partition, Get-PhysicalDisk ([fix/ps7-storage-module](https://github.com/nitroos-org/reactos/commit/8f1c41b0c77))
- The root enumerator answers IRP_MN_QUERY_CAPABILITIES with Windows' root capabilities and completes other unhandled minors instead of asserting ([fix/pnproot-query-capabilities](https://github.com/nitroos-org/reactos/commit/e3547154578))
- Floppy answers the mount manager's name and unique-ID queries without a media check, so an empty drive registers on arrival instead of 30 failing queries with head seeks — amd64 boot 4.3 s ... ([fix/mountmgr-floppy-opens](https://github.com/nitroos-org/reactos/commit/4e0f8436515))

</details>

<details><summary>Shell/explorer/common-controls (20 fixes)</summary>

- Comctl32 imagelist synced to Wine 11.21 (completes the comctl32 Wine-following files: comctl32_pending.cfg retired) ([fix/comctl32c](https://github.com/nitroos-org/reactos/commit/b9d853a3813))
- Comctl32 toolbar and propsheet synced to Wine 11.21 (toolbar built v5 + v6 like the other version-specific controls, ReactOS's ToolbarV6 class retired ([fix/comctl32b](https://github.com/nitroos-org/reactos/commit/42cc44e4e6f))
- Comctl32 controls that follow Wine synced to Wine 11.21 (listview, treeview, header, tab, tooltips, rebar, status, ... and their winetests) ([fix/comctl32-wine11](https://github.com/nitroos-org/reactos/commit/e8b84caaf50))
- A file renamed from cmd or PowerShell is renamed on the desktop and in folder windows (FsRtl kept a stale last-record offset when a directory read completed from buffered changes, so ... ([fix/deskrename](https://github.com/nitroos-org/reactos/commit/a6595c3f2ed))
- CreateProcessWithLogonW/runas starts GUI programs on the caller's desktop with the new logon SID granted there (and removed at exit), returns the process information ([fix/runas-gui-window](https://github.com/nitroos-org/reactos/commit/6bc0960099d))
- The Vista-style file dialog accepts forward slashes in typed paths, typed URLs are left alone in all comdlg32 dialogs ([fix/itemdlg-slashes](https://github.com/nitroos-org/reactos/commit/045efb86cbe))
- No Internet item in the Desktop folder enumeration while it is hidden (Windows 10 does not list it ([fix/file-dialog-polish](https://github.com/nitroos-org/reactos/commit/a7f8ee553c1))
- 139 Windows 10 known folder definitions, registered-folder GetPath/SetPath, redirection caps, shellpath winetest 0 failures ([fix/shell32-knownfolder-parity](https://github.com/nitroos-org/reactos/commit/a1317bb0786))
- CLSID_KnownFolderManager is a registered in-proc class of shell32 (ThreadingModel Both) as Windows ([fix/shell32-knownfoldermanager-coclass](https://github.com/nitroos-org/reactos/commit/d8814ce8619))
- Address bar runs command lines on any parse failure ([fix/explorer-addressbar-badpath](https://github.com/nitroos-org/reactos/commit/c644b4495bc))
- IAttachmentExecute (CLSID_AttachmentServices) in shdocvw, Zone.Identifier as Windows 10 writes it, Open File security warning for high-risk Internet files ([feat/attachment-services](https://github.com/nitroos-org/reactos/commit/e9635d242d2))
- Shell32 IFileOperation (CLSID_FileOperation was unregistered, so every Chrome download failed with FILE_FAILED before the file was moved into place) ([fix/chrome-download-error](https://github.com/nitroos-org/reactos/commit/16726127cae))
- One Control Panel item for the feature manager (appwiz.cpl), optionalfeatures.exe launcher, Programs and Features task link ([fix/cpl-features-single-entry](https://github.com/nitroos-org/reactos/commit/242cc097dc6))
- Runas never prompts ([fix/shell32-runas-nonadmin-no-dialog](https://github.com/nitroos-org/reactos/commit/bb686a4af33))
- FolderWindowResources tolerances and bounded close wait, SetDeskBarSite re-entry and failure-path cleanup, combase endpoint error logs the status ([fix/explorer-leak-nits](https://github.com/nitroos-org/reactos/commit/59ce17fa79d))
- Free the Folders-pane close-button image list and the folder window icons, one COM RPC endpoint per process (a pipe and an event leaked per apartment) ([fix/explorer-handle-leaks](https://github.com/nitroos-org/reactos/commit/20b7b7feff9))
- ShellExecute(Ex) report ERROR_NO_ASSOCIATION/SE_ERR_NOASSOC and ERROR_FILE_NOT_FOUND like Windows (rebased onto the landed runas fix) ([fix/shellexecute-noassoc-codes](https://github.com/nitroos-org/reactos/commit/770b4c1f2eb))
- "runas" from an elevated caller (or with SEE_MASK_FLAG_NO_UI) starts the target with the caller's own token like Windows with UAC off ([fix/shell32-runas-admin-no-prompt](https://github.com/nitroos-org/reactos/commit/7e194e75a4a))
- Explorer.exe hands a file or shortcut on its command line to the running shell, which starts it with SW_SHOWDEFAULT as Windows does ([fix/explorer-lnk-forward-showdefault](https://github.com/nitroos-org/reactos/commit/2361d491a58))
- Explorer.exe runs a file or shortcut given on its command line, shortcuts accept the open verb, expand %VAR% in their arguments and return the process handle — as on Windows ([fix/shell-lnk-launch-args](https://github.com/nitroos-org/reactos/commit/8487a09e95b))

</details>

<details><summary>Setup/boot (9 fixes)</summary>

- The C:\Windows junction object gets the Windows directory's protected DACL (set on the reparse point, never its target) ([fix/windir](https://github.com/nitroos-org/reactos/commit/762c5408e24))
- Setup no longer requests or installs Wine Gecko (i386 asked the old package manager for it ([fix/setupgecko](https://github.com/nitroos-org/reactos/commit/be9f824c7d7))
- Product-name strings say NitroOS (one build-time name, BRAND_PRODUCT_NAME ([fix/brand-main](https://github.com/nitroos-org/reactos/commit/90fc821966c))
- COM asks rpcss for a just-started local server's class every 10-100 ms instead of after a fixed second (Edge Update lost its install lock in that second ([fix/webview2-setup](https://github.com/nitroos-org/reactos/commit/abc3ad9ba14))
- Syssetup writes ProgramFilesDir/CommonFilesDir, the (x86) and the W6432 values expanded in both registry views, as Windows 10 ([fix/programfiles-w6432-registry](https://github.com/nitroos-org/reactos/commit/48498d776d6))
- Setup generates a Windows-style DESKTOP-XXXXXXX computer name when unattended setup gives '*' or no name ([fix/computer-name-default](https://github.com/nitroos-org/reactos/commit/d52943a5e4a))
- Unattended setup fails fast on an in-use dir, GUI message actionable, refuse a target inside another system dir ([fix/setup-install-dir-guard-followups](https://github.com/nitroos-org/reactos/commit/f9768b8f5a6))
- New installs default to \Windows ([fix/setup-default-windows-dir](https://github.com/nitroos-org/reactos/commit/9f5d2c90594))
- Setup makes \Windows a junction to the system directory on NTFS ([fix/setup-c-windows-junction](https://github.com/nitroos-org/reactos/commit/eca0fb52979))

</details>

<details><summary>Services/security/profiles (18 fixes)</summary>

- OnlineIdSystemAuthenticator GetTicketAsync: E_INVALIDARG for the default (zero) ApplicationId and E_POINTER for a NULL request as on Windows ([fix/wordticket](https://github.com/nitroos-org/reactos/commit/bc312a865da))
- Windows.Globalization.Language, NetworkInformation, OnlineIdSystemAuthenticator and OnlineIdServiceTicketRequest activatable (Wine 11.21 + RFC 5646 IsWellFormed, oracle-matched defaults ([fix/winrtword](https://github.com/nitroos-org/reactos/commit/29d1cf70d20))
- Password change with an old password of up to 14 characters verifies the NT hash when no LM hash is stored (every such change failed with 86), NULL domain/user = the caller ([fix/smalls13](https://github.com/nitroos-org/reactos/commit/9223984a965))
- Security hardening (group G8): fixes in this area, described generically ([sec/g8-misc](https://github.com/nitroos-org/reactos/commit/32c3aa5c7bd))
- Security hardening: a group of fixes in this area; details are withheld until users have had time to update ([fix/secacl](https://github.com/nitroos-org/reactos/commit/57c2c276858))
- Seclogon gives a CreateProcessWithLogon child the user's own environment (USERPROFILE was SYSTEM's Default) ([fix/profile](https://github.com/nitroos-org/reactos/commit/f491afb7c27))
- Security hardening (group G5): fixes in this area, described generically ([sec/g5-lsa](https://github.com/nitroos-org/reactos/commit/35e555ff9a9))
- Security hardening (group G6): fixes in this area, described generically ([sec/g6-user](https://github.com/nitroos-org/reactos/commit/99fa4bf6a3c))
- Security hardening (group G4): fixes in this area, described generically ([sec/g4-ldr](https://github.com/nitroos-org/reactos/commit/11eb2ad0d68))
- Lsass private bytes, handles and logon sessions stay flat (rpcrt4 frees [in] array/union stub blocks, LSA/SAM handle and SID leaks, a logon session is deleted with its last token, winlogon ... ([fix/lsass-leaks](https://github.com/nitroos-org/reactos/commit/37caac5633a))
- Security hardening (group G2): fixes in this area, described generically ([sec/g2-links](https://github.com/nitroos-org/reactos/commit/bec31b3dafd))
- Security hardening (group G1): fixes in this area, described generically ([sec/g1-followups](https://github.com/nitroos-org/reactos/commit/abe228515ec))
- PackageManager implements IPackageManager3-9 (completed-with-error deployment operations), combase returns the loader error for an in-process server that fails to load (Office 365 C2R ... ([fix/office-c2r-service](https://github.com/nitroos-org/reactos/commit/6a4ffd71ebc))
- Sppc.dll license client (file-backed store, license install/query/enumerate ([osppc](https://github.com/nitroos-org/reactos/commit/c0993c044a7))
- Shlobj.h switches on byte packing only after shtypes.h/shobjidl.h (as Wine), so shobjidl structs (KNOWNFOLDER_DEFINITION, ...) have the Windows layout everywhere (Office C2R service NULL ... ([fix/office-c2r-service-crash](https://github.com/nitroos-org/reactos/commit/ea96363f5e7))
- LSASRV: LsarLookupNames(2): free the per-name SIDs of the internal lookup result ([8c04390a317](https://github.com/nitroos-org/reactos/commit/8c04390a317))
- SCardSvr/CertPropSvc/ScDeviceEnum service entries and triggers, Calais database, winscard stopped-service answers and card queries as Windows (eID Klient) ([fix/scard-service-parity](https://github.com/nitroos-org/reactos/commit/167007f5dfd))
- Security hardening: a group of fixes in this area; details are withheld until users have had time to update ([fix/atsvc-caller-check](https://github.com/nitroos-org/reactos/commit/3a009cab56e))

</details>

<details><summary>User-mode system DLLs (36 fixes)</summary>

- KERNEL32: Ship the Unicode License v3 notice of the CLDR-generated locale tables on the CD (reactos/3rdParty/Unicode-CLDR-LICENSE.txt) ([87c24e29c3a](https://github.com/nitroos-org/reactos/commit/87c24e29c3a))
- IL-only PE32 widening keeps LOAD_CONFIG and refuses a TLS directory like Windows (STATUS_INVALID_IMAGE_FORMAT) ([fix/smalls11](https://github.com/nitroos-org/reactos/commit/a25a9fb74aa))
- Msvcrt, concrt140, msvcp90 and the msvcp140 codecvt test synced to Wine 11.21 (cxx.h 13-argument RTTI ([fix/msvcrt-wine11](https://github.com/nitroos-org/reactos/commit/285314e79a6))
- The loader gives back the references of resolved forwarders when the exporting DLL unloads (every GetProcAddress/FreeLibrary through a forwarder leaked one), names a missing forwarder ... ([fix/ntdllfu](https://github.com/nitroos-org/reactos/commit/1d1d7d83eb8))
- Performance counters for PS7 Get-Counter and typeperf: LogicalDisk, Thread and Processor Information objects, more PhysicalDisk/Processor counters, wildcard counters, counter arrays ... ([fix/pdhfu](https://github.com/nitroos-org/reactos/commit/eb6580b5f43))
- WMI root\StandardCimv2 with read-only MSFT_NetTCPConnection, MSFT_NetUDPEndpoint, MSFT_NetIPAddress, MSFT_NetIPInterface, MSFT_NetRoute, MSFT_NetAdapter, MSFT_DNSClientServerAddress ... ([feat/wmi-standardcimv2](https://github.com/nitroos-org/reactos/commit/a7ab154ae0f))
- Msafd keeps the bind-time address handle on SO_UPDATE_CONNECT_CONTEXT (it leaked one \Device\Tcp handle per ConnectEx socket ([fix/ps7-handle-leak](https://github.com/nitroos-org/reactos/commit/296866ecaa4))
- CreatePrivateNamespace fails with ERROR_ALREADY_EXISTS on an existing namespace and OpenPrivateNamespace asks for MAXIMUM_ALLOWED (a second .NET Framework process got CLR error 80070005) ([fix/second-clr-process](https://github.com/nitroos-org/reactos/commit/7f6f0e1461b))
- Typeperf.exe on pdh (counters, -sc/-si/-cf/-q/-qx/-o/-f CSV|TSV/-y), output 1:1 with Windows 10 22H2 except the NitroOS banner ([feat/typeperf](https://github.com/nitroos-org/reactos/commit/d61b1ee984c))
- NtOpenDirectoryObject, NtCreateKeyedEvent, NtCreateJobObject and NtCreateThread store NULL in the caller's handle on failure (as Windows, oracle) ([fix/null-handle-native](https://github.com/nitroos-org/reactos/commit/e342a412cea))
- The stubless server frees the top-level block it allocated for [in] bogus struct / conformant varying / bogus array reference parameters (amd64 lsass leak on every lookup) ([fix/rpc-server-inparam-leak](https://github.com/nitroos-org/reactos/commit/38084b53352))
- A loaded module's full path is answered from the loaded list without file access (GetModuleHandle no longer queues behind disk I/O) ([fix/ldr-getdllhandle-loaded-first](https://github.com/nitroos-org/reactos/commit/2affdb601d3))
- An exception in the import walk fails the load and sweeps what it mapped (a retry no longer succeeds half-loaded), hard-error text names the import and the module path ([fix/ldr-nested-walk-exception](https://github.com/nitroos-org/reactos/commit/2f6e84618da))
- A collided unwind during exception dispatch no longer marks the record as unwinding (.NET x64 same-method hardware-fault catch) ([fix/rtl-collided-unwind](https://github.com/nitroos-org/reactos/commit/ede1dba3254))
- A forwarded import that cannot be resolved fails cleanly (no unload sweep inside an import walk — the exporting DLL was freed while its forwarder string was read), exception-safe walk ... ([fix/loader-forwarder-failure-uaf](https://github.com/nitroos-org/reactos/commit/7e7c753c632))
- CreateProcessStdHandles checks the child inherited the parent's event, not a handle that happens to have the same value (x64 flake) ([fix/createprocess-stdhandles-x64](https://github.com/nitroos-org/reactos/commit/cc1635788e7))
- NtFlushBuffersFileEx (Windows parameter checks, WoW64 thunk), ZwQuerySystemInformationEx alias ([fix/ntdll-syscalls-app-ranked](https://github.com/nitroos-org/reactos/commit/48e43bd1918))
- Kernel32 duplicate exports removed and rejected at configure time, RtlWow64GetProcessMachines as Windows, RtlFls test layout ([fix/ntdll-export-followups](https://github.com/nitroos-org/reactos/commit/0f089bfc128))
- Wminet_utils.dll (System.Management's native helper) in the Framework dirs ([fix/wminet-utils](https://github.com/nitroos-org/reactos/commit/e50ed348037))
- ~100 Windows 10 ntdll exports (bounds-checking CRT, bitmap Ex, UTF-8/GUID/Ethernet strings, FLS, misc Rtl) ([fix/ntdll-cheap-exports](https://github.com/nitroos-org/reactos/commit/46f46ee83e5))
- Cfgmgr32 property-key, interface/class property, interface-key and CR-mapping functions, CM_Register_Notification delivery, setupapi property functions, advapi32 ... ([fix/cfgmgr32-advapi32-exports](https://github.com/nitroos-org/reactos/commit/9b080bc67c0))
- Kernelbase forwards 82 Windows kernelbase exports to their kernel32/ntdll implementations (stub targets deliberately not forwarded), 8 implemented exports exposed at 0x601 ([fix/kernelbase-forwarders](https://github.com/nitroos-org/reactos/commit/6f560f1ae61))
- RtlVirtualUnwind's second lookup searches only image function tables (no dynamic-table callbacks), defined outputs on NULL returns, indirect targets validated — review follow-ups of ... ([fix/rtl-unwind-followups](https://github.com/nitroos-org/reactos/commit/20a5b2798b5))
- IOCTL_DISK_PERFORMANCE in partmgr, page-fault counters in SystemPerformanceInformation, Network Interface instances named by adapter description, GetThreadUILanguage — real PhysicalDisk and ... ([fix/pdh-followups-counters](https://github.com/nitroos-org/reactos/commit/d11b13ddcf5))
- I386 RtlCaptureContext sets ContextFlags to CONTEXT_FULL (0x10007) as Windows does ([fix/rtl-i386-capturecontext-flags](https://github.com/nitroos-org/reactos/commit/6e6c11832d3))
- HKEY_PERFORMANCE_DATA/TEXT/NLSTEXT in advapi32 (PERF_DATA_BLOCK for the built-in counter sets, Counter/Help tables), PdhFormatFromRawValue, counter sets shared in sdk/lib/perfdata — .NET ... ([fix/hkey-performance-data](https://github.com/nitroos-org/reactos/commit/57ae191b59a))
- Pdh counter sets (Memory, Processor, System, PhysicalDisk, Network Interface, Process), enumeration, wildcard expansion and path parsing, real-time data source binding, Perflib\009 name ... ([fix/pdh-get-counter](https://github.com/nitroos-org/reactos/commit/c7417f35e06))
- ASSOCIATORS OF / REFERENCES OF with all WQL keywords, 11 association classes (disk links joined on real indices, schema pruning before filling), mi.dll ... ([fix/ps7-wmi-associations](https://github.com/nitroos-org/reactos/commit/92a1fe61992))
- Process snapshot functions (PssCaptureSnapshot/QuerySnapshot/WalkSnapshot/…, VA clone unsupported) and the WoW64 SystemHandleInformation thunk — LibreOffice Python macros run ([fix/kernel32-pss-snapshot](https://github.com/nitroos-org/reactos/commit/763e1c2fd68))
- WMI serves root\Interop and the CIM class hierarchy (abstract base classes, superclass chain, class enumeration, method metadata, Win32_Environment), mi.dll class enumeration and Windows ... ([fix/ps7-pester-wmi-cim](https://github.com/nitroos-org/reactos/commit/f449255eb9a))
- Amd64 RtlLookupFunctionEntry/RtlVirtualUnwind follow indirect RUNTIME_FUNCTION entries (cold-code fragments), so 64-bit Microsoft .NET Framework (clr.dll) no longer fast-fails in exception ... ([fix/rtl-unwind-indirect](https://github.com/nitroos-org/reactos/commit/c122524d0fa))
- LsaLookupSids/Names/Names2 return single-block results and free the RPC pieces (no leak) ([fix/lsa-lookup-leak](https://github.com/nitroos-org/reactos/commit/69d9127f807))
- Wbemprox stale-view bounds for __RELPATH/__PATH, mi.dll stale-application-copy safety, worker module pin, ConnectServer outside the lock ([fix/wmi-review2-nits](https://github.com/nitroos-org/reactos/commit/9dc938700b4))
- No hard-error popup for runtime LoadLibrary missing imports ([fix/ldr-no-runtime-harderror](https://github.com/nitroos-org/reactos/commit/2ec8baaa4ae))
- ProcessCommandLineInformation (class 60) + WMI CommandLine ([fix/ps-commandline-info](https://github.com/nitroos-org/reactos/commit/b7de10b7ba5))
- Mi.dll MI client over WMI + wbemprox Win10 classes (Get-CimInstance, Get-ComputerInfo) ([feat/mi-dll-wmi-m1](https://github.com/nitroos-org/reactos/commit/e1ce32a99d5))

</details>

<details><summary>Browser/COM/Wine imports (66 fixes)</summary>

- WINESYNC: Remove a committed Python bytecode cache and ignore __pycache__ ([e2eebbf35f8](https://github.com/nitroos-org/reactos/commit/e2eebbf35f8))
- Msxml3 XPath parses prefixed function calls (the Wine-11.21 engine split ms:string-compare( at the colon) and implements ms:string-compare ([fix/wordapv](https://github.com/nitroos-org/reactos/commit/745688427b2))
- Mapi32, d3d10, d3d10_1, ddrawex, dplayx, dpnet synced to Wine 11.21 (DirectPlay8Create returns real objects) ([fix/winebulk4](https://github.com/nitroos-org/reactos/commit/faaf9611b12))
- WINESYNC: 55 class-(a) modules recorded at Wine-11.21 ([f0e839f9dd9](https://github.com/nitroos-org/reactos/commit/f0e839f9dd9))
- Urlmon and ieframe synced to Wine 11.21 ([fix/urlmon-wine11](https://github.com/nitroos-org/reactos/commit/31d81ea72af))
- Cscript CreateObject works for objects with IObjectWithSite (vbscript site without a host IServiceProvider, Shell.Application keeps its site) ([fix/smalls8](https://github.com/nitroos-org/reactos/commit/dcb0e91af86))
- Mshtml, jscript and the mshtml headers synced to Wine 11.21 (Gecko 2.47.4 kept ([fix/mshtml-wine11](https://github.com/nitroos-org/reactos/commit/ec00f4d925a))
- Msxml3 and msxml4 synced to Wine 11.21 (Wine 11 DOM layer ([fix/msxml3-wine11](https://github.com/nitroos-org/reactos/commit/88f8c7ad971))
- Mshtml extern abort task was freed twice (destructor + task runner) on every download hand-off: heap corruption in Release, heap assertion in Debug ([fix/mhheap](https://github.com/nitroos-org/reactos/commit/93aabe6486c))
- Mshtml synced to Wine 10.0 (upstream PR 9550), Gecko 2.47.4 pins (official WineHQ sha256), detach_inner_window document list fix, SysWOW64 mshtml.tlb, sync XHR works in pages ([fix/mshtml-wine10](https://github.com/nitroos-org/reactos/commit/fdd9acf876e))
- Wine 11.21 header syncs (oleauto CHAR types + oleaut32 vartype, wine/test.h wait_child_process both forms, dbgeng.h + dbgeng, IEnumObjects + actxprxy, wmcodecdsp/mediaobj + mf transform ... ([fix/wineheaders](https://github.com/nitroos-org/reactos/commit/369a8f6f8bd))
- Wine 11.21 sync batch 2 (18 class-A modules) ([fix/winebulk2](https://github.com/nitroos-org/reactos/commit/ccab198426a))
- VBScript FormatDateTime(vbShortTime) 24-hour hh:mm like Windows ([fix/smalls4](https://github.com/nitroos-org/reactos/commit/cc12420d86e))
- Hnetcfg registers the NetFwPublicTypeLib typelib (late-bound HNetCfg.FwPolicy2 works) ([fix/smalls3](https://github.com/nitroos-org/reactos/commit/ac30c701b82))
- Wine 11.21 sync batch 1 (16 class-A modules) ([fix/winebulk1](https://github.com/nitroos-org/reactos/commit/36a4e065843))
- A documented winesync tool against upstream Wine (winewatch.py scan/merge/check, configs for every Wine-derived module, README) ([fix/winesync](https://github.com/nitroos-org/reactos/commit/55b006f96e3))
- Wine Gecko 2.40 (the mshtml engine) can be installed on demand from featmgr (32-bit into SysWOW64 and 64-bit on amd64, same sources and pins as appwiz's add-on prompt, compatibility only ([fix/gecko-featmgr](https://github.com/nitroos-org/reactos/commit/d6dc419bfcf))
- Wintypes RoIsApiContractPresent/RoIsApiContractMajorVersionPresent (Windows 10 22H2 contract table) and combase CoRegisterActivationFilter (Office WINWORD crashed on these stubs ([fix/office-winword](https://github.com/nitroos-org/reactos/commit/4d8647e2436))
- A glass popup that grows after it is shown keeps its backdrop for the part it had and reads only the newly exposed part (Firefox context menu showed a black strip) ([fix/firefox-context-menu](https://github.com/nitroos-org/reactos/commit/bd56d3d453f))
- Msi custom-action API parity (feature cost tree, component cost strings, view errors reported once, deferred actions see only CustomActionData/ProductCode/UserSID) ([fix/msi-winetests](https://github.com/nitroos-org/reactos/commit/c5bad7d15c6))
- The Win32k lockdown mitigation is kept per process, offered in the options mask and applied from the CreateProcess attribute (Firefox's OpenH264 plugin process was set up as unlocked and ... ([fix/firefox-gmp-window](https://github.com/nitroos-org/reactos/commit/6a5c8c62f1e))
- Advapi32 Perf provider, esent and wininet exports msedge.dll delay-loads ([fix/webview2-exports](https://github.com/nitroos-org/reactos/commit/70f555aaff6))
- Iexplore.exe opens the default browser with the URL (Windows 11 model ([fix/iexplore-default-browser](https://github.com/nitroos-org/reactos/commit/19c12ffbb24))
- Native 64-bit thread stacks get a three-page guard region re-armed three pages deep, as on Windows x64 (dav1d's 8 KB-stride stack probe crashed Firefox tabs on explore.microsoft.com) ([fix/firefox-thread-stack](https://github.com/nitroos-org/reactos/commit/0fa7fdacd4c))
- Get_xml no longer modifies the document (thread-safe serialization), undeclared prefix reports 0xc00ce01d, inherited-namespace placement pinned to the oracle ([fix/msxml-inherited-ns](https://github.com/nitroos-org/reactos/commit/d632d08a91b))
- PowerShell 7 preinstalled + headline shell, Firefox installed at first boot as default browser in place of Wine IE ([feat/pwsh-firefox-defaults](https://github.com/nitroos-org/reactos/commit/da43f8274c7))
- Msxml schema cache accepts the msxml \uXXXX/\xXX escapes in xs:pattern (Office C2R AppV configuration schema) ([fix/msxml-xsd-unicode-escape](https://github.com/nitroos-org/reactos/commit/2cd5faaa136))
- Msxml node.xml keeps namespace declarations inherited from outside the subtree ([fix/msxml-removechild](https://github.com/nitroos-org/reactos/commit/575fbb7a8ff))
- Rpcrt4 client association re-binds without a stale group id when an endpoint was taken over (OLE_<pid> reuse) ([fix/qmgr-setnotifyinterface](https://github.com/nitroos-org/reactos/commit/db856b81fd9))
- 32-bit clients can create the BITS manager (WoW64 CLSID view + SysWOW64 proxy/stub), as on Windows ([fix/qmgr-bits-32bit-clsid](https://github.com/nitroos-org/reactos/commit/68c10e51b07))
- Msi InstallFiles reports ACTIONDATA/progress per file while a cabinet extracts (KROS 0% bar) ([fix/msi-cab-progress](https://github.com/nitroos-org/reactos/commit/aa182a9e4e6))
- Dismapi.dll (online image, no provisioned packages ([fix/office-c2r-centennial](https://github.com/nitroos-org/reactos/commit/5ad1e17dd8d))
- BITS writes a ranged file's ranges one after the other in the order given (as documented), progress totals as sums, FileRangesTransferred (IBackgroundCopyCallback3) ([fix/office-c2r-bits-ranges](https://github.com/nitroos-org/reactos/commit/aa55cdbff57))
- Restart Manager (rstrtmgr.dll, oracle-calibrated), combase no longer deadlocks when the last stub of an interface goes away inside its own call ([fix/office-c2r-rstrtmgr](https://github.com/nitroos-org/reactos/commit/768158f8e54))
- A 64-bit child of a WoW64 process gets the native environment view, BITS AddFileWithRanges/GetFileRanges with ranged HTTP transfers ([fix/office-c2r-walls](https://github.com/nitroos-org/reactos/commit/f626b62cb73))
- Ole32 storage reports a full medium instead of retrying the failed growth for every block (msiexec spun on a full disk) ([fix/libreoffice-msi-hang](https://github.com/nitroos-org/reactos/commit/30c5c4301da))
- The API set schema wins over app-local contract copies (Windows 10), forwarder loader tests, ncalrpc needs no security package ([fix/office-c2r-phase2](https://github.com/nitroos-org/reactos/commit/89ec620c049))
- IApplicationActivationManager opens the browser chooser for the Settings 'Default apps' page, per-user RegisteredApplications honoured, bchooser lists per-user browsers ([fix/chrome-make-default](https://github.com/nitroos-org/reactos/commit/0fdb862b967))
- Msi makes installed fonts usable at once (AddFontResource + WM_FONTCHANGE), win32k keeps a loaded font's file open without write/delete sharing, GetSystemFileCacheSize, 32-bit icon paths in ... ([fix/kros-small-followups](https://github.com/nitroos-org/reactos/commit/b5b44e78bca))
- Shell32 honours FileExts UserChoice for extensions (ShellExecute, AssocQueryString), browser chooser adopts Firefox's own client and only takes web associations ([fix/default-browser-url-handlers](https://github.com/nitroos-org/reactos/commit/284ea083373))
- Web browser handlers registered via %ProgramW6432% on amd64, so 32-bit programs can open URLs (ShellExecute of http/https/.html failed with ERROR_PATH_NOT_FOUND — KROS ALFA plus login) ([fix/kros-login-next](https://github.com/nitroos-org/reactos/commit/1687df02a31))
- Msiexec /a of an installed product works (the admin image ignores the registered install location and copies the given package, never the cached one), SFN bit in the admin image word count ([fix/msi-admin-followups](https://github.com/nitroos-org/reactos/commit/2e661d4edac))
- Webservices.dll (Wine 10.0) and a minimal AppXDeploymentClient PackageManager reporting no packages — the Office Deployment Tool no longer crashes at start and downloads Office ... ([fix/office-c2r-phase1](https://github.com/nitroos-org/reactos/commit/cdc6434a718))
- Mscoree vtable-entry exports + IJW DllMain handed to the Microsoft CLR (mixed-mode System.Data), WoW64 NtContinue accepts the exception-state context bits (32-bit CLR resume after catch) — ... ([fix/kros-alfa-install](https://github.com/nitroos-org/reactos/commit/c53c8ba9573))
- WinHTTP accepts response header lines longer than 1460 bytes (the Chrome online installer's ML-DSA-signed update check was truncated → RESPONSE_NOT_TRUSTED) ([fix/chrome-installer](https://github.com/nitroos-org/reactos/commit/a43c806fa11))
- Rapps image overlay (current Firefox + ESR via Mozilla latest redirects, hide Firefox 28/3.6), browser chooser defaults to firefox ([fix/browser-default-modern-firefox](https://github.com/nitroos-org/reactos/commit/a8742fb0f11))
- DwmFlush waits for the next vblank (an immediate return made Firefox 128's vsync thread spin ([fix/firefox-cold-start](https://github.com/nitroos-org/reactos/commit/d5135360447))
- SystemLogicalProcessorInformation reports processor packages (MSVC hardware_concurrency() was 0 ([fix/firefox-captcha](https://github.com/nitroos-org/reactos/commit/6cac5c6842c))
- Msiexec /a names the image directories from the source DefaultDir (scoop MSI extraction layout), honours SHORTFILENAMES ([fix/msi-admin-install-layout](https://github.com/nitroos-org/reactos/commit/fec2175c689))
- Chain winetest does not index past a partial chain when AIA retrieval is unavailable ([fix/crypt32-chain-winetest-av](https://github.com/nitroos-org/reactos/commit/a758b5d30de))
- Rollback of registry keys/values, MoveFiles/PatchFiles and user cancel ([fix/msi-pass6](https://github.com/nitroos-org/reactos/commit/af84d9082c1))
- ProcessHandleTable snapshot with slack (Chrome killed renderers with SBOX_FATAL_CLOSEHANDLES on a racing handle) ([fix/chrome-renderer-handletable-race](https://github.com/nitroos-org/reactos/commit/bdd878f1fa0))
- Roll back failed installs (files, directories, product registration), Environment prefix/suffix removal, package.c fixes (66 -> 29) ([fix/msi-rollback-and-package](https://github.com/nitroos-org/reactos/commit/f40e94c9c3b))
- IDispatch for all Task Scheduler 2.0 objects through the typelib (late-bound PowerShell/VBScript/JScript/.NET dynamic failed with E_NOTIMPL), DISPIDs/_NewEnum/Item in the IDL, optional ... ([fix/taskschd-idispatch](https://github.com/nitroos-org/reactos/commit/4731f460530))
- Registry key paths are not source paths in MsiQueryFeatureState, publish/unpublish on partial maintenance removal, remove the empty product directory ([fix/msi-maintenance-flows](https://github.com/nitroos-org/reactos/commit/7b285d8508d))
- Feature tree and action states like Windows for ADDLOCAL/ADDSOURCE/ADVERTISE/REMOVE, source-only features, VersionMsi/VersionDatabase/ServicePackLevelMinor ([fix/msi-costing-action-state](https://github.com/nitroos-org/reactos/commit/556f095f25d))
- Remove the empty directories a product leaves, delete emptied ini files, VersionNT from the version the process sees ([fix/msi-uninstall-dirs-versionnt](https://github.com/nitroos-org/reactos/commit/da47325e1b9))
- New env vars as REG_SZ, real EstimatedSize, Usage/Patches keys and Assignment, platform list parsing, multi-string removal ([fix/msi-elevated-install-tests](https://github.com/nitroos-org/reactos/commit/9c320512fcb))
- Wined3d waits for the first present of each swapchain, so a window's first frame is on screen when Present/EndDraw returns, as on Windows ([fix/d2d1-first-present](https://github.com/nitroos-org/reactos/commit/81e03563664))
- A ControlCondition Default action makes the control the dialog default, so Enter on a WiX VerifyReadyDlg installs instead of firing the hidden Repair button — full-UI MSI installs register ... ([fix/msi-ui-install-registration](https://github.com/nitroos-org/reactos/commit/82c3f667bec))
- Task Scheduler 2.0 instances, Stop, last/next run and enable over SchRpc, S4U tasks via an LSA network logon (registrant/admin rules unchanged), Windows-shaped TaskCache registry copy ... ([feat/taskschd-phase1](https://github.com/nitroos-org/reactos/commit/693a6bd6089))
- Cscript/wscript honour //S (DisplayLogo/Timeout saved in HKCU and read at startup) and //H:, and split arguments with the Windows Script Host rules ([fix/wsh-arg-splitting](https://github.com/nitroos-org/reactos/commit/f7628aa7920))
- WglMakeCurrent rebinds a context that is already current on the thread to a new DC/window, as Windows does — Direct3D/Direct2D window presents (wined3d swap chains) reach the screen ([fix/npp-scintilla-paint](https://github.com/nitroos-org/reactos/commit/71c4c847ab3))
- Nested job objects as on Windows 8+ (assigning a jobbed process nests the job, per-job accounting, terminate/kill-on-close through the hierarchy, breakaway through the chain) — Chrome and ... ([fix/chrome-headless](https://github.com/nitroos-org/reactos/commit/b11fa5ff61d))
- Wevtapi queries, reads and renders the classic event logs (XPath subset, system values and event XML, message formatting, publisher metadata) ([fix/wevtapi-get-winevent](https://github.com/nitroos-org/reactos/commit/0967d4205ae))
- LibreOffice 26.8 starts — crypt32 CryptExportPublicKeyInfoFromBCryptKeyHandle (+ time stamp entry points), bcrypt key algorithm name, msvcp140 money facet ids, httpapi 2.0 entry points ... ([fix/libreoffice-start](https://github.com/nitroos-org/reactos/commit/d59555f177e))

</details>

<details><summary>Applications/tools (91 fixes)</summary>

- Locale tables (146 .nls + calendar data) regenerated from Unicode CLDR 49.0.0 by the in-tree, deterministic sdk/tools/nlsgen (pinned sha256) ([fix/nlscldr](https://github.com/nitroos-org/reactos/commit/049578fe905))
- NTFS-NG sends a word-aligned (locked, system-mapped) non-cached write buffer to the device directly, up to 1 MB per request, instead of one synchronous 64 KB bounce round trip per piece ... ([fix/ionobuf](https://github.com/nitroos-org/reactos/commit/fe5874fd937))
- A terminating thread runs at LOW_REALTIME_PRIORITY so TerminateProcess of an idle-class process under CPU load completes promptly (faster than Windows ([fix/killff](https://github.com/nitroos-org/reactos/commit/f85f0250480))
- SO_RCVTIMEO applies to datagram sockets too (a UDP recv with a timeout waited forever ([fix/smalls17](https://github.com/nitroos-org/reactos/commit/0ddc96c6196))
- Netsh set/add/delete dns no longer crash netcfgx or turn DHCP off, gwmetric optional ([fix/smalls16](https://github.com/nitroos-org/reactos/commit/f645701afb5))
- Firewall.cpl hides the Domain section when not domain-joined (as Windows) ([fix/smalls9](https://github.com/nitroos-org/reactos/commit/844f25aa97a))
- Cmd dir /r finds streams for any path ([fix/smalls6](https://github.com/nitroos-org/reactos/commit/20625ba08f8))
- NtSaveMergedKeys merges High into Low (values and subkeys) ([fix/smalls7](https://github.com/nitroos-org/reactos/commit/01d53e2f1b7))
- CmSaveMergedKeys takes its two KCB locks in bucket order ([fix/smalls5](https://github.com/nitroos-org/reactos/commit/b743020cf2f))
- Featmgr's GPU pack (Mesa lavapipe/llvmpipe) downloads from https://repo.nitroos.org/featmgr (published and verified 2026-11-05) ([fix/lvpbase](https://github.com/nitroos-org/reactos/commit/6db0a832d06))
- DXGI lists the WARP software adapter after the display adapter, as Windows 10 does (Office looks for a software-flagged WARP adapter) ([fix/office-p23](https://github.com/nitroos-org/reactos/commit/cf4313bb619))
- Get-WinEvent/EventLog gaps: EvtSeek origins, EvtSubscribe on local logs, EvtExportLog/EvtClearLog with backup, publisher HelpLink, registered channel names, manifest publisher metadata ... ([feat/ps7-eventlog](https://github.com/nitroos-org/reactos/commit/1a148ef552c))
- Unsafe catalog cleanup items are dropped with a log line instead of failing the install (otter, siv, wxmedit, x64dbg, diskview, filemon, winobj install) ([fix/featmgr-catalog-compat](https://github.com/nitroos-org/reactos/commit/86f537e60a4))
- The ReactOS Applications Manager is replaced by featmgr (native RAPPS-catalog provider: pinned https catalog, SHA-256/SHA-1 verified silent installs, confined uninstall ([feat/replace-rapps](https://github.com/nitroos-org/reactos/commit/4b639994fd0))
- WScript.Shell shortcuts implement TargetPath (expanded canonical path as Windows 10), Description, FullName, Hotkey, Load, and open an existing .lnk ([fix/sweep5-findings](https://github.com/nitroos-org/reactos/commit/1879ff78a67))
- NitroSetWindowGlass only marks windows of the calling process ([fix/review-followups](https://github.com/nitroos-org/reactos/commit/4087b16f189))
- Featmgr GPU Pack (Mesa llvmpipe/lavapipe) downloads per-file SHA-256/size-pinned payload over https from the NitroOS domain (BaseUrl placeholder until set), removable with a safe in-use ... ([fix/lavapipe-pack-download](https://github.com/nitroos-org/reactos/commit/c43b8344269))
- Featmgr makes scoop WezTerm use the system ConPTY (bundled conpty.dll/OpenConsole.exe disabled) and seeds WezTerm/Alacritty configs with pwsh and an installed font (never over a user ... ([feat/terminal-scoop-defaults](https://github.com/nitroos-org/reactos/commit/2603858539e))
- Shutdown/logoff send WM_QUERYENDSESSION/WM_ENDSESSION to every unowned top-level window of a thread, as Windows (Firefox saved its session only by window-order luck) ([fix/shutdown-endsession](https://github.com/nitroos-org/reactos/commit/41bcdaa5dec))
- SystemLogicalProcessorInformation reports the processor caches from CPUID (RelationCache, as Windows) ([fix/lp-cache-info](https://github.com/nitroos-org/reactos/commit/81583c1af15))
- Component installs serialized by a global mutex with a detect re-check (first boot and a manual install no longer race), msiexec 1618 retried ([fix/featmgr-install-lock](https://github.com/nitroos-org/reactos/commit/93fc583b6b4))
- Fsutil output, usage, errors and exit codes match Windows 10 22H2 (fsutil_apitest) ([fix/fsutil-output-1to1](https://github.com/nitroos-org/reactos/commit/dfd8b40e8f3))
- Register SysWOW64 explorerframe.dll (32-bit Open/Save dialog was blank), hide the Internet Browser desktop icon as Windows 10 ([fix/demo-polish](https://github.com/nitroos-org/reactos/commit/2627a31873c))
- FileDispositionInformationEx with POSIX semantics (RemoveDirectory unlinks a directory at once even with stray handles open, as Windows 10) ([fix/scoop-install-race](https://github.com/nitroos-org/reactos/commit/59796ae9412))
- Windows-style features tree, 'Installing additional components' window, components.ini (https + SHA-256), .NET 4.8 at first boot (AutoInstall=0 switch) ([feat/featmgr-components-ui](https://github.com/nitroos-org/reactos/commit/43d3e697187))
- Refreshenv (cmd and pwsh) brings the current window's PATH up to date after a featmgr install ([fix/scoop-path-refresh](https://github.com/nitroos-org/reactos/commit/e062654ff87))
- Featmgr tells the user to open a new terminal (or refresh $env:Path) after an install changed PATH ([fix/scoop-path-after-install](https://github.com/nitroos-org/reactos/commit/07634439051))
- The Windows 10 system ICU (icu.dll + icuuc/icuin forwarders, ICU4C 64.2, Globalization\ICU\icudtl.dat filtered to the locales the NLS knows) ([feat/system-icu](https://github.com/nitroos-org/reactos/commit/884d84de867))
- Scoop uninstall deletes the machine SCOOP variable instead of leaving it empty ([NullString]::Value under PowerShell 7) ([fix/scoop-uninstall-machine-var](https://github.com/nitroos-org/reactos/commit/a6d53bfd43c))
- Scoop catalog entry, Software tab tip and readme explain that the one-liner refuses to run as administrator here and give the -RunAsAdmin routes ([fix/scoop-admin-hint](https://github.com/nitroos-org/reactos/commit/f2df5ed4c68))
- The C:\scoop lock ACL keeps the installing user's SID (initdb/pg_ctl drop Administrators via CreateRestrictedToken and could not chmod their data dir), live-test for postgresql ([fix/scoop-postgresql-initdb](https://github.com/nitroos-org/reactos/commit/07cca94f851))
- Nitro Features shows progress, offers Cancel (job-object kill ([fix/featmgr-progress-cancel](https://github.com/nitroos-org/reactos/commit/c76669ae379))
- PowerShell 7.6.9 LTS (x64 MSI, pinned + verified at configure) installed during setup on amd64 ([feat/preinstall-powershell7](https://github.com/nitroos-org/reactos/commit/50c9dd5f570))
- CMS key unwrap via CryptDecrypt (incl ([fix/crypt32-cms-decrypt-streaming](https://github.com/nitroos-org/reactos/commit/0a077b41eb1))
- New-/Remove-NetIPAddress, Set-NetIPInterface -Dhcp, Set-DnsClientServerAddress, Set-Volume label, Set-/Export-ScheduledTask ([fix/ps7-write-cmdlets](https://github.com/nitroos-org/reactos/commit/51817b1f141))
- Weekly, monthly and monthly day-of-week triggers in taskschd, schedsvc and schtasks ([fix/schedsvc-weekly-monthly](https://github.com/nitroos-org/reactos/commit/6f3813f9309))
- Clean-room ScheduledTasks module for PowerShell 7 over Schedule.Service (13 commands, Windows types/views/errors ([fix/ps7-scheduledtasks-module](https://github.com/nitroos-org/reactos/commit/93f6745f289))
- Ren splits a full source path safely (unterminated copy gave error 123 on i386), destination must be a bare name, Windows wildcard mask rules and error levels ([fix/cmd-ren-fullpath](https://github.com/nitroos-org/reactos/commit/315991461c4))
- Clean-room Microsoft.PowerShell.LocalAccounts for PowerShell 7 (15 cmdlets, Windows types/properties/errors), machine PSModulePath, netapi32 stores the UF_PASSWD_CANT_CHANGE DACL ([fix/ps7-localaccounts-module](https://github.com/nitroos-org/reactos/commit/1b24fb08783))
- Systeminfo.exe with the Windows field set and formats (LIST/CSV/TABLE, /NH, /S errors), values from the system itself ([fix/systeminfo](https://github.com/nitroos-org/reactos/commit/2fd69a5b5e1))
- NormalizeString and IsNormalizedString with Unicode 11 tables (generated from the UCD), NFKC nameprep in the Idn functions, normaliz forwards ([fix/normalizestring-nfkc](https://github.com/nitroos-org/reactos/commit/82364ed3355))
- BCryptDeriveKey HMAC, TLS PRF and SP800-56A concat KDFs ([fix/bcrypt-derivekey-kdfs](https://github.com/nitroos-org/reactos/commit/9cab9ec4998))
- Drwtsn32 ends the debuggee and needs no answer when nobody can give one ([fix/drwtsn32-unattended-hang](https://github.com/nitroos-org/reactos/commit/e1c960c909e))
- Secur32 invalid handles, i386 PF_FASTFAIL_AVAILABLE, GetModuleFileName truncation error ([fix/stubs-r2-followups](https://github.com/nitroos-org/reactos/commit/cc94a18168f))
- Functions real apps import that were stubs — IdnTo*, ucrtbase _dpcomp/_except1, vcruntime140 helpers, std::_Throw_Cpp_error, Safer* hash/type checks, QueryContextAttributesEx ... ([fix/app-imported-stubs-r2](https://github.com/nitroos-org/reactos/commit/343b8e79697))
- KERNELBASE: Drop the duplicate threadpool callback forwarders from kernelbase_fwd.lst ([e095e9d02f6](https://github.com/nitroos-org/reactos/commit/e095e9d02f6))
- FOS_PICKFOLDERS item dialog behaves as Windows (typed folder navigates first, empty name box returns the current folder, Windows' error messages, leaf name in the name box) ([fix/folder-picker-windows-parity](https://github.com/nitroos-org/reactos/commit/08cc0706ef5))
- Wevtapi reads .evtx files (EvtQueryFilePath, BinXml templates rendered to event XML, XPath on file events), bounds-checked against untrusted input — Get-WinEvent -Path ([feat/wevtapi-evtx-reader](https://github.com/nitroos-org/reactos/commit/945c730f8da))
- Msvcp140 exports match Windows' msvcp140.dll exactly (messages/time_get ids, time_base and time_get<unsigned short> vtables, get_new_handler, x64 _Iosb decoration) ([fix/msvcp140-missing-exports](https://github.com/nitroos-org/reactos/commit/02c51f7bd1f))
- Msvcp140 exports the moneypunct intl constants as Windows does (the facets themselves are header-built on Windows too) ([feat/msvcp-money-facets](https://github.com/nitroos-org/reactos/commit/661498f265b))
- The item dialog with FOS_PICKFOLDERS accepts a typed or selected folder (full pidl instead of a simple pidl for the folder check) ([fix/craft-folder-picker-typed-path](https://github.com/nitroos-org/reactos/commit/4b0d8358407))
- IDXGIFactory4::EnumWarpAdapter returns a software (WARP) adapter with the Basic Render Driver identity, as Windows does ([fix/dxgi-enumwarpadapter](https://github.com/nitroos-org/reactos/commit/2a8fab252a7))
- Llvmpipe staging is atomic and pin-checked on every configure, rclone timeouts, per-arch Mesa3D status line at configure end ([fix/mesa-staging-atomic](https://github.com/nitroos-org/reactos/commit/a215a2a6080))
- AddClipboardFormatListener/RemoveClipboardFormatListener and WM_CLIPBOARDUPDATE as on Windows, a real GetUpdatedClipboardFormats, clipboard sequence number and viewer-chain fixes ([feat/clipboard-format-listener](https://github.com/nitroos-org/reactos/commit/1b950bbd3dc))
- GetCharWidthFloatW returns widths in 1/16 units as on Windows — Git Bash (mintty) lays out its cells correctly ([fix/mintty-text-cells](https://github.com/nitroos-org/reactos/commit/6fe655101eb))
- D3d12.dll for images below 0xA00 whose D3D12CreateDevice reports no D3D12-capable adapter (DXGI_ERROR_UNSUPPORTED), as Windows 10 without one ([fix/d3d12-stub-unsupported](https://github.com/nitroos-org/reactos/commit/12c0ee1ce52))
- [BOOTDATA] default the per-app Direct3D renderer to no3d for Code.exe so Chromium falls back to software compositing (no dcomp.dll) — Visual Studio Code paints ([fix/vscode-paint](https://github.com/nitroos-org/reactos/commit/a823fd50484))
- Wevtutil.exe (im/um manifest registration in the Windows WINEVT registry shape, el/ep/gl/gli/qe over wevtapi, Windows output formats and exit codes) ([feat/wevtutil](https://github.com/nitroos-org/reactos/commit/161e0ee73dc))
- CryptAcquireCertificatePrivateKey leaves the Windows 10 errors when no key is found (NTE_BAD_PROV_TYPE, or STATUS_NOT_FOUND with the NCrypt key spec for NCrypt callers) ([fix/crypt32-ncrypt-test-parity](https://github.com/nitroos-org/reactos/commit/80647377ccf))
- MsV1_0S4ULogon (password-less network logon of a local account ([feat/msv1_0-s4u-logon](https://github.com/nitroos-org/reactos/commit/0a4718850bd))
- CNG (NCrypt) key handles for certificates (CERT_NCRYPT_KEY_HANDLE_PROP_ID, CryptAcquireCertificatePrivateKey NCrypt flags), NCrypt ECC keys, PKCS #8 blobs, NCryptDecrypt, the Windows export ... ([feat/crypt32-ncrypt-key-handle](https://github.com/nitroos-org/reactos/commit/d7360a1259a))
- CertFindCertificateInStore supports CERT_FIND_HASH_STR (thumbprint text, parsed as on Windows) — PowerShell Cert:\...\<thumbprint> Get-Item/Test-Path/Remove-Item work ([fix/crypt32-find-hash-str](https://github.com/nitroos-org/reactos/commit/7e90c5fb426))
- CMS enveloped messages with AES/3DES/DES/RC2/RC4, CMS recipients (issuer+serial, SKI ([feat/crypt32-cms-enveloped](https://github.com/nitroos-org/reactos/commit/cdebcee53c6))
- Native PKCS #12 reader/writer — PFXImportCertStore, PFXExportCertStoreEx, PFXVerifyPassword (3DES/RC2/RC4/AES, PBES2, SHA-1/SHA-256 MAC, Windows iteration cap) ([feat/crypt32-pkcs12](https://github.com/nitroos-org/reactos/commit/efb43f97061))
- WinFsp 2.1 loads (driver imports resolved from system32\drivers), rclone mount and sshfs-win work — sc config binPath, QueryServiceObjectSecurity (services.exe no longer hangs on driver ... ([fix/winfsp-apps](https://github.com/nitroos-org/reactos/commit/c249817b2aa))
- The version-lie shims answer VerifyVersionInfo with the same comparison as RtlVerifyVersionInfo (shared header), against the faked version ([fix/shims-versionlie-verify](https://github.com/nitroos-org/reactos/commit/2074cb0ee84))
- Catalog files (CryptCAT create/enumerate/persist, CDF), the PowerShell script SIP (pwrshsip) and WinVerifyTrust blob subjects with digest verification — PowerShell 7 FileCatalog and ... ([fix/ps7-pester-security](https://github.com/nitroos-org/reactos/commit/de73bebc091))
- RtlVerifyVersionInfo and VerSetConditionMask match Windows 10 on every case of a 278,802-request oracle sweep (SP minor alone, signed major/minor compare, empty condition mask, bit 63) ([fix/verifyversioninfo-sp](https://github.com/nitroos-org/reactos/commit/9f610c0fae8))
- A by-name attribute query of a named pipe or the pipe directory is answered from basic+standard information when the file system refuses FileNetworkOpenInformation, as on Windows — ... ([fix/ps7-pester-hostprocess](https://github.com/nitroos-org/reactos/commit/e3d8976d05b))
- ICMP echo replies always carry a Data pointer and finish at the first reply for a single host, 32-bit callers under WoW64 can ping, IcmpSendEcho2Ex, Windows 10 error codes — PowerShell 7 ... ([fix/ps7-test-connection](https://github.com/nitroos-org/reactos/commit/81cf7d6977b))
- Windows 10 locale data for the 143 shared locales (measured on Windows 10), calendar names/eras/lists, locale-name fixes (neutral, script-tagged, default name length), CAL_ITWODIGITYEARMAX ... ([fix/locale-data-win10](https://github.com/nitroos-org/reactos/commit/db1ef5695cc))
- WQL string comparison and LIKE (with _ and [set]) are case-insensitive as on Windows — Get-CimInstance Win32_Service -Filter "Name='Winmgmt'" finds the service ([fix/wbemprox-wql-case](https://github.com/nitroos-org/reactos/commit/3ac38434e98))
- Microsoft .NET Framework 4.8 is a one-click feature (featmgr dotnet-framework-4.8, SHA-256 verified download) and its official installers get the WIN7SP1 compatibility layer automatically ... ([feat/dotnet48-featmgr](https://github.com/nitroos-org/reactos/commit/7aaeabed9af))
- A future-dated time stamp is accepted when the signer and TSA are valid at that time, as on Windows 10 ([fix/wintrust-future-timestamp-parity](https://github.com/nitroos-org/reactos/commit/ff66f294ae3))
- Featmgr refuses a software install that cannot fit on the system volume with a clear message instead of a bare installer exit code ([fix/featmgr-ps7-install](https://github.com/nitroos-org/reactos/commit/ab1a38bd023))
- Onto current main ([msclr-land](https://github.com/nitroos-org/reactos/commit/6e573f3ff75))
- The machine's own name, its trailing-dot form and its own/loopback addresses resolve locally (dnsapi, ws2_32 getnameinfo) ([fix/ps7-own-hostname](https://github.com/nitroos-org/reactos/commit/606c35a3452))
- CopyFile/CopyFileEx, cmd copy and xcopy carry NTFS alternate data streams (and CreateDirectoryEx template streams) ([fix/copyfile-streams](https://github.com/nitroos-org/reactos/commit/6b516dfd37d))
- WoW64 passes FileStreamInformation through, so FindFirstStreamW (Get-Item -Stream, dir /r) works in 32-bit processes ([fix/ps7-ads-enum](https://github.com/nitroos-org/reactos/commit/03d8faad1f1))
- SystemPageFileInformation returns the paging files (WoW64, WMI ([fix/ps7-pester-utility](https://github.com/nitroos-org/reactos/commit/fba83ba5909))
- InterlockedPushListSListEx, IPHLPAPI GetIpNetEntry2/ResolveIpNetEntry2/compartment IDs, SetCredentialsAttributesA/W (msquic loads), WoW64 honours the 32-bit thread error mode in ... ([fix/ps7-missing-imports](https://github.com/nitroos-org/reactos/commit/a8cbb33b045))
- Alternate data streams (FindFirstStreamW, ntfsng stream handles), scrrun Drives, npfs pipe directory listing, shell FolderItems IEnumVARIANT and typelib names — 15 PS7 Pester tests fixed ([fix/ps7-pester-fs-proc](https://github.com/nitroos-org/reactos/commit/71b0b0a05f9))
- Featmgr Scoop M2 — per-bucket index (fast lists at 4k+ apps), installed-version column, /update and Update button, known-bucket Add dialog (/add-bucket /trust for others), search ([feat/featmgr-scoop-m2](https://github.com/nitroos-org/reactos/commit/b6f265fae7f))
- Icacls.exe (display/grant/deny/remove/inheritance/reset/T/C/Q/save/restore/setowner/findsid, Windows ordering and exit codes) ([feat/cmdutils-icacls](https://github.com/nitroos-org/reactos/commit/aeccd8fa7dc))
- Exit /b N propagates through CALL and cmd /c exactly as Windows (130-check oracle-derived test) ([fix/cmd-exit-b-errorlevel](https://github.com/nitroos-org/reactos/commit/01060518ee1))
- Scoop in featmgr — 'scoop' feature (machine-wide C:\scoop, depends on powershell-7, pinned+hashed installer, admin-only ACL), /import-scoop main|extras into the software catalog ... ([feat/featmgr-scoop](https://github.com/nitroos-org/reactos/commit/6449cb2a4e9))
- Npfs counts only open server handles against MaximumInstances and removes an orphaned pipe's name, so a new instance can be created while old clients drain (Windows 10 semantics) ([fix/npfs-instance-reuse](https://github.com/nitroos-org/reactos/commit/f39b021a0a7))
- System-wide WGPU_BACKEND=gl default (no D3D12 in the image ([fix/deckcraft-launch](https://github.com/nitroos-org/reactos/commit/72cc660ef0e))
- CreateSymbolicLinkW accepts ALLOW_UNPRIVILEGED_CREATE ([fix/symlink-unprivileged](https://github.com/nitroos-org/reactos/commit/1f5e00c0aff))
- Mklink use-after-free of parsed args ([fix/cmd-mklink-junction](https://github.com/nitroos-org/reactos/commit/90f72122e59))
- Attrib /L acts on the link itself ([fix/attrib-l](https://github.com/nitroos-org/reactos/commit/c7b24dec1e6))

</details>

<details><summary>Build/toolchain/API surface (21 fixes)</summary>

- Windows.UI.Composition stages 2-3: visual tree (ContainerVisual, SpriteVisual, VisualCollection, color/surface brushes, InsetClip), CompositionGraphicsDevice from D3D11, drawing surfaces ... ([fix/winui23](https://github.com/nitroos-org/reactos/commit/df3d0341083))
- Windows.UI.Composition stages 0-1: Compositor and Core.CompositorController activatable from dcomp.dll (as on Windows ([fix/winui01](https://github.com/nitroos-org/reactos/commit/28e86b9e73d))
- BAD_POOL_CALLER for a pool call at an invalid IRQL prints the caller's stack (with modules) to the debug port first (diagnostic for the FAT-load 0xC2 seen once) ([fix/bugc2](https://github.com/nitroos-org/reactos/commit/467fb2df76a))
- Amd64 SEH plugin turns off tree VRP in SEH functions (VRP jump threading moved try-block code past the scope range, so __finally blocks did not run: 38 functions in an amd64 Release build ... ([fix/fatcreate](https://github.com/nitroos-org/reactos/commit/2b1a1f0df50))
- Kernel32 exports the Win10 names its API sets resolve to (synchronization barriers, token/ACL helpers, GlobalAddAtomEx, disk space information, time zone conversions, NUMA and threadpool ... ([fix/k32exp](https://github.com/nitroos-org/reactos/commit/e4414dea914))
- Advapi32 audit category lookups, CveEventWrite, dynamic time zone enumeration and the perf counter value calls, as Windows 10 answers them (measured) ([fix/advexp](https://github.com/nitroos-org/reactos/commit/08fbdd0cde9))
- Combase exports the HMONITOR/HRGN wire marshallers, the 64-bit handle marshaller names and the standard stub buffer entry points ([fix/comexp](https://github.com/nitroos-org/reactos/commit/a60b1563c35))
- CoreMessaging.dll with CreateDispatcherQueueController and Windows.System.DispatcherQueue, written from the documented API (Word creates a dispatcher queue on its render thread) ([fix/coremessaging](https://github.com/nitroos-org/reactos/commit/ecc1c32178e))
- OOBEComplete, WerRegisterCustomMetadata, GetTempPath2W/A (SYSTEM gets %SystemRoot%\SystemTemp) and RtlConvertDeviceFamilyInfoToString, which Edge Update looks up (ole32 DllCanUnloadNow left ... ([fix/omaha-exports](https://github.com/nitroos-org/reactos/commit/9b999bf926a))
- GPL-2.0-or-later headers on 92 fork-authored source files that had none (third-party and Wine-derived files untouched) ([chore/license-headers](https://github.com/nitroos-org/reactos/commit/50be1ca16b6))
- Programs and Features strings in all appwiz languages ([l10n/appwiz-programs](https://github.com/nitroos-org/reactos/commit/ba88e5fc1d8))
- Kernel32 exports the real BaseThreadInitThunk (x86 __fastcall as Windows), ntdll resolves it by the Windows name (Firefox x86 hook no longer runs threads at address 0) ([feat/basethreadinitthunk-export](https://github.com/nitroos-org/reactos/commit/47a47931cf6))
- AppPolicyGet*, LoadPackagedLibrary, FromApp/Numa file mappings, threadpool Ex, CtrlRoutine, InitiateShutdownW/A (real shutdown), ucrtbase export typos ([fix/missing-exports](https://github.com/nitroos-org/reactos/commit/9b55ec22003))
- RoResolveNamespace rejects/ignores over-long directories as Windows instead of searching a truncated path ([fix/winmd-followups](https://github.com/nitroos-org/reactos/commit/d8eb35e7153))
- Ipconfig translations carry every string ID of the 1:1 rewrite (no blank lines under a non-English UI language) ([fix/l10n-new-strings](https://github.com/nitroos-org/reactos/commit/111989ac548))
- WinMetadata (Windows.*.winmd generated by widl --winmd), wintypes RoResolveNamespace, combase parameterized-type IIDs (.NET WinRT interop, Office C2R) ([feat/winrt-winmd-metadata](https://github.com/nitroos-org/reactos/commit/fe43656a1bf))
- Qt apps load — the loader leaves read-only shared image sections alone when relocating (every relocated Qt DLL failed), Get/SetThreadDescription at the Win7 export level ... ([fix/qt-platform-plugin](https://github.com/nitroos-org/reactos/commit/4acc95bc7a5))
- RtlCrc64 (CRC-64/NVME), RtlExtractBitMap, RtlGetSystemTimePrecise, RtlGetInterruptTimePrecise per oracle ([fix/ntdll-item4-exports](https://github.com/nitroos-org/reactos/commit/dba97a27ba9))
- NTDLL_APITEST: IoctlAllowlist: fix the i386 build (serial constants defined by both winioctl.h and ntddser.h) ([06ca16f55ef](https://github.com/nitroos-org/reactos/commit/06ca16f55ef))
- Dcomp and bcp47langs built at 0x601 with Windows exports, xinput1_4, forward-only sechost/kernel.appcore/msvcp_win, both arches ([fix/wow64-unbuilt-dlls](https://github.com/nitroos-org/reactos/commit/1f172812f3e))
- Pwrshplugin.dll (System32 + SysWOW64) with Windows' 12 exports that report WSMan as not supported, version resource 10.0.x — the stock PowerShell 7 MSIs (x64 and x86) install by ... ([feat/pwrshplugin-msi](https://github.com/nitroos-org/reactos/commit/7ebf991f498))

</details>

<details><summary>Tests only (6 fixes)</summary>

- TerminateProcess ends a process whatever its threads wait on (21 wait kinds, x64/x86) and a CPU-starved idle-priority child within 180 s (Windows: 33-43 s) ([fix/killff](https://github.com/nitroos-org/reactos/commit/430eea75e62))
- MmFaultFileHeld keeps kmtest.sys loaded when a hook caller is stuck at test end (a later service stop returned it into the unloaded image: STOP 0x50), the Touch->Held wait is bounded ... ([fix/kmtest-faultfileheld](https://github.com/nitroos-org/reactos/commit/aa175209e19))
- Register 14 built-but-unregistered apitests ([fix/apitests-unregistered](https://github.com/nitroos-org/reactos/commit/21fdd35f9c2))
- Security hardening: a group of fixes in this area; details are withheld until users have had time to update ([test/regacl-r0](https://github.com/nitroos-org/reactos/commit/c52a96b3823))
- FramelessCaptionDrag apitest (frameless and NCCALCSIZE-captionless windows, Chromium-style child hit test) ([fix/frameless-caption-drag](https://github.com/nitroos-org/reactos/commit/a37bbe1fbe3))
- ShellExecuteExLnk explorer row matches Windows' interactive behaviour (marker via link arguments ([fix/oracle-verified-tests](https://github.com/nitroos-org/reactos/commit/ec6a6bd8ca9))

</details>
### Limitations
- **Pager:** on by default on amd64 only. i386 has it off. `NitroPager=0` (HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management) turns it off on amd64. FAT page files were not reviewed under the pager. On a 1 GB VM, a 1400 MB allocation running at the same time as a `git clone` still fails in 4 of 9 runs (ticket MM-pager-default-1gb-hog-plus-clone-still-fails, not a regression).
- **Locale formats** now come from Unicode CLDR 49 instead of data measured on Windows. Many non-English cultures format dates, numbers and currency differently from Windows (e.g. en-GB, ru-RU, ja-JP, zh-CN long dates; ko-KR, hi-IN, es-ES short dates; ar-SA defaults to the Gregorian calendar with Arabic-Indic digits; en-US negative currency -$2,353.00 instead of ($2,353.00)); .NET apps show these. English (US), German, French and Portuguese (Brazil) match. Closer mapping is planned for 2026.11-2.
- **Live CD desktop:** the live desktop of the boot CD has no 32-bit apps (installed systems are unaffected).
- **Word** stops at its splash screen (Office installs, Word does not start).
- **UIPI is missing:** there is no cross-process window message isolation between integrity levels.
- **VS Code integrated terminal** renders corrupted (only the leftmost part of the canvas shows) with its default WebGL renderer, in about 16 of 18 launches. Workaround: set `terminal.integrated.gpuAcceleration` to `off`, or start VS Code with `--use-angle=swiftshader --enable-unsafe-swiftshader`. Cause: the ANGLE-over-D3D11 path; shared D3D11 resources are not implemented yet (ticket VSCODE-webgl-terminal-glyph-corruption).
- **Installs made before `57c2c276858`** had user-writable system directories (the Windows directory, System32, Program Files, ProgramData, Temp and the drive root). The upgrade boot of this release fixes existing installs.
- **2026.10-5 amd64 binaries** have the SEH `__finally` defect (38 functions across ntoskrnl, ucrtbase, advapi32, kernel32 and fastfat could skip a `__finally` block). This release is built with the fix; upgrade from 2026.10-5.
- Tested under QEMU/KVM only; real-hardware paths (notably the APIC HAL) are not covered.

### Licences
- ReactOS and the fork's own code: GPL-2.0-or-later (0 added files with GPL-2.0-only SPDX identifiers in the release patch).
- Locale data (`kernel32` locale tables and calendar data): generated from Unicode CLDR 49.0.0, Unicode License v3 (Unicode-3.0). The notice ships on the CD and installs as `3rdParty\LICENSE-CLDR` in the system directory (source: `sdk/tools/nlsgen/LICENSE-CLDR`, listed in `media/doc/3rd Party Files.txt`).
- Mesa (MIT), LLVM (Apache-2.0 with LLVM exception) and the Vulkan loader: licence texts ship on the CD next to the DLLs; the DLLs are standalone aggregates.
- Fonts: their licences are in `3rdParty` on the CD.

### Changes since the previous Nitro
This is the first release after 2026.10-5 (published 2026-10-30, `575d4ccb913`). The upstream base is unchanged (`e46f4bb1a1e`); the delta is 592 first-parent changes, all listed under Bug fixes above. The artifacts change too: the ISOs are hosted on repo.nitroos.org, and the source ships as one cumulative patch over `e46f4bb1a1e` instead of a per-topic series.

## reactos-nitro 2026.10-5

**Tagged `nitro-2026.10-5`** (2026-10-30) at fork `main` [`575d4ccb913`](https://github.com/nitroos-org/reactos/commit/575d4ccb91319dc59bdc583c73067556e10a4516). The modern-app release: **Windows 10 is now the default reported identity**, **Chrome and Claude Desktop run** (Chrome with software rendering), **32-bit Firefox renders at the default sandbox again** (32+64-bit side by side), **Firefox 158** and **PowerShell 7** run, and the guest gets a real Task Scheduler, a correct clock and a memory commit cap that stops runaway apps crashing the guest.

| Item | Value |
|---|---|
| Release tag | `nitro-2026.10-5` → [`575d4ccb913`](https://github.com/nitroos-org/reactos/commit/575d4ccb91319dc59bdc583c73067556e10a4516) |
| ReactOS base | upstream [`e46f4bb1a1e`](https://github.com/reactos/reactos/commit/e46f4bb1a1ea7c722f941231565144fe0c286d3e) (2026-10-25, weekly sync #4; 59 upstream commits since the `f53b1d15601` base of 2026.10-2/-3/-4) |
| Patch series | 52 topic patches on `e46f4bb1a1e` (cumulative, whole series); series head `52800c001a5`; applied tree `0ad3bbd2ebde` — verified identical to the release commit. This release's delta over 2026.10-4: 242 changes (236 branch merges + 6 direct commits) plus the upstream sync. |
| Artifacts | amd64-with-WoW64 ISO, i386 ISO, source tarball, patch series |

| Artifact | Size | sha256 |
|---|---|---|
| reactos-nitro-2026.10-5-amd64-wow64-Release.iso | 651165696 | `7664b76950df1138e7a6b06dc0d9624e633dc7eb830d9096867a76fc377319e7` |
| reactos-nitro-2026.10-5-i386-Release.iso | 417333248 | `5ee77829c1ad65c47b217c31c443aab29d64c620a024c71d807d55e4331fe4db` |
| reactos-nitro-2026.10-5-src.tar.gz | 123228851 | `563a941deaadb57449e8b43efdca31e343ff6194fdccc5fccb7fb460eda2281d` |
| nitro-2026.10-5-patch-series.tar.gz | 31016367 | `7917b4e1600d073739c3967e769b9c3e9580f348fe432ca14ac306b85ffed295` |

### Highlights
- **Firefox no longer freezes after a GPU-process crash**: a crashed process now terminates without running DLL shutdown code, as on Windows.
- **PowerShell 7 is usable**: culture-aware sorting and hashing work (jobs, CLIXML, no red errors), HTTPS works through schannel SCH_CREDENTIALS, hostname lookups complete (GetAddrInfoExW), registry tree delete works, and a one-click install is available through the feature manager (`featmgr /install powershell-7`).
- **Scoop works end to end on NTFS**: robocopy, `select()` timeouts and git over HTTPS are fixed; `scoop install 7zip`, `scoop install git` and `scoop bucket add extras` work.
- **WordCraft runs** (`prntvpt.dll`, `windows.data.pdf.dll`), and the **ArtCraft Launcher installs apps** (standard-handle fix).
- **RTL thread pool reworked to behave like Windows** (gradual worker injection, long-function work items, persistent work items).
- **Windows 10 identity by default** (10.0.19045): modern apps that gate on `VersionNT` / Win10 now launch. Backed by a version-gated NT6–Win10 API parity batch (UTF-8 `WideCharToMultiByte`, user32/win32k, shell32, object manager, named pipes, kernelbase exports, Win8 pointer-input exports, `NtOpenKeyEx`, `ProcessHandleTable`, TokenElevation, XState/CONTEXT_EX).
- **Chrome works and is stable for long sessions** — with software rendering (`dcomp.dll` is absent, so Chrome runs with software compositing, set by default policy). Stock Chrome 156 installs (including the enterprise MSI), paints and browses remote sites with no flags; Chrome 3-hour soaks ran with no crashes; QUIC, `GetAddrInfoEx`, NLA network-change, registry change-notification and registered-wait fixes behind it.
- **Claude Desktop works**: installable from the feature manager, renders and connects online, sandboxed and unsandboxed; paints reliably under host load (winmm `timeGetTime` fix); stable in 3-hour soaks.
- **Firefox 32-bit renders at the DEFAULT sandbox again** — "32-bit + 64-bit side by side" is true at default settings (no `security.sandbox.content.level=0` workaround needed). Firefox 158 x86 and x64 start and render; Firefox 128 x64 renders with the sandbox on.
- **PowerShell 7 runs** (.NET 8 CoreCLR): XState/CONTEXT_EX context API, IL-only AnyCPU assembly loading, `FlushProcessWriteBuffers`, `FindNLSStringEx` and locale fixes underneath the usability work above.
- **Task Scheduler 2.0** (`taskschd.dll`, `schedsvc`, triggers) — the Chrome auto-update task fires; real COM surrogate (`dllhost.exe`).
- **Guest clock fix**: the TSC frequency is taken from the hypervisor, drift ≤0.004% under heavy load (was tens of percent).
- **Memory commit cap**: commit is limited to what RAM and pageable memory can back, so a runaway app gets allocation failures instead of crashing the whole guest; critical processes keep a reserve.
- **Optional Features / feature manager** (Control Panel + `featmgr`): install/enable/disable/uninstall with dependencies, software tier (rapps import), GPU pack (lavapipe/llvmpipe) and WinFsp as add-ons; first-login default-browser chooser.
- **WinFsp file systems**: WinFsp 2.x installs (MSI, including its 32-bit custom actions under WoW64) and mounts user-mode file systems on a drive letter and UNC paths. The memfs sample is verified; killing a file-system server cleanly unmounts it and a remount succeeds. Also available as an Optional Feature.
- **Graphics**: Mesa software OpenGL (llvmpipe) and lavapipe Vulkan with an in-box Khronos Vulkan loader on amd64; virtio-gpu 2D display driver.
- **Four new app-compat DLLs**: `tbs`, `wer`, `ndfapi`, `wevtapi`.
- **Many WoW64, networking and kernel fixes** — WoW64 thunk batches (AcceptEx/ConnectEx, LSA, registry notify, console), NDIS/tcpip lifetime and request-serialization fixes, UDP/Winsock semantics, Vista IP-helper APIs, KDBG/NMI robustness on SMP, plus a security hardening batch (described generically below).

### What reactos-nitro adds on top of ReactOS
#### OS identity and API parity
Win10 as the default reported identity (`VERBUMP_TIER=100`) with a version-gated parity layer that keeps NT 5.2 behaviour safe; NT6+/Win8+ semantics in the object manager, named pipes, section mapping, `NtClose`, shell32, appshim/apphelp, kernelbase (≈720 forwarded exports, a real import library, AppContainer moniker APIs), user32 pointer-input APIs and the CRT/ucrtbase.
#### Application compatibility
Chrome (installer, updater, sandbox, QUIC, omnibox/registry-watch, renderer delay-load), Claude Desktop (feature-manager package, per-machine wined3d settings, paint reliability), Firefox (32-bit sandbox under WoW64, 158 x86 dependencies, about:support), PowerShell 7 (CoreCLR prerequisites), a COM surrogate host, Task Scheduler 2.0, and the registered-wait thread pool reworked to Windows semantics (shared wait threads, pool dispatch), the worker thread pool reworked to inject threads like Windows, process-crash semantics without DLL detach, and PowerShell 7 / Scoop support (schannel SCH_CREDENTIALS, culture-aware sort keys and hashing, asynchronous GetAddrInfoExW, RegDeleteTree, robocopy, select() timeouts, a feature-manager entry), plus WordCraft and ArtCraft Launcher support.
#### WoW64
New thunks (`NtOpenKeyEx`, `NtNotifyChangeKey`, AFD AcceptEx/ConnectEx super-accept/connect, datagram receive, LSA logon/sessions, console), 32-bit COM registration, more SysWOW64 DLLs (`cfgmgr32`, `avrt`, `credui`, `oledlg`, `hid`, schannel, games/media), 64-bit-wide NtUserPfn tables, and 32-bit Chromium-style sandboxes.
#### Networking
Reference-counted IP interfaces and neighbour entries, NDIS request serialization/unbind/halt fixes, UDP zero-length/connect/`getsockname` semantics, `WSARecvMsg` with `IP_PKTINFO`, refused-connect `SO_ERROR`, `NotifyIpInterfaceChange`/`NotifyRouteChange2`, Vista interface and unicast tables, NLA provider, ChaCha20-Poly1305 client suites and TLS error codes.
#### Kernel, memory and SMP
Commit cap and critical-process tier, guest timekeeping, ExInterlocked/IPI deadlock fix, KDBG/NMI freeze fixes on amd64/x86 SMP, I/O-manager port-request cancel on close, PnP disable/re-enable persistence, process-quota and cache-manager races, WinFsp support: MUP UNC paths and ECP delivery, FsRtl/Rtl exports, reparse-point and junction handling, and recovery when a file-system server is killed (the terminating-thread APC deadlock fix).
#### Graphics
Mesa llvmpipe/lavapipe + Vulkan loader (amd64), virtio-gpu 2D XDDM driver, wined3d machine-wide configuration.
#### Setup, services and shell
SCM failure actions, bitness-independent service status, real surrogate/RPCSS context rundown, default-browser chooser and `UserChoice` handling, `.lnk` property-store persistence, jump-list COM classes, optional-features CPL.

## Bug fixes

242 changes: one line for each of the 236 branches merged into Nitro main and each of 6 small changes committed directly on main since `nitro-2026.10-4`, grouped by area (click an area to open it). Security-related fixes are described generically on purpose. This is the delta of this release only; it includes everything from `nitro-2026.10-4` (whose section is below). The weekly upstream ReactOS sync (0036d5334c8) is not listed here.

<details><summary>Application compatibility (Chrome, Claude Desktop, Firefox) (34 fixes)</summary>

- advapi32: Chrome enterprise MSI installs run silently; the service-control protocol is now bitness-independent for 32-bit services. ([c783084710a](https://github.com/nitroos-org/reactos/commit/c783084710a2ee3bf7935d3a44754479341aa456))
- advapi32: GetFirmwareType + SystemBootEnvironmentInformation + AddConditionalAce ([8c15c304902](https://github.com/nitroos-org/reactos/commit/8c15c304902155a6d115843a4a4a42cd84ec7a97))
- boot bootdata: ship 32-bit cfgmgr32, avrt, credui in SysWOW64 (Firefox 158 x86 starts) ([5217dc8d9d0](https://github.com/nitroos-org/reactos/commit/5217dc8d9d00a4bd71d79f9480d964efcaa9bbe7))
- boot bootdata: ship 32-bit oledlg.dll in SysWOW64 ([200cf391e58](https://github.com/nitroos-org/reactos/commit/200cf391e58bd6d5bdeca4cf39323dbe864dc10f))
- browserchooser: first-login default-browser chooser ([243fc91265a](https://github.com/nitroos-org/reactos/commit/243fc91265af543318c3540b4f4e3ef88733e083))
- browserchooser: don't block unattended setup on the chooser ([ba1f2a51a17](https://github.com/nitroos-org/reactos/commit/ba1f2a51a17dfe6a249c7b6111e222dd0da98dd3))
- browserchooser: wait for network before RAPPS Firefox install ([fd161017941](https://github.com/nitroos-org/reactos/commit/fd161017941c2d18b8c498bf7b56e5c693617374))
- combase: COM surrogate: review fixes for dead-server retry, external locks and reference tracking at disconnect. ([7a1cec5a965](https://github.com/nitroos-org/reactos/commit/7a1cec5a96565a89a569961932f8a3173d9f2ba2))
- DLLHOST: real COM surrogate (was a stub): /Processid:{CLSID}, hosts the in-proc server; shipped in SysWOW64 too ([7632937a1f2](https://github.com/nitroos-org/reactos/commit/7632937a1f238046db08c781fb8408e9fa6ea7e5))
- featmgr: Feature manager: add a 'claude-desktop' catalog entry that downloads Anthropic's official package at install time (checksum-verified). ([d2637c3b88f](https://github.com/nitroos-org/reactos/commit/d2637c3b88f76051d45e5f86f1fa3231fb928ccf))
- kernel32: honour PROC_THREAD_ATTRIBUTE_JOB_LIST (Chrome sandbox registered-wait leak) ([31fcd2ead00](https://github.com/nitroos-org/reactos/commit/31fcd2ead006c9ea1086e81ee65cc2f1203a8b4d))
- kernel32: PowerShell 7 runs: XState/CONTEXT_EX context API, IL-only AnyCPU assembly loading, FlushProcessWriteBuffers, FindNLSStringEx and locale fixes. ([ef0a0af93ae](https://github.com/nitroos-org/reactos/commit/ef0a0af93aed4db4fafd94fdf0cad7e0233664ea))
- kernel32/ntdll: a crashed process terminates without running DLL detach or atexit code, as Windows does; fixes Firefox freezing after a GPU-process crash. ([607836aada2](https://github.com/nitroos-org/reactos/commit/607836aada22cf19e76930b02e5b386c480eabf8))
- kernelbase: fix Chrome renderer 'Aw, Snap' crashes: QueryUnbiasedInterruptTimePrecise now resolves in the locked-down renderer ([7dd216cf820](https://github.com/nitroos-org/reactos/commit/7dd216cf820d97a46989d83e5684d50230fcb65c))
- msafd: msafd WSARecvMsg (Chrome QUIC works) ([26ed0cd40be](https://github.com/nitroos-org/reactos/commit/26ed0cd40be43689b783416775800f498fd30ff4))
- npfs: record the pipe server process ID and implement GetNamedPipeServerProcessId. ([31ef7501f8d](https://github.com/nitroos-org/reactos/commit/31ef7501f8d62072a3d2edce29667d6be568c882))
- NtNotifyChangeKey: NtNotifyChangeKey resets the caller's event when arming a request, fixing a Chrome omnibox busy loop. ([6cbe625b8ab](https://github.com/nitroos-org/reactos/commit/6cbe625b8ab880c3441865917de17e7490c0478f))
- ntoskrnl config: Implement registry change notification (RegNotifyChangeKeyValue/NtNotifyChangeKey), including the WoW64 thunk. ([fd0898cf566](https://github.com/nitroos-org/reactos/commit/fd0898cf566a79715c5c1a602cb2f08fb5b5834c))
- ntoskrnl ex: Terminate the SystemProcessInformation list correctly, fixing a Firefox about:support crash. ([deae7cf6e5c](https://github.com/nitroos-org/reactos/commit/deae7cf6e5ca517852e7f5d92a635778bd0044db))
- rpcrt4: Chrome 156 installs, paints and browses with no flags; default policy selects software compositing, plus supporting WoW64 and RPC fixes. ([35cef67c493](https://github.com/nitroos-org/reactos/commit/35cef67c49351c3c3f2838b4bd2c71096c57fc41))
- RPCSS: RPCSS and COM: purge a dead surrogate's class registrations and retry dead-server activation with a fresh launch. ([cfcbf5f8cba](https://github.com/nitroos-org/reactos/commit/cfcbf5f8cba22413ffce2f48659d8e4000383c30))
- rtl: fix a sandboxed-browser content-process crash (Firefox 128 renders with the sandbox on) ([e8b88d0afc8](https://github.com/nitroos-org/reactos/commit/e8b88d0afc8831d8e36b63d268df9e19767b267e))
- rtl: registered-wait callbacks are wrapped in SEH so cleanup always runs and an exception cannot leak a wait. ([da636fca9f9](https://github.com/nitroos-org/reactos/commit/da636fca9f9f796e9b80f33d458c6b316571b6b1))
- rtl: registered-wait callbacks run in the pool as Windows (oracle-measured semantics) ([85ed5249d4b](https://github.com/nitroos-org/reactos/commit/85ed5249d4bc92228de331cbef40b3016ddc9e7e))
- rtl: registered waits share wait threads, up to 63 waits to a thread ([b69d22e2da4](https://github.com/nitroos-org/reactos/commit/b69d22e2da42d6fb66673aaef1ce4677f1e14343))
- rtl: registered waits no longer use the capped worker pool; they run on their own wait threads. ([eca063879b3](https://github.com/nitroos-org/reactos/commit/eca063879b3d32a84967eb46c62cbf61a92ce536))
- SHELL32: ShellExecute/AssocQuery honour the user's default-browser choice for URL protocols; browser chooser gains /settings ([39fbf2bd787](https://github.com/nitroos-org/reactos/commit/39fbf2bd787f674598ed0200e6e83624774170da))
- win32k: SC_MAXIMIZE on a hidden top-level window maximizes it without showing it (Chrome --start-maximized). ([fabb257e3df](https://github.com/nitroos-org/reactos/commit/fabb257e3dfeccefaa4c55051fe535879f2f6b57))
- win32k: non-interactive desktop heap now matches Windows, fixing Chrome updater and dllhost DLL-init failures. ([6837ee878dc](https://github.com/nitroos-org/reactos/commit/6837ee878dcd103daa15c85ce23012be50ccbce6))
- win32ss user: SC_MAXIMIZE/SC_MINIMIZE are no-ops when already in that state (Windows); revert keep-hidden ([b56fbda2f2a](https://github.com/nitroos-org/reactos/commit/b56fbda2f2ae7a1873c26d6c2c9ffd672c971c0a))
- wined3d: read machine-wide Direct3D settings when the per-user value is absent, so every user gets a painting Claude window. ([e3fe00c267a](https://github.com/nitroos-org/reactos/commit/e3fe00c267ae3e12250314477fae02cf0b0e3303))
- winmm: timeGetTime always reads interrupt time as Windows does; it stepped backwards and crashed Chromium. ([7bb2ab20134](https://github.com/nitroos-org/reactos/commit/7bb2ab2013480bbcd87ee5010f64fbc95b84e0bd))
- wow64: 32-bit Chromium-style sandboxes work under WoW64 (Firefox 32-bit renders at the default sandbox) ([f455dc3d462](https://github.com/nitroos-org/reactos/commit/f455dc3d462fff7632542c310060345d921ecb97))
- ws2_32: implement GetAddrInfoExW/FreeAddrInfoEx(W) + fix GetAddrInfoW AI_CANONNAME heap under-alloc ([7f835d77b23](https://github.com/nitroos-org/reactos/commit/7f835d77b232615984470c3d398deaf81ec284c9))

</details>

<details><summary>Windows 10 identity and NT6/Win8+ parity (29 fixes)</summary>

- afd: afd.sys accepts both NT5 and NT6 AFD_CREATE_PACKET layouts ([00ec0eb4971](https://github.com/nitroos-org/reactos/commit/00ec0eb49717f1fa9bd30585e03f1426b1d0f453))
- appcompat: appshim and apphelp: Win8 and later conformance. ([f2e3d7fc4ac](https://github.com/nitroos-org/reactos/commit/f2e3d7fc4ac452661b6c3d494cc04a60a7c995f8))
- boot bootdata: Flip main to the Windows 10 identity by default. ([7605b5c9813](https://github.com/nitroos-org/reactos/commit/7605b5c98130520403fc1970821e877e6026ee1f))
- boot bootdata: Add the VC++ runtime registry keys and ucrtbase version information. ([5c4581a31bf](https://github.com/nitroos-org/reactos/commit/5c4581a31bf1c2b2adcd17d10e81a7f6672efcd2))
- include: Make the Windows 10 identity data (version strings and registry values) consistent. ([0e34518569f](https://github.com/nitroos-org/reactos/commit/0e34518569f4004e520a9dc1bd7e5417dd012ab7))
- kernelbase: implement the AppContainer SID and moniker mapping APIs as Windows does. ([0ccd81a148d](https://github.com/nitroos-org/reactos/commit/0ccd81a148d13e2caf64047e5d9e76b2013652f1))
- kernelbase: forward ~720 kernelbase.dll exports to kernel32 ([f44d5b597b0](https://github.com/nitroos-org/reactos/commit/f44d5b597b0647c8f66fd9cb0e1aca0397e99485))
- kernelbase: build a real import library so shlwapi and vbscript load kernelbase.dll statically. ([24184568504](https://github.com/nitroos-org/reactos/commit/241845685040c1bf9b8ed68ab42c89cc04e2eee2))
- msvcrt: Executive, process, memory, I/O and CRT: 36 Windows 10 conformance fixes. ([cefc93b26a3](https://github.com/nitroos-org/reactos/commit/cefc93b26a38e4368f9e3bfb7922062de07c93dd))
- npfs: cache the Vista-or-later check at driver entry and add a prefix-shadow regression test. ([cbb067edbfa](https://github.com/nitroos-org/reactos/commit/cbb067edbfa7de79115d8c00115364b6fb211506))
- npfs: Vista and later named-pipe semantics. ([b3c3631e1ac](https://github.com/nitroos-org/reactos/commit/b3c3631e1ac411717d5f42ffdeee4aae3a07e71d))
- ntdll: Windows 10 object-manager parity. ([7cedf53f358](https://github.com/nitroos-org/reactos/commit/7cedf53f358eec5422e45fec32f8f5ec9b9e3251))
- ntdll: Pick the highest supported-OS version a manifest declares at or below Windows 7, so Burn/WiX installers no longer fail. ([4ff58178197](https://github.com/nitroos-org/reactos/commit/4ff5817819792fcd70517da563cef8ce2ce95a33))
- ntdll: RtlGetNtVersionNumbers honors effective process version ([2ebe6db0fc1](https://github.com/nitroos-org/reactos/commit/2ebe6db0fc188ebcaa0e3d0ee4e5c6461582b2c3))
- ntoskrnl ex: NtSetSystemInformation and SetWindowPlacement: NT 6.1+ parity. ([d54919563b4](https://github.com/nitroos-org/reactos/commit/d54919563b42b33b18e7050b04871c8dd2648b24))
- ntoskrnl ex: RtlCaptureStackBackTrace NULL buffer, Win10 NtClose of pseudo handles, ExRaiseHardError unknown options ([27178a636e4](https://github.com/nitroos-org/reactos/commit/27178a636e4dd38a23723836f83b96059f68d6db))
- ntoskrnl kd64: NtQuery/SetDebugFilterState default mask for out-of-range ComponentId ([997f709de32](https://github.com/nitroos-org/reactos/commit/997f709de3246be11d593cb05c07b538504d202a))
- ntoskrnl mm: section creation and map-view validation as Windows 8+/10 ([8f55b0bb2b0](https://github.com/nitroos-org/reactos/commit/8f55b0bb2b0f103980c75ac1d585239f8108992b))
- ntoskrnl mm: ntoskrnl mm: return plain STATUS_INVALID_PARAMETER for an invalid ZeroBits value at Windows 10. ([f0abb540e39](https://github.com/nitroos-org/reactos/commit/f0abb540e394756e5c39be502d9e3eac3a575ce5))
- ntoskrnl se: NtQueryInformationToken(TokenHasRestrictions) ([70e62f360d1](https://github.com/nitroos-org/reactos/commit/70e62f360d1e68b95c2d0275bece0b4168a7e12c))
- ntoskrnl se: Implement the TokenElevation and TokenElevationType token information classes with Windows semantics. ([f4709241c60](https://github.com/nitroos-org/reactos/commit/f4709241c60cf0065f9d1c0a68591bf65c6b1bfd))
- shell32: CIDLData empty-GetData status + comment nits ([3be9b0eacf5](https://github.com/nitroos-org/reactos/commit/3be9b0eacf5691f01848bd83f224efbf80d4e4b3))
- shell32: NT6+ behaviours. ([39db2208da8](https://github.com/nitroos-org/reactos/commit/39db2208da8c73bb63bc4a88a26eb326c8cf3c8f))
- shlwapi: fix string-function crashes (parser and CRT streamout). ([d527453c843](https://github.com/nitroos-org/reactos/commit/d527453c843cb61b0ec81ad1f743cf709d49dc01))
- user32: fix InternalGetWindowIcon, CopyImage stretching and a non-client-size result. ([ed9c762b93c](https://github.com/nitroos-org/reactos/commit/ed9c762b93cea6d9c9243804ff01654f4206e0da))
- user32: gate five Win8 pointer exports at 0x601 and document ChangeWindowMessageFilterEx behaviour. ([007f31a74a5](https://github.com/nitroos-org/reactos/commit/007f31a74a53ba9f5da2a4124ecba340588c0582))
- WideCharToMultiByte: Vista and later UTF-8 semantics, plus NLS fixes. ([3ce1faee3c6](https://github.com/nitroos-org/reactos/commit/3ce1faee3c60dcbc2bb076a5922ffa896d917415))
- win32ss user: user32 and win32k: Vista to Win8 window, class and character-handling parity, version-gated. ([5acaebcaf8d](https://github.com/nitroos-org/reactos/commit/5acaebcaf8d6434f273518f21886905ed228effa))
- win32ss user: Win8 pointer/input/window-helper user32 exports (0x601 floor) ([ecd015a35a6](https://github.com/nitroos-org/reactos/commit/ecd015a35a6b48e69fbc903cf0c93c700d14dc75))

</details>

<details><summary>Optional Features / feature manager (7 fixes)</summary>

- featmgr: Optional Features / addon manager (first slice) ([06ad7ec9d10](https://github.com/nitroos-org/reactos/commit/06ad7ec9d10558dd103896bdda3c576826132750))
- featmgr: optfeatures two-tier tabs + Install/Enable/Disable/Uninstall + state/guards ([c44075866fa](https://github.com/nitroos-org/reactos/commit/c44075866fa9392db2dd9bfceff0b68cf4695714))
- featmgr: featmgr dependency-aware disable + real uninstall + queryable state + CLI filters ([70136914b1e](https://github.com/nitroos-org/reactos/commit/70136914b1e1426e87c446dea08d7f5e4c4088be))
- featmgr: /import-rapps converter + software-tier schema/state/install ([29e60150044](https://github.com/nitroos-org/reactos/commit/29e60150044d05a45835ae47cd10ee51ce56f93b))
- featmgr: lavapipe GPU pack as an installable Optional Feature ([342b42976f4](https://github.com/nitroos-org/reactos/commit/342b42976f4a78dc7e3606ac93f687678099aed4))
- featmgr: WinFsp as a functional OS-feature addon ([c3c52e04970](https://github.com/nitroos-org/reactos/commit/c3c52e04970af0a485e716db22a58ed11cfe86d7))
- optionalfeatures: Optional Features in Control Panel (optionalfeatures.cpl + appwiz hook) + Windows-native features dialog ([98686a78674](https://github.com/nitroos-org/reactos/commit/98686a78674c96092fbc36d9f021beac127142fa))

</details>

<details><summary>Graphics (Mesa, lavapipe, Vulkan, virtio-gpu) (6 fixes)</summary>

- 3rdparty: lavapipe/llvmpipe as the default GPU (amd64) ([e718b3cd68e](https://github.com/nitroos-org/reactos/commit/e718b3cd68ee601550a6b021eafa85a6e85d2eee))
- 3rdparty: Ship Mesa softpipe as the default OpenGL driver and add llvmpipe and lavapipe Vulkan as an optional image component. ([0d301733f35](https://github.com/nitroos-org/reactos/commit/0d301733f35a25f23d7ffe051df654f846ede604))
- 3rdparty: Ship the in-box Khronos Vulkan loader (v1.4.366), dispatching to llvmpipe/lavapipe. ([0a9736f882d](https://github.com/nitroos-org/reactos/commit/0a9736f882d4c0ac8281896c755509f9d4aeeb1e))
- drivers: fix ([70d1b523c8d](https://github.com/nitroos-org/reactos/commit/70d1b523c8d8e1eb865406b59f8198f68d544952))
- featmgr: featmgr Removable flag; lavapipe-pack non-removable base component ([4540d5d6e54](https://github.com/nitroos-org/reactos/commit/4540d5d6e544a888c75967f48dba039c862886d5))
- win32ss drivers: Add a virtio-gpu 2D display driver (XDDM miniport) that works with the stock framebuffer driver. ([da04826afcc](https://github.com/nitroos-org/reactos/commit/da04826afcc338f5dee594c9cce09eb936435b1b))

</details>

<details><summary>Kernel: Ke, HAL and amd64 (11 fixes)</summary>

- HAL: take the TSC frequency from the hypervisor when it reports one (guest clock drift fix; <=0.004% under heavy load) ([cfa6f71705f](https://github.com/nitroos-org/reactos/commit/cfa6f71705fc79affa6148ed9e2eb4f29b0cab71))
- KD: no time-slip DPC before debugger init, and KdSystemDebugControl is inactive without a debugger. ([12c30bcbf14](https://github.com/nitroos-org/reactos/commit/12c30bcbf14b1b848cc478a5561f6e41fdd36c26))
- KDBG: thread attach and backtrace now work on amd64 and stop safely on bogus frames. ([08ada49d0d2](https://github.com/nitroos-org/reactos/commit/08ada49d0d2afb46b827e8f6f76842a5d29d1b90))
- Ke: spin on a busy ExInterlocked lock with the caller's interrupt state, fixing an IPI deadlock. ([085c7098311](https://github.com/nitroos-org/reactos/commit/085c7098311292eb98f236efc43bd965526797b7))
- ntoskrnl kd64: !gdi.* KDBG commands print through KDBG and no longer hang ([db7ef607da1](https://github.com/nitroos-org/reactos/commit/db7ef607da1bb883d457d989d19185f91281e902))
- ntoskrnl ke: tolerate repeated and simultaneous NMIs while the debugger freezes processors ([271f83b4fb3](https://github.com/nitroos-org/reactos/commit/271f83b4fb35f226b3b609e2556b57f3e31194d9))
- ntoskrnl ke: fix debugger freeze hangs when NMIs arrive during a session on amd64 SMP ([615b10d55e8](https://github.com/nitroos-org/reactos/commit/615b10d55e8b5a11c571729c0b267902334ea930))
- ntoskrnl ke: NMI grace window calibrated, separate grace-drop counter, port guard, KeFreezeNmiDpc test ([6208ac3bcd7](https://github.com/nitroos-org/reactos/commit/6208ac3bcd7595a5cc87d58c506257945d1413a5))
- ntoskrnl ke: Set the bugcheck-callback checksum as Windows does, so non-Reason bugcheck callbacks run. ([d48aeca4aa1](https://github.com/nitroos-org/reactos/commit/d48aeca4aa1850857009d4af9e37739e9e7e27e1))
- ntoskrnl ke: AtDpcLevel spinlock IRQL checks only on checked builds, as Windows ([28a8220c554](https://github.com/nitroos-org/reactos/commit/28a8220c554188da26f84ac3700010c314dffa5b))
- win32ss user: win32k registers its KDBG callback once (DriverEntry) ([ee2a10fc5ff](https://github.com/nitroos-org/reactos/commit/ee2a10fc5ffbd13752b4f4d1d15c4778c0c5072a))

</details>

<details><summary>Kernel: Mm and Cc (6 fixes)</summary>

- cc: fix a race on duplicate buffer-control blocks taking an exclusive pin. ([849db3bac95](https://github.com/nitroos-org/reactos/commit/849db3bac954d4adace5981f2cd12c829e524380))
- cc: CcPinMappedData no longer unpins a NULL buffer-control block. ([84f17b87891](https://github.com/nitroos-org/reactos/commit/84f17b8789146c07b8776cb1b5f49b7f9e037732))
- ntoskrnl cc: fix two regressions from earlier hardening (cache-manager reference count, message-queue callback leak) ([f9ec3197a60](https://github.com/nitroos-org/reactos/commit/f9ec3197a60f1ba996235e02ed9ec487390b319b))
- ntoskrnl cc: serialize Cc BCB PinCount under BcbSpinLock ([f57ab4c50ac](https://github.com/nitroos-org/reactos/commit/f57ab4c50ace0890b0b5be8fcb1037538d7e23b1))
- ntoskrnl mm: Memory manager: commit only what RAM and pageable memory can back, so runaway apps fail allocations instead of crashing the guest. ([224390a00e2](https://github.com/nitroos-org/reactos/commit/224390a00e2488d507ebc15c6b8a5b8b35181641))
- ntoskrnl mm: allow owner reprotect of PEB/TEB NoChange VADs ([bc5e07ac90c](https://github.com/nitroos-org/reactos/commit/bc5e07ac90c7a9edbed127ce25469b0a3ef1ebda))

</details>

<details><summary>Kernel: Ob, Se, Ps, Ex and registry (8 fixes)</summary>

- advapi32: RegDeleteTree(hKey, "") keeps the key handle open as on Windows; fixes PowerShell Remove-Item -Recurse on registry keys. ([be196deddb7](https://github.com/nitroos-org/reactos/commit/be196deddb7be3730ea1aa5cf3822aac5f0e11cf))
- NtFilterToken: SIDs to disable now become deny-only as on Windows. ([28b13e31eb2](https://github.com/nitroos-org/reactos/commit/28b13e31eb232eab0e0508eaa3fa498e9994a70e))
- ntoskrnl config: implement NtOpenKeyEx / ZwOpenKeyEx ([0c3249c2299](https://github.com/nitroos-org/reactos/commit/0c3249c229940d30ffacb624607a5a124328797c))
- ntoskrnl ex: add NOHANDLECACHE boot option as a handle-cache kill-switch ([c4c7cedf479](https://github.com/nitroos-org/reactos/commit/c4c7cedf479cc3690f47ed5990803fec99436b26))
- ntoskrnl include: implement NtQueryInformationProcess(ProcessHandleTable) ([52b2ec398a9](https://github.com/nitroos-org/reactos/commit/52b2ec398a9f9fa7b718e4d4300080624dad182c))
- ntoskrnl ob: Restore Windows DuplicateHandle access-widening semantics (corrects a 2026.10-4 regression) and apply security descriptors given at creation. ([f514b1afe11](https://github.com/nitroos-org/reactos/commit/f514b1afe11dfa69fc345612bd8bd0c148653e32))
- ntoskrnl ps: PspCreateThread no longer wipes the process granted-access mask ([d3687656b8d](https://github.com/nitroos-org/reactos/commit/d3687656b8d1db5f1ff0eac63385cb5be477f197))
- PipGetDriverTagPriority: validate hive-supplied driver group-order data instead of asserting (hardening) ([e0871a89792](https://github.com/nitroos-org/reactos/commit/e0871a89792cff7f51fd43e882d95c649bf7ddf9))

</details>

<details><summary>I/O manager and file-system libraries (11 fixes)</summary>

- I/O: limit a single MDL to 64 MB on 32-bit systems (restores Windows behaviour for large unbuffered writes on i386; 64-bit unchanged). ([71b0d80ee81](https://github.com/nitroos-org/reactos/commit/71b0d80ee81d5427eb460136e11324dd2c711b5b))
- kernel32: CreateFile makes a directory for POSIX+DIRECTORY+CREATE_NEW ([8f17dce29c8](https://github.com/nitroos-org/reactos/commit/8f17dce29c8b551a733b0286aeac17cd37a81c50))
- msi: 1/2): 32-bit MSI custom actions via WoW64 CA server + reparse-point FSCTL passthrough ([5a15e7673c4](https://github.com/nitroos-org/reactos/commit/5a15e7673c4cffc138ea7f0822550ec632f49d87))
- mup: WinFsp UNC paths + ECP delivery ([d24582ebe9e](https://github.com/nitroos-org/reactos/commit/d24582ebe9e8703b32116f435cf4567f0346af9a))
- ntoskrnl fsrtl: WinFsp enablement (kernel/Rtl exports) — WinFsp mounts ([75ebde4a688](https://github.com/nitroos-org/reactos/commit/75ebde4a68857b59a9891a10f598c9662f41051d))
- ntoskrnl io: Cancel a process's port-only requests when it closes its last handle. ([354a3fecc14](https://github.com/nitroos-org/reactos/commit/354a3fecc14aadb8bdcc1bbed80cd4e697d1664e))
- ntoskrnl io: keep a disabled device's node so it can be enabled again ([d265af14616](https://github.com/nitroos-org/reactos/commit/d265af14616b3ac987b30704793804829f846370))
- PnP: a disabled device stays disabled across reboot, and state changes require an administrator. ([9dfc9efc68e](https://github.com/nitroos-org/reactos/commit/9dfc9efc68e99c0a9ddbe19f3dddd89864e1d179))
- PnP: read the driver tag priority from the correct GroupOrderList key. ([2f0d6b50d0e](https://github.com/nitroos-org/reactos/commit/2f0d6b50d0e4ac0f81dfc82ddec1326f60387ebe))
- tests: add an ntfsng reparse-point routing regression test. ([ea137543c13](https://github.com/nitroos-org/reactos/commit/ea137543c1392e78882d176275c486fe59f7144f))
- WinFsp: alert a terminating thread out of an alertable wait so its exit APC runs, fixing a hang when a file-system server is killed. ([a0e34195281](https://github.com/nitroos-org/reactos/commit/a0e34195281933d14281f06e1b0e644bf14fe438))

</details>

<details><summary>win32k, GDI and USER (16 fixes)</summary>

- tests: positive-control tests and logging for the message-handling hardening. ([1d4abd09ad5](https://github.com/nitroos-org/reactos/commit/1d4abd09ad5aa279c36c662c20e120143add29fc))
- win32k: fix a memory-safety bug in hook-chain teardown ([e855f0a5c48](https://github.com/nitroos-org/reactos/commit/e855f0a5c485036db60799f245e1aa568169df75))
- win32k: fix a memory-safety bug in hook installation ([a98f19b446c](https://github.com/nitroos-org/reactos/commit/a98f19b446cc8a19e0daac9691dbbd81018d4364))
- win32k: validate window-position message parameters at the syscall boundary ([b207c4f4f35](https://github.com/nitroos-org/reactos/commit/b207c4f4f354598490e86f97fc2b08ce4dee8a72))
- win32k: menu state hygiene on destroy and focus tracking ([4c8b1be1d4e](https://github.com/nitroos-org/reactos/commit/4c8b1be1d4ee59b46977f960b523707c8db628ee))
- win32k: menu, hook and class hardening ([f78f862089b](https://github.com/nitroos-org/reactos/commit/f78f862089b3df23c7e6f49d197dda501f467985))
- win32k: fix a scroll-bar subclassing regression introduced by the hardening ([60e7287ae62](https://github.com/nitroos-org/reactos/commit/60e7287ae62e06139082dc6b32d44791c38830ef))
- win32k: return NULL instead of a bogus pointer when scroll-bar data is missing; callers already handle NULL. ([8230a926926](https://github.com/nitroos-org/reactos/commit/8230a926926872a621da9f0f322d83858ed8980b))
- win32k: fix a memory-safety bug in message-queue teardown on thread exit. ([371bfe68589](https://github.com/nitroos-org/reactos/commit/371bfe68589de0f69199dbc81897928918c0fb91))
- win32k: security hardening of internal window-message handling (local-privilege class) ([926dde795bf](https://github.com/nitroos-org/reactos/commit/926dde795bfbd04e64b642e24d36cb661afcb062))
- win32k: harden user-pointer handling in message and callback paths ([cce177b56f5](https://github.com/nitroos-org/reactos/commit/cce177b56f5a66f7a98623555a0de11c05c8da32))
- win32k: further hardening of window-procedure dispatch and menu data ([a0b5c28f73c](https://github.com/nitroos-org/reactos/commit/a0b5c28f73cb70383f2c1c3ade1d7b9166ed8fa8))
- win32ss user: deliver cross-thread SendNotifyMessage after sender exits ([1543ea89788](https://github.com/nitroos-org/reactos/commit/1543ea897881a23e987d129e8ff65fda5355e523))
- win32ss user: restore desktop DT_GWL_* slots (B6 regression) + winpos parent-lifetime ([8c8c30d9791](https://github.com/nitroos-org/reactos/commit/8c8c30d9791917889c2b97eeb580ca48fd563810))
- win32ss user: SPI_GETFOREGROUNDLOCKTIMEOUT defaults to 200000 ([5601463dd0d](https://github.com/nitroos-org/reactos/commit/5601463dd0d1443126acc0ab5fad33e54f399f3c))
- wow64: Hardening batch across win32k and system DLLs. ([f51f8c06cce](https://github.com/nitroos-org/reactos/commit/f51f8c06cceef2507e1912d204018bce2964abe8))

</details>

<details><summary>Console and csrss (1 fix)</summary>

- kernel32: GUI processes get NULL standard handles instead of stale ones, and USESTDHANDLES without inheritance gives NULL; ArtCraft Launcher installs apps. ([301a087b4c5](https://github.com/nitroos-org/reactos/commit/301a087b4c518b88991481efa7802b47a6cda345))

</details>

<details><summary>WoW64 (16 fixes)</summary>

- boot bootdata: Ship the 32-bit schannel.dll in SysWOW64 so 32-bit applications get working TLS under WoW64. ([108701fa77f](https://github.com/nitroos-org/reactos/commit/108701fa77f706e6e7e2fbbe29e7f0562217dff2))
- boot bootdata: Register 15 more 32-bit COM DLLs and ship extra 32-bit DLLs, so 32-bit installers find their COM classes. ([a303fcc13ff](https://github.com/nitroos-org/reactos/commit/a303fcc13ffb7a740149fc3eefc5d718d61356e3))
- boot bootdata: ship pdh/msdmo/d3d9/ddraw/wmvcore in SysWOW64 ([fe7c237e652](https://github.com/nitroos-org/reactos/commit/fe7c237e652a930bf60da800006352f0156224e2))
- boot bootdata: Ship the 32-bit hid.dll in SysWOW64; 32-bit user32 tests now run under WoW64. ([6d283e4e3d1](https://github.com/nitroos-org/reactos/commit/6d283e4e3d1415ad38d9b60588dbfa27f9c9cf47))
- msafd: WoW64 ConnectEx (SUPER_CONNECT), AcceptEx short remote-address length → WSAEFAULT ([62acc7c3533](https://github.com/nitroos-org/reactos/commit/62acc7c3533f8faceb16a77540b3bf7abe3603ca))
- wow64: WoW64 IOCTL_AFD_RECV_DATAGRAM thunk ([464928835c2](https://github.com/nitroos-org/reactos/commit/464928835c2c972f6eb31518c95d203495eae3ef))
- wow64: SysWOW64 games/media DLLs + WoW64 ConsolepAttach ([ae59ef23baa](https://github.com/nitroos-org/reactos/commit/ae59ef23baa61dee4fdb5482592fed51412843ec))
- wow64: thunk ConsolepFree/GetConsoleWindow/GetProcessList ([e96d7451c96](https://github.com/nitroos-org/reactos/commit/e96d7451c96dc2b3d6281cd1b98d19add5018b69))
- wow64: WoW64 AcceptEx (SUPER_ACCEPT) and AFD connect-data thunks; native-size GET_CONTEXT ([653ee21742b](https://github.com/nitroos-org/reactos/commit/653ee21742b476d5b8b28e60e943e67270cbe8b6))
- wow64: translate LPC to the LSA authentication port so LogonUser works in 32-bit processes ([2663fac46e3](https://github.com/nitroos-org/reactos/commit/2663fac46e3e5f54f1d52e8ea8551b95aad0f07b))
- WoW64: support logon sessions, MSV1_0 password change and policy notifications for 32-bit processes. ([75b801cf5f8](https://github.com/nitroos-org/reactos/commit/75b801cf5f8decab71793a64461e65f42b4d02e2))
- wow64: WoW64 thunk for NtOpenKeyEx ([fb9f562f339](https://github.com/nitroos-org/reactos/commit/fb9f562f33946cf531c4071fc498f8c02b0368a8))
- WoW64: use 64-bit-wide NtUser callback tables in the 32-bit guest so 32-bit apps get window-procedure stubs like native. ([2b2deb75e7d](https://github.com/nitroos-org/reactos/commit/2b2deb75e7d01b2d952a6b78abef349941eec680))
- wow64: WoW64 P0 syscall thunks (ObjectTypeInformation, AFD send-datagram/get-context, FsFullSize) ([124cfbb6522](https://github.com/nitroos-org/reactos/commit/124cfbb6522bd7ebc0a5fed1f573b60bd80d933c))
- wow64: WoW64 P1 thunk batch (6 verified) ([d8d01fd13d3](https://github.com/nitroos-org/reactos/commit/d8d01fd13d3e7206973b674e31c5ccad97706266))
- wow64: WoW64 P2 trivia thunks (6 verified) ([f1c63dcd1ab](https://github.com/nitroos-org/reactos/commit/f1c63dcd1ab85c0f2983248454ab7b0935b3c49c))

</details>

<details><summary>Networking (40 fixes)</summary>

- AFD: AcceptEx error paths release their buffers and locked pages ([89f15489bc6](https://github.com/nitroos-org/reactos/commit/89f15489bc63af3524911f8c9759b105ead67c50))
- AFD: fix a memory-safety bug on a failed local-address query ([95e7dde5971](https://github.com/nitroos-org/reactos/commit/95e7dde59710cd08a9ce19cd20fdf6321dff08f7))
- AFD: validate output buffers and accept-state handling (hardening) ([df756200873](https://github.com/nitroos-org/reactos/commit/df75620087318454dfca08d7f5982878f84c26c3))
- afd: SO_ERROR after a refused connect as on Windows ([6c816f3d05f](https://github.com/nitroos-org/reactos/commit/6c816f3d05feaa7991224e67c70a5f2a55ff91b3))
- AFD: defensive request validation (hardening). ([e429a3f7d07](https://github.com/nitroos-org/reactos/commit/e429a3f7d07b896693c47c14a21451a046f32db3))
- afd: per-datagram completion + close-waits-all + stub-transport tests ([3ebf636efd3](https://github.com/nitroos-org/reactos/commit/3ebf636efd35e85fa6c0ce86b9f4406b41635f04))
- afd: don't bugcheck on unbound raw datagram sendto ([03c5a77e8c7](https://github.com/nitroos-org/reactos/commit/03c5a77e8c7d9bf4b31f6ea7f6a1c36f223db3d8))
- afd: no-ARP source query, Windows UDP connect/FIONREAD/event semantics ([f58d0b7c0d0](https://github.com/nitroos-org/reactos/commit/f58d0b7c0d0ffa9803fa2b511cb5f883fa8fd62f))
- afd: deliver zero-length UDP datagrams; getsockname after a UDP connect reports the route interface ([515a0816ed2](https://github.com/nitroos-org/reactos/commit/515a0816ed26265d1a18aded6b89cb1c613cab55))
- afd: keep the implicit local binding after a refused connect ([75216b1b406](https://github.com/nitroos-org/reactos/commit/75216b1b406f83a6ffa0cd75bce08e5ef916bc61))
- afd: WSARecvMsg IP_PKTINFO control data, MSG_CTRUNC, truncated datagram byte count ([1409d1a6e05](https://github.com/nitroos-org/reactos/commit/1409d1a6e054c493e6843e8cfa36cc8980dc70cc))
- iphlpapi: Vista interface and unicast address tables ([b72a6598503](https://github.com/nitroos-org/reactos/commit/b72a65985031a3fe7329f05bfb10d96ef1eec84d))
- iphlpapi: add NotifyIpInterfaceChange, NotifyUnicastIpAddressChange and NotifyRouteChange2 with route-change notification. ([9cd2cf8b520](https://github.com/nitroos-org/reactos/commit/9cd2cf8b520ec7f451f5a3a0742a260419c57faf))
- iphlpapi: module lifetime, route key, old tcpip backoff, per-file cap, NotifyRouteChange, WoW64 route ioctl ([e88b0528ac2](https://github.com/nitroos-org/reactos/commit/e88b0528ac229de975a481875a3a280ada8ddfde))
- msafd: SO_RCVBUF and SO_SNDBUF getsockopt return what was set, with no clamping of the caller's value. ([352f42cf08a](https://github.com/nitroos-org/reactos/commit/352f42cf08a824f5ed0e7e940bca3eacaaccb917))
- msafd: reject a negative option length in setsockopt with WSAEFAULT, and extend the receive-buffer tests. ([7ce78b00452](https://github.com/nitroos-org/reactos/commit/7ce78b00452cc7eab0c3c8c07b2f217c5c8bca2a))
- mswsock: NLA Winsock namespace provider + rnr NULL-class fix + WsNqLookupServiceBegin DNS-regression fix ([631ecbf27be](https://github.com/nitroos-org/reactos/commit/631ecbf27be3f71ff6f5d950d073d160b0cdc5ef))
- NDIS: stop raising IRQL over the whole miniport worker, which faulted on paged pool; a regression from an earlier change. ([025318b1f38](https://github.com/nitroos-org/reactos/commit/025318b1f38c4ad6febda5ee8e563ff3dfee0570))
- NDIS: abort pended requests on halt, remove and reset, and preallocate the per-adapter work item. ([2d79fdcb3e8](https://github.com/nitroos-org/reactos/commit/2d79fdcb3e8c052556ff37a450a31522bc73f727))
- ndis: tcpip bind-failure teardown, NDIS fails transfers at halt, ndisuio write length ([14f84490680](https://github.com/nitroos-org/reactos/commit/14f844906801341cabd72309cc46354902d191d0))
- NDIS: on adapter remove or stop, halt the miniport first and then unbind every protocol with reference-counted bindings. ([e2a3a4317ef](https://github.com/nitroos-org/reactos/commit/e2a3a4317ef5f51454a8a517bfd9e0879614d888))
- NDIS: allow only one request at a time per miniport, as on Windows, and make each tcpip NDIS call wait on its own event. ([73ef1bd6738](https://github.com/nitroos-org/reactos/commit/73ef1bd6738f4d508f4871026a96b890bd6e1c63))
- NDIS: fix a hang where a miniport reset waited behind the very request it aborted, and add a fake-miniport test driver. ([ae0e2cebf4b](https://github.com/nitroos-org/reactos/commit/ae0e2cebf4be0b2a67c09d64650c6e78b1d351bf))
- ndisuio: ndisuio send/read/transfer lifetime, NDIS transfer/reset references ([8eff58c78a3](https://github.com/nitroos-org/reactos/commit/8eff58c78a3e96a64c57e3bca3f9231a5a3ffc2b))
- schannel: report the Windows context attributes and answer the ALPN query with "none" (fixes git over HTTPS and Scoop bucket add). ([57ec3a5521c](https://github.com/nitroos-org/reactos/commit/57ec3a5521c0b2fb3067dc0c213092f5a8346a16))
- schannel: first SCH_CREDENTIALS credential support and test coverage (reworked in the updater-safe change listed with it). ([f13b71626a0](https://github.com/nitroos-org/reactos/commit/f13b71626a08725920ed751d98148af7411be484))
- schannel: accept SCH_CREDENTIALS credentials by translating them to the existing credential path, so .NET 8 and PowerShell 7 HTTPS work; keeps the proven TLS data path (fixes intermittent Google updater crashes during Chrome MSI installs). ([9535680b025](https://github.com/nitroos-org/reactos/commit/9535680b025308171246ee07885231249f1a6f18))
- schannel: report untrusted issuer ahead of expiry ([2ee6ddbec6e](https://github.com/nitroos-org/reactos/commit/2ee6ddbec6e672f44703dda254acc79008af7cf8))
- tcpip: serialize looped UDP delivery (fixes datagram drop/recvfrom-hang race) ([b4ab1129898](https://github.com/nitroos-org/reactos/commit/b4ab1129898bbfcdd7082fe4b2fa92a64c6b848f))
- tcpip: NotifyAddrChange and CancelIPChangeNotify now signal on IPv4 address, interface and link changes. ([25ad69e6a49](https://github.com/nitroos-org/reactos/commit/25ad69e6a49c21b5630c1c74c8ba2f28e8e22dff))
- tcpip: validate untrusted network-supplied lengths in the ARP/neighbor code (memory-safety hardening) ([81cc1d95900](https://github.com/nitroos-org/reactos/commit/81cc1d95900eaf73453215336daeb0dc17419e62))
- tcpip: reference-count IP interfaces and neighbour entries, and fix a bucket-lock deadlock and a route-table overrun. ([0ff193b5767](https://github.com/nitroos-org/reactos/commit/0ff193b5767089b0d2e808162128957880944c9d))
- winhttp: surface specific TLS cert error ([1dadf4d4de8](https://github.com/nitroos-org/reactos/commit/1dadf4d4de872d06e4f81a48b1d89e6e0654c720))
- Winsock: WSAEnumNameSpaceProviders returns a valid list again (crashed with the new NLA provider); DNS provider registered under the DNS namespace. ([dee4bd5ca39](https://github.com/nitroos-org/reactos/commit/dee4bd5ca39357706726d1ae67ac6127d5c79c49))
- ws2_32: GetAddrInfoExW overlapped lookups complete on a worker thread as on Windows, fixing .NET and PowerShell 7 hostname hangs. ([1c797601e00](https://github.com/nitroos-org/reactos/commit/1c797601e00eaefd9b4853ccd24a33b122e47beb))
- ws2_32: GetAddrInfoW now fails with WSANOTINITIALISED outside WSAStartup/WSACleanup instead of returning success. ([fbc328ab04e](https://github.com/nitroos-org/reactos/commit/fbc328ab04e6a7c685aecab86f6ec8b5ca6e77e7))
- ws2_32: getservbyname and getservbyport now return the service alias list. ([c5df391d5cf](https://github.com/nitroos-org/reactos/commit/c5df391d5cf4b7071e5b663f51df788e75960b03))
- ws2_32: getaddrinfo accepts trailing whitespace after an IPv4 address, as inet_aton does. ([f40caa6f89b](https://github.com/nitroos-org/reactos/commit/f40caa6f89b56eb1510ba043c22670aaa1e4086b))
- ws2_32: select() accepts every timeout value Windows accepts; fixes git over HTTPS and Scoop bucket add. ([9e19e4735cc](https://github.com/nitroos-org/reactos/commit/9e19e4735ccd2c1334668aa211ec19f41b4a4b83))
- ws2_32: bind returns WSAEFAULT for a bad address and picks an ephemeral port for a TCP wildcard bind. ([3f272041d41](https://github.com/nitroos-org/reactos/commit/3f272041d41cd4edfc5f7df64c2d0d60b4ad6c45))

</details>

<details><summary>Storage, USB, PnP and drivers (1 fix)</summary>

- mouclass/kbdclass: serialize pending-IRP decisions under one spinlock, fixing input stalls under load. ([9fa5d159064](https://github.com/nitroos-org/reactos/commit/9fa5d159064639f58cf4dea434a4b85451ee4c07))

</details>

<details><summary>Shell, explorer and common controls (5 fixes)</summary>

- shell: fix an AssocQueryString/AssocQueryKey crash at the NT6 identity and add NT6 parity. ([29df196f682](https://github.com/nitroos-org/reactos/commit/29df196f6821871e11dfe2702c6f15435866b364))
- shell: don't ASSERT/double-append on dir of a UNC share root ([aafc5fd1b16](https://github.com/nitroos-org/reactos/commit/aafc5fd1b1652b2a6a456f70820ad7aab4f46919))
- shell32: review fixes for shell-link property-store handling, including safer block replacement and parsing. ([79da1aec295](https://github.com/nitroos-org/reactos/commit/79da1aec295743847fedcedec4ef12fa5e1e4c93))
- shell32: add the jump-list COM classes, fixing the 'Failed to begin Jump List transaction' error in Electron apps. ([d89a1851529](https://github.com/nitroos-org/reactos/commit/d89a1851529ff913cf2410a414e12941bed4ce3a))
- shell32: shell links now persist property-store values in the .lnk file. ([0d095a75f83](https://github.com/nitroos-org/reactos/commit/0d095a75f8378d0c3972c51112ba39b3df29d46a))

</details>

<details><summary>Setup and boot (1 fix)</summary>

- rapps: Feature manager: live software-tier install round trip and silent mode for the rapps installer. ([424831fff92](https://github.com/nitroos-org/reactos/commit/424831fff9275b6f449adaaefef28e05fa14f9a0))

</details>

<details><summary>Printing (1 fix)</summary>

- win32ss printing: fix a memory-safety bug when deleting a printer port with jobs in flight ([5ad363d33be](https://github.com/nitroos-org/reactos/commit/5ad363d33beeabcfe87381536747b89ef579a6b7))

</details>

<details><summary>Services, security and profiles (5 fixes)</summary>

- netapi32: NetLocalGroupGetMembers heap corruption, empty-alias and NetUserDel SAM fixes ([3badb842dac](https://github.com/nitroos-org/reactos/commit/3badb842dac68719ce503c65e7501ca94b4a52c8))
- samsrv: NetUserDel does not fail for a domain without alias members ([1fa71fd6ab3](https://github.com/nitroos-org/reactos/commit/1fa71fd6ab36034c04297a34a868461afce6f960))
- Services: use a 32-bit cookie as the service status handle so WoW64 services work and unknown handles fail cleanly. ([7eace4e9e1a](https://github.com/nitroos-org/reactos/commit/7eace4e9e1a030e7185f246c8e1e8ed1c3d3e09e))
- services: Service failure actions: review fixes for services deleted or shut down while an action is pending. ([e41e57fa739](https://github.com/nitroos-org/reactos/commit/e41e57fa73967d73337b690d5028be982095768f))
- services: The service control manager now runs failure actions (restart, run command, reboot) with delays and a reset period. ([ee353e7b5e9](https://github.com/nitroos-org/reactos/commit/ee353e7b5e92954063edd69acc96a0702949d090))

</details>

<details><summary>User-mode system DLLs (11 fixes)</summary>

- kernel32: WriteProcessMemory keeps execute permission on executable ranges, fixing a self-patching regression. ([678c39705c1](https://github.com/nitroos-org/reactos/commit/678c39705c14d959d09f4408bc2cc0b3c3a26043))
- kernel32: implement Boundary Descriptor / Private Namespace family ([2dfd2e6d1d7](https://github.com/nitroos-org/reactos/commit/2dfd2e6d1d7823ed0d111cb4b726b622a6a08bda))
- kernel32: LCMapStringEx sort keys and LCMAP_HASH now work, fixing .NET culture-aware hashing (PowerShell 7 jobs, CLIXML). ([2dc1b9dc1e0](https://github.com/nitroos-org/reactos/commit/2dc1b9dc1e07d3e4056e7495cceb96094ff702df))
- kernel32: Correct WriteProcessMemory comments and test expectations and add a check that writes never gain execute permission. ([1840711a0b2](https://github.com/nitroos-org/reactos/commit/1840711a0b2ee83af1a5c1f3d83f52b2c9287768))
- kernel32/ntdll: implement the XState and CONTEXT_EX context APIs, which PowerShell 7 and CoreCLR need. ([3ede716e4f8](https://github.com/nitroos-org/reactos/commit/3ede716e4f8cbb64d28d1adbf04c8b4ee585d20b))
- msvcrt: export wcstok_s at 0x502, fixing the native NT 5.2 full-tree link. ([e2940f0f076](https://github.com/nitroos-org/reactos/commit/e2940f0f0769fed73d517f8485ed5bd367c4b318))
- ntdll: Version-gated Vista to Win10 API parity fixes that keep NT 5.2 behaviour safe. ([5e7c04653c5](https://github.com/nitroos-org/reactos/commit/5e7c04653c5d4e3b0657d990d5e105c517626c40))
- rtl: activation-context manifest probe reads only the headers and resources it needs (hardening) ([b6671bf92c5](https://github.com/nitroos-org/reactos/commit/b6671bf92c512e4948e39ba8b73355afd37c8bfb))
- rtl: activation-context manifest probe: bound the PE resource-directory walk (hardening) ([df15091dee3](https://github.com/nitroos-org/reactos/commit/df15091dee32ed9313e3188c8a69131e453658a5))
- rtl: the thread pool injects worker threads gradually like Windows (one per CPU, then about one per 160 ms); long-running work gets a thread at once. ([712a32490b8](https://github.com/nitroos-org/reactos/commit/712a32490b8be58c0cb9af2132fb8e1a675c89cd))
- ucrtbase: Implement the ucrtbase lrint family, which crashed codec and audio callers, and make log2 exact for powers of two. ([eecba8ed7d1](https://github.com/nitroos-org/reactos/commit/eecba8ed7d1b64fe931fc32cd17943bad9802bc6))

</details>

<details><summary>Browser, COM and Wine imports (9 fixes)</summary>

- 3rdparty: enable ChaCha20-Poly1305 client cipher suites ([057f375a4df](https://github.com/nitroos-org/reactos/commit/057f375a4dfecbcce502abe19ede3e0cea73fdab))
- cmake: ndfapi.dll Network Diagnostic Framework (17 impl + 7 stubs) ([a6ab370e260](https://github.com/nitroos-org/reactos/commit/a6ab370e26010d30fd4786d415785f34829a9ec1))
- cmake: wer.dll Windows Error Reporting (9 impl + stubs) ([7ca94078a52](https://github.com/nitroos-org/reactos/commit/7ca94078a52089e2cdb8ff09b36989306583b3a5))
- cmake: wevtapi.dll Windows Event Log API (36 impl, Wine 11.19) ([9cd46ace112](https://github.com/nitroos-org/reactos/commit/9cd46ace1124aa4e6c3b995df256bcb08dc634f1))
- mscoree: Groundwork for hosting the Microsoft CLR; .NET Framework apps still do not run (see Limitations). ([49fed9726af](https://github.com/nitroos-org/reactos/commit/49fed9726affd9c69f487d75e8dc4e4d8130a912))
- prntvpt: Add prntvpt.dll (from Wine) and a windows.data.pdf.dll stub, so WordCraft starts. ([35ab2e47b03](https://github.com/nitroos-org/reactos/commit/35ab2e47b0302cb0e2d42228ecb28c59d5f2e11d))
- taskschd: Task Scheduler 2.0: taskschd.dll from Wine, the schedsvc service and triggers; the Chrome auto-update task runs. ([a1802f9ac3d](https://github.com/nitroos-org/reactos/commit/a1802f9ac3def66d155f5da7f9e511a46d231e34))
- tbs: tbs.dll TPM Base Services (fail-like-Windows without TPM) ([7ae40fdcaf7](https://github.com/nitroos-org/reactos/commit/7ae40fdcaf7fa701c9ffc8591afa21ee6180e2ee))
- urlmon: end a protocol on its apartment thread when a handoff cannot be posted. ([8fe5f7f3409](https://github.com/nitroos-org/reactos/commit/8fe5f7f3409502c30181c4611388799777bbc889))

</details>

<details><summary>Applications and tools (5 fixes)</summary>

- appcompat: Application shims may now set only an allowlist of process-parameter flags. ([27d7948466c](https://github.com/nitroos-org/reactos/commit/27d7948466cd02a3ac8ea1de5666c18449b6d00a))
- cmdutils: robocopy.exe implemented; unblocks the Scoop installer. ([194c804ff30](https://github.com/nitroos-org/reactos/commit/194c804ff30be96aac04878502bd92189659654a))
- featmgr: Feature manager: center the Control Panel dialog and fetch the online catalog. ([0cb026648bc](https://github.com/nitroos-org/reactos/commit/0cb026648bc108cbe166abd5a4af40789cc466e6))
- featmgr: Feature manager: add a 'powershell-7' entry that installs the official 7.4.22 MSI (SHA-256 verified); explorer re-reads the environment on change. ([d191031a1e1](https://github.com/nitroos-org/reactos/commit/d191031a1e180e064bc3bfcfd5a03d7da18d3f8a))
- optionalfeatures: Optional Features: license-clean in-tree icon for the Control Panel applet. ([0facf99debe](https://github.com/nitroos-org/reactos/commit/0facf99debeb67b95fa8e01947dd4d20e29785a6))

</details>

<details><summary>Build, toolchain and API surface (1 fix)</summary>

- tools: host-native ex/handle.c stress harness ([9eb6838f926](https://github.com/nitroos-org/reactos/commit/9eb6838f926eed3b10e8e6d718982f1f5eb24bf3))

</details>

<details><summary>Tests only (18 fixes)</summary>

- tests: drop a duplicate AFD packet typedef that broke the ENABLE_ROSTESTS build. ([167c280c8a9](https://github.com/nitroos-org/reactos/commit/167c280c8a9754c344148a6165e3f5cac675f891))
- tests: cover the handoff of a worker's reference when posting fails or the apartment ends. ([2a7ca0e24e9](https://github.com/nitroos-org/reactos/commit/2a7ca0e24e9de614538a8be18ec4309630e75920))
- tests: run the AFD accept-state, connect-options and output-probe tests under WoW64 too. ([4c1ae8e9254](https://github.com/nitroos-org/reactos/commit/4c1ae8e925476e04cd07949e279c85c5573ad4d6))
- tests: stronger regression tests for the window-procedure hardening. ([73ec4062994](https://github.com/nitroos-org/reactos/commit/73ec40629946faa7ed7823fdfe82e9038334e74d))
- tests: add a ws2_32 test that opens many sockets. ([bbcfd8fbd50](https://github.com/nitroos-org/reactos/commit/bbcfd8fbd50f33849c6e23fdd1938b577ada5f29))
- tests: positive and boundary cases for mapping section views. ([ef65da08dbd](https://github.com/nitroos-org/reactos/commit/ef65da08dbdca3ce65036ca8d364ae6f2a11cfd2))
- tests: resolve Vista-and-later APIs at run time so the tests link at 0x502. ([530b21f00ee](https://github.com/nitroos-org/reactos/commit/530b21f00ee3d649ac547ae52c384fefd9f71303))
- tests: map ObTypes expectations to Windows 10 NTDDI levels. ([69d5a21f6f2](https://github.com/nitroos-org/reactos/commit/69d5a21f6f20cb02b598c43c049a5f81112424f2))
- tests: Register upstream's NtUser PFN test with ReactOS-aware expectations. ([4c30242f216](https://github.com/nitroos-org/reactos/commit/4c30242f216deb7ef35ca7006087f94e6e648e88))
- tests: adapt the NtKeyedEvent and ZeroBits tests to Windows 10 behaviour. ([c63526185d0](https://github.com/nitroos-org/reactos/commit/c63526185d0833ceb37337a332a02c41a77c1fcc))
- tests: regression tests for DuplicateHandle on read-only sections. ([267cc9e1666](https://github.com/nitroos-org/reactos/commit/267cc9e166660233a7e70bbfb5e39a4e51707a72))
- tests: Add a test showing that the mscoreei.dll load failure is its own precondition, not a loader bug. ([134bef162d8](https://github.com/nitroos-org/reactos/commit/134bef162d8f7a6b3bbed478e9d6801a3f7f50a3))
- tests: Test fix: the ws2_32 close test binds to an ephemeral port instead of a fixed one. ([0ee2a246784](https://github.com/nitroos-org/reactos/commit/0ee2a246784c107c07ed9e0c2cf4802ec64ec6dc))
- tests: guard a NULL gethostbyname result in the ws2_32 test helper. ([8ac4e723ba6](https://github.com/nitroos-org/reactos/commit/8ac4e723ba6ad2f199baba248d9e83632ca8a470))
- tests: add fmod/fmodf tests for msvcrt and ucrtbase. ([8f2782b4521](https://github.com/nitroos-org/reactos/commit/8f2782b4521136087fd31e6247c2a461ce05729a))
- tests: add a TokenLogonSid test under WoW64. ([44a2b7636a9](https://github.com/nitroos-org/reactos/commit/44a2b7636a9d7281e994e2b37b954d2772d57c4c))
- tests: add a kernel test for the ExInterlocked/IPI deadlock that is safe in the default suite. ([1b42b09e7fd](https://github.com/nitroos-org/reactos/commit/1b42b09e7fd434e34a88ae23dc2f156500ffa9f9))
- tests: add kernel and Winsock tests for dynamic worker-thread growth and pending-I/O teardown. ([982b59e68bb](https://github.com/nitroos-org/reactos/commit/982b59e68bbb7d0e480fbda0e3807288c8507d03))

</details>

### Limitations
- **Chrome runs with software rendering only.** There is no DirectComposition (`dcomp.dll`) and no hardware GPU acceleration; Claude Desktop uses its per-app software renderer configuration. Direct3D/DXGI windowed present over the display driver is not supported.
- **32-bit MFC applications fail at their first window** (WoW64 cross-process global-hook gap, `WIN32K-global-hooks-cross-process`); other 32-bit apps are unaffected.
- **Memory commit is capped at roughly physical RAM** until private memory becomes pageable (working-set trimming and a modified-page writer are not implemented). Size VMs accordingly; **Firefox 128 x64 thrashes when it reaches the cap.**
- **.NET Framework 4.x managed applications still do not run** (`mscoree` is the Wine/Mono implementation; no Microsoft CLR host). PowerShell 7 (CoreCLR) is a separate path and works.
- Extended CPU state (YMM/AVX-512) does not round-trip through `GetThreadContext`/`SetThreadContext` (the user-mode XState API reports it supported; the kernel does not save it).
- **Known open security hardening items, local-privilege class.** A set of local hardening items remains open (including enforcement of some access-control checks in local services and the registry, and the remaining medium/low findings from our internal audits). They require local access to the machine. This is alpha-quality software: do not expose it to untrusted local users or networks.
- Reverse DNS lookups via `getnameinfo` / `gethostbyaddr` fail (WSAHOST_NOT_FOUND) even though `nslookup` resolves them; pre-existing.
- **An 8 GB VM uses only about 3 GB of RAM** (memory above 4 GB is not used yet; fix in progress).
- **DeckCraft does not start** (it needs DirectX 12 only).
- **WMI / CIM cmdlets** in PowerShell 7 are not included yet.
- `attrib /L` is missing (Scoop's installers print "Invalid parameter format - /L" but complete).
- Double-clicking the stock PowerShell MSI shows a WMF message; install PowerShell 7 with `featmgr /install powershell-7` instead.
- GetSystemMetrics reports no mouse (`SM_MOUSEPRESENT` = 0) even though the mouse works; inherited from upstream ReactOS's input-device rework. Apps that check for a mouse may behave as if none is attached.
- WoW64 is amd64 only. Firefox ESR 128 and Firefox 158 are the tested versions (newer versions are expected to work but are untested); the 64-bit sandbox process starts but its restrictions are not fully enforced.
- Carried forward from earlier releases: some 32-bit applications leave redraw artifacts when dragged; everything in upstream ReactOS's own list of known issues also applies.
- **Resolved since 2026.10-4:** the `DuplicateHandle` access-widening regression is corrected; Firefox 32-bit now renders at the default sandbox; end-to-end HTTP browsing works in Chrome.

### Changes since reactos-nitro 2026.10-4
New upstream base `e46f4bb1a1e` (weekly sync #4). This release makes Windows 10 the default identity and adds Chrome, Claude Desktop, Firefox 158/32-bit-at-default-sandbox, a usable PowerShell 7 and Scoop support, Task Scheduler 2.0, the feature manager, Mesa/lavapipe graphics, the guest-clock and memory-commit fixes, a large WoW64/networking/kernel fix batch (242 changes listed above) and a security hardening batch. Shipped as the replayed 52-topic patch series on the new base.


## reactos-nitro 2026.10-4

**Tagged `nitro-2026.10-4`** (2026-10-24) at fork `main` [`be9d31b1b9a`](https://github.com/nitroos-org/reactos/commit/be9d31b1b9a4dcdc3123026d15a3bab5c44c35ea). The app-compatibility release: **Claude Desktop (Electron/Chromium) now runs, renders, and connects to the live service on reactos-nitro**, on the same upstream base as 2026.10-2/-3, plus Firefox 32+64 side-by-side rendering and a kernel security/stability batch.

| Item | Value |
|---|---|
| Release tag | `nitro-2026.10-4` → [`be9d31b1b9a`](https://github.com/nitroos-org/reactos/commit/be9d31b1b9a4dcdc3123026d15a3bab5c44c35ea) |
| ReactOS base | upstream [`f53b1d15601`](https://github.com/reactos/reactos/commit/f53b1d1560120c529763b18963fdee68f39118af) — unchanged from 2026.10-2/-3 |
| Patch set | Cumulative over-base patch `nitro-2026.10-4-over-f53b1d15601.cumulative.patch` (verified: apply on `f53b1d15601` → tree == REL_SHA, diff empty). 3232 commits (2440 non-merge) over upstream, same base as 2026.10-2/-3. Direct-main cut (no replayed topic stack this release). |
| Artifacts | amd64-with-WoW64 ISO, i386 ISO, source tarball, patch (sha256 below) |

| Artifact | Size | sha256 |
|---|---|---|
| reactos-nitro-2026.10-4-amd64-wow64-Release.iso | 550502400 | `dd47c83e75cedaf198c08a55bbea4fcaf2b4b45a5ce41bb1d25e88fa40363d36` |
| reactos-nitro-2026.10-4-i386-Release.iso | 500170752 | `3cf90c74d81509a715bec45aec7d6333218b536ed1955c781c3b62f3886b6aa7` |
| reactos-nitro-2026.10-4-src.tar.gz | 113864152 | `72d72c0a369966113224773e65df1cf58401e4534a082ffd1be64aa8509cc928` |
| nitro-2026.10-4-over-f53b1d15601.cumulative.patch | 44891396 | `14c141906570d7cb54731b4fb5e72e422cabc56ddddc0414c28fec045eb386e4` |

### Highlights
**Claude Desktop runs and renders on reactos-nitro** — the modern Electron/Chromium app class is unlocked. Stock Electron 37.6.0 / Chrome 138 installs (via the extracted app payload), launches, creates its GUI window, and paints its UI through Chromium's SkiaRenderer/GPU compositor over ANGLE→SwiftShader (pure-CPU). Reaching a painted window required a chain of Windows-correctness fixes across win32k, user32, ntdll and the Object Manager — each an app-agnostic multiplier that benefits the whole Electron/Chromium (and GL/D3D) class, not just Claude. Claude Desktop also **connects to the live claude.ai** — the login UI loads, Cloudflare challenge passes, HTTP/2 — over the guest's standard networking (Chromium's own TLS stack; no new OS crypto code required). Also in this release: **Firefox 158** UI + content sandbox (level 1), and **Firefox-ESR 32-bit and 64-bit rendering content side-by-side on a single amd64+WoW64 system** (T104: the WoW64 thread-handle regression that blanked the 32-bit broker is fixed), and a kernel **security batch** (a tcpip type-confusion/UAF regression fix and FsRtl change-notify use-after-free fixes). Gate: 4-config builds rc=0, both ISOs boot to desktop, reproducibility verified (rebased stack tree == main), T0 + targeted suites green. **Correction (post-release):** the full regression suite completed after this tag and found that one change in this release — the `ObDuplicateObject` handle-access change (T100) — is Windows-incorrect; see Limitations.

### What reactos-nitro adds on top of ReactOS
- **Modern Electron/Chromium rendering (Claude Desktop):** `GetDC(NULL)` screen DC now associates with the desktop window (ANGLE EGL display bring-up); Win8 pointer-input `user32` exports (Chromium delay-load); app-manifest OS-version honored in `GetVersion`/`RtlGetVersion` per-process (Crashpad handler spawn); `advapi32` `BuildSecurityDescriptorW/A` implemented + unprivileged mandatory-label set (Crashpad named-pipe SD); `ntdll` `RtlGetUnloadEventTraceEx` implemented + `RTL_UNLOAD_EVENT_TRACE` pointer-width (crashpad dump capture). (An `ObDuplicateObject` change for Chromium read-only shared-memory also shipped but is superseded — see Limitations.) Per-app config (SwiftShader ICD + `no3d`) ships with the Claude app. See the render recipe in the harness docs.
- **32-bit COM / MSXML (WoW64):** 32-bit `msxml3` shipped + registered in SysWOW64 so x86 `CoCreateInstance(DOMDocument30)` works on amd64 — unblocks a broad class of 32-bit installers/apps.
- **Firefox 158**: Win10 per-app identity + Win10 API surface + content-sandbox level 1 (64-bit UI renders). WoW64 Firefox-ESR 32-bit + 64-bit both launch with browser chrome + navigation; side-by-side *content* rendering is limited (broker/sandbox) and tracked under T86.
- **Security hardening:** tcpip `SET_INFORMATION_EX` entity type-check (fixes a non-admin kernel type-confusion/UAF regression); FsRtl change-notify cancel/cleanup races, lost-wakeup and a reuse-UAF closed; GDI `CreateDIBSection` size-math overflow guards. (The `ObDuplicateObject` handle-access change originally listed here is **withdrawn as a security fix** — it was over-broad and Windows-incorrect; see Limitations.)
- **Stability:** process-quota block-switch race (prevents a QUOTA_UNDERFLOW bugcheck); ramdisk AddDevice flag-mask fix; OOM test-hook stripped from Release kernels.

## Bug fixes

<details><summary>App compatibility — Electron/Chromium (Claude Desktop) (8 fixes)</summary>

- WIN32SS/NTUSER: `IntWindowFromDC` returns the desktop window for the windowless screen DC so ANGLE's `eglGetPlatformDisplay(GetDC(NULL))` succeeds — T96 ([437c7de7424](https://github.com/nitroos-org/reactos/commit/437c7de7424))
- WIN32SS/USER32: export Win8 pointer-input APIs (`GetPointerType`/`GetPointerDevice`/`GetPointerPenInfo`/`RegisterPointerDeviceNotifications`) so Chromium delay-load resolves — T100 ([44ce99c2dfd](https://github.com/nitroos-org/reactos/commit/44ce99c2dfd))
- NDK/RTL: `RTL_UNLOAD_EVENT_TRACE.SizeOfImage` pointer-wide (x64) so crashpad records the unloaded-module list — T100 ([64cea71224c](https://github.com/nitroos-org/reactos/commit/64cea71224c))
- NTOS:OB: `ObDuplicateObject` change for Chromium read-only shared-memory — ⚠️ **SUPERSEDED/INCORRECT:** this blanket-blocked user-mode handle-access widening, which is Windows-incorrect (Windows re-evaluates the object's security descriptor and grants when the DACL allows). It over-broadly broke legitimate `DuplicateHandle` elevation and is being corrected next release — T100 ([77bb7949b8d](https://github.com/nitroos-org/reactos/commit/77bb7949b8d)). See Limitations.
- KERNEL32/NTDLL: honor the app-manifest compatibility version in `GetVersion`/`RtlGetVersion` (per-process) so Crashpad's handler spawn succeeds — T94 ([58a90fb603b](https://github.com/nitroos-org/reactos/commit/58a90fb603b))
- ADVAPI32/NTOS:OB: implement `BuildSecurityDescriptorW/A` + allow an unprivileged process to set its own mandatory label, fixing Crashpad's named-pipe security descriptor — T90 ([c092b8f5038](https://github.com/nitroos-org/reactos/commit/c092b8f5038))
- NTDLL/RTL: implement `RtlGetUnloadEventTraceEx` (was a stub returning success without writing its out-pointers → AV in Vista+ callers) — T97 ([d315f811604](https://github.com/nitroos-org/reactos/commit/d315f811604))
- SYSWOW64/SYSSETUP: ship + register 32-bit `msxml3`/`libxslt` so x86 `CoCreateInstance(DOMDocument30)` works on amd64 — T93 ([0c5d91b1606](https://github.com/nitroos-org/reactos/commit/0c5d91b1606))
</details>

<details><summary>Firefox 158 / WoW64 (4 fixes)</summary>

- WOW64: `NtCreateThread` thunk clamps the returned guest handle's access to what the thread object grants (via `NtQueryObject`) instead of requesting `THREAD_ALL_ACCESS` verbatim — fixes WoW64 `CreateThread` failing (the thread generic mapping grants `0x1f0fff`, not `0x1fffff`, so the over-request was rejected), which had blanked 32-bit Firefox's Chromium broker init under WoW64. Enables FF32+FF64 side-by-side content render — T104 ([cd739717eaf](https://github.com/nitroos-org/reactos/commit/cd739717eaf))
- Shim-engine PAGE_READWRITE fix + Firefox 116+ per-app Win10 identity + `QueryUnbiasedInterruptTimePrecise`/`GetSystemCpuSetInformation`/`CM_Register_Notification` ([03f77e1dc84](https://github.com/nitroos-org/reactos/commit/03f77e1dc84))
- KERNEL32: validate+accept `CHILD_PROCESS_POLICY`/`COMPONENT_FILTER`/`JOB_LIST` proc-thread attributes ([41be7cf42c0](https://github.com/nitroos-org/reactos/commit/41be7cf42c0))
- NTOS:MM: `NtReadVirtualMemory` Windows partial-copy semantics (`STATUS_PARTIAL_COPY`) ([9b08b829477](https://github.com/nitroos-org/reactos/commit/9b08b829477))
</details>

<details><summary>Filesystems — FsRtl / ntfsng change-notify (3 fixes)</summary>

- ntfsng: change-notify torn-read race across rename (over-read into watcher output) + drop 64KB/watch alloc — T87 ([efdb9109733](https://github.com/nitroos-org/reactos/commit/efdb9109733))
- NTOS:FSRTL: change-notify cancel/cleanup unlink race + completed-IRP requeue + report-size cluster (bugcheck 0x139 negative control) — T88 ([0715ca13dce](https://github.com/nitroos-org/reactos/commit/0715ca13dce))
- NTOS:FSRTL: change-notify lost-wakeup + reuse-edge use-after-free — T98 ([424e479fc37](https://github.com/nitroos-org/reactos/commit/424e479fc37))
</details>

<details><summary>Networking (1 fix, security)</summary>

- tcpip: `SET/QUERY_INFORMATION_EX` ADDRESS_OBJECT/CONNECTION entity type-check + live reference — fixes a non-admin kernel type-confusion/pool-corruption/UAF regression — T89 ([34fb956b444](https://github.com/nitroos-org/reactos/commit/34fb956b444))
</details>

<details><summary>win32k GDI (1 fix)</summary>

- `DIB_CreateDIBSection` size-math made overflow-safe (64-bit pitch, checked alloc, reject bad dimensions) — T62 ([7016ac5f357](https://github.com/nitroos-org/reactos/commit/7016ac5f357))
</details>

<details><summary>Kernel — Ps / Mm (2 fixes)</summary>

- NTOS:PS: quota block-switch race vs lockless charges (epoch-quiesce; prevents a QUOTA_UNDERFLOW 0x21 bugcheck, Release-affecting) — T57 ([f83de5692f5](https://github.com/nitroos-org/reactos/commit/f83de5692f5))
- NTOS:MM: compile the `MmDbgTestOomRetryBound` test hook out of Release kernels (release-prep) — T61 ([d1b1077cba6](https://github.com/nitroos-org/reactos/commit/d1b1077cba6))
</details>

<details><summary>Drivers & toolchain (2 fixes)</summary>

- ramdisk: `RamdiskAddDevice` clears only `DO_DEVICE_INITIALIZING` (was wiping `DO_DIRECT_IO`/`DO_POWER_PAGABLE`) — T75 ([0e439e7d3e7](https://github.com/nitroos-org/reactos/commit/0e439e7d3e7))
- ADVAPI32: link at DLL 0x502 (guard the `RtlAddMandatoryAce` Vista+ dependency) — T95 ([51a81438119](https://github.com/nitroos-org/reactos/commit/51a81438119))
</details>

### Limitations
- **Claude render requires the per-app config** (SwiftShader ICD + `no3d` + ANGLE launch flags) that ships with the Claude app packaging; it is pure-CPU rendering (no GPU acceleration).
- **Firefox 32+64 side-by-side content render is verified via `file://`**, exercising the full broker → sandbox-target → content-process → renderer → JS pipeline on both browsers simultaneously. End-to-end `http` page fetch inside the test VMs is a current harness/network-profile limitation (not a product gap in the WoW64 broker fix) — live web browsing depends on the same guest networking that the `-5` runtime/crypto work addresses.
- .NET Framework 4.5 enablement is in progress (MSXML/32-bit-COM installer wall cleared this release; the CLR itself is not yet supported — tracked for `-5`).
- ⚠️ **Known regression — `ObDuplicateObject` (T100):** this release's `ObDuplicateObject` change blanket-blocks user-mode `DuplicateHandle` requests that widen access beyond the source handle. That is **Windows-incorrect** — Windows re-evaluates the object's security descriptor and grants the wider access when the DACL permits. As a result, legitimate DACL-granted handle-access elevation is broken in `-4` (surfaced by `NtOpenProcess`/token/handle-duplication tests, including Windows-validated ones). Headline apps (Claude Desktop, Firefox) are unaffected. The correct fix — re-evaluating the security descriptor Windows-style, while still preventing write-elevation of Chromium's restrictive-SD read-only sections — lands in `-5`. (Originally mis-described here as an elevation-of-privilege *fix*; it is not.)

### Changes since reactos-nitro 2026.10-3
Same upstream base. This release adds the Claude Desktop / Electron render chain (win32k/user32/ntdll/Ob), 32-bit MSXML COM, Firefox 158 + sandbox-L1, and a kernel security/stability batch (tcpip type-confusion regression, FsRtl notify UAFs, quota-switch bugcheck, GDI size-math, ramdisk flags; note the `ObDuplicateObject` change is superseded — see Limitations). Shipped as a cumulative over-base patch (3225 commits / 2436 non-merge over f53b1d15601) rather than a replayed topic stack (this window landed direct-to-main).

## reactos-nitro 2026.10-3

**Tagged `nitro-2026.10-3`** (2026-10-22) at fork `main` [`92f4586f849`](https://github.com/nitroos-org/reactos/commit/92f4586f849d3e5bdc2538ff1e9374136b865721). A large security, stability and SMP point release on the same upstream base as 2026.10-2.

| Item | Value |
|---|---|
| Release tag | `nitro-2026.10-3` → [`92f4586f849`](https://github.com/nitroos-org/reactos/commit/92f4586f849d3e5bdc2538ff1e9374136b865721) |
| ReactOS base | upstream [`f53b1d15601`](https://github.com/reactos/reactos/commit/f53b1d1560120c529763b18963fdee68f39118af) (sync #3, 2026-10-20) — unchanged from 2026.10-2 |
| Patch series | 47 topic patches on `f53b1d15601` (`main-rebased-20261022` → `9eeefc87b93`); applied tree `63dc72d7479` (== main) |
| Artifacts | amd64-with-WoW64 ISO, i386 ISO, source tarball, patch series (sha256 below) |

| Artifact | sha256 |
|---|---|
| reactos-nitro-2026.10-3-amd64-wow64.iso | `98f19a091d271e7cd5689c162dd120fe94b3d7fffd6b5583659f8cf6839a572c` |
| reactos-nitro-2026.10-3-i386.iso | `afe05a0ed86c0fb965c165ef88a1a2f0a4e356c16a4c8a1ad4713a664d3fe65f` |
| reactos-nitro-2026.10-3-src.tar.xz | `5b82c65a880ac4744ed20ddba56346c35cee41055dd84c93f2ba1da4b0d7d29b` |
| reactos-nitro-2026.10-3-patch-series.tar.xz | `e6e46c5c02aeff4122bd0d4c85ab180bc06b34d74621d544033d77eb610f69f1` |

### Highlights
A broad security + stability + SMP release on the same base as 2026.10-2. Headline areas: a deep kernel/win32k security sweep (verified attack-surface seams in win32k GDI/USER capture, kernel streaming, AFD, mountmgr, keyboard/mouse class, dbgk/kd64, WoW64), non-admin TCP/UDP sockets, and two SMP throughput levers (per-CPU object-handle cache and per-CPU nonpaged pool). Gate: 4-config builds rc=0, both ISOs boot to desktop, reproducibility verified (rebased stack tree == main), and the T1 regression suite showed only known-flaky entries (0 attributable to this release).

### What reactos-nitro adds on top of ReactOS
- **Security sweep (Astra#2/#3 verified seams):** win32k GDI/USER user-pointer capture and size-math (`ExtEscape`, `CreateDIBSection` ppvBits, clipboard CF_DIB, menu/atom/icon/hook handlers); kernel-streaming user-buffer capture; AFD use-after-free / enum-events overflow / METHOD_NEITHER arbitrary-write / transport-address leak; mountmgr IOCTL validation; keyboard/mouse class pending-IRP handling; dbgk fault-under-mutex + kd64 physical R/W; WoW64 residual cross-process queries.
- **Non-admin networking (T66):** standard users can open `\Device\Tcp` and create/bind/listen/connect TCP plus UDP; privileged control (ARP/route/IP-address writes) stays admin-gated behind an explicit access check.
- **SMP:** per-CPU object-handle FirstFree cache (ABA-safe, kill-switch); per-CPU nonpaged pool descriptors; building on 2026.10-2's pool scaling.
- **Stability/robustness:** ramdisk I/O-range OOB guard; process-quota underflow and limited-quota fixes; bounded OOM fault retry; device query-remove/eject; a real-hardware FreeLdr video-mode fallback (boots where a passed-through/real VBIOS disagrees with INT 10h).

## Bug fixes

<details><summary>Kernel: Ke, HAL and amd64 (2 fixes)</summary>

- NTOS:KE/x64: do not copy undeclared exception parameters to user mode ([c2855982a39](https://github.com/nitroos-org/reactos/commit/c2855982a39))
- SDK/RTL: do not leave the user pointer in `Dest` when `ProbeAndCaptureUnicodeString` fails to allocate ([fa8a0c7a7db](https://github.com/nitroos-org/reactos/commit/fa8a0c7a7db))
</details>

<details><summary>Kernel: Mm and Cc (10 fixes)</summary>

- NTOS:MM: bound the `MmAccessFault` OOM retry taken under a held lock, so a low-memory fault can no longer spin forever ([fix/mm-mdl-oom-bounded](https://github.com/nitroos-org/reactos/commit/8267776f1825dcc46e362795c100c969176e34bf))
- NTOS:MM: enforce a copy-on-write-aware maximum protection for image views ([6c3f58ed7ed](https://github.com/nitroos-org/reactos/commit/6c3f58ed7ed))
- NTOS:MM: enforce the section maximum protection in `MmProtectSectionView` ([ef493c744eb](https://github.com/nitroos-org/reactos/commit/ef493c744eb))
- NTOS:MM: do not trust the user buffer header in `MemorySectionName` ([efc948abe40](https://github.com/nitroos-org/reactos/commit/efc948abe40))
- NTOS:MM: preserve both boundary pages in `MmZeroSegmentRange` (`CcZeroData`) ([059d9068f2f](https://github.com/nitroos-org/reactos/commit/059d9068f2f))
- NTOS:EX/MM: true per-processor nonpaged small-block pool lookaside lists ([8cbd43795e2](https://github.com/nitroos-org/reactos/commit/8cbd43795e2))
- NTOS:MM: per-descriptor nonpaged pool spinlock + descriptor array (slice 1/4) ([fc189f7bc18](https://github.com/nitroos-org/reactos/commit/fc189f7bc18))
- NTOS:MM: wire nonpaged pool per-descriptor selection + header `PoolIndex` (slices 2+3/4) ([a1d5be35d5b](https://github.com/nitroos-org/reactos/commit/a1d5be35d5b))
- NTOS:MM: activate per-CPU nonpaged pool descriptors (slice 4/4) ([aab06648164](https://github.com/nitroos-org/reactos/commit/aab06648164))
- NTOS:EX/MM: restore lookaside depth adjustment, make the page-walk check DBG-only ([c01821e7e04](https://github.com/nitroos-org/reactos/commit/c01821e7e04))
</details>

<details><summary>Kernel: Ob, Se, Ps, Ex and registry (14 fixes)</summary>

- NTOS:OB/EX: per-CPU object-handle `FirstFree` cache, ABA-safe, behind a kill-switch (SMP lever) ([fix/handle-percpu-cache](https://github.com/nitroos-org/reactos/commit/44b34bde1cb4eff496f71f35c14a76aaf9aaa3be))
- NTOS:PS: fix a process-quota underflow on a self-block switch ([fix/ps-quota-selfblock-underflow](https://github.com/nitroos-org/reactos/commit/bfe5e8b940bc3e9fb33f72ea5072272e9499f951))
- NTOS:OB: remove now-unused `Total` locals (fix i386 Debug `-Werror`) ([d461572b669](https://github.com/nitroos-org/reactos/commit/d461572b669))
- NTOS:OB: per-processor cache-line-padded object-type counters ([475b1853035](https://github.com/nitroos-org/reactos/commit/475b1853035))
- NTOS:PS: do not drop the process reference on quota-block allocation failure ([6f285d389ab](https://github.com/nitroos-org/reactos/commit/6f285d389ab))
- NTOS:PS: honor requested pool limits in `PspSetQuotaLimits` ([9b8fd94d56e](https://github.com/nitroos-org/reactos/commit/9b8fd94d56e))
- NTOS:PS: use the matching return threshold for paged and nonpaged pool quota ([e78f377052c](https://github.com/nitroos-org/reactos/commit/e78f377052c))
- NTOS:PS: fix a wild write and uninitialized quota block in the limited-quota path ([a5d8172e23c](https://github.com/nitroos-org/reactos/commit/a5d8172e23c))
- NTOS:PS: charge and return default-block pool quota without `PspQuotaLock` ([cc478a6962d](https://github.com/nitroos-org/reactos/commit/cc478a6962d))
- NTOS:EX: do not trust the output buffer in `SystemProcessInformation` ([a3a0524b2be](https://github.com/nitroos-org/reactos/commit/a3a0524b2be))
- NTOS:PS: reject a cross-process thread in `NtGetNextThread` ([a72b2cb8ee1](https://github.com/nitroos-org/reactos/commit/a72b2cb8ee1))
- NTOS:PS: probe the user TLS slot in `ThreadZeroTlsCell` ([32a31cbba77](https://github.com/nitroos-org/reactos/commit/32a31cbba77))
- NTOS:dbgk/kd64: fix a `SetInformation` fault taken under a mutex + the kd64 physical read/write command ([fix/kd64-dbgk-capture](https://github.com/nitroos-org/reactos/commit/8be3bf4b6a7e761bbccac47b98d614e769ca5b15))
- SDK:CMLIB: reject undersized/unaligned hive cells before linking them (registry) ([cfe545a521e](https://github.com/nitroos-org/reactos/commit/cfe545a521e))
</details>

<details><summary>I/O manager and file-system libraries (3 fixes)</summary>

- NTOS:IO: serialize PnP property-change events with the queue lock ([745246dd841](https://github.com/nitroos-org/reactos/commit/745246dd841))
- MSFS: probe and capture `ReadTimeout` in `FileMailslotSetInformation` ([214bd796cb1](https://github.com/nitroos-org/reactos/commit/214bd796cb1))
- NPFS: fix an output overflow and a PEPROCESS leak in `FSCTL_PIPE_QUERY_CLIENT_PROCESS` ([8bab9c5b36d](https://github.com/nitroos-org/reactos/commit/8bab9c5b36d))
</details>

<details><summary>win32k, GDI and USER (8 fixes)</summary>

- WIN32K:GDI: user-pointer capture and size-math hardening (Astra#2) ([fix/win32k-gdi-usercapture](https://github.com/nitroos-org/reactos/commit/0bc42fa3afce25319b05be838a47bc1766c856e0))
- WIN32K:GDI: `NtGdiCreateDIBSection` capture + `ppvBits` arbitrary-write fix ([fix/win32k-dibsection-capture](https://github.com/nitroos-org/reactos/commit/90f6d75990b47cc67c54afa317cba03e5f766a3b))
- WIN32K: clipboard CF_DIB size check + `FamInfo` NULL-init ([fix/win32k-clipboard-faminfo](https://github.com/nitroos-org/reactos/commit/8112aa606f4c6a9476b58a1f4f80277180ec7db1))
- WIN32K:USER: `NtUserGetAtomName` double-fetch ([fix/useratom-doublefetch](https://github.com/nitroos-org/reactos/commit/499b8487bd7602c03690c80226a8a18b137b69ca))
- WIN32K:USER: `NtUserGetIconInfo` probe + leak; `SetWindowsHookEx` `ModuleName.Length` OOB ([fix/ntuser-geticon-hook](https://github.com/nitroos-org/reactos/commit/0938fcc82440a5196104ae973002605a76567ec4))
- WIN32K:USER: `IntSetMenuInfo` kernel-pointer type-confusion EoP ([fix/menu-typeconfusion](https://github.com/nitroos-org/reactos/commit/b9ac8118a69d10b584966de921c9b6bfd3a1bfae))
- WIN32SS:NTUSER: `NtUserSetWindowWord` OOB write when `cbwndExtra < 2` (unsigned underflow) ([0aeb445daec](https://github.com/nitroos-org/reactos/commit/0aeb445daec))
- WIN32SS:ENG: reject an overflowing bitmap size in `SURFACE_AllocSurface` ([258179096ed](https://github.com/nitroos-org/reactos/commit/258179096ed))
</details>

<details><summary>WoW64 (2 fixes)</summary>

- WoW64: amd64 fibers + `NtQuerySystemInformationEx` + wow64 test crashes ([fix/wow64-fiber-threadexit](https://github.com/nitroos-org/reactos/commit/cfcc82290cb59c031c40a09be468391b493a88e8))
- WoW64: residual cross-process queries — `ProcessImageInformation`, `ThreadDescriptorTableEntry` ReturnLength, amd64 `Win32StartAddress` ([fix/wow64-residual-queries](https://github.com/nitroos-org/reactos/commit/ae88a070738a704844a15edb3c01a1e4a29a13cd))
</details>

<details><summary>Networking (4 fixes)</summary>

- AFD: window-size SET_INFO use-after-free + enum-events overflow + METHOD_NEITHER arbitrary-write + transport-address leak ([fix/afd-cluster](https://github.com/nitroos-org/reactos/commit/09c310fbc8bc2e5292dccada6adc1490c157c1ad))
- AFD: `AfdAccept` length + requestor-mode checks, `LockBuffers` probe, `AfdCreateSocket` EA over-read ([fix/afd-input-validation](https://github.com/nitroos-org/reactos/commit/52c66667ac3776306e261fc7400151ea69a30e98))
- TCP/IP: non-admin TCP/UDP via an Everyone ACE + an explicit control-access gate for privileged requests ([fix/tcp-nonadmin-dacl](https://github.com/nitroos-org/reactos/commit/6d1cf318108fddd8f11a6162b304294f98e0b86f))
- Network: time-bound the receive DPC so one pass cannot hold DISPATCH_LEVEL for seconds ([47b12cdea20](https://github.com/nitroos-org/reactos/commit/47b12cdea20))
</details>

<details><summary>Audio and multimedia (1 fix)</summary>

- KS (kernel streaming): user-buffer capture — method/property/event double-fetch + type-confusion ([fix/ks-userbuffer-capture](https://github.com/nitroos-org/reactos/commit/53adfb2fadadf0e9d1a67a77c736cff5b01034a6))
</details>

<details><summary>Storage, USB, PnP and drivers (4 fixes)</summary>

- mountmgr: IOCTL input validation ([fix/mountmgr-p1-ioctl-validation](https://github.com/nitroos-org/reactos/commit/701bb72d90524a2d0fa546f12dafebcf7284f22c))
- ramdisk: `RamdiskReadWrite` OOB guard + NULL-MDL / uninit-relations / self-deadlock / loader-block / PnP-IRP fixes ([fix/ramdisk-native-bundle](https://github.com/nitroos-org/reactos/commit/91a039cb373ff4190693db851f468d2a2b99cf93))
- kbdclass/mouclass: a second read no longer orphans a pending IRP ([fix/kbdmou-pendingirp](https://github.com/nitroos-org/reactos/commit/3732b730437d1958aaf6c6b0e58f1844c24cb73a))
- PnP: device query-remove / eject / disable / uninstall, with correct veto (`CR_REMOVE_VETOED`) ([unit/queryremove](https://github.com/nitroos-org/reactos/commit/34d861c96fb6a1aa29c749a08543c1e495a95611))
</details>

<details><summary>Setup and boot (1 fix)</summary>

- FreeLdr: do not assert on a BDA-vs-INT-10h video-mode mismatch, so it boots on real hardware / a passed-through GPU VBIOS ([fix/freeldr-pcvideo-bda](https://github.com/nitroos-org/reactos/commit/743b3ae0f9b2453c0d9dc55e34a8ed8375c3185c))
</details>

<details><summary>Browser, COM and Wine imports (1 fix)</summary>

- MSHTML: do not re-set an unchanged link media, fixing an IE hang and runaway memory on `loadCSS` pages ([b8fc159f5ca](https://github.com/nitroos-org/reactos/commit/b8fc159f5ca))
</details>

<details><summary>Tests only (6 fixes)</summary>

- kmtest: fix a `PsQuotaLimitedTrim` double-eval that tripped a teardown ASSERT (test-only) ([fix/quota-trim-bugcheck](https://github.com/nitroos-org/reactos/commit/1f283a97d74648cfcddf38729052a53897f56073))
- KMTESTS: `PsQuotaLimited` tests for private quota blocks ([a80df7f7dc4](https://github.com/nitroos-org/reactos/commit/a80df7f7dc4))
- MSFS_APITEST: `FileMailslotSetInformation` contract test ([4d9acd1d170](https://github.com/nitroos-org/reactos/commit/4d9acd1d170))
- NPFS_APITEST: `FSCTL_PIPE_QUERY_CLIENT_PROCESS` contract test ([e85e6df6324](https://github.com/nitroos-org/reactos/commit/e85e6df6324))
- NTDLL_APITEST: `SystemProcessInformation` image-name and fault tests ([ee26d6920f0](https://github.com/nitroos-org/reactos/commit/ee26d6920f0))
- NTDLL_APITEST: `NtQueryVirtualMemory` `MemorySectionName` tests ([9a261367d2a](https://github.com/nitroos-org/reactos/commit/9a261367d2a))
</details>

### Limitations
- 32-bit Firefox renders blank at the default sandbox (WoW64 sandbox limitation).
- APIC HAL: single I/O-APIC, active-high; MADT polarity overrides not honored (real-hardware risk; QEMU-tolerated).
- `accept()` returns `WSAEACCES` for all users — no inbound TCP servers yet; outbound/client TCP works.
- Cross-process `WriteProcessMemory`/`VirtualProtectEx` into a remote process's image data sections fails — a side-effect of the W^X image-protection hardening in this release (breaks sandboxed Chromium/Electron children and code injectors; the hardening is kept, the remote-write path is fixed in a follow-up).
- OS identity is `5.2.3790` (Server 2003); applications that require Windows 7 (e.g. current Firefox ESR) will refuse to run.

### Changes since the previous Nitro
On the same upstream base as 2026.10-2 (no new upstream sync). 56 changes on `main`'s first-parent spine since the `nitro-2026.10-2` tag: 22 reviewed fix-branch merges plus 34 direct commits — a kernel/win32k security sweep, SMP pool and handle-cache work, quota and Mm hardening, and storage/networking fixes (grouped above).

## reactos-nitro 2026.10-2

**Tagged `nitro-2026.10-2`** (2026-10-20) at fork `main` [`054e4af6a88`](https://github.com/nitroos-org/reactos/commit/054e4af6a88f283b3e149103ded67ac211562ba2). A security + upstream-sync point release on top of 2026.10-1.

| Item | Value |
|---|---|
| Release tag | `nitro-2026.10-2` → [`054e4af6a88`](https://github.com/nitroos-org/reactos/commit/054e4af6a88f283b3e149103ded67ac211562ba2) |
| ReactOS base | upstream [`f53b1d15601`](https://github.com/reactos/reactos/commit/f53b1d1560120c529763b18963fdee68f39118af) (sync #3, 2026-10-19) |
| Patch series | 52 cumulative topic patches on `f53b1d15601` (2026.10-1's 48 + the 4 new this release, listed under Bug fixes); applied tree `e0cf06866f46` (== main) |
| Artifacts | amd64-with-WoW64 ISO, i386 ISO, source tarball, patch series |

### Highlights
4 security fixes plus upstream sync #3 (Wine-10.0 DCOMP/TBS/WEVTAPI/BCP47LANGS import + a basic IShellItem2). Gate 8/8, zero T1 regressions over the identical base.

### What reactos-nitro adds on top of ReactOS
#### Upstream sync (#3)
Merged ReactOS master up to `f53b1d15601`: Wine-10.0 import of DCOMP, TBS, WEVTAPI and BCP47LANGS, plus a basic IShellItem2. The fork kernel is kept byte-identical to the previous release (one upstream KE change set aside to preserve the fork's cross-processor lock handling).

## Bug fixes

<details><summary>Kernel: Ke, HAL and amd64 (2 fixes)</summary>

- NTOS:KE amd64: mask user-supplied MXCSR to the processor-supported bits before load, so a reserved-bit thread context can no longer GP-fault the kernel on `NtContinue`/`SetThreadContext` return-to-user ([fix/amd64-mxcsr-sanitize](https://github.com/nitroos-org/reactos/commit/5c8a2ac7e054fea52e646251c8416c38be9a695b))
- NTOS:KE/EX: bounds-check the kernel profiling object — reject the exclusive endpoint and compute the bucket count as SIZE_T, preventing an out-of-bounds counter write ([fix/ke-profile-bounds](https://github.com/nitroos-org/reactos/commit/77dbdcff4eba391b844a8359caeb8db3cf62baad))
</details>

<details><summary>Kernel: Ob, Se, Ps, Ex and registry (1 fix)</summary>

- NTOS:SE/OB: return a mandatory-label SACL on process and thread objects, as Windows does; also fixes an object-manager use-after-free in the insert-cleanup path and removes a stopgap that failed every label query ([fix/se-process-thread-integrity-label](https://github.com/nitroos-org/reactos/commit/1b6aa10b6e09d59b2d2789ea0f173898da88f0c0))
</details>

<details><summary>win32k, GDI and USER (1 fix)</summary>

- WIN32K:GDI: initialize the `ExtEscape` result so the failure paths cannot return uninitialized kernel stack bytes to user mode ([fix/gdi-extescape-result-init](https://github.com/nitroos-org/reactos/commit/4748c129fd8b3e69f8317cba6ac7e89a41c7dfef))
</details>

### Limitations
Unchanged from 2026.10-1 — including that 32-bit Firefox under WoW64 renders only with its content sandbox disabled.

### Changes since the previous Nitro
This is a point release: the section above lists only what's **new in 2026.10-2** (upstream sync #3 + 4 security fixes). **2026.10-2 includes everything from 2026.10-1** — the full ~554 fixes and all feature tracks are in the [2026.10-1 section below](#reactos-nitro-20261). Gate 8/8; T1 (154 shards) zero batch-introduced regressions.

## reactos-nitro 2026.10-1

**Tagged `nitro-2026.10-1` (private) on 2026-10-19**, annotated tag at fork `main` [`c203b29276c`](https://github.com/nitroos-org/reactos/commit/c203b29276c5e7d4fc54177d8d999c54d5cb2a45). Gated green (both ISOs boot-tested; TC 32/64, WoW64-GUI, Firefox 64-bit, NTFS 4h soak, licence-clean all PASS). **Not yet public** — publication is held pending the FF32-under-WoW64 render fix (see Known limitations), a trademark check of the "reactos-nitro" name, and the NTFS driver licence-statement clean-up.

| Item | Value |
|---|---|
| Release tag | `nitro-2026.10-1` → [`c203b29276c`](https://github.com/nitroos-org/reactos/commit/c203b29276c5e7d4fc54177d8d999c54d5cb2a45) (private fork) |
| ReactOS base | upstream [`ac1ab9e3043`](https://github.com/reactos/reactos/commit/ac1ab9e30438dedbc1a2216db8ca8682f5fc871f) "[DISKPART] Sort the volumes list", committed 2026-10-18 11:12 UTC |
| Patch series | 48 topic patches on `ac1ab9e3043`, series head [`926768aba66`](https://github.com/nitroos-org/reactos/commit/926768aba66); applied tree byte-identical to the tag (`94e948f52aad`) |
| Artifacts | amd64-with-WoW64 ISO (444,596,224 B), i386 ISO (391,118,848 B), source tarball, per-topic patch series — rebuilt from the series at release time (see RELEASE-PROCESS) |

### Highlights

547 fix branches and several feature stacks, 2,316 non-merge commits on top of upstream ReactOS `ac1ab9e3043`.

- Run 32-bit Windows programs on the 64-bit build (WoW64); Total Commander and Firefox ESR, each as 32-bit and 64-bit side by side, are the proof. Upstream has no WoW64.
- A new NTFS driver, built on the Linux kernel's `fs/ntfs` with our own journal and power-cut recovery; NTFS is the default file system.
- Btrfs: WinBtrfs 1.10 (upstream ReactOS ships 1.8.1).
- Multi-processor support on x86 and amd64, installed by default; upstream's amd64 kernel does not start extra processors.
- Smooth window dragging on AMD hosts (shadow framebuffer, USB polling and pointer fixes).
- Faster and sturdier networking: a reworked receive path and 57 networking fixes.
- Setup and profiles: setup preselects NTFS, upstream's C:\Users layout with Windows-style legacy folder links, working profile deletion.
- Memory manager and cache hardening, including commit charging with a commit limit.
- Desktop and tools: LCD text rendering, bundled open fonts, crash reports with a Windows 7-style dialog, Disk Management.
- Not yet: the Firefox sandbox starts on 64-bit but does not restrict anything (see Limitations).

### What reactos-nitro adds on top of ReactOS

For each area: what upstream ReactOS has at the base commit, then what reactos-nitro adds.

#### WoW64: 32-bit programs on 64-bit ReactOS

**ReactOS master ([`ac1ab9e3043`](https://github.com/reactos/reactos/commit/ac1ab9e3043)):**
- Not supported: there is no `dll/wow64`, and process creation reports "WOW64 is not supported yet" ([CORE-17655](https://jira.reactos.org/browse/CORE-17655)).

**reactos-nitro 2026.10-1:**
- 32-bit Windows programs run on the 64-bit build (amd64 only; on in the WoW64 ISO).
- SysWOW64, system-call and win32k thunks, file-system and registry redirection, and 32-bit consoles, windows, threads and process creation.
- Total Commander 11.58 and Firefox ESR 128, each 32-bit and 64-bit, run side by side in one session.

#### NTFS

**ReactOS master ([`ac1ab9e3043`](https://github.com/reactos/reactos/commit/ac1ab9e3043)):**
- The original ReactOS NTFS driver, with install support and write enabled by default on a normal install. It is still labelled experimental (the enabling registry value is named `MyDataDoesNotMatterSoEnableExperimentalWriteSupportForEveryNTFSVolume`), partial (several operations still return STATUS_NOT_IMPLEMENTED), and has no journaling or power-cut recovery.

**reactos-nitro 2026.10-1:**
- `ntfsng`, a read/write NTFS driver built on the Linux kernel's `fs/ntfs` ([Linux v7.3-rc6](https://github.com/nitroos-org/reactos/commit/77c12ef7d52bc291a0243ef5aeac1fa79c8b69cd), imported unmodified; GPL-2.0-or-later except two MIT files), running through a compatibility layer.
- Journaling with replay after a power cut, and a boot-time consistency check.
- NTFS is the default system file system, and setup preselects it.

#### Btrfs

**ReactOS master ([`ac1ab9e3043`](https://github.com/reactos/reactos/commit/ac1ab9e3043)):**
- WinBtrfs 1.8.1.

**reactos-nitro 2026.10-1:**
- [WinBtrfs 1.10](https://github.com/maharmstone/btrfs#changelog), which brings in 1.8.2, 1.9 and 1.10:
-   - the block group tree from Linux 6.1;
-   - writing free-space bitmaps and compressed inline files;
-   - better handling of removable devices and of out-of-memory/out-of-space errors;
-   - fixes for crashes when moving files across subvolumes or creating many files at once;
-   - fixes for snapshot extent-tree corruption, a hard-link deadlock and a hang under heavy load.
- Our own fixes on top ([`fix/btrfs-sync-1.10-main`](https://github.com/nitroos-org/reactos/commit/54e35c388a2617297197655bdbae650d6e0092f5)): volumes from current btrfs-progs mount, `format /FS:BTRFS` works, and drive letters stick.

#### Display

**ReactOS master ([`ac1ab9e3043`](https://github.com/reactos/reactos/commit/ac1ab9e3043)):**
- The VBE/Bochs `framebuf` driver draws straight into video memory.

**reactos-nitro 2026.10-1:**
- A shadow framebuffer: drawing goes to a cached surface and only changed areas are copied to video memory, which makes window dragging smooth and cheap.
- USB UHCI polling and software-pointer fixes, so pointer movement doesn't flicker.

#### SMP and kernel

**ReactOS master ([`ac1ab9e3043`](https://github.com/reactos/reactos/commit/ac1ab9e3043)):**
- amd64: the processor start-up routines (`KeStartAllProcessors`, `HalStartNextProcessor`) are stubs, so amd64 runs on one processor.
- i386: an MP kernel and HAL are built, but setup selects the single-processor kernel.

**reactos-nitro 2026.10-1:**
- Multi-processor kernels on x86 and amd64; setup installs the MP kernel and HAL on ACPI machines.
- Tested from 1 to 16 vCPUs, with long soaks on AMD and Intel hosts.
- Much faster cross-processor thread hand-off and wake-up.
- A fix for the root cause of an Intel-only STOP 0x7E.
- Fixes for x87/FPU state handling, cross-processor TLB flushes (batched and targeted), and amd64 interrupt, NMI and exception delivery.
- 32 kernel fixes in the list below.

#### Networking

**ReactOS master ([`ac1ab9e3043`](https://github.com/reactos/reactos/commit/ac1ab9e3043)):**
- The lwIP-based TCP/IP driver and AFD.

**reactos-nitro 2026.10-1:**
- A reworked receive path: lwIP core lock, in-order TCP receive per adapter, fewer thread hops and reassembly locking.
- 57 networking fixes: AFD connect, accept and cancel paths, TCP/IP, Winsock and iphlpapi.

#### Setup and user profiles

**ReactOS master ([`ac1ab9e3043`](https://github.com/reactos/reactos/commit/ac1ab9e3043)):**
- NTFS install support (formatter and boot sector) and the C:\Users profile layout.

**reactos-nitro 2026.10-1:**
- The graphical setup preselects NTFS.
- The C:\Users layout, with fixes for shell calls that crash in upstream's stubs, and a read-only Downloads folder.
- Windows-style legacy links (`Documents and Settings`, `Default`, `ProgramData`).
- `DeleteProfile` works and refuses to delete a loaded profile. Upstream's version cannot succeed.

#### Disk Management

**ReactOS master ([`ac1ab9e3043`](https://github.com/reactos/reactos/commit/ac1ab9e3043)):**
- Only the `diskmgmt.msc` console file; there is no Disk Management implementation.

**reactos-nitro 2026.10-1:**
- Disk Management (`diskmgmt.msc`) as a standalone application with a Windows-like disk list and graphical layout view. It isn't built on ReactOS's in-progress MMC; porting it to MMC is planned.
- Covers basic, extended and GPT disks, SAS disks and optical drives.
- Disk operations:
  - initialize a disk (MBR or GPT);
  - create, delete and format volumes;
  - shrink and extend volumes in place;
  - change drive letters and mount points.
- Windows 7-style dialogs and wizards.
- Write safety: changes are planned against the disk layout the user saw, and are re-checked before writing.
- `SetVolumeMountPointW` and `DeleteVolumeMountPointW` implemented in kernel32.

#### Internet Explorer and the web stack

**ReactOS master ([`ac1ab9e3043`](https://github.com/reactos/reactos/commit/ac1ab9e3043)):**
- Wine-based `iexplore` with mshtml/urlmon/wininet. Wine Gecko is not provided on amd64.

**reactos-nitro 2026.10-1:**
- Wine Gecko is provided on amd64, and `iexplore` offers to install Gecko when it's missing.
- File downloads work: download dialogs and engine in ieframe, `DoFileDownload` from shdocvw, and content the page can't show is handed to the download path.
- Fixes:
  - urlmon tasks queued during a read are now run;
  - wininet fails stalled reads after the timeout and reports damaged gzip bodies like Windows;
  - raw HTTP headers are read as ANSI;
  - saving an Internet shortcut whose properties are open works.

#### Other

**reactos-nitro 2026.10-1:**
- ARM3 commit charging with a commit limit, so over-commit fails with STATUS_COMMITMENT_LIMIT, plus special-pool debugging.
- LCD text rendering and bundled open fonts.
- Crash dumps, Application Error event 1000 and a Windows 7-style crash dialog.
- Wine syncs (Media Foundation, kernel32 profile, setupapi) and upstream sync #2.

### Fixed upstream ReactOS JIRA reports

These reactos-nitro fixes address bugs reported in the [ReactOS JIRA](https://jira.reactos.org). They're listed here to make porting easier. The list includes only reports that clearly describe the same bug. Some reports were already closed upstream, for example as *Won't Fix*. Fixes with possible but unconfirmed matches are not listed.

| Upstream report | Summary (status at lookup) | Fixed in reactos-nitro by |
|---|---|---|
| [CORE-8717](https://jira.reactos.org/browse/CORE-8717) | ConvertStringSidToSid succeeds for empty string and other invalid SIDs / Reopened / Unresolved | [fix/advapi32-convert-string-sid](https://github.com/nitroos-org/reactos/commit/9bb9005936e7f4f25b4672c3eeaf7e4200f30a2c) |
| [CORE-12498](https://jira.reactos.org/browse/CORE-12498) | telnet is looking for telnet.ini which is not distributed with ReactOS; Reopened, no resolution | [fix/telnet-install-config](https://github.com/nitroos-org/reactos/commit/b5e19db1e584c47709cc4a7b25b1e65b1ead7206) |
| [CORE-12604](https://jira.reactos.org/browse/CORE-12604) | ReactOS executes extensionless files in PATH / Open / Unresolved | [fix/cmd-search-extensionless](https://github.com/nitroos-org/reactos/commit/a2cf87a2385c45912696d67519e596a4174dca56) |
| [CORE-14795](https://jira.reactos.org/browse/CORE-14795) | CMD ECHO - incorrect text output to file / Untriaged / Unresolved | [fix/cmd-redirect-ansi](https://github.com/nitroos-org/reactos/commit/25a3be3914f44d06b7225f2e21cdac91bb22e5a6) |
| [CORE-15934](https://jira.reactos.org/browse/CORE-15934) | REGRESSION: Resizing a themed window causes a classic frame to be drawn temporarily and makes the caption blink (Resolved/Fixed) | [fix/win32k-framechanged-screen-region](https://github.com/nitroos-org/reactos/commit/65fdcb4306f4ab6641b71bf3e03df3dffcd2a6bd) |
| [CORE-16061](https://jira.reactos.org/browse/CORE-16061) | Vista Paint and Minesweeper cannot locate RegisterApplicationRestart in KERNEL32.DLL - Resolved/Postponed | [fix/kernel32-application-restart](https://github.com/nitroos-org/reactos/commit/632cc8034617b0b37c1363e672e32208f73efb34) |
| [CORE-16273](https://jira.reactos.org/browse/CORE-16273) | IniFileMapping is not implemented - Untriaged | [fix/kernel32-profile-sync](https://github.com/nitroos-org/reactos/commit/9b644b68fd6cd4cde766f50f9efd43dd874e9690) |
| [CORE-16444](https://jira.reactos.org/browse/CORE-16444) | Shell Find Fails on some Folders using Shell Extensions (RECYCLED or fonts) (Open) | [fix/shellfind-font-folder-results-v2](https://github.com/nitroos-org/reactos/commit/c56e3d5307a30e75516230022181244aa92c9468) |
| [CORE-17317](https://jira.reactos.org/browse/CORE-17317) | [Kernel32] Implement missing I64 flag in format_message / Untriaged / Unresolved | [fix/kernel32-formatmessage-int64](https://github.com/nitroos-org/reactos/commit/a96335568d79b05b0ee8277326c64ab540f7d2ac) |
| [CORE-18390](https://jira.reactos.org/browse/CORE-18390) | SPI_GETWHEELSCROLLCHARS incorrectly reported as invalid - Resolved/Postponed | [fix/win32k-wheel-scroll-chars](https://github.com/nitroos-org/reactos/commit/1e51c969ad44e733c335feea6ef3a0cd9b95120b) |
| [CORE-18789](https://jira.reactos.org/browse/CORE-18789) | FILE_APPEND_DATA not handled correctly in NtCreateFile & friends; Untriaged/None | [fix/append-write](https://github.com/nitroos-org/reactos/commit/678833d1e2cd9a26c1b099354419977cf393455e) |
| [CORE-19707](https://jira.reactos.org/browse/CORE-19707) | Inno Setup 6 based installers do not work / Untriaged / Unresolved | [fix/ntdll-actctx-loadfrom](https://github.com/nitroos-org/reactos/commit/53e2c8a0ee496debe89db83ff5ae52e794d8a368) |
| [CORE-19855](https://jira.reactos.org/browse/CORE-19855) | Shortcut's 'Start in' field treated incorrectly - Open | [fix/shell32-exec-context-menu-directory](https://github.com/nitroos-org/reactos/commit/8d089d7697b4e47b9b0937297caf9c76e75397f4) |
| [CORE-20067](https://jira.reactos.org/browse/CORE-20067) | [MAGNIFIER] Help button should be grayed out (Resolved, Won't Fix) | [fix/help-unavailable-disabled](https://github.com/nitroos-org/reactos/commit/5a03092962cdd75111e756a8604d5c4da784d3ec) |
| [CORE-20129](https://jira.reactos.org/browse/CORE-20129) | Broken file operations when using folder starting by '...' - Open | [fix/shlwapi-path-dotdot-prefix](https://github.com/nitroos-org/reactos/commit/2264a530e0fc1ef84956ed1c9f945407adbb4670) |
| [CORE-20687](https://jira.reactos.org/browse/CORE-20687) | [COMCTL32] Bugs when setting the title of property sheets or wizards - Untriaged | [fix/comctl32-propsheet-title](https://github.com/nitroos-org/reactos/commit/d99b70541d359a58f062d3fac4840812991db9c6) |
| [CORE-20756](https://jira.reactos.org/browse/CORE-20756) | Notepad > Save as 'All files' adds the extension anyway - Untriaged | [fix/comdlg32-defext-keep-extension](https://github.com/nitroos-org/reactos/commit/a14b21ad5a83a3be5ef1e0b441a10b1ebb6343a2) |

## Bug fixes

554 fixes: one line for each of the 547 fix branches merged into Nitro main and each of 7 small fixes committed directly on main, grouped by area (click an area to open it). The wording comes from the fix's own commit message; security-related fixes are described only generically. Each branch name links to its last commit on GitHub, and a ReactOS JIRA ticket is named where we found a clear match.

<details><summary>Kernel: Ke, HAL and amd64 (32 fixes)</summary>

- HAL: Make the DMA lock exclude other threads ([fix/hal-dma-32bit-bounce](https://github.com/nitroos-org/reactos/commit/10bd01a90bd42528682d7451609ff5ebb1e42eb5))
- HALX86: Refuse ports outside the PCI configuration range in the AMLI handler ([fix/hal-amli-pci-handler](https://github.com/nitroos-org/reactos/commit/c75f7702b8e0e45d98753652c250de2cee294a3d))
- HALX86: Send the APC interrupt EOI before enabling interrupts ([fix/hal-apc-eoi](https://github.com/nitroos-org/reactos/commit/239f92ea5cc37d6c58a49d779482f80a130c9bb3))
- HALX86: Count lost RTC periods in the APIC HAL clock interrupt ([fix/hal-clock-lost-ticks](https://github.com/nitroos-org/reactos/commit/f7a2c6672ae31354db510517a45fd6e10a201b90))
- HALX86: Reset the map register index for each queued channel grant ([fix/hal-dma-queued-channel](https://github.com/nitroos-org/reactos/commit/f6989394c945c684ed0d92c4db210d1af3b8a8f3))
- HALX86: Write a redirection entry's destination before unmasking it ([fix/hal-ioapic-destination-order](https://github.com/nitroos-org/reactos/commit/2c1e0e009e5e1db870445b4ae820b2aefeba1092))
- HALX86: Do not read the I/O APIC when deferring an interrupt ([fix/hal-ioapic-vector-atomic](https://github.com/nitroos-org/reactos/commit/e0321c602a08173391a14aeb539e51ca8caa765b))
- HALX86: Take the CMOS lock to acknowledge the RTC clock interrupt ([fix/hal-rtc-ack-lock](https://github.com/nitroos-org/reactos/commit/8f6ce562564491926c2f65ee60224ea7e4a0081c))
- HALX86: Read RTC register C after enabling the clock interrupt ([fix/hal-rtc-register-c](https://github.com/nitroos-org/reactos/commit/f170a6d4208a954456463e901130db12648db1de))
- HALX86: Do not reset the TSC during calibration ([fix/hal-tsc-no-reset](https://github.com/nitroos-org/reactos/commit/1e0ba781a47ffc3c5be81ca8435b550b1148e336))
- HALX86:APIC: Lower the lazy IRQL with interrupts disabled ([fix/hal-apic-lazy-irql-lower](https://github.com/nitroos-org/reactos/commit/8e53fe18c4c6e656a25bdb13c95de384041b6c0d))
- HALX86:APIC: fix a security-related bug ([fix/hal-apic-profile-trap](https://github.com/nitroos-org/reactos/commit/dc551055776e7dd297683dce8e9e0f5f0d295a32))
- NTOS:EX: Report a keyed event pairing to the side whose wait just timed out ([fix/ke-keyed-event-timeout-race-v2](https://github.com/nitroos-org/reactos/commit/d1e415ade66f7d20f31aa0efe45289e82c847b1c))
- NTOS:EX: Return the cycle time of each process in SystemProcessInformation ([fix/ntos-cpu-usage-info-v2](https://github.com/nitroos-org/reactos/commit/07c6e88531a0cf1337c30db1178c7d442fc10c77))
- NTOS:KE: Let one processor report an unclaimed NMI on i386 ([fix/amd64-nmi](https://github.com/nitroos-org/reactos/commit/778999c6b4582c46c6cdced532c253577b4cccb3))
- NTOS:KE: Return STATUS_NOT_IMPLEMENTED from NtSetLdtEntries on amd64 ([fix/amd64-setldtentries-stub](https://github.com/nitroos-org/reactos/commit/3efcaaa6e695173100d8f3d3ed66da55c28864ab))
- NTOS:KE: Set the Inserted state in KeInsertByKeyDeviceQueue ([fix/devqueue-insert-by-key](https://github.com/nitroos-org/reactos/commit/da9c28ce8cdb5046318a58678487b0e8056ed565))
- NTOS:KE: Deliver a user mode fast fail as a second chance exception ([fix/fastfail-second-chance](https://github.com/nitroos-org/reactos/commit/c15cf6d6ae9b294cb43d8baa6b9ff4531f529048))
- NTOS:KE: Read the thread's queue under the dispatcher lock in KeWaitForGate ([fix/ke-gate-wait-queue](https://github.com/nitroos-org/reactos/commit/8bd04dc388dc7686bb142d830e565ad35bd7ff94))
- NTOS:KE: Clear a processor from the idle summary when it gets a thread ([fix/ke-idle-summary](https://github.com/nitroos-org/reactos/commit/123d2b8c4874a5dc41f9b6dff1696d7c842d0635))
- NTOS:KE: Read the thread state once in KiSetPriorityThread ([fix/ke-setpriority-state](https://github.com/nitroos-org/reactos/commit/61be28cac6e6a307f0d9e7c6c51e84764680a1da))
- NTOS:KE: Place deferred ready threads before choosing the next thread in KiSwapThread ([fix/ke-swap-deferred-ready](https://github.com/nitroos-org/reactos/commit/1ac8c3ce0f019e673cc2478f227737f7712e815c))
- NTOS:KE/NDK: fix a security-related bug ([fix/amd64-syscall-17-args](https://github.com/nitroos-org/reactos/commit/03fbea61318d2ba9aee3220f4761a584ce980444))
- NTOS:KE/x64: Deliver a user-mode stack fault as an exception ([fix/amd64-stack-fault-v2](https://github.com/nitroos-org/reactos/commit/ba212840961bd6f404d0d9f56601ab84e3ac2cd9))
- NTOS:KE/x64: Enable interrupts in trap handlers that can wait ([fix/amd64-trap-interrupts-main](https://github.com/nitroos-org/reactos/commit/95fe0f3940edcdf560bdddcbb3ca20f30442b090))
- NTOS:KE/x64: Send an EOI for an unexpected interrupt only when it is in service ([fix/amd64-unexpected-int-eoi](https://github.com/nitroos-org/reactos/commit/6abb8f88e5a1cb053c4ebeea72a2515a845f6cfb))
- NTOS:KE/x64: amd64 kernel delivers STATUS_STACK_OVERFLOW to the program instead of killing it when its stack runs out ([fix/ke-amd64-dispatch-stack-overflow-retry](https://github.com/nitroos-org/reactos/commit/f5dad9688000af58cf9ebe7321eb72c656ed7853))
- NTOS:KE/x64: Handle __fastfail (INT 29h) ([fix/ke-amd64-fastfail](https://github.com/nitroos-org/reactos/commit/fe5c75384c984a9a190fbd785bd669b95e51dca7))
- NTOS:MM: Size the amd64 paged pool like the x86 one ([fix/amd64-paged-pool-size](https://github.com/nitroos-org/reactos/commit/5e10b13e133f998898b7762bf6da96a5af939345))
- PSEH: Preserve amd64 funclet call frames and run scope cleanup ([fix/amd64-pseh-flags](https://github.com/nitroos-org/reactos/commit/61f1d6da5c0d159442ca859e3d6a5ffc0d061d41))
- PSEH: Keep x64 GCC handlers of try blocks that never complete ([fix/amd64-pseh-noreturn-try](https://github.com/nitroos-org/reactos/commit/c040901ca922f7ccf573f84e5b0436a4d29ec400))
- SDK:ASM: Lay out the amd64 user-mode system call stubs as Windows does ([fix/amd64-ntdll-stub-layout](https://github.com/nitroos-org/reactos/commit/c08bfc06aa9c9bdd4f98ccb0c075a9a52997bdb6))

</details>

<details><summary>Kernel: Mm and Cc (48 fixes)</summary>

- NTDLL:LDR: Do not refuse a relocated EXE that has a start context ([fix/mm-image-high-base](https://github.com/nitroos-org/reactos/commit/f2f40deb153c4367b1182deafe172017e53e41ce))
- NTOS:CC: Reference the shared cache map while CcFlushCache uses it ([fix/cc-flush-reference](https://github.com/nitroos-org/reactos/commit/f299f728af2d63032c54535724cdbb3a292f2d47))
- NTOS:CC: Fix the VACB range test in CcPurgeCacheSection ([fix/cc-purge-overlap](https://github.com/nitroos-org/reactos/commit/c3d34d7a2811036a117647be7d459a986ebd35ad))
- NTOS:CC/NTOS:MM: CcZeroData keeps mapped data when a file grows, OOM-safe [pick] ([main/uf23-cc-zero](https://github.com/nitroos-org/reactos/commit/435ad17941d59f1a42a615a3f8e5afcb7bc9289a))
- NTOS:CM: Read the special pool tag from the registry ([fix/mm-special-pool-tag-registry](https://github.com/nitroos-org/reactos/commit/af197ebd6a60c2bcce5b589d35df6dc63108d1c2))
- NTOS:KE: Retire DPCs on the DPC stack in the x86 idle loop ([fix/mm-idle-dpc-stack](https://github.com/nitroos-org/reactos/commit/8b9008304f7bf4694611f395ea760a0da49af8cc))
- NTOS:MM: Compute the next PTE from the PXE when skipping an empty PXE ([fix/mm-amd64-empty-pxe](https://github.com/nitroos-org/reactos/commit/b4c7fdc1b4b28e73f40baf1425be824a3ce8ec7d))
- NTOS:MM: Free the amd64 hyperspace page tables of a deleted process ([fix/mm-amd64-hyperspace-leak](https://github.com/nitroos-org/reactos/commit/37179bd152b5643e0b6ce1afffa3eea7510c9d06))
- NTOS:MM: Drop the LRU walk's page reference when the trim wraps around ([fix/mm-balancer-lru-reference](https://github.com/nitroos-org/reactos/commit/0d8c5d4b050da907fb633eb5ddb591075ed88244))
- NTOS:MM: Retry for free pages when the cache copies file data ([fix/mm-cache-page-in-wait](https://github.com/nitroos-org/reactos/commit/f66f6259db6a9beeac6fe45362f5e68bdca10e01))
- NTOS:MM: Throttle page requests only when pages or the cache are under pressure ([fix/mm-consumer-throttle](https://github.com/nitroos-org/reactos/commit/96bd31303c8949ddf2488e61d35ddf9392b11ea9))
- NTOS:MM: Make the private copy of a copy-on-write page writable on SMP ([fix/mm-cow-smp-write](https://github.com/nitroos-org/reactos/commit/b346641a808bc71112ef7268ca8a4459dd79d7b4))
- NTOS:MM: Make a copy-on-write view page writable once it is private ([fix/mm-cow-view-region](https://github.com/nitroos-org/reactos/commit/5ca9a1f960edde695f9c485295511848ccb2503c))
- NTOS:MM: Free pages in transition when decommitting them ([fix/mm-decommit-transition](https://github.com/nitroos-org/reactos/commit/969a8da8d03eead29931d3d1647cac40b096d2cc))
- NTOS:MM: Flush the TB once per page-table run when deleting virtual addresses ([fix/mm-delete-va-tb-batch](https://github.com/nitroos-org/reactos/commit/e5a7a55f29eabf0e038c0c1b79a81b33e0856465))
- NTOS:MM: Refuse a data section view with nothing to map ([fix/mm-extend-section-rounding-v3](https://github.com/nitroos-org/reactos/commit/a19716f2f52a038694ea0b8495efa5bbe951657f))
- NTOS:MM: Wait for TB flushes without holding the PFN lock ([fix/mm-flush-outside-pfn-lock](https://github.com/nitroos-org/reactos/commit/d3b23c5fd84e248bc669d5bb01167b5e46140427))
- NTOS:MM: Write adjacent dirty pages of a file with one paging write ([fix/mm-flush-write-clusters](https://github.com/nitroos-org/reactos/commit/05d888150c56363b55e075f0113774abcdfa814b))
- NTOS:MM: Return STATUS_IMAGE_MACHINE_TYPE_MISMATCH for a foreign image view ([fix/mm-image-machine-mismatch](https://github.com/nitroos-org/reactos/commit/65bd363f23ce52e924fa9b6c2cb876ef2aec4fcf))
- NTOS:MM: fix a memory-safety bug ([fix/mm-image-unmap-uaf](https://github.com/nitroos-org/reactos/commit/b397b92d605d3b82c37a2cd2ccdba0a614e460ec))
- NTOS:MM: fix a security-related bug ([fix/mm-image-validation](https://github.com/nitroos-org/reactos/commit/cbc5dbd60675518a336f27cd07d61e0231b4eb2a))
- NTOS:MM: Tear down user mappings of locked MDLs at process exit ([fix/mm-mdl-view-exit](https://github.com/nitroos-org/reactos/commit/98fec594f131c454da9b42180ca2ff073329c5c3))
- NTOS:MM: Fail a paged pool expansion when too few pages are available ([fix/mm-pagedpool-expand](https://github.com/nitroos-org/reactos/commit/eb34142a6af68925547659c232a43de9894ea5d7))
- NTOS:MM: fix a security-related bug ([fix/mm-pagingfile-map-free](https://github.com/nitroos-org/reactos/commit/ab29b388276b4e1dd15a69909a44afe303b7a7fc))
- NTOS:MM: Fail a prototype PTE fault with STATUS_NO_MEMORY ([fix/mm-proto-fault-no-memory](https://github.com/nitroos-org/reactos/commit/21cb2fadbc338ef078cd984fc84bd13914f34b1e))
- NTOS:MM: Keep MEM_MAPPED for ARM3 section views in NtQueryVirtualMemory ([fix/mm-queryvm-section-view](https://github.com/nitroos-org/reactos/commit/69ddbf5f1130d357f33395cf039d94fa34d10be5))
- NTOS:MM: Map ordinary RAM cached: give its PFNs the MiCached attribute ([fix/mm-ram-cache-attribute-v2](https://github.com/nitroos-org/reactos/commit/43745bdaf7f5ce79dc35a04db58047925a9d62bd))
- NTOS:MM: Do not wait for the balancer while holding address space locks ([fix/mm-rebalance-wait-locks](https://github.com/nitroos-org/reactos/commit/2fa185eb5d883de51edee21cc92f1a305bd05213))
- NTOS:MM: Charge commitment once per committed page ([fix/mm-recommit-charge](https://github.com/nitroos-org/reactos/commit/bf7ae60bee7ac3f2b9347c4f2b6a5c35bb2fe5b3))
- NTOS:MM: Take the rmap entry of a section fault before mapping the page ([fix/mm-rmap-alloc-wait](https://github.com/nitroos-org/reactos/commit/b15617905a77e90d9b22ae3642dff7dcc370ac94))
- NTOS:MM: Return STATUS_SECTION_TOO_BIG for sections larger than they may be ([fix/mm-section-too-big](https://github.com/nitroos-org/reactos/commit/330e4bcdb6e88d3a66f4b12c2facd3a8f9e7baa7))
- NTOS:MM: Purge and flush a data section up to its highest page table ([fix/mm-segment-range](https://github.com/nitroos-org/reactos/commit/f6cfb1956dea647792246cb02bc023486b301c6c))
- NTOS:MM: Check for a single-entry import list before reading its count ([fix/mm-single-import](https://github.com/nitroos-org/reactos/commit/683e988267e16659bb4908f34be786aba8a889ab))
- NTOS:MM: fix a memory-safety bug ([fix/mm-stack-guarantee](https://github.com/nitroos-org/reactos/commit/488a6e8833112b240821f61152bae3dc20837c46))
- NTOS:MM: Forget the saved swap entry of a page whose write failed ([fix/mm-swap-bitmap](https://github.com/nitroos-org/reactos/commit/246e8084dfbe6b8195801745e4ff14da8a5f2e9d))
- NTOS:MM: Bring a locked page back from transition without unlinking it ([fix/mm-transition-original-pte](https://github.com/nitroos-org/reactos/commit/4ab6a69d9768d83e8a7522221b63d05d7762d028))
- NTOS:MM: Do not let a file be truncated under a mapped view ([fix/mm-truncate-mapped-view](https://github.com/nitroos-org/reactos/commit/8ebff34b520e7ba9f6a9b07efa45ac03cecfd629))
- NTOS:MM: Wait for paging file I/O on a view before freeing it ([fix/mm-unmap-wait-entry](https://github.com/nitroos-org/reactos/commit/9c62d2ecac20b50834d5ed396acb0e54155d4cc0))
- NTOS:MM: Flush decommitted and deleted user pages on the process's processors only ([fix/mm-user-tb-flush-targets](https://github.com/nitroos-org/reactos/commit/956de32e504b6aeaec26b298ab80a0f2c91b1577))
- NTOS:MM: Unlock a VirtualLock'd page made inaccessible ([fix/mm-virtuallock-delete](https://github.com/nitroos-org/reactos/commit/50531cdb0b1fe0ff933a79b4f673f0ea7d12a63f))
- NTOS:MM: Resolve a transition fault without requiring free pages ([fix/mm-zero-free-pages](https://github.com/nitroos-org/reactos/commit/02de0dbbf85cdd08dd1593515f286ffa37d9134e))
- NTOS:MM: Zero free pages through a cached mapping and flush them ([fix/mm-zero-pages-cached](https://github.com/nitroos-org/reactos/commit/c84aea2c9192bbcfa0350169a076123ac4385ec0))
- NTOS:MM: Do not put the active processors into a new process's PEB affinity mask ([fix/process-affinity-peb](https://github.com/nitroos-org/reactos/commit/22f27fd354716bc45e9aabf588c7b1fb667f5d59))
- NTOS:MM: snapshot PagesUsed in the consumer trim branch ([fix/uf22-balancer-trim-target-v2](https://github.com/nitroos-org/reactos/commit/7c2b8266c89e7232e51846874119d86810e615fa))
- NTOS:MM: Mark files with writable sections from a handle as user-mapped ([fix/uf23b-fatloss-evict-v2](https://github.com/nitroos-org/reactos/commit/b68629e9ff8e1b0e19cea74292c8ae42fe484d48))
- NTOS:MM: Do not wait for the balancer while holding AddressCreationLock [pick] ([main/mm-wait-address-lock](https://github.com/nitroos-org/reactos/commit/3e9b5634349286b1f99434efe37fbcff993d1bc9))
- ntoskrnl mm: Mm: retry a transient in-page read before bugchecking ([fix/mm-inpage-retry](https://github.com/nitroos-org/reactos/commit/a2aace7a77d1871a88c4eb10d706b29db7386e16))
- ntoskrnl mm: mm: balance segment reference on MiRosUnmapViewOfSection claim-loop early return ([fix/mm-section-segment-ref-leak](https://github.com/nitroos-org/reactos/commit/30c8a1035c11c23d4492ccc7bc9bebb8489227e4))

</details>

<details><summary>Kernel: Ob, Se, Ps, Ex and registry (38 fixes)</summary>

- CMLIB: fix a security-related bug ([fix/cmlib-amd64-recovery](https://github.com/nitroos-org/reactos/commit/f3c54e419c1251d97df9020f631c0b7abdbed64f))
- CMLIB: Mark every block of a cell dirty ([fix/cmlib-mark-cell-dirty](https://github.com/nitroos-org/reactos/commit/aa9b4df9e81a48029bbb469d3ad40adbc7f85dc8))
- NTOS:CM: fix a security-related bug ([fix/p2-ntoskrnl-misc](https://github.com/nitroos-org/reactos/commit/139849ec47082be3fe2ce9a0b61e3582879920c5))
- NTOS:CM/SDK:CMLIB: Report a failed hive flush ([fix/cm-flush-failure](https://github.com/nitroos-org/reactos/commit/9e9e08af954db07bba5b80e35fc35856f71e77ba))
- NTOS:EX: Restore the default timer resolution on the last release ([fix/ex-timer-resolution](https://github.com/nitroos-org/reactos/commit/0a194e77e4acf15e4d80ed5c99937838853d1510))
- NTOS:EX: fix a security-related bug ([fix/kernel-dst-transition](https://github.com/nitroos-org/reactos/commit/c2cb8b8769f57ae4e3e518db2ff829ea8a3752a5))
- NTOS:EX/NTOS:PS: Report the private commit charge in bytes ([fix/ntos-process-private-bytes](https://github.com/nitroos-org/reactos/commit/5ddcb3f4fc04e80b88203d39ab15dc1f0abee6ec))
- NTOS:FSRTL: Keep paged FILE_LOCK fields out of the range spin lock ([fix/fsrtl-filelock-internal-sync](https://github.com/nitroos-org/reactos/commit/a66dc6138fcde3898b3d77b37da6fb2ce7d60630))
- NTOS:FSRTL: Compare byte-range lock offsets as unsigned ([fix/fsrtl-filelock-signed-compare](https://github.com/nitroos-org/reactos/commit/c566feb82d45b8ea605804603d098c99a9615737))
- NTOS:FSRTL: Lock per-stream contexts by FCB header version, as Windows ([fix/fsrtl-teardown-ctx](https://github.com/nitroos-org/reactos/commit/f2a88140ef58d045c0b92efe837fef5b1051db14))
- NTOS:IO: Keep the end-of-file offset of append-only synchronous writes ([fix/append-write](https://github.com/nitroos-org/reactos/commit/678833d1e2cd9a26c1b099354419977cf393455e)) — fixes [CORE-18789](https://jira.reactos.org/browse/CORE-18789)
- NTOS:IO: Implement FileIoCompletionNotificationInformation ([fix/io-completion-notification-modes](https://github.com/nitroos-org/reactos/commit/8840b64162cbb0b927a94a5384f93eec1474fd51))
- NTOS:IO: Answer FileFsDeviceInformation without the file object lock ([fix/io-getfiletype-deadlock](https://github.com/nitroos-org/reactos/commit/26571b9c7009a3d3174682c897f4e250a1d7f317))
- NTOS:IO/NTOS:OB: Balance object lifetime in the cancel APC and unnamed handle count ([fix/p2-io-ob-lifetime](https://github.com/nitroos-org/reactos/commit/1813c6d6582c06cdc41461cf9e1ab74ae74f47db))
- NTOS:KD: Release the time refresh lock only when the time-slip work item got it ([fix/kd-timeslip-lock](https://github.com/nitroos-org/reactos/commit/cf7d7f60dcda27401aec55d20abaf42c48fe98e2))
- NTOS:KDBG: Keep the full stack limit across a debugger entry ([fix/kdbg-stack-limit](https://github.com/nitroos-org/reactos/commit/b3a6a9a7060cb2cca96a2b47670f1c659119400e))
- NTOS:LPC: fix a security-related bug ([fix/lpc-callback-validation](https://github.com/nitroos-org/reactos/commit/23a80a6f1048b62944fd93adb6c29a16a01227c7))
- NTOS:LPC: Free the reply message of an exiting thread ([fix/lpc-exit-thread](https://github.com/nitroos-org/reactos/commit/70c406400be146088d8d86dbc3c7766d59b7f013))
- NTOS:LPC: Match every reply-semaphore release with exactly one wait ([fix/lpc-reply-wait-window](https://github.com/nitroos-org/reactos/commit/fa6075cc9248e861523581dc3830a04c0e694e04))
- NTOS:MM: Flush the address of a trimmed working set entry ([fix/kernel-misc](https://github.com/nitroos-org/reactos/commit/42c2b80fdc279413e663583ae0da0e9cb7c23659))
- NTOS:OB: Initialise the handle returned by NtCreateDirectoryObject ([fix/ob-createdir-handle](https://github.com/nitroos-org/reactos/commit/308273b4c3ef1f759ca711b6484a084dc7a38ab9))
- NTOS:OB: Unlock the handle entry on a write reference type mismatch ([fix/ob-write-ref-unlock](https://github.com/nitroos-org/reactos/commit/d8edb96ddbbcf441ad67a894243e5924a43c3244))
- NTOS:PNP: Protect the user-mode PnP event queue with a lock ([fix/pnp-event-queue-lock](https://github.com/nitroos-org/reactos/commit/6b2cd8c63f14e3b38cb96a2c80840580e9b4abf5))
- NTOS:PO/KERNEL32: Implement power request objects ([fix/kernel32-power-requests](https://github.com/nitroos-org/reactos/commit/7b8f54832589c8d8b04d788452aab39b1e9fb700))
- NTOS:PS: Answer ThreadAmILastThread from the active thread count ([fix/exit-last-thread](https://github.com/nitroos-org/reactos/commit/15c4dc745c8ca6182ac4cae16f94b266cab1e43c))
- NTOS:PS: Grant PROCESS_QUERY_LIMITED_INFORMATION and accept it for the limited query classes ([fix/openprocess-limited-query-v2](https://github.com/nitroos-org/reactos/commit/b09cf3c11cbc6b250d4854068dc7084a96791630))
- NTOS:PS: Attach to the target process when writing another thread's impersonation TEB info ([fix/teb-impersonation-attach](https://github.com/nitroos-org/reactos/commit/ab8f9a2713f8753ceb792c13ee5816d4cca2cc38))
- NTOS:SE: fix a security-related bug ([fix/se-accesscheck-capture](https://github.com/nitroos-org/reactos/commit/6f5b0176e0fc03b040d38774ec9b26988e5d4ca4))
- NTOS:SE: Honour the auto-inheritance flags in SeAssignSecurityEx ([fix/se-auto-inherit](https://github.com/nitroos-org/reactos/commit/0b474e55aa48cb0b4a26756c4aca2c7522a591ed))
- NTOS:SE: Copy the restricting SIDs into a filtered token ([fix/se-restricted-sids-copy](https://github.com/nitroos-org/reactos/commit/fa3207cff016188f520adabe48a9e356296eafcd))
- NTOS:SE/NTOS:OB: fix a security-related bug ([fix/se-token-integrity-label](https://github.com/nitroos-org/reactos/commit/f6cde1a5ae669896581906f80884117b02e48f93))
- NTOS:SE/WOW64: fix a security-related bug ([fix/se-token-logon-sid](https://github.com/nitroos-org/reactos/commit/14b2bc93270c37c90a880637ffe1ad80306eae39))
- PS/CONSRV: fix a security-related bug ([fix/kernel-handle-list-inherit](https://github.com/nitroos-org/reactos/commit/57435902888c7551f885dd1af0103bb5396a2b54))
- SDK:CMLIB: Stop the dirty-block walks at the hive's last block ([fix/cmlib-dirty-walk-bound-v2](https://github.com/nitroos-org/reactos/commit/cbe683b1f081a3326bd3ff0f31eb84834b538bcb))
- SDK:CMLIB: Flush a hive's base block before writing its bins ([fix/cmlib-flush-base-block](https://github.com/nitroos-org/reactos/commit/cf19e20e3160e48198257906c8f6a923c8c47816))
- SDK:CMLIB: Refuse a hive with a cell of size 0 or past its bin ([fix/cmlib-zero-size-cell](https://github.com/nitroos-org/reactos/commit/8f93ed53ef28cade2f593ea029aea63879da3893))
- SDK:CMLIB/NTOS:CM: Serialize hive cell allocation and keep replaced block lists ([fix/cmlib-concurrent-hive-growth](https://github.com/nitroos-org/reactos/commit/b2b6bc5d6976cbec540b53d9b9b75bf3fc0ccab6))
- SDK:CMLIB/NTOS:CM: fix a security-related bug ([fix/cmlib-hive-log-rules](https://github.com/nitroos-org/reactos/commit/c6d3de9f435c65a3c7159da99403b3f09da0d646))

</details>

<details><summary>I/O manager and file-system libraries (2 fixes)</summary>

- HIDPARSER: Clear the description on failure; no report ID byte for absent reports ([fix/hidparse-description](https://github.com/nitroos-org/reactos/commit/1ba7d06b275deca11a9c23270a1726cf72e286b7))
- SPTILIB: Clear the output tail of the caller's IRP, not of the new one ([fix/sptilib-output-buffer-irp](https://github.com/nitroos-org/reactos/commit/e2ab8a90d8cfd81f8d58480e362b7e20d3a51bf4))

</details>

<details><summary>NTFS (ntfsng) (11 fixes)</summary>

- BOOTDATA: Register untfs.dll as the NTFS format provider ([fix/ntfs-format-provider](https://github.com/nitroos-org/reactos/commit/3031af1bd64329554c6206829ce30886c6d80123))
- NTFSLIB: Write the cluster bitmap in pieces of about 1 MB ([fix/ntfslib-bitmap-chunked-write](https://github.com/nitroos-org/reactos/commit/f5cdaa7286e4dfa031fdb4c98097cb6c3c88f484))
- NTFSNG: Pass PnP requests on to the storage device, dismount at its removal ([fix/dmshrink-pnp-dup](https://github.com/nitroos-org/reactos/commit/b5c321d32a30bbfe38f500a836679a7a54cea75e))
- NTFSNG: Round-2: fix release lock regression + harden diagnostic gating ([fix/fs-release-hygiene](https://github.com/nitroos-org/reactos/commit/5362f3fa6946938a90802f5d9927bfed19938a7d))
- NTFSNG: Read whole sectors when a device read is not sector-aligned ([fix/ntfsng-aligned-device-read](https://github.com/nitroos-org/reactos/commit/a2d853aae7801bdfe0f6e54c17ed40f09f4a066f))
- NTFSNG: fix a security-related bug ([fix/ntfsng-managevolume-privgate](https://github.com/nitroos-org/reactos/commit/678f85e01acc4d9e01cf484a47dde732aeeab7e5))
- NTFSNG: fix a security-related bug ([fix/ntfsng-relative-open-fcb](https://github.com/nitroos-org/reactos/commit/0a713d4fc1afb42f0938920158c1c621da9d3d8c))
- NTFSNG: Report FILE_PERSISTENT_ACLS later, reparse points on NTFS 3.x only ([fix/ntfsng-report-reparse-support](https://github.com/nitroos-org/reactos/commit/e9ccb06e9669556989d23741d47761270473f2cf))
- NTFSNG: fix a memory-safety bug ([fix/ntfsng-security-cluster](https://github.com/nitroos-org/reactos/commit/ed8d0f1c52373051775e91bc1181cd4a56b36096))
- NTFSNG: fix a security-related bug ([fix/ntfsng-volume-open-dacl-parity](https://github.com/nitroos-org/reactos/commit/41329bf981e67f2b987c7b16dfafc6eeebb82fa5))
- NTFSNG: read an immutable cluster size on the paging-file path ([fix/uf22-ntfsng-pagefile-divzero-v2](https://github.com/nitroos-org/reactos/commit/27cb71f7d881c49563c58f59cbf8d77b3550483f))

</details>

<details><summary>FAT (3 fixes)</summary>

- FASTFAT: Purge at shutdown also when a fat write failed ([fix/fastfat-fat-write-race](https://github.com/nitroos-org/reactos/commit/ebef9d5d6b43b5094cd5d343127952a08220cdf4))
- FASTFAT: Answer FileAttributeTagInformation ([fix/fs-allinfo-attrtag](https://github.com/nitroos-org/reactos/commit/207fd41338d8055ee95f3151fbe32073fea162fb))
- FORMAT: Stop at a wrong volume label and at the end of input ([fix/fat32-format](https://github.com/nitroos-org/reactos/commit/5e368205524a96f2acfbf8549e0668134f3625fe))

</details>

<details><summary>Btrfs and other file systems (7 fixes)</summary>

- BTRFS: Refuse a volume with an unfinished block group tree conversion ([fix/btrfs-block-group-tree](https://github.com/nitroos-org/reactos/commit/f51001308c2d645e1caf38712d99a70a39c984c5))
- BTRFS: Write the whole free space info item on 32-bit ([fix/btrfs-free-space-info-size](https://github.com/nitroos-org/reactos/commit/ff658f47d352117cb475eeec9cf5cb4e8a2d12f5))
- BTRFS: Sync to v1.10 ([fix/btrfs-sync-1.10-main](https://github.com/nitroos-org/reactos/commit/54e35c388a2617297197655bdbae650d6e0092f5))
- BTRFS: Small robustness fixes from the filesystem P2 sweep ([fix/p2-fs](https://github.com/nitroos-org/reactos/commit/63296a3096539a1ec636327d6ff24a740b0d07a0))
- NPFS: Fix the name, index number and short buffers of FileAllInformation ([fix/npfs-query-info](https://github.com/nitroos-org/reactos/commit/d8473eaba5fafe7f7986b6ab97efe3292fa8e4cd))
- NTFS: Accept NTFS volumes on GPT partitions ([fix/gpt-volumes](https://github.com/nitroos-org/reactos/commit/a378470c20f90576e3735219d5b62f6fa74d1c57))
- VFATLIB/CHKDSK: Do not report a mounted volume's normal state as damage ([fix/vfat-chkdsk-live-volume](https://github.com/nitroos-org/reactos/commit/9b6a1295c54d784d2af1b65ee9433395f6c74dc6))

</details>

<details><summary>win32k, GDI and USER (49 fixes)</summary>

- BOCHSMP: Map the frame buffer write-combined ([fix/framebuffer-write-combined](https://github.com/nitroos-org/reactos/commit/156732102a7b6a605684a6898ad2260234efc68d))
- FRAMEBUF: Never let the engine draw past the shadow ([fix/framebuf-shadow-surface-v2](https://github.com/nitroos-org/reactos/commit/748681206bbdf0ea6983e5e1fd124342a62f5a4b))
- GDI32: Size theme image margins and resize theme images as Windows does ([fix/gdi32-drawstream-margins](https://github.com/nitroos-org/reactos/commit/89b767567c0ff8c9ce19fd329e45f0f84b782a15))
- GDI32: Let GdiDrawStream draw a theme image another DC holds ([fix/gdi32-drawstream-shared-bitmap](https://github.com/nitroos-org/reactos/commit/189604cc1265688d919016140862c522de94451f))
- GDI32: Form the size of a PolyPatBlt batch in a wide type ([fix/gdi32-polypatblt-batch-size](https://github.com/nitroos-org/reactos/commit/8fbf70bde27f9f83afe450038f2d6025de1fde74))
- GDI32/WIN32K: GetCharacterPlacementW applies pair kerning on its own ([fix/uf29-extent-no-kerning-v3](https://github.com/nitroos-org/reactos/commit/2b4dac4d0de94d795e152144c97f1d26e6cd4f8f))
- LOCALSPL: State the limit of the SetJob priority rollback ([fix/p2-win32k-deadcode](https://github.com/nitroos-org/reactos/commit/22ed80445abd61f7424806265b5a1ce0935b6efc))
- NTUSER: Track a right press in the caption only on the frame's own caption ([fix/nc-rbutton-hittest](https://github.com/nitroos-org/reactos/commit/4ba6ba7595077021b0b3aeb66aa23f7947f3ac19))
- NTUSER: fix a security-related bug ([fix/ntuser-buildhwndlist-probe](https://github.com/nitroos-org/reactos/commit/c173bc99cd9bedc032e8415040bdd1fc4ebe6ca2))
- NTUSER: UpdateLayeredWindow changes geometry and content with no callback in between ([fix/ntuser-ulw-cross-thread](https://github.com/nitroos-org/reactos/commit/a99b05ff921782487c1237822bfd23a5e4b0a01f))
- NTUSER: PrintWindow: pin the render DC; keep the image's palette ([fix/ntuser-wmprint-printwindow](https://github.com/nitroos-org/reactos/commit/b39cfa0a0585c182e097a653467d9108adbb2236))
- NTUSER: Erase the frame of a frame change in place in screen coordinates ([fix/win32k-framechanged-screen-region](https://github.com/nitroos-org/reactos/commit/65fdcb4306f4ab6641b71bf3e03df3dffcd2a6bd)) — fixes [CORE-15934](https://jira.reactos.org/browse/CORE-15934)
- NTUSER/USER32_APITEST: Do not make a queue of another desktop the foreground queue ([fix/win32k-foreground-other-desktop](https://github.com/nitroos-org/reactos/commit/1ff6944f68ff918042d5d06dd800359d2a5dcabb))
- NTUSER/USER32_APITEST: Flash state, not WNDS_ACTIVEFRAME, keeps the flashed frame active ([fix/win32k-ncpaint-active-frame](https://github.com/nitroos-org/reactos/commit/4c6f350834763955417f6ce9d8fe3c8b99729546))
- NTUSER/USER32_APITEST: Drawing the caption and the frame leaves the client area alone ([fix/win32k-ncpaint-exclude-client](https://github.com/nitroos-org/reactos/commit/bf5d0fd9dce4ebb87b4c31177a9cbf6e0c888781))
- NTUSER/USER32_APITEST: Restore a minimized or maximized window from its screen rectangle ([fix/win32k-restore-ncframe-v3](https://github.com/nitroos-org/reactos/commit/cb1910d389a0169c107690778b5ca29dff3471ac))
- SYSMAIN: Firefox: make the Windows 7 SP1 version answer a default that explicit layers override ([fix/text-rendering-firefox](https://github.com/nitroos-org/reactos/commit/8277c8301d1f124c70a198b7a901b6f98efcee64))
- USER32: Load PNG compressed icon images from resources ([fix/user32-loadimage-png-icon](https://github.com/nitroos-org/reactos/commit/407806fa3899b9f9b7fc3f83caace44d04000c3f))
- USER32: Add SetCoalescableTimer, IsWindowArranged, RegisterSuspendResumeNotification and GetAutoRotationState, export the rest of the Windows 8 ones ([fix/user32-missing-exports](https://github.com/nitroos-org/reactos/commit/a8c2f482da03ca9526977743f8cc85848d141eee))
- USER32: The padded border of SystemParametersInfo and SystemParametersInfoForDpi follows the frame ([fix/user32-padded-border-metric](https://github.com/nitroos-org/reactos/commit/0bc52fb7a40e36c77a2c70c4248adfc776e97a8d))
- USER32: Keep the allocation checks of MsgiUnicodeToAnsiMessage ([fix/user32-wtoa-message-inline](https://github.com/nitroos-org/reactos/commit/964e32d499655cd2b9b7283938a0b4a96d00b1f6))
- USP10/USER32_APITEST: Draw fallback-font text in EDIT controls ([fix/edit-linked-glyphs](https://github.com/nitroos-org/reactos/commit/b18ffc61b543038558a8735c3f6ce041d2658fff))
- UXTHEME: fix a security-related bug ([fix/win32k-apihook-unload-once](https://github.com/nitroos-org/reactos/commit/5b4fc134ebb88ff525dd1dcccb68b3d35391b2b7))
- WIN32K: Keep wake bits truthful so MWMO_INPUTAVAILABLE waits can block ([fix/msgwait-wake-bits](https://github.com/nitroos-org/reactos/commit/9f999748cee6d78eb03d41e7d78629a1f9a02c19))
- WIN32K: Update the visible region of a DC that another process holds when a window moves ([fix/win32k-dce-foreign-visrgn](https://github.com/nitroos-org/reactos/commit/5acfab119bdd5109c81fc0007f611b8f0f8d4fbd))
- WIN32K: Serialize the monitor list of a graphics device ([fix/win32k-enumdisplaydevices-race](https://github.com/nitroos-org/reactos/commit/4074aef101b19cf57289ff12f210aa9ace3652ea))
- WIN32K: Pointer: show it after a switch also for a driver that hands it to the engine; convert only with a translation ([fix/win32k-pointer-flicker-v2](https://github.com/nitroos-org/reactos/commit/4a50965f493ed8f843ce897749374a1eee6899f9))
- WIN32K: Do not read the destination in StretchBlt when the ROP does not use it ([fix/win32k-stretchblt-dest-read](https://github.com/nitroos-org/reactos/commit/ffe18e86bbd70248c7f70084d3c83f23b7549a81))
- WIN32K/GDI32_APITEST: Add a Debug-only region allocation failure injection and tests ([fix/win32k-region-alloc-failure](https://github.com/nitroos-org/reactos/commit/0a806e7d568b87cfffbbcca0b1e37a2ef417d84b))
- WIN32K/GDI32_APITEST: fix a security-related bug ([fix/win32k-text-draw-lone-surrogate](https://github.com/nitroos-org/reactos/commit/3c80cc64a67806106a045898ffe9609f790958fa))
- WIN32K/GDI32_APITEST: Measure text extents from the glyph advance, not from a rendered bitmap ([fix/win32k-text-extent-no-raster](https://github.com/nitroos-org/reactos/commit/075d582c0b680524d9a3190372a610161963bb20))
- WIN32K/USER32: Post WM_MOUSEHWHEEL for horizontal wheel input ([fix/win32k-mousehwheel-v2](https://github.com/nitroos-org/reactos/commit/9307a750d9a2c90d23496501ed896c2b2971d6b9))
- WIN32K:GDI: fix a security-related bug ([fix/dwrite-fontface-from-hdc-private](https://github.com/nitroos-org/reactos/commit/13d9ac63bf9bbd8526f9518ca36cdca6a893b6ea))
- WIN32K:NTGDI: Keep the bitmap of a DC marked as selected across RestoreDC ([fix/gdi-restoredc-bitmap-owner](https://github.com/nitroos-org/reactos/commit/d1f31e82de197c7619325ed638588a5a6087602e))
- WIN32K:NTGDI: Free a font when its resource is removed ([fix/win32k-font-creator-ref-removal](https://github.com/nitroos-org/reactos/commit/fdfe0d4c8a458af4a2261ce1efe5d7e6deed4219))
- WIN32K:NTUSER: Keep the full pointer-sized menu item data ([fix/ntuser-menu-itemdata-64bit](https://github.com/nitroos-org/reactos/commit/038224c1aac4f40874db3193737f7824f6f13a68))
- WIN32K:NTUSER: Do not truncate the module handle of a WinEvent hook on x64 ([fix/ntuser-winevent-hook-module-x64](https://github.com/nitroos-org/reactos/commit/7d8438b08b15e5878018c694d465ccc8a0707cf4))
- WIN32K:NTUSER: Judge hung threads as Windows does ([fix/win32k-hung-window-v3](https://github.com/nitroos-org/reactos/commit/7c3c5f60801f73b9909db27414651a90b37ff67f))
- WIN32K:NTUSER: Zero the KBDLLHOOKSTRUCT before it is filled ([fix/win32k-kbdhook-zero-fill](https://github.com/nitroos-org/reactos/commit/6bd9d8242a8a99ed24d0f8897121ebc31a7b9219))
- WIN32K:NTUSER: Implement SPI_GETWHEELSCROLLCHARS and SPI_SETWHEELSCROLLCHARS ([fix/win32k-wheel-scroll-chars](https://github.com/nitroos-org/reactos/commit/1e51c969ad44e733c335feea6ef3a0cd9b95120b)) — fixes [CORE-18390](https://jira.reactos.org/browse/CORE-18390)
- WIN32SS:DIB: Do not step a negative-height StretchBlt flip through the source stride ([fix/win32k-stretchblt-negative-height-v2](https://github.com/nitroos-org/reactos/commit/5cdbec32ed6d86bc5c798fbd1ed6fea9a4ef591c))
- WIN32SS:NTGDI: Release the SharedFace creator reference for private fonts ([fix/win32k-font-sharedface-leak-v2](https://github.com/nitroos-org/reactos/commit/e8af6e5b83438ab486b985151c5b896e131f034e))
- WIN32SS:NTUSER: Store the mouse sonar and vanish settings in UserPreferencesMask ([fix/ntuser-mouse-vanish-sonar](https://github.com/nitroos-org/reactos/commit/1b8f087eb2c9242a3aaae649c9dc13c2e3606434))
- WIN32SS:NTUSER: Free a withdrawn sent message instead of parking it for the receiver ([fix/uf19-attach-queue-v2](https://github.com/nitroos-org/reactos/commit/822498ef95c84613c52b5b861fc65bddc489a865))
- WIN32SS:NTUSER: Fail the process callout when the process is terminating ([fix/win32k-process-terminating](https://github.com/nitroos-org/reactos/commit/9ad27b19d5de58129c2acf58cdea8b62ea155810))
- WINED3D: Release the adapter when OpenGL adapter initialisation fails ([fix/wined3d-gl-adapter-leak](https://github.com/nitroos-org/reactos/commit/f92138dc6b8def54d70ad54d6e24defbe0ffb477))
- WINED3D/DDRAW: Do not show the no 3D notice for DirectDraw ([fix/ddraw-no3d-sw-gl](https://github.com/nitroos-org/reactos/commit/79d7677c61177b27a6804e83e857590d3524f5d5))
- WINED3D/DDRAW: Fail quietly without OpenGL 2.0, as Windows does without a 3D driver ([fix/wined3d-quiet-no-3d](https://github.com/nitroos-org/reactos/commit/90dc6656728b7ec9d05fe02af742853e9d5d9c7e))
- WINHTTP/QUARTZ: Do not call a function inside HRESULT_FROM_WIN32 ([fix/winerror-hresult-from-win32-single-eval](https://github.com/nitroos-org/reactos/commit/86231bed87f9721f5a8b9dc9f760eccf90350c56))

</details>

<details><summary>Console and csrss (7 fixes)</summary>

- CONSRV: Bring the cursor into the view after output that only moved it ([fix/consrv-view-follow-cr](https://github.com/nitroos-org/reactos/commit/fa882940169ec0faa2f0cb3d3a29b3950fe4862c))
- CONSRV: Limit the console window repaints while output streams ([fix/consrv-view-follow-repaint-limit](https://github.com/nitroos-org/reactos/commit/9e1bce9eb5980a44c303a80df12d516b6c758fbb))
- CONSRV: fix a security-related bug ([fix/consrv-write-stall-v2](https://github.com/nitroos-org/reactos/commit/ee428ea768d77588e9031faf50b4a82cf6d48f7b))
- CONSRV/KERNEL32: csrss survives a client crash in console control-event delivery [pick] ([main/csrss-survive-alone](https://github.com/nitroos-org/reactos/commit/6d7a4195dbb36b8aa2fb5652ddf22ad07fc08370))
- CSRSRV: fix a memory-safety bug ([fix/csrsrv-thread-refcount](https://github.com/nitroos-org/reactos/commit/748b6642ff52444d0f8e2a8523ed859e0f9319ad))
- WINLOGON: Keep EWX_FORCE for the logoff that starts a shutdown ([fix/shutdown-hang-with-force](https://github.com/nitroos-org/reactos/commit/f38398670646b8633c00bab1122a6f94ceeb030a))
- WINSRV: Accept hard errors that come without a CSR thread ([fix/winsrv-harderror-null-thread](https://github.com/nitroos-org/reactos/commit/a4290dcbf54bba7259eb70e68b788542ea04e295))

</details>

<details><summary>WoW64 (36 fixes)</summary>

- CMAKE: Keep WoW64 table generation out of default native builds ([fix/wow64-native-i386-stubs](https://github.com/nitroos-org/reactos/commit/427fc062520d7e3b17ff659b3069c161e2e53d29))
- KERNEL32: Accept the DEP mitigation attribute from a WOW64 guest on amd64 ([fix/kernel32-wow64-dep-attribute](https://github.com/nitroos-org/reactos/commit/7656641d8873930456bf62bf09a7dceb394a5cb7))
- MSI: Treat a failed window enumeration like an incomplete list ([fix/msi-wow64-ca-honest](https://github.com/nitroos-org/reactos/commit/30b77862d886031f32fa6c5a93011c462c8a9da3))
- NTDLL: Initialize the native TLS list in WoW64 processes ([fix/wow64-tc32-tlslist](https://github.com/nitroos-org/reactos/commit/c8ac8e964823995f0372e99d61140a4ed1045552))
- NTDLL/WOW64: Keep the guest APC routine out of the callee's home space ([fix/wow64-ff32-apc-home-space](https://github.com/nitroos-org/reactos/commit/029c1d6e96cac503d72d1e0f0ad09364afd70b83))
- NTDLL/WOW64: Give the guest system call stubs the NT 5.2 x64 WOW64 layout ([fix/wow64-guest-stub-layout](https://github.com/nitroos-org/reactos/commit/0e7370f326e999ed65f7dd791462344ad87de2d6))
- NTOS:KE: Load the WoW64 FS base of a thread attached to another process ([fix/wow64-tc32-cmteb](https://github.com/nitroos-org/reactos/commit/6a317eb5731f82f6c6f6f873245359b3637cb378))
- NTOS:PS: Declare the WoW64 flag of PspSetProcessTlsInformation only where it is used ([fix/wow64-ff32-dynamic-tls](https://github.com/nitroos-org/reactos/commit/5f549a72d6f88fb0f25ff3e42df4f5f12a47a86d))
- NTUSER: Keep the client area when the frame changes in place ([fix/wow64-tc32-lister-framechanged](https://github.com/nitroos-org/reactos/commit/e124449b5127881897433df4335e711f0408a26d))
- RTL: Report the 32-bit modules of a WOW64 process in the debug buffer ([fix/tasklist-wow64-modules](https://github.com/nitroos-org/reactos/commit/483f4066939838fd03aa888fc74d12d1a82c959e))
- SYSWOW64: Install dwrite.dll and user32_vista.dll for 32-bit programs ([fix/wow64-ff32-syswow64-deps](https://github.com/nitroos-org/reactos/commit/a97cf79b13568e0c6d2a6db4897b3331170a2c9f))
- USER32: Align the MsgWaitForMultipleObjectsEx wait structure ([fix/wow64-idle-msgloop-spin](https://github.com/nitroos-org/reactos/commit/34fc0328e8efc8fc43c4af3d36c316e9d87a9f04))
- WOW64: Translate console screen buffer, code page, language and shutdown CSR calls ([fix/wow64-console-csr-batch](https://github.com/nitroos-org/reactos/commit/ae268e8250677a0c6e6bd80d04be4918dce1d763))
- WOW64: Check and copy the guest attribute values before creating the process ([fix/wow64-ff32-attribute-list](https://github.com/nitroos-org/reactos/commit/556dfced79703616d04199baef8f59413c1290e0))
- WOW64: Grow the guest stack before any local of the exception dispatch is written ([fix/wow64-ff32-exception-stack-limit](https://github.com/nitroos-org/reactos/commit/5d6a5dd48e6cfae42e620e2884c82ac119b42712))
- WOW64: Give the first guest thread a growable stack with a guard page ([fix/wow64-first-thread-stack-guard](https://github.com/nitroos-org/reactos/commit/ed5408f5a26ecb9b3e9e2ed33243ca2075ae324c))
- WOW64: Read the job information arguments once ([fix/wow64-job-information](https://github.com/nitroos-org/reactos/commit/38b135e2ab8bba9bc60fa22250077980ccfdbd74))
- WOW64: Translate NtPowerInformation for the fixed-size records ([fix/wow64-power-information](https://github.com/nitroos-org/reactos/commit/c4e9fc60a4a44c30a806618d3174e19e7dfb418c))
- WOW64: Translate NtQueryInformationProcess image file name classes ([fix/wow64-process-image-name](https://github.com/nitroos-org/reactos/commit/1c8ab233ecce06819e25febdbced51c7845178e5))
- WOW64: Translate more NtQueryInformationProcess classes ([fix/wow64-query-information-process](https://github.com/nitroos-org/reactos/commit/fef0409cfb8d6a30cd13881b9a411044a282cc6c))
- WOW64: Translate NtQueryVirtualMemory(MemorySectionName) ([fix/wow64-query-virtual-memory](https://github.com/nitroos-org/reactos/commit/7eb62166fbfe8187e13cf6cb58ea42dd4893802d))
- WOW64: Translate NtRaiseHardError ([fix/wow64-tc32-crashreport](https://github.com/nitroos-org/reactos/commit/c9c523c631b56034f622ec27f36c93805d64503e))
- WOW64: fix a security-related bug ([fix/wow64-token-dacl-label](https://github.com/nitroos-org/reactos/commit/ac2fecbf6d6654c59df726552d8d7f4e5da8c1a6))
- WOW64: Name the untranslated request in the unsupported diagnostics ([fix/wow64-unsupported-diag](https://github.com/nitroos-org/reactos/commit/52134ff90e129f02148c4b8f10ccb8ccec9b960e))
- WOW64/USER32: Review follow-up for the idle message loop test ([fix/wow64-idle-loop-followups](https://github.com/nitroos-org/reactos/commit/c57e3f7cbae890d97d1a29106ae6a354d3fc97ec))
- WOW64WIN: Keep the transition guard set on a popped callback frame ([fix/wow64-callback-return-intransition](https://github.com/nitroos-org/reactos/commit/1cc03ad63d92ef02ce15d6234fc67c0c1cd6b3e4))
- WOW64WIN: Guard checked-build-only services instead of dropping them ([fix/wow64-p3-l1-gen-dbg](https://github.com/nitroos-org/reactos/commit/133d8804260790900b2d9d63b33d8c98b4a2a6a5))
- WOW64WIN: fix a security-related bug ([fix/wow64-p3-l2-ime-probe](https://github.com/nitroos-org/reactos/commit/610e5567882d0e33c528e1db8b2dcfc9c53ee87a))
- WOW64WIN: Never forward an unreadable guest structure in its 32-bit layout ([fix/wow64-p3-l3-display](https://github.com/nitroos-org/reactos/commit/23aeabdaa0e5f340b691be9acab65f0bf711455c))
- WOW64WIN: Snapshot guest GDI structures and fix EnumObjects copy-back ([fix/wow64-p3-l4-gdi-extra](https://github.com/nitroos-org/reactos/commit/044bc060ca0bce2d902c1ef31430f4658abcc4f4))
- WOW64WIN: Copy back every mouse point the kernel fills ([fix/wow64-p3-l5-mousemove](https://github.com/nitroos-org/reactos/commit/c0bd55ad421aa59a1f49d20022e596de66293c9c))
- WOW64WIN: fix a security-related bug ([fix/wow64-p3-l6-rawinput](https://github.com/nitroos-org/reactos/commit/55bb168a2160a3003166dbd39867efa92562c65a))
- WOW64WIN: Refuse native HID preparsed data for 32-bit callers ([fix/wow64-p3-l7-misc-preparsed](https://github.com/nitroos-org/reactos/commit/b0b34895a40a1dc26c665e2610345a55d245d49b))
- WOW64WIN: Translate WM_MEASUREITEM and WM_DRAWITEM ([fix/wow64-tc32-ownerdraw](https://github.com/nitroos-org/reactos/commit/45d2e03ecf23a12b6cb75a638d77ce7fb80ee394))
- wow64win: wow64win: list the 39 bucket services in the generator's manual set [pick] ([main/wow64win-bucket-manual-set](https://github.com/nitroos-org/reactos/commit/9502f19be6c2b8b45e1947fb856f47ef2347d060))
- wow64win: wow64win: single generator regen after s1-s5 bucket merges; drop three reconciled duplicate thunks [pick] ([main/wow64win-stage1c-regen](https://github.com/nitroos-org/reactos/commit/1f53c219d25ef0bc67ed893e668c2aaf1ad19674))

</details>

<details><summary>Networking (57 fixes)</summary>

- AFD: Wait for in-flight transport requests before freeing a closed socket ([fix/afd-close-inflight](https://github.com/nitroos-org/reactos/commit/5466bb0f712709c4df4d09ec499dd912cf0d51f7))
- AFD: Complete a cancelled connect only after its transport connect, close the old endpoint on listen ([fix/afd-connect-cancel](https://github.com/nitroos-org/reactos/commit/bc2ba8b5023356cde36e6c108d0748017f8aa022))
- AFD: Issue the first listen from a work item and retry failed listens ([fix/afd-listen](https://github.com/nitroos-org/reactos/commit/b49924191a5557a904dbec84a0de6296b75d006c))
- AFD: Honour the listen backlog ([fix/afd-listen-backlog](https://github.com/nitroos-org/reactos/commit/a8f46b6d34404d7f5d78a930824fc78f7f6862b1))
- AFD: Release the buffers of a cancelled datagram receive ([fix/afd-recvdatagram-cancel-cleanup](https://github.com/nitroos-org/reactos/commit/6c16f86142e989c7f1d746a49e5327229ee81d6a))
- AFD: Scatter received datagrams over all buffers of the request ([fix/afd-recvfrom-multibuf](https://github.com/nitroos-org/reactos/commit/a52f46350bd0062d3cbbae0d96e57926ef946b24))
- AFD: fix a memory-safety bug ([fix/afd-security-core](https://github.com/nitroos-org/reactos/commit/eb2fbb213354746b55f9ba797d7550cf27eda473))
- AFD: Complete a select request whose poll could not be allocated ([fix/afd-select-alloc-failure](https://github.com/nitroos-org/reactos/commit/99d435d6ae181e65453cc738ba263c33f9b9d0c6))
- AFD: Check select requests and reference only sockets ([fix/afd-select-handles](https://github.com/nitroos-org/reactos/commit/bb23ac1a8fc3d338d28ba64ede20d933c6e2ad5e))
- AFD: Allocate the receive work item before opening the address ([fix/afd-udp-recv-rearm](https://github.com/nitroos-org/reactos/commit/fd9d19a90d7bc0986a8a706d790a2bc781925905))
- AFD/MSAFD: Put the AcceptEx initial data at the start of the buffer ([fix/afd-acceptex-initial-data-m8b1](https://github.com/nitroos-org/reactos/commit/892b4a8838dc9c255876177b77957f0debf8aeb8))
- AFD/MSAFD: Fail a second ConnectEx instead of freeing the buffer the first one uses ([fix/afd-connectex-overlap](https://github.com/nitroos-org/reactos/commit/21aa363d94e8545008cd6782b8f8e7226ce1d433))
- E1000: Reclaim every finished transmit descriptor ([fix/e1000-tx-reclaim-all](https://github.com/nitroos-org/reactos/commit/af06b043205de5ccfc9c5ad977419a152aba3904))
- IPHLPAPI: GetAdaptersAddresses: fill the interface luid and report the Windows 7 structure size ([fix/iphlpapi-adapters-luid-bestinterface](https://github.com/nitroos-org/reactos/commit/f19ca93b6da89a28a9c28f76a7069dd66ef11d11))
- IPHLPAPI: Implement GetBestRoute2 for IPv4 ([fix/iphlpapi-getbestroute2](https://github.com/nitroos-org/reactos/commit/9b7647ff5bc0ad55e28bbd6837ecc0be560bbcd1))
- IPHLPAPI: The Vista IP table getters fail instead of reporting success with no table ([fix/iphlpapi-ip-table-not-supported](https://github.com/nitroos-org/reactos/commit/1e2402a2b71015e4bf6b4e6f7fa7c2fdd8ed4aa3))
- IPHLPAPI: Add NotifyIpInterfaceChange and CancelMibChangeNotify2 ([fix/iphlpapi-ipinterface-change](https://github.com/nitroos-org/reactos/commit/d59e978ea5685811565942feb8d84b99d5e89bd3))
- IPHLPAPI: Add NotifyRouteChange2 and NotifyUnicastIpAddressChange ([fix/iphlpapi-notify-route-change2](https://github.com/nitroos-org/reactos/commit/2dc47e66875e763adbdeabf77b9fdc77b2c5002d))
- IPHLPAPI/IPHLPAPI_APITEST: List every TCP and UDP socket in the table functions ([fix/iphlpapi-tcpudp-all-rows](https://github.com/nitroos-org/reactos/commit/79bc675ad5b2e3295ca581d8724393bc5f67a7fd))
- MSAFD: Return INVALID_SOCKET from every WSPAccept failure exit ([fix/msafd-accept-invalid-socket](https://github.com/nitroos-org/reactos/commit/138bb32622cfd59f2ed210559dac63c53f08e939))
- MSAFD: Time out a receive only where it can be cancelled alone ([fix/msafd-socket-timeouts](https://github.com/nitroos-org/reactos/commit/73ca54872d03355f6f43e1a40d28e048ec4575a3))
- MSAFD: Fail rejected SIO_UDP_CONNRESET requests also when overlapped ([fix/net-udp-connreset-v2](https://github.com/nitroos-org/reactos/commit/bede2eac13fc886f4a9084c3a54623354e92cfd1))
- MSAFD: Leave the byte count of a failed send unchanged ([fix/tcpip-abortive-close](https://github.com/nitroos-org/reactos/commit/2641a1980a3f7153d059345253bfcc9cc078c0ae))
- MSAFD/WS2_32_APITEST: Let SO_UPDATE_CONNECT_CONTEXT run after getpeername ([fix/msafd-getpeername-connect](https://github.com/nitroos-org/reactos/commit/210e02b15f9c024692fec7996e725fa1566f927a))
- NDIS: Do not send queued packets from the interrupt DPC ([fix/ndis-dpc-send-budget](https://github.com/nitroos-org/reactos/commit/dc35710e39a62b1e84559c5e3945f801502ff9ff))
- NDIS: Pass the miniport's own context to MiniportHalt ([fix/ndis-halt-context](https://github.com/nitroos-org/reactos/commit/1686b1f7044eafc889512ecda69fafd60fcd2acb))
- NDIS: fix a security-related bug ([fix/p2-net](https://github.com/nitroos-org/reactos/commit/6576554d9f48d48619ab8b3287a3f3704fe6e146))
- NET: NET SHARE: accept printer and serial ports and the other paths Windows takes ([fix/net-share-device-path](https://github.com/nitroos-org/reactos/commit/3dc79c9398072e6e38de3ed82f1502c93e611f9c))
- NET: net start lists the started services and reports results like Windows ([fix/net-start-list](https://github.com/nitroos-org/reactos/commit/662f5a001535dd8a331bdfcddc7d44d4bf91b78e))
- NET: Implement NET VIEW and rewrite NET SHARE to match Windows ([fix/net-view-share](https://github.com/nitroos-org/reactos/commit/be71022d1f9459615d71b48d728fe0e66869d4ef))
- NETAPI32: Add NetGetAadJoinInformation and NetFreeAadJoinInformation ([fix/netapi32-missing-exports](https://github.com/nitroos-org/reactos/commit/8be6c1874c060109867562f1d38291c7cb03884e))
- NETCFGX: Fix the indentation of a trace in ExtractParameterValue ([fix/netcfgx-gcc13-indent](https://github.com/nitroos-org/reactos/commit/3f9d46eccc526aef1f177fb6169a247218ab27c0))
- RPCRT4: Let the reading thread cancel its own pipe read on server stop ([fix/rpcrt4-np-close-read](https://github.com/nitroos-org/reactos/commit/7cd8533b469dad676db9768852ad4d38c1992c63))
- RPCSS: Create and signal Global\ScmCreatedEvent ([fix/rpcss-scm-created-event](https://github.com/nitroos-org/reactos/commit/262f964815abdba01d7294a5b2b3cc787e241072))
- schannel: fix a security-related bug ([fix/schannel-cert-check-ordering](https://github.com/nitroos-org/reactos/commit/af2e84dfcd003864ca77d6a0d0f832c9d8634da8))
- SCHANNEL: fix a security-related bug ([fix/schannel-server-cert-validation](https://github.com/nitroos-org/reactos/commit/5f4ab40f29a24218f06ab1265c1d114c43737023))
- SRVSVC: NetrServerGetInfo: allocate the server name on its own ([fix/net-localgroup-server-info-v2](https://github.com/nitroos-org/reactos/commit/3f76980ef57f2d691829c8fd47c2d8330ce64760))
- TCPIP: Do not revive a closing address file in AddrFindShared ([fix/p2-tcpip-memsafety](https://github.com/nitroos-org/reactos/commit/dd52ec799acec04d05a816712a0c6467c47abbe1))
- TCPIP: Do not revive a dying address file in the receive search ([fix/tcpip-addrfile-ref](https://github.com/nitroos-org/reactos/commit/28babc677f4cc89034508d3fe2789d0f47e313d2))
- TCPIP: Do not touch a request the cancel routine does not own ([fix/tcpip-cancel-request](https://github.com/nitroos-org/reactos/commit/bf2b7021a565c38311837d5df4708c6e6354079f))
- TCPIP: Report exhausted ephemeral TCP ports as WSAENOBUFS ([fix/tcpip-ephemeral-port](https://github.com/nitroos-org/reactos/commit/53312e67db49bc57f3c5e12a2a44c8aaf5c405ab))
- TCPIP: fix a security-related bug ([fix/tcpip-icmp-options](https://github.com/nitroos-org/reactos/commit/57f4d2849c173853199fb8242f006b0f94f71027))
- TCPIP: Take only the ICMP replies that answer the echo request ([fix/tcpip-icmp-reply-match](https://github.com/nitroos-org/reactos/commit/6269b14ad2925bec9522f7a41ff44be2c561699a))
- TCPIP: fix a memory-safety bug ([fix/tcpip-ipv4-reasm-bounds](https://github.com/nitroos-org/reactos/commit/c5b6d432b5f00fb4915cf2d32bef2dec6bdc8759))
- TCPIP: fix a security-related bug ([fix/tcpip-ipv4-reasm-total](https://github.com/nitroos-org/reactos/commit/10ef9a6a9707f86ef3776da715a2d88b9d535d15))
- TCPIP: Keep the unspecified address of an address file across a connect ([fix/tcpip-listener-after-cancel](https://github.com/nitroos-org/reactos/commit/d01a34546ae7f8a69b559c94bcf9e2085d8df947))
- TCPIP: Coalesce media status changes in one worker per adapter ([fix/tcpip-media-status-order](https://github.com/nitroos-org/reactos/commit/9c7bd667dc069db2762fe6b277d9f700e0e27fb7))
- TCPIP: Return TCP ports in network order from the lwIP glue ([fix/tcpip-peer-port-order](https://github.com/nitroos-org/reactos/commit/c9e5bed199737326a0a1ce9bb62c4e2ecc81b381))
- TCPIP: Publish an interface's three TDI entities together ([fix/tcpip-socket-scale](https://github.com/nitroos-org/reactos/commit/44338a8e1cada51df6c9fcf57eaacb35359793e4))
- TCPIP: Let a SYN reopen a connection in TIME-WAIT ([fix/tcpip-timewait-reopen](https://github.com/nitroos-org/reactos/commit/3ef0e081c0bcd1fe56880cb38c9c44edf23704f0))
- TCPIP/AFD: Discard unread data when the peer resets the connection ([fix/tcpip-abortive-close-data](https://github.com/nitroos-org/reactos/commit/f1468a9a209c7a86013149ccbbedc966f8ab41d6))
- WININET: Fail a stalled read after the receive timeout, and report a damaged gzip body like Windows ([fix/wininet-gzip-stall-v2](https://github.com/nitroos-org/reactos/commit/7d45027d8a9f1570f8bd219acde7f72e555f9a60))
- WS2_32: gethostname: fail instead of returning an untouched or unterminated name ([fix/ws2_32-gethostname-errors](https://github.com/nitroos-org/reactos/commit/bec84ed077129dedd83f4c67aebdb862e6cf1740))
- WS2_32: Add GetHostNameW and GetAddrInfoExCancel ([fix/ws2_32-missing-exports](https://github.com/nitroos-org/reactos/commit/7350867149eddb98956033bc44eaaa07846e59d0))
- WS2_32: Size the query-set buffer with the alignment the copy uses ([fix/ws2_32-queryset-align](https://github.com/nitroos-org/reactos/commit/9453b74cc229d27cb75167ca12fe33195180942c))
- WS2_32: Retry WSAPoll only for a confirmed stale socket, and without waiting ([fix/ws2_32-wsapoll](https://github.com/nitroos-org/reactos/commit/57a1c6a72e5dd75b3e4ad6064a6dff5d155552e6))
- WS2_32: WSASendMsg: reserve the completion context before sending, leave the event alone when a routine is given ([fix/ws2_32-wsasendmsg](https://github.com/nitroos-org/reactos/commit/46ccaf4e64896c91ea89e92095c464a6fbf0fd24))

</details>

<details><summary>Audio and multimedia (6 fixes)</summary>

- MMSYS: Keep the volume tray icon when OK changes nothing ([fix/mmsys-tray-icon-unchanged](https://github.com/nitroos-org/reactos/commit/ac57e989fb963aeae9f51407f94ac2f003251223))
- SNDREC32: Do not hang on exit when the recording device failed to open ([fix/sndrec32-close-hang](https://github.com/nitroos-org/reactos/commit/abdd107b6613a3cfe213901db55ed50a75114804))
- SNDREC32: Say once that there is no recording device ([fix/sndrec32-no-input-device-v3](https://github.com/nitroos-org/reactos/commit/4db60c2901a2b03d982db1f733279a55c9ee7fb3))
- WINMM: Load the multimedia drivers on first use, not in DllMain ([fix/winmm-lazy-drivers](https://github.com/nitroos-org/reactos/commit/f10b55a7ff07e55c2dcaacd83b69ebbb09ee42b8))
- WINMM: Hold the system timer resolution while timeBeginPeriod is active ([fix/winmm-timebeginperiod-resolution](https://github.com/nitroos-org/reactos/commit/6693aa6d63a6e2a0a34073e5408c1e47309db0bf))
- WINMM: Read the QPC frequency once in timeGetTime ([fix/winmm-timegettime-div0](https://github.com/nitroos-org/reactos/commit/f2301e5030a1f60126d2a391b6d8511255fda11b))

</details>

<details><summary>Storage, USB, PnP and drivers (6 fixes)</summary>

- ATAPI: Flush the write cache after a FUA write on disks without NCQ ([fix/atapi-fua-flush](https://github.com/nitroos-org/reactos/commit/8360dcbfdfb5a8eee876be05988a0d4326570e3f))
- hid: fix a security-related bug ([fix/hidparser-descriptor-bounds-r2](https://github.com/nitroos-org/reactos/commit/237085375d07b0d1cf45238899e6a82fd5a0575c))
- MOUHID/NTUSER: Fix absolute pointer scaling and the move to (0, 0) ([fix/abs-pointer-mapping](https://github.com/nitroos-org/reactos/commit/188ca9fa0bfcc9988f164e9508758f0966253cb9))
- MOUNTMGR: Link one stored volume name, and keep offline and saved links right ([fix/mountmgr-stored-links](https://github.com/nitroos-org/reactos/commit/1c7dd9992ef828296054e96dbfca5338b1c49416))
- PCI: Make BAR-sizing failure handling safe against the two-pass query ([fix/pci-bar-sizing-decode-off](https://github.com/nitroos-org/reactos/commit/91f3e15da57697c293e1a24d7a3f8e70c3e35980))
- USBUHCI: Poll interrupt endpoints at their period ([fix/usbuhci-drag-interrupt-poll](https://github.com/nitroos-org/reactos/commit/e02645c587f2c15894e44e58911c9a39f99eeaf5))

</details>

<details><summary>Shell, explorer and common controls (50 fixes)</summary>

- BROWSEUI: Keep the last results and the final status of a finished search ([fix/shellfind-search-complete](https://github.com/nitroos-org/reactos/commit/4b089240d68d289edb6c003eb078aa093c7ab2ae))
- BROWSEUI/EXPLORER: Keep the taskbar toolbars across logons ([fix/taskbar-toolbars-persist-v3](https://github.com/nitroos-org/reactos/commit/32d75e0450ce5d6e90b68a1cdc4a9bc61e6ced04))
- BROWSEUI/FONTEXT: Search: list the items of the Fonts folder ([fix/shellfind-font-folder-results-v2](https://github.com/nitroos-org/reactos/commit/c56e3d5307a30e75516230022181244aa92c9468)) — fixes [CORE-16444](https://jira.reactos.org/browse/CORE-16444)
- BROWSEUI/SHELL32: Ask for a disk when opening an empty drive ([fix/explorer-insert-disk](https://github.com/nitroos-org/reactos/commit/cbe26fad60ee04a40f3881ac105a56a7b72c6383))
- CMD: Keep the console cursor size at the prompt ([fix/cmd-cursor-size](https://github.com/nitroos-org/reactos/commit/29ecd7336e27ab511a44e7db162d0ea448c7a73b))
- CMD: Add the ftype internal command ([fix/cmd-ftype](https://github.com/nitroos-org/reactos/commit/01737a9fe0c2596838dbc2443416b0a7fd644dec))
- CMD: Start the external programs of a pipeline directly ([fix/cmd-pipe-direct-exec](https://github.com/nitroos-org/reactos/commit/9876d73dcef72b890ae424af9395f8a4f60393b8))
- CMD: Write redirected output in the console code page by default ([fix/cmd-redirect-ansi](https://github.com/nitroos-org/reactos/commit/25a3be3914f44d06b7225f2e21cdac91bb22e5a6)) — fixes [CORE-14795](https://jira.reactos.org/browse/CORE-14795)
- CMD: Run only files with a PATHEXT extension for a name without one ([fix/cmd-search-extensionless](https://github.com/nitroos-org/reactos/commit/a2cf87a2385c45912696d67519e596a4174dca56)) — fixes [CORE-12604](https://jira.reactos.org/browse/CORE-12604)
- CMD: start: give the program its name as typed ([fix/cmd-start-command-line](https://github.com/nitroos-org/reactos/commit/34efc0ffe44ab8d3e79ce4730433613d36226fe0))
- COMCTL32: Implement ImageList_WriteEx ([fix/comctl32-missing-exports-v2](https://github.com/nitroos-org/reactos/commit/f264240c6f5daa5b83af68d62709bcd47fc447e5))
- COMCTL32: Keep the wizard caption on a page that has no title ([fix/comctl32-propsheet-title](https://github.com/nitroos-org/reactos/commit/d99b70541d359a58f062d3fac4840812991db9c6)) — fixes [CORE-20687](https://jira.reactos.org/browse/CORE-20687)
- COMCTL32: Export TaskDialog and TaskDialogIndirect, number the version 6 icon exports like Windows ([fix/comctl32-v6-ordinals](https://github.com/nitroos-org/reactos/commit/1c5a339c22147105eff7af161e14549f8ed1d938))
- COMCTL32: Send a full NMITEMACTIVATE for Enter in the list-view ([fix/listview-enter-itemactivate](https://github.com/nitroos-org/reactos/commit/07fdcc5f1a9b76e72936eab9ed1e5c438021afd1))
- COMCTL32: Toolbar: forward the notifications of its child controls to the parent ([fix/toolbar-forward-child-notify](https://github.com/nitroos-org/reactos/commit/424776c69fca2c96b33248738e0de4acc0eb3b77))
- COMDLG32: fix a memory-safety bug ([fix/comdlg32-canonical-path-overflow](https://github.com/nitroos-org/reactos/commit/9fb0c558b8b2a09f7083a5660573972032ffdd6d))
- COMDLG32: Add the default extension only to a name that has none ([fix/comdlg32-defext-keep-extension](https://github.com/nitroos-org/reactos/commit/a14b21ad5a83a3be5ef1e0b441a10b1ebb6343a2)) — fixes [CORE-20756](https://jira.reactos.org/browse/CORE-20756)
- COMDLG32: Fall back to the personal folder as documented ([fix/comdlg32-save-dialog-folder](https://github.com/nitroos-org/reactos/commit/78de99821d70c7bd25887ab176c36efcec67d14f))
- EXPLORER: Make "Show Quick Launch" show the Quick Launch toolbar ([fix/explorer-show-quick-launch](https://github.com/nitroos-org/reactos/commit/96117a2f8ff0b6ccfc18269339cbc75a3f22782a))
- EXPLORER: Never wait for another process's window on the shell thread ([fix/shell-no-blocking-sends](https://github.com/nitroos-org/reactos/commit/d81ea6eb3f95b2f6600bca20d06ea246bab68a0a))
- EXPLORER: Notice a full-screen window that moves or resizes and keeps its handle ([fix/shell-tray-topmost-fullscreen](https://github.com/nitroos-org/reactos/commit/2f00c35e48e26beb6ee237603e059f7b5c240313))
- IEFRAME: Save an Internet shortcut whose icon properties are still held open ([fix/ieframe-urlsave-open-properties](https://github.com/nitroos-org/reactos/commit/cdd934436dee6004aa97110932e20b026ea55f6a))
- KERNEL32: Fail foreign-machine images on 64-bit without a hard error ([fix/kernel32-amd64-foreign-image](https://github.com/nitroos-org/reactos/commit/dbdf7c33a0ac0a766bfdc4ff9d95e1160466329f))
- NETSHELL: Report that Repair is not supported from the folder menu ([fix/netshell-status-repair](https://github.com/nitroos-org/reactos/commit/0a72aefb849cef7d6285b8a16d4a7ce7eb944b35))
- NETSHELL/APPHELP: fix a security-related bug ([fix/p2-dlls-shell-print](https://github.com/nitroos-org/reactos/commit/430df20cb91478e490f7a9f057b7d3ef8e44627c))
- NTOBJSHEX: Show a Properties page for NT objects ([fix/ntobjshex-properties-v2](https://github.com/nitroos-org/reactos/commit/311117ce937b7257722c836c45b58fc92f640d6d))
- PROPSYS: Implement VariantCompare ([fix/propsys-variantcompare-v2](https://github.com/nitroos-org/reactos/commit/21d06586e5b66fd81fb92d7f27d52ee8ffbed027))
- SHELL32: Report a failed rename in a folder view ([fix/defview-rename-error-box](https://github.com/nitroos-org/reactos/commit/6e90e9541ff0abbcfc13150ffb2fdc76b108c79c))
- SHELL32: Count the size of items of other folders in the status bar ([fix/defview-statusbar-item-size](https://github.com/nitroos-org/reactos/commit/3920df73de1ded54bb9440a930d4d2c061299802))
- SHELL32: Let the Properties of several files change their attributes ([fix/filedefext-multi-attributes](https://github.com/nitroos-org/reactos/commit/b9d7f580d05dce1bffc0e1ccac6e62fdc1e6cd37))
- SHELL32: Implement IApplicationAssociationRegistration ([fix/shell32-app-assoc-registration](https://github.com/nitroos-org/reactos/commit/bea734aeaab150ce307138feaaea91dfa10ff4c6))
- SHELL32: Show no error box when the user cancels a copy ([fix/shell32-copy-cancel-silent](https://github.com/nitroos-org/reactos/commit/b5b1098edf879f0cffe4b8eb4f28185b7acea289))
- SHELL32: Do not set an unchanged drive label on OK ([fix/shell32-drive-label-unchanged-m](https://github.com/nitroos-org/reactos/commit/600f4fe60bf9a91057f7dd270100f807ab947c8d))
- SHELL32: Report the result of the drive Tools buttons ([fix/shell32-drive-tools-honest](https://github.com/nitroos-org/reactos/commit/869e1175f5026bd9c46de8b780ef42bc30919bee))
- SHELL32: Pass the ShellExecuteEx directory to the context menu handler ([fix/shell32-exec-context-menu-directory](https://github.com/nitroos-org/reactos/commit/8d089d7697b4e47b9b0937297caf9c76e75397f4)) — fixes [CORE-19855](https://jira.reactos.org/browse/CORE-19855)
- SHELL32: Fonts folder shortcut: bind the font shell extension ([fix/shell32-fonts-folder-fontext](https://github.com/nitroos-org/reactos/commit/524f53e771c54432145d5057100651b7bfd0e2a2))
- SHELL32: Expand a default-user known folder value that starts with another variable ([fix/shell32-known-folder-defs](https://github.com/nitroos-org/reactos/commit/4f392b70c80e21a9bce7fcc18d84d70b4d1b4937))
- SHELL32: Implement SHGetKnownFolderPath and related functions ([fix/shell32-known-folder-path](https://github.com/nitroos-org/reactos/commit/3dcc19baa955b0e42c674e34a6982586e1019b2d))
- SHELL32: Make Yes the default of the Empty Recycle Bin question ([fix/shell32-recycle-confirm-default](https://github.com/nitroos-org/reactos/commit/bb582f86133556ef9e57fc6f9907eaddb11439d8))
- SHELL32: Honor CMIC_MASK_NOASYNC for Recycle Bin item properties ([fix/shell32-recycle-item-props](https://github.com/nitroos-org/reactos/commit/0e4bacb8262d56113e935a9b4743e877af07537c))
- SHELL32: Keep the icon and save the comment of a shortcut ([fix/shell32-shortcut-comment-icon](https://github.com/nitroos-org/reactos/commit/976378b860678d57e67456d2d7838a3e5f7d809f))
- SHELL32: Implement SHGetStockIconInfo and stop the NT 6 stubs from killing callers ([fix/shell32-vista-stubs](https://github.com/nitroos-org/reactos/commit/f64c9661ff0aa852bca62bf244ff53a40dd2e53e))
- SHELL32: Add SHGetPropertyStoreForWindow ([fix/shell32-window-property-store](https://github.com/nitroos-org/reactos/commit/1e64c43806f3836e56d6b3d803a04d609abdebc9))
- SHELL32: Do not set folder attributes when reading them failed ([fix/shellpath-attr-read-check](https://github.com/nitroos-org/reactos/commit/abe63f45ba6f98b8f360bbf0054bebbaf655147f))
- SHELL32/USRMGR: Run as on program shortcuts; enable Reset Password ([fix/shortcut-runas-reset-password-v2](https://github.com/nitroos-org/reactos/commit/4dc803a91336157d4947a697746c9d34b9baeb7c))
- URLMON: fix a memory-safety bug ([fix/browser-heap-corruption](https://github.com/nitroos-org/reactos/commit/78dc648bad2eab7eb3d5d8d3373f3382d6c24f7f))
- USER32: Keep icon identifiers apart from the handle array on 64-bit ([fix/pickicon-sparse-grid](https://github.com/nitroos-org/reactos/commit/1b7a175ec8d46c549293eecc254c3eb76f33568c))
- UXTHEME: Free the window contexts on uahStop, not on uahShutdown ([fix/uxtheme-defer-hook-shutdown-cleanup](https://github.com/nitroos-org/reactos/commit/81255dca8fd43023c7812736c3b343e5888b3fad))
- UXTHEME: Add BeginPanningFeedback, UpdatePanningFeedback and EndPanningFeedback ([fix/uxtheme-panning-feedback](https://github.com/nitroos-org/reactos/commit/dec7e1677609bbc0bce395124732ffe46a062e91))
- UXTHEME: Number the named functions as Windows 10 does ([fix/uxtheme-win10-ordinals](https://github.com/nitroos-org/reactos/commit/f192df9f4f2e2d7afc6cc2a84e1dd96e9d0a971f))

</details>

<details><summary>Setup and boot (24 fixes)</summary>

- BOOTDATA: Default "Show Control Panel in My Computer" to checked ([fix/folderopts-cpl-default](https://github.com/nitroos-org/reactos/commit/54d788b35991637e1a763ad99e544dacfc1c3f0c))
- BOOTDATA/KERNEL32_APITEST: Drop internal project names from comments ([fix/remove-internal-names](https://github.com/nitroos-org/reactos/commit/fddaeeeb42574801bf9e034c9af72f127c3281ec))
- DEVMGR: Report a failure to enable or disable a device ([fix/devmgr-disable-honest](https://github.com/nitroos-org/reactos/commit/6c08280a20a363c2597fb20736fc49f2748c3f46))
- DEVMGR: Give the focus to OK when the problem button loses it ([fix/devmgr-troubleshoot-button](https://github.com/nitroos-org/reactos/commit/29d0fb267d0a851b838b3e75595cb5daad26403e))
- FREELDR: Apply a hive log only to the interrupted write it belongs to ([fix/freeldr-hive-log-rules](https://github.com/nitroos-org/reactos/commit/4cd2c3bee221a4ceb2984f6f092bab6ee57d4954))
- FREELDR: Read hive log dirty blocks from after the dirty vector ([fix/freeldr-log-offset](https://github.com/nitroos-org/reactos/commit/c6cc7434638a2507f66558d8fa331ffd05e95151))
- FREELDR: Check the NTFS attribute list context allocation ([fix/freeldr-ntfs-attribute-extents](https://github.com/nitroos-org/reactos/commit/ddf5d247a53391230bff61ece6fc573a9b858855))
- FREELDR: fix a memory-safety bug ([fix/freeldr-stall-calibration](https://github.com/nitroos-org/reactos/commit/a8f56d53e7c3ee0f4ba259070c04e4c9528e1ec1))
- HDWWIZ: Fail the search when a device that has a name does not answer ([fix/hdwwiz-search-honest-v3](https://github.com/nitroos-org/reactos/commit/369699aa19c15874ab1bb1184dba12085fc1952e))
- RAPPS: Say once that a 32-bit installer cannot run on 64-bit without WoW64 ([fix/rapps-amd64-32bit-installer](https://github.com/nitroos-org/reactos/commit/966166d99131653b77d241d95aaeafc7664e2ed6))
- RAPPS: Skip packages whose dependency did not install ([fix/rapps-dependency-failure](https://github.com/nitroos-org/reactos/commit/c3e6f1c79b3c0edf645fc38c5030a9596d64b897))
- RAPPS: Retry a busy server and report the HTTP status ([fix/rapps-download-retry](https://github.com/nitroos-org/reactos/commit/e3d89a394ede8e12ae51215415ec4ea97ab1b947))
- RAPPS: Do not call SHGetKnownFolderPath on NT5 ([fix/rapps-generate-nt5](https://github.com/nitroos-org/reactos/commit/856b15254e6ffc56c2bffaf3168c74585b74cc15))
- RAPPS: Count a cancelled installer as not completed ([fix/rapps-installer-exit-code](https://github.com/nitroos-org/reactos/commit/9a03473d8ee0385bf898a1998a0391c4cb227295))
- RAPPS: Ask before running a download that is a console program ([fix/rapps-standalone-program](https://github.com/nitroos-org/reactos/commit/1019fddfaa14cf122748573554c234ac298bae32))
- REACTOS/BOOTDATA: Default to NTFS in the graphical installer too ([fix/guisetup-ntfs-default](https://github.com/nitroos-org/reactos/commit/d23861e424f5e045b260803aa93f834dc41c209e))
- SETUPAPI: fix a security-related bug ([fix/setupapi-device-property-v2](https://github.com/nitroos-org/reactos/commit/9135a160b628c31ea6f007363579e978c7cdc738))
- SETUPAPI: Implement the disk space list drive and INF section functions ([fix/setupapi-diskspace-sections](https://github.com/nitroos-org/reactos/commit/9645971598c8eaae1ff54bd8624c12c9054c9515))
- SETUPAPI: Implement SetupAddToDiskSpaceList and SetupQuerySpaceRequiredOnDrive ([fix/setupapi-diskspace-sync](https://github.com/nitroos-org/reactos/commit/c522fad70bc26b130de19eddd5b22a0c26d9dec1))
- SETUPAPI: Ask for a restart when a running device gets its driver again ([fix/setupapi-reinstall-running-device](https://github.com/nitroos-org/reactos/commit/d057ed442fdf782345fe7be7bc40e1109fc8b2bb))
- SETUPAPI: Do not ask for a restart after the first install of a device that keeps its drivers ([fix/setupapi-started-first-install](https://github.com/nitroos-org/reactos/commit/b944a87d8faa7516fb6f6cba5fa03e5acb3ef393))
- SYSDM: Show the small dump for a CrashDumpEnabled value the dialog does not know ([fix/sysdm-crashdump-values](https://github.com/nitroos-org/reactos/commit/be277623784ec99ef8c35aff3a1d3ed106ca4fa9))
- SYSDM: Grey out the Error Reporting button ([fix/sysdm-error-reporting-disabled](https://github.com/nitroos-org/reactos/commit/70573d89f357da63f9d49bbddf8e3f627c48f915))
- SYSDM: Implement the processor scheduling radios of Performance Options ([fix/sysdm-processor-scheduling](https://github.com/nitroos-org/reactos/commit/c52abed52647e4aefb97c70a1ee8f92e91515fd4))

</details>

<details><summary>Printing (6 fixes)</summary>

- COMDLG32/NOTEPAD: Report a printer device context that cannot be created ([fix/print-dc-failure-honest](https://github.com/nitroos-org/reactos/commit/ce0e343058ed60392f955cb17ceef74063603fe8))
- LOCALSPL: Schedule a job only through the handle that added it ([fix/localspl-addjob-other-handle-v2](https://github.com/nitroos-org/reactos/commit/cd0aa56fc4eec29df6233f50c76aeddeec13d08b))
- LOCALSPL: Keep every job at its own sorted place in the printer queue ([fix/localspl-delete-printer](https://github.com/nitroos-org/reactos/commit/ed076b39229e3c9638326f78a5040673c1c626cf))
- LOCALSPL/WINSPOOL: Print a document into the output file given to StartDocPrinter ([fix/localspl-print-to-file](https://github.com/nitroos-org/reactos/commit/77168743aa3164d52c73300174089da4b2a6de9a))
- SHELL32: Add a context menu for printers in the Printers folder ([fix/printers-folder-verbs](https://github.com/nitroos-org/reactos/commit/cedadb919efc5d7b6d698da601c882529d54de4f))
- WINSPOOL: AbortPrinter: keep a spooled document whose abort failed ([fix/printers-abortprinter-v3](https://github.com/nitroos-org/reactos/commit/a2a7b18ca2364c7b108791cb1bd7c47fb632bf19))

</details>

<details><summary>Services, security and profiles (22 fixes)</summary>

- ADVAPI32: Implement CloseTrace ([fix/advapi32-closetrace](https://github.com/nitroos-org/reactos/commit/ec804b17f9953a92b766836a1855a07ec01c11c4))
- ADVAPI32: Parse the S-R-I-S string of ConvertStringSidToSidW strictly ([fix/advapi32-convert-string-sid](https://github.com/nitroos-org/reactos/commit/9bb9005936e7f4f25b4672c3eeaf7e4200f30a2c)) — fixes [CORE-8717](https://jira.reactos.org/browse/CORE-8717)
- ADVAPI32: Fail EncryptFile instead of reporting success without encrypting ([fix/advapi32-encryptfile-honest](https://github.com/nitroos-org/reactos/commit/28eb46379e5730660776a315a809efe5ae710af8))
- ADVAPI32: Return the key class from RegEnumKeyExA ([fix/advapi32-enumkey-class](https://github.com/nitroos-org/reactos/commit/6e9774c0a4af6d4a70753af57e6f3eb2c7e7a495))
- ADVAPI32: fix a security-related bug ([fix/advapi32-equaldomainsid](https://github.com/nitroos-org/reactos/commit/4c04275525dc1845043fa1a235112e16a07d1541))
- ADVAPI32: fix a security-related bug ([fix/advapi32-regenumvalue-terminator](https://github.com/nitroos-org/reactos/commit/baa909277716f4d7ed941cb96dcb1633aed04b1c))
- ADVAPI32: fix a security-related bug ([fix/advapi32-reggetvalue-expand](https://github.com/nitroos-org/reactos/commit/e07c5b358855d4b886a839d9e729bf556d3573c3))
- ADVAPI32/ADVAPI32_APITEST: Export the event provider functions ([fix/advapi32-event-provider](https://github.com/nitroos-org/reactos/commit/e7fcfc3800b191c95b9539f8ce3232ef5b796dcf))
- ADVAPI32/ADVAPI32_APITEST: fix a security-related bug ([fix/sddl-rights-numeric](https://github.com/nitroos-org/reactos/commit/cb3ced5fdc24a7c14a1f079d839be480f7494862))
- LSASRV: Unlink the deregistered logon context, not its successor ([fix/lsa-deregister](https://github.com/nitroos-org/reactos/commit/f3e1aa617e65578f0a416483a27a2e620a08f2da))
- NTDLL/ADVAPI32: Add EtwEventSetInformation and EnableTraceEx2 ([fix/advapi32-missing-exports](https://github.com/nitroos-org/reactos/commit/c9a266ba16dfb6f6484b54167cf5346324ad0bd6))
- NTMARTA/ADVAPI32_APITEST: Honour ACE inheritance flags and inherit from the parent on unprotected DACLs ([fix/ntmarta-inherit-acl](https://github.com/nitroos-org/reactos/commit/9e0655f183f9bfce772c89a73f9890f247dee133))
- NTMARTA/ADVAPI32_APITEST: fix a security-related bug ([fix/ntmarta-setentries-oldacl](https://github.com/nitroos-org/reactos/commit/3c877f3af0c9b7755d82bd000f01339f4d55f9c9))
- SERVICES: Store the FailureActions value in the layout Windows uses ([fix/hivesys-recovery-defaults](https://github.com/nitroos-org/reactos/commit/fb22433006f43f240e98c5baf6e630153fc85bf4))
- SERVICES: Start every own-process service in a process of its own ([fix/scm-own-process-image-v2](https://github.com/nitroos-org/reactos/commit/e6a8c02b33259d860353d2e9ebfb283527f3f0c7))
- TELNETD: Clear the stop flag of a run after the shutdown flag ([fix/telnetd-stop](https://github.com/nitroos-org/reactos/commit/45c1061970877993e385a3a538f596e6909e261c))
- UMPNPMGR: Initialize the notification lock before starting threads ([fix/umpnpmgr-init-order](https://github.com/nitroos-org/reactos/commit/5f3430ad626cf47dd20493fdc909cba8ac6476ed))
- UMPNPMGR: Report no dock station when there is no docking information ([fix/umpnpmgr-no-dock-info](https://github.com/nitroos-org/reactos/commit/55314903197aa04deab83dcbf84280f6105d4471))
- USERENV: Remove directory trees through handles ([fix/userenv-deleteprofile-check](https://github.com/nitroos-org/reactos/commit/bc71ac61a515ae307d411b64925a3eceb7c9e577))
- USERENV: Create the legacy profile junctions on NTFS ([fix/userenv-legacy-profile-junctions](https://github.com/nitroos-org/reactos/commit/ad207d52732f16d534a5c9734d930f745e564082))
- USERENV: Add DeriveAppContainerSidFromAppContainerName ([fix/userenv-missing-exports](https://github.com/nitroos-org/reactos/commit/1fc4df69931e47827d8ba735b40092dbef271676))
- USERENV/SHELL32: Take upstream's Vista profile layout series as it is ([fix/upstream-users-profile-layout](https://github.com/nitroos-org/reactos/commit/2c5c6e27bae09219a62dca251e9fb88b8786f80c))

</details>

<details><summary>User-mode system DLLs (67 fixes)</summary>

- CMAKE: Regenerate sdk/cmake/baseaddress.cmake: the i386 image of kernel32 no longer fits below ntdll ([fix/kernel32-volume-mountpoint-r1015](https://github.com/nitroos-org/reactos/commit/fe236f8237f13f2867886b629ff3c166e7ffe60b))
- CRT: Do not give DLLs a TLS directory they do not need ([fix/dll-no-crt-tls-directory](https://github.com/nitroos-org/reactos/commit/d8804816b3a429774d1bc5a633a709703a85a1e8))
- KERNEL32: EnumUILanguages: list the installed UI languages only ([fix/enumuilanguages-installed](https://github.com/nitroos-org/reactos/commit/2f8e0a9bff28ac8c4e004bb94448f3a914b8461d))
- KERNEL32: Always release gcsAppCert after the application certification setup ([fix/kernel32-appcert-lock-race](https://github.com/nitroos-org/reactos/commit/ebfec408b94dd24d8f01873bf8419927424fa93e))
- KERNEL32: Application recovery registration; restart settings as measured on Windows ([fix/kernel32-application-recovery](https://github.com/nitroos-org/reactos/commit/9e9b4f561da6132d4ae2a283d8f6a538595e4cb7))
- KERNEL32: Implement application restart registration ([fix/kernel32-application-restart](https://github.com/nitroos-org/reactos/commit/632cc8034617b0b37c1363e672e32208f73efb34)) — fixes [CORE-16061](https://jira.reactos.org/browse/CORE-16061)
- KERNEL32: Implement CreateEventExA/W and CreateMutexExA/W ([fix/kernel32-create-event-mutex-ex](https://github.com/nitroos-org/reactos/commit/5c957f560d794d11c7f4f4f25486cb256afb808f))
- KERNEL32: fix a security-related bug ([fix/kernel32-createfile-sqos](https://github.com/nitroos-org/reactos/commit/a2fadb3ff7902345aee8990e422b29594da1582d))
- KERNEL32: Implement CreateRemoteThreadEx ([fix/kernel32-createremotethreadex](https://github.com/nitroos-org/reactos/commit/bbf2938ad7ce70d6fefbba21b22fde7ad4353db4))
- KERNEL32: Implement QueryIdleProcessorCycleTime, QueryProcessCycleTime and QueryThreadCycleTime ([fix/kernel32-cycle-time](https://github.com/nitroos-org/reactos/commit/6e58a31aa1bc23c471a993d05b5b58752d5c05bb))
- KERNEL32: GetDynamicTimeZoneInformation: empty key names, result of GetTimeZoneInformation ([fix/kernel32-dynamic-time-zone](https://github.com/nitroos-org/reactos/commit/30577d877dc7f5f2805a62e7e0551d48f5727e60))
- KERNEL32: Honour FILE_FLAG_FIRST_PIPE_INSTANCE in CreateNamedPipeW ([fix/kernel32-first-pipe-instance](https://github.com/nitroos-org/reactos/commit/6687a834ee527baca7a2e976ea7a121018ae4237))
- KERNEL32: Read an I64 FormatMessage argument as 64 bits on 32-bit systems ([fix/kernel32-formatmessage-int64](https://github.com/nitroos-org/reactos/commit/a96335568d79b05b0ee8277326c64ab540f7d2ac)) — fixes [CORE-17317](https://jira.reactos.org/browse/CORE-17317)
- KERNEL32: Report the binary type of an image as Windows does ([fix/kernel32-getbinarytype](https://github.com/nitroos-org/reactos/commit/071d816c830882ed1ce3974554b145aab0b7881f))
- KERNEL32: Implement GetLogicalProcessorInformationEx ([fix/kernel32-logical-processor-ex](https://github.com/nitroos-org/reactos/commit/6b368cd4a50bc155918509e7d24b1101db5026c1))
- KERNEL32: Accept the mitigation policy process attribute ([fix/kernel32-mitigation-policy-attribute](https://github.com/nitroos-org/reactos/commit/a6012db2b2193e7db45041c3c83f232bf2eb6775))
- KERNEL32: Refuse PROC_THREAD_ATTRIBUTE_HANDLE_LIST instead of emulating it ([fix/kernel32-proc-thread-attribute-list](https://github.com/nitroos-org/reactos/commit/5856f2c746d1bd91135f82656f404852ff257115))
- KERNEL32: Sync profile.c with Wine 11.19 and map INI files through IniFileMapping ([fix/kernel32-profile-sync](https://github.com/nitroos-org/reactos/commit/9b644b68fd6cd4cde766f50f9efd43dd874e9690)) — fixes [CORE-16273](https://jira.reactos.org/browse/CORE-16273)
- KERNEL32: Implement ResolveLocaleName ([fix/kernel32-resolvelocalename](https://github.com/nitroos-org/reactos/commit/3e7e6a20743232576e687809b2db4148cdf89215))
- KERNEL32: Implement SetEnvironmentStringsW ([fix/kernel32-setenvironmentstrings](https://github.com/nitroos-org/reactos/commit/c1d8b0170fff8c93d25c772aa6c5b7624e38c399))
- KERNEL32: Implement SetFileInformationByHandle ([fix/kernel32-setfileinformationbyhandle](https://github.com/nitroos-org/reactos/commit/5476c605eebefed0b657b0e421a516187590856c))
- KERNEL32: Implement SetThreadErrorMode and GetThreadErrorMode ([fix/kernel32-thread-error-mode](https://github.com/nitroos-org/reactos/commit/476b3569f4db7c053fa3320d0a24aba12ed0a5b4))
- KERNEL32: GetTimeZoneInformationForYear: do not read a year value of more than 44 bytes ([fix/kernel32-timezone-for-year](https://github.com/nitroos-org/reactos/commit/4bc508d10a607ef311acf3fd734ded619be54c79))
- KERNEL32: Export the token functions that Windows 7 exports ([fix/kernel32-token-forwarders](https://github.com/nitroos-org/reactos/commit/4b0645fc9fd881560309dffab3447b7b1dfdc249))
- KERNEL32: Add WerRegisterRuntimeExceptionModule and WerUnregisterRuntimeExceptionModule ([fix/kernel32-wer-runtime-exception-module](https://github.com/nitroos-org/reactos/commit/3919a3b6cda59c8f0ca4bb2b12eefee5772d0bd8))
- KERNEL32: Add GetMaximumProcessorGroupCount, GetProcessGroupAffinity and SetWaitableTimerEx ([fix/kernel32-win7-thread-processor](https://github.com/nitroos-org/reactos/commit/af41ff507ad67fe3544d3e595814360f26b818ca))
- KERNEL32: Take the parent UI languages from the locale data ([fix/preferred-ui-languages](https://github.com/nitroos-org/reactos/commit/29dfa9100e15c09dd51c95cb6388b41418f7be00))
- KERNEL32: Report fail fast exceptions like unhandled ones ([fix/raisefailfast-report](https://github.com/nitroos-org/reactos/commit/b4ad2a8faf5ee0bc961fd43529e68a5e062a48ff))
- KERNEL32/KERNEL32_APITEST: Do not zero the tail of WIN32_FIND_DATA in FindFirstFile/FindNextFile ([fix/findfirstfile-keep-tail](https://github.com/nitroos-org/reactos/commit/89940c947af3d1880a6feb8153c80050488394c4))
- KERNEL32/KERNEL32_APITEST: Share the process UI languages with kernel32_vista, test the full language list and processors above 31 ([fix/kernel32-missing-exports](https://github.com/nitroos-org/reactos/commit/3a8e80a936801f7275423d13978679d9d2a2d968))
- KERNEL32/KERNELBASE: Export WaitOnAddress and WakeByAddress* from kernelbase only ([fix/waitonaddress-kernelbase-only](https://github.com/nitroos-org/reactos/commit/d8c1e90568699adcc0b3ea29f8fa3b418430952a))
- KERNEL32_VISTA: Implement GetFileMUIInfo ([fix/kernel32-getfilemuiinfo](https://github.com/nitroos-org/reactos/commit/3df03b4f16e589eaa58acfd66d3d60334ec4d28c))
- KERNEL32_VISTA: Require a file handle for the absorbed priority hint class ([fix/p2-usermode-kernel32](https://github.com/nitroos-org/reactos/commit/ed4147866471311babb9cb2cbdf20a0f455ffdf1))
- KERNELBASE: fix a security-related bug ([fix/kernelbase-interrupt-time](https://github.com/nitroos-org/reactos/commit/b9a5f86893da916be39012b6cf1b2794324fb85b))
- KERNELBASE: PathCanonicalizeW: only treat a whole ".." segment as a parent reference ([fix/shlwapi-path-dotdot-prefix](https://github.com/nitroos-org/reactos/commit/2264a530e0fc1ef84956ed1c9f945407adbb4670)) — fixes [CORE-20129](https://jira.reactos.org/browse/CORE-20129)
- NTDLL: Dynamic TLS: guarded header reads, a failed thread frees its stack ([fix/ldr-dynamic-tls](https://github.com/nitroos-org/reactos/commit/b539ffb4b75aa1e1b23835233aa34f0ea5bcf7f5))
- NTDLL: Unlink the FLS data of an exiting thread before freeing it ([fix/ldr-fls-before-tls-v3](https://github.com/nitroos-org/reactos/commit/54178f6d7d2902ecc990ac584143b473f238b12a))
- NTDLL: Honor the loadFrom path of a manifest file element in DLL redirection ([fix/ntdll-actctx-loadfrom](https://github.com/nitroos-org/reactos/commit/53e2c8a0ee496debe89db83ff5ae52e794d8a368)) — fixes [CORE-19707](https://jira.reactos.org/browse/CORE-19707)
- NTDLL: RtlQueryPerformanceFrequency always writes its output; relax the time test ([fix/qpc-unbiased-interrupt-time](https://github.com/nitroos-org/reactos/commit/766e6db02f95c8661c5b94a542b122499ca84c75))
- NTDLL: Heap walk: the ranges of a segment must be in ascending order and inside it, so a damaged list ends the walk ([fix/rtl-walk-heap](https://github.com/nitroos-org/reactos/commit/c56c49b05883800046399da8057b9edb82e03fff))
- NTDLL/ADVAPI32: fix a security-related bug ([fix/advapi32-add-mandatory-ace](https://github.com/nitroos-org/reactos/commit/929225ff09bbdb998800c4bcc62fece7213b5781))
- NTDLL/ADVAPI32: Implement the manifest-based event provider functions ([fix/etw-event-provider](https://github.com/nitroos-org/reactos/commit/f547ebc518fd00ac4e8b4e3043d16a4c44665fd1))
- NTDLL/KERNEL32: Restore the fiber checks inverted by the NT5-isms change ([fix/msvcrt-thread-detach-free](https://github.com/nitroos-org/reactos/commit/d71feff634d5d6d592adbf11b4d8e65b11e4ad35))
- NTDLL/KERNEL32: List the heaps of a process for the Toolhelp heap functions ([fix/toolhelp-heaps](https://github.com/nitroos-org/reactos/commit/cbb0effeb2e31732187fa26ca7625da71453ff14))
- NTDLL/NTDLL_APITEST: Report only the API set contracts the loader resolves, keep the shared UTF-8 decoder as it was ([fix/ntdll-missing-exports](https://github.com/nitroos-org/reactos/commit/d715d0f379c47e0ef31a8a3f3a4fc9f77dec7430))
- PSEH: Do not let a call before an x64 GCC try block return into it ([fix/x64-gcc-pseh](https://github.com/nitroos-org/reactos/commit/31d8fba88a49b0f102b188531158e7805cab3877))
- RTL: Do not lose condition variable wake-ups ([fix/rtl-condvar-lost-wakeups](https://github.com/nitroos-org/reactos/commit/dac3a7287a64f24773f96dcdb0ab8b428760d50f))
- RTL: Claim static critical section debug entries atomically ([fix/rtl-critsect-debuginfo-race](https://github.com/nitroos-org/reactos/commit/a3b056366437e59a14bf85254ee1d235ff9475b7))
- RTL: Copy environment variable values whole or not at all in RtlExpandEnvironmentStrings ([fix/rtl-expandenv-small-buffer](https://github.com/nitroos-org/reactos/commit/e0450ae0550c6b84e4db375648db42b5ad3612ae))
- RTL: Fix the cached walk in RtlGetElementGenericTable ([fix/rtl-generictable-walk](https://github.com/nitroos-org/reactos/commit/8463844c8667f974dac8d4eb7b925d672617a291))
- RTL: Do not commit past the end of a handle table reservation ([fix/rtl-handle-table-end](https://github.com/nitroos-org/reactos/commit/cb7ca8ea6316025b274a0a133f52610686c11dc2))
- RTL: Fill the whole tail pattern on 64-bit ([fix/rtl-heap-tail-pattern](https://github.com/nitroos-org/reactos/commit/0acb926f2639d71d737e14170eeb41e8131f856e))
- RTL: Answer RtlIsRangeAvailable for a window with Start above End ([fix/rtl-range-available-reversed](https://github.com/nitroos-org/reactos/commit/dd6742567c840bed31952921d4f0991425fa3fb7))
- RTL: fix a security-related bug ([fix/rtl-remote-modules-bound](https://github.com/nitroos-org/reactos/commit/6b332ccfa6a48f8633dde1dd56a0ea0f8a5d89b9))
- RTL: Block SRW lock waiters on a keyed event instead of sleeping in ticks ([fix/rtl-srw-backoff-convoy](https://github.com/nitroos-org/reactos/commit/3f120eca95f9fb81ed88837adde147f5e8fa99c1))
- RTL: Let SRW lock waiters give the processor away ([fix/rtl-srw-wait-backoff](https://github.com/nitroos-org/reactos/commit/2c0fc8da3ab60d83aaa142d5d05e7cd55f079fbe))
- RTL: Take the SRW wait block lock only on a lock word that has wait blocks ([fix/rtl-srw-wait-block-lifetime](https://github.com/nitroos-org/reactos/commit/45258a10b8666c8cf97e9a6b82fb1640819f6b9f))
- RTL: Do not start a wait callback after the wait was cancelled ([fix/rtl-wait-deregister](https://github.com/nitroos-org/reactos/commit/423a581b9a2a4095bfc45f6ea251c7800e6d57c0))
- RTL: Fix the shared count encoding in RtlTryAcquireSRWLockShared ([fix/srw-try-shared](https://github.com/nitroos-org/reactos/commit/52979b8c3b7c8f85d0d87793247693bc89cfbde3))
- RTL/NTDLL: Implement RtlAddGrowableFunctionTable, RtlGrowFunctionTable and RtlDeleteGrowableFunctionTable ([fix/ntdll-growable-function-tables](https://github.com/nitroos-org/reactos/commit/7850c61328b5806dfd6e223f82e7244dde06cd77))
- RTL/x64: Set EXCEPTION_TARGET_UNWIND after a collided unwind ([fix/rtl-amd64-collided-unwind](https://github.com/nitroos-org/reactos/commit/1290624eb8c02cb5523ab6e9bc7937b5ff396931))
- SDK/KERNEL32: Use the Windows layouts of the Toolhelp structures ([fix/toolhelp-x64-structs](https://github.com/nitroos-org/reactos/commit/9a33b3af64d0e605cd4e0ec600238196b24d1d95))
- SDK:RTL: Fail a NULL string default in RtlQueryRegistryValues ([fix/rtl-query-registry-default](https://github.com/nitroos-org/reactos/commit/056dfc4b60d6d0fa3ddff900b227b4347ef94bbe))
- UCRTBASE: Raise the exceptions of the C99 math functions ([fix/ucrtbase-c99-math](https://github.com/nitroos-org/reactos/commit/0ef691b3bc72d3578a9f8f4ac0fc8dd1370c965f))
- UCRTBASE: Load the x87 control word through the environment image ([fix/ucrtbase-fenv](https://github.com/nitroos-org/reactos/commit/faed9f5f2128561bcfdb00439118e982e3768574))
- UCRTBASE: lgamma(0) raises divide-by-zero; the _o_ gamma and logb variants leave errno alone ([fix/ucrtbase-gamma-logb](https://github.com/nitroos-org/reactos/commit/cfab89b8d75b10648d4da589b907078393e53a91))
- UCRTBASE: Report the math errors of the legacy functions like Windows ([fix/ucrtbase-math-errno](https://github.com/nitroos-org/reactos/commit/e8d8502ab207f1281855253f7a02f8210786c834))

</details>

<details><summary>Browser, COM and Wine imports (14 fixes)</summary>

- APISETS: Resolve the WinRT apisets to combase ([fix/apisets-winrt-combase](https://github.com/nitroos-org/reactos/commit/5d2d3b66f0a4cde0eb29aaa2a68bb1827d4d8407))
- CRYPTBASE: Add cryptbase.dll forwarder DLL ([fix/cryptbase-forwarder](https://github.com/nitroos-org/reactos/commit/ed7e480a123842e879f066d018f1690ddf7227c3))
- IFMON: Show the lease of a DHCP adapter in "show config" ([fix/ifmon-show-dhcp-config](https://github.com/nitroos-org/reactos/commit/8c740f01d7eff86f308c0bec93518fc9d4cd8713))
- KTMW32/NTDLL: Add ktmw32 and fail transactions cleanly ([fix/ktmw32-transactions](https://github.com/nitroos-org/reactos/commit/83fb6fec74596288346c30904b8c4b27dd369962))
- MSHTML: Read the raw HTTP headers as the ANSI text they are ([fix/mshtml-http-headers-ansi](https://github.com/nitroos-org/reactos/commit/e18fda464e0d89d4fd23cdb635d51f378ace5b9f))
- MSI: Give a package with ALLUSERS the all-users Start Menu and Desktop folders ([fix/msi-allusers-folders](https://github.com/nitroos-org/reactos/commit/e8b4ed38f2a539cc558592269f8ef21941dbd590))
- MSI: Publish the first feature to the texts that follow a SelectionTree ([fix/msi-seltree-initial-events](https://github.com/nitroos-org/reactos/commit/98e789b488c183ebd2247c8a6b8da788afa1b8c9))
- MSI/MSIEXEC: Log why a package was refused before its log opened ([fix/msi-early-failure-log-v2](https://github.com/nitroos-org/reactos/commit/f2ad212695e51bcaf196c865dbb1ffb89f88a753))
- NETID: Disable the Network ID button ([fix/netid-network-id-disabled](https://github.com/nitroos-org/reactos/commit/3d0bceb827a0e12ffd13e7066adc5b6ad58c6acd))
- PSAPI/PSAPI_APITEST: Read the lists of a WOW64 process again when they grow, signal the test child from the child ([fix/psapi-missing-exports](https://github.com/nitroos-org/reactos/commit/5784e0ef523d0369bbbaba549daa4d65ca0b4683))
- SHCORE/APISETS: Add GetFeatureVariant, RecordFeatureUsage, RecordFeatureError and UnsubscribeFeatureStateChangeNotification ([fix/shcore-missing-exports](https://github.com/nitroos-org/reactos/commit/721903e0b7470ddd28d40ffeae0b752835efa639))
- TDH/SDK: Import tdh.dll from Wine-11.19 ([fix/tdh-import](https://github.com/nitroos-org/reactos/commit/d926ceed6230ab36a49ad5ede0120607d675c83c))
- UIAUTOMATIONCORE: Add UiaDisconnectAllProviders ([fix/uiautomationcore-missing-exports-v2](https://github.com/nitroos-org/reactos/commit/9ede0086b67defee8dc138b4fad4ba4c0b41b249))
- URLMON: Run protocol tasks that were queued during a client Read ([fix/urlmon-read-task-queue-v2](https://github.com/nitroos-org/reactos/commit/a29b8321636bc32727a587d4de188a481f0bcef5))

</details>

<details><summary>Applications and tools (33 fixes)</summary>

- ACCESS: Fill the High Contrast color scheme list ([fix/access-high-contrast-schemes](https://github.com/nitroos-org/reactos/commit/63fd56cc41c87b0aca9974d24ab976b7f021306e))
- ACGENRAL: Import uxtheme only when a shim uses it ([fix/msvcrt-lock-recursion](https://github.com/nitroos-org/reactos/commit/4c76ce831d34ca5156e50d197e56da0ca174209b))
- APPWIZ/SYSSETUP: Provide Wine Gecko on amd64 ([fix/gecko-amd64](https://github.com/nitroos-org/reactos/commit/46307bf334cfccbde4256f05e7b58c08f73e5468))
- AT: Take the rest of the command line as the command ([fix/at-command-rest-of-line](https://github.com/nitroos-org/reactos/commit/6968d144c1fd716f27fe9f8a680342ce5301a683))
- CACLS: Refuse volumes without ACLs and report errors ([fix/cacls-fat-no-acls](https://github.com/nitroos-org/reactos/commit/e04c672cb6bb86d957198a53db561ca322765f3b))
- CHARMAP: Implement Search in the advanced view ([fix/charmap-search](https://github.com/nitroos-org/reactos/commit/56f44646a65550f33669e10fc56ba4847c23ca0d))
- CLEANMGR: Do not ask to delete files when nothing is ticked ([fix/cleanmgr-nothing-selected](https://github.com/nitroos-org/reactos/commit/38e2895d22f61df9982a9f34e4ba91c00c07d1ab))
- DISKMGMT: Shrink Volume shrinks the partition in place ([fix/dmshrink-dm-grow-r1015b](https://github.com/nitroos-org/reactos/commit/2e58894df95e0b7fcb1bfad10e9047e761b179b2))
- DISKPART: Clear the volume focus on create, keep numbers across all reads ([fix/diskpart-create-selects-volume](https://github.com/nitroos-org/reactos/commit/796b99bcb4ba607afef934df41c5ee562f30d6a6))
- DISKPART: Do not wait for a volume after resizing a partition without one ([fix/p2-usermode-cli](https://github.com/nitroos-org/reactos/commit/e6733a6857c725a0b921f2e14691bb84bbf7f77a))
- FIND: Never match an empty search string ([fix/find-empty-string](https://github.com/nitroos-org/reactos/commit/296151085b3e95812f86aaf67e0f6a2d25bf2a1d))
- FINDSTR: Scan only new input for line ends and report standard input read errors ([fix/findstr](https://github.com/nitroos-org/reactos/commit/2fe3b3380bcd457b088dcf6bf1e0ab0422f03e0c))
- JOY: Disable the Troubleshoot button ([fix/joy-troubleshoot-disabled](https://github.com/nitroos-org/reactos/commit/fb01838d7f518d045c796db681b38c36739234dd))
- KERNEL32/NTMARTA: fix a security-related bug ([fix/usermode-security-cluster](https://github.com/nitroos-org/reactos/commit/8d80b221ffa46d32cbe0371047cef8c07b13f39b))
- MAGNIFY: Disable the Help button, there is no help file ([fix/help-unavailable-disabled](https://github.com/nitroos-org/reactos/commit/5a03092962cdd75111e756a8604d5c4da784d3ec)) — fixes [CORE-20067](https://jira.reactos.org/browse/CORE-20067)
- MSCONFIG: Disable the startup selection that cannot be saved ([fix/msconfig-general-honest](https://github.com/nitroos-org/reactos/commit/f585dde006332f39cb379c5793d53d609e43fb2f))
- MSI/MSIEXEC: Report a package that cannot be opened as Windows does ([fix/msi-open-failure-codes](https://github.com/nitroos-org/reactos/commit/106fbcdf6c7b5c716d2ac08ac87d9ee297dc86e1))
- NOTEPAD: Warn before saving Unicode text as ANSI ([fix/notepad-ansi-save-warning](https://github.com/nitroos-org/reactos/commit/b6c4742886fbf175fe7e4aceb7faa8fde8e0117f))
- NOTEPAD: Make Replace All linear and undoable in one step ([fix/notepad-replace-all-linear](https://github.com/nitroos-org/reactos/commit/17d264ab5f802ed8d3eb19dcf8fd096d4917d188))
- REGEDIT: Pass the key path to the permissions dialog ([fix/regedit-permissions](https://github.com/nitroos-org/reactos/commit/31278d89eddbe5b1a9a1f3375dd38fbe8adcc75e))
- SCHEDSVC: Check stored schedules, arm the timer in UTC of the start date, lock at start ([fix/at-hang](https://github.com/nitroos-org/reactos/commit/c28c72a3d7c0e808c14f9e891a578eee139d0620))
- SCHTASKS: Report that the Task Scheduler is not available instead of doing nothing ([fix/schtasks-honest](https://github.com/nitroos-org/reactos/commit/123341247be5be783e6bed00697c6b53f61e0ac4))
- SERVMAN: Show the pending and paused service states ([fix/servman-pending-status](https://github.com/nitroos-org/reactos/commit/34ecb64433e275aa28e826afaf46ed01fffa135d))
- SERVMAN: fix a security-related bug ([fix/servman-recovery-save](https://github.com/nitroos-org/reactos/commit/b7be8dfcb79e844049455323d63a354f8f0cb678))
- SORT: Sort the named file and take the Windows options ([fix/sort-file-argument-v2](https://github.com/nitroos-org/reactos/commit/63cf20763a07a700317eb06090fd68cebfede961))
- TASKLIST: Accept the largest 64-bit filter value ([fix/tasklist-switches-v3](https://github.com/nitroos-org/reactos/commit/4467c08999ac00edaeda84bc5940ad4d318952dc))
- TASKMGR: Do not close on Enter ([fix/taskmgr-enter](https://github.com/nitroos-org/reactos/commit/015aab0b0d0c04e30caf28483a5c1d47c98eb083))
- TASKMGR: Show one CPU usage history graph per processor ([fix/taskmgr-per-cpu-graphs](https://github.com/nitroos-org/reactos/commit/049839771a03e3c57e49dd237615acb6dd3030b9))
- TELEPHON: Say that Phone and Modem Options is not available ([fix/telephon-not-available](https://github.com/nitroos-org/reactos/commit/b8a7abedfd969f0e07862669c39e5ca2dc255ef8))
- TELNET: Install telnet.ini and telnet.cfg ([fix/telnet-install-config](https://github.com/nitroos-org/reactos/commit/b5e19db1e584c47709cc4a7b25b1e65b1ead7206)) — fixes [CORE-12498](https://jira.reactos.org/browse/CORE-12498)
- TIMEOUT: End the wait only on a key press, not on a key release ([fix/timeout-wait](https://github.com/nitroos-org/reactos/commit/0e95215b82c61dc1769d86996d174694b4fd2d04))
- USER32: IsDialogMessageW: Stop the mnemonic search after one full pass ([fix/user32-dialog-accel-loop](https://github.com/nitroos-org/reactos/commit/ae102eefbd17144f8697093eed28a3aca34b7727))
- WORDPAD: Say that printing is not available instead of blank pages ([fix/wordpad-print-honest](https://github.com/nitroos-org/reactos/commit/af1dbff8c6d67cc6e938c5b02b9447c3ec3391cf))

</details>

<details><summary>Build, toolchain and API surface (10 fixes)</summary>

- APISETS: fix a security-related bug ([fix/apisets-real-hosts](https://github.com/nitroos-org/reactos/commit/b229be0aa30a1833363d455f6adc203ed950df35))
- APISETS: Host the shcore contracts that shcore implements in shcore.dll ([fix/apisets-shcore-hosts](https://github.com/nitroos-org/reactos/commit/b4d75640680f176f52ca9db5cb1bbedfea07acc2))
- APISETS/NTDLL: Ignore the minor version and the manifest when resolving API sets ([fix/apisets-minor-manifest](https://github.com/nitroos-org/reactos/commit/3ad4c066e85b6ac494e793c5791f594fc8c35825))
- include: About: keep the upstream version lines and fit the Nitro lines ([fix/nitro-version-string](https://github.com/nitroos-org/reactos/commit/75a6134fb6aa46ad3625efae9c13880d7cf90bbb))
- PSDK/CABINET_APITEST: Do not pack FDINOTIFICATION to 4 bytes on x64 ([fix/cabinet-fdi-x64-layout](https://github.com/nitroos-org/reactos/commit/2a6e7f5b31bef3c04a393d458c5f06adf736af9b))
- RTL/KERNEL32: Implement RtlFormatMessageEx and format messages through it ([fix/formatmessage-rtl-v2](https://github.com/nitroos-org/reactos/commit/9edb16a53513c90c1ed2b2e56517118fb7670f5b))
- RTL/TZLIB: Identify the current time zone by its key name alone ([fix/auto-dst-disabled](https://github.com/nitroos-org/reactos/commit/67694f3c2b21eafce9b6c68702b53914ce61ec03))
- SDK: Name the revision only from a build tag ([fix/version-revision-date-tag](https://github.com/nitroos-org/reactos/commit/b814861d5826e502b904b750df456046a1d78e0b))
- SDK:MC: Print whole addresses in the hard error texts on 64-bit ([fix/hard-error-address-amd64](https://github.com/nitroos-org/reactos/commit/d61f0afecf3951e52fa740c3b4761e59ae3bf312))
- VCRUNTIME: Fix _AddressOfReturnAddress for x64 GCC ([fix/x64-gcc-return-address](https://github.com/nitroos-org/reactos/commit/2aa7d91b4dcf0b8f081681c4ef361885f4e79374))

</details>

<details><summary>Tests only (26 fixes)</summary>

- ADVAPI32_APITEST: tests for a security-related fix [test-only] ([fix/logon-user-processes](https://github.com/nitroos-org/reactos/commit/1f5ab40693d094654ee904f131bca926fdff82cd))
- APITESTS: Link WoW64 services in static RTL tests [test-only] [pick] ([main/wow64-rtl-unittest-link](https://github.com/nitroos-org/reactos/commit/7426aa673f03210c7d54b155b8d4ee380e14c6bc))
- CRT_APITEST: strlen: clear the direction flag before calling ok() [test-only] ([fix/rosautotest-pipe-nul-m](https://github.com/nitroos-org/reactos/commit/27a1ad93210ba2a9fd7d00c95ae9d186f6eeb0ea))
- GDI32_APITEST: tests for a security-related fix [test-only] ([fix/d3dkmt-dcmem-probe](https://github.com/nitroos-org/reactos/commit/66bdb43cacd68820b0ece7d7422d9f506c850cc4))
- GDI32_WINETEST: Skip the font name checks for FontSubstitutes aliases [test-only] ([fix/gdi32-font-alias-families](https://github.com/nitroos-org/reactos/commit/957f72a208e45f2e05f06529dee6fb27f27d99ee))
- KERNEL32_APITEST: Expect the NTSTATUS in the BindIoCompletionCallback error code [test-only] ([fix/rtl-iocompletion-callback](https://github.com/nitroos-org/reactos/commit/b02e5a87455b7c2342ea00178375634d203cae20))
- KMTESTS:CC: Run the CcLazyWriteDelete test on ReactOS only [test-only] ([fix/cc-lazywrite-dirty-list](https://github.com/nitroos-org/reactos/commit/91f2c6249db853396d8040fe88c34593138e3615))
- KMTESTS:CC: Test concurrent exclusive pins in CcPinRead [test-only] ([fix/cc-pin-lost-race](https://github.com/nitroos-org/reactos/commit/3097d087d36e4c9d419d966c887cdb5266608a90))
- KMTESTS:CC: Do not pass NULL for required CcCopyRead parameters [test-only] ([fix/kmtest-cccopyread-contract](https://github.com/nitroos-org/reactos/commit/484b23f8e0669c582bccdae06214e95c0cb7b247))
- KMTESTS:IO: Call IoFastQueryNetworkAttributes from a system thread [test-only] ([fix/io-deletefile-handle-leak](https://github.com/nitroos-org/reactos/commit/0887a658f96d41351d0a2270c2946c0c9c47c194))
- MSI_WINETEST: Restore the end of test_selectiontree_events [test-only] [pick] ([main/msi-seltree-merge-fix](https://github.com/nitroos-org/reactos/commit/b6a4289b8b292596c158d52c5cc24ce280deffb2))
- NTDLL_APITEST: Do not expect a missed change for a watcher without a buffer [test-only] ([fix/fsrtl-notify-delete](https://github.com/nitroos-org/reactos/commit/eb8bb780232b2e6f2e5d27792738d55e3aaf7ceb))
- NTDLL_APITEST: tests for a security-related fix [test-only] ([fix/io-write-mdl-status](https://github.com/nitroos-org/reactos/commit/9a8182da2fc4699dd83dd976a56af8058351d610))
- NTDLL_APITEST: BadSegmentFault: accept the faulting instruction as the address [test-only] ([fix/ke-amd64-badss-record-leak](https://github.com/nitroos-org/reactos/commit/0ca6e3ef5b0609b23db6bd1f1b6c54570fb8e0cb))
- NTDLL_APITEST: tests for a security-related fix [test-only] ([fix/timezone-privilege](https://github.com/nitroos-org/reactos/commit/bca92924137c9aaffea0c7c89691bbd028aa263e))
- NTFS_APITEST: Expect what Windows does on the empty-name junction reopen [test-only] ([fix/ntfsng-relopen-junction-acl](https://github.com/nitroos-org/reactos/commit/23614592a13c90e022c1d13f18af787051d25fd9))
- ROSAUTOTEST: Keep the --list for a module pattern that ends in "test" [test-only] ([fix/rosautotest-named-test](https://github.com/nitroos-org/reactos/commit/3aa9063d912a8970cc74089e0df617a7e2726626))
- ROSAUTOTEST: Do not lose the rest of a run to a pipe left open by a test [test-only] ([fix/rosautotest-unique-pipe](https://github.com/nitroos-org/reactos/commit/9ebb183b5479b072e3b06356e382f02ed71cde0f))
- SETUPAPI_APITEST: tests for a security-related fix [test-only] ([fix/pnproot-double-free](https://github.com/nitroos-org/reactos/commit/7814ea2076dfdddd3c3c5aaf0406b8a488645fdc))
- stackoverflow-amd64-frame: tests for a security-related fix [test-only] ([fix/stackoverflow-amd64-frame](https://github.com/nitroos-org/reactos/commit/d3855aa29e0afa2af91be4cbafc76cddabc496f8))
- stackoverflowthread-t1-timeout: tests for a security-related fix [test-only] ([fix/stackoverflowthread-t1-timeout](https://github.com/nitroos-org/reactos/commit/a9524479ba1002b57379c4559b6b52a2dec8f475))
- USERENV_APITEST: Delete read-only, hidden and system entries in the DeleteProfile test [test-only] ([fix/userenv-deleteprofile-test-attrs](https://github.com/nitroos-org/reactos/commit/44a0a189344bafb96d8f47787be4e2d9adec5887))
- USERENV_APITEST: DeleteProfile: use todo_if(is_reactos()) for the loaded-hive check [test-only] ([fix/userenv-deleteprofile-todo](https://github.com/nitroos-org/reactos/commit/12a4463321e093a864ffc3285017d3a6d507d098))
- WIN32U_APITEST: tests for a security-related fix [test-only] ([fix/dxg-createsurface-probe](https://github.com/nitroos-org/reactos/commit/b3aadc547daf08981de5d91c4ec916ab02471798))
- WOW64_APITEST: Give the win32k test stubs a native i386 path [test-only] ([fix/wow64-test-stubs-native-gate](https://github.com/nitroos-org/reactos/commit/cc6d95668f6aef81f142cf0f6d78efc51a52aa86))
- WS2_32_WINETEST: Query the completion notification modes before closing the socket [test-only] ([fix/ws2-winetest-iocn-query](https://github.com/nitroos-org/reactos/commit/7ecaa32d7b71dc142c2e36c53a736b8bc18d4b28))

</details>


### Limitations

- Firefox sandbox: on 64-bit it starts at default levels but restrictions are not enforced (a test that tries to create files in 5 places: all denied on Windows 10, all allowed here). Do not treat the build as sandboxed. 32-bit Firefox runs with the sandbox off.
- Firefox ESR 140 and later do not fully run yet: `cryptbase.dll` is now provided, but they still need `QueryUnbiasedInterruptTimePrecise`, which ReactOS does not implement. ESR 128 is the tested version. The Firefox installers (NSIS) only run with the Windows 10 compatibility layer set.
- Known flaky areas: 32-bit Firefox https tab sometimes never loads (2 of about 8 runs); 64-bit Firefox parent spun at 95.7% CPU in 1 of 8 runs; about 3 of 45 boots hang in ipconfig before the network is up; the full test run shows 16 regressions against upstream, no bugchecks.
- WoW64 is amd64 only and opt-in; some 32-bit system calls are not thunked yet (about 250 debug messages per Firefox run), some console calls from 32-bit console programs fail, CreateProfile fails in 32-bit processes, and the 32-bit Total Commander installers use `Program Files`.
- NTFS: no repair tool beyond journal replay; a junction in the middle of a path returns an error; persistent-ACL support is not reported.
- SMP: more processors do not always mean more throughput (8 versus 4 vCPUs was within noise on Intel and not measurable on AMD hosts); cached parallel reads fall as vCPUs rise; i386 on AMD hosts is far slower than amd64 because every interrupt-priority change exits to the host.
- Display: the shadow framebuffer is one paged-pool block (about 33 MB at 4K); 3200x2400 can break into the kernel; 8 bpp and real VESA hardware are untested.
- Memory: private memory is not paged out, so heavy memory pressure can stall instead of failing cleanly.
- Btrfs: volumes spanning several disks (RAID, multi-device) are not mounted; Check Disk is not supported.
- Installing onto Btrfs from the graphical setup hangs or fails.
- Some reported kernel issues are still under investigation.

### Changes since the previous Nitro

This is the first Nitro release, so there is no previous Nitro to compare with. The complete list of what it contains is the Bug fixes section above (one line per fix branch) together with the tracks. For context, relative to the last internal changelog (main `44e1aa62b1d`, 2026-10-18):
- The upstream base moved from `6ba28adc72c` to `ac1ab9e3043` (two merges, not rebases); upstream's own C:\Users layout was adopted.
- Main moved on by six integration batches, to `23dd8f7e336`.
- The first patch series has 47 topics; a branding patch set is kept aside, and reactos#9618 is merged upstream and no longer counts as a fork change.
- Six experimental branches (an NTFS check tool, several Windows 6 API sets, a software OpenGL) were closed without merging.
