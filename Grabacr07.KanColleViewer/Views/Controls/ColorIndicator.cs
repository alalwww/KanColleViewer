using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class ColorIndicator : ProgressBar
	{
		static ColorIndicator()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorIndicator), new FrameworkPropertyMetadata(typeof(ColorIndicator)));
		}

		#region LimitedValue 依存関係プロパティ

		public LimitedValue LimitedValue
		{
			get { return (LimitedValue)this.GetValue(LimitedValueProperty); }
			set { this.SetValue(LimitedValueProperty, value); }
		}
		public static readonly DependencyProperty LimitedValueProperty =
			DependencyProperty.Register("LimitedValue", typeof(LimitedValue), typeof(ColorIndicator), new UIPropertyMetadata(new LimitedValue(), LimitedValuePropertyChangedCallback));

		private static void LimitedValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var source = (ColorIndicator)d;
			var value = (LimitedValue)e.NewValue;

			source.ChangeColor(value);
			UpdateToolTip(source, value, source.ToolTipEnabled, source.ToolTipFormatter, source.ToolTipLabel);
		}

		#endregion

		#region ToolTip関連

		/// <summary>
		/// デフォルトのツールチップフォーマッター。
		/// </summary>
		public static readonly Func<LimitedValue, string, string> defaultToolTipFormatter = (v, l) => (String.IsNullOrEmpty(l) ? "" : l + ":") + v.Current + "/" + v.Maximum;

		#region ToolTipEnabled 依存関係プロパティ

		/// <summary>
		/// ツールチップ有効/無効依存関係プロパティの値を設定または参照します。
		/// </summary>
		public bool ToolTipEnabled
		{
			get { return (bool)GetValue(ToolTipEnabledProperty); }
			set { SetValue(ToolTipEnabledProperty, value); }
		}

		/// <summary>
		/// ツールチップ有効/無効の依存関係プロパティ。
		/// </summary>
		public static readonly DependencyProperty ToolTipEnabledProperty =
			 DependencyProperty.Register("ToolTipEnabled",
			typeof(bool), typeof(ColorIndicator),
			new UIPropertyMetadata(true, ToolTipEnabledPropertyChangedCallback));

		private static void ToolTipEnabledPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var source = (ColorIndicator)d;
			var enabled = (bool)e.NewValue;
			UpdateToolTip(source, source.LimitedValue, enabled, source.ToolTipFormatter, source.ToolTipLabel);
		}

		#endregion

		#region ToolTipLabel 依存関係プロパティ

		/// <summary>
		/// ツールチップラベル依存関係プロパティの値を設定または参照します。
		/// </summary>
		public string ToolTipLabel
		{
			get { return (string)GetValue(ToolTipLabelProperty); }
			set { SetValue(ToolTipLabelProperty, value); }
		}

		/// <summary>
		/// ツールチップラベルの依存関係プロパティ。
		/// </summary>
		public static readonly DependencyProperty ToolTipLabelProperty =
			 DependencyProperty.Register("ToolTipLabel",
			typeof(string), typeof(ColorIndicator),
			new UIPropertyMetadata(null, ToolTipLabelPropertyChangedCallback));

		private static void ToolTipLabelPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var source = (ColorIndicator)d;
			var label = (string)e.NewValue;
			UpdateToolTip(source, source.LimitedValue, source.ToolTipEnabled, source.ToolTipFormatter, label);
		}

		#endregion

		#region ToolTipFormatter 依存関係プロパティ

		/// <summary>
		/// ツールチップのフォーマッター依存関係プロパティの値を設定または参照します。
		/// 
		/// このプロパティの持つメソッドのシグネチャは、第一引数にインジケーターの値、
		/// 第二引数にツールチップのラベルとして用いる文字列を受け取り
		/// ツールチップに表示する文字列を返します。
		/// 
		/// </summary>
		public Func<LimitedValue, string, string> ToolTipFormatter
		{
			get { return (Func<LimitedValue, string, string>)GetValue(ToolTipFormatterProperty); }
			set { SetValue(ToolTipFormatterProperty, value); }
		}

		/// <summary>
		/// ツールチップのフォーマッター依存関係プロパティの値を設定または参照します。
		/// </summary>
		public static readonly DependencyProperty ToolTipFormatterProperty =
			DependencyProperty.Register("ToolTipFormatter",
			typeof(Func<LimitedValue, string, string>), typeof(ColorIndicator),
			new UIPropertyMetadata(defaultToolTipFormatter, ToolTipFormatterPropertyChangedCallback));

		private static void ToolTipFormatterPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var source = (ColorIndicator)d;
			var formatter = (Func<LimitedValue, string, string>)e.NewValue;
			UpdateToolTip(source, source.LimitedValue, source.ToolTipEnabled, formatter, source.ToolTipLabel);
		}

		#endregion

		#endregion

		private void ChangeColor(LimitedValue value)
		{
			this.Maximum = value.Maximum;
			this.Minimum = value.Minimum;
			this.Value = value.Current;

			Color color;
			var percentage = value.Maximum == 0 ? 0.0 : value.Current / (double)value.Maximum;

			// 0.25 以下のとき、「大破」
			if (percentage <= 0.25) color = Color.FromRgb(255, 32, 32);

			// 0.5 以下のとき、「中破」
			else if (percentage <= 0.5) color = Color.FromRgb(240, 128, 32);

			// 0.75 以下のとき、「小破」
			else if (percentage <= 0.75) color = Color.FromRgb(240, 240, 0);

			// 0.75 より大きいとき、「小破未満」
			else color = Color.FromRgb(64, 200, 32);

			this.Foreground = new SolidColorBrush(color);
		}

		private static void UpdateToolTip(ColorIndicator source, LimitedValue value,
				bool enabled, Func<LimitedValue, string, string> formatter, string label)
		{
			ToolTipService.SetToolTip(source, enabled ? formatter(value, label) : null);
		}
	}
}
