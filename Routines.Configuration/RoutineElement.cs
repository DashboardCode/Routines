using System;
using System.Configuration;

namespace Vse.Routines.Configuration
{
    public class RoutineElement : ConfigurationElement, ICollectionMemberElement
    {
        private static readonly ConfigurationProperty NamespaceProperty =
            new ConfigurationProperty("namespace", typeof(string), "", ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty ClassProperty =
            new ConfigurationProperty("class", typeof(string), "", ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty MemberProperty =
            new ConfigurationProperty("member", typeof(string), "", ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty ForProperty =
            new ConfigurationProperty("for", typeof(string), "", ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty ConfigElementCollectionProperty =
            new ConfigurationProperty("", typeof(ResolvableElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

        private static readonly ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection
                                     {
                                         NamespaceProperty,
                                         ClassProperty,
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
                    throw new InvalidOperationException($"Routine's element Namespace property ({Namespace}) should be valid .NET namespace name ");

            if (!Class.IsNullOrWhiteSpaceOrAsterix())
                if (Class.Contains(".") || !char.IsLetter(Class[0]))
                    throw new InvalidOperationException($"Routine's element Class property ({Class}) should be valid .NET class name");

            if (!Member.IsNullOrWhiteSpaceOrAsterix())
                if (Member.Contains(".") || !char.IsLetter(Member[0]))
                    throw new InvalidOperationException($"Routine's element Member property ({Member}) should be valid .NET member name");

            if (!Member.IsNullOrWhiteSpaceOrAsterix())
                if (Class.IsNullOrWhiteSpaceOrAsterix())
                    throw new InvalidOperationException($"Member '{Member}' can't be configured without Class");
        }
        public string Key
        {
            get
            {
                string @namespace = StringExtensions.ReplaceEmptyWithAsterix(Namespace);
                string @class = StringExtensions.ReplaceEmptyWithAsterix(Class);
                string member = StringExtensions.ReplaceEmptyWithAsterix(Member);
                string @for = StringExtensions.ReplaceEmptyWithAsterix(For);
                var key = @namespace + "."+@class + "." + member + "." + @for;
                return key;
            }
        }
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
        [ConfigurationProperty("class")]
        public string Class
        {
            get
            {
                return this["class"] as string;
            }
            set
            {
                this["class"] = value;
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
