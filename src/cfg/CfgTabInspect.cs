namespace fish.Mods.RainWorld.VisibleID;
using Menu.Remix.MixedUI;

public class CfgTabInspect: CfgTab {
	Configurable<string> n1_id = CosmeticBind("");
	
	public CfgTabInspect(): base("Inspect") {
		bool slugpups = ModManager.MSC && (
				Custom.rainWorld.progression.miscProgressionData.beaten_Gourmand_Full
				|| MoreSlugcats.MoreSlugcats.chtUnlockSlugpups.Value
				|| Cfg.Spoilers.Value
			);
		
		var tbx_2_id = new Menu.Remix.MixedUI.OpTextBox(n1_id, new(80f, 520f), 100f) { description = "Type an ID number to inspect its stats" };
		var inspect_lbls = new Menu.Remix.MixedUI.OpLabel[] {
			new(195f, 523f, "Type in the field to show stats for a given ID number"),
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
		var slugpup_lbls = new Menu.Remix.MixedUI.OpLabel[] {
			new(185f, 280f, "0"),
			new(185f, 260f, "0"),
			new(185f, 240f, "0"),
			new(185f, 220f, "0"),
			new(185f, 200f, "0"),
			new(185f, 180f, "0"),
			new(185f, 160f, "0"),
			new(185f, 140f, "0"),
			new(185f, 120f, "0"),
			new(185f, 100f, "0"),
			new(185f,  80f, "0"),
			new(185f,  60f, "0"),
			new(185f,  40f, "0"),
			new(185f,  20f, "0"),
			new(335f, 268f, "0"),
			new(335f, 255f, "0"),
			new(335f, 242f, "0"),
			new(340f, 198f, "0"),
			new(315f, 280f, "000000"),
			new(315f, 210f, "000000"),
			new(10f, 300f, "Slugpup Stats", true),
			new(282f, 280f, "body"),
			new(285f, 210f, "eye"),
			new(315f, 268f, "H"),
			new(315f, 255f, "S"),
			new(315f, 242f, "L"),
			new(315f, 198f, "eye"),
			new(30f, 280f, "(spd)"), new(70f, 280f, "Run Speed:"),
			new(30f, 260f, "(wgt)"), new(70f, 260f, "Body Weight:"),
			new(30f, 240f, "(vz0)"), new(70f, 240f, "Visibility, Standing:"),
			new(30f, 220f, "(vz1)"), new(70f, 220f, "Visibility, Crouched:"),
			new(30f, 200f, "(lou)"), new(70f, 200f, "Loudness:"),
			new(30f, 180f, "(lng)"), new(70f, 180f, "Lung Capacity:"),
			new(30f, 160f, "(pol)"), new(70f, 160f, "Pole Climbing:"),
			new(30f, 140f, "(tun)"), new(70f, 140f, "Tunnel Climbing:"),
			new(30f, 120f, "(bal)"), new(70f, 120f, "Balance:"),
			new(30f, 100f, "(met)"), new(70f, 100f, "Metabolism:"),
			new(30f,  80f, "(stl)"), new(70f,  80f, "Stealth:"),
			new(30f,  60f, "(siz)"), new(70f,  60f, "Size:"),
			new(30f,  40f, "(wde)"), new(70f,  40f, "Wideness:"),
																new(70f,  20f, "Dark:"),
		};
		var slugpup_boxes = new Menu.Remix.MixedUI.OpRect[] {
			new(new(280f, 250f), new(30f, 30f), 1f),
			new(new(280f, 180f), new(30f, 30f), 1f),
		};
		var foodpref_lbls = new Menu.Remix.MixedUI.OpLabel[] {
			new(307f, 149f, "0"), // danglefruit
			new(307f, 127f, "0"), // waternut
			new(307f, 101f, "0"), // jellyfish
			new(307f,  75f, "0"), // slimemold
			new(307f,  50f, "0"), // eggbugegg
			new(307f,  25f, "0"), // fireegg
			new(414f, 151f, "0"), // seed
			new(414f, 127f, "0"), // gooieduck
			new(414f, 102f, "0"), // lillypuck
			new(414f,  77f, "0"), // glowweed
			new(414f,  54f, "0"), // dandelionpeach
			new(414f,  27f, "0"), // neuron
			new(519f, 150f, "0"), // centipede2
			new(519f, 128f, "0"), // centipede1
			new(519f, 103f, "0"), // vulturegrub
			new(519f,  80f, "0"), // smallneedleworm
			new(519f,  52f, "0"), // hazer
		};
		Menu.Remix.MixedUI.OpImage[] foodicons = null;
		
		if(ModManager.MSC)
			foodicons = new Menu.Remix.MixedUI.OpImage[] {
				new(new(283f, 150f), "Symbol_DangleFruit"),
				new(new(284f, 130f), "Symbol_WaterNut"),
				new(new(280f, 100f), "Symbol_JellyFish"),
				new(new(280f,  75f), "Symbol_SlimeMold"),
				new(new(283f,  50f), "Symbol_EggBugEgg"),
				new(new(279f,  25f), "Symbol_FireEgg"),
				new(new(388f, 158f), "Symbol_Seed"),
				new(new(392f, 151f), "Symbol_Seed"),
				new(new(395f, 160f), "Symbol_Seed"),
				new(new(385f, 127f), "Symbol_GooieDuck"),
				new(new(385f, 102f), "Symbol_LillyPuck"),
				new(new(385f,  77f), "Symbol_GlowWeed"),
				new(new(385f,  49f), "Symbol_DandelionPeach"),
				new(new(391f,  24f), "Symbol_Neuron"),
				new(new(488f, 150f), "Kill_Centipede2"),
				new(new(490f, 128f), "Kill_Centipede1"),
				new(new(491f, 105f), "Kill_VultureGrub"),
				new(new(495f,  80f), "Kill_SmallNeedleWorm"),
				new(new(493f,  52f), "Kill_Hazer"),
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
				
				if(ModManager.MSC) {
					float bal, met, stl, siz, wde, eye, h, s, l;
					bool drk;
					using(new Seeded(eid.RandomSeed)) {
						bal = Mathf.Pow(Rand.Range(0f, 1f), 1.5f);
						met = Mathf.Pow(Rand.Range(0f, 1f), 1.5f);
						stl = Mathf.Pow(Rand.Range(0f, 1f), 1.5f);
						siz = Mathf.Pow(Rand.Range(0f, 1f), 1.5f);
						wde = Mathf.Pow(Rand.Range(0f, 1f), 1.5f);
						h = Mathf.Lerp(Rand.Range(0.15f, 0.58f), Rand.value, Mathf.Pow(Rand.value, 1.5f - met));
						s = Mathf.Pow(Rand.Range(0f, 1f), 0.3f + stl * 0.3f);
						drk = (Rand.Range(0f, 1f) <= 0.3f + stl * 0.2f);
						l = Mathf.Pow(Rand.Range(drk ? 0.9f : 0.75f, 1f), 1.5f - stl);
						eye = Mathf.Pow(Rand.Range(0f, 1f), 2f - stl * 1.5f);
					}
					
					// Thank you Vigaro#0795
					Color GetBodyColor(ref float h, ref float s, ref float l, ref bool dark, int seed) {
						switch(seed) {
							case 1000: {
								var c = new Color(.6f, .7f, .9f);
								var c2 = Custom.RGB2HSL(c);
								h = c2.x;
								s = c2.y;
								l = c2.z;
								return c;
							}
							case 1001: {
								dark = false;
								var c = new Color(.48f, .87f, .81f);
								var c2 = Custom.RGB2HSL(c);
								h = c2.x;
								s = c2.y;
								l = c2.z;
								return c;
							}
							case 1002: {
								dark = true;
								var c = new Color(.43922f, .13725f, .23529f);
								var c2 = Custom.RGB2HSL(c);
								h = c2.x;
								s = c2.y;
								l = c2.z;
								return c;
							}
							default: return Custom.HSL2RGB(h, s, Mathf.Clamp(dark? 1f - l : l, .01f, 1f), 1f);
						}
					}
					
					var c_body = GetBodyColor(ref h, ref s, ref l, ref drk, eid.RandomSeed);
					var c_eyes = Color.Lerp((!drk)? Color.black : (new Color(1f,1f,1f,2f) - Color.black), Color.white, eye * .25f);
					
					if(eid.RandomSeed is 1000 or 1001) c_eyes = Color.black;
					
					var scugstats = new SlugcatStats(MoreSlugcats.MoreSlugcatsEnums.SlugcatStatsName.Slugpup, false);
					scugstats.runspeedFac *= 0.85f + 0.15f * met + 0.15f * (1f - bal) + 0.1f * (1f - stl);
					scugstats.bodyWeightFac *= 0.85f + 0.15f * wde + 0.1f * met;
					scugstats.generalVisibilityBonus *= 0.8f + 0.2f * (1f - stl) + 0.2f * met;
					scugstats.visualStealthInSneakMode *= 0.75f + 0.35f * stl + 0.15f * (1f - met);
					scugstats.loudnessFac *= 0.8f + 0.2f * wde + 0.2f * (1f - stl);
					scugstats.lungsFac *= 0.8f + 0.2f * (1f - met) + 0.2f * (1f - stl);
					scugstats.poleClimbSpeedFac *= 0.85f + 0.15f * met + 0.15f * bal + 0.1f * (1f - stl);
					scugstats.corridorClimbSpeedFac *= 0.85f + 0.15f * met + 0.15f * (1f - bal) + 0.1f * (1f - stl);
					
					slugpup_lbls[ 0].text = $"{scugstats.runspeedFac}";
					slugpup_lbls[ 1].text = $"{scugstats.bodyWeightFac}";
					slugpup_lbls[ 2].text = $"{scugstats.generalVisibilityBonus}";
					slugpup_lbls[ 3].text = $"{scugstats.visualStealthInSneakMode}";
					slugpup_lbls[ 4].text = $"{scugstats.loudnessFac}";
					slugpup_lbls[ 5].text = $"{scugstats.lungsFac}";
					slugpup_lbls[ 6].text = $"{scugstats.poleClimbSpeedFac}";
					slugpup_lbls[ 7].text = $"{scugstats.corridorClimbSpeedFac}";
					slugpup_lbls[ 8].text = $"{bal}";
					slugpup_lbls[ 9].text = $"{met}";
					slugpup_lbls[10].text = $"{stl}";
					slugpup_lbls[11].text = $"{siz}";
					slugpup_lbls[12].text = $"{wde}";
					slugpup_lbls[13].text = drk? "yes" : "no";
					slugpup_lbls[14].text = $"{h}";
					slugpup_lbls[15].text = $"{s}";
					slugpup_lbls[16].text = $"{l}";
					slugpup_lbls[17].text = $"{eye}";
					
					slugpup_lbls[18].text = c_body.AsHex();
					slugpup_lbls[19].text = c_eyes.AsHex();
					Menu.Remix.MixedUI.OpRect box;
					box = slugpup_boxes[0]; box.colorFill = c_body;
					box = slugpup_boxes[1]; box.colorFill = c_eyes;
					
					float[] foodpref = new float[17];
					using(new Seeded(eid.RandomSeed))
						for(int i = 0; i < foodpref.Length; ++i) {
							(float a, float b) = i switch {
									0 => (personality.nervous,    personality.energy),
									1 => (personality.sympathy,   personality.aggression),
									2 => (personality.energy,     personality.nervous),
									3 => (personality.energy,     personality.aggression),
									4 => (personality.dominance,  personality.energy),
									5 => (personality.aggression, personality.sympathy),
									6 => (personality.dominance,  personality.bravery),
									7 => (personality.sympathy,   personality.bravery),
									8 => (personality.aggression, personality.nervous),
									9 => (personality.nervous,    personality.energy),
								10 => (personality.bravery,    personality.dominance),
								11 => (personality.bravery,    personality.nervous),
								12 => (personality.bravery,    personality.dominance),
								13 => (personality.energy,     personality.aggression),
								14 => (personality.dominance,  personality.bravery),
								15 => (personality.aggression, personality.sympathy),
								16 => (personality.nervous,    personality.sympathy),
									_ => throw new Exception("invalid index while processing food preferences")
							};
							
							a *= Custom.PushFromHalf(Rand.value, 2f);
							b *= Custom.PushFromHalf(Rand.value, 2f);
							foodpref[i] = Mathf.Clamp(Mathf.Lerp(a - b, Mathf.Lerp(-1f, 1f, Custom.PushFromHalf(Rand.value, 2f)), Custom.PushFromHalf(Rand.value, 2f)), -1f, 1f);
						}
					
					for(int i = 0; i < foodpref.Length; ++i) foodpref_lbls[i].text = $"{foodpref[i]}";
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
			} else {
				foreach(var lbl in inspect_lbls.Take(12)) lbl.text = "";
				foreach(var lbl in slugpup_lbls.Take(20)) lbl.text = "";
				if(ModManager.MSC) foreach(var box in slugpup_boxes) box.colorFill = new(0f,0f,0f,1f);
				foreach(var lbl in foodpref_lbls) lbl.text = "";
			}
		};
		
		this.AddItems(new Menu.Remix.MixedUI.UIelement[] {
			new Menu.Remix.MixedUI.OpLabel(10f, 550f, "Inspect an ID", true),
			tbx_2_id,
		});
		this.AddItems(inspect_lbls);
		if(ModManager.MSC) {
			this.AddItems(slugpup_lbls);
			this.AddItems(slugpup_boxes);
			this.AddItems(foodpref_lbls);
			this.AddItems(foodicons);
		}
		
		Cfg.Instance.OnActivate += () => {
			if(ModManager.MSC) {
				slugpups = Custom.rainWorld.progression.miscProgressionData.beaten_Gourmand_Full || MoreSlugcats.MoreSlugcats.chtUnlockSlugpups.Value || Cfg.Spoilers.Value;
				
				foreach(var i in slugpup_lbls)  i.Hidden = !slugpups;
				foreach(var i in slugpup_boxes) i.Hidden = !slugpups;
				foreach(var i in foodpref_lbls) i.Hidden = !slugpups;
				foreach(var i in foodicons)     i.Hidden = !slugpups;
			}
		};
	}
}
