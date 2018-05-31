﻿using HamstarHelpers.DebugHelpers;
using HamstarHelpers.ItemHelpers;
using Microsoft.Xna.Framework;
using BetterPaint.Items;
using System;
using Terraria;
using Terraria.ModLoader;


namespace BetterPaint.Commands {
	class GivePaintCommand : ModCommand {
		public override CommandType Type {
			get {
				if( Main.netMode == 0 ) {
					return CommandType.World;
				}
				return CommandType.Console;
			}
		}
		public override string Command { get { return "paintgive"; } }
		public override string Usage { get { return "/paintgive"; } }
		public override string Description { get { return "Gives player a random-hued color cartridge."; } }


		////////////////

		public override void Action( CommandCaller caller, string input, string[] args ) {
			var mymod = (BetterPaintMod)this.mod;
			int paint_type = this.mod.ItemType<ColorCartridgeItem>();

			int idx = ItemHelpers.CreateItem( Main.LocalPlayer.position, paint_type, 1, ColorCartridgeItem.Width, ColorCartridgeItem.Height );
			Item paint_item = Main.item[idx];
			if( paint_item == null || paint_item.IsAir ) {
				throw new Exception( "Could not create cheaty paint." );
			}

			Func<byte> rand = () => (byte)Main.rand.Next( 0, 255 );
			var rand_clr = new Color( rand(), rand(), rand(), 255 );

			var myitem = (ColorCartridgeItem)paint_item.modItem;
			myitem.SetColor( rand_clr );

			caller.Reply( "Random color cartridge created: " + rand_clr, rand_clr );
		}
	}
}