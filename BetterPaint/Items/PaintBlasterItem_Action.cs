﻿using BetterPaint.Painting;
using BetterPaint.Painting.Brushes;
using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Helpers.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace BetterPaint.Items {
	partial class PaintBlasterItem : ModItem {
		public bool CanPaintAt( Item paintItem, ushort tileX, ushort tileY ) {
			var myworld = ModContent.GetInstance<BetterPaintWorld>();

			if( this.CurrentBrush != PaintBrushType.Erase && PaintBlasterHelpers.GetPaintAmount(paintItem) <= 0 ) {
				return false;
			}

			Tile tile = Main.tile[tileX, tileY];
			
			switch( this.Layer ) {
			case PaintLayerType.Foreground:
				return myworld.Layers.Foreground.CanPaintAt( tile );
			case PaintLayerType.Background:
				return myworld.Layers.Background.CanPaintAt( tile );
			case PaintLayerType.Anyground:
				return myworld.Layers.Foreground.CanPaintAt( tile ) || myworld.Layers.Background.CanPaintAt( tile );
			default:
				throw new ModHelpersException( "Not implemented." );
			}
		}


		public bool HasMatchingPaintAt( Color color, ushort tileX, ushort tileY ) {
			var mymod = (BetterPaintMod)this.mod;
			var myworld = ModContent.GetInstance<BetterPaintWorld>();

			switch( this.Layer ) {
			case PaintLayerType.Foreground:
				if( myworld.Layers.Foreground.GetRawColorAt( tileX, tileY ) == color ) {
					return true;
				}
				break;
			case PaintLayerType.Background:
				if( myworld.Layers.Background.GetRawColorAt( tileX, tileY ) == color ) {
					return true;
				}
				break;
			case PaintLayerType.Anyground:
				if( myworld.Layers.Foreground.HasColorAt(tileX, tileY) ) {
					if( myworld.Layers.Foreground.GetRawColorAt( tileX, tileY ) == color ) {
						return true;
					}
				} else if( myworld.Layers.Background.HasColorAt( tileX, tileY ) ) {
					if( myworld.Layers.Background.GetRawColorAt( tileX, tileY ) == color ) {
						return true;
					}
				}
				break;
			default:
				throw new ModHelpersException( "Not implemented." );
			}

			return false;
		}


		////////////////

		public float ApplyBrushAt( int worldX, int worldY ) {
			var mymod = (BetterPaintMod)this.mod;
			var myworld = ModContent.GetInstance<BetterPaintWorld>();
			float uses = 0;
			Color color = Color.White;
			byte glow = 0;

			Item paintItem = this.GetCurrentPaintItem();

			if( paintItem != null ) {	// Eraser doesn't need paint
				color = PaintBlasterHelpers.GetPaintColor( paintItem );
				glow = PaintBlasterHelpers.GetPaintGlow( paintItem );
			}
			
			uses = myworld.Layers.ApplyColorAt( mymod, this.Layer, this.CurrentBrush, color, glow, this.BrushSize, this.PressurePercent, worldX, worldY );
			
			if( paintItem != null && uses > 0 ) {
				PaintBlasterHelpers.ConsumePaint( paintItem, uses );
			}

			return uses;
		}

		
		public bool AttemptCopyColorAt( Player player, ushort tileX, ushort tileY ) {
			var myworld = ModContent.GetInstance<BetterPaintWorld>();
			PaintLayer data;

			int copyType = ModContent.ItemType<CopyCartridgeItem>();
			int copyItemIdx = ItemFinderHelpers.FindIndexOfFirstOfItemInCollection( player.inventory, new HashSet<int> { copyType } );
			if( copyItemIdx == -1 ) {
				return false;
			}

			switch( this.Layer ) {
			case PaintLayerType.Foreground:
				data = myworld.Layers.Foreground;
				break;
			case PaintLayerType.Background:
				data = myworld.Layers.Background;
				break;
			case PaintLayerType.Anyground:
				if( myworld.Layers.Foreground.HasColorAt( tileX, tileY ) ) {
					data = myworld.Layers.Foreground;
				} else {
					data = myworld.Layers.Background;
				}
				break;
			default:
				throw new ModHelpersException( "Not implemented." );
			}

			if( !data.HasColorAt(tileX, tileY) ) {
				return false;
			}

			Color colorAt = (Color)data.GetRawColorAt( tileX, tileY );

			CopyCartridgeItem.SetWithColor( player, player.inventory[copyItemIdx], colorAt );

			return true;
		}
	}
}
