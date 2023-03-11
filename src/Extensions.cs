namespace fish.rainworld.visibleid;

static class Extensions {
	public static BepInEx.Logging.ManualLogSource Logger { private get; set; }
	public static void Info  (object o) => Logger.LogInfo    (o);
	public static void Warn  (object o) => Logger.LogWarning (o);
	public static void Error (object o) => Logger.LogError   (o);
	
	public static void AddLabeledCheckbox(this Menu.Remix.MixedUI.OpTab tab, Configurable<bool> setting, Vec2 pos, string desc = null)
		=> tab.AddItems(new Menu.Remix.MixedUI.UIelement[] {
			new Menu.Remix.MixedUI.OpCheckBox(setting, pos),
			new Menu.Remix.MixedUI.OpLabel(pos.x + 30f, pos.y + 3f, desc ?? setting.info.description, false),
		});
}

class Seeded: IDisposable {
	Rand.State tmpState;
	
	public Seeded(int seed) {
		tmpState = Rand.state;
		Rand.InitState(seed);
	}
	
	public void Dispose() => Rand.state = tmpState;
}