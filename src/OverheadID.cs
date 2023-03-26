namespace fish.Mods.RainWorld.VisibleID;

public class OverheadID: CosmeticSprite {
	PhysicalObject obj;
	
	#region Convenience properties
		   int ID   => obj.abstractPhysicalObject.ID.number;
		string Type => (obj is Creature c)? c.abstractCreature.creatureTemplate.type.value : null;
		 float agg  => (obj is Creature c)? c.abstractCreature.personality.aggression      : 0f;
		 float brv  => (obj is Creature c)? c.abstractCreature.personality.bravery         : 0f;
		 float dom  => (obj is Creature c)? c.abstractCreature.personality.dominance       : 0f;
		 float nrg  => (obj is Creature c)? c.abstractCreature.personality.energy          : 0f;
		 float nrv  => (obj is Creature c)? c.abstractCreature.personality.nervous         : 0f;
		 float sym  => (obj is Creature c)? c.abstractCreature.personality.sympathy        : 0f;
		 float dge  => (obj as Scavenger)?.dodgeSkill    ?? 0f;
		 float mid  => (obj as Scavenger)?.midRangeSkill ?? 0f;
		 float mle  => (obj as Scavenger)?.meleeSkill    ?? 0f;
		 float blk  => (obj as Scavenger)?.blockingSkill ?? 0f;
		 float rea  => (obj as Scavenger)?.reactionSkill ?? 0f;
	#endregion
	
	public static bool IDLabelVisible = false;
	public static bool StatsVisible = false;
	
	public OverheadID(PhysicalObject o) {
		(obj = o).room.AddObject(this);
		VisibleID.Labels.Add(o, this);
	}
	
	public override void Update(bool eu) {
		if(obj.room is not null) {
			if(room != obj.room) {
				room.RemoveObject(this);
				obj.room.AddObject(this);
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
		
		FLabel Lbl(string text, UnityEngine.Color color, Vec2 pos, float scale = 1f) => new FLabel(Custom.GetDisplayFont(), text) { anchorX = .5f, scale = scale, color = color, x = pos.x, y = pos.y };
		
		FContainer lbl = new();
			lbl.AddChild(bg);
			lbl.AddChild(new FLabel(Custom.GetDisplayFont(), $"{ID}") { anchorX = .5f, scale = .75f, y = +1f });
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
		|| obj.room is null
		|| !obj.room.BeingViewed
		|| (!Cfg.Objects.Value && obj is not Creature)
		|| (Cfg.Dead.Value && obj is Creature c && c.dead)
		|| (!IDLabelVisible && !StatsVisible)
		|| (obj is Overseer ovr && (ovr.mode == Overseer.Mode.SittingInWall || ovr.mode == Overseer.Mode.Withdrawing || ovr.mode == Overseer.Mode.Zipping))
		|| (obj is Fly fly && fly.BitesLeft is 0)
		|| (obj is Player ply && !ply.isNPC && (!Cfg.Players.Value && !Cfg.PlyrAttr.Value))) {
			hide();
		} else {
			show();
			
			BodyChunk op_chunk = (obj as Creature)?.mainBodyChunk ?? obj.firstChunk;
			Vec2 t = Vec2.Lerp(op_chunk.lastPos, op_chunk.pos, time) - campos;
			(top.x, top.y) = (t.x, t.y + 53f);
			
			lbl.isVisible = IDLabelVisible;
			attr.isVisible = StatsVisible;
			stat.isVisible = StatsVisible && (obj is Scavenger);
			
			if(obj is Player p && !p.isNPC) {
				lbl.isVisible = lbl.isVisible && Cfg.Players.Value;
				attr.isVisible = attr.isVisible && Cfg.PlyrAttr.Value;
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
		VisibleID.Labels.Remove(obj);
		Info($"Active label count {VisibleID.Labels.Count}");
		base.Destroy();
	}
}
