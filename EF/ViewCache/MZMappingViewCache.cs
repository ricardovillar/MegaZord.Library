﻿namespace MegaZord.Library.EF.ViewCache
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure.MappingViews;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml.Linq;

 

    /// <summary>
    /// An implementation of the <see cref="DbMappingViewCache"/> class.
    /// </summary>
    public class MZMappingViewCache : DbMappingViewCache
    {
        private readonly string _hash;
        private readonly Dictionary<string, DbMappingView> _views;

        private static string GetExtentFullName(EntitySetBase entitySet)
        {
            Debug.Assert(entitySet != null, "entitySet != null");

            return string.Format("{0}.{1}", entitySet.EntityContainer.Name, entitySet.Name);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="MZMappingViewCache"/> class.
        /// </summary>
        /// <param name="views">Xml element containing views for a container mapping.</param>
        public MZMappingViewCache(XElement views)
        {
            if (views == null)
            {
                throw new ArgumentNullException("views");
            }

            _hash = (string)views.Attribute("hash");

            if (string.IsNullOrWhiteSpace(_hash))
            {
                throw new InvalidOperationException("The hash of the mapping cannot be null or empty string.");
            }

            _views = views
                .Elements("view")
                .ToDictionary(v => (string)v.Attribute("extent"), v => new DbMappingView((string)v.Value));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MZMappingViewCache"/> class.
        /// </summary>
        /// <param name="hash">The hash of the mapping.</param>
        /// <param name="views">View definitions.</param>
        public MZMappingViewCache(string hash, Dictionary<EntitySetBase, DbMappingView> views)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                throw new ArgumentNullException("hash");
            }

            if (views == null)
            {
                throw new ArgumentNullException("views");
            }

            _hash = hash;
            _views = views.ToDictionary(kvp => GetExtentFullName(kvp.Key), kvp => kvp.Value);
        }

        /// <summary>
        /// Returns view definition for the given <paramref name="entitySet"/>.
        /// </summary>
        /// <param name="entitySet"><see cref="EntitySetBase"/> to return views for.</param>
        /// <returns>
        /// View definition for the given <paramref name="entitySet"/> or null if the definition does not exist.
        /// </returns>
        public override DbMappingView GetView(EntitySetBase entitySet)
        {
            DbMappingView mappingView;
            _views.TryGetValue(GetExtentFullName(entitySet), out mappingView);

            return mappingView;
        }

        /// <summary>
        /// The hash of the mapping.
        /// </summary>
        public override string MappingHashValue
        {
            get { return _hash; }
        }
    }
}
