﻿using System;
using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;

namespace DashboardCode.AdminkaV1.Injected
{
    public class Resolver : IContainer
    {
        ConfigurationContainer configurationContainer;
        public Resolver(ConfigurationContainer configurationContainer)
        {
            this.configurationContainer = configurationContainer;
        }
        public T Resolve<T>() where T : new()
        {
            var t = new T();
            var serialized = configurationContainer.ResolveSerialized<T>();
            if (t is IProgress<string>)
            {
                ((IProgress<string>)t).Report(serialized);
            }
            else
            {
                if (serialized != null)
                {
                    t = InjectedManager.DeserializeJson<T>(serialized);
                }
            }
            return t;
        }
    }
}
