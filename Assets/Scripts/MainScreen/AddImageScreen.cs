using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class AddImageScreen : MonoBehaviour
{
    [SerializeField] private PhotosController _photosController;

    private PetPlane _petPlane;
    private ScreenVisabilityHandler _screenVisabilityHandler;

    public void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void Enable(PetPlane plane)
    {
        _petPlane = plane;

        if (_petPlane.PetData.ImagePath != null)
        {
            _photosController.SetPhotos(_petPlane.PetData.ImagePath);
        }
        else
        {
            _photosController.ResetPhotos();
        }
        
        _screenVisabilityHandler.EnableScreen();
    }

    public void OnSaveClicked()
    {
        var photo = _photosController.GetPhoto();
        
        if (photo != null)
        {
            _petPlane.PetData.ImagePath = photo;
        }
        
        _petPlane.UpdateUI();
        _screenVisabilityHandler.DisableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }
}
