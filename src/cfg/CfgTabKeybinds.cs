namespace fish.rainworld.visibleid;
using Menu.Remix.MixedUI;

public class CfgTabKeybinds: CfgTab {
	public CfgTabKeybinds(): base("Keybinds") {
		AddItems(
			new OpLabel(10f, 550f, "Keybindings", true),
			new OpLabel(350f, 530f, "Toggle"),
			new OpLabel(430f, 530f, "While Held"),
			new OpLabel(20f, 500f, "Show ID Labels", false),
			new OpKeyBinder(Cfg.ToggleID, new(210f, 492f), new(100f, 35f))
				{ description = "This button will toggle overhead ID & name labels" },
			new OpRadioButtonGroupEx(Cfg.ToggleIDMode, new RadioPair(this, new(350f, 500f))),
			new OpLabel(20f, 450f, "Show Personality & Traits", false),
			new OpKeyBinder(Cfg.ToggleStats, new(210f, 442f), new(100f, 35f))
				{ description = "This button will toggle the overhead personality & skills display" },
			new OpRadioButtonGroupEx(Cfg.ToggleStatsMode, new RadioPair(this, new(350f, 450f)))
		);
	}
}

file class OpRadioButtonGroupEx: Menu.Remix.MixedUI.OpRadioButtonGroup {
	public OpRadioButtonGroupEx(Configurable<int> configurable, params Menu.Remix.MixedUI.OpRadioButton[] buttons): base(configurable) => SetButtons(buttons);
}

file class RadioPair {
	OpRadioButton[] btns = new OpRadioButton[2];
	
	public RadioPair(CfgTabKeybinds parent, Vec2 pos)
		=> parent.AddItems(
			btns[0] = new OpRadioButton(pos),
			btns[1] = new OpRadioButton(pos.x+80f, pos.y)
		);
	
	public static implicit operator OpRadioButton[] (RadioPair o) => o.btns;
}
