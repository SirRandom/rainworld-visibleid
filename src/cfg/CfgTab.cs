namespace fish.Mods.RainWorld.VisibleID;
using Menu.Remix.MixedUI;

public abstract class CfgTab: OpTab {
	public CfgTab(string name): base(Cfg.Instance, name) {}
	protected static Configurable<T> CosmeticBind<T>(T init) => new(Cfg.Instance, null, init, null);
}
