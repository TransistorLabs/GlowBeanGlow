using System.Windows;
using System.Windows.Controls;

namespace GlowBeanGlow.ControlLibrary
{
	public class FrameViewer : ListBox
	{
		static FrameViewer()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameViewer), new FrameworkPropertyMetadata(typeof(FrameViewer)));
		}
	}
}
