﻿using Easy.Data;
using Easy.Web.CMS;
using Easy.Web.CMS.WidgetTemplate;
using Easy.Web.CMS.Zone;
using Easy.Web.Attribute;
using Easy.Web.Controller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Easy.CMS.Common.ViewModels;
using Easy.Extend;
using Easy.Constant;
using Easy.Web.CMS.Widget;
using EasyZip;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;

namespace Easy.CMS.Common.Controllers
{
    [AdminTheme, Authorize]
    public class WidgetController : Controller
    {
        private readonly IWidgetService _widgetService;
        private readonly IWidgetTemplateService _widgetTemplateService;


        public WidgetController(IWidgetService widgetService, IWidgetTemplateService widgetTemplateService)
        {
            _widgetService = widgetService;
            _widgetTemplateService = widgetTemplateService;
        }

        [ViewDataZones]
        public ActionResult Create(QueryContext context)
        {
            var template = _widgetTemplateService.Get(context.WidgetTemplateID);
            var widget = template.CreateWidgetInstance();
            widget.PageID = context.PageID;
            widget.LayoutID = context.LayoutID;
            widget.ZoneID = context.ZoneID;
            widget.FormView = template.FormView;
            if (widget.PageID.IsNotNullAndWhiteSpace())
            {
                widget.Position = _widgetService.GetAllByPageId(context.PageID).Count(m => m.ZoneID == context.ZoneID) + 1;
            }
            else
            {
                widget.Position = _widgetService.GetByLayoutId(context.LayoutID).Count(m => m.ZoneID == context.ZoneID) + 1;
            }
            ViewBag.WidgetTemplateName = template.Title;
            ViewBag.ReturnUrl = context.ReturnUrl;
            if (template.FormView.IsNotNullAndWhiteSpace())
            {
                return View(template.FormView, widget);
            }
            return View(widget);
        }
        [HttpPost, ViewDataZones]
        [ValidateInput(false)]
        public ActionResult Create(WidgetBase widget, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(widget);
            }
            widget.CreateServiceInstance().AddWidget(widget);
            if (widget.ActionType == ActionType.Continue)
            {
                return RedirectToAction("Edit", new { widget.ID, ReturnUrl });
            }
            else if (!ReturnUrl.IsNullOrEmpty())
            {
                return Redirect(ReturnUrl);
            }
            else if (!widget.PageID.IsNullOrEmpty())
            {
                return RedirectToAction("Design", "Page", new { module = "admin", ID = widget.PageID });
            }
            else
            {
                return RedirectToAction("LayoutWidget", "Layout", new { module = "admin" });
            }
        }
        [ViewDataZones]
        public ActionResult Edit(string ID, string ReturnUrl)
        {
            var widgetBase = _widgetService.Get(ID);
            var widget = widgetBase.CreateServiceInstance().GetWidget(widgetBase);
            ViewBag.ReturnUrl = ReturnUrl;

            var template = _widgetTemplateService.Get(
                m =>
                    m.PartialView == widget.PartialView && m.AssemblyName == widget.AssemblyName &&
                    m.ServiceTypeName == widget.ServiceTypeName && m.ViewModelTypeName == widget.ViewModelTypeName).FirstOrDefault();
            if (template != null)
            {
                ViewBag.WidgetTemplateName = template.Title;
            }
            if (widget.FormView.IsNotNullAndWhiteSpace())
            {
                return View(widget.FormView, widget);
            }
            return View(widget);
        }

        [HttpPost, ViewDataZones]
        [ValidateInput(false)]
        public ActionResult Edit(WidgetBase widget, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(widget);
            }
            widget.CreateServiceInstance().UpdateWidget(widget);
            if (!ReturnUrl.IsNullOrEmpty())
            {
                return Redirect(ReturnUrl);
            }
            if (!widget.PageID.IsNullOrEmpty())
            {
                return RedirectToAction("Design", "Page", new { module = "admin", ID = widget.PageID });
            }
            else
            {
                return RedirectToAction("LayoutWidget", "Layout", new { module = "admin" });
            }
        }

        [HttpPost]
        public JsonResult SaveWidgetZone(List<WidgetBase> widgets)
        {
            foreach (var widget in widgets)
            {
                _widgetService.Update(widget, new Data.DataFilter(new List<string> { "ZoneID", "Position" }).Where("ID", OperatorType.Equal, widget.ID));
            }
            return Json(true);
        }
        [HttpPost]
        public JsonResult DeleteWidget(string ID)
        {
            WidgetBase widget = _widgetService.Get(ID);
            if (widget != null)
            {
                widget.CreateServiceInstance().DeleteWidget(ID);
                return Json(ID);
            }
            return Json(false);
        }

        public PartialViewResult Templates()
        {
            return PartialView(_widgetService.Get(m => m.IsTemplate == true));
        }

        [HttpPost]
        public PartialViewResult AppendWidget(WidgetBase widget)
        {
            var widgetPart = _widgetService.ApplyTemplate(widget, HttpContext);
            if (widgetPart == null)
            {
                widgetPart = new HtmlWidget { PartialView = "Widget.HTML", HTML = "<label class='text-danger'>模板已被删除，添加失败！</label>" }.ToWidgetPart();
            }
            return PartialView("DesignWidget", new DesignWidgetViewModel(widgetPart, widget.PageID));
        }
        [HttpPost]
        public JsonResult CancelTemplate(string Id)
        {
            var widget = _widgetService.Get(Id);
            if (!widget.IsSystem)
            {
                widget.IsTemplate = false;
                if (widget.PageID.IsNotNullAndWhiteSpace() || widget.LayoutID.IsNotNullAndWhiteSpace())
                {
                    _widgetService.Update(widget);
                }
                else
                {
                    widget.CreateServiceInstance().DeleteWidget(Id);
                }
            }
            return Json(Id);
        }

        public FileResult Pack(string ID)
        {
            var widget = _widgetService.Get(ID);
            var file = _widgetService.PackWidget(ID);
            return File(file.ToMemoryStream(), "Application/zip", widget.WidgetName + ".zip");
        }
        [HttpPost]
        public ActionResult InstallWidgetTemplate(string returnUrl)
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    _widgetService.InstallPackWidget(Request.Files[0].InputStream);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            return Redirect(returnUrl);
        }
    }
}
