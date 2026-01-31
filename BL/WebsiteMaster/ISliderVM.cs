using Microsoft.AspNetCore.Mvc;
using MO.Common;
using MO.Indicator;
using MO.WebsiteMaster;

namespace BL.WebsiteMaster
{
    public interface ISliderVM
    {
        Task<result> SaveSliderAsync(mst_Slider model);
        Task<List<SliderListVM>> GetAllSliderImage();
        Task<result> UpdateSliderAsync(mst_Slider entity);
        Task<result> deleteSliderRow(int rowId);
        Task<result> ToggleStatusAsync(int id, bool isActive);
        Task<result> GetAllAnnouncement();

    }
}