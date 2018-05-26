﻿using System;
using System.Collections.Generic;
using System.IO;
using HamstarHelpers.TmlHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace BetterPaint.Items {
	partial class ColorCartridgeItem : ModItem {
		public const int Width = 12;
		public const int Height = 16;

		public static Texture2D OverlayTex { get; internal set; }
		public static Texture2D CartridgeTex { get; internal set; }

		static ColorCartridgeItem() {
			ColorCartridgeItem.OverlayTex = null;
			ColorCartridgeItem.CartridgeTex = null;
		}


		////////////////

		public static IList<int> GetPaintCartridges( Player player ) {
			IList<int> colors_idxs = new List<int>();
			Item[] inv = player.inventory;
			int cartridge_type = BetterPaintMod.Instance.ItemType<ColorCartridgeItem>();

			for( int i = 0; i < inv.Length; i++ ) {
				Item item = inv[i];
				if( item == null || item.IsAir || item.type != cartridge_type ) { continue; }

				colors_idxs.Add( i );
			}

			return colors_idxs;
		}


		////////////////

		public int TimesUsed { get; private set; }
		public Color MyColor { get; private set; }


		////////////////

		public override bool CloneNewInstances { get { return true; } }

		public override ModItem Clone() {
			var clone = (ColorCartridgeItem)base.Clone();
			clone.TimesUsed = this.TimesUsed;
			clone.MyColor = this.MyColor;
			return clone;
		}


		////////////////

		public override void SetStaticDefaults() {
			this.DisplayName.SetDefault( "Color Cartridge" );
			this.Tooltip.SetDefault( "Needs a paint blaster to use" + '\n' +
				"Mix cartridges at a paint mixer station" );

			if( ColorCartridgeItem.OverlayTex == null ) {
				ColorCartridgeItem.OverlayTex = this.mod.GetTexture( "Items/ColorCartridgeItem_Color" );
				ColorCartridgeItem.CartridgeTex = this.mod.GetTexture( "Items/ColorCartridgeItem" );
			}

			TmlLoadHelpers.AddModUnloadPromise( () => {
				ColorCartridgeItem.OverlayTex = null;
				ColorCartridgeItem.CartridgeTex = null;
			} );
		}


		public override void SetDefaults() {
			this.TimesUsed = 0;
			this.MyColor = Color.White;

			this.item.width = ColorCartridgeItem.Width;
			this.item.height = ColorCartridgeItem.Height;
			this.item.ammo = this.item.type;
			this.item.value = Item.buyPrice( 0, 0, 10, 0 );
		}


		public override void ModifyTooltips( List<TooltipLine> tooltips ) {
			var mymod = (BetterPaintMod)this.mod;
			float capacity = mymod.Config.PaintCartridgeCapacity;
			float percent = 1f - ((float)this.TimesUsed / capacity);

			var tip1 = new TooltipLine( this.mod, "BetterPaint: Color Indicator", "Color value: " + this.MyColor.ToString() ) {
				overrideColor = this.MyColor
			};
			var tip2 = new TooltipLine( this.mod, "BetterPaint: Capacity", "Capacity: " + (int)( percent * 100 ) + "%" ) {
				overrideColor = percent > 0.15f ? Color.White : Color.Red
			};

			tooltips.Add( tip1 );
			tooltips.Add( tip2 );
		}


		////////////////

		public override void Load( TagCompound tag ) {
			if( tag.ContainsKey("color") ) {
				byte[] bytes = tag.GetByteArray( "color" );

				this.MyColor = new Color( bytes[0], bytes[1], bytes[2], bytes[3] );
			}
			if( tag.ContainsKey( "times_used" ) ) {
				this.TimesUsed = tag.GetInt( "times_used" );
			}
		}

		public override TagCompound Save() {
			return new TagCompound {
				{ "color", new byte[] { this.MyColor.R, this.MyColor.G, this.MyColor.B, this.MyColor.A } },
				{ "times_used", this.TimesUsed }
			};
		}

		////////////////

		public override void NetSend( BinaryWriter writer ) {
			writer.Write( (byte)this.MyColor.R );
			writer.Write( (byte)this.MyColor.G );
			writer.Write( (byte)this.MyColor.B );
			writer.Write( (byte)this.MyColor.A );
			writer.Write( (int)this.TimesUsed );
		}

		public override void NetRecieve( BinaryReader reader ) {
			this.MyColor = new Color( reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() );
			this.TimesUsed = reader.ReadInt32();
		}


		////////////////

		public void SetColor( Color color ) {
			this.MyColor = color;
		}

		public void SetTimesUsed( int uses ) {
			this.TimesUsed = uses;
		}
	}
}
