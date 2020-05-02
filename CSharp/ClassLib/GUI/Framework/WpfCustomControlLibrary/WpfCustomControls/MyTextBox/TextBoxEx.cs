using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WpfCustomControls.MyTextBox
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfCustomControls.MyTextBox"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WpfCustomControls.MyTextBox;assembly=WpfCustomControls.MyTextBox"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:TextBoxEx/>
    ///
    /// </summary>
    public class TextBoxEx : TextBox
    {
        char[] mNGFileFolder = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

        public enum TextKind
        {
            None,
            Integer,
            FileOrFolderName
        }

        public TextKind InputTextKind { get; private set; }

        static TextBoxEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxEx), new FrameworkPropertyMetadata(typeof(TextBoxEx)));
        }

        public TextBoxEx()
        {
            InputTextKind = TextKind.None;
        }

        public void SetInputTextKind(TextKind tKind)
        {
            InputTextKind = tKind;
            if (ToolTip == null)
            {   // ToolTip の自動設定
                var tt = new ToolTip();
                switch (tKind)
                {
                    case TextKind.Integer:
                        tt.Content = $"入力可能数値は {int.MinValue} - {int.MaxValue} です。";
                        ToolTip = tt;
                        break;

                    case TextKind.FileOrFolderName:
                        tt.Content = $"下記文字列は入力できません。{Environment.NewLine}{string.Join(",", mNGFileFolder)}";
                        ToolTip = tt;
                        break;

                    case TextKind.None:
                    default:
                        break;
                }
            }
        }

        public bool CheckNGInput(string input)
        {
            switch (InputTextKind)
            {
                case TextKind.Integer:
                    // 変換できなければ Int じゃない
                    var isNegative = Text.Length > 0 && Text[0] == '-';
                    if (this.CaretIndex == 0 && isNegative == false && input == "-") return false;
                    var text = Text.Insert(this.CaretIndex, input);
                    return int.TryParse(text, out _) == false;

                case TextKind.FileOrFolderName:
                    return input.Any(c => mNGFileFolder.Contains(c));

                case TextKind.None:
                default:
                    return false; // チェックなし
            }
        }

        int? beforeSelectionStart = null;
        public void CheckTextChanged()
        {
            switch (InputTextKind)
            {
                case TextKind.Integer:
                    if (Text.Contains(" ") == false) return;
                    if (beforeSelectionStart == null) beforeSelectionStart = this.SelectionStart;
                    Text = Text.Replace(" ", string.Empty);
                    if (beforeSelectionStart != null && beforeSelectionStart > 0) this.Select(beforeSelectionStart.Value - 1, 0);
                    beforeSelectionStart = null;
                    break;

                case TextKind.FileOrFolderName:
                    if (Text.Length > 0 && (Text[0] == ' ') == false) return;
                    Text = Text.TrimStart(new char[] { ' ' });
                    break;

                case TextKind.None:
                default:
                    break;
            }
        }
    }
}
