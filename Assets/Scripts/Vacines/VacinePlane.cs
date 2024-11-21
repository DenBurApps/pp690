using System;
using TMPro;
using UnityEngine;

public class VacinePlane : MonoBehaviour
{
   [SerializeField] private TMP_Text _kind;
   [SerializeField] private TMP_Text _date;

   public event Action<VacinePlane> Deleted;
   
   public bool IsActive { get; private set; }
   
   public VaccineData Data { get; private set; }
   
   public void Enable(VaccineData data)
   {
      gameObject.SetActive(true);
      IsActive = true;

      Data = data;

      _kind.text = Data.Kind;
      _date.text = Data.Date.ToString("dd MMM yyyy");
   }

   public void Disable()
   {
      gameObject.SetActive(false);
      IsActive = false;
   }

   public void Reset()
   {
      if (Data != null)
      {
         Data = null;
      }
      _kind.text = string.Empty;
      _date.text = string.Empty;
   }

   public void OnDeleted()
   {
      Deleted?.Invoke(this);
   }
}
