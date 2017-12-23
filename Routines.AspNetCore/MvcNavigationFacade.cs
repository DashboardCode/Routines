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
        public readonly List<TMM> Selected;
        //private readonly string name;
        private readonly string optionId;
        private readonly string optionName;
        private readonly List<TfID> Ids;
        private readonly Func<TF, TfID> getId;
        private readonly Func<TP, TF, TMM> construct;
        private readonly Func<string, TfID> toId;

        public MvcNavigationFacade(
            //string name,
            Expression<Func<TF, TfID>> getId,
            string optionName,
            Func<TP, TF, TMM> construct,
            Func<string, TfID> toId=null
            )
        {
            //this.controller = controller;
            //this.name = name;
            this.optionId = MemberExpressionExtensions.GetMemberName(getId);
            this.optionName = optionName;
            this.getId = getId.Compile();
            //Options = options;
            Ids = new List<TfID>();
            Selected = new List<TMM>();
            this.construct = construct;
            if (toId == null)
                toId = Converters.GetParser<TfID>();
            this.toId = toId;
        }

        public void Parse(
            Controller controller,
            TP tp,
            IReadOnlyCollection<TF> options, string formField)
        {
            var stringValues = controller.Request.Form[formField];
            if (stringValues.Count() > 0)
            {
                foreach (var s in stringValues)
                    Ids.Add(toId(s));
                options.Where(e => Ids.Any(e2 => EqualityComparer<TfID>.Default.Equals(e2, getId(e))))
                    .ToList()
                    .ForEach(e => Selected.Add(construct(tp, e)));
            }
        }

        public void SetViewDataMultiSelectList(Controller controller, IReadOnlyCollection<TF> options, string formField)
        {
            controller.ViewData[formField + "MultiSelectList"] = new MultiSelectList(options, optionId, optionName);
            controller.ViewData[formField] = Ids;
        }

        public void SetViewDataMultiSelectList(Controller controller, IEnumerable<TfID> ids, IReadOnlyCollection<TF> options, string formField)
        {
            controller.ViewData[formField + "MultiSelectList"] = new MultiSelectList(options, optionId, optionName);
            controller.ViewData[formField] = ids;
        }
    }
}