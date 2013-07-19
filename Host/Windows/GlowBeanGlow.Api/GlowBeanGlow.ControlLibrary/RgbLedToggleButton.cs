using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace GlowBeanGlow.ControlLibrary
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
	///     <MyNamespace:RgbLedToggleButton/>
	///
	/// </summary>
	public class RgbLedToggleButton : ToggleButton
	{
		static RgbLedToggleButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(RgbLedToggleButton), new FrameworkPropertyMetadata(typeof(RgbLedToggleButton)));
		}

		public void SetRed(int red)
		{
			var color = OnColor;
			color.R = (byte) red;
			OnColor = color;
		}

		public void SetBlue(int blue)
		{
			var color = OnColor;
			color.B = (byte)blue;
			OnColor = color;
		}

		public void SetGreen(int green)
		{
			var color = OnColor;
			color.G = (byte)green;
			OnColor = color;
		}
		
		public Color OnColor
		{
			get { return (Color)GetValue(OnColorProperty); }
			set { SetValue(OnColorProperty, value); }
		}

		public static readonly DependencyProperty OnColorProperty = DependencyProperty.Register("OnColor", typeof (Color),
		                                                                                        typeof (RgbLedToggleButton),
		                                                                                        new FrameworkPropertyMetadata(
		                                                                                        	Colors.Red,
																									FrameworkPropertyMetadataOptions
		                                                                                        		.None
																										, OnOnColorChanged,
																										OnCoerceOnColor
																										));

		public static void OnOnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			
		}

		public static object OnCoerceOnColor(DependencyObject d, object value)
		{
			var color = (Color)value;
			var r = color.R;
			var g = color.G;
			var b = color.B;
			var top = (int)(Math.Max(r, Math.Max(b, g)));
			color.A = (byte) top;
			return color;
		}

		public string StaticText
		{
			get { return (string)GetValue(StaticTextProperty); }
			set { SetValue(StaticTextProperty, value); }
		}

		

		// Using a DependencyProperty as the backing store for StaticText.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty StaticTextProperty =
			DependencyProperty.Register("StaticText", typeof(string), typeof(RgbLedToggleButton), new UIPropertyMetadata("test text here"));

		

		public Color OffColor
		{
			get { return (Color)GetValue(OffColorProperty); }
			set { SetValue(OffColorProperty, value); }
		}

		public static readonly DependencyProperty OffColorProperty = DependencyProperty.Register("OffColor", typeof (Color),
		                                                                                         typeof (RgbLedToggleButton),
		                                                                                         new FrameworkPropertyMetadata
		                                                                                         	(Colors.Blue));


		public RgbLedToggleButton()
		{
			OnColor = Colors.Red;
			OffColor = Colors.Blue;
		}

		protected override void OnClick()
		{
			//Prevent normal click
			//base.OnClick();
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			//_mousePos = e.MouseDevice.GetPosition(this);
			IsChecked = !IsChecked;
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			var newMousePos = e.MouseDevice.GetPosition(this);
			//if(_mousePos.X != newMousePos.X || _mousePos.Y != newMousePos.Y)
			//	IsChecked = !IsChecked;
			base.OnMouseUp(e);
		}
	}
}
