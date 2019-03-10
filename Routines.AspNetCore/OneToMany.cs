using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public class OneToMany<TP, TF, TfID, TDAL> : IOneToMany<TP, TDAL> where TP : class where TF : class 
    {
        private readonly Action<Action<string, object>, IReadOnlyCollection<TF>, TfID> addViewData;
        private readonly Func<TDAL, IReadOnlyCollection<TF>> getOptions;
        private readonly string formFieldName;
        private readonly Func<TP, TfID> getTpTfId;

        private readonly Func<string, TfID> parseId;

        public OneToMany(
            string formFieldName,
            Action<Action<string, object>, IReadOnlyCollection<TF>, TfID> addViewData,
            //MvcOneToManyNavigationFacade<TP, TF, TfID> navigation,
            Func<TDAL, IReadOnlyCollection<TF>> getOptions,
            Func<TP, TfID> getTpTfId,
            
            Func<string, TfID> toId = null
            )
        {
            this.formFieldName = formFieldName;
            this.addViewData = addViewData;
            //this.navigation = navigation;
            this.getOptions = getOptions;
            this.getTpTfId = getTpTfId;
            this.parseId = parseId ?? Converters.GetParser<TfID>();
        }

        public void PrepareDefaultOptions(Action<string, object> addViewData, TDAL repository)
        {
            var options = getOptions(repository);
            this.addViewData(addViewData, options, default(TfID));
        }


        public void PreparePersistedOptions(Action<string, object> addViewData, TDAL repository, out Action<TP> addViewDataMultiSelectList)
        {
            var options = getOptions(repository);
            addViewDataMultiSelectList = (entity) =>
                this.addViewData(addViewData, options, getTpTfId(entity));
        }

        public void PrepareParsedOptions(Action<string, object> addViewData, HttpRequest request, TP entity, TDAL repository, out Action addViewDataMultiSelectList)
        {
            var options = getOptions(repository);

            var stringValues = request.Form[formFieldName];
            var textValue = stringValues.ToString();
            var id = parseId(textValue);
            addViewDataMultiSelectList = () => this.addViewData(addViewData, options, id);
        }
    }
}