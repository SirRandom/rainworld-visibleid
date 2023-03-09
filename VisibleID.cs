global using System;
global using System.Collections.Generic;
global using System.Linq;

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
	public const string Version = "2.3";
	
	public static VisibleID Instance { get; private set; }
	
	public static System.Runtime.CompilerServices.ConditionalWeakTable<Creature, OverheadID> LabelsEx { get; } = new();
	
	public static Dictionary<Creature, OverheadID> Labels { get; } = new();
	public static Dictionary<(int, string), string> Names { get; } = new();
	
	static bool cfg_init = false;
	
	public void Awake() {
		Instance = this;
		Extensions.Logger = Logger;
		
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
				MachineConnector.SetRegisteredOI(Id, Cfg.Instance);
				cfg_init = true;
			}
		};
		
		On.RainWorld.PostModsInit += (o,s) => { o(s); ReloadNames(); };
		Info("Visible ID has initialized");
	}
	
	void ReloadNames() {
		Names.Clear();
			foreach(var e in Cfg.Names.Value.Split(';')) {
				var triplet = e.Split(':');
				if(int.TryParse(triplet[0], out int id))
					try {
						Names.Add((id, triplet[2]), triplet[1]);
					} catch(ArgumentException) {
						Warn($"Tried to populate {nameof(Names)} dictionary with (id,type) pair that already exists!");
					}
			}
	}
	
	public void Update() {
		if(Input.anyKeyDown) {
			if(Input.GetKeyDown(Cfg.ToggleID.Value)) {
				Cfg.ShowIDs.Value = !Cfg.ShowIDs.Value;
				Info($"{(Cfg.ShowIDs.Value? "SHOWING" : "HIDING")} IDS");
			}
			if(Input.GetKeyDown(Cfg.ToggleStats.Value)) {
				Cfg.Attrs.Value = !Cfg.Attrs.Value;
				Info($"{(Cfg.Attrs.Value? "SHOWING" : "HIDING")} PERSONALITIES & SKILLS");
			}
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
		static Configurable<int>    n_id   = new(Instance, null, 0, null);
		static Configurable<string> n_name = new(Instance, null, "", null);
		static Configurable<string> n_crea = new(Instance, null, "", null);
		
		static Configurable<T> bind<T>(string name, T init) => Instance.config.Bind<T>($"{nameof(fish)}_{nameof(visibleid)}_{name}", init);
		static Configurable<T> bind<T>(string name, T init, string desc) => Instance.config.Bind<T>($"{nameof(fish)}_{nameof(visibleid)}_{name}", init, new ConfigurableInfo(desc));
		
		public override void Initialize() {
			// switch(mod.version) {
			// 	case "0200":
			// 	case "0201":
			// 		break;
			// 	default: break;
			// }
			
			Tabs = new[] {
				new Menu.Remix.MixedUI.OpTab(this, "Main"),
				new Menu.Remix.MixedUI.OpTab(this, "Names"),
				//new Menu.Remix.MixedUI.OpTab(this, "Inspect"),
			};
			#region Tabs[0]
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
			#endregion
			#region Tabs[1]
				var bad_red     = new UnityEngine.Color(.85f, .35f, .4f);
				var tbx_id      = new Menu.Remix.MixedUI.OpTextBox(n_id, new(15f, 475f), 100f) { description = "Enter the creature ID that you wish to name" };
				var tbx_name    = new Menu.Remix.MixedUI.OpTextBox(n_name, new(125f, 475f), 200f) { allowSpace = true, description = "What should this creature be named?" };
				var cbx_crea    = new Menu.Remix.MixedUI.OpComboBox(n_crea, new(335f, 475f), 140f, new List<Menu.Remix.MixedUI.ListItem>(CreatureTemplate.Type.values.entries.Select(s => new Menu.Remix.MixedUI.ListItem(s)))) { description = "Select what type of creature this name should apply to" };
				var btn_add     = new Menu.Remix.MixedUI.OpSimpleButton(new(485f, 475f), new(40f, 24f), "add") { description = "Click to add the given name mapping" };
				var btn_del     = new Menu.Remix.MixedUI.OpSimpleButton(new(535f, 475f), new(40f, 24f), "del") { description = "Click to remove the given name mapping" };
				var btn_del_all = new Menu.Remix.MixedUI.OpHoldButton(new(515f, 515f), 10f, "del all") { description = "Click to remove all name mappings", colorEdge = bad_red, colorFill = bad_red };
				var namelist    = new Menu.Remix.MixedUI.OpScrollBox(new(20f, 20f), new(500f, 430f), 0f);
				
				var names_lbl = new Menu.Remix.MixedUI.OpLabelLong(new(10f, 10f), new(500f, 0f), "") {
					alignment = FLabelAlignment.Left,
					verticalAlignment = Menu.Remix.MixedUI.OpLabel.LabelVAlignment.Bottom,
				};
				var errs = new Menu.Remix.MixedUI.OpLabel(10f, 520f, "") {
					alignment = FLabelAlignment.Left,
					color = bad_red,
				};
				
				string label_text() => string.IsNullOrEmpty(Names.Value)? "No mappings created" : string.Join("\n", Names.Value.Split(';').Select(v => {
					var x = v.Split(':');
					return $"{x[0]}={x[1]} ({x[2]})";
				}));
				
				void UpdateNamesLabel() {
					names_lbl.text = label_text();
					namelist.SetContentSize(names_lbl.LineHeight * (2 + names_lbl.text.Count(c => c is '\n')));
				}
				
				void ClearInputBoxes() {
					tbx_id.value = "";
					tbx_name.value = "";
				}
				
				void SetError(string err) {
					errs.text = err;
					new System.Threading.Timer(_ => errs.text = "", null, Convert.ToInt32(TimeSpan.FromSeconds(7.0).TotalMilliseconds), System.Threading.Timeout.Infinite);
				}
				
				tbx_name.OnValueChanged += (s,newv,oldv) => {
					if(newv.Contains(':') || newv.Contains(';')) {
						s.value = oldv;
						SetError("Name cannot contain colons (:) or semicolons (;)");
					}
				};
				
				void AddName(int id, string name, string type) {
					if(!VisibleID.Names.ContainsKey((id, type))) {
						Names.Value += $";{id}:{name}:{type}";
						if(Names.Value[0] is ';') Names.Value = Names.Value.Substring(1);
						config.Save();
						VisibleID.Instance.ReloadNames();
					} else
						SetError($"Name already defined for {type} with id {id}");
				}
				
				void RemoveName(int id, string type) {
					var entries = Names.Value.Split(';');
					
					string tgt = null;
					foreach(var i in entries) {
						var fields = i.Split(':');
						if(int.TryParse(fields[0], out int i_id) && i_id == id && fields[2] == type) { tgt = i; break; }
					}
					
					if(tgt is not null) {
						var i = entries.IndexOf(tgt);
						Names.Value = string.Join(";", entries.Take(i).Concat(entries.Skip(i+1)));
						config.Save();
						VisibleID.Instance.ReloadNames();
					} else
						SetError($"No name found for {type} with id {id}");
				}
				
				btn_add.OnClick += t => {
					if(!string.IsNullOrEmpty(tbx_id.value) && int.TryParse(tbx_id.value, out int id) && !string.IsNullOrEmpty(tbx_name.value) && !string.IsNullOrEmpty(cbx_crea.value)) {
						AddName(id, tbx_name.value, cbx_crea.value);
						UpdateNamesLabel();
						ClearInputBoxes();
					} else
						SetError("To add a name, specify both the creature's id and type");
				};
				
				btn_del.OnClick += t => {
					if(int.TryParse(tbx_id.value, out int id)) {
						RemoveName(id, cbx_crea.value);
						UpdateNamesLabel();
						ClearInputBoxes();
					} else
						SetError("Creature id not parseable as integer");
				};
				
				btn_del_all.OnPressDone += t => {
					t.Menu.PlaySound(SoundID.MENU_Security_Button_Release);
					Names.Value = "";
					config.Save();
					UpdateNamesLabel();
					ClearInputBoxes();
				};
				
				Tabs[1].AddItems(new Menu.Remix.MixedUI.UIelement[] {
					new Menu.Remix.MixedUI.OpLabel(10f, 550f, "ID to Name Mapping", true),
					new Menu.Remix.MixedUI.OpLabel(30f, 500f, "ID", false),
					new Menu.Remix.MixedUI.OpLabel(140f, 500f, "Name", false),
					new Menu.Remix.MixedUI.OpLabel(350f, 500f, "Creature Type", false),
					namelist, errs, btn_del_all, tbx_id, tbx_name, btn_add, btn_del, names_lbl, cbx_crea,
				});
			#endregion
			// #region Tabs[2]
			// 	var tbx_2_id = new Menu.Remix.MixedUI.OpTextBox(n_id, new(15f, 475f), 100f);
			// 	var lbl_id = new Menu.Remix.MixedUI.OpLabel[6+5+1] {
			// 		new(30f, 500f, ""),
			// 		new(30f, 500f, ""),
			// 		new(30f, 500f, ""),
			// 		new(30f, 500f, ""),
			// 		new(30f, 500f, ""),
			// 		new(30f, 500f, ""),
			// 		new(30f, 500f, ""),
			// 		new(30f, 500f, ""),
			// 		new(30f, 500f, ""),
			// 		new(30f, 500f, ""),
			// 		new(30f, 500f, ""),
			// 		new(30f, 500f, ""),
			// 	};
				
			// 	void UpdateStatLabels() {
			// 		if(int.TryParse(tbx_2_id.value, out int id)) {
			// 			var eid = new EntityID(-1, id);
			// 			var personality = new AbstractCreature.Personality(eid);
						
			// 			lbl_id[ 0].text = $"ID: {id}";
			// 			lbl_id[ 1].text = $"Aggression (agg): {personality.aggression}";
			// 			lbl_id[ 2].text = $"Bravery (brv): {personality.bravery}";
			// 			lbl_id[ 3].text = $"Dominance (dom): {personality.dominance}";
			// 			lbl_id[ 4].text = $"Energy (nrg): {personality.energy}";
			// 			lbl_id[ 5].text = $"Nervousness (nrv): {personality.nervous}";
			// 			lbl_id[ 6].text = $"Sympathy (sym): {personality.sympathy}";
			// 			lbl_id[ 7].text = $"";
			// 			lbl_id[ 8].text = $"";
			// 			lbl_id[ 9].text = $"";
			// 			lbl_id[10].text = $"";
			// 			lbl_id[11].text = $"";
			// 		} else
			// 			foreach(var lbl in lbl_id) lbl.text = "";
			// 	}
				
			// 	tbx_2_id.OnChange += () => UpdateStatLabels();
				
			// 	Tabs[2].AddItems(new Menu.Remix.MixedUI.UIelement[] {
			// 		new Menu.Remix.MixedUI.OpLabel(10f, 550f, "Inspect an ID", true),
			// 		tbx_2_id,
			// 		lbl_id[ 0],
			// 		lbl_id[ 1],
			// 		lbl_id[ 2],
			// 		lbl_id[ 3],
			// 		lbl_id[ 4],
			// 		lbl_id[ 5],
			// 		lbl_id[ 6],
			// 		lbl_id[ 7],
			// 		lbl_id[ 8],
			// 		lbl_id[ 9],
			// 		lbl_id[10],
			// 		lbl_id[11],
			// 	});
			// #endregion
			
			names_lbl._AddToScrollBox(namelist);
			UpdateNamesLabel();
			ClearInputBoxes();
		}
	}
}
