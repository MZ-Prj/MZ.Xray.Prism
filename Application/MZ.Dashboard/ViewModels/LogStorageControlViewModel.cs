using MZ.Core;
using MZ.Domain.Models;
using MZ.Loading;
using MZ.Util;
using MZ.Xray.Engine;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MZ.Dashboard.ViewModels
{
    public class LogStorageControlViewModel : MZBindableBase
    {
        #region Service
        private readonly ILoadingService _loadingService;
        #endregion

        #region Params
        private LoadingModel _loadingModel;
        public LoadingModel LoadingModel { get => _loadingModel ??= _loadingService[MZRegionNames.DashboardRegion]; set => SetProperty(ref _loadingModel, value); }


        private DateTime? _startSelectedDate = DateTime.Today;
        public DateTime? StartSelectedDate { get => _startSelectedDate; set => SetProperty(ref _startSelectedDate, value); }

        private DateTime? _endSelectedDate = DateTime.Today.AddDays(1).AddSeconds(-1);
        public DateTime? EndSelectedDate
        {
            get => _endSelectedDate;
            set
            {
                if (value.HasValue)
                {
                    var endOfDay = value.Value.Date.AddDays(1).AddSeconds(-1);
                    SetProperty(ref _endSelectedDate, endOfDay);
                }
                else
                {
                    SetProperty(ref _endSelectedDate, null);
                }
            }
        }

        private ICollectionView _filteredLogs;
        public ICollectionView FilteredLogs { get => _filteredLogs; set => SetProperty(ref _filteredLogs, value); }


        private string _searchLogText;
        public string SearchLogText
        {
            get => _searchLogText;
            set
            {
                if (SetProperty(ref _searchLogText, value))
                {
                    FilteredLogs.Refresh();
                }
            }
        }


        private ObservableCollection<LogComboboxModel> _logLevels = [];
        public ObservableCollection<LogComboboxModel> LogLevels { get => _logLevels; set => SetProperty(ref _logLevels, value); }

        private ObservableCollection<LogControlModel> _logs = [];
        public ObservableCollection<LogControlModel> Logs { get => _logs; set => SetProperty(ref _logs, value); }

        private LogComboboxModel _selectedLogLevel;
        public LogComboboxModel SelectedLogLevel { get => _selectedLogLevel; set => SetProperty(ref _selectedLogLevel, value); }


        #endregion

        #region Command
        private DelegateCommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ??= new DelegateCommand(MZAction.Wrapper(SearchButton));
        #endregion

        public LogStorageControlViewModel(IContainerExtension container, ILoadingService loadingService) : base(container)
        {
            _loadingService = loadingService;

            base.Initialize();
        }

        public override void InitializeModel()
        {
            LogLevels =
            [
                new () { Text = "info", ColorBrush = Brushes.SkyBlue },
                new () { Text = "warning", ColorBrush = Brushes.Orange },
                new () { Text = "error", ColorBrush = Brushes.Red }
            ];

            SelectedLogLevel = LogLevels.FirstOrDefault();
            SearchButton();
            InitializeFilter();
        }

        private void InitializeFilter()
        {
            FilteredLogs = CollectionViewSource.GetDefaultView(Logs);
            FilteredLogs.Filter = FilterLogs;
        }

        private bool FilterLogs(object item)
        {
            if (item is LogControlModel images)
            {
                if (string.IsNullOrWhiteSpace(SearchLogText))
                {
                    return true;
                }
                return images.Message?.IndexOf(SearchLogText, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }

        private void SearchButton()
        {
            Logs.Clear();
            
            using (_loadingService[MZRegionNames.LogStorageControl].Show())
            {
                UpdateLogs("./logs");
            }
        }

        private void UpdateLogs(string path)
        {
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "*.txt")
                    .OrderBy(FileNameOrderByDecress);

                foreach (var file in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var parts = fileName.Split('-');
                    if (parts.Length != 2)
                    {
                        continue;
                    }

                    var level = parts[0];
                    var dateStr = parts[1];

                    if (!DateTime.TryParseExact(dateStr, "yyyyMMddHH", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    {
                        continue;
                    }

                    if (StartSelectedDate.HasValue && date < StartSelectedDate.Value)
                    {
                        continue;
                    }

                    if (EndSelectedDate.HasValue && date > EndSelectedDate.Value)
                    {
                        continue;
                    }

                    if (SelectedLogLevel != null && !string.Equals(SelectedLogLevel.Text, level, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string content = ReadFileContent(file);
                    if (string.IsNullOrEmpty(content))
                    {
                        continue;
                    }

                    var lines = content.Split(["\r\n", "\n"], StringSplitOptions.None);

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string message = lines[i];
                        var logItem = new LogControlModel
                        {
                            Date = date,
                            LogLevel = level,
                            LineNumber = i + 1,
                            Message = message,
                            ColorBrush = SelectedLogLevel.ColorBrush,
                        };

                        _dispatcher.Invoke(() =>
                        {
                            Logs.Add(logItem);
                        });
                    }
                }
            }
        }

        private string ReadFileContent(string filePath)
        {
            var tempPath = Path.GetTempFileName();
            try
            {
                File.Copy(filePath, tempPath, true);
                return File.ReadAllText(tempPath);
            }
            catch (IOException)
            {
                return string.Empty;
            }
            finally
            {
                try { File.Delete(tempPath); } catch { }
            }
        }

        private DateTime FileNameOrderByDecress(string file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var parts = fileName.Split('-');
            if (parts.Length != 2)
            {
                return DateTime.MinValue;
            }
            if (DateTime.TryParseExact(parts[1], "yyyyMMddHH", CultureInfo.InvariantCulture, DateTimeStyles.None, out var datetime))
            {
                return datetime;
            }
            return DateTime.MinValue;
        }

    }
}
