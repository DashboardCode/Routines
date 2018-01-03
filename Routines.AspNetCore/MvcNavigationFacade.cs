using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DashboardCode.Routines.AspNetCore
{
    public class MvcOneToManyNavigationFacade<TEntity, TF, TfID> where TEntity : class where TF : class
    {
        private TfID Id;

        public  readonly string viewDataSelectListKey;
        private readonly string selectListOptionValuePropertyName;
        private readonly string selectListOptionTextPropertyName;
        private readonly Func<TF, TfID> getId;
        private readonly Func<string, TfID> toId;

        public MvcOneToManyNavigationFacade(
            string viewDataSelectListKey,
            Func<TF, TfID> getId,
            string selectListOptionValuePropertyName,
            string selectListOptionTextPropertyName,
            Func<string, TfID> toId = null
            )
        {
            this.viewDataSelectListKey = viewDataSelectListKey;
            this.getId = getId;
            this.selectListOptionValuePropertyName = selectListOptionValuePropertyName;
            this.selectListOptionTextPropertyName = selectListOptionTextPropertyName;
            this.toId = toId ?? Converters.GetParser<TfID>();
            Id = default(TfID);
        }

        public void AddViewData(Action<string, object> addViewData, IReadOnlyCollection<TF> options)
        {
            addViewData(viewDataSelectListKey, new SelectList(options, selectListOptionValuePropertyName, selectListOptionTextPropertyName, Id));
        }

        public void AddViewData(Action<string, object> addViewData, IReadOnlyCollection<TF> options, TfID id)
        {
            addViewData(viewDataSelectListKey, new SelectList(options, selectListOptionValuePropertyName, selectListOptionTextPropertyName, id));
        }

        public void Parse(
            HttpRequest request,
            TEntity tp,
            IReadOnlyCollection<TF> options,
            string formField)
        {
            var stringValues = request.Form[formField];
            var textValue = stringValues.ToString();
            Id = toId(textValue);
        }
    }

    public class MvcViewDataMultiSelectListFacade<TF, TfID>
    {
        public  readonly string viewDataMultiSelectListKey;
        private readonly string multiSelectListOptionValuePropertyName;
        private readonly string multiSelectListOptionTextPropertyName;

        public MvcViewDataMultiSelectListFacade(
            string viewDataMultiSelectListKey,
            string multiSelectListOptionValuePropertyName,
            string multiSelectListOptionTextPropertyName
            )
        {
            this.viewDataMultiSelectListKey = viewDataMultiSelectListKey;
            this.multiSelectListOptionValuePropertyName = multiSelectListOptionValuePropertyName;
            this.multiSelectListOptionTextPropertyName  = multiSelectListOptionTextPropertyName;
        }

        public void AddViewData(Action<string, object> addViewData, IReadOnlyCollection<TF> options, IEnumerable<TfID> selectedIds)
        {
            addViewData(viewDataMultiSelectListKey, new MultiSelectList(options, multiSelectListOptionValuePropertyName, multiSelectListOptionTextPropertyName, selectedIds));
        }
    }
}