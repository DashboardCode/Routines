using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DashboardCode.Routines.AspNetCore
{
    public class MvcNavigationFacade<TP, TF, TMM, TfID> where TP : class where TF : class
    {
        public readonly Controller controller;
        public readonly List<TMM> Selected;
        private readonly string name;
        private readonly IReadOnlyCollection<TF> Options;
        private readonly string optionId;
        private readonly string optionName;
        private readonly List<TfID> Ids;
        private readonly Func<TF, TfID> getId;

        public MvcNavigationFacade(Controller controller,
            string name,
            Expression<Func<TF, TfID>> getId,
            string optionName,
            IReadOnlyCollection<TF> options
            )
        {
            this.controller = controller;
            this.name = name;
            this.optionId = MemberExpressionExtensions.GetMemberName(getId);
            this.optionName = optionName;
            this.getId = getId.Compile();
            Options = options;
            Ids = new List<TfID>();
            Selected = new List<TMM>();
        }

        public void Parse(
            Func<TF, TMM> construct,
            Func<string, TfID> toId)
        {
            var stringValues = controller.Request.Form[name];
            if (stringValues.Count() > 0)
            {
                foreach (var s in stringValues)
                    Ids.Add(toId(s));
                Options.Where(e => Ids.Any(e2 => EqualityComparer<TfID>.Default.Equals(e2, getId(e))))
                    .ToList()
                    .ForEach(e => Selected.Add(construct(e)));
            }
        }

        public void Reset()
        {
            controller.ViewData[name + "MultiSelectList"] = new MultiSelectList(Options, optionId, optionName);
            controller.ViewData[name] = Ids;
        }

        public void Reset(IEnumerable<TfID> ids)
        {
            controller.ViewData[name + "MultiSelectList"] = new MultiSelectList(Options, optionId, optionName);
            controller.ViewData[name] = ids;
        }
    }

    public class MvcNavigationFacade2<TP, TF, TMM, TfID> where TP : class where TF : class
    {
        public readonly List<TMM> Selected;
        private readonly string name;
        private readonly string optionId;
        private readonly string optionName;
        private readonly List<TfID> Ids;
        private readonly Func<TF, TfID> getId;
        private readonly Func<TP, TF, TMM> construct;
        private readonly Func<string, TfID> toId;

        public MvcNavigationFacade2(
            string name,
            Expression<Func<TF, TfID>> getId,
            string optionName,
            Func<TP, TF, TMM> construct,
            Func<string, TfID> toId
            )
        {
            //this.controller = controller;
            this.name = name;
            this.optionId = MemberExpressionExtensions.GetMemberName(getId);
            this.optionName = optionName;
            this.getId = getId.Compile();
            //Options = options;
            Ids = new List<TfID>();
            Selected = new List<TMM>();
            this.construct = construct;
            this.toId = toId;
        }

        public void Parse(
            Controller controller,
            TP tp,
            IReadOnlyCollection<TF> options)
        {
            var stringValues = controller.Request.Form[name];
            if (stringValues.Count() > 0)
            {
                foreach (var s in stringValues)
                    Ids.Add(toId(s));
                options.Where(e => Ids.Any(e2 => EqualityComparer<TfID>.Default.Equals(e2, getId(e))))
                    .ToList()
                    .ForEach(e => Selected.Add(construct(tp, e)));
            }
        }

        public void SetViewDataMultiSelectList(Controller controller, IReadOnlyCollection<TF> options)
        {
            controller.ViewData[name + "MultiSelectList"] = new MultiSelectList(options, optionId, optionName);
            controller.ViewData[name] = Ids;
        }

        public void SetViewDataMultiSelectList(Controller controller, IEnumerable<TfID> ids, IReadOnlyCollection<TF> options)
        {
            controller.ViewData[name + "MultiSelectList"] = new MultiSelectList(options, optionId, optionName);
            controller.ViewData[name] = ids;
        }
    }
}