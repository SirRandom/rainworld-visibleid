namespace fish.Mods.RainWorld.VisibleID;
using Menu.Remix.MixedUI;

public class CfgTabMain: CfgTab {
	public CfgTabMain(): base("Main") {
		this._AddItem(new OpLabel(10f, 550f, "General Options", true));
		this.AddLabeledCheckbox(Cfg.ShowIDs,  new(20f, 490f));
		this.AddLabeledCheckbox(Cfg.Attrs,    new(20f, 450f));
		this.AddLabeledCheckbox(Cfg.Players,  new(20f, 410f));
		this.AddLabeledCheckbox(Cfg.PlyrAttr, new(20f, 370f));
		this.AddLabeledCheckbox(Cfg.Dead,     new(20f, 330f));
		this.AddLabeledCheckbox(Cfg.Objects,  new(20f, 290f));
		this.AddLabeledCheckbox(Cfg.Spoilers, new(20f, 250f));
	}
}
