using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
        #region 列挙
        /// <summary>
        /// テキストボックスの種類
        /// </summary>
        public enum TextKind
        {
            /// <summary>
            /// 設定なし
            /// </summary>
            None,
            /// <summary>
            /// 整数
            /// </summary>
            Integer,
            /// <summary>
            /// ファイルやフォルダ名
            /// </summary>
            FileOrFolderName
        }
        #endregion

        #region プライベートメンバ
        /// <summary>
        /// ファイルやフォルダの禁止文字群
        /// </summary>
        char[] mNGFileFolder = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
        #endregion

        #region パブリックメンバ
        /// <summary>
        /// テキストボックスの種類
        /// </summary>
        public TextKind InputTextKind { get; private set; }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// これは自動生成されたもの（カスタムコントロールには必須？）
        /// </summary>
        static TextBoxEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxEx), new FrameworkPropertyMetadata(typeof(TextBoxEx)));
        }
        public TextBoxEx()
        {
            InputTextKind = TextKind.None;
        }
        #endregion

        #region テキストボックスの種類をセット
        /// <summary>
        /// テキストボックスの種類をセットし、それに見合ったツールチップも設定する。
        /// 既にツールチップがxamlによって設定されている場合、ツールチップは設定されない。
        /// </summary>
        /// <param name="tKind">テキストボックスの種類</param>
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
        #endregion

        #region 入力文字にNGがあるかチェック
        /// <summary>
        /// 入力文字にNGなものがあるかチェックする。
        /// PreviewTextInputイベントに使用できる。
        /// </summary>
        /// <param name="input">入力文字</param>
        /// <returns>true: 禁止文字あり、false: 禁止文字なし</returns>
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
        #endregion

        #region 入力値の空白を調整する
        /// <summary>
        /// 前回のフォーカス位置（本メソッドでもTextChangedイベントが発生するため、フォーカス位置を覚えておく）
        /// </summary>
        int? mBeforeSelectionStart = null;
        /// <summary>
        /// 空白が入力された場合、テキストボックスの種類によって調整する。
        /// </summary>
        public void CheckTextChanged()
        {
            switch (InputTextKind)
            {
                case TextKind.Integer:
                    if (Text.Contains(" ") == false) return;
                    if (mBeforeSelectionStart == null) mBeforeSelectionStart = this.SelectionStart;
                    Text = Text.Replace(" ", string.Empty);
                    if (mBeforeSelectionStart != null && mBeforeSelectionStart > 0) this.Select(mBeforeSelectionStart.Value - 1, 0);
                    mBeforeSelectionStart = null;
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
        #endregion
    }
}
