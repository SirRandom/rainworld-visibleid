namespace fish.Mods.RainWorld.VisibleID;

static class Hooks {
	public static void HookEverything() {
		On.PhysicalObject.Update += (o,s,eu) => { o(s,eu); SafeAttachOverheadID(s); };
		
		On.RainWorldGame.ExitGame    += (o,s, death, quit)  => { o(s, death, quit);  ClearLabels(); };
		On.RainWorldGame.ExitToMenu  += (o,s)               => { o(s);               ClearLabels(); };
		On.RainWorldGame.Win         += (o,s, malnourished, fromWarpPoint) => { o(s, malnourished, fromWarpPoint); ClearLabels(); };
		On.ArenaSitting.NextLevel    += (o,s, procmgr)      => { o(s, procmgr);      ClearLabels(); };
		On.ArenaSitting.SessionEnded += (o,s, session)      => { o(s, session);      ClearLabels(); };
		
		On.RainWorld.OnModsInit += (o,s) => { o(s); MachineConnector.SetRegisteredOI(VisibleID.Id, Cfg.Instance); };
		
		On.RainWorld.PostModsInit += (o,s) => { o(s); Cfg.EarlySetup_RunASAP(); };
	}
	
	static void ClearLabels() {
		foreach(var label in OverheadID.Instances.Values.ToList()) label.Destroy();
	}
	
	static void SafeAttachOverheadID(PhysicalObject self) {
		if(OverheadID.OkayToCreateNewOverheadIDs && self.room is not null && !OverheadID.Instances.ContainsKey(self))
			new OverheadID(self);
	}
}
