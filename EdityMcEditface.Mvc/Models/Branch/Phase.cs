using EdityMcEditface.Mvc.Controllers;
using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Models.Branch
{
    [HalModel]
    [HalActionLink(PhaseController.Rels.SetPhase, typeof(PhaseController))]
    public class Phase
    {
        public String Name { get; set; }

        public bool Current { get; set; }
    }
}
