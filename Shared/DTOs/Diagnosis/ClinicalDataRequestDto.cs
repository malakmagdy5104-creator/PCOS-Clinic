using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Diagnosis
{
    public class ClinicalDataRequestDto
    {
        [Range(10, 100, ErrorMessage = "Age must be between 10 and 100")]
        public int Age { get; set; }

        [Range(20, 300, ErrorMessage = "Weight must be between 20 and 300 kg")]
        public double Weight { get; set; }

        [Range(100, 250, ErrorMessage = "Height must be between 100 and 250 cm")]
        public double Height { get; set; }

        [Range(10, 60)]
        public double Bmi { get; set; }

        public bool MenstrualCycleRegular { get; set; }

        [Range(15, 60)]
        public double AverageCycleLength { get; set; }

        [Range(0, 7)]
        public int FastFoodFrequency { get; set; }

        public bool RegularPhysicalActivity { get; set; }

        // Physical Symptoms
        public bool HairGrowth { get; set; }
        public bool SkinDarkening { get; set; }
        public bool WeightGain { get; set; }
        public bool DifficultyLosingWeight { get; set; }
        public bool Pimples { get; set; }
        public bool HairLoss { get; set; }

        [Range(20, 80)]
        public double? WaistInch { get; set; }

        [Range(20, 80)]
        public double? HipInch { get; set; }

        [Range(40, 200)]
        public int PulseRate { get; set; }

        [Range(10, 40)]
        public int? RespiratoryRate { get; set; }

        public int? BloodGroup { get; set; }

        [Range(80, 200)]
        public int SystolicBP { get; set; }

        [Range(50, 130)]
        public int DiastolicBP { get; set; }

        [Range(0, 40)]
        public double MaritalStatusYears { get; set; }

        public bool IsPregnant { get; set; }

        [Range(0, 20)]
        public int NoOfAbortions { get; set; }
    }
}