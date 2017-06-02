using EdityMcEditface.Mvc.Models.Branch;
using EdityMcEditface.Mvc.Models.Page;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace EdityMcEditface.Mvc.Controllers
{
    [Route("edity/[controller]")]
    [ResponseCache(NoStore = true)]
    [ProducesHal]
    [TypeFilter(typeof(HalModelResultFilterAttribute))]
    [Authorize]
    public class PhaseController : Controller
    {
        /// <summary>
        /// Each controller should define a Rels class inside of it. Note that each rel across the system
        /// should be unique, so name them with an appropriate prefix. Right now these rel names are used
        /// by the codegen to make functions, which is why they need to be unique (technically only unique
        /// across any endpoints that have the same rels, but globally is a good enough approximiation of this,
        /// I hope to fix this in the future.
        /// </summary>
        public static class Rels
        {
            public const String List = "ListPhases";
            public const String SetPhase = "SetPhase";
        }

        ProjectFinder projectFinder;
        IPhaseDetector phaseDetector;

        public PhaseController(ProjectFinder projectFinder, IPhaseDetector phaseDetector)
        {
            this.projectFinder = projectFinder;
            this.phaseDetector = phaseDetector;
        }

        /// <summary>
        /// Get the list of phases. Not paged.
        /// </summary>
        /// <returns>The list of phases.</returns>
        [HttpGet]
        [HalRel(Rels.List)]
        public PhaseCollection Get()
        {
            return new PhaseCollection(PhaseEnumerable());
        }

        private IEnumerable<Phase> PhaseEnumerable()
        {
            yield return new Phase()
            {
                Name = Phases.Edit.ToString(),
                Current = this.phaseDetector.Phase == Phases.Edit
            };

            yield return new Phase()
            {
                Name = Phases.Draft.ToString(),
                Current = this.phaseDetector.Phase == Phases.Draft
            };
        }

        /// <summary>
        /// Set the phase that is currently active.
        /// </summary>
        /// <param name="name">The name of the phase to make active</param>
        /// <returns></returns>
        [HttpPut("{Name}")]
        [HalRel(Rels.SetPhase)]
        public void Put(Phases name)
        {
            phaseDetector.Phase = name;
        }
    }
}
