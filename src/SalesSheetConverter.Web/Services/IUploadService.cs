using Microsoft.AspNetCore.Components.Forms;

namespace SalesSheetConverter.Web.Services;

public interface IUploadService
{
    public Task<string> Upload(List<IBrowserFile> files);
}