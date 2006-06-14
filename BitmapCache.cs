using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Fractals
{
	public class BitmapCacheItem 
	{
		Fragment owner;
		Bitmap bitmap;
		int x, y;
		
		public Fragment Owner {
			get {
				return owner;
			}
			set {
				owner = value;
			}
		}
		
		public Bitmap Bitmap {
			get {
				return bitmap;
			}
		}
		
		public int X {
			get {
				return x;
			}
		}
		
		public int Y {
			get {
				return y;
			}
		}
		
		public BitmapCacheItem(Bitmap bitmap, int x, int y)
		{
			this.bitmap = bitmap;
			this.x = x;
			this.y = y;
		}
	}
	
	public class BitmapCache {
		const int fragmentsPerBitmap = 16;
		const int bitmapCount = 8;
		
		List<BitmapCacheItem> caches = new List<BitmapCacheItem>();
		Dictionary<Fragment, BitmapCacheItem> fragmentsWithCache = new Dictionary<Fragment, BitmapCacheItem>();
		
		public BitmapCache()
		{
			// Create cache set
			for (int i = 0; i < bitmapCount; i++) {
				Bitmap bitmap = new Bitmap(fragmentsPerBitmap * Fragment.BitmapSize,
				                           fragmentsPerBitmap * Fragment.BitmapSize,
				                           PixelFormat.Format32bppPArgb);
				
				for(int x = 0; x < fragmentsPerBitmap; x++) {
					for(int y = 0; y < fragmentsPerBitmap; y++) {
						caches.Add(new BitmapCacheItem(bitmap,
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
				BitmapCacheItem item = caches[0];
				caches.RemoveAt(0);
				caches.Add(item);
				
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
