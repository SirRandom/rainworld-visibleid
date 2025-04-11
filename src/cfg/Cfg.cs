namespace fish.Mods.RainWorld.VisibleID;

public sealed class Cfg: OptionInterface {
	Cfg() {}
	public static Cfg Instance { get; } = new();
	
	public const int SupportedConfigVersion = 1;
	
	public static Configurable<int> ConfigVersion { get; } = bind(nameof(ConfigVersion), 0,
		$"The version of this configuration. This value is presently used to migrate the value of the \"{nameof(CfgNames)}\" setting across different mod versions. In the future it may be used for further attempts in forward-/backward-compatibility"
	);
	
	public static Configurable<KeyCode> ToggleID          { get; } = bind("ToggleID"        , Keys.Tab, "The key that should toggle ID display on and off for creatures");
	public static Configurable<int>     ToggleIDMode      { get; } = bind("ToggleIDMode"    , 0,        $"The mode for the {nameof(ToggleID)} keybind. Should it be a toggle (0) button or a \"hold\" (1) button?");
	public static Configurable<KeyCode> ToggleObjID       { get; } = bind("ToggleObjID"     , Keys.Tab, "The key that should toggle ID display on and off for objects");
	public static Configurable<int>     ToggleObjIDMode   { get; } = bind("ToggleObjIDMode" , 0,        $"The mode for the {nameof(ToggleObjID)} keybind. Should it be a toggle (0) button or a \"hold\" (1) button?");
	public static Configurable<KeyCode> ToggleStats       { get; } = bind("ToggleStats"     , Keys.End, "The key that should toggle personality & traits display on and off");
	public static Configurable<int>     ToggleStatsMode   { get; } = bind("ToggleStatsMode" , 0,        $"The mode for the {nameof(ToggleStats)} keybind. Should it be a toggle (0) button or a \"hold\" (1) button?");
	
	public static Configurable<bool> ShowIDs  { get; } = bind("ShowIDs"  , false, "Should ID labels be on at the start of the game?");
	public static Configurable<bool> Attrs    { get; } = bind("Attrs"    , false, "Should the personality & skills readout be on at the start of the game?");
	public static Configurable<bool> Players  { get; } = bind("Players"  , true,  "Should we show ID labels for players?");
	public static Configurable<bool> PlyrAttr { get; } = bind("PlyrAttr" , false, "Should we show personality traits for players?");
	public static Configurable<bool> Dead     { get; } = bind("Dead"     , false, "Should labels disappear if the attached creature dies?");
	public static Configurable<bool> Objects  { get; } = bind("Objects"  , false, "Should ID labels for objects be on at the start of the game?");
	public static Configurable<bool> Spoilers { get; } = bind("Spoilers" , false, "Show potential spoilers?");
	
	public static Configurable<string> CfgNames { get; } = bind("Names", "");
	public static Configurable<string> OldNames { get; } = bind(nameof(OldNames), "");
	
	public static NameMap Names;
	
	static Configurable<T> bind<T>(string name, T init, string desc = null) => Instance.config.Bind<T>($"fish_visibleid_{name}", init, desc is null ? null : new ConfigurableInfo(desc));
	
	public static void Save() => Instance.config.Save();
	
	public override void Initialize()
		=> Tabs = [
			new CfgTabMain(),
			new CfgTabKeybinds(),
			new CfgTabNames(),
			new CfgTabInspect(),
		];
	
	public static void EarlySetup_RunASAP() {
		Info($"Beginning early config setup");
		if(ConfigVersion.Value is not SupportedConfigVersion) {
			Info($"  Migrating from {nameof(ConfigVersion)} {ConfigVersion.Value} to {SupportedConfigVersion}");
				OldNames.Value = CfgNames.Value;
				if(ConfigVersion.Value < 1) CfgNames.Value = (Names = new(CfgNames.Value, ConfigVersion.Value)).ToString();
				ConfigVersion.Value = SupportedConfigVersion;
				Save();
			Info($"  Done");
		}
		OverheadID.CreatureIDLabelVisible = Cfg.ShowIDs.Value;
		OverheadID.ObjectIDLabelVisible = Cfg.Objects.Value;
		OverheadID.StatsVisible = Cfg.Attrs.Value;
		ReloadNames();
		Info($"Early config setup complete");
	}
	
	public static void ReloadNames() => Names = new(CfgNames.Value, ConfigVersion.Value);
}
