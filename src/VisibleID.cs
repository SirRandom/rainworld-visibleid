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

global using static fish.rainworld.visibleid.Extensions;

#pragma warning disable CS0618
[assembly: System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace fish.rainworld.visibleid;

[BepInEx.BepInPlugin(Id, Name, Version)]
public class VisibleID: BepInEx.BaseUnityPlugin {
	public const string Id      = $"{nameof(fish)}.{nameof(visibleid)}";
	public const string Name    = "Visible ID";
	public const string Version = "2.6";
	
	public static VisibleID Instance { get; private set; }
	
	public static Dictionary<PhysicalObject, OverheadID> Labels { get; } = new();
	public static Dictionary<(int, string), string> Names { get; } = new();
	
	public void Awake() {
		Instance = this;
		Extensions.Logger = Logger;
		
		On.PhysicalObject.Update += (o,s,eu) => { o(s,eu);
			if((OverheadID.IDLabelVisible || OverheadID.StatsVisible) && s.room is not null && !Labels.ContainsKey(s)) {
				if(!Cfg.Objects.Value) {
					if(s is Creature)
						new OverheadID(s);
				} else
					new OverheadID(s);
			}
		};
		
		void ClearLabels() { foreach(var label in Labels.Values.ToList()) label.Destroy(); }
		On.RainWorldGame.ExitGame    += (o,s, death, quit)  => { o(s, death, quit);  ClearLabels(); };
		On.RainWorldGame.ExitToMenu  += (o,s)               => { o(s);               ClearLabels(); };
		On.RainWorldGame.Win         += (o,s, malnourished) => { o(s, malnourished); ClearLabels(); };
		On.ArenaSitting.NextLevel    += (o,s, procmgr)      => { o(s, procmgr);      ClearLabels(); };
		On.ArenaSitting.SessionEnded += (o,s, session)      => { o(s, session);      ClearLabels(); };
		
		On.RainWorld.OnModsInit += (o,s) => { o(s);
			MachineConnector.SetRegisteredOI(Id, Cfg.Instance);
		};
		
		On.RainWorld.PostModsInit += (o,s) => { o(s);
			ReloadNamesFromConfig();
			Info($"{Name} configuration version {Cfg.ConfigVersion.Value}");
			Cfg.EarlySettingCleanup_RunASAP();
			OverheadID.IDLabelVisible = Cfg.ShowIDs.Value;
			OverheadID.StatsVisible = Cfg.Attrs.Value;
		};
		
		Info("Visible ID has initialized");
	}
	
	public void ReloadNamesFromConfig() {
		Names.Clear();
		foreach(var e in Cfg.Names.Value.Split(';')) {
			var triplet = e.Split(':');
			if(int.TryParse(triplet[0], out int id)) {
				try {
					Names.Add((id, triplet[2]), triplet[1]);
				} catch(ArgumentException) {
					Warn($"Tried to populate {nameof(Names)} dictionary with (id,type) pair that already exists!");
				}
			}
		}
	}
	
	public void Update() {
		switch(Cfg.ToggleIDMode.Value) {
			case (int) Cfg.KeybindMode.Toggle:
				if(Input.anyKeyDown && Input.GetKeyDown(Cfg.ToggleID.Value)) {
					OverheadID.IDLabelVisible = !OverheadID.IDLabelVisible;
					Info($"{(OverheadID.IDLabelVisible? "SHOWING" : "HIDING")} IDS");
				}
				break;
			
			case (int) Cfg.KeybindMode.Held:
				bool previous_value = OverheadID.IDLabelVisible;
				OverheadID.IDLabelVisible = Input.GetKey(Cfg.ToggleID.Value);
				if(previous_value != OverheadID.IDLabelVisible)
					Info($"{(OverheadID.IDLabelVisible? "SHOWING" : "HIDING")} IDS");
				break;
			
			default:
				Error($"Invalid {nameof(Cfg.KeybindMode)} for {nameof(Cfg.ToggleIDMode)}; defaulting to {nameof(Cfg.KeybindMode.Toggle)}");
				Cfg.ToggleIDMode.Value = (int) Cfg.KeybindMode.Toggle;
				Cfg.Instance.config.Save();
				goto case (int) Cfg.KeybindMode.Toggle;
		}
		switch(Cfg.ToggleStatsMode.Value) {
			case (int) Cfg.KeybindMode.Toggle:
				if(Input.anyKeyDown && Input.GetKeyDown(Cfg.ToggleStats.Value)) {
					OverheadID.StatsVisible = !OverheadID.StatsVisible;
					Info($"{(OverheadID.StatsVisible? "SHOWING" : "HIDING")} PERSONALITIES & SKILLS");
				}
				break;
			
			case (int) Cfg.KeybindMode.Held:
				bool previous_value = OverheadID.StatsVisible;
				OverheadID.StatsVisible = Input.GetKey(Cfg.ToggleStats.Value);
				if(previous_value != OverheadID.StatsVisible)
					Info($"{(OverheadID.StatsVisible? "SHOWING" : "HIDING")} PERSONALITIES & SKILLS");
				break;
			
			default:
				Error($"Invalid {nameof(Cfg.KeybindMode)} for {nameof(Cfg.ToggleStatsMode)}; defaulting to {nameof(Cfg.KeybindMode.Toggle)}");
				Cfg.ToggleStatsMode.Value = (int) Cfg.KeybindMode.Toggle;
				Cfg.Instance.config.Save();
				goto case (int) Cfg.KeybindMode.Toggle;
		}
	}
}
