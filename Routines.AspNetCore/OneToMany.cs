using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public class OneToMany<TP, TF, TfID> : IOneToMany<TP> where TP : class where TF : class 
    {
        private readonly Action<Action<string, object>, IReadOnlyCollection<TF>, TfID> addViewData;
        private readonly Func<IRepository<TP>, IReadOnlyCollection<TF>> getOptions;
        private readonly string formField;
        private readonly Func<TP, TfID> getId;

        private readonly Func<string, TfID> toId;

        public OneToMany(
            string formFieldName,
            Action<Action<string, object>, IReadOnlyCollection<TF>, TfID> addViewData,
            //MvcOneToManyNavigationFacade<TP, TF, TfID> navigation,
            Func<IRepository<TP>, IReadOnlyCollection<TF>> getOptions,
            Func<TP, TfID> getId,
            
            Func<string, TfID> toId = null
            )
        {
            this.formField = formFieldName;
            this.addViewData = addViewData;
            //this.navigation = navigation;
            this.getOptions = getOptions;
            this.getId = getId;
            this.toId = toId;
        }

        public void PrepareDefaultOptions(Action<string, object> addViewData, IRepository<TP> repository)
        {
            var options = getOptions(repository);
            this.addViewData(addViewData, options, default(TfID));
        }


        public void PreparePersistedOptions(Action<string, object> addViewData, IRepository<TP> repository, out Action<TP> addViewDataMultiSelectList)
        {
            var options = getOptions(repository);
            addViewDataMultiSelectList = (entity) =>
                this.addViewData(addViewData, options, getId(entity));
        }

        public void PrepareParsedOptions(Action<string, object> addViewData, HttpRequest request, TP entity, IRepository<TP> repository, out Action addViewDataMultiSelectList)
        {
            var options = getOptions(repository);

            var stringValues = request.Form[formField];
            var textValue = stringValues.ToString();
            var id = toId(textValue);
            addViewDataMultiSelectList = () => this.addViewData(addViewData, options, id);
        }
    }
}