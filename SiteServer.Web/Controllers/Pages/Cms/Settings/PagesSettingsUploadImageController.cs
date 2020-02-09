﻿using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    [RoutePrefix("pages/cms/settings/settingsUploadImage")]
    public partial class PagesSettingsUploadImageController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<ObjectResult<Site>> GetConfig([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigUpload))
            {
                return Request.Unauthorized<ObjectResult<Site>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            return new ObjectResult<Site>
            {
                Value = site
            };
        }

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigUpload))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);

            site.ImageUploadDirectoryName = request.ImageUploadDirectoryName;
            site.ImageUploadDateFormatString = request.ImageUploadDateFormatString;
            site.IsImageUploadChangeFileName = request.IsImageUploadChangeFileName;
            site.ImageUploadTypeCollection = request.ImageUploadTypeCollection.Replace("|", ",");
            site.ImageUploadTypeMaxSize = request.ImageUploadTypeMaxSize;
            site.PhotoSmallWidth = request.PhotoSmallWidth;
            site.PhotoMiddleWidth = request.PhotoMiddleWidth;

            await DataProvider.SiteRepository.UpdateAsync(site);

            await auth.AddSiteLogAsync(request.SiteId, "修改图片上传设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}