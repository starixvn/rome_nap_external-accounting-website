// --------------------------------------------------------------------------
// <copyright file="CreateFunctionModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.13">Created</change>
// </history>

namespace Fanex.ExternalAccount.ViewModel.ManageFunction
{
    public class CreateFunctionModel
    {
        public string Name { get; set; }

        public string Key { get; set; }

        public string Description { get; set; }

        public int DisplayOrder { get; set; }

        public int ParentId { get; set; }

        public string ParentName { get; set; }
    }
}