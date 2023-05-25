global using System;
global using System.Collections.Generic;
global using System.Linq;

global using UnityEngine;

global using Custom = RWCustom.Custom;
global using Rand = UnityEngine.Random;
global using Input = UnityEngine.Input;
global using KeyCode = UnityEngine.KeyCode;
global using Keys = UnityEngine.KeyCode;
global using Vec2 = UnityEngine.Vector2;

global using static fish.Mods.RainWorld.VisibleID.Extensions;

#pragma warning disable CS0618
[assembly: System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace fish.Mods.RainWorld.VisibleID;

[BepInEx.BepInPlugin(Id, Name, Version)]
public class VisibleID: BepInEx.BaseUnityPlugin {
	public const string Id      = "fish.visibleid";
	public const string Name    = "Visible ID";
	public const string Version = "2.7";
	
	public static VisibleID Instance { get; private set; }
	
	public VisibleID() {
		Instance = this;
		Extensions.Logger = Logger;
	}
	
	public void Awake() {
		Hooks.HookEverything();
		Info("Visible ID has initialized");
	}
	
	public void Update() {
		InterpretKeyBind(Cfg.ToggleIDMode,    Cfg.ToggleID,    ref OverheadID.CreatureIDLabelVisible);
		InterpretKeyBind(Cfg.ToggleObjIDMode, Cfg.ToggleObjID, ref OverheadID.ObjectIDLabelVisible);
		InterpretKeyBind(Cfg.ToggleStatsMode, Cfg.ToggleStats, ref OverheadID.StatsVisible);
	}
	
	void InterpretKeyBind(Configurable<int> mode, Configurable<KeyCode> key, ref bool target) {
		switch(mode.Value) {
			case (int) KeybindMode.Toggle:
				if(Input.anyKeyDown && Input.GetKeyDown(key.Value))
					target = !target;
				break;
			
			case (int) KeybindMode.Held:
				target = Input.GetKey(key.Value);
				break;
			
			default:
				Error($"Invalid {nameof(KeybindMode)} for bound key {key}; defaulting to {nameof(KeybindMode.Toggle)}");
				mode.Value = (int) KeybindMode.Toggle;
				Cfg.Save();
				goto case (int) KeybindMode.Toggle;
		}
	}
}
