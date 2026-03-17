using System;

namespace Domain.Entities
{
    public class ClinicalData
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public int Age { get; set; }
        public float Weight { get; set; }
        public float Height { get; set; }
        public float Bmi { get; set; }

        public bool MenstrualCycleRegular { get; set; }
        public int AverageCycleLength { get; set; }
        public int FastFoodFrequency { get; set; }
        public bool RegularPhysicalActivity { get; set; }

        // Physical Symptoms
        public bool HairGrowth { get; set; }
        public bool SkinDarkening { get; set; }
        public bool WeightGain { get; set; }
        public bool DifficultyLosingWeight { get; set; }
        public bool Pimples { get; set; }
        public bool HairLoss { get; set; }

        // Additional AI Features
        public float WaistInch { get; set; }
        public float HipInch { get; set; }
        public int PulseRate { get; set; }
        public int RespiratoryRate { get; set; }
        public int BloodGroup { get; set; }
        public int SystolicBP { get; set; }
        public int DiastolicBP { get; set; }
        public int MaritalStatusYears { get; set; }
        public bool IsPregnant { get; set; }
        public int NoOfAbortions { get; set; }

        public string? PhysicalSymptoms { get; set; }
        public string? RecentMedicalReports { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}