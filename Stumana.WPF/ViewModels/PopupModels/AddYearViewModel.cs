using System.Windows.Input;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels;

public class AddYearViewModel : BaseViewModel
{
    private string _yearId;

    private string YearId
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

    private bool _isYearIdInvalid = false;

    public bool IsYearIdInvalid
    {
        get => _isYearIdInvalid;
        set
        {
            _isYearIdInvalid = value;
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

    public ICommand AddYearCommand { get; set; }
    public ICommand CancelCommand { get; set; }

    public AddYearViewModel()
    {
        AddYearCommand = new RelayCommand(AddYear);
        CancelCommand = new RelayCommand(() => ModalNavigationStore.Instance.Close());
    }

    private string GenerateYearID(string? startYear, string? endYear)
    {
        if (string.IsNullOrEmpty(startYear) || string.IsNullOrEmpty(endYear))
            return String.Empty;

        string idPrefix = "YR";
        return idPrefix + startYear.Substring(startYear.Length - 2) + endYear.Substring(endYear.Length - 2);
    }

    private async void AddYear()
    {
        try
        {
            CheckError();
            if (IsYearIdInvalid || IsStartYearInvalid || IsEndYearInvalid || IsMinAgeInvalid || IsMaxAgeInvalid || IsMinScoreInvalid || IsMaxCapacityInvalid)
                throw new Exception("Input không hợp lệ.");

            SchoolYear newYear = new SchoolYear
            {
                Id = YearId,
                StartYear = int.Parse(StartYear),
                EndYear = int.Parse(EndYear)
            };
            await GenericDataService<SchoolYear>.Instance.CreateOneAsync(newYear);

            Asset newAsset = new Asset
            {
                Id = YearId,
                MinAge = int.Parse(MinAge),
                MaxAge = int.Parse(MaxAge),
                ScoreToPass = double.Parse(MinScore),
                MaxCapacity = int.Parse(MaxCapacity)
            };
            await GenericDataService<Asset>.Instance.CreateOneAsync(newAsset);
        }
        catch (Exception e)
        {
            ToastMessageViewModel.ShowErrorToast("Thêm năm học không thành công. " + e.Message);
        }
    }

    private async void CheckError()
    {
        IsYearIdInvalid = false;
        IsStartYearInvalid = false;
        IsEndYearInvalid = false;
        IsMinAgeInvalid = false;
        IsMaxAgeInvalid = false;
        IsMinScoreInvalid = false;
        IsMaxCapacityInvalid = false;

        if (string.IsNullOrEmpty(YearId))
            IsYearIdInvalid = true;

        if (string.IsNullOrEmpty(StartYear) || int.Parse(StartYear) <= 0)
            IsStartYearInvalid = true;

        if (string.IsNullOrEmpty(EndYear) || int.Parse(EndYear) <= 0)
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