using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.AspNetCore
{
    public class ManyToMany<TP, TF, TMM, TfID> : IManyToMany<TP> where TP : class where TF : class where TMM : class
    {
        private readonly MvcNavigationFacade2<TP, TF, TMM, TfID> navigation;
        private readonly Func<IRepository<TP>, IReadOnlyCollection<TF>> getOptions;
        private readonly Expression<Func<TP, ICollection<TMM>>> getRelatedExpression;
        private readonly Func<TP, ICollection<TMM>> getRelated;
        private readonly Func<TMM, TMM, bool> equalsById;
        private readonly Func<TMM, TfID> getTmmId;
        public ManyToMany(
            MvcNavigationFacade2<TP, TF, TMM, TfID> navigation,
            Func<IRepository<TP>, IReadOnlyCollection<TF>> getOptions,
            //Func<TP, ICollection<TMM>> getRelated,
            Expression<Func<TP, ICollection<TMM>>> getRelatedExpression, 
            Func<TMM, TMM, bool> equalsById,
            Func<TMM, TfID> getTmmId
            )
        {
            this.navigation = navigation;
            this.getOptions = getOptions;
            this.getRelatedExpression = getRelatedExpression;
            getRelated = getRelatedExpression.Compile();
            this.equalsById = equalsById;
            this.getTmmId = getTmmId;
        }

        public void SetViewDataMultiSelectList(RoutineController controller, IRepository<TP> repository)
        {
            var options = getOptions(repository);
            SetViewDataMultiSelectList(controller, options);
        }

        public void PrepareOptions(RoutineController controller, IRepository<TP> repository, out Action<TP> setViewDataMultiSelectLists)
        {
            var options = getOptions(repository);
            setViewDataMultiSelectLists = (entity) =>
                navigation.SetViewDataMultiSelectList(controller, getRelated(entity).Select(getTmmId), options);
        }

        void SetViewDataMultiSelectList(RoutineController controller, IReadOnlyCollection<TF> options)
        {
            navigation.SetViewDataMultiSelectList(controller, options);
        }

        public void ParseRequest(RoutineController controller, TP entity, IRepository<TP> repository, out Action<IBatch<TP>> modifyRelated, out Action setViewDataMultiSelectList)
        {
            var options = getOptions(repository);
            navigation.Parse(controller, entity, options);
            modifyRelated = (batch) => batch.ModifyRelated(entity, getRelatedExpression, navigation.Selected, equalsById);
            setViewDataMultiSelectList = () => navigation.SetViewDataMultiSelectList(controller, options);
        }
    }
}