using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Data.Context;
using ServicesAbstractions;
using Shared.DTOs.Diagnosis;
using System.Net.Http.Json;
using System.Text.Json;

namespace Persistence.Data.Services
{
    public class DiagnosisService : IDiagnosisService
    {
        private readonly SmartPcosDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly UserManager<ApplicationUser> _userManager;

        public DiagnosisService(
            SmartPcosDbContext context,
            HttpClient httpClient,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _httpClient = httpClient;
            _userManager = userManager;
        }

       
        public async Task<bool> SaveClinicalDataAsync(ClinicalDataRequestDto model, string userId)
        {
            var clinicalData = new ClinicalData
            {
                UserId = userId,
                Age = model.Age,
                Weight = (float)model.Weight,
                Height = (float)model.Height,
                Bmi = (float)model.Bmi,
                MenstrualCycleRegular = model.MenstrualCycleRegular,
                AverageCycleLength = (int)model.AverageCycleLength,
                FastFoodFrequency = model.FastFoodFrequency,
                RegularPhysicalActivity = model.RegularPhysicalActivity,
                HairGrowth = model.HairGrowth,
                SkinDarkening = model.SkinDarkening,
                WeightGain = model.WeightGain,
                DifficultyLosingWeight = model.DifficultyLosingWeight,
                Pimples = model.Pimples,
                HairLoss = model.HairLoss,
                WaistInch = (float)(model.WaistInch ?? 30.0),
                HipInch = (float)(model.HipInch ?? 36.0),
                PulseRate = model.PulseRate,
                RespiratoryRate = model.RespiratoryRate ?? 20,
                BloodGroup = model.BloodGroup ?? 11,
                SystolicBP = model.SystolicBP,
                DiastolicBP = model.DiastolicBP,
                MaritalStatusYears = (int)model.MaritalStatusYears,
                IsPregnant = model.IsPregnant,
                NoOfAbortions = model.NoOfAbortions,
                CreatedAt = DateTime.UtcNow
            };

            await _context.ClinicalData.AddAsync(clinicalData);
            return await _context.SaveChangesAsync() > 0;
        }

       
        public async Task<DiagnosisResultDto?> AnalyzePcosAsync(string userId, IFormFile imageFile)
        {
            if (imageFile == null || string.IsNullOrEmpty(userId))
                return null;

            var clinicalData = await _context.ClinicalData
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            if (clinicalData == null)
                return null;

            
            using var content = new MultipartFormDataContent();
            using var stream = imageFile.OpenReadStream();

            content.Add(new StreamContent(stream), "file", imageFile.FileName);

            var imgResponse = await _httpClient.PostAsync(
                "https://mohamedyahya72-pcos-api.hf.space/predict",
                content);

            if (!imgResponse.IsSuccessStatusCode)
            {
                var error = await imgResponse.Content.ReadAsStringAsync();
                throw new Exception($"Image API Error: {error}");
            }

            var imageResult = await imgResponse.Content
                .ReadFromJsonAsync<ImagePredictionResponseDto>();


           
            var tabularPayload = new
            {
                data = new Dictionary<string, object>   
                {
                    { "Age (yrs)", clinicalData.Age },
                    { "Weight (Kg)", clinicalData.Weight },
                    { "Height(Cm)", clinicalData.Height },
                    { "BMI", clinicalData.Bmi },
                    { "Blood Group", clinicalData.BloodGroup },
                    { "Pulse rate(bpm)", clinicalData.PulseRate },
                    { "RR (breaths/min)", clinicalData.RespiratoryRate },
                    { "Hb(g/dl)", 12.0 },
                    { "Cycle(R/I)", clinicalData.MenstrualCycleRegular ? 2 : 4 },
                    { "Cycle length(days)", clinicalData.AverageCycleLength },
                    { "Marital Status (Yrs)", clinicalData.MaritalStatusYears },
                    { "Pregnant(Y/N)", clinicalData.IsPregnant ? 1 : 0 },
                    { "No. of aborptions", clinicalData.NoOfAbortions },
                    { "I   beta-HCG(mIU/mL)", 1.99 },
                    { "II    beta-HCG(mIU/mL)", 1.99 },
                    { "FSH(mIU/mL)", 7.9 },
                    { "LH(mIU/mL)", 3.6 },
                    { "FSH/LH", 2.1 },
                    { "Hip(inch)", clinicalData.HipInch },
                    { "Waist(inch)", clinicalData.WaistInch },
                    { "Waist:Hip Ratio", clinicalData.HipInch == 0 ? 0 : clinicalData.WaistInch / clinicalData.HipInch },
                    { "TSH (mIU/L)", 2.5 },
                    { "AMH(ng/mL)", 4.5 },
                    { "PRL(ng/mL)", 20.0 },
                    { "Vit D3 (ng/mL)", 25.0 },
                    { "PRG(ng/mL)", 0.3 },
                    { "RBS(mg/dl)", 90.0 },
                    { "Weight gain(Y/N)", clinicalData.WeightGain ? 1 : 0 },
                    { "hair growth(Y/N)", clinicalData.HairGrowth ? 1 : 0 },
                    { "Skin darkening (Y/N)", clinicalData.SkinDarkening ? 1 : 0 },
                    { "Hair loss(Y/N)", clinicalData.HairLoss ? 1 : 0 },
                    { "Pimples(Y/N)", clinicalData.Pimples ? 1 : 0 },
                    { "Fast food (Y/N)", clinicalData.FastFoodFrequency > 2 ? 1 : 0 },
                    { "Reg.Exercise(Y/N)", clinicalData.RegularPhysicalActivity ? 1 : 0 },
                    { "BP _Systolic (mmHg)", clinicalData.SystolicBP },
                    { "BP _Diastolic (mmHg)", clinicalData.DiastolicBP },
                    { "Follicle No. (L)", 5 },
                    { "Follicle No. (R)", 5 },
                    { "Avg. Foll. Size (L) (mm)", 15.0 },
                    { "Avg. Foll. Size (R) (mm)", 15.0 },
                    { "Endometrium (mm)", 8.5 }
                }
            };

           
            var tabResponse = await _httpClient.PostAsJsonAsync(
                "https://mohamedyahya72-pcos-tabular-api.hf.space/predict",
                tabularPayload);

            if (!tabResponse.IsSuccessStatusCode)
            {
                var error = await tabResponse.Content.ReadAsStringAsync();
                throw new Exception($"Tabular API Error: {error}");
            }

            var tabularResult = await tabResponse.Content
                .ReadFromJsonAsync<TabularPredictionResponseDto>();


            double? conf = null;

            if (imageResult != null && !string.IsNullOrEmpty(imageResult.confidence))
            {
                var cleaned = imageResult.confidence.Replace("%", "").Trim();

                if (double.TryParse(cleaned, out var parsed))
                    conf = parsed;
            }


            var ultrasoundRecord = new UltrasoundImage
            {
                UserId = userId,
                ImagePath = imageFile.FileName,
                AiPrediction = imageResult?.prediction,
                Confidence = conf,
                HeatmapBase64 = imageResult?.gradcam_image,
                UploadedAt = DateTime.UtcNow
            };

            await _context.UltrasoundImages.AddAsync(ultrasoundRecord);
            await _context.SaveChangesAsync();


            return new DiagnosisResultDto
            {
                ImagePrediction = imageResult?.prediction ?? "N/A",
                Confidence = conf,
                Heatmap = imageResult?.gradcam_image,
                TabularPrediction = tabularResult?.prediction ?? "N/A",
                AnalysisDate = DateTime.UtcNow,
                FinalStatus =
                    (imageResult?.prediction?.Contains("PCOS") == true ||
                     tabularResult?.prediction?.Contains("PCOS") == true)
                    ? "Infected"
                    : "Healthy"
            };
        }


        public async Task<List<AnalysisHistoryDto>> GetUserHistoryAsync(string userId)
        {
            return await _context.UltrasoundImages
                .Where(u => u.UserId == userId)
                .OrderByDescending(u => u.UploadedAt)
                .Select(u => new AnalysisHistoryDto
                {
                    AnalysisId = u.Id,
                    Date = u.UploadedAt,
                    ImagePrediction = u.AiPrediction,
                    Confidence = u.Confidence,
                    FinalStatus = u.AiPrediction.Contains("Infected") ? "Infected" : "Healthy"
                })
                .ToListAsync();
        }
        public async Task<DashboardDto> GetDashboardDataAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            var history = await _context.UltrasoundImages
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.UploadedAt)
                .Select(x => new AnalysisHistoryDto
                {
                    AnalysisId = x.Id,
                    ImagePrediction = x.AiPrediction,
                    FinalStatus = x.AiPrediction,
                    Confidence = x.Confidence,
                    Date = x.UploadedAt
                })
                .ToListAsync();

            return new DashboardDto
            {
                UserName = user.UserName,
                Email = user.Email,
                TotalAnalyses = history.Count,
                History = history
            };
        }
    }
}