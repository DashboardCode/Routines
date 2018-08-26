using System;

namespace DashboardCode.Routines.Configuration.Classic
{
    public class DeserializerClassic : IGWithConstructorFactory<string>
    {
        readonly Func<string, Type, object> deserializator;
        public DeserializerClassic(Func<string, Type, object> deserializator)
        {
            this.deserializator = deserializator;
        }

        public TOutput Create<TOutput>(string input) where TOutput : new()
        {
            if (input==null)
               return new TOutput();
            else
            {
                var o = deserializator(input, typeof(TOutput)); //JsonConvert.DeserializeObject(input, typeof(TOutput));
                return (TOutput)o;
            }
        }
    }
}