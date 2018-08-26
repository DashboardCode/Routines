using Microsoft.Extensions.Configuration;

namespace DashboardCode.Routines.Configuration.Standard
{
    public class DeserializerStandard : IGWithConstructorFactory<IConfigurationSection>
    {
        public TOutput Create<TOutput>(IConfigurationSection input) where TOutput : new()
        {
            var output = new TOutput();
            ConfigurationBinder.Bind(input, output);
            return output;
        }
    }
}