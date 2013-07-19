using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GlowBeanGlow.ViewModels
{
	public class AnimatorView : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private int _channel1Red;
		public int Channel1Red
		{
			get { return _channel1Red; }
			set 
			{ 
				_channel1Red = value;
				OnPropertyChanged("Channel1Red");
			}
		}

		private int _channel1Green;
		public int Channel1Green
		{
			get { return _channel1Green; }
			set
			{
				_channel1Green = value;
				OnPropertyChanged("Channel1Green");
			}
		}

		private int _channel1Blue;
		public int Channel1Blue
		{
			get { return _channel1Blue; }
			set
			{
				_channel1Blue = value;
				OnPropertyChanged("Channel1Blue");
			}
		}

		private int _channel2Red;
		public int Channel2Red
		{
			get { return _channel2Red; }
			set
			{
				_channel2Red = value;
				OnPropertyChanged("Channel2Red");
			}
		}

		private int _channel2Green;
		public int Channel2Green
		{
			get { return _channel2Green; }
			set
			{
				_channel2Green = value;
				OnPropertyChanged("Channel2Green");
			}
		}

		private int _channel2Blue;
		public int Channel2Blue
		{
			get { return _channel2Blue; }
			set
			{
				_channel2Blue = value;
				OnPropertyChanged("Channel2Blue");
			}
		}

		// Create the OnPropertyChanged method to raise the event
		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}
	}
}
