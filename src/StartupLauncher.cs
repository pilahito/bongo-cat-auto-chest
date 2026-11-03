using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32;

namespace BongoClaim
{
    // Optional launch-time repair; no resident watcher. The game's Autostart
    // off/on sequence restores its direct EXE entry. Enable this option again
    // afterwards to restore the checked launcher. Steam's helper and marker
    // are co-located with the game; Windows sign-in keeps its LocalAppData copy.
    public static class StartupLauncher
    {
        private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string RunName = "BongoCat", HelperName = "BongoCatAutoChest.exe";
        private const string MutexName = @"Local\BongoCatAutoChest.Startup";
        private const string SteamMutexName = @"Local\BongoCatAutoChest.SteamLaunch";
        private const string GuardSetupError = "Steam launch guard setup is missing or invalid for this game folder. Open Bongo Cat Auto Chest and set up its Steam launch guard again.";

        [DataContract]
        private sealed class StartupState
        {
            [DataMember] public int Version = 1;
            [DataMember] public string Root, Command, OriginalValue;
            [DataMember] public int OriginalKind;
        }
        private sealed class RunValue { public string Value; public RegistryValueKind Kind; }
        [DataContract]
        private sealed class SteamGuardState
        {
            [DataMember(IsRequired = true)] public string Root;
            [DataMember(IsRequired = true)] public bool Enabled;
        }

