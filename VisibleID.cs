global using System;
global using System.Collections.Generic;
global using System.Linq;

global using Input = UnityEngine.Input;
global using KeyCode = UnityEngine.KeyCode;
global using Keys = UnityEngine.KeyCode;
global using Vec2 = UnityEngine.Vector2;
global using Dbg = UnityEngine.Debug;

#pragma warning disable CS0618
[assembly: System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace fish.rainworld.visibleid;

static class Extensions {
	public static void AddLabeledCheckbox(this Menu.Remix.MixedUI.OpTab tab, Configurable<bool> setting, Vec2 pos, string desc = null)
		=> tab.AddItems(new Menu.Remix.MixedUI.UIelement[] {
			new Menu.Remix.MixedUI.OpCheckBox(setting, pos),
			new Menu.Remix.MixedUI.OpLabel(pos.x + 30f, pos.y + 3f, desc ?? setting.info.description, false),
		});
}

[BepInEx.BepInPlugin(ModId, "Visible ID", "2.1")]
public class VisibleID: BepInEx.BaseUnityPlugin {
	public const string ModId = $"{nameof(fish)}.{nameof(visibleid)}";
	
	public static VisibleID Instance { get; private set; }
	
	public static Dictionary<Creature, OverheadID> Labels { get; } = new();
	public static Dictionary<(int, string), string> Names { get; } = new();
	
	static bool cfg_init = false;
	
	public void Awake() {
		Instance = this;
		
		On.Creature.Update += (o,s, eu) => { o(s, eu);
			if((Cfg.ShowIDs.Value || Cfg.Attrs.Value) && s.room is not null && !Labels.ContainsKey(s)) new OverheadID(s);
		};
		
		void ClearLabels() { foreach(var label in Labels.Values.ToList()) label.Destroy(); }
		On.RainWorldGame.ExitGame    += (o,s, death, quit)  => { o(s, death, quit);  ClearLabels(); };
		On.RainWorldGame.ExitToMenu  += (o,s)               => { o(s);               ClearLabels(); };
		On.RainWorldGame.Win         += (o,s, malnourished) => { o(s, malnourished); ClearLabels(); };
		On.ArenaSitting.NextLevel    += (o,s, procmgr)      => { o(s, procmgr);      ClearLabels(); };
		On.ArenaSitting.SessionEnded += (o,s, session)      => { o(s, session);      ClearLabels(); };
		
		On.RainWorld.OnModsInit += (o,s) => { o(s);
			if(!cfg_init) {
				MachineConnector.SetRegisteredOI(ModId, Cfg.Instance);
				cfg_init = true;
			}
		};
		
		On.RainWorld.PostModsInit += (o,s) => { o(s); ReloadNames(); };
	}
	
	void ReloadNames() {
		Names.Clear();
			foreach(var e in Cfg.Names.Value.Split(';')) {
				var triplet = e.Split(':');
				if(int.TryParse(triplet[0], out int id))
					try {
						Names.Add((id, triplet[2]), triplet[1]);
					} catch(ArgumentException) {
						Logger.LogWarning($"Tried to populate {nameof(Names)} dictionary with (id,type) pair that already exists!");
					}
			}
	}
	
	public void Update() {
		if(Input.GetKeyDown(Cfg.ToggleID.Value)) {
			Cfg.ShowIDs.Value = !Cfg.ShowIDs.Value;
			Logger.LogInfo($"{(Cfg.ShowIDs.Value? "SHOWING" : "HIDING")} IDS");
		}
		if(Input.GetKeyDown(Cfg.ToggleStats.Value)) {
			Cfg.Attrs.Value = !Cfg.Attrs.Value;
			Logger.LogInfo($"{(Cfg.Attrs.Value? "SHOWING" : "HIDING")} PERSONALITIES & SKILLS");
		}
	}
	
	public class Cfg: OptionInterface {
		Cfg() {}
		public static Cfg Instance { get; } = new();
		public static Configurable<KeyCode> ToggleID    { get; } = bind(nameof(ToggleID   ), Keys.Tab, "The key that should toggle ID display on and off");
		public static Configurable<KeyCode> ToggleStats { get; } = bind(nameof(ToggleStats), Keys.End, "The key that should toggle personality & traits display on and off");
		public static Configurable<bool>    ShowIDs     { get; } = bind(nameof(ShowIDs    ), false,    "Should ID labels be on at the start of the game?");
		public static Configurable<bool>    Attrs       { get; } = bind(nameof(Attrs      ), false,    "Should the personality & skills readout be on at the start of the game?");
		public static Configurable<bool>    Players     { get; } = bind(nameof(Players    ), true,     "Should we show ID labels for players?");
		public static Configurable<bool>    PlyrAttr    { get; } = bind(nameof(PlyrAttr   ), false,    "Should we show personality traits for players?");
		public static Configurable<bool>    Dead        { get; } = bind(nameof(Dead       ), false,    "Should labels disappear when the attached creature dies?");
		
		public static Configurable<string> Names { get; } = bind(nameof(Names), "");
		
		static Configurable<string> n_lbl  = new(Instance, null, "", null);
		static Configurable<string> n_id   = new(Instance, null, "", null);
		static Configurable<string> n_name = new(Instance, null, "", null);
		static Configurable<string> n_crea = new(Instance, null, "", null);
		
		static Configurable<T> bind<T>(string name, T init) => Instance.config.Bind<T>($"{nameof(fish)}_{nameof(visibleid)}_{name}", init);
		static Configurable<T> bind<T>(string name, T init, string desc) => Instance.config.Bind<T>($"{nameof(fish)}_{nameof(visibleid)}_{name}", init, new ConfigurableInfo(desc));
		
		void AddName(int id, string name, string type) {
			if(!VisibleID.Names.ContainsKey((id, type))) {
				Names.Value += $";{id}:{name}:{type}";
				if(Names.Value[0] is ';') Names.Value = Names.Value.Substring(1);
				config.Save();
				VisibleID.Instance.ReloadNames();
			}
		}
		
		void RemoveName(int id) {
			var entries = Names.Value.Split(';');
			
			string tgt = null;
			foreach(var i in entries) {
				var fields = i.Split(':');
				if(int.TryParse(fields[0], out int i_id) && i_id == id) { tgt = i; break; }
			}
			
			if(tgt is not null) {
				var i = entries.IndexOf(tgt);
				Names.Value = string.Join(";", entries.Take(i).Concat(entries.Skip(i+1)));
				config.Save();
				VisibleID.Instance.ReloadNames();
			}
		}
		
		public override void Initialize() {
			Tabs = new[] {
				new Menu.Remix.MixedUI.OpTab(this, "Main"),
				new Menu.Remix.MixedUI.OpTab(this, "Names"),
			};
			Tabs[0].AddItems(new Menu.Remix.MixedUI.UIelement[] {
				new Menu.Remix.MixedUI.OpLabel(10f, 550f, "Visible ID Options", true),
				new Menu.Remix.MixedUI.OpLabel(20f, 500f, "Toggle ID Display", false),
				new Menu.Remix.MixedUI.OpKeyBinder(ToggleID, new(250f, 492f), new(100f, 35f)),
				new Menu.Remix.MixedUI.OpLabel(20f, 450f, "Toggle Personality & Traits Display", false),
				new Menu.Remix.MixedUI.OpKeyBinder(ToggleStats, new(250f, 442f), new(100f, 35f)),
			});
			Tabs[0].AddLabeledCheckbox(ShowIDs,  new(20f, 390f));
			Tabs[0].AddLabeledCheckbox(Attrs,    new(20f, 350f));
			Tabs[0].AddLabeledCheckbox(Players,  new(20f, 310f));
			Tabs[0].AddLabeledCheckbox(PlyrAttr, new(20f, 270f));
			Tabs[0].AddLabeledCheckbox(Dead,     new(20f, 230f));
			
			var tbx_id   = new Menu.Remix.MixedUI.OpTextBox(n_id, new(15f, 475f), 100f);
			var tbx_name = new Menu.Remix.MixedUI.OpTextBox(n_name, new(125f, 475f), 200f);
			var cbx_crea = new Menu.Remix.MixedUI.OpComboBox(n_crea, new(335f, 475f), 140f, new List<Menu.Remix.MixedUI.ListItem>(CreatureTemplate.Type.values.entries.Select(s => new Menu.Remix.MixedUI.ListItem(s))));
			var btn_add = new Menu.Remix.MixedUI.OpSimpleButton(new(485f, 475f), new(40f, 24f), "add");
			var btn_del = new Menu.Remix.MixedUI.OpSimpleButton(new(535f, 475f), new(40f, 24f), "del");
			var names_lbl = new Menu.Remix.MixedUI.OpLabel(35f, 445f, label_text(), false) {
				alignment = FLabelAlignment.Left,
				verticalAlignment = Menu.Remix.MixedUI.OpLabel.LabelVAlignment.Top,
			};
			
			string label_text() => string.IsNullOrEmpty(Names.Value)? "No mappings created" : string.Join("\n", Names.Value.Split(';').Select(v => {
				var x = v.Split(':');
				return $"{x[0]}={x[1]} ({x[2]})";
			}));
			
			btn_add.OnClick += t => {
				if(!string.IsNullOrEmpty(tbx_id.value) && int.TryParse(tbx_id.value, out int id) && !string.IsNullOrEmpty(tbx_name.value) && !string.IsNullOrEmpty(cbx_crea.value)) {
					AddName(id, tbx_name.value, cbx_crea.value);
					names_lbl.text = label_text();
				}
			};
			
			btn_del.OnClick += t => {
				if(int.TryParse(tbx_id.value, out int id)) {
					RemoveName(id);
					names_lbl.text = label_text();
				}
			};
			
			Tabs[1].AddItems(new Menu.Remix.MixedUI.UIelement[] {
				new Menu.Remix.MixedUI.OpLabel(10f, 550f, "ID to Name Mapping", true),
				new Menu.Remix.MixedUI.OpLabel(30f, 500f, "ID", false),
				new Menu.Remix.MixedUI.OpLabel(140f, 500f, "Name", false),
				new Menu.Remix.MixedUI.OpLabel(350f, 500f, "Creature Type", false),
				tbx_id, tbx_name, btn_add, btn_del, names_lbl, cbx_crea,
			});
		}
	}
}

public class OverheadID: CosmeticSprite {
	bool scav;
	Creature creature;
	
	   int ID => creature.abstractCreature.ID.number;
	string Type => creature.abstractCreature.creatureTemplate.type.value;
	 float agg => creature.abstractCreature.personality.aggression;
	 float brv => creature.abstractCreature.personality.bravery;
	 float dom => creature.abstractCreature.personality.dominance;
	 float nrg => creature.abstractCreature.personality.energy;
	 float nrv => creature.abstractCreature.personality.nervous;
	 float sym => creature.abstractCreature.personality.sympathy;
	 float dge => creature is Scavenger s? s.dodgeSkill    : 0f;
	 float mid => creature is Scavenger s? s.midRangeSkill : 0f;
	 float mle => creature is Scavenger s? s.meleeSkill    : 0f;
	 float blk => creature is Scavenger s? s.blockingSkill : 0f;
	 float rea => creature is Scavenger s? s.reactionSkill : 0f;
	
	public OverheadID(Creature c) {
		scav = c is Scavenger;
		(creature = c).room.AddObject(this);
		VisibleID.Labels.Add(c, this);
	}
	
	public override void Update(bool eu) {
		if(creature.room is not null) {
			if(room != creature.room) {
				room.RemoveObject(this);
				creature.room.AddObject(this);
			}
		}
		base.Update(eu);
	}
	
	public override void ApplyPalette(RoomCamera.SpriteLeaser leaser, RoomCamera cam, RoomPalette pal) => leaser.sprites[0].color = UnityEngine.Color.black;
	
	public override void InitiateSprites(RoomCamera.SpriteLeaser leaser, RoomCamera cam) {
		var bg = new FSprite("pixel") { scaleX = 44f, scaleY = 15f, };
		FContainer top = new();
		
		leaser.sprites    = new[] { bg  };
		leaser.containers = new[] { top };
		
		FLabel Lbl(string text, UnityEngine.Color color, Vec2 pos, float scale = 1f) => new FLabel("DisplayFont", text) { anchorX = .5f, scale = scale, color = color, x = pos.x, y = pos.y };
		
		FContainer lbl = new();
			lbl.AddChild(bg);
			lbl.AddChild(new FLabel("DisplayFont", $"{ID}") { anchorX = .5f, scale = .75f, y = +1f });
		top.AddChild(lbl);
		
		FContainer attr   = new() { y = -14f };
		FContainer attr_a = new() { y = +4f };
		FContainer attr_b = new() { y = -4f };
			attr_a.AddChild(Lbl($"agg",      new(255,60,0),  new(-57.5f,0f), 0.5f));
			attr_a.AddChild(Lbl($"brv",      new(100,0,180), new(-34.5f,0f), 0.5f));
			attr_a.AddChild(Lbl($"dom",      new(255,0,150), new(-11.5f,0f), 0.5f));
			attr_a.AddChild(Lbl($"nrg",      new(255,255,0), new(+11.5f,0f), 0.5f));
			attr_a.AddChild(Lbl($"nrv",      new(0,80,190),  new(+34.5f,0f), 0.5f));
			attr_a.AddChild(Lbl($"sym",      new(0,255,110), new(+57.5f,0f), 0.5f));
		attr.AddChild(attr_a);
			attr_b.AddChild(Lbl($"{agg:F2}", new(255,60,0),  new(-57.5f,0f), 0.5f));
			attr_b.AddChild(Lbl($"{brv:F2}", new(100,0,180), new(-34.5f,0f), 0.5f));
			attr_b.AddChild(Lbl($"{dom:F2}", new(255,0,150), new(-11.5f,0f), 0.5f));
			attr_b.AddChild(Lbl($"{nrg:F2}", new(255,255,0), new(+11.5f,0f), 0.5f));
			attr_b.AddChild(Lbl($"{nrv:F2}", new(0,80,190),  new(+34.5f,0f), 0.5f));
			attr_b.AddChild(Lbl($"{sym:F2}", new(0,255,110), new(+57.5f,0f), 0.5f));
		attr.AddChild(attr_b);
		top.AddChild(attr);
		
		FContainer stat   = new() { y = -28f };
		FContainer stat_a = new() { y = +4f };
		FContainer stat_b = new() { y = -4f };
			stat_a.AddChild(Lbl($"dge",      new(255,255,255), new(-46f,0f), 0.5f));
			stat_a.AddChild(Lbl($"mid",      new(255,255,255), new(-23f,0f), 0.5f));
			stat_a.AddChild(Lbl($"mle",      new(255,255,255), new(+ 0f,0f), 0.5f));
			stat_a.AddChild(Lbl($"blk",      new(255,255,255), new(+23f,0f), 0.5f));
			stat_a.AddChild(Lbl($"rea",      new(255,255,255), new(+46f,0f), 0.5f));
		stat.AddChild(stat_a);
			stat_b.AddChild(Lbl($"{dge:F2}", new(255,255,255), new(-46f,0f), 0.5f));
			stat_b.AddChild(Lbl($"{mid:F2}", new(255,255,255), new(-23f,0f), 0.5f));
			stat_b.AddChild(Lbl($"{mle:F2}", new(255,255,255), new(+ 0f,0f), 0.5f));
			stat_b.AddChild(Lbl($"{blk:F2}", new(255,255,255), new(+23f,0f), 0.5f));
			stat_b.AddChild(Lbl($"{rea:F2}", new(255,255,255), new(+46f,0f), 0.5f));
		stat.AddChild(stat_b);
		top.AddChild(stat);
		
		cam.ReturnFContainer("Foreground").AddChild(top);
	}
	
	public override void DrawSprites(RoomCamera.SpriteLeaser leaser, RoomCamera cam, float time, Vec2 campos) {
		var top  = leaser.containers[0];
		var lbl  = top.GetChildAt(0) as FContainer;
		var attr = top.GetChildAt(1) as FContainer;
		var stat = top.GetChildAt(2) as FContainer;
		
		void hide() => top.isVisible = false;
		void show() => top.isVisible = true;
		
		if(room is null
		|| !room.BeingViewed
		|| creature.room is null
		|| !creature.room.BeingViewed
		|| (VisibleID.Cfg.Dead.Value && creature.dead)
		|| (!VisibleID.Cfg.ShowIDs.Value && !VisibleID.Cfg.Attrs.Value)
		|| (creature is Overseer ovr && (ovr.mode == Overseer.Mode.SittingInWall || ovr.mode == Overseer.Mode.Withdrawing || ovr.mode == Overseer.Mode.Zipping))
		|| (creature is Fly fly && fly.BitesLeft is 0)
		|| (creature is Player ply && !ply.isNPC && (!VisibleID.Cfg.Players.Value && !VisibleID.Cfg.PlyrAttr.Value))) {
			hide();
		} else {
			show();
			
			Vec2 t = Vec2.Lerp(creature.mainBodyChunk.lastPos, creature.mainBodyChunk.pos, time) - campos;
			(top.x, top.y) = (t.x, t.y + 53f);
			
			lbl.isVisible = VisibleID.Cfg.ShowIDs.Value;
			attr.isVisible = VisibleID.Cfg.Attrs.Value;
			stat.isVisible = VisibleID.Cfg.Attrs.Value && scav;
			
			if(creature is Player p && !p.isNPC) {
				lbl.isVisible = lbl.isVisible && VisibleID.Cfg.PlyrAttr.Value;
				attr.isVisible = attr.isVisible && VisibleID.Cfg.PlyrAttr.Value;
			}
			
			if(attr.isVisible) {
				if(stat.isVisible) {
					attr.y = -14f;
					stat.y = -28f;
				} else attr.y = -14f;
			} else {
				if(stat.isVisible) stat.y = -14f;
			}
			
			if(lbl.isVisible) {
				var bg = lbl.GetChildAt(0) as FSprite;
				var id = lbl.GetChildAt(1) as FLabel;
				
				if(VisibleID.Names.TryGetValue((ID, Type), out string name)) id.text = name;
				
				float width = id.textRect.xMax - id.textRect.xMin;
				bg.scaleX = width + 10f;
			}
		}
		
		base.DrawSprites(leaser, cam, time, campos);
	}
	
	public override void Destroy() {
		RemoveFromRoom();
		VisibleID.Labels.Remove(creature);
		Dbg.Log($"Active label count {VisibleID.Labels.Count}");
		base.Destroy();
	}
}
