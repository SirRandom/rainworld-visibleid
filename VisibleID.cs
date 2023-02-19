global using System;
global using System.Threading;
global using System.Collections.Generic;
global using System.Linq;

global using Input = UnityEngine.Input;
global using Keys = UnityEngine.KeyCode;
global using Vec2 = UnityEngine.Vector2;
global using Dbg = UnityEngine.Debug;

#pragma warning disable CS0618
[assembly: System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, SkipVerification = true)]

namespace fish.rainworld.visibleid;

[BepInEx.BepInPlugin($"{nameof(fish)}.{nameof(visibleid)}", "Visible ID", "1.0")]
public class VisibleID: BepInEx.BaseUnityPlugin {
	public static bool ShowIDs { get; set; } = false;
	public static Dictionary<Creature, OverheadID> Labels { get; } = new();
	
	public void Awake() {
		On.Creature.Update += (orig, creature, eu) => {
			orig(creature, eu);
			if(ShowIDs && creature.room is not null) {
				if(!Labels.ContainsKey(creature)) new OverheadID(creature);
			}
		};
		
		On.RainWorldGame.ExitGame    += (orig, rwg, death, quit)  => { orig(rwg, death, quit);  ClearLabels(); };
		On.RainWorldGame.ExitToMenu  += (orig, rwg)               => { orig(rwg);               ClearLabels(); };
		On.RainWorldGame.Win         += (orig, rwg, malnourished) => { orig(rwg, malnourished); ClearLabels(); };
		On.ArenaSitting.NextLevel    += (orig, sitting, procmgr)  => { orig(sitting, procmgr);  ClearLabels(); };
		On.ArenaSitting.SessionEnded += (orig, sitting, session)  => { orig(sitting, session);  ClearLabels(); };
	}
	
	public void Update() {
		if(Input.GetKeyDown(Keys.Tab)) {
			ShowIDs = !ShowIDs;
			Dbg.Log($"[{nameof(fish)}.{nameof(visibleid)}] {(ShowIDs? "SHOWING IDS" : "HIDING IDS")}");
		}
	}
	
	void ClearLabels() {
		foreach(var label in Labels.Values.ToList()) label.Destroy();
	}
}

public class OverheadID: CosmeticSprite {
	Creature creature;
	
	public OverheadID(Creature c) {
		creature = c;
		c.room.AddObject(this);
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
		leaser.containers = new FContainer[1] { new() };
		leaser.sprites = new FSprite[1] {
			new FSprite("pixel", true) {
				scaleX = 44f,
				scaleY = 15f,
			}
		};
		leaser.containers[0].AddChild(leaser.sprites[0]);
		leaser.containers[0].AddChild(new FLabel("DisplayFont", creature.abstractCreature.ID.number.ToString()) {
			anchorX = 0.5f,
			scale = 0.75f,
		});
		cam.ReturnFContainer("Foreground").AddChild(leaser.containers[0]);
	}
	
	public override void DrawSprites(RoomCamera.SpriteLeaser leaser, RoomCamera cam, float time, Vec2 campos) {
		void hide() => leaser.containers[0].isVisible = false;
		void show() => leaser.containers[0].isVisible = true;
		
		if(room is not null && room.BeingViewed && creature.room is not null && creature.room.BeingViewed && VisibleID.ShowIDs) {
			if(creature is Overseer o && (
				   o.mode == Overseer.Mode.SittingInWall
				|| o.mode == Overseer.Mode.Withdrawing
				|| o.mode == Overseer.Mode.Zipping
			)) hide();
			else if(creature is Fly f && (f.BitesLeft is 0 || f.dead)) hide();
			else show();
			
			Vec2 t = Vec2.Lerp(creature.mainBodyChunk.lastPos, creature.mainBodyChunk.pos, time) - campos;
			(leaser.sprites[0].x, leaser.sprites[0].y) = (t.x, t.y + 53f);
			(leaser.containers[0].GetChildAt(1).x,leaser.containers[0].GetChildAt(1).y) = (t.x, t.y + 54f);
		} else hide();
		
		base.DrawSprites(leaser, cam, time, campos);
	}
	
	public override void Destroy() {
		RemoveFromRoom();
		VisibleID.Labels.Remove(creature);
		Dbg.Log($"Active label count {VisibleID.Labels.Count}");
		base.Destroy();
	}
}
