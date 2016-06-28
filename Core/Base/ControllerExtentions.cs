// --------------------------------------------------------------------------
// <copyright file="ControllerExtentions.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.4.5">Created</change>
// </history>

namespace Fanex.ExternalAccount.Core.Base
{
    using System.Web.Mvc;
    using Newtonsoft.Json;

    /// <summary>
    /// Extension for controller.
    /// </summary>
    public static class ControllerExtentions
    {
        public static JsonBase JsonExt(this Controller controller, object data, JsonSerializerSettings settings = null)
        {
            return new JsonBase(data, settings);
        }
    }
}