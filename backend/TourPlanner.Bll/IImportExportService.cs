using TourPlanner.Bll.Dtos;

namespace TourPlanner.Bll;

public interface IImportExportService
{
    Task<ExportTourDataDto> Export(string userId);
    Task<int> Import(string userId, ImportTourDataDto importData);
}
