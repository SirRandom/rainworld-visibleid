namespace fish.rainworld.visibleid;
using Menu.Remix.MixedUI;

public class CfgTabNames: OpTab {
	Configurable<string> n0_lbl  = new(Cfg.Instance, null, "", null);
	Configurable<int>    n0_id   = new(Cfg.Instance, null,  0, null);
	Configurable<string> n0_name = new(Cfg.Instance, null, "", null);
	Configurable<string> n0_crea = new(Cfg.Instance, null, "", null);
	
	public CfgTabNames(): base(Cfg.Instance, "Names") {
		var bad_red     = new UnityEngine.Color(.85f, .35f, .4f);
		var tbx_id      = new OpTextBox(n0_id, new(15f, 475f), 100f) { description = "Enter the creature ID that you wish to name" };
		var tbx_name    = new OpTextBox(n0_name, new(125f, 475f), 200f) { allowSpace = true, description = "What should this creature be named?" };
		var cbx_crea    = new OpComboBox(n0_crea, new(335f, 475f), 140f, new List<Menu.Remix.MixedUI.ListItem>(CreatureTemplate.Type.values.entries.Select(s => new Menu.Remix.MixedUI.ListItem(s)))) { description = "Select what type of creature this name should apply to" };
		var btn_add     = new OpSimpleButton(new(485f, 475f), new(40f, 24f), "add") { description = "Click to add the given name mapping" };
		var btn_del     = new OpSimpleButton(new(535f, 475f), new(40f, 24f), "del") { description = "Click to remove the given name mapping" };
		var btn_del_all = new OpHoldButton(new(515f, 515f), 10f, "del all") { description = "Click to remove all name mappings", colorEdge = bad_red, colorFill = bad_red };
		var namelist    = new OpScrollBox(new(20f, 20f), new(500f, 430f), 0f);
		
		var names_lbl = new Menu.Remix.MixedUI.OpLabelLong(new(10f, 10f), new(500f, 0f), "") {
			alignment = FLabelAlignment.Left,
			verticalAlignment = Menu.Remix.MixedUI.OpLabel.LabelVAlignment.Bottom,
		};
		var errs = new Menu.Remix.MixedUI.OpLabel(10f, 520f, "") {
			alignment = FLabelAlignment.Left,
			color = bad_red,
		};
		
		string label_text() => string.IsNullOrEmpty(Cfg.Names.Value)? "No mappings created" : string.Join("\n", Cfg.Names.Value.Split(';').Select(v => {
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
				Cfg.Names.Value += $";{id}:{name}:{type}";
				if(Cfg.Names.Value[0] is ';') Cfg.Names.Value = Cfg.Names.Value.Substring(1);
				Cfg.Instance.config.Save();
				VisibleID.Instance.ReloadNames();
			} else
				SetError($"Name already defined for {type} with id {id}");
		}
		
		void RemoveName(int id, string type) {
			var entries = Cfg.Names.Value.Split(';');
			
			string tgt = null;
			foreach(var i in entries) {
				var fields = i.Split(':');
				if(int.TryParse(fields[0], out int i_id) && i_id == id && fields[2] == type) { tgt = i; break; }
			}
			
			if(tgt is not null) {
				var i = entries.IndexOf(tgt);
				Cfg.Names.Value = string.Join(";", entries.Take(i).Concat(entries.Skip(i+1)));
				Cfg.Instance.config.Save();
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
			Cfg.Names.Value = "";
			Cfg.Instance.config.Save();
			UpdateNamesLabel();
			ClearInputBoxes();
		};
		
		this.AddItems(
			new OpLabel(10f, 550f, "ID to Name Mapping", true),
			new OpLabel(30f, 500f, "ID", false),
			new OpLabel(140f, 500f, "Name", false),
			new OpLabel(350f, 500f, "Creature Type", false),
			namelist, errs, btn_del_all, tbx_id, tbx_name, btn_add, btn_del, names_lbl, cbx_crea
		);
		
		names_lbl._AddToScrollBox(namelist);
		
		Cfg.Instance.OnActivate += UpdateNamesLabel;
		Cfg.Instance.OnActivate += ClearInputBoxes;
	}
}
