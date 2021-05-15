using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using ffmpegGUI.Commands;

namespace ffmpegGUI.MVVM.ViewModel
{
    /// <summary>
    /// this application setting
    /// </summary>
    [Serializable, XmlRoot]
    public class AppSettingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region private member
        double _startTrimTime;
        double _endTrimTime;
        bool _isCompress = false;
        int _compressRate = 32;
        bool _isScaling = false;
        double _verticalScale;
        double _horizontalScale;

        bool _isChanged;
        #endregion

        #region public member
        /// <summary>
        /// start trimming time
        /// </summary>
        public double StartTrimTime
        {
            get => _startTrimTime;
            set => SetProperty(ref _startTrimTime, value);
        }
        /// <summary>
        /// end trimming time
        /// </summary>
        public double EndTrimTime
        {
            get => _endTrimTime;
            set => SetProperty(ref _endTrimTime, value);
        }
        /// <summary>
        /// Do compress movie ?
        /// </summary>
        public bool IsCompress
        {
            get => _isCompress;
            set => SetProperty(ref _isCompress, value);
        }
        /// <summary>
        /// compress rate
        /// </summary>
        public int CompressRate
        {
            get => _compressRate;
            set => SetProperty(ref _compressRate, value);
        }
        /// <summary>
        /// Do scale window size
        /// </summary>
        public bool IsScaling
        {
            get => _isScaling;
            set => SetProperty(ref _isScaling, value);
        }
        /// <summary>
        /// vertical scale size. -1 mean that not scaling.
        /// </summary>
        public double VerticalScale
        {
            get => _verticalScale;
            set => SetProperty(ref _verticalScale, value);
        }
        /// <summary>
        /// horizontal scale size. -1 mean that not scaling.
        /// </summary>
        public double HorizontalScale
        {
            get => _horizontalScale;
            set => SetProperty(ref _horizontalScale, value);
        }
        /// <summary>
        /// setting save command
        /// </summary>
        [XmlIgnore]
        public DelegateCommand SaveCommand { get; private set; }
        #endregion

        #region ctor
        /// <summary>
        /// ctor
        /// </summary>
        public AppSettingViewModel()
        {
            SaveCommand = new DelegateCommand(onSave, canExecuteSave);
        }
        #endregion

        #region notify methods
        /// <summary>
        /// when value is diffrent, set property and notify changed.
        /// </summary>
        /// <typeparam name="T"> all Type</typeparam>
        /// <param name="storage">private member</param>
        /// <param name="value">new value</param>
        /// <param name="propertyName">property name</param>
        /// <returns>true: setting is success, false: storage equal value</returns>
        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            _isChanged |= true;
            SaveCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// notify changed property.
        /// </summary>
        /// <param name="propertyName">propertyName</param>
        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Save (serialize)
        /// <summary>
        /// serialize xml format.
        /// </summary>
        public void Save()
        {
            var xmlSerializer = new XmlSerializer(typeof(AppSettingViewModel));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            using (var sw = new StreamWriter(@"C:\temp\test.xml"))
            {
                xmlSerializer.Serialize(sw, this, namespaces);
            }
        }
        #endregion

        #region Load (deserialize)
        /// <summary>
        /// deserialize AppSetting
        /// </summary>
        /// <returns>AppSetting Object</returns>
        public void Load()
        {
            if (File.Exists(@"C:\temp\test.xml") == false) return;

            var xmlSerializer = new XmlSerializer(typeof(AppSettingViewModel));
            AppSettingViewModel appSetting;
            using (var sr = new StreamReader(@"C:\temp\test.xml"))
            {
                appSetting = xmlSerializer.Deserialize(sr) as AppSettingViewModel;
            }
            SaveCommand = new DelegateCommand(onSave, canExecuteSave);
            StartTrimTime = appSetting.StartTrimTime;
            EndTrimTime = appSetting.EndTrimTime;
            IsCompress = appSetting.IsCompress;
            CompressRate = appSetting.CompressRate;
            IsScaling = appSetting.IsScaling;
            VerticalScale = appSetting.VerticalScale;
            HorizontalScale = appSetting.HorizontalScale;
            _isChanged = false;
            SaveCommand.RaiseCanExecuteChanged();
        }
        #endregion

        void onSave(object param) => Save();
        bool canExecuteSave(object param) => _isChanged;
    }
}
