using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Rectangle = System.Drawing.Rectangle;

namespace GlowBeanGlow
{
	/// <summary>
	/// Interaction logic for GlowBeanGlow.xaml
	/// </summary>
	public partial class GlowBeanWindow : Window
	{
		protected WindowFrame CurrentFrame { get; set; }

		//protected AnimatorView ViewModel = new AnimatorView();
		public GlowBeanWindow()
		{
			InitializeComponent();
			CurrentFrame = new WindowFrame();
			DataContext = CurrentFrame;
		}

		private void Ch1Red_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var color = Led1.OnColor;
			color.R = (byte)CurrentFrame.Red0;

			Led1.OnColor = color;
			Led2.OnColor = color;
			Led3.OnColor = color;
			Led4.OnColor = color;
			Led5.OnColor = color;
			Led6.OnColor = color;
			Led7.OnColor = color;
			Led8.OnColor = color;
			Led9.OnColor = color;
		}

		private void Ch1Green_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Led1.SetGreen(CurrentFrame.Green0);
			Led2.SetGreen(CurrentFrame.Green0);
			Led3.SetGreen(CurrentFrame.Green0);
			Led4.SetGreen(CurrentFrame.Green0);
			Led5.SetGreen(CurrentFrame.Green0);
			Led6.SetGreen(CurrentFrame.Green0);
			Led7.SetGreen(CurrentFrame.Green0);
			Led8.SetGreen(CurrentFrame.Green0);
			Led9.SetGreen(CurrentFrame.Green0);
		}

		private void Ch1Blue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Led1.SetBlue(CurrentFrame.Blue0);
			Led2.SetBlue(CurrentFrame.Blue0);
			Led3.SetBlue(CurrentFrame.Blue0);
			Led4.SetBlue(CurrentFrame.Blue0);
			Led5.SetBlue(CurrentFrame.Blue0);
			Led6.SetBlue(CurrentFrame.Blue0);
			Led7.SetBlue(CurrentFrame.Blue0);
			Led8.SetBlue(CurrentFrame.Blue0);
			Led9.SetBlue(CurrentFrame.Blue0);
		}

		private void Ch2Red_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var color = Led10.OnColor;
			color.R = (byte)CurrentFrame.Red1;

			Led10.OnColor = color;
			Led11.OnColor = color;
		}

		private void Ch2Green_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Led10.SetGreen(CurrentFrame.Green1);
			Led11.SetGreen(CurrentFrame.Green1);
		}

		private void Ch2Blue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Led10.SetBlue(CurrentFrame.Blue1);
			Led11.SetBlue(CurrentFrame.Blue1);
		}

		private void SaveFrameButton_Click(object sender, RoutedEventArgs e)
		{
			var frame = GetFrameFromControls();
			FramesViewer.Items.Add(frame);
			FramesViewer.ScrollIntoView(frame);
		}

		private ImageSource GetThumbnail()
		{
			var renderTarget = new RenderTargetBitmap(545, 460, 96, 96, PixelFormats.Pbgra32);
			renderTarget.Render(DisplayCanvas);
			BitmapFrame bitmapFrame = BitmapFrame.Create(renderTarget);
			var encoder = new PngBitmapEncoder();
			encoder.Frames.Add(bitmapFrame);

			var converter = new ImageSourceConverter();
			var baseImageStream = new MemoryStream();

			encoder.Save(baseImageStream);

			var bitmap = (Bitmap)System.Drawing.Image.FromStream(baseImageStream);

			var cropped = bitmap.Clone(new Rectangle(175, 20, 365, 440), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			var resized = new Bitmap(75, 85);
			Graphics g = Graphics.FromImage(resized);
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.DrawImage(cropped, 0, 0, 75, 85);
			g.Dispose();

			var croppedAndResizedStream = new MemoryStream();

			resized.Save(croppedAndResizedStream, ImageFormat.Png);
			var from = converter.ConvertFrom(croppedAndResizedStream);
			var source = (ImageSource)from;
			
			return source;

		}

		private void SetControlsToFrame(WindowFrame frame)
		{
			CurrentFrame.Red0 = frame.Red0;
			CurrentFrame.Green0 = frame.Green0;
			CurrentFrame.Blue0 = frame.Blue0;
			CurrentFrame.Red1 = frame.Red1;
			CurrentFrame.Green1 = frame.Green1;
			CurrentFrame.Blue1 = frame.Blue1;

			Led1.IsChecked = frame.IsBitOn(0);
			Led2.IsChecked = frame.IsBitOn(1);
			Led3.IsChecked = frame.IsBitOn(2);
			Led4.IsChecked = frame.IsBitOn(3);
			Led5.IsChecked = frame.IsBitOn(4);
			Led6.IsChecked = frame.IsBitOn(5);
			Led7.IsChecked = frame.IsBitOn(6);
			Led8.IsChecked = frame.IsBitOn(7);
			Led9.IsChecked = frame.IsBitOn(8);
			Led10.IsChecked = frame.IsBitOn(9);
			Led11.IsChecked = frame.IsBitOn(10);
		}

		private WindowFrame GetFrameFromControls()
		{
			var frame = new WindowFrame
			{
				Red0 = (byte)Channel1Red.Value,
				Blue0 = (byte)Channel1Blue.Value,
				Green0 = (byte)Channel1Green.Value,
			};
			frame.SetBitValue(0, Led1.IsChecked.Value);
			frame.SetBitValue(1, Led2.IsChecked.Value);
			frame.SetBitValue(2, Led3.IsChecked.Value);
			frame.SetBitValue(3, Led4.IsChecked.Value);
			frame.SetBitValue(4, Led5.IsChecked.Value);
			frame.SetBitValue(5, Led6.IsChecked.Value);
			frame.SetBitValue(6, Led7.IsChecked.Value);
			frame.SetBitValue(7, Led8.IsChecked.Value);
			frame.SetBitValue(8, Led9.IsChecked.Value);
			frame.SetBitValue(9, Led10.IsChecked.Value);
			frame.SetBitValue(10, Led11.IsChecked.Value);

			frame.Thumbnail = GetThumbnail();
			return frame;
		}

		private void FramesViewer_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			var frame = FramesViewer.SelectedItem as WindowFrame;
			if(frame != null)
			{
				SetControlsToFrame(frame);
				UpdateFrameButton.IsEnabled = true;
			}
		}

		private void UpdateFrameButton_Click(object sender, RoutedEventArgs e)
		{
			if (FramesViewer.SelectedIndex >= 0)
			{
				var frame = GetFrameFromControls();
				
				var index = FramesViewer.SelectedIndex;
				FramesViewer.Items[index] = frame;
				
				//Why is this necessary?
				//It seems like there are strange redraw or selection issues if I don't do this:
				FramesViewer.SelectedIndex = index;
			}
		}

	}
}
