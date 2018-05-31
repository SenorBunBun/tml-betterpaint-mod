﻿using BetterPaint.Painting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;


namespace BetterPaint.Items {
	partial class PaintBlasterUI {
		private void CheckUISettingsInteractions( ref Rectangle layer_rect, ref Rectangle size_rect, ref Rectangle copy_rect, ref Rectangle press_rect ) {
			Player player = Main.LocalPlayer;

			if( layer_rect.Contains( Main.mouseX, Main.mouseY ) ) {
				this.CycleLayer();
			} else
			if( size_rect.Contains( Main.mouseX, Main.mouseY ) ) {
				this.CycleBrushSize();
			} else
			if( copy_rect.Contains( Main.mouseX, Main.mouseY ) ) {
				this.IsCopying = !this.IsCopying;
			} else
			if( press_rect.Contains( Main.mouseX, Main.mouseY ) ) {
				this.CyclePressure();
			}
		}


		private void CheckUIBrushInteractions( ref Rectangle brush_rect, ref Rectangle spray_rect, ref Rectangle bucket_rect, ref Rectangle scrape_rect ) {
			Player player = Main.LocalPlayer;
			
			if( this.CurrentBrush != PaintBrushType.Stream && brush_rect.Contains( Main.mouseX, Main.mouseY ) ) {
				this.CurrentBrush = PaintBrushType.Stream;
			} else
			if( this.CurrentBrush != PaintBrushType.Spray && spray_rect.Contains( Main.mouseX, Main.mouseY ) ) {
				this.CurrentBrush = PaintBrushType.Spray;
			} else
			if( this.CurrentBrush != PaintBrushType.Spatter && bucket_rect.Contains( Main.mouseX, Main.mouseY ) ) {
				this.CurrentBrush = PaintBrushType.Spatter;
			} else
			if( this.CurrentBrush != PaintBrushType.Erase && scrape_rect.Contains( Main.mouseX, Main.mouseY ) ) {
				this.CurrentBrush = PaintBrushType.Erase;
			}
		}


		private void CheckUIColorInteractions( IDictionary<int, Rectangle> palette_rects ) {
			int inv_idx = -1;
			
			foreach( var kv in palette_rects ) {
				if( kv.Value.Contains( Main.mouseX, Main.mouseY ) ) {
					inv_idx = kv.Key;
					break;
				}
			}

			if( inv_idx != -1 ) {
				this.CurrentCartridgeInventoryIndex = inv_idx;
			}
		}


		////////////////

		public void CycleLayer() {
			switch( this.Layer ) {
			case PaintLayer.Foreground:
				this.Layer = PaintLayer.Background;
				break;
			case PaintLayer.Background:
				this.Layer = PaintLayer.Anyground;
				break;
			case PaintLayer.Anyground:
				this.Layer = PaintLayer.Foreground;
				break;
			default:
				throw new NotImplementedException();
			}
		}

		public void CycleBrushSize() {
			switch( this.BrushSize ) {
			case PaintBrushSize.Small:
				this.BrushSize = PaintBrushSize.Large;
				break;
			case PaintBrushSize.Large:
				this.BrushSize = PaintBrushSize.Small;
				break;
			default:
				throw new NotImplementedException();
			}
		}

		public void CyclePressure() {
			if( this.PressurePercent >= 0.75f ) {
				this.PressurePercent = 0.25f;
			} else if( this.PressurePercent <= 0.25f ) {
				this.PressurePercent = 0.625f;
			} else {
				this.PressurePercent = 1f;
			}
		}
	}
}
