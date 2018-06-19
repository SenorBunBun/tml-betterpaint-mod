﻿using BetterPaint.Painting;
using HamstarHelpers.Components.Network;
using Microsoft.Xna.Framework;
using System;
using Terraria;


namespace BetterPaint.NetProtocols {
	class PaintStrokeProtocol : PacketProtocol {
		public static void SyncToAll( PaintLayerType layer, PaintBrushType brush_type, Color color, bool is_lit,
				PaintBrushSize brush_size, float pressure_percent, int rand_seed, int world_x, int world_y ) {
			if( Main.netMode != 1 ) { throw new Exception( "Not client" ); }

			var protocol = new PaintStrokeProtocol( layer, brush_type, color, is_lit, brush_size, pressure_percent, rand_seed, world_x, world_y );
			protocol.SendToServer( true );
		}



		////////////////
		
		public int Layer = (int)PaintLayerType.Foreground;
		public int BrushType = (int)PaintBrushType.Stream;
		public Color MyColor = Color.White;
		public bool IsLit = false;
		public int BrushSize = (int)PaintBrushSize.Small;
		public float PressurePercent = 1;
		public int RandSeed = -1;
		public int WorldX = 0;
		public int WorldY = 0;


		////////////////

		public PaintStrokeProtocol() { }

		private PaintStrokeProtocol( PaintLayerType layer, PaintBrushType brush_type, Color color, bool is_lit,
				PaintBrushSize brush_size, float pressure_percent, int rand_seed, int world_x, int world_y ) {
			this.Layer = (int)layer;
			this.BrushType = (int)brush_type;
			this.MyColor = color;
			this.IsLit = is_lit;
			this.BrushSize = (int)brush_size;
			this.PressurePercent = pressure_percent;
			this.WorldX = world_x;
			this.WorldY = world_y;
		}

		////////////////

		public override void SetServerDefaults() { }

		////////////////

		private void Receive() {
			var mymod = BetterPaintMod.Instance;
			var myworld = BetterPaintMod.Instance.GetModWorld<BetterPaintWorld>();
			var layer = (PaintLayerType)this.Layer;
			var brush_type = (PaintBrushType)this.BrushType;

			myworld.Layers.ApplyColorAtNoSync( mymod, layer, brush_type, this.MyColor, this.IsLit, (PaintBrushSize)this.BrushSize,
				this.PressurePercent, this.RandSeed, this.WorldX, this.WorldY );
		}


		protected override void ReceiveWithServer( int from_who ) {
			this.Receive();
		}

		protected override void ReceiveWithClient() {
			this.Receive();
		}
	}
}
