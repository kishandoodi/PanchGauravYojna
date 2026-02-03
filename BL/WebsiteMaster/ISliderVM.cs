using Microsoft.AspNetCore.Mvc;
using MO.Common;
using MO.Indicator;
using MO.WebsiteMaster;

namespace BL.WebsiteMaster
{
    public interface ISliderVM
    {
        #region slider image master
        Task<result> SaveSliderAsync(mst_Slider model);
        Task<List<SliderListVM>> GetAllSliderImage();
        Task<result> UpdateSliderAsync(mst_Slider entity);
        Task<result> deleteSliderRow(int rowId);
        Task<result> ToggleStatusAsync(int id, bool isActive);
        #endregion
        #region Accouncement master
        Task<List<AnnouncementList>> GetAllAnnouncement();
        Task<result> SaveAnnouncement(mst_Announcement model);
        Task<result> UpdateAnnouncement(mst_Announcement entity);
        Task<result> deleteAnnouncementRow(int rowId);
        Task<result> ToggleAnnouncementStatus(int id, bool? isActive);
        Task<result> ToggleAnnouncementIsNew(int id, bool? isnew);
        #endregion
        #region Home Page Component
        Task<HomePageVM> GetHomePageComponent();
        #endregion

    }
}