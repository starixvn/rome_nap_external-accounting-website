// --------------------------------------------------------------------------
// <copyright file="EditMenuModel.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.ViewModel.ManageMenu
{
    /// <summary>
    /// Edit menu model.
    /// </summary>
    public class EditMenuModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public int FunctionId { get; set; }

        public int ParentId { get; set; }

        public string ParentName { get; set; }

        public int DisplayOrder { get; set; }
    }
}