        public static string StartupDirectory { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BongoCatAutoChest"); } }
        public static string LogPath { get { return Path.Combine(StartupDirectory, "startup.log"); } }
        private static string StatePath { get { return Path.Combine(StartupDirectory, "startup.json"); } }
        private static string HelperPath { get { return Path.Combine(StartupDirectory, HelperName); } }
        private static string SteamLauncherDirectory(string root) { return Path.Combine(root, ".bongo-claim", "launcher"); }
        private static string SteamGuardPath(string root) { return Path.Combine(SteamLauncherDirectory(root), "steam-guard.json"); }
        private static string GuardSetupMessage(string root)
        {
            try { return GuardSetupError + " Expected marker: " + SteamGuardPath(NormalizeRoot(root)); }
            catch (ArgumentException) { return GuardSetupError + " Expected marker: .bongo-claim\\launcher\\steam-guard.json in the selected game folder."; }
        }

        private static string NormalizeRoot(string root)
        {
            if (String.IsNullOrWhiteSpace(root)) throw new ArgumentException("Choose the Bongo Cat game folder first.");
            var full = Path.GetFullPath(root);
            if (full.Length > Path.GetPathRoot(full).Length) full = full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (full.IndexOf('"') >= 0 || full.IndexOf('\r') >= 0 || full.IndexOf('\n') >= 0) throw new ArgumentException("The game folder contains an invalid command-line character.");
            return full;
        }
        private static string Quote(string value) { return "\"" + value + "\""; }
        private static string Command(string root) { return Quote(HelperPath) + " --launch " + Quote(root); }
        private static bool Equal(string left, string right) { return String.Equals(left, right, StringComparison.OrdinalIgnoreCase); }
        private static bool CurrentControls(Inspection result) { return result.Supported && result.Installed && result.RuntimeControls && result.OwnedTokenControls; }

        private static RunValue ReadRun()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKey, false))
            {
                if (key == null) return new RunValue();
                var value = key.GetValue(RunName, null, RegistryValueOptions.DoNotExpandEnvironmentNames);
                if (value == null) return new RunValue();
                var text = value as string;
                if (text == null) throw new InvalidOperationException("The Bongo Cat startup entry has an unfamiliar value. It was left unchanged.");
                return new RunValue { Value = text, Kind = key.GetValueKind(RunName) };
            }
        }
        private static bool SameRun(RunValue left, RunValue right) { return String.Equals(left.Value, right.Value, StringComparison.Ordinal) && (left.Value == null || left.Kind == right.Kind); }
        private static bool DirectGameCommand(string command, string root)
        {
            if (command == null) return true;
            var exe = Path.Combine(root, "BongoCat.exe");
            return Equal(command.Trim(), Quote(exe)) || Equal(command.Trim(), exe);
        }

        private static StartupState ReadState()
        {
            if (!File.Exists(StatePath)) return null;
            StartupState state;
            using (var stream = File.OpenRead(StatePath)) state = (StartupState)new DataContractJsonSerializer(typeof(StartupState)).ReadObject(stream);
            if (state == null || state.Version != 1 || !Equal(NormalizeRoot(state.Root), state.Root) || !String.Equals(state.Command, Command(state.Root), StringComparison.Ordinal)
                || !DirectGameCommand(state.OriginalValue, state.Root)
                || (state.OriginalValue != null && state.OriginalKind != (int)RegistryValueKind.String && state.OriginalKind != (int)RegistryValueKind.ExpandString))
                throw new InvalidOperationException("The saved startup backup is invalid. The startup entry was left unchanged.");
            return state;
        }
        private static void SaveState(StartupState state) { SaveData(StatePath, state); }
        private static void SaveData<T>(string path, T state)
        {
            var pending = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (var stream = File.Create(pending)) new DataContractJsonSerializer(typeof(T)).WriteObject(stream, state);
                if (File.Exists(path)) File.Replace(pending, path, null); else File.Move(pending, path);
            }
            finally { if (File.Exists(pending)) File.Delete(pending); }
        }
        private static void CopyVerified(string source, string target)
        {
            if (!File.Exists(source)) throw new FileNotFoundException("Extract the whole release ZIP before enabling the startup launcher.", source);
            var expected = PatchEngine.Hash(source);
            if (Equal(Path.GetFullPath(source), Path.GetFullPath(target))) return;
            if (File.Exists(target) && Equal(PatchEngine.Hash(target), expected)) return;
            var pending = target + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                File.Copy(source, pending, false);
                if (!Equal(PatchEngine.Hash(pending), expected)) throw new IOException("The copied startup helper failed verification. The startup entry was left unchanged.");
                if (File.Exists(target)) File.Replace(pending, target, null); else File.Move(pending, target);
                if (!Equal(PatchEngine.Hash(target), expected)) throw new IOException("The startup helper failed verification. The startup entry was left unchanged.");
            }
            finally { if (File.Exists(pending)) File.Delete(pending); }
        }
        private static bool TakeMutex(Mutex mutex)
        {
            try { return mutex.WaitOne(0); }
            catch (AbandonedMutexException) { return true; }
        }

        private static void Deploy() { DeployTo(StartupDirectory); }
        private static void DeployTo(string directory)
        {
            var sourceDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.CreateDirectory(directory);
            CopyVerified(Path.Combine(sourceDirectory, "Mono.Cecil.dll"), Path.Combine(directory, "Mono.Cecil.dll"));
            CopyVerified(Path.Combine(sourceDirectory, HelperName), Path.Combine(directory, HelperName));
        }

        public static string PrepareSteamLaunchCommand(string root)
        {
            root = NormalizeRoot(root);
            PatchEngine.EnsureGameClosed(root);
            CheckSteamUpdate(root);
            if (!File.Exists(Path.Combine(root, "BongoCat.exe"))) throw new InvalidOperationException("Choose the folder containing BongoCat.exe.");
            var inspection = PatchEngine.Inspect(PatchEngine.GameAssembly(root));
            if (!CurrentControls(inspection)) throw new InvalidOperationException("Install the supported current controls before setting up the Steam launch guard. " + inspection.Message);
            using (var mutex = new Mutex(false, MutexName))
            {
                if (!TakeMutex(mutex)) throw new InvalidOperationException("The startup launcher is busy. Try again after it finishes.");
                try
                {
                    // Explicit setup replaces a stale marker after a moved game or a damaged setup.
                    Deploy();
                    DeployTo(SteamLauncherDirectory(root));
                    PatchEngine.EnsureGameClosed(root);
                    SaveData(SteamGuardPath(root), new SteamGuardState { Root = root, Enabled = true });
                    LogSteam(root, "Steam launch guard enabled. Marker: " + SteamGuardPath(root));
                    return FormatArguments(new[] { Path.Combine(SteamLauncherDirectory(root), HelperName) }) + " --steam-launch %command%";
                }
                finally { mutex.ReleaseMutex(); }
            }
        }

        // Pure validation is shared with tests. A disabled matching marker
        // explicitly requests an ordinary launch after Restore original.
        public static bool SteamGuardRequiresPatch(string root, string json)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(json)) throw new SerializationException();
                SteamGuardState state;
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    state = (SteamGuardState)new DataContractJsonSerializer(typeof(SteamGuardState)).ReadObject(stream);
                if (state == null || !Equal(NormalizeRoot(root), NormalizeRoot(state.Root))) throw new SerializationException();
                return state.Enabled;
            }
            catch (SerializationException) { throw new InvalidOperationException(GuardSetupMessage(root)); }
            catch (ArgumentException) { throw new InvalidOperationException(GuardSetupMessage(root)); }
        }

        public static string DisableSteamGuard(string root)
        {
            root = NormalizeRoot(root);
            using (var mutex = new Mutex(false, SteamMutexName))
            {
                if (!TakeMutex(mutex)) throw new InvalidOperationException("Close the existing Bongo Cat session and let its Steam launch guard finish, then restore again.");
                try
                {
                    var marker = SteamGuardPath(root);
                    if (!File.Exists(marker)) return "No Steam launch guard was configured. Nothing was changed.";
                    try { SteamGuardRequiresPatch(root, File.ReadAllText(marker)); }
                    catch (InvalidOperationException) { return "No matching valid Steam launch guard setup was found. It was left unchanged."; }
                    SaveData(marker, new SteamGuardState { Root = root, Enabled = false });
                    LogSteam(root, "Steam launch guard disabled; the Steam wrapper will launch without applying the patch.");
                    return "Steam launch guard disabled. The existing Steam launch option will start the original game without reinstalling the patch.";
                }
                finally { mutex.ReleaseMutex(); }
            }
        }

        public static bool EnabledFor(string root)
        {
            if (String.IsNullOrWhiteSpace(root)) return false;
            return String.Equals(ReadRun().Value, Command(NormalizeRoot(root)), StringComparison.Ordinal);
        }
        public static string Enable(string root)
        {
            root = NormalizeRoot(root);
            PatchEngine.EnsureGameClosed(root);
            var inspection = PatchEngine.Inspect(PatchEngine.GameAssembly(root));
            if (!CurrentControls(inspection)) throw new InvalidOperationException("Install the supported current controls before enabling the startup launcher. " + inspection.Message);
            if (!File.Exists(Path.Combine(root, "BongoCat.exe"))) throw new InvalidOperationException("Choose the folder containing BongoCat.exe.");
            using (var mutex = new Mutex(false, MutexName))
            {
                if (!TakeMutex(mutex)) throw new InvalidOperationException("The startup launcher is busy. Try again after it finishes.");
                try
                {
                    var current = ReadRun();
                    var prior = ReadState();
                    if (prior != null && String.Equals(current.Value, prior.Command, StringComparison.Ordinal) && !Equal(root, prior.Root))
                        throw new InvalidOperationException("The startup launcher already belongs to another game folder. Disable it for that folder first.");
                    var owned = String.Equals(current.Value, Command(root), StringComparison.Ordinal);
                    if (owned && (prior == null || !Equal(root, prior.Root))) throw new InvalidOperationException("The startup entry has no matching original backup. It was left unchanged.");
                    if (!owned && !DirectGameCommand(current.Value, root)) throw new InvalidOperationException("The Bongo Cat startup entry points somewhere else. It was left unchanged.");
                    Deploy();
                    var state = owned ? prior : new StartupState { Root = root, Command = Command(root), OriginalValue = current.Value, OriginalKind = (int)current.Kind };
                    SaveState(state);
                    PatchEngine.EnsureGameClosed(root);
                    if (!SameRun(current, ReadRun())) throw new InvalidOperationException("The startup entry changed during setup. It was left unchanged; check it and try again.");
                    using (var key = Registry.CurrentUser.CreateSubKey(RunKey)) key.SetValue(RunName, state.Command, RegistryValueKind.String);
                    Log("Startup launcher enabled.");
                    return "Startup launcher enabled. It will check the patch before starting Bongo Cat through Steam at sign-in.";
                }
                finally { mutex.ReleaseMutex(); }
            }
        }
        public static string Disable(string root)
        {
            root = NormalizeRoot(root);
            using (var mutex = new Mutex(false, MutexName))
            {
                if (!TakeMutex(mutex)) throw new InvalidOperationException("The startup launcher is busy. Try again after it finishes.");
                try
                {
                    var current = ReadRun();
                    if (!String.Equals(current.Value, Command(root), StringComparison.Ordinal)) return "The startup entry was already removed or changed. Nothing was changed.";
                    var state = ReadState();
                    if (state == null || !Equal(state.Root, root)) throw new InvalidOperationException("The original startup backup is missing. The startup entry was left unchanged.");
                    if (!SameRun(current, ReadRun())) throw new InvalidOperationException("The startup entry changed. Nothing was changed.");
                    using (var key = Registry.CurrentUser.CreateSubKey(RunKey))
                    {
                        if (state.OriginalValue == null) key.DeleteValue(RunName, false);
                        else key.SetValue(RunName, state.OriginalValue, (RegistryValueKind)state.OriginalKind);
                    }
                    Log("Original startup setting restored.");
                    return "The previous Bongo Cat startup setting was restored.";
                }
                finally { mutex.ReleaseMutex(); }
            }
        }

        // Steam invokes the guard after updating and may already mark the app
        // running: 4 = installed, 68 = installed + running. Reject other flags.
        public static bool IsUpdatePending(string manifest)
        {
            if (manifest == null) return true;
            var match = Regex.Match(manifest, "\"StateFlags\"\\s*\"([0-9]+)\"", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            ulong flags;
            return !match.Success || !UInt64.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out flags) || (flags != 4 && flags != 68);
        }
        private static void CheckSteamUpdate(string root)
        {
            var common = Directory.GetParent(root);
            var steamApps = common == null ? null : common.Parent;
            if (steamApps == null) return;
            var manifest = Path.Combine(steamApps.FullName, "appmanifest_3419430.acf");
            if (!File.Exists(manifest)) return;
            if (IsUpdatePending(File.ReadAllText(manifest)))
                throw new InvalidOperationException("Steam has a pending or incomplete Bongo Cat update. Let Steam finish updating, then launch through Bongo Cat Auto Chest again. No game files were changed by this check.");
        }
        public static Inspection Prepare(string root)
        {
            root = NormalizeRoot(root);
            PatchEngine.EnsureGameClosed(root);
            if (!File.Exists(Path.Combine(root, "BongoCat.exe"))) throw new InvalidOperationException("The Bongo Cat game folder is unavailable. Check that its drive is connected, then try again.");
            CheckSteamUpdate(root);
            var result = PatchEngine.Inspect(PatchEngine.GameAssembly(root));
            if (!result.Supported) throw new InvalidOperationException("The game cannot be started with the current patch: " + result.Message);
            if (!CurrentControls(result))
            {
                PatchEngine.Install(root, true);
                result = PatchEngine.Inspect(PatchEngine.GameAssembly(root));
            }
            if (!CurrentControls(result)) throw new InvalidOperationException("The patch could not be verified. The game was not started.");
            return result;
        }
        private static bool GameRunning(string root)
        {
            foreach (var process in Process.GetProcessesByName("BongoCat"))
                using (process)
                    try { if (Equal(Path.GetDirectoryName(process.MainModule.FileName), root)) return true; }
                    catch (System.ComponentModel.Win32Exception) { return true; }
                    catch (InvalidOperationException) { }
            return false;
        }
        private static string SteamExe()
        {
            var locations = new[] {
                Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null) as string,
                Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null) as string,
                Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null) as string
            };
            foreach (var location in locations)
            {
                if (String.IsNullOrWhiteSpace(location)) continue;
                var exe = Path.GetFullPath(Path.Combine(location, "steam.exe"));
                if (File.Exists(exe)) return exe;
            }
            throw new InvalidOperationException("Steam could not be found. Open Steam once, then launch through Bongo Cat Auto Chest again.");
        }
        private static string RunningStatus(string root, bool steamGuard = false)
        {
            if (!CurrentControls(PatchEngine.Inspect(PatchEngine.GameAssembly(root))))
                throw new InvalidOperationException("Bongo Cat is already running without the current auto-claim controls. Exit it normally, then launch it through the configured Steam launch guard or Bongo Cat Auto Chest. The running game was left untouched.");
            if (steamGuard) LogSteam(root, "Bongo Cat is already running with the current controls; guard preparation skipped.");
            else Log("Bongo Cat is already running with the current controls; startup preparation skipped.");
            return "Bongo Cat is already running with the current controls. No game files were changed.";
        }

        private static bool MatchesVerifiedInstall(string root, Inspection inspection)
        {
            var state = PatchEngine.LoadState(root);
            if (state == null || !Equal(state.PatchedHash, inspection.Hash) || state.OriginalHash == null || !Regex.IsMatch(state.OriginalHash, "^[0-9a-fA-F]{64}$")) return false;
            var backup = Path.Combine(root, ".bongo-claim", "backups", state.OriginalHash.ToLowerInvariant() + ".dll");
            return File.Exists(backup) && Equal(PatchEngine.Hash(backup), state.OriginalHash);
        }

        public static string Launch(string root)
        {
            Log("Startup launcher invoked.");
            try
            {
                root = NormalizeRoot(root);
                if (GameRunning(root)) return RunningStatus(root);
                using (var mutex = new Mutex(false, MutexName))
                {
                    if (!TakeMutex(mutex)) { Log("Another startup launcher invocation is already working; skipped."); return "The startup launcher is already working."; }
                    try
                    {
                        if (GameRunning(root)) return RunningStatus(root);
                        var steam = SteamExe();
                        var prepared = Prepare(root);
                        CheckSteamUpdate(root);
                        Log("Patch verified; starting Bongo Cat through Steam.");
                        using (var process = Process.Start(new ProcessStartInfo(steam, "-applaunch 3419430") { UseShellExecute = false, WindowStyle = ProcessWindowStyle.Hidden, WorkingDirectory = Path.GetDirectoryName(steam) })) { }
                        var wait = Stopwatch.StartNew();
                        while (!GameRunning(root) && wait.Elapsed < TimeSpan.FromSeconds(60)) Thread.Sleep(500);
                        if (!GameRunning(root)) throw new InvalidOperationException("Steam was opened, but Bongo Cat did not start within 60 seconds. Finish any Steam update or sign-in, then launch through Bongo Cat Auto Chest again.");
                        var after = PatchEngine.Inspect(PatchEngine.GameAssembly(root));
                        if (!CurrentControls(after) || (!Equal(after.Hash, prepared.Hash) && !MatchesVerifiedInstall(root, after)))
                            throw new InvalidOperationException("Steam replaced the game files during launch without a verified guard repair. Exit Bongo Cat normally, finish its Steam update, then launch through the configured Steam launch guard or Bongo Cat Auto Chest again. The running game was left untouched.");
                        Log("Bongo Cat started; the installed patch and any Steam guard repair are verified.");
                        return "Bongo Cat started through Steam with the patch verified.";
                    }
                    finally { mutex.ReleaseMutex(); }
                }
            }
            catch (Exception ex) { Log("Startup stopped: " + ex.Message); throw; }
        }

        public static string ValidateSteamGamePath(string exe)
        {
            var volume = String.IsNullOrWhiteSpace(exe) ? null : Path.GetPathRoot(exe);
            if (String.IsNullOrWhiteSpace(volume) || volume.Length < 3 || (volume[volume.Length - 1] != '\\' && volume[volume.Length - 1] != '/'))
                throw new ArgumentException("Steam must supply the full path of BongoCat.exe.");
            var full = Path.GetFullPath(exe);
            if (!Equal(Path.GetFileName(full), "BongoCat.exe") || !File.Exists(full))
                throw new InvalidOperationException("The Steam launch command must point to an existing BongoCat.exe.");
            return NormalizeRoot(Path.GetDirectoryName(full));
        }

        // Windows CommandLineToArgvW/CRT quoting. Shell metacharacters remain
        // ordinary argument text because the child is created without a shell.
        public static string FormatArguments(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            var result = new StringBuilder();
            foreach (var arg in args)
            {
                if (arg == null) throw new ArgumentException("A game argument is missing.");
                if (result.Length != 0) result.Append(' ');
                var quote = arg.Length == 0 || arg.IndexOf('"') >= 0;
                foreach (var character in arg) if (Char.IsWhiteSpace(character)) { quote = true; break; }
                if (!quote) { result.Append(arg); continue; }
                result.Append('"');
                var slashes = 0;
                foreach (var character in arg)
                {
                    if (character == '\\') { slashes++; continue; }
                    result.Append('\\', character == '"' ? slashes * 2 + 1 : slashes);
                    result.Append(character); slashes = 0;
                }
                result.Append('\\', slashes * 2); result.Append('"');
            }
            return result.ToString();
        }

        public static string SteamLaunch(string[] originalCommand)
        {
            if (originalCommand == null || originalCommand.Length == 0) throw new ArgumentException("Steam did not provide the original Bongo Cat launch command.");
            var root = ValidateSteamGamePath(originalCommand[0]);
            var args = new string[originalCommand.Length - 1];
            Array.Copy(originalCommand, 1, args, 0, args.Length);
            var arguments = FormatArguments(args);
            var marker = SteamGuardPath(root);
            var markerExists = File.Exists(marker);
            LogSteam(root, "Steam launch guard invoked. Marker: " + marker + "; exists=" + markerExists + (markerExists ? "." : "; game folder=" + root + "."));
            try
            {
                using (var mutex = new Mutex(false, SteamMutexName))
                {
                    if (!TakeMutex(mutex)) throw new InvalidOperationException("The Steam launch guard is already running. Close the existing Bongo Cat session before launching another.");
                    try
                    {
                        var enabled = SteamGuardRequiresPatch(root, File.Exists(marker) ? File.ReadAllText(marker) : null);
                        if (GameRunning(root))
                        {
                            if (enabled) return RunningStatus(root, true);
                            LogSteam(root, "Disabled Steam launch guard skipped an already running game.");
                            return "Bongo Cat is already running.";
                        }
                        if (enabled) { Prepare(root); LogSteam(root, "Steam launch guard verified the patch after Steam's update step."); }
                        else LogSteam(root, "Steam launch guard is explicitly disabled; launching without patch preparation.");
                        // Steam already performed its update step and supplied the
                        // original command. Calling Steam here would recurse.
                        using (var game = Process.Start(new ProcessStartInfo(Path.GetFullPath(originalCommand[0]), arguments) { UseShellExecute = false, WorkingDirectory = root }))
                        {
                            if (game == null) throw new InvalidOperationException("Steam's original Bongo Cat executable could not be started.");
                            game.WaitForExit();
                        }
                        LogSteam(root, "Steam-launched Bongo Cat session ended.");
                        return "The Bongo Cat session ended.";
                    }
                    finally { mutex.ReleaseMutex(); }
                }
            }
            catch (Exception ex) { LogSteam(root, "Steam launch stopped: " + ex.Message); throw; }
        }
        private static void Log(string status) { AppendLog(LogPath, status); }
        private static void LogSteam(string root, string status) { AppendLog(Path.Combine(SteamLauncherDirectory(root), "steam-launch.log"), status); }
        private static void AppendLog(string path, string status)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                var line = Regex.Replace(status ?? "", @"[\x00-\x1f\x7f\u0085\u2028\u2029]+", " ");
                File.AppendAllText(path, DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture) + " " + line + Environment.NewLine, Encoding.UTF8);
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
}

