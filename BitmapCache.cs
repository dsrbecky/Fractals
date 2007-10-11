using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Fractals
{
	public class BitmapCache {
		Renderer renderer;
		
		public const int BitmapSize = 256;
		const int fragmentsPerBitmap = BitmapSize / Fragment.BitmapSize;
		const int bitmapCount = 8;
		
		LinkedList<BitmapCacheItem> caches = new LinkedList<BitmapCacheItem>();
		Dictionary<Fragment, BitmapCacheItem> fragmentsWithCache = new Dictionary<Fragment, BitmapCacheItem>();
		
		public BitmapCache(Renderer renderer)
		{
			this.renderer = renderer;
			// Create cache set
			for (int i = 0; i < bitmapCount; i++) {
				Renderer.Texture tex = renderer.MakeTexture(BitmapSize, BitmapSize);
				
				for(int x = 0; x < fragmentsPerBitmap; x++) {
					for(int y = 0; y < fragmentsPerBitmap; y++) {
						caches.AddLast(new BitmapCacheItem(tex,
						                                   x * Fragment.BitmapSize,
						                                   y * Fragment.BitmapSize));
					}
				}
			}
		}
		
		public void ReleaseCache(Fragment f)
		{
			if (IsCached(f)) {
				fragmentsWithCache[f].Owner = null;
				fragmentsWithCache.Remove(f);
			}
		}
		
		public BitmapCacheItem AllocateCache(Fragment f)
		{
			if (IsCached(f)) {
				return fragmentsWithCache[f];
			} else {
				// Take item from head and put it at tail
				BitmapCacheItem item = caches.First.Value;
				caches.RemoveFirst();
				caches.AddLast(item);
				
				// Allocate the item to freagment
				if (item.Owner != null) {
					ReleaseCache(item.Owner);
				}
				item.Owner = f;
				fragmentsWithCache[f] = item;
				
				return item;
			}
		}
		
		public bool IsCached(Fragment f)
		{
			return fragmentsWithCache.ContainsKey(f);
		}
		
		public BitmapCacheItem this[Fragment f] {
			get {
				if (IsCached(f)) {
					return fragmentsWithCache[f];
				} else {
					return null;
				}
			}
		}
	}
}
