namespace fish.Mods.RainWorld.VisibleID;

public class NameMap {
	Dictionary<(int id, string type), string> names;
	
	public NameMap(string names_str, int version) => names = ParseNames(names_str, version);
	
	public string GetName(int id, string type)
		=> names.TryGetValue((id, type), out string n) ? n : null;
	
	public bool GetName(int id, string type, out string name)
		=> names.TryGetValue((id, type), out name);
	
	public void AddName(int id, string type, string name) {
		try {
			names.Add((id, type), name);
			SaveToConfig();
		} catch(ArgumentException) {
			Warn($"Couldn't add name to {nameof(NameMap)}. The specified {nameof(Creature)} is already named. id={id} type={type} name={name}");
		}
	}
	
	public void DelName(int id, string type) {
		if(names.Remove((id, type)))
			SaveToConfig();
		else
			Warn($"Couldn't remove name for {nameof(Creature)} with id {id} and type {type}");
	}
	
	public void Clear() {
		names.Clear();
		SaveToConfig();
	}
	
	public void ForEach(Action<int, string, string> a) { foreach(var kv in names) a(kv.Key.id, kv.Key.type, kv.Value); }
	
	public bool IsEmpty() => names.Count is 0;
	
	void SaveToConfig() {
		Cfg.CfgNames.Value = ToString();
		Cfg.Save();
	}
	
	static Dictionary<(int id, string type), string> ParseNames(string names, int version) => version switch {
		0 => ParseNamesVersion0(names),
		1 => ParseNamesVersion1(names),
		_ => throw new InvalidOperationException($"Unknown {nameof(version)} \"{version}\" in {nameof(NameMap)}.{nameof(ParseNames)}")
	};
	static Dictionary<(int id, string type), string> ParseNamesVersion0(string names) => ParseByDelimiters(names, ';', ':');
	static Dictionary<(int id, string type), string> ParseNamesVersion1(string names) => ParseByDelimiters(names, '\x001E', '\x001F');
	static Dictionary<(int id, string type), string> ParseByDelimiters(string names, char between_records, char between_fields) {
		Dictionary<(int id, string type), string> mapping = new();
		
		if(!string.IsNullOrEmpty(names))
			foreach(var record in names.Split(between_records)) {
				try {
					var (id,name,type) = record.Split(between_fields);
					mapping.Add((int.Parse(id),type), name);
				} catch(Exception e) {
					Error($"Problem in {nameof(ParseNamesVersion1)} while adding data to {nameof(mapping)}:\n{e.Message}\n{e.StackTrace}");
				}
			}
		
		return mapping;
	}
	
	public override string ToString()
		=> names.Count > 0 ? string.Join('\x001E'.ToString(), names.Select(kv => $"{kv.Key.id}\x001F{kv.Value}\x001F{kv.Key.type}")) : string.Empty;
}
