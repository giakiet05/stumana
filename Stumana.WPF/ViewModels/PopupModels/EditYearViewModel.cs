using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;
using Stumana.WPF.ViewModels.MainViewModels.YearOption;

namespace Stumana.WPF.ViewModels.PopupModels;

public class EditYearViewModel : BaseViewModel
{
    private string _yearId;

    public string YearId
    {
        get => _yearId;
        set
        {
            _yearId = value;
            OnPropertyChanged();
        }
    }

    private string _startYear;

    public string StartYear
    {
        get => _startYear;
        set
        {
            _startYear = value;
            OnPropertyChanged();
        }
    }

    private string _endYear;

    public string EndYear
    {
        get => _endYear;
        set
        {
            _endYear = value;
            OnPropertyChanged();
        }
    }

    private string _minAge;

    public string MinAge
    {
        get => _minAge;
        set
        {
            _minAge = value;
            OnPropertyChanged();
        }
    }

    private string _maxAge;

    public string MaxAge
    {
        get => _maxAge;
        set
        {
            _maxAge = value;
            OnPropertyChanged();
        }
    }

    private string _minScore;

    public string MinScore
    {
        get => _minScore;
        set
        {
            _minScore = value;
            OnPropertyChanged();
        }
    }

    private string _maxCapacity;

    public string MaxCapacity
    {
        get => _maxCapacity;
        set
        {
            _maxCapacity = value;
            OnPropertyChanged();
        }
    }

    private bool _isStartYearInvalid = false;

    public bool IsStartYearInvalid
    {
        get => _isStartYearInvalid;
        set
        {
            _isStartYearInvalid = value;
            OnPropertyChanged();
        }
    }

    private bool _isEndYearInvalid = false;

    public bool IsEndYearInvalid
    {
        get => _isEndYearInvalid;
        set
        {
            _isEndYearInvalid = value;
            OnPropertyChanged();
        }
    }

    private bool _isMinAgeInvalid = false;

    public bool IsMinAgeInvalid
    {
        get => _isMinAgeInvalid;
        set
        {
            _isMinAgeInvalid = value;
            OnPropertyChanged();
        }
    }

    private bool _isMaxAgeInvalid = false;

    public bool IsMaxAgeInvalid
    {
        get => _isMaxAgeInvalid;
        set
        {
            _isMaxAgeInvalid = value;
            OnPropertyChanged();
        }
    }

    private bool _isMinScoreInvalid = false;

    public bool IsMinScoreInvalid
    {
        get => _isMinScoreInvalid;
        set
        {
            _isMinScoreInvalid = value;
            OnPropertyChanged();
        }
    }

    private bool _isMaxCapacityInvalid = false;

    public bool IsMaxCapacityInvalid
    {
        get => _isMaxCapacityInvalid;
        set
        {
            _isMaxCapacityInvalid = value;
            OnPropertyChanged();
        }
    }

    public ICommand EditYearCommand { get; set; }
    public ICommand CancelCommand { get; set; }

    private readonly EventHandler? _onEditSchoolYear;
    private readonly YearViewModel.YearTableRow _selectedYear;

    public EditYearViewModel(YearViewModel.YearTableRow selectedYear, EventHandler? updateTable)
    {
        _selectedYear = selectedYear;
        _onEditSchoolYear = updateTable;

        EditYearCommand = new RelayCommand(EditYear);
        CancelCommand = new RelayCommand(ModalNavigationStore.Instance.Close);

        LoadYearData();
    }

    private async void LoadYearData()
    {
        if (_selectedYear == null)
            return;

        YearId = _selectedYear.YearID;

        // Load SchoolYear data
        var schoolYear = await GenericDataService<SchoolYear>.Instance.GetOneAsync(sy => sy.Id == _selectedYear.YearID);
        if (schoolYear != null)
        {
            StartYear = schoolYear.StartYear.ToString();
            EndYear = schoolYear.EndYear.ToString();
        }

        // Load Asset data
        var asset = await GenericDataService<Asset>.Instance.GetOneAsync(a => a.YearId == _selectedYear.YearID);
        if (asset != null)
        {
            MinAge = asset.MinAge.ToString();
            MaxAge = asset.MaxAge.ToString();
            MinScore = asset.ScoreToPass.ToString();
            MaxCapacity = asset.MaxCapacity.ToString();
        }
    }

    private async void EditYear()
    {
        try
        {
            CheckError();
            if (IsStartYearInvalid || IsEndYearInvalid || IsMinAgeInvalid || IsMaxAgeInvalid || IsMinScoreInvalid || IsMaxCapacityInvalid)
                throw new Exception("Input không hợp lệ.");

            // Update SchoolYear
            var schoolYear = await GenericDataService<SchoolYear>.Instance.GetOneAsync(sy => sy.Id == YearId);
            if (schoolYear != null)
            {
                schoolYear.StartYear = int.Parse(StartYear);
                schoolYear.EndYear = int.Parse(EndYear);
                await GenericDataService<SchoolYear>.Instance.UpdateOneAsync(schoolYear, sy => sy.Id == schoolYear.Id);
            }

            // Update Asset
            var asset = await GenericDataService<Asset>.Instance.GetOneAsync(a => a.YearId == YearId);
            if (asset != null)
            {
                asset.MinAge = int.Parse(MinAge);
                asset.MaxAge = int.Parse(MaxAge);
                asset.ScoreToPass = double.Parse(MinScore);
                asset.MaxCapacity = int.Parse(MaxCapacity);
                await GenericDataService<Asset>.Instance.UpdateOneAsync(asset, a => a.Id == asset.Id);
            }

            _onEditSchoolYear?.Invoke(this, EventArgs.Empty);
            ModalNavigationStore.Instance.Close();
            ToastMessageViewModel.ShowSuccessToast("Cập nhật năm học thành công");
        }
        catch (Exception e)
        {
            ToastMessageViewModel.ShowErrorToast("Cập nhật năm học không thành công. " + e.Message);
        }
    }

    private async void CheckError()
    {
        IsStartYearInvalid = false;
        IsEndYearInvalid = false;
        IsMinAgeInvalid = false;
        IsMaxAgeInvalid = false;
        IsMinScoreInvalid = false;
        IsMaxCapacityInvalid = false;

        if (string.IsNullOrEmpty(StartYear) || int.Parse(StartYear) <= 1900)
            IsStartYearInvalid = true;

        if (string.IsNullOrEmpty(EndYear) || int.Parse(EndYear) <= 1900)
            IsEndYearInvalid = true;

        if (string.IsNullOrEmpty(MinAge) || int.Parse(MinAge) <= 0)
            IsMinAgeInvalid = true;

        if (string.IsNullOrEmpty(MaxAge) || int.Parse(MaxAge) <= 0)
            IsMaxAgeInvalid = true;

        if (string.IsNullOrEmpty(MinScore) || !double.TryParse(MinScore, out double minScore) || minScore > 10 || minScore < 0)
            IsMinScoreInvalid = true;

        if (string.IsNullOrEmpty(MaxCapacity) || int.Parse(MaxCapacity) <= 0)
            IsMaxCapacityInvalid = true;
    }
} 