using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models
{
    [HalModel]
    [HalEntryPoint]
    [HalSelfActionLink(EntryPointController.Rels.Get, typeof(EntryPointController))]
    [HalActionLink(BranchController.Rels.List, typeof(BranchController))]
    public class EntryPoint
    {
    }
}
