namespace fish.rainworld.visibleid;

public class Cfg: OptionInterface {
	Cfg() {}
	public static Cfg Instance { get; } = new();
	
	#region Actual Configurables
		public static Configurable<KeyCode> ToggleID    { get; } = bind(nameof(ToggleID   ), Keys.Tab, "The key that should toggle ID display on and off");
		public static Configurable<KeyCode> ToggleStats { get; } = bind(nameof(ToggleStats), Keys.End, "The key that should toggle personality & traits display on and off");
		public static Configurable<bool>    ShowIDs     { get; } = bind(nameof(ShowIDs    ), false,    "Should ID labels be on at the start of the game?");
		public static Configurable<bool>    Attrs       { get; } = bind(nameof(Attrs      ), false,    "Should the personality & skills readout be on at the start of the game?");
		public static Configurable<bool>    Players     { get; } = bind(nameof(Players    ), true,     "Should we show ID labels for players?");
		public static Configurable<bool>    PlyrAttr    { get; } = bind(nameof(PlyrAttr   ), false,    "Should we show personality traits for players?");
		public static Configurable<bool>    Dead        { get; } = bind(nameof(Dead       ), false,    "Should labels disappear when the attached creature dies?");
		
		public static Configurable<string> Names { get; } = bind(nameof(Names), "");
	#endregion
	#region Cosmetic Configurables
		static Configurable<string> n0_lbl  = new(Instance, null, "", null);
		static Configurable<int>    n0_id   = new(Instance, null,  0, null);
		static Configurable<string> n0_name = new(Instance, null, "", null);
		static Configurable<string> n0_crea = new(Instance, null, "", null);
		
		static Configurable<int> n1_id = new(Instance, null, 0, null);
	#endregion
	
	static Configurable<T> bind<T>(string name, T init) => Instance.config.Bind<T>($"{nameof(fish)}_{nameof(visibleid)}_{name}", init);
	static Configurable<T> bind<T>(string name, T init, string desc) => Instance.config.Bind<T>($"{nameof(fish)}_{nameof(visibleid)}_{name}", init, new ConfigurableInfo(desc));
	
