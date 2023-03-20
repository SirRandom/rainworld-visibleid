namespace fish.rainworld.visibleid;
using Menu.Remix.MixedUI;

public class CfgTabKeybinds: CfgTab {
	public CfgTabKeybinds(): base("Keybinds")
		=> AddItems(
			new OpLabel(10f, 550f, "Keybindings", true),
			new OpLabel(20f, 500f, "Toggle ID Display", false),
			new OpKeyBinder(Cfg.ToggleID, new(250f, 492f), new(100f, 35f))
				{ description = "This button will toggle overhead ID & name labels" },
			new OpLabel(20f, 450f, "Toggle Personality & Traits Display", false),
			new OpKeyBinder(Cfg.ToggleStats, new(250f, 442f), new(100f, 35f))
				{ description = "This button will toggle the overhead personality & skills display" }
		);
}
