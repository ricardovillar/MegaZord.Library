using MegaZord.Library.Common;
using MegaZord.Library.EF;
using MegaZord.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace MegaZord.Library.MVC
{
    public enum EnumMZGridViewColumnType
    {
        [Description("Coluna do tipo texto")]
        Text = 0,
        [Description("Coluna do tipo numerico")]
        Long = 1,
        [Description("Coluna do tipo data")]
        Date = 2,
        [Description("Coluna do tipo checkboxk")]
        CheckBox = 4,
        [Description("Coluna do tipo radio")]
        Radio = 8,
        [Description("Coluna do tipo select")]
        Select = 16,
        [Description("Coluna do tipo email")]
        Email = 32,
        [Description("Coluna do tipo decimal")]
        Decimal = 64
    }

    public class MZGridViewColumn
    {
        protected MZGridViewColumn(string pHeader, EnumMZGridViewColumnType pColumType,
            bool pVisible, string pCssClass, MemberExpression pMemberExp)
        {
            this.Header = pHeader;
            this.ColumnType = pColumType;
            this.Visible = pVisible;
            this.CssClass = pCssClass;
            this.MemberExp = pMemberExp;
        }
        public string Header { get; private set; }
        public bool Visible { get; private set; }
        public string CssClass { get; private set; }
        public EnumMZGridViewColumnType ColumnType { get; private set; }

        public MemberExpression MemberExp { get; private set; }

    }


    public class MZGridViewColumnText<T> : MZGridViewColumn where T : MZEntity
    {
        public MZGridViewColumnText(string pHeader, Expression<Func<T, string>> expression, bool visible, string pCssClass)
            : base(pHeader, EnumMZGridViewColumnType.Text, visible, pCssClass, (MemberExpression)expression.Body)
        {
            this.Field = expression;

        }

        public Expression<Func<T, string>> Field { get; private set; }
    }

    public class MZGridViewColumnLong<T> : MZGridViewColumn where T : MZEntity
    {
        public MZGridViewColumnLong(string pHeader, Expression<Func<T, long>> expression, bool visible, string pCssClass)
            : base(pHeader, EnumMZGridViewColumnType.Long, visible, pCssClass, (MemberExpression)expression.Body)
        {
            this.Field = expression;
        }

        public Expression<Func<T, long>> Field { get; private set; }

    }
    public class MZGridViewSettings
    {
        public MZGridViewSettings()
        {
            this.Columns = new List<MZGridViewColumn>();
        }
        public string BodyClass { get; set; }
        public string RowClass { get; set; }
        public string HeaderRowClass { get; set; }
        public string ActionColClass { get; set; }
        public string ActionIconClass { get; set; }
        public string DeleteIcon { get; set; }
        public string DeleteFunctionName { get; set; }
        public string EditIcon { get; set; }
        public string EditFunctionName { get; set; }
        public IList<MZGridViewColumn> Columns { get; private set; }

    }
}
