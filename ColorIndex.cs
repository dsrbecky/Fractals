using System;
using System.Drawing;

namespace Fractals
{
	public struct ColorIndex
	{
		uint index;
		
		public uint Index {
			get {
				return index;
			}
		}
		
		public ColorIndex(uint index)
		{
			this.index = index;
		}
		
		public ColorIndex(uint a, uint r, uint g, uint b)
		{
			this.index = a * 0x01000000 +
			             r * 0x00010000 +
			             g * 0x00000100 +
			             b * 0x00000001;
		}
		
		public ColorIndex(double a, double r, double g, double b)
		{
			this.index = Math.Min(255, Math.Max(0, (uint)a)) * 0x01000000 +
			             Math.Min(255, Math.Max(0, (uint)r)) * 0x00010000 +
			             Math.Min(255, Math.Max(0, (uint)g)) * 0x00000100 +
			             Math.Min(255, Math.Max(0, (uint)b)) * 0x00000001;
		}
	}
}
