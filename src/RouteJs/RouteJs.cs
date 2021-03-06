﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace RouteJs
{
	/// <summary>
	/// Main class for RouteJS. Handles retrieving the list of routes.
	/// </summary>
	public class RouteJs
	{
		private readonly RouteCollection _routeCollection;
		private readonly IEnumerable<IRouteFilter> _routeFilters;
		private readonly IEnumerable<IDefaultsProcessor> _defaultsProcessors;

		/// <summary>
		/// Initializes a new instance of the <see cref="RouteJs" /> class.
		/// </summary>
		/// <param name="routeCollection">The route collection.</param>
		/// <param name="routeFilters">Any filters to apply to the routes</param>
		/// <param name="defaultsProcessors">Handler to handle processing of default values</param>
		public RouteJs(RouteCollection routeCollection, IEnumerable<IRouteFilter> routeFilters, IEnumerable<IDefaultsProcessor> defaultsProcessors)
		{
			_routeCollection = routeCollection;
			_routeFilters = routeFilters;
			_defaultsProcessors = defaultsProcessors;
		}

		/// <summary>
		/// Gets all the JavaScript-visible routes.
		/// </summary>
		/// <returns>A list of JavaScript-visible routes</returns>
		public IEnumerable<RouteInfo> GetRoutes()
		{
			var routes = _routeCollection.GetRoutes();

			foreach (var routeBase in routes.Where(AllowRoute))
			{
				if (routeBase is Route)
				{
					yield return GetRoute((Route)routeBase);
				}
			}
		}

		/// <summary>
		/// Check whether this route should be exposed in the JavaScript
		/// </summary>
		/// <param name="route">Route to check</param>
		/// <returns><c>true</c> if the route should be exposed</returns>
		private bool AllowRoute(RouteBase route)
		{
			return _routeFilters.All(filter => filter.AllowRoute(route));
		}

		/// <summary>
		/// Gets information about the specified route
		/// </summary>
		/// <param name="route">The route</param>
		/// <returns>Route information</returns>
		private RouteInfo GetRoute(Route route)
		{			
			var routeInfo = new RouteInfo
			{
				Url = route.Url,
				Constraints = route.Constraints ?? new RouteValueDictionary()
			};

			foreach (var processor in _defaultsProcessors)
			{
				processor.ProcessDefaults(route, routeInfo);
			}

			return routeInfo;
		}
	}
}
