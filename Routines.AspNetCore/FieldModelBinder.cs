// usuccessful attempt to undertand the MVC Model Binding. 
// conclusion: for codegenerated controller it is better to parse Form directly
// https://github.com/aspnet/Mvc/issues/7133#issuecomment-352012482

//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//using Microsoft.AspNetCore.Mvc.ModelBinding;

//namespace DashboardCode.Routines.AspNetCore
//{
//    public class FieldsModelBinder<T> : IModelBinder
//    {
//        private readonly Dictionary<string, Action<T, string>> propertyBinders;
//        private readonly Func<T> constructor;

//        public FieldsModelBinder(Func<T> constructor, Dictionary<string, Action<T, string>> propertyBinders)
//        {
//            this.constructor = constructor;
//            this.propertyBinders = propertyBinders;
//        }

//        public Task BindModelAsync(ModelBindingContext modelBindingContext)
//        {
//            T t = constructor();
//            foreach (var pair in propertyBinders)
//            {
//                var propertyName = pair.Key;
//                var valueProvider = modelBindingContext.ValueProvider;
//                var propertyValueProviderResult = valueProvider.GetValue(propertyName);
//                if (propertyValueProviderResult == ValueProviderResult.None)
//                {
//                    modelBindingContext.Result = ModelBindingResult.Failed();
//                    return Task.CompletedTask;
//                }
//                else
//                {
//                    var value = propertyValueProviderResult.FirstValue;
//                    var action = pair.Value;
//                    action(t, value);
//                }
//            }
//            modelBindingContext.Result = ModelBindingResult.Success(t);
//            return Task.CompletedTask;
//        }
//    }

//}



//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
//using System;
//using System.Collections.Generic;

// https://github.com/aspnet/Mvc/issues/7133
//namespace DashboardCode.Routines.AspNetCore
//{
//    public class ControllerModelBinder<T> : ComplexTypeModelBinder
//    {
//        public ControllerModelBinder(
//            IModelMetadataProvider metadataProvider,
//            ModelBinderFactory modelBinderFactory
//            ) : base(Collect(metadataProvider, modelBinderFactory))
//        {
//        }


//        private static Dictionary<ModelMetadata, IModelBinder> Collect(
//            IModelMetadataProvider metadataProvider, ModelBinderFactory modelBinderFactory)
//        {
//            var propertyBinders = new Dictionary<ModelMetadata, IModelBinder>();

//            var modelMetadataCollection = metadataProvider.GetMetadataForProperties(typeof(T));
//            foreach (var modelMetadata in modelMetadataCollection)
//            {
//                for (var i = 0; i < modelMetadata.Properties.Count; i++)
//                {
//                    var propertyModelMetadata = modelMetadata.Properties[i];
//                    //property
//                    //var b = modelMetadata.
//                    ModelBinderProviderContext modelBinderProviderContext = new ModelBinderProviderContext();
//                    var modelBinder = modelBinderProviderContext.CreateBinder(propertyModelMetadata);
//                    propertyBinders.Add(propertyModelMetadata, modelBinder);
//                }
//            }
//            return propertyBinders;
//        }

//        public virtual IModelBinder CreateBinder(ModelMetadata metadata, ModelBinderFactory modelBinderFactory)
//        {
//            if (metadata == null)
//                throw new ArgumentNullException("metadata");
//            ModelMetadata modelMetadata = metadata;
//            return this._factory.CreateBinderCoreCached(new ModelBinderFactory.DefaultModelBinderProviderContext(this, metadata), (object)modelMetadata);
//        }

//        public virtual IModelBinder CreateBinder(ModelMetadata metadata, ModelBinderFactory modelBinderFactory)
//        {
//            if (metadata == null)
//                throw new ArgumentNullException("metadata");
//            ModelMetadata modelMetadata = metadata;
//            return modelBinderFactory.CreateBinderCoreCached(new ModelBinderFactory.DefaultModelBinderProviderContext(this, metadata), (object)modelMetadata);
//        }

//        protected override bool CanBindProperty(ModelBindingContext bindingContext, ModelMetadata propertyMetadata)
//        {
//            return base.CanBindProperty(bindingContext, propertyMetadata);
//        }
//    }
//}
