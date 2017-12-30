using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DashboardCode.Routines.AspNetCore
{
    public class MvcNavigationFacade<TEntity, TF, TMM, TfID> where TEntity : class where TF : class
    {
        public  readonly List<TMM> Selected;
        public  readonly string viewDataMultiSelectListKey;
        private readonly string multiSelectListOptionValuePropertyName;
        private readonly string multiSelectListOptionTextPropertyName;
        private readonly List<TfID> Ids;
        private readonly Func<TF, TfID> getId;
        private readonly Func<TEntity, TF, TMM> construct;
        private readonly Func<string, TfID> toId;

        public MvcNavigationFacade(
            string viewDataMultiSelectListKey,
            Func<TF, TfID> getId,
            string multiSelectListOptionValuePropertyName,
            string multiSelectListOptionTextPropertyName,
            Func<TEntity, TF, TMM> construct,
            Func<string, TfID> toId=null
            )
        {
            this.viewDataMultiSelectListKey = viewDataMultiSelectListKey;
            this.getId = getId;
            this.multiSelectListOptionValuePropertyName = multiSelectListOptionValuePropertyName;
            this.multiSelectListOptionTextPropertyName = multiSelectListOptionTextPropertyName;
            this.construct = construct;
            this.toId = toId?? Converters.GetParser<TfID>();
            Ids = new List<TfID>();
            Selected = new List<TMM>();
        }

        public void Parse(
            Controller controller,
            TEntity tp,
            IReadOnlyCollection<TF> options, 
            string formField)
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

        public void SetViewDataMultiSelectList(Controller controller, IReadOnlyCollection<TF> options)
        {
            controller.ViewData[viewDataMultiSelectListKey] 
                = new MultiSelectList(options, multiSelectListOptionValuePropertyName, multiSelectListOptionTextPropertyName, Ids);
        }

        public void SetViewDataMultiSelectList(Controller controller, IReadOnlyCollection<TF> options, IEnumerable<TfID> ids)
        {
            controller.ViewData[viewDataMultiSelectListKey] 
                = new MultiSelectList(options, multiSelectListOptionValuePropertyName, multiSelectListOptionTextPropertyName, ids);
        }
    }
}