using System;
using System.Web;
using System.Web.Mvc;
using System.Security.Permissions;
using System.Text;
using System.Collections.Generic;
using System.Web.WebPages;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using MegaZord.Library.EF;
namespace MegaZord.Library.MVC
{
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class GridViewExtensions
    {
        #region
        //public static HelperResult MZGridView<T>(
        //    this HtmlHelper html,
        //    MZGridViewData<T> data,
        //    Action<MZGridViewData<T>> headerTemplate,
        //    Action<T, string> itemTemplate,
        //    string cssClass,
        //    string cssAlternatingClass,
        //    Action<T> editItemTemplate,
        //    Action<MZGridViewData<T>> footerTemplate,
        //     Expression<Func<T, object>> expression
        //    )
        //{
        //    headerTemplate(data);

        //    int i = 0;
        //    foreach (var item in data.PagedList)
        //    {
        //        if (!item.Equals(data.EditItem))
        //        {
        //            itemTemplate(item, (i % 2 == 0 ? cssClass : cssAlternatingClass));
        //        }
        //        else
        //        {
        //            editItemTemplate(item);
        //        }

        //        i++;
        //    }

        //    footerTemplate(data);
        //    return new HelperResult(writer =>
        //    {
        //        MemberExpression body = (MemberExpression)expression.Body;


        //        writer.Write(body.Member.Name);
        //    });
        //}
        #endregion

        private static string GetHTMLValue<T>(T item, MZGridViewColumn col) where T : MZEntity
        {

            var fieldName = col.MemberExp.Member.Name;

            var value = item.GetType().GetProperty(fieldName).GetValue(item, null);

            return value != null ? value.ToString() : string.Empty;
        }
        private static string GetIdValue<T>(T item, Expression<Func<T, long>> KeyColumn) where T : MZEntity
        {
            var fieldName = ((MemberExpression)KeyColumn.Body).Member.Name;

            var value = item.GetType().GetProperty(fieldName).GetValue(item, null);

            return value != null ? value.ToString() : "0";
        }
        public static void MZGridView<T>(
            this HtmlHelper html,
            MZGridViewData<T> data,
            Expression<Func<T, long>> KeyColumn,
            MZGridViewSettings settings) where T : MZEntity
        {
            const string divBase = "<div class=\"{0}\">{1}</div>";
            const string divBaseAction = "<div class=\"{0}\" onClick=\"{1}('{2}'); return false;\">{3}</div>";

            const string emptyString = "&nbsp;";

            var visibleColumns = settings.Columns.Where(x => x.Visible).ToList();


            var colHeader = new StringBuilder();
            //header dos botoes
            colHeader.AppendFormat(divBase, settings.ActionColClass, emptyString);

            //header das colunas
            foreach (var col in visibleColumns)
            {
                colHeader.AppendFormat(divBase, col.CssClass, col.Header);
            }

            //linha do header
            var header = new StringBuilder();
            header.AppendFormat(divBase, settings.HeaderRowClass, colHeader.ToString());


            var rows = new StringBuilder();
            foreach (var item in data.PagedList)
            {
                var colsRow = new StringBuilder();
                var pkValue = GetIdValue<T>(item, KeyColumn);

                var btnDelete = string.Format(divBaseAction, settings.ActionIconClass, settings.DeleteFunctionName, pkValue, settings.DeleteIcon);
                var btnEdit = string.Format(divBaseAction, settings.ActionIconClass, settings.EditFunctionName, pkValue, settings.EditIcon);
                colsRow.AppendFormat(divBase, settings.ActionColClass, string.Format("{0}{1}", btnEdit, btnDelete));

                foreach (var col in visibleColumns)
                {
                    colsRow.AppendFormat(divBase, col.CssClass, GetHTMLValue<T>(item, col));
                }
                rows.AppendFormat(divBase, settings.RowClass, colsRow.ToString());


            }
            //grid completa
            var grid = new StringBuilder();
            grid.AppendLine(header.ToString());
            grid.AppendLine(string.Format(divBase, settings.BodyClass, rows.ToString()));

            html.ViewContext.Writer.Write(grid);

        }

    }
}
