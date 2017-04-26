//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Reflection;

//namespace JsonNet.Test
//{
//    //public class RoutinesContract : JsonContract
//    //{
        
//    //}

//    public class RoutinesContractResolver : DefaultContractResolver
//    {
//        public RoutinesContractResolver()
//        {

//        }

//        override 
//        public new static readonly ConverterContractResolver Instance = new ConverterContractResolver();

//        protected override JsonContract CreateContract(Type objectType)
//        {
//            JsonContract contract = base.CreateContract(objectType);
//            contract.
//            // this will only be called once and then cached
//            if (objectType == typeof(DateTime) || objectType == typeof(DateTimeOffset))
//            {
//                contract.Converter = new JavaScriptDateTimeConverter();
//            }
    
//            return contract;
//        }

//        protected override JsonDynamicContract CreateDynamicContract(Type objectType)
//        {
//            return base.CreateDynamicContract(objectType);
//        }
//        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
//        {
//            if (objectType == typeof(CultureInfo))
//                return new List<MemberInfo>();
//            else
//                return base.GetSerializableMembers(objectType);  //new List<MemberInfo>(objectType.GetProperties().ToList());
//        }

//        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
//        {
//            //memberSerialization.
//            var prop = base.CreateProperty(member, memberSerialization);
            
//            //if (!prop.Writable)
//            //{
//            //    var property = member as PropertyInfo;
//            //    if (property != null)
//            //    {
//            //        var hasPrivateSetter = property.GetSetMethod(true) != null;
//            //        prop.Writable = hasPrivateSetter;
//            //    }
//            //}

//            return prop;
//        }

//        //protected override JsonProperty CreateProperty(
//        //    MemberInfo member,
//        //    MemberSerialization memberSerialization)
//        //{
//        //    var property = base.CreateProperty(member, memberSerialization);
//        //    if (property.PropertyName == "Programs")
//        //    {
//        //        property.ShouldSerialize = i => false;
//        //    }
//        //    return property;
//        //}
//    }
//}
