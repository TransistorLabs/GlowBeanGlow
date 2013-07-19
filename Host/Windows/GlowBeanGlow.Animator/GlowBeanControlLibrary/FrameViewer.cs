using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GlowBeanControlLibrary
{
	public class FrameViewer : ListBox
	{
		static FrameViewer()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameViewer), new FrameworkPropertyMetadata(typeof(FrameViewer)));
		}
	}
}
