using System;
using System.ComponentModel;
using System.Windows.Media;
using GlowBeanGlow.Api;

namespace GlowBeanGlow
{
	public class WindowFrame : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public const int FrameSizeInBytes = 8;

		public ImageSource Thumbnail { get; set; }

		private byte _red;
		public byte Red
		{
			get { return _red; }
			set
			{
				_red = value;
				Notify("Red");
			}
		}

		private byte _green;
		public byte Green
		{
			get { return _green; }
			set
			{
				_green = value;
				Notify("Green");

			}
		}

		private byte _blue;
		public byte Blue
		{
			get { return _blue; }
			set
			{
				_blue = value;
				Notify("Blue");

			}
		}

		public UInt16 OnBits { get; set; }

		public bool IsBitOn(int bit)
		{
			return (OnBits & (1 << bit)) > 0;
		}

		public void SetBitValue(int bit, bool value)
		{
			if(value)
			{
				OnBits |= (ushort)(1 << bit); //turn bit on
			}
			else
			{
				OnBits &= (ushort)~(1 << bit); //turn bit off
			}
		}

		public void Clear()
		{
			this.Blue = 0;
			this.Red = 0;
			this.Green = 0;
			this.OnBits = 0x00;
		}

		protected void Notify(string propName)
		{
			if(this.PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
			}
		}
	}
}
