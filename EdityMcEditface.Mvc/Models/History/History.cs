﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using Halcyon.HAL.Attributes;
using Threax.AspNetCore.Halcyon.Ext;
using EdityMcEditface.Mvc.Controllers;

namespace EdityMcEditface.Mvc.Models
{
    [HalModel]
    [HalActionLink(EntryPointController.Rels.Get, typeof(EntryPointController))]
    public class History
    {
        public History()
        {

        }

        public History(Commit commit)
        {
            Message = commit.Message;
            Name = commit.Author.Name;
            Email = commit.Author.Email;
            When = commit.Author.When;
            Sha = commit.Sha;
        }

        public string Message { get; internal set; }
        public string Sha { get; internal set; }
        public string Name { get; internal set; }
        public string Email { get; internal set; }
        public DateTimeOffset When { get; internal set; }
    }
}
