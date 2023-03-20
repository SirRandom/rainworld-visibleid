namespace fish.rainworld.visibleid;
using Menu.Remix.MixedUI;

public class CfgTabMain: OpTab {
	public CfgTabMain(): base(Cfg.Instance, "Main") {
		this.AddItems(
			new OpLabel(10f, 550f, "Visible ID Options", true),
			new OpLabel(20f, 500f, "Toggle ID Display", false),
			new OpKeyBinder(Cfg.ToggleID, new(250f, 492f), new(100f, 35f))
				{ description = "This button will toggle overhead ID & name labels" },
			new OpLabel(20f, 450f, "Toggle Personality & Traits Display", false),
			new OpKeyBinder(Cfg.ToggleStats, new(250f, 442f), new(100f, 35f))
				{ description = "This button will toggle the overhead personality & skills display" }
		);
		this.AddLabeledCheckbox(Cfg.ShowIDs,  new(20f, 390f));
		this.AddLabeledCheckbox(Cfg.Attrs,    new(20f, 350f));
		this.AddLabeledCheckbox(Cfg.Players,  new(20f, 310f));
		this.AddLabeledCheckbox(Cfg.PlyrAttr, new(20f, 270f));
		this.AddLabeledCheckbox(Cfg.Dead,     new(20f, 230f));
		this.AddLabeledCheckbox(Cfg.Objects,  new(20f, 190f));
		this.AddLabeledCheckbox(Cfg.Spoilers, new(20f, 150f));
	}
}
