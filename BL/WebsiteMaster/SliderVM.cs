using DL;
using MO.Common;
using MO.Indicator;
using MO.WebsiteMaster;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.WebsiteMaster
{
    public class SliderVM : ISliderVM
    {
        #region Properties
        private readonly ISQLHelper _iSql;
        #endregion
        #region Constructor
        public SliderVM(ISQLHelper iSql)
        {
            _iSql = iSql;
        }
        #endregion
        #region slider image save , get & update,delete methods
        public async Task<result> SaveSliderAsync(mst_Slider model)
        {
            result res = new result();

            try
            {
                var param = new SqlParameter[]
                {
                    new SqlParameter("@Action", "Insert"),
                    new SqlParameter("@ImageBase64", model.ImageBase64),
                    new SqlParameter("@Title", model.Title ?? ""),
                    new SqlParameter("@DisplayOrder", model.DisplayOrder),
                    new SqlParameter("@IsActive", model.IsActive)
                };
                DataSet ds = await _iSql.ExecuteProcedure("SP_Manage_WebsiteMaster", param.ToArray());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    res.status = Convert.ToBoolean(row["Status"]);
                    res.message = row["Message"].ToString();
                }
                else
                {
                    res.status = false;
                    res.message = "No response ";
                }
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
            }

            return res;
        }
        public async Task<List<SliderListVM>> GetAllSliderImage()
        {
            List<SliderListVM> list = new List<SliderListVM>();

            try
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@Action", "GetAllSliderList")
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_Manage_WebsiteMaster", param.ToArray());

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(new SliderListVM
                    {
                        Id = row.Field<int>("Id"),
                        Title = row.Field<string>("Title"),
                        DisplayOrder = row.Field<int?>("DisplayOrder"),
                        IsActive = row.Field<bool>("IsActive"),
                        CreatedDate = row.Field<DateTime>("CreatedDate"),
                        ImageBase64 = row.Field<string>("ImageBase64")
                    });
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            return list;
        }
        public async Task<result> UpdateSliderAsync(mst_Slider entity)
        {
            result res = new result();

            try
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@Action", "UpdateSlider"),
            new SqlParameter("@Id", entity.Id),
            new SqlParameter("@Title", entity.Title ?? (object)DBNull.Value),
            new SqlParameter("@DisplayOrder", entity.DisplayOrder ?? (object)DBNull.Value),
            new SqlParameter("@IsActive", entity.IsActive),
            new SqlParameter("@ImageBase64",
                string.IsNullOrEmpty(entity.ImageBase64)
                    ? (object)DBNull.Value
                    : entity.ImageBase64)
                };

                DataSet ds = await _iSql.ExecuteProcedure(
                    "SP_Manage_WebsiteMaster",
                    param.ToArray()
                );

                if (ds != null &&
                    ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    res.status = Convert.ToBoolean(row["Status"]);
                    res.message = row["Message"].ToString();
                }
                else
                {
                    res.status = false;
                    res.message = "No response from database";
                }
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
            }

            return res;
        }
        public async Task<result> deleteSliderRow(int rowId)
        {
            result res = new result();

            try
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@Action", "DeleteSliderRow"),
            new SqlParameter("@Id", rowId),
                };


                DataSet ds = await _iSql.ExecuteProcedure(
                    "SP_Manage_WebsiteMaster",
                    param.ToArray()
                );

                if (ds != null &&
                    ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    res.status = Convert.ToBoolean(row["Status"]);
                    res.message = row["Message"].ToString();
                }
                else
                {
                    res.status = false;
                    res.message = "No response from database";
                }
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
            }

            return res;
        }
        public async Task<result> ToggleStatusAsync(int id, bool isActive)
        {
            result res = new result();

            try
            {
                var param = new SqlParameter[]
                {
              new SqlParameter("@Action", "ToggleStatus"),
        new SqlParameter("@Id", id),
        new SqlParameter("@IsActive", isActive)
                };


                DataSet ds = await _iSql.ExecuteProcedure(
                    "SP_Manage_WebsiteMaster",
                    param.ToArray()
                );

                if (ds != null &&
                    ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    res.status = Convert.ToBoolean(row["Status"]);
                    res.message = row["Message"].ToString();
                }
                else
                {
                    res.status = false;
                    res.message = "No response from database";
                }
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
            }

            return res;
        }
        #endregion
        #region Announcement save,get,& update,delete methods
        public async Task<result> SaveAnnouncement(mst_Announcement model)
        {
            result res = new result();

            try
            {
                var param = new SqlParameter[]
                {
                    new SqlParameter("@Action", "Insert"),
                    new SqlParameter("@ImageBase64", model.ImageBase64),
                    new SqlParameter("@Title", model.Title ?? ""),
                    new SqlParameter("@DisplayOrder", model.DisplayOrder),
                    new SqlParameter("@IsActive", model.IsActive),
                    new SqlParameter("@IsNew", model.IsNew)
                };
                DataSet ds = await _iSql.ExecuteProcedure("SP_Manage_Announcement", param.ToArray());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    res.status = Convert.ToBoolean(row["Status"]);
                    res.message = row["Message"].ToString();
                }
                else
                {
                    res.status = false;
                    res.message = "No response ";
                }
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
            }

            return res;
        }
        public async Task<List<AnnouncementList>> GetAllAnnouncement()
        {
            List<AnnouncementList> list = new List<AnnouncementList>();

            try
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@Action", "GetAllAnnouncementList")
                };

                DataSet ds = await _iSql.ExecuteProcedure("SP_Manage_Announcement", param.ToArray());
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new AnnouncementList
                        {
                            Id = row.Field<int>("Id"),
                            Title = row.Field<string>("Title"),
                            DisplayOrder = row.Field<int?>("DisplayOrder"),
                            IsActive = row.Field<bool>("IsActive"),
                            IsNew = row.Field<bool?>("IsNew") ?? false,
                            CreatedDate = row.Field<DateTime>("CreatedDate"),
                            ImageBase64 = row.Field<string>("ImageBase64")
                        });
                    }
                }
                else
                {
                    //res.status = false;
                    //res.message = "No response from database";
                }



            }
            catch (Exception ex)
            {
                throw;
            }

            return list;
        }
        public async Task<result> UpdateAnnouncement(mst_Announcement entity)
        {
            result res = new result();

            try
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@Action", "UpdateAnnouncement"),
            new SqlParameter("@Id", entity.Id),
            new SqlParameter("@Title", entity.Title ?? (object)DBNull.Value),
            new SqlParameter("@DisplayOrder", entity.DisplayOrder ?? (object)DBNull.Value),
            new SqlParameter("@IsActive", entity.IsActive),
            new SqlParameter("@IsNew", entity.IsNew),
            new SqlParameter("@ImageBase64",
                string.IsNullOrEmpty(entity.ImageBase64)
                    ? (object)DBNull.Value
                    : entity.ImageBase64)
                };

                DataSet ds = await _iSql.ExecuteProcedure(
                    "SP_Manage_Announcement",
                    param.ToArray()
                );

                if (ds != null &&
                    ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    res.status = Convert.ToBoolean(row["Status"]);
                    res.message = row["Message"].ToString();
                }
                else
                {
                    res.status = false;
                    res.message = "No response from database";
                }
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
            }

            return res;
        }
        public async Task<result> deleteAnnouncementRow(int rowId)
        {
            result res = new result();

            try
            {
                var param = new SqlParameter[]
                {
            new SqlParameter("@Action", "DeleteAnnouncementRow"),
            new SqlParameter("@Id", rowId),
                };


                DataSet ds = await _iSql.ExecuteProcedure(
                    "SP_Manage_Announcement",
                    param.ToArray()
                );

                if (ds != null &&
                    ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    res.status = Convert.ToBoolean(row["Status"]);
                    res.message = row["Message"].ToString();
                }
                else
                {
                    res.status = false;
                    res.message = "No response from database";
                }
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
            }

            return res;
        }
        public async Task<result> ToggleAnnouncementStatus(int id, bool? isActive)
        {
            result res = new result();

            try
            {
                var param = new SqlParameter[]
                {
              new SqlParameter("@Action", "ToggleStatusAnnouncement"),
        new SqlParameter("@Id", id),
        new SqlParameter("@IsActive", isActive)
                };


                DataSet ds = await _iSql.ExecuteProcedure(
                    "SP_Manage_Announcement",
                    param.ToArray()
                );

                if (ds != null &&
                    ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    res.status = Convert.ToBoolean(row["Status"]);
                    res.message = row["Message"].ToString();
                }
                else
                {
                    res.status = false;
                    res.message = "No response from database";
                }
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
            }

            return res;
        }
        public async Task<result> ToggleAnnouncementIsNew(int id, bool? isnew)
        {
            result res = new result();

            try
            {
                var param = new SqlParameter[]
                {
              new SqlParameter("@Action", "ToggleStatusIsnew"),
        new SqlParameter("@Id", id),
        new SqlParameter("@IsNew", isnew)
                };


                DataSet ds = await _iSql.ExecuteProcedure(
                    "SP_Manage_Announcement",
                    param.ToArray()
                );

                if (ds != null &&
                    ds.Tables.Count > 0 &&
                    ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    res.status = Convert.ToBoolean(row["Status"]);
                    res.message = row["Message"].ToString();
                }
                else
                {
                    res.status = false;
                    res.message = "No response from database";
                }
            }
            catch (Exception ex)
            {
                res.status = false;
                res.message = ex.Message;
            }

            return res;
        }
        #endregion
        #region Home page Related
        public async Task<HomePageVM> GetHomePageComponent()
        {
            HomePageVM model = new();

            try
            {
                DataSet ds = await _iSql.ExecuteProcedure("sp_GetHomePageData");

                // ---------- Slider ----------
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        model.Sliders.Add(new HomePageSliderVM
                        {
                            Title = row.Field<string?>("Title"),
                            DisplayOrder = row.Field<int?>("DisplayOrder"),
                            ImageBase64 = row.Field<string?>("ImageBase64")
                        });
                    }
                }

                // ---------- Announcement ----------
                if (ds.Tables.Count > 1)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        model.Announcements.Add(new AnnouncementVM
                        {
                            Title = row.Field<string?>("Title"),
                            DisplayOrder = row.Field<int?>("DisplayOrder"),
                            FileBase64 = row.Field<string?>("ImageBase64"),
                            IsNew = row.Field<bool?>("IsNew") ?? false
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching homepage data", ex);
            }

            return model;
        }
        #endregion
    }
}
