using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace DashboardCode.Routines.Configuration.NETFramework
{
    public class RoutineElement :  ConfigurationElement, ICollectionMemberElement, IRoutineResolvable //, IRoutineResolvableRecord
    {
        private static readonly ConfigurationProperty NamespaceProperty =
            new ConfigurationProperty("namespace", typeof(string), "", ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty TypeProperty =
            new ConfigurationProperty("type", typeof(string), "", ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty MemberProperty =
            new ConfigurationProperty("member", typeof(string), "", ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty ForProperty =
            new ConfigurationProperty("for", typeof(string), "", ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty ConfigElementCollectionProperty =
            new ConfigurationProperty("", typeof(ResolvableElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

        private static readonly ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection
                                     {
                                         NamespaceProperty,
                                         TypeProperty,
                                         MemberProperty,
                                         ForProperty,
                                         ConfigElementCollectionProperty
                                     };
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

        public void Validate()
        {
            if (!Namespace.IsNullOrWhiteSpaceOrAsterix())
                if (!char.IsLetter(Namespace[0]))
                    throw new InvalidOperationException($"Routine's element Namespace property '{Namespace}' should be valid .NET namespace name ");

            if (!Type.IsNullOrWhiteSpaceOrAsterix())
                if (Type.Contains(".") || !char.IsLetter(Type[0]))
                    throw new InvalidOperationException($"Routine's element Type property '{Type}' should be valid .NET class name");

            if (!Member.IsNullOrWhiteSpaceOrAsterix())
                if (Member.Contains(".") || !char.IsLetter(Member[0]))
                    throw new InvalidOperationException($"Routine's element Member property '{Member}' should be valid .NET member name");

            if (!Member.IsNullOrWhiteSpaceOrAsterix())
                if (Type.IsNullOrWhiteSpaceOrAsterix())
                    throw new InvalidOperationException($"Member '{Member}' can't be configured without Type");
        }
        public string Key
        {
            get
            {
                string @namespace = StringExtensions.ReplaceEmptyWithAsterix(Namespace);
                string type = StringExtensions.ReplaceEmptyWithAsterix(Type);
                string member = StringExtensions.ReplaceEmptyWithAsterix(Member);
                string @for = StringExtensions.ReplaceEmptyWithAsterix(For);
                var key = @namespace + "."+type + "." + member + "." + @for;
                return key;
            }
        }

        //public RoutineResolvableRecord GetRoutineResolvableRecord() =>
        //    new RoutineResolvableRecord() { Namespace = this.Namespace, Type = this.Type, Member = this.Member, For=this.For, Resolvables= ((IEnumerable)base[""]).Cast<IResolvable>().ToArray() };

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
        [ConfigurationProperty("member")]
        public string Member
        {
            get
            {
                return this["member"] as string;
            }
            set
            {
                this["member"] = value;
            }
        }
        [ConfigurationProperty("for")]
        public string For
        {
            get
            {
                return this["for"] as string;
            }
            set
            {
                this["for"] = value;
            }
        }
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public ResolvableElementCollection Resolvables
        {
            get
            {
                return (ResolvableElementCollection)base[""];
            }
        }

        IEnumerable<IResolvable> IRoutineResolvable.Resolvables
        {
            get
            {
                return ((IEnumerable)base[""]).Cast<IResolvable>();
            }
        }

        #region Debug
        public static readonly DateTime StaticCreatedAt = DateTime.Now;
        public readonly DateTime CreatedAt = DateTime.Now;
        public override string ToString()
        {
            var txt = Key+" "+ CreatedAt;
            foreach (var r in Resolvables)
            {
                txt += "  "+ r.ToString();
            }
            return txt;
        }
        #endregion
    }
}
