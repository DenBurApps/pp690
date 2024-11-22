using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PetPlane : MonoBehaviour
{
    [SerializeField] private Sprite _male;
    [SerializeField] private Sprite _female;

    [SerializeField] private PhotosController _imagePlacer;
    [SerializeField] private Button _photoButton;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private Image _genderLogo;
    [SerializeField] private TMP_Text _age;
    [SerializeField] private Button _infoButton;
    [SerializeField] private TMP_Text _weight;
    [SerializeField] private TMP_Text _vaccines;
    [SerializeField] private TMP_Text _events;

    [SerializeField] private Button _vacineButton;
    [SerializeField] private Button _weightButton;
    [SerializeField] private Button _eventsButton;

    public event Action<PetPlane> PhotoButtonClicked;
    public event Action<PetPlane> VacineButtonClicked;
    public event Action<PetPlane> EventButtonClicked;
    public event Action<PetPlane> WeightButtonClicked;
    public event Action<PetPlane> InfoButtonClicked;
    public event Action Edited;
    
    public bool IsActive { get; private set; }
    public PetData PetData { get; private set; }

    private void OnEnable()
    {
        _photoButton.onClick.AddListener(OnPhotoClicked);
        _vacineButton.onClick.AddListener(OnVaccineClicked);
        _weightButton.onClick.AddListener(OnWeightClicked);
        _eventsButton.onClick.AddListener(OnEventClicked);
        _infoButton.onClick.AddListener(OnInfoClicked);
    }

    private void OnDisable()
    {
        _photoButton.onClick.RemoveListener(OnPhotoClicked);
        _vacineButton.onClick.RemoveListener(OnVaccineClicked);
        _weightButton.onClick.RemoveListener(OnWeightClicked);
        _eventsButton.onClick.RemoveListener(OnEventClicked);
        _infoButton.onClick.RemoveListener(OnInfoClicked);
    }

    public void Enable(PetData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        PetData = data;

        gameObject.SetActive(true);
        IsActive = true;

        UpdateUI();
    }

    public void UpdateData(PetData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        PetData = data;
        UpdateUI();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        IsActive = false;
    }

    public void UpdateUI()
    {
        _genderLogo.sprite = PetData.IsMale ? _male : _female;
        _name.text = PetData.Name;
        
        DateTime today = DateTime.Today;
        int years = today.Year - PetData.BirthDate.Year;
        int months = today.Month - PetData.BirthDate.Month;
        int days = today.Day - PetData.BirthDate.Day;

        if (days < 0)
        {
            months--;
            days += DateTime.DaysInMonth(today.Year, today.Month == 1 ? 12 : today.Month - 1);
        }

        if (months < 0)
        {
            years--;
            months += 12;
        }

        _age.text = $"{years} years {months} months {days} days old";

        if (PetData.WeightDatas.Count > 0)
        {
            var selectedData = PetData.WeightDatas.FirstOrDefault();

            if (selectedData != null)
                _weight.text = selectedData.TotalWeight.ToString() + "KG";
        }
        else
        {
            _weight.text = 0 + "KG";
        }

        if (PetData.EventDatas.Count > 0)
        {
            _events.text = PetData.EventDatas.Count.ToString();
        }
        else
        {
            _events.text = "0";
        }

        if (PetData.VaccineDatas.Count > 0)
        {
            _vaccines.text = PetData.VaccineDatas.Count.ToString();
        }
        else
        {
            _vaccines.text = "0";
        }

        if (PetData.ImagePath != null)
        {
            _imagePlacer.SetPhotos(PetData.ImagePath);
        }
        else
        {
            _imagePlacer.ResetPhotos();
        }
        
        Edited?.Invoke();
    }
    
    public void Reset()
    {
        PetData = null;

        _name.text = string.Empty;
        _genderLogo.sprite = null;
        _age.text = "0 years 0 months 0 days old";
        _weight.text = "0KG";
        _vaccines.text = "0";
        _events.text = "0";

        gameObject.SetActive(false);
        IsActive = false;
    }

    private void OnWeightClicked() => WeightButtonClicked?.Invoke(this);
    private void OnVaccineClicked() => VacineButtonClicked?.Invoke(this);
    private void OnEventClicked() => EventButtonClicked?.Invoke(this);
    private void OnInfoClicked() => InfoButtonClicked?.Invoke(this);
    private void OnPhotoClicked() => PhotoButtonClicked?.Invoke(this);
}