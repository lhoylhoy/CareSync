namespace CareSync.Domain.Entities;

public class VitalSigns
{
    public VitalSigns(Guid id, DateTime measuredAt)
    {
        Id = id;
        MeasuredAt = measuredAt;
    }

    public VitalSigns(Guid id, Guid medicalRecordId, DateTime measuredAt, decimal? temperature,
        int? systolicBp, int? diastolicBp, int? heartRate, int? respiratoryRate,
        decimal? weight, decimal? height, decimal? oxygenSaturation, string? notes)
    {
        Id = id;
        MedicalRecordId = medicalRecordId;
        MeasuredAt = measuredAt;
        Temperature = temperature;
        SystolicBp = systolicBp;
        DiastolicBp = diastolicBp;
        HeartRate = heartRate;
        RespiratoryRate = respiratoryRate;
        Weight = weight;
        Height = height;
        OxygenSaturation = oxygenSaturation;
        Notes = notes;

        ValidateVitalSigns();
    }

    public VitalSigns(Guid id, Guid medicalRecordId, decimal? temperature = null,
        int? systolicBp = null, int? diastolicBp = null, int? heartRate = null,
        int? respiratoryRate = null, decimal? weight = null, decimal? height = null,
        decimal? oxygenSaturation = null, string? notes = null)
    {
        Id = id;
        MedicalRecordId = medicalRecordId;
        MeasuredAt = DateTime.UtcNow;
        Temperature = temperature;
        SystolicBp = systolicBp;
        DiastolicBp = diastolicBp;
        HeartRate = heartRate;
        RespiratoryRate = respiratoryRate;
        Weight = weight;
        Height = height;
        OxygenSaturation = oxygenSaturation;
        Notes = notes;

        ValidateVitalSigns();
    }

    public Guid Id { get; private set; }
    public Guid MedicalRecordId { get; private set; }
    public DateTime MeasuredAt { get; private set; }
    public DateTime MeasurementDate => MeasuredAt; // Alias for compatibility
    public decimal? Temperature { get; private set; } // Celsius
    public int? SystolicBp { get; private set; } // mmHg
    public int? DiastolicBp { get; private set; } // mmHg
    public int? HeartRate { get; private set; } // BPM
    public int? RespiratoryRate { get; private set; } // per minute
    public decimal? Weight { get; private set; } // kg
    public decimal? Height { get; private set; } // cm
    public decimal? OxygenSaturation { get; private set; } // %
    public string? Notes { get; private set; }

    // Calculated BMI
    public decimal? Bmi => Weight.HasValue && Height is > 0
        ? Math.Round(Weight.Value / (Height.Value / 100 * (Height.Value / 100)), 2)
        : null;

    private void ValidateVitalSigns()
    {
        if (Temperature.HasValue && (Temperature.Value < 30 || Temperature.Value > 45))
            throw new ArgumentException("Temperature must be between 30°C and 45°C");

        if (SystolicBp.HasValue && (SystolicBp.Value < 70 || SystolicBp.Value > 300))
            throw new ArgumentException("Systolic BP must be between 70 and 300 mmHg");

        if (DiastolicBp.HasValue && (DiastolicBp.Value < 40 || DiastolicBp.Value > 150))
            throw new ArgumentException("Diastolic BP must be between 40 and 150 mmHg");

        if (HeartRate.HasValue && (HeartRate.Value < 30 || HeartRate.Value > 200))
            throw new ArgumentException("Heart rate must be between 30 and 200 BPM");

        if (OxygenSaturation.HasValue && (OxygenSaturation.Value < 70 || OxygenSaturation.Value > 100))
            throw new ArgumentException("Oxygen saturation must be between 70% and 100%");
    }

    public void UpdateTemperature(decimal temperature)
    {
        if (temperature < 30 || temperature > 45)
            throw new ArgumentException("Temperature must be between 30°C and 45°C", nameof(temperature));

        Temperature = temperature;
    }

    public void UpdateBloodPressure(int systolic, int diastolic)
    {
        if (systolic < 50 || systolic > 300)
            throw new ArgumentException("Systolic BP must be between 50 and 300 mmHg", nameof(systolic));

        if (diastolic < 30 || diastolic > 200)
            throw new ArgumentException("Diastolic BP must be between 30 and 200 mmHg", nameof(diastolic));

        if (systolic <= diastolic)
            throw new ArgumentException("Systolic BP must be greater than diastolic BP");

        SystolicBp = systolic;
        DiastolicBp = diastolic;
    }

    public void UpdateHeartRate(int heartRate)
    {
        if (heartRate < 30 || heartRate > 220)
            throw new ArgumentException("Heart rate must be between 30 and 220 BPM", nameof(heartRate));

        HeartRate = heartRate;
    }

    public void UpdateRespiratoryRate(int respiratoryRate)
    {
        if (respiratoryRate < 5 || respiratoryRate > 60)
            throw new ArgumentException("Respiratory rate must be between 5 and 60 per minute",
                nameof(respiratoryRate));

        RespiratoryRate = respiratoryRate;
    }

    public void UpdateWeight(decimal weight)
    {
        if (weight <= 0 || weight > 1000)
            throw new ArgumentException("Weight must be between 0 and 1000 kg", nameof(weight));

        Weight = weight;
    }

    public void UpdateHeight(decimal height)
    {
        if (height <= 0 || height > 300)
            throw new ArgumentException("Height must be between 0 and 300 cm", nameof(height));

        Height = height;
    }

    public void UpdateOxygenSaturation(decimal oxygenSaturation)
    {
        if (oxygenSaturation < 70 || oxygenSaturation > 100)
            throw new ArgumentException("Oxygen saturation must be between 70% and 100%", nameof(oxygenSaturation));

        OxygenSaturation = oxygenSaturation;
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
    }

    public void Update(DateTime measuredAt, decimal? temperature, int? systolicBp, int? diastolicBp,
        int? heartRate, int? respiratoryRate, decimal? weight, decimal? height, decimal? oxygenSaturation,
        string? notes)
    {
        MeasuredAt = measuredAt;
        Temperature = temperature;
        SystolicBp = systolicBp;
        DiastolicBp = diastolicBp;
        HeartRate = heartRate;
        RespiratoryRate = respiratoryRate;
        Weight = weight;
        Height = height;
        OxygenSaturation = oxygenSaturation;
        Notes = notes;

        ValidateVitalSigns();
    }
}
