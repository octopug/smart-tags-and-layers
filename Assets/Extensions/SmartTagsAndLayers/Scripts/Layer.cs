using UnityEngine;

namespace SmartTagsAndLayers
{
	public sealed class Layer
	{
		public Layer(string _name)
		{
			name = _name;
		}

		private readonly string name;

		public string Name { get { return name; } }
		public int Mask { get { return LayerMask.GetMask(name); } }
		public int Value { get { return LayerMask.NameToLayer(name); } }
	}
}
