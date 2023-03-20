namespace fish.rainworld.visibleid;
using Menu.Remix.MixedUI;

public partial class Cfg {
	OpTab InitializeMainTab() {
		OpTab tab = new(this, "Main");
		
		tab.AddItems(
			new OpLabel(10f, 550f, "Visible ID Options", true),
			new OpLabel(20f, 500f, "Toggle ID Display", false),
			new OpKeyBinder(ToggleID, new(250f, 492f), new(100f, 35f))
				{ description = "This button will toggle overhead ID & name labels" },
			new OpLabel(20f, 450f, "Toggle Personality & Traits Display", false),
			new OpKeyBinder(ToggleStats, new(250f, 442f), new(100f, 35f))
				{ description = "This button will toggle the overhead personality & skills display" }
		);
		tab.AddLabeledCheckbox(ShowIDs,  new(20f, 390f));
		tab.AddLabeledCheckbox(Attrs,    new(20f, 350f));
		tab.AddLabeledCheckbox(Players,  new(20f, 310f));
		tab.AddLabeledCheckbox(PlyrAttr, new(20f, 270f));
		tab.AddLabeledCheckbox(Dead,     new(20f, 230f));
		tab.AddLabeledCheckbox(Objects,  new(20f, 190f));
		tab.AddLabeledCheckbox(Spoilers, new(20f, 150f));
		
		return tab;
	}
}
