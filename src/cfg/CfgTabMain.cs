namespace fish.rainworld.visibleid;
using Menu.Remix.MixedUI;

public class CfgTabMain: CfgTab {
	public CfgTabMain(): base("Main") {
		this._AddItem(new OpLabel(10f, 550f, "General Options", true));
		this.AddLabeledCheckbox(Cfg.ShowIDs,  new(20f, 390f));
		this.AddLabeledCheckbox(Cfg.Attrs,    new(20f, 350f));
		this.AddLabeledCheckbox(Cfg.Players,  new(20f, 310f));
		this.AddLabeledCheckbox(Cfg.PlyrAttr, new(20f, 270f));
		this.AddLabeledCheckbox(Cfg.Dead,     new(20f, 230f));
		this.AddLabeledCheckbox(Cfg.Objects,  new(20f, 190f));
		this.AddLabeledCheckbox(Cfg.Spoilers, new(20f, 150f));
	}
}
