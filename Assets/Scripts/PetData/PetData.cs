using System;
using System.Collections.Generic;

[Serializable]
public class PetData
{
    public string Name;
    public string BirthDateString;
    public DateTime BirthDate;
    public string PetType;
    public string Breed;
    public string Coloring;
    public bool IsMale;
    public bool IsIndoors;
    public bool IsSpayed;
    public byte[] ImagePath;

    public List<VaccineData> VaccineDatas;
    public List<EventData> EventDatas;
    public List<WeightData> WeightDatas;

    public PetData(string name, DateTime birthDate, string petType, string breed, string coloring, bool isMale, bool isIndoors, bool isSpayed, byte[] imagePath)
    {
        Name = name;
        BirthDate = birthDate;
        PetType = petType;
        Breed = breed;
        Coloring = coloring;
        IsMale = isMale;
        IsIndoors = isIndoors;
        IsSpayed = isSpayed;
        ImagePath = imagePath;

        VaccineDatas = new List<VaccineData>();
        EventDatas = new List<EventData>();
        WeightDatas = new List<WeightData>();
    }
}

[Serializable]
public class VaccineData
{
    public string Kind;
    public DateTime Date;

    public VaccineData(string kind, DateTime date)
    {
        Kind = kind;
        Date = date;
    }
}

[Serializable]
public class EventData
{
    public string Kind;
    public DateTime Date;
    public byte[] ImagePath;

    public EventData(string kind, DateTime date, byte[] imagePath)
    {
        Kind = kind;
        Date = date;
        ImagePath = imagePath;
    }
}

[Serializable]
public class WeightData
{
    public bool IsPetOnly;
    public int TotalWeight;
    public int OwnerWeight;
    public DateTime Date;


    public WeightData(bool isPetOnly, int totalWeight, int ownerWeight, DateTime date)
    {
        IsPetOnly = isPetOnly;
        TotalWeight = totalWeight;
        OwnerWeight = ownerWeight;
        Date = date;
    }
}
