namespace fish.rainworld.visibleid;

public partial class Cfg: OptionInterface {
	Cfg() {}
	public static Cfg Instance { get; } = new();
	
	#region Actual Configurables
		public static Configurable<int> ConfigVersion { get; } = bind(nameof(ConfigVersion), 0,
			$"The version of this configuration. This value is presently used to migrate the value of the \"{nameof(Names)}\" setting across different mod versions. In the future it may be used for further attempts in forward-/backward-compatibility"
		);
		
		public static Configurable<KeyCode> ToggleID    { get; } = bind(nameof(ToggleID   ), Keys.Tab, "The key that should toggle ID display on and off");
		public static Configurable<KeyCode> ToggleStats { get; } = bind(nameof(ToggleStats), Keys.End, "The key that should toggle personality & traits display on and off");
		public static Configurable<bool>    ShowIDs     { get; } = bind(nameof(ShowIDs    ), false,    "Should ID labels be on at the start of the game?");
		public static Configurable<bool>    Attrs       { get; } = bind(nameof(Attrs      ), false,    "Should the personality & skills readout be on at the start of the game?");
		public static Configurable<bool>    Players     { get; } = bind(nameof(Players    ), true,     "Should we show ID labels for players?");
		public static Configurable<bool>    PlyrAttr    { get; } = bind(nameof(PlyrAttr   ), false,    "Should we show personality traits for players?");
		public static Configurable<bool>    Dead        { get; } = bind(nameof(Dead       ), false,    "Should labels disappear when the attached creature dies?");
		public static Configurable<bool>    Objects     { get; } = bind(nameof(Objects    ), false,    "Should we show ID labels for objects?");
		public static Configurable<bool>    Spoilers    { get; } = bind(nameof(Spoilers   ), false,    "Show potential spoilers?");
		
		public static Configurable<string> Names { get; } = bind(nameof(Names), "");
	#endregion
	
	static Configurable<T> bind<T>(string name, T init) => Instance.config.Bind<T>($"{nameof(fish)}_{nameof(visibleid)}_{name}", init);
	static Configurable<T> bind<T>(string name, T init, string desc) => Instance.config.Bind<T>($"{nameof(fish)}_{nameof(visibleid)}_{name}", init, new ConfigurableInfo(desc));
	
	public override void Initialize()
		=> Tabs = new Menu.Remix.MixedUI.OpTab[] {
			new CfgTabMain(),
			new CfgTabNames(),
			new CfgTabInspect(),
		};
	
	public static void EarlySettingCleanup_RunASAP() {
		
	}
}
