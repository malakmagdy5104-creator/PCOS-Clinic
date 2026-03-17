using Microsoft.AspNetCore.Http;
using Shared.DTOs.Diagnosis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesAbstractions
{
    public interface IDiagnosisService
    {
        Task<bool> SaveClinicalDataAsync(ClinicalDataRequestDto model, string userId);
        Task<DiagnosisResultDto?> AnalyzePcosAsync(string userId, IFormFile imageFile);
        Task<List<AnalysisHistoryDto>> GetUserHistoryAsync(string userId);
        Task<DashboardDto> GetDashboardDataAsync(string userId);
    }
}
