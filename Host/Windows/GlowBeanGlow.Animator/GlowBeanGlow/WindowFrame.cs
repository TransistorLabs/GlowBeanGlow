using System;
using System.ComponentModel;
using System.Windows.Media;


namespace GlowBeanGlow
{
	public class WindowFrame : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public const int FrameSizeInBytes = 8;

		public ImageSource Thumbnail { get; set; }

		private byte _red0;
		public byte Red0
		{
			get { return _red0; }
			set
			{
				_red0 = value;
				Notify("Red0");
			}
		}

		private byte _green0;
		public byte Green0
		{
			get { return _green0; }
			set
			{
				_green0 = value;
				Notify("Green0");

			}
		}

		private byte _blue0;
		public byte Blue0
		{
			get { return _blue0; }
			set
			{
				_blue0 = value;
				Notify("Blue0");

			}
		}

		private byte _red1;
		public byte Red1
		{
			get { return _red1; }
			set 
			{ 
				_red1 = value;
				Notify("Red1");
			}
		}

		private byte _blue1;
		public byte Blue1
		{
			get { return _blue1; }
			set
			{
				_blue1 = value;
				Notify("Blue1");
			}
		}

		private byte _green1;
		public byte Green1
		{
			get { return _green1; }
			set
			{
				_green1 = value;
				Notify("Green1");
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

		public byte[] ToByteArray()
		{
			var bytes = new byte[FrameSizeInBytes];
			bytes[0] = Red0;
			bytes[1] = Green0;
			bytes[2] = Blue0;
			bytes[3] = Red1;
			bytes[4] = Green1;
			bytes[5] = Blue1;
			bytes[6] = (byte)(OnBits & 0xff); //Get low byte
			bytes[7] = (byte)((OnBits >> 8) & 0xff); //Get high byte

			return bytes;
		}

		public void Clear()
		{
			this.Blue0 = 0;
			this.Blue1 = 0;
			this.Red0 = 0;
			this.Red1 = 0;
			this.Green0 = 0;
			this.Green1 = 0;
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
