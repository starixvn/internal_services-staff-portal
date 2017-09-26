// TicketDesk - Attribution notice
// Contributor(s):
//
//      Stephen Redd (https://github.com/stephenredd)
//      Wootz (https://github.com/Wootz)
//
// This file is distributed under the terms of the Microsoft Public
// License (Ms-PL). See http://opensource.org/licenses/MS-PL
// for the complete terms of use.
//
// For any distribution that contains code from this file, this notice of
// attribution must remain intact, and a copy of the license must be
// provided to the recipient.

using System.Web.Optimization;

namespace TicketDesk.Web.Client
{
    // TODO: This may not be OWIN compliant in OWIN hosts other than Microsoft.Owin.Host.SystemWeb
    public partial class Startup
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/content/styles").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/site.css",
                    "~/Content/font-awesome.css"));

            bundles.Add(new StyleBundle("~/content/wizard").Include("~/Content/wizard.css"));

            bundles.Add(new StyleBundle("~/content/select2")
                    .Include("~/Content/css/select2.css", new CssRewriteUrlTransform())
                    .Include("~/Content/css/select2-bootstrap.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/content/editor")
                    .Include("~/Scripts/dropzone/dropzone.css", new CssRewriteUrlTransform())
                    .Include("~/Scripts/dropzone/basic.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/content/summernote").Include(
                    "~/Scripts/summernote/summernote.css"));

            bundles.Add(new StyleBundle("~/content/error-pages").Include(
                    "~/Content/font-awesome.css",
                    "~/Content/ticketdesk/error.css"));

            bundles.Add(new StyleBundle("~/content/file-manager").Include(
                    "~/Scripts/filemanager/styles/reset.css",
                    "~/Scripts/filemanager/scripts/jquery.contextmenu/dist/jquery.contextMenu.min.css",
                    "~/Scripts/filemanager/scripts/alertify.js/dist/css/alertify.css")
                    .Include("~/Scripts/filemanager/styles/filemanager.css", new CssRewriteUrlTransform())
                    .Include("~/Scripts/filemanager/scripts/custom-scrollbar-plugin/jquery.mCustomScrollbar.min.css", new CssRewriteUrlTransform()));

            ////
            ////Script bundle
            ////
            bundles.Add(new ScriptBundle("~/bundles/user-login")
                .Include("~/Scripts/ticketdesk/User/login.js"));

            bundles.Add(new ScriptBundle("~/bundles/file-manager")
                .Include(
                    "~/Scripts/filemanager/scripts/jquery-ui/jquery-ui.min.js",
                    "~/Scripts/filemanager/scripts/jquery-browser.js",
                    "~/Scripts/filemanager/scripts/knockout-3.4.0.js",
                    "~/Scripts/filemanager/scripts/jquery-mousewheel/jquery.mousewheel.min.js",
                    "~/Scripts/filemanager/scripts/jquery.splitter/dist/jquery-splitter.js",
                    "~/Scripts/filemanager/scripts/jquery.contextmenu/dist/jquery.contextMenu.min.js",
                    "~/Scripts/filemanager/scripts/alertify.js/dist/js/alertify.js",
                    "~/Scripts/filemanager/scripts/clipboard.js/dist/clipboard.min.js",
                    "~/Scripts/filemanager/scripts/jquery.fileDownload/jquery.fileDownload.min.js",
                    "~/Scripts/filemanager/scripts/javascript-templates/js/tmpl.min.js",
                    "~/Scripts/filemanager/scripts/toast/lib/toast.min.js",
                    "~/Scripts/filemanager/scripts/purl/purl.min.js",
                    "~/Scripts/filemanager/scripts/filemanager.min.js",
                    "~/Scripts/filemanager/config/filemanager.init.js"));

            bundles.Add(new ScriptBundle("~/bundles/editticket")
                .Include("~/Scripts/ticketdesk/edit-ticket.js"));

            bundles.Add(new ScriptBundle("~/bundles/application-settings").Include(
                    "~/Scripts/ticketdesk/application-settings.js"));

            bundles.Add(new ScriptBundle("~/bundles/ticketcenter").Include(
                    "~/Scripts/ticketdesk/ticketcenter.js",
                    "~/Scripts/jquery.clickable-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin-users").Include(
                    "~/Scripts/ticketdesk/admin-users.js",
                    "~/Scripts/jquery.clickable-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/project-settings").Include(
                   "~/Scripts/jquery.clickable-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/search").Include(
                   "~/Scripts/ticketdesk/search.js",
                   "~/Scripts/jquery.clickable-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/admin-edit-user").Include(
                "~/Scripts/ticketdesk/admin-edit-user.js"));

            bundles.Add(new ScriptBundle("~/bundles/editor").Include(
                    "~/Scripts/dropzone/dropzone.js",
                    "~/Scripts/ticketdesk/ticket-file-uploader.js",
                    "~/Scripts/ticketdesk/ticket-details.js",
                    "~/Scripts/ticketdesk/ticket-tags.js"));

            bundles.Add(new ScriptBundle("~/bundles/select2").Include(
                    "~/Scripts/select2.js",
                    "~/Scripts/jquery.ui.sortable/jquery-ui-custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/summernote").Include(
                "~/Scripts/summernote/summernote.js"));

            bundles.Add(new ScriptBundle("~/bundles/markdown").Include(
                    "~/Scripts/pagedown/Markdown.Converter.js",
                    "~/Scripts/pagedown/Markdown.Sanitizer.js",
                    "~/Scripts/pagedown/Markdown.Editor.js"));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                    "~/Scripts/jquery-{version}.js",
                    "~/Scripts/jquery.unobtrusive-ajax.js",
#if (DEBUG)
                    "~/Scripts/jquery.validate.js",
                    "~/Scripts/jquery.validate.unobtrusive.js",
#else
                    "~/Scripts/jquery.validate.min.js",
                    "~/Scripts/jquery.validate.unobtrusive.min.js",
#endif
                    "~/Scripts/cldr.js",
                    "~/Scripts/cldr/event.js",
                    "~/Scripts/cldr/supplemental.js",
                    "~/Scripts/jquery.globalize/globalize.js",
                    "~/Scripts/bootstrap.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            // Localization bundles en-US (empty bundles to avoid 404)
            bundles.Add(new ScriptBundle("~/bundles/select2_locale_en-US"));
            bundles.Add(new ScriptBundle("~/bundles/common_locale_en-US").Include(
                    "~/Scripts/jquery.globalize/cultures/globalize.culture.en-US.js",
                    "~/Scripts/ticketdesk/locale_en-US.js"));
            bundles.Add(new ScriptBundle("~/bundles/summernote_locale_en-US"));

            // Localization bundles pt-BR
            bundles.Add(new ScriptBundle("~/bundles/select2_locale_pt-BR").Include(
                    "~/Scripts/Select2-locales/select2_locale_pt-BR.js"));
            bundles.Add(new ScriptBundle("~/bundles/common_locale_pt-BR").Include(
                    "~/Scripts/jQueryValidate-locales/jquery.validate_locale_pt-BR.js",
                    "~/Scripts/jquery.globalize/cultures/globalize.culture.pt-BR.js",
                    "~/Scripts/ticketdesk/locale_pt-BR.js"));
            bundles.Add(new ScriptBundle("~/bundles/summernote_locale_pt-BR").Include(
                "~/Scripts/summernote/summernote-pt-BR.js"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
#if (DEBUG)
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}