using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.Routines.Configuration
{
    public class ConfigurationContainer<TSerialized> : IContainer
    {
        private readonly List<IResolvableConfigurationRecord<TSerialized>> elements = new List<IResolvableConfigurationRecord<TSerialized>>();
        IGWithConstructorFactory<TSerialized> deserializer;
        public ConfigurationContainer(
            IEnumerable<IRoutineConfigurationRecord<TSerialized>> routineConfigurationRecords,
            IGWithConstructorFactory<TSerialized> deserializer, 
            MemberTag memberTag)
        {
            this.deserializer = deserializer;
            var memberRoutineConfigurationRecords = routineConfigurationRecords.LimitRoutineConfigurationRecords(memberTag);
            foreach (var routineConfigurationRecord in memberRoutineConfigurationRecords)
            {
                if (routineConfigurationRecord.For.IsNullOrWhiteSpaceOrAsterix())
                {
                    foreach (var resolvableConfigurationRecord in routineConfigurationRecord.Resolvables)
                    {
                        if (!elements.Any(e => e.Type == resolvableConfigurationRecord.Type && e.Namespace == resolvableConfigurationRecord.Namespace))
                            elements.Add(resolvableConfigurationRecord);
                    }
                }
            }
        }

        public ConfigurationContainer(IEnumerable<IRoutineConfigurationRecord<TSerialized>> routineConfigurationRecords,
            IGWithConstructorFactory<TSerialized> deserializer, 
            MemberTag memberTag, string @for)
        {
            this.deserializer = deserializer;
            var memberRoutineConfigurationRecords = routineConfigurationRecords.LimitRoutineConfigurationRecords(memberTag);
            foreach (var routineConfigurationRecord in memberRoutineConfigurationRecords)
            {
                if (routineConfigurationRecord.For.IsNullOrWhiteSpaceOrAsterix() || routineConfigurationRecord.For == @for)
                {
                    foreach (var resolvableConfigurationRecord in routineConfigurationRecord.Resolvables)
                    {
                        if (!elements.Any(e => e.Type == resolvableConfigurationRecord.Type && e.Namespace == resolvableConfigurationRecord.Namespace))
                            elements.Add(resolvableConfigurationRecord);
                    }
                }
            }
        }

        public T Resolve<T>() where T : new()
        {
            var serialized = this.ResolveSerialized<T>();
            if (serialized == null)
                return new T();
            return deserializer.Create<T>(serialized); 
        }


        public TSerialized ResolveSerialized<T>()
        {
            var type = typeof(T);
            var typeName = type.Name;
            var typeNamespace = type.Namespace;
            var @value = ResolveString(typeNamespace, typeName);
            return @value;
        }

        public TSerialized ResolveString(string typeNamespace, string typeName)
        {
            var @value = default(TSerialized);
            var config = elements.FirstOrDefault(e => (e.Namespace == typeNamespace || string.IsNullOrEmpty(e.Namespace)) && e.Type == typeName);
            if (config != null)
                @value = config.Value;
            return @value;
        }
    }
}