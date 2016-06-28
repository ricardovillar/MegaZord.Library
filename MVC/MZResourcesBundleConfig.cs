using System;
using System.Web.Mvc;
using MegaZord.Library.Common;
using MegaZord.Library.MVC.Filters;
using System.Web.Optimization;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using MegaZord.Library.Helpers;
using MegaZord.Library.MVC;

namespace MegaZord.Library.MVC
{

    public class MZBundleFileOrder : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }


    }
    public class MZResourcesBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles, IList<string> jsFiles, IList<string> cssFiles)
        {
            BundleTable.EnableOptimizations = true;


            #region Busca CSS

            var style = new StyleBundle("~/EstilosInfra");
            style.Include(cssFiles.ToArray());
            style.Orderer = new MZBundleFileOrder();
            #endregion

            #region Busca JS
            var script = new ScriptBundle("~/ScriptsInfra");
            script.Include(jsFiles.ToArray());
            script.Orderer = new MZBundleFileOrder();
            #endregion

            if (!MZHelperConfiguration.MZEnableMinify)
            {
                style.Transforms.Clear();
                script.Transforms.Clear();
            }
            bundles.Add(style);
            bundles.Add(script);


        }


    }
}