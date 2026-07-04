using TourPlanner.Bll.Dtos;

namespace TourPlanner.Bll;

public interface IImportExportService
{
    Task<ExportTourDataDto> Export(string userId);
}
