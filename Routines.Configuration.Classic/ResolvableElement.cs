using System;
using System.Configuration;
using System.Diagnostics;


namespace DashboardCode.Routines.Configuration.Classic
{
    [DebuggerDisplay("{Namespace}.{Type}; {InstanceCreatedAt}/{StaticCreatedAt}")]
    [DebuggerTypeProxy(typeof(ResolvableElementDebugView))]
    public class ResolvableElement : ConfigurationElement, ICollectionMemberElement, IResolvableConfigurationRecord<string> //, IResolvableRecord
    {
        private static readonly ConfigurationProperty namespaceProperty =
            new ConfigurationProperty("namespace", typeof(string), "", ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty typeProperty =
            new ConfigurationProperty("type", typeof(string), "", ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationProperty valueProperty =
            new ConfigurationProperty("value", typeof(string), "", ConfigurationPropertyOptions.IsRequired);

        private static readonly ConfigurationPropertyCollection properties = 
            new ConfigurationPropertyCollection { namespaceProperty, typeProperty, valueProperty };

        public override bool IsReadOnly()
        {
            return false;
        }

        #region Overrides
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return properties;
            }
        }
        protected override void PostDeserialize()
        {
            base.PostDeserialize();
            Validate();
        }
        #endregion

        #region Properties
        [ConfigurationProperty("namespace")]
        public string Namespace
        {
            get
            {
                return this["namespace"] as string;
            }
            set
            {
                this["namespace"] = value;
            }
        }
        [ConfigurationProperty("type")]
        public string Type
        {
            get
            {
                return this["type"] as string;
            }
            set
            {
                this["type"] = value;
            }
        }
        [ConfigurationProperty("value")]
        public string Value
        {
            get
            {
                return this["value"] as string;
            }
            set
            {
                this["value"] = value;
            }
        }
        #endregion

        //public ResolvableRecord GetResolvableRecord() =>
        //    new ResolvableRecord { Namespace = this.Namespace, Type = this.Type, Value = this.Value };

        public void Validate()
        {
            if (Type.Contains(".") || !Type[0].IsLetterOrUnderscore())
                throw new InvalidOperationException("Config's element Type property should be valid .NET type name");
        }
        public string Key { get; } = Guid.NewGuid().ToString();

        

        #region Debug
        public static readonly DateTime StaticCreatedAt = DateTime.Now;
        public readonly DateTime InstanceCreatedAt = DateTime.Now;

        public override string ToString()
        {
            return $"{Namespace}.{Type}; {InstanceCreatedAt}/{StaticCreatedAt}";
        }
        #endregion

        [DebuggerNonUserCode]
        class ResolvableElementDebugView
        {
            private readonly ResolvableElement resolvableElement;

            public string Namespace { get { return resolvableElement.Namespace; } }
            public string Type      { get { return resolvableElement.Type; } }
            public string Value     { get { return resolvableElement.Value; } }

            public ResolvableElementDebugView(ResolvableElement resolvableElement) =>
                this.resolvableElement = resolvableElement;
        }
    }
}