using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GlowBeanControlLibrary
{
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:GlowBeanControlLibrary"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is 
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:GlowBeanControlLibrary;assembly=GlowBeanControlLibrary"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Browse to and select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file.
	///
	///     <MyNamespace:ColorSlider/>
	///
	/// </summary>
	public class ColorSlider : Slider
	{
		static ColorSlider()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorSlider), new FrameworkPropertyMetadata(typeof(ColorSlider)));
		}



		public Color TargetColor
		{
			get { return (Color)GetValue(TargetColorProperty); }
			set { SetValue(TargetColorProperty, value); }
		}

		// Using a DependencyProperty as the backing store for TargetColor.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TargetColorProperty =
			DependencyProperty.Register("TargetColor", typeof(Color), typeof(ColorSlider), new FrameworkPropertyMetadata(Colors.Red));

		
	}
}
