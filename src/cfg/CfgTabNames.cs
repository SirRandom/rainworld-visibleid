namespace fish.Mods.RainWorld.VisibleID;
using Menu.Remix.MixedUI;

public class CfgTabNames: CfgTab {
	Configurable<string> n0_lbl  = CosmeticBind("");
	Configurable<int>    n0_id   = CosmeticBind( 0);
	Configurable<string> n0_name = CosmeticBind("");
	Configurable<string> n0_crea = CosmeticBind("");
	
	public CfgTabNames(): base("Names") {
		var bad_red     = new UnityEngine.Color(.85f, .35f, .4f);
		var tbx_id      = new OpTextBox(n0_id, new(15f, 475f), 100f) { description = "Enter the creature ID that you wish to name" };
		var tbx_name    = new OpTextBox(n0_name, new(125f, 475f), 200f) { allowSpace = true, description = "What should this creature be named?" };
		var cbx_crea    = new OpComboBox(n0_crea, new(335f, 475f), 140f, new List<ListItem>(CreatureTemplate.Type.values.entries.Select(s => new ListItem(s)))) { description = "Select what type of creature this name should apply to" };
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
		
		void UpdateNamesLabel() {
			string label_text() {
				if(Cfg.Names.IsEmpty()) return "No names mapped";
				else {
					System.Text.StringBuilder sb = new();
					Cfg.Names.ForEach((id,type,name) => sb.Append($"{id}={name} ({type})\n"));
					sb.Remove(sb.Length - 1, 1);
					return sb.ToString();
				}
			}
			
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
		
		btn_add.OnClick += t => {
			if(!string.IsNullOrEmpty(tbx_id.value) && int.TryParse(tbx_id.value, out int id) && !string.IsNullOrEmpty(tbx_name.value) && !string.IsNullOrEmpty(cbx_crea.value)) {
				Cfg.Names.AddName(id, cbx_crea.value, tbx_name.value);
				UpdateNamesLabel();
				ClearInputBoxes();
			} else
				SetError("To add a name, specify both the creature's id and type");
		};
		
		btn_del.OnClick += t => {
			if(int.TryParse(tbx_id.value, out int id)) {
				Cfg.Names.DelName(id, cbx_crea.value);
				UpdateNamesLabel();
				ClearInputBoxes();
			} else
				SetError("Creature id not parseable as integer");
		};
		
		btn_del_all.OnPressDone += t => {
			t.Menu.PlaySound(SoundID.MENU_Security_Button_Release);
			Cfg.Names.Clear();
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