	public override void Initialize() {
		Tabs = new[] {
			new Menu.Remix.MixedUI.OpTab(this, "Main"),
			new Menu.Remix.MixedUI.OpTab(this, "Names"),
			new Menu.Remix.MixedUI.OpTab(this, "Inspect"),
		};
		#region Tabs[0]
			Tabs[0].AddItems(new Menu.Remix.MixedUI.UIelement[] {
				new Menu.Remix.MixedUI.OpLabel(10f, 550f, "Visible ID Options", true),
				new Menu.Remix.MixedUI.OpLabel(20f, 500f, "Toggle ID Display", false),
				new Menu.Remix.MixedUI.OpKeyBinder(ToggleID, new(250f, 492f), new(100f, 35f)) { description = "This button will toggle overhead ID & name labels" },
				new Menu.Remix.MixedUI.OpLabel(20f, 450f, "Toggle Personality & Traits Display", false),
				new Menu.Remix.MixedUI.OpKeyBinder(ToggleStats, new(250f, 442f), new(100f, 35f)) { description = "This button will toggle the overhead personality & skills display" },
			});
			Tabs[0].AddLabeledCheckbox(ShowIDs,  new(20f, 390f));
			Tabs[0].AddLabeledCheckbox(Attrs,    new(20f, 350f));
			Tabs[0].AddLabeledCheckbox(Players,  new(20f, 310f));
			Tabs[0].AddLabeledCheckbox(PlyrAttr, new(20f, 270f));
			Tabs[0].AddLabeledCheckbox(Dead,     new(20f, 230f));
		#endregion
		#region Tabs[1]
			var bad_red     = new UnityEngine.Color(.85f, .35f, .4f);
			var tbx_id      = new Menu.Remix.MixedUI.OpTextBox(n0_id, new(15f, 475f), 100f) { description = "Enter the creature ID that you wish to name" };
			var tbx_name    = new Menu.Remix.MixedUI.OpTextBox(n0_name, new(125f, 475f), 200f) { allowSpace = true, description = "What should this creature be named?" };
			var cbx_crea    = new Menu.Remix.MixedUI.OpComboBox(n0_crea, new(335f, 475f), 140f, new List<Menu.Remix.MixedUI.ListItem>(CreatureTemplate.Type.values.entries.Select(s => new Menu.Remix.MixedUI.ListItem(s)))) { description = "Select what type of creature this name should apply to" };
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
		#region Tabs[2]
			var tbx_2_id = new Menu.Remix.MixedUI.OpTextBox(n1_id, new(80f, 520f), 100f) { description = "Type an ID number to inspect its stats" };
			var inspect_lbls = new Menu.Remix.MixedUI.OpLabel[] {
				new(10f, 320f, "Type in the field to show stats for a given ID number"),
				new(170f, 460f, "0"), new(170f, 440f, "0"), new(170f, 420f, "0"), new(170f, 400f, "0"), new(170f, 380f, "0"), new(170f, 360f, "0"),
				new(445f, 460f, "0"), new(445f, 440f, "0"), new(445f, 420f, "0"), new(445f, 400f, "0"), new(445f, 380f, "0"),
				new(30f, 523f, "ID entry"),
				new(10f, 480f, "Personality Traits", true),
				new(300f, 480f, "Scavenger Skills", true),
				new(30f, 460f, "(agg)"), new(70f, 460f, "Aggression:"),
				new(30f, 440f, "(brv)"), new(70f, 440f, "Bravery:"),
				new(30f, 420f, "(dom)"), new(70f, 420f, "Dominance:"),
				new(30f, 400f, "(nrg)"), new(70f, 400f, "Energy:"),
				new(30f, 380f, "(nrv)"), new(70f, 380f, "Nervousness:"),
				new(30f, 360f, "(sym)"), new(70f, 360f, "Sympathy:"),
				new(320f, 460f, "(dge)"), new(355f, 460f, "Dodge:"),
				new(320f, 440f, "(mid)"), new(355f, 440f, "Mid-Range:"),
				new(320f, 420f, "(mle)"), new(355f, 420f, "Melee:"),
				new(320f, 400f, "(blk)"), new(355f, 400f, "Blocking:"),
				new(320f, 380f, "(rea)"), new(355f, 380f, "Reaction:"),
			};
			
			tbx_2_id.OnChange += () => {
				if(int.TryParse(tbx_2_id.value, out int id)) {
					var eid = new EntityID(-1, id);
					var personality = new AbstractCreature.Personality(eid);
					
					float dge, mid, mle, blk, rea;
					using(new Seeded(eid.RandomSeed)) {
						dge = Custom.PushFromHalf(Mathf.Lerp((Rand.value >= 0.5f) ? personality.aggression : personality.nervous, Rand.value, Rand.value), 1f + Rand.value);
						mid = Custom.PushFromHalf(Mathf.Lerp((Rand.value >= 0.5f) ? personality.aggression : personality.energy,  Rand.value, Rand.value), 1f + Rand.value);
						mle = Custom.PushFromHalf(Rand.value, 1f + Rand.value);
						blk = Custom.PushFromHalf(Mathf.InverseLerp(0.35f, 1f, Mathf.Lerp((Rand.value >= 0.5f) ? personality.energy : personality.bravery, Rand.value, Rand.value)), 1f + Rand.value);
						rea = Custom.PushFromHalf(Mathf.Lerp(personality.energy, Rand.value, Rand.value), 1f + Rand.value);
					}
					
					inspect_lbls[ 0].text = $"Showing stats for ID {id}";
					inspect_lbls[ 1].text = $"{personality.aggression}";
					inspect_lbls[ 2].text = $"{personality.bravery}";
					inspect_lbls[ 3].text = $"{personality.dominance}";
					inspect_lbls[ 4].text = $"{personality.energy}";
					inspect_lbls[ 5].text = $"{personality.nervous}";
					inspect_lbls[ 6].text = $"{personality.sympathy}";
					inspect_lbls[ 7].text = $"{dge}";
					inspect_lbls[ 8].text = $"{mid}";
					inspect_lbls[ 9].text = $"{mle}";
					inspect_lbls[10].text = $"{blk}";
					inspect_lbls[11].text = $"{rea}";
				} else
					foreach(var lbl in inspect_lbls.Take(12)) lbl.text = "";
			};
			
			Tabs[2].AddItems(new Menu.Remix.MixedUI.UIelement[] {
				new Menu.Remix.MixedUI.OpLabel(10f, 550f, "Inspect an ID", true),
				tbx_2_id,
			});
			Tabs[2].AddItems(inspect_lbls);
		#endregion
		
		names_lbl._AddToScrollBox(namelist);
		UpdateNamesLabel();
		ClearInputBoxes();
	}
}
