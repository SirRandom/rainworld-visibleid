namespace fish.Mods.RainWorld.VisibleID;

public sealed class Cfg: OptionInterface {
	Cfg() {}
	public static Cfg Instance { get; } = new();
	
	public static Configurable<int> ConfigVersion { get; } = bind(nameof(ConfigVersion), 0,
		$"The version of this configuration. This value is presently used to migrate the value of the \"{nameof(Names)}\" setting across different mod versions. In the future it may be used for further attempts in forward-/backward-compatibility"
	);
	
	public enum KeybindMode: int {
		Toggle = 0,
		Held = 1,
	}
	
	public static Configurable<KeyCode> ToggleID        { get; } = bind(nameof(ToggleID       ), Keys.Tab, "The key that should toggle ID display on and off");
	public static Configurable<int>     ToggleIDMode    { get; } = bind(nameof(ToggleIDMode   ), 0,        "The mode for the ToggleID keybind. Should it be a toggle button or a \"hold\" button?");
	public static Configurable<KeyCode> ToggleStats     { get; } = bind(nameof(ToggleStats    ), Keys.End, "The key that should toggle personality & traits display on and off");
	public static Configurable<int>     ToggleStatsMode { get; } = bind(nameof(ToggleStatsMode), 0,        "The mode for the ToggleStats keybind. Should it be a toggle button or a \"hold\" button?");
	
	public static Configurable<bool>    ShowIDs         { get; } = bind(nameof(ShowIDs        ), false,    "Should ID labels be on at the start of the game?");
	public static Configurable<bool>    Attrs           { get; } = bind(nameof(Attrs          ), false,    "Should the personality & skills readout be on at the start of the game?");
	public static Configurable<bool>    Players         { get; } = bind(nameof(Players        ), true,     "Should we show ID labels for players?");
	public static Configurable<bool>    PlyrAttr        { get; } = bind(nameof(PlyrAttr       ), false,    "Should we show personality traits for players?");
	public static Configurable<bool>    Dead            { get; } = bind(nameof(Dead           ), false,    "Should labels disappear when the attached creature dies?");
	public static Configurable<bool>    Objects         { get; } = bind(nameof(Objects        ), false,    "Should we show ID labels for objects?");
	public static Configurable<bool>    Spoilers        { get; } = bind(nameof(Spoilers       ), false,    "Show potential spoilers?");
	
	public static Configurable<string> Names { get; } = bind(nameof(Names), "");
	
	static Configurable<T> bind<T>(string name, T init, string desc = null) => Instance.config.Bind<T>($"fish_visibleid_{name}", init, desc is null ? null : new ConfigurableInfo(desc));
	
	public override void Initialize()
		=> Tabs = new Menu.Remix.MixedUI.OpTab[] {
			new CfgTabMain(),
			new CfgTabKeybinds(),
			new CfgTabNames(),
			new CfgTabInspect(),
		};
	
	const char fs = '\x001C';
	const char gs = '\x001D';
	const char rs = '\x001E';
	const char us = '\x001F';
	
	public static void EarlySettingCleanup_RunASAP() {
		
	}
	
	static Dictionary<(int id, string type), string> ParseNamesVersion0(string names) {
		Dictionary<(int id, string type), string> mapping = new();
		
		foreach(var record in names.Split(';')) {
			var (id,type,name) = record.Split(':');
			try {
				mapping.Add((int.Parse(id),type), name);
			} catch(Exception e) {
				Error($"Problem in {nameof(ParseNamesVersion0)} while adding data to {nameof(mapping)}:\n{e.Message}\n{e.StackTrace}");
			}
		}
		
		return mapping;
	}
	
	static Dictionary<(int id, string type), string> ParseNamesVersion1(string names) {
		Dictionary<(int id, string type), string> mapping = new();
		
		foreach(var record in names.Split(rs)) {
			var (id,type,name) = record.Split(us);
			try {
				mapping.Add((int.Parse(id),type), name);
			} catch(Exception e) {
				Error($"Problem in {nameof(ParseNamesVersion1)} while adding data to {nameof(mapping)}:\n{e.Message}\n{e.StackTrace}");
			}
		}
		
		return mapping;
	}
	
	static string ToLatestNamesFormat(Dictionary<(int id, string type), string> mapping)
		=> string.Join(rs.ToString(), mapping.Select(kv => $"{kv.Key.id}{us}{kv.Key.type}{us}{kv.Value}"));
}